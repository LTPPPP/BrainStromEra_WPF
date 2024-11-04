using BrainStormEra_WPF.Models;
using BrainStormEra_WPF.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BrainStormEra_WPF.ViewModel.Course.Chapter
{
    public class ChapterViewModel : BaseViewModel
    {
        public ObservableCollection<Models.Chapter> chapterList { get; set; }
        public ICommand AddChapterCommand { get; }
        public ICommand DeleteChapterCommand { get; }
        public ICommand SearchChapterCommand { get; }

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
            newitem = new Models.Chapter();
            chapterList = new ObservableCollection<Models.Chapter>();
            LoadChapters();
            AddChapterCommand = new RelayCommand(AddChapter);
            DeleteChapterCommand = new RelayCommand(DeleteChapter);
            SearchChapterCommand = new RelayCommand(SearchChapter);
        }

        private void LoadChapters()
        {
            using (var context = new PrnDbFpContext())
            {
                var chapters = context.Chapters.Where(ch => ch.CourseId == CourseId).ToList();
                chapterList = new ObservableCollection<Models.Chapter>(chapters);
            }
        }

        private void AddChapter(object obj)
        {
            // Implement logic to add chapter
        }

        private void DeleteChapter(object obj)
        {
            // Implement logic to delete chapter
        }

        private void SearchChapter(object obj)
        {
            // Implement logic to search chapter
        }

        private Models.Chapter _newitem;
        public Models.Chapter newitem
        {
            get { return _newitem; }
            set
            {
                _newitem = value;
                OnPropertyChanged(nameof(newitem));
            }
        }
    }
}
