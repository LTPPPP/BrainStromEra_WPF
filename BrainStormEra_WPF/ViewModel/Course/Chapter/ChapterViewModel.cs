using BrainStormEra_WPF.Instructor;
using BrainStormEra_WPF.Lesson;
using BrainStormEra_WPF.Models;
using BrainStormEra_WPF.Utilities;
using BrainStormEra_WPF.ViewModel.Course.Chapter.Lesson;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace BrainStormEra_WPF.ViewModel.Course.Chapter
{
    public class ChapterViewModel : BaseViewModel
    {
        private PrnDbFpContext _context;
        public ObservableCollection<Models.Chapter> chapterList { get; set; }
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand ViewLessonCommand { get; }
        private string _courseId;
        public string CourseId
        {
            get { return _courseId; }
            set
            {
                _courseId = value;
                LoadChapters();
                OnPropertyChanged(nameof(CourseId));
            }
        }

        public ChapterViewModel()
        {
            _context = new PrnDbFpContext();
            NewChapter = new Models.Chapter();
            chapterList = new ObservableCollection<Models.Chapter>();
            AddCommand = new RelayCommand(AddChapter);
            DeleteCommand = new RelayCommand(DeleteChapter);
            UpdateCommand = new RelayCommand(UpdateChapter);
            ClearCommand = new RelayCommand(ClearFields);
            ViewLessonCommand = new RelayCommand(OpenLessonPage);
            LoadChapters();
        }
        private void ClearFields(object obj)
        {

            NewChapter = new Models.Chapter
            {
                CourseId = CourseId
            };
            OnPropertyChanged(nameof(NewChapter));
        }
        private void LoadChapters()
        {
            if (string.IsNullOrEmpty(CourseId))
                return;

            using (var context = new PrnDbFpContext())
            {
                var chapters = context.Chapters.Where(ch => ch.CourseId == CourseId).ToList();

                chapterList.Clear();
                foreach (var chapter in chapters)
                {
                    chapterList.Add(chapter);
                }
            }
        }
        private void AddChapter(object obj)
        {
            if (string.IsNullOrEmpty(NewChapter.ChapterName) ||
                string.IsNullOrEmpty(NewChapter.ChapterDescription) ||
                NewChapter.ChapterCreatedAt == default)
            {
                MessageBox.Show("Please fill in the blank fields.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(NewChapter.CourseId))
            {
                NewChapter.CourseId = CourseId;
            }

            if (string.IsNullOrEmpty(NewChapter.CourseId))
            {
                MessageBox.Show("Course ID is not set. Please select a course.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new PrnDbFpContext())
            {

                bool chapterNameExists = context.Chapters.Any(c => c.CourseId == NewChapter.CourseId && c.ChapterName == NewChapter.ChapterName);
                if (chapterNameExists)
                {
                    MessageBox.Show("The chapter name already exists. Please choose a different name.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var maxId = context.Chapters
                    .Where(c => c.ChapterId.StartsWith("CH"))
                    .OrderByDescending(c => c.ChapterId)
                    .Select(c => c.ChapterId)
                    .FirstOrDefault();

                int newIdNumber = 1;
                if (maxId != null && int.TryParse(maxId.Substring(2), out int parsedId))
                {
                    newIdNumber = parsedId + 1;
                }
                string newChapterId = $"CH{newIdNumber:D3}";
                int maxOrder = context.Chapters
                    .Where(c => c.CourseId == NewChapter.CourseId)
                    .OrderByDescending(c => c.ChapterOrder)
                    .Select(c => c.ChapterOrder ?? 0)
                    .FirstOrDefault();

                int newChapterOrder = maxOrder + 1;
                var chapter = new Models.Chapter
                {
                    ChapterId = newChapterId,
                    CourseId = NewChapter.CourseId,
                    ChapterName = NewChapter.ChapterName,
                    ChapterDescription = NewChapter.ChapterDescription,
                    ChapterOrder = newChapterOrder,
                    ChapterCreatedAt = NewChapter.ChapterCreatedAt,
                    ChapterStatus = 1,

                };
                context.Chapters.Add(chapter);
                context.SaveChanges();
                chapterList.Add(chapter);
                NewChapter = new Models.Chapter { CourseId = CourseId };
                OnPropertyChanged(nameof(NewChapter));
            }
        }
        private void UpdateChapter(object obj)
        {

            if (SelectedChapter == null)
                return;
            if (string.IsNullOrEmpty(NewChapter.ChapterName) ||
                string.IsNullOrEmpty(NewChapter.ChapterDescription) ||
                NewChapter.ChapterCreatedAt == default)
            {
                MessageBox.Show("Please fill in the required fields: Chapter Name, Description, Status, and Created At.",
                                "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            using (var context = new PrnDbFpContext())
            {
                var chapter = context.Chapters.Find(SelectedChapter.ChapterId);
                if (chapter != null)
                {

                    bool chapterNameExists = context.Chapters
                        .Any(c => c.CourseId == chapter.CourseId &&
                                  c.ChapterName == NewChapter.ChapterName &&
                                  c.ChapterId != chapter.ChapterId);

                    if (chapterNameExists)
                    {
                        MessageBox.Show("Chapter name already exists in the selected course. Please choose a different name.",
                                        "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    chapter.ChapterName = NewChapter.ChapterName;
                    chapter.ChapterDescription = NewChapter.ChapterDescription;
                    chapter.ChapterOrder = NewChapter.ChapterOrder;
                    chapter.ChapterStatus = 1;
                    chapter.ChapterCreatedAt = NewChapter.ChapterCreatedAt;

                    context.SaveChanges();
                    var index = chapterList.IndexOf(SelectedChapter);
                    if (index >= 0)
                    {
                        chapterList[index] = chapter;
                    }
                    NewChapter = new Models.Chapter();
                    OnPropertyChanged(nameof(NewChapter));
                }
            }
        }
        private void DeleteChapter(object obj)
        {
            if (SelectedChapter == null)
                return;
            using (var context = new PrnDbFpContext())
            {
                var chapter = context.Chapters.Find(SelectedChapter.ChapterId);
                if (chapter != null)
                {
                    context.Chapters.Remove(chapter);
                    context.SaveChanges();


                    chapterList.Remove(SelectedChapter);


                    SelectedChapter = null;
                    NewChapter = new Models.Chapter();
                    OnPropertyChanged(nameof(NewChapter));
                }
            }
        }
        private Models.Chapter _newChapter;
        public Models.Chapter NewChapter
        {
            get { return _newChapter; }
            set
            {
                _newChapter = value;
                OnPropertyChanged(nameof(NewChapter));
            }
        }
        private Models.Chapter _selectedChapter;
        public Models.Chapter SelectedChapter
        {
            get { return _selectedChapter; }
            set
            {
                _selectedChapter = value;
                OnPropertyChanged(nameof(SelectedChapter));

                if (_selectedChapter != null)
                {

                    NewChapter = new Models.Chapter
                    {
                        ChapterId = _selectedChapter.ChapterId,
                        CourseId = _selectedChapter.CourseId,
                        ChapterName = _selectedChapter.ChapterName,
                        ChapterDescription = _selectedChapter.ChapterDescription,
                        ChapterOrder = _selectedChapter.ChapterOrder,
                        ChapterStatus = _selectedChapter.ChapterStatus,
                        ChapterCreatedAt = _selectedChapter.ChapterCreatedAt
                    };
                }
            }
        }


        private void OpenLessonPage(object obj)
        {
            if (SelectedChapter == null)
            {
                MessageBox.Show("Please select a chapter to view its lessons.", "No Chapter Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Tạo `LessonPage` và khởi tạo `LessonViewModel` với `ChapterId`
            var lessonPage = new ManagementLesson(SelectedChapter.ChapterId);
            var lessonViewModel = new LessonViewModel(SelectedChapter.ChapterId); // Truyền ChapterId
            lessonPage.DataContext = lessonViewModel;


            HomePageInstructor.MainFrameInstance.Navigate(lessonPage);
        }

    }
}

