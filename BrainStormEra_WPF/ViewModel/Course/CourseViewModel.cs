using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using BrainStormEra_WPF.Common.Feedback;
using BrainStormEra_WPF.Models;
using BrainStormEra_WPF.Utilities;
using Microsoft.Win32;
using System.Windows.Controls;
using BrainStormEra_WPF.Instructor;
using BrainStormEra_WPF.ViewModel.Course.Chapter;

namespace BrainStormEra_WPF.ViewModel
{
    public class CourseViewModel : BaseViewModel
    {
        public ObservableCollection<Models.Course> courselist { get; set; }

        public ICommand Addcomand { get; }
        public ICommand Editcomand { get; }
        public ICommand Deletecomand { get; }
        public ICommand ClearCommand { get; }
        public ICommand UploadPictureCommand { get; }
        public ICommand FeedbackCommand { get; }
        public ICommand ViewChapterCommand { get; }

        private string _userId;
        private BitmapImage? _uploadedCoursePicture;
        private BitmapImage _courseImage;
        private readonly Models.Account _account;

        public CourseViewModel(string userId, Models.Course? course = null)
        {
            _userId = userId;
            courselist = new ObservableCollection<Models.Course>();

            Addcomand = new RelayCommand(Add);
            Editcomand = new RelayCommand(Edit);
            Deletecomand = new RelayCommand(Delete);
            ClearCommand = new RelayCommand(Clear);
            UploadPictureCommand = new RelayCommand(UploadPicture);
            FeedbackCommand = new RelayCommand(ViewFeedback);
            ViewChapterCommand = new RelayCommand(OpenChapterPage);

            InitializeCourse(course);
            Load();
        }

        private void Clear(object obj)
        {
            newitem = new Models.Course();
            SetCourseImage(null);
            OnPropertyChanged(nameof(newitem));
        }

        private void InitializeCourse(Models.Course? course)
        {
            if (course != null)
            {
                newitem = course;
                SetCourseImage(course.CoursePicture);
            }
            else
            {
                newitem = new Models.Course();
                SetCourseImage(null);
            }
        }

        private void Delete(object obj)
        {
            using (var context = new PrnDbFpContext())
            {
                context.Courses.Remove(selectitem);
                context.SaveChanges();

                courselist.Remove(selectitem);
                newitem = new Models.Course();

            }
        }

        private void Edit(object obj)
        {
            if (string.IsNullOrEmpty(newitem.CourseName) ||
         string.IsNullOrEmpty(newitem.CourseDescription) ||
         newitem.CourseStatus == null ||
         newitem.Price == 0)
            {
                MessageBox.Show("Please fill in all fields.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new PrnDbFpContext())
            {
                if (selectitem != null)
                {
                    var item = context.Courses.FirstOrDefault(g => g.CourseId == selectitem.CourseId);

                    if (item != null)
                    {

                        bool isDuplicateName = context.Courses.Any(c => c.CourseName == newitem.CourseName && c.CourseId != selectitem.CourseId);
                        if (isDuplicateName)
                        {
                            MessageBox.Show("Course name already exists. Please choose a different name.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }


                        item.CourseName = newitem.CourseName;
                        item.CourseDescription = newitem.CourseDescription;
                        item.CourseStatus = newitem.CourseStatus;
                        item.Price = newitem.Price;
                        item.CoursePicture = newitem.CoursePicture;

                        context.SaveChanges();


                        var index = courselist.IndexOf(selectitem);
                        courselist[index] = item;
                        courselist.Clear();
                        Load();


                        OnPropertyChanged(nameof(courselist));
                        newitem = new Models.Course();
                    }
                }
            }
        }

        private void Add(object obj)
        {
            if (string.IsNullOrEmpty(newitem.CourseName) ||
       string.IsNullOrEmpty(newitem.CourseDescription) ||
       newitem.CourseStatus == null ||
       newitem.Price == 0)
            {
                MessageBox.Show("Please fill in all fields.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new PrnDbFpContext())
            {

                bool isDuplicateName = context.Courses.Any(c => c.CourseName == newitem.CourseName);
                if (isDuplicateName)
                {
                    MessageBox.Show("Course name already exists. Please choose a different name.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                newitem.CourseId = GenerateNewCourseId();
                newitem.CreatedBy = _userId;
                newitem.CoursePicture = _uploadedCoursePicture != null ? ConvertImageToByteArray(_uploadedCoursePicture) : null;


                context.Courses.Add(newitem);
                context.SaveChanges();


                Load();


                newitem = new Models.Course();


                SetCourseImage(null);
            }
        }

        private void Load()
        {
            using (var context = new PrnDbFpContext())
            {
                var courses = context.Courses
            .Where(c => c.CreatedBy == _userId)
            .Select(c => new
            {
                Course = c,
                CreatorName = context.Accounts
                    .Where(a => a.UserId == c.CreatedBy)
                    .Select(a => a.FullName)
                    .FirstOrDefault()
            })
            .ToList();
                courselist.Clear();
                foreach (var item in courses)
                {

                    item.Course.CreatedBy = item.CreatorName;
                    courselist.Add(item.Course);
                }
            }
        }

        private void UploadPicture(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                _uploadedCoursePicture = new BitmapImage(new Uri(openFileDialog.FileName));
                newitem.CoursePicture = ConvertImageToByteArray(_uploadedCoursePicture);
                SetCourseImage(newitem.CoursePicture);
                MessageBox.Show("Image uploaded successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private byte[]? ConvertImageToByteArray(BitmapImage image)
        {
            if (image == null) return null;

            using (var ms = new MemoryStream())
            {
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(ms);
                return ms.ToArray();
            }
        }

        private Models.Course _newitem;
        public Models.Course newitem
        {
            get { return _newitem; }
            set
            {
                _newitem = value;
                OnPropertyChanged(nameof(newitem));
            }
        }

        private Models.Course _selectitem;
        public Models.Course selectitem
        {
            get { return _selectitem; }
            set
            {
                _selectitem = value;
                OnPropertyChanged(nameof(selectitem));

                if (_selectitem != null)
                {
                    newitem.CourseId = _selectitem.CourseId;
                    newitem.CourseName = _selectitem.CourseName;
                    newitem.CourseDescription = _selectitem.CourseDescription;
                    newitem.CourseStatus = _selectitem.CourseStatus;
                    newitem.CoursePicture = _selectitem.CoursePicture;
                    newitem.Price = _selectitem.Price;
                    newitem.CourseCreatedAt = _selectitem.CourseCreatedAt;
                    newitem.CreatedBy = _selectitem.CreatedBy;
                    newitem.CourseCategories = _selectitem.CourseCategories;

                    SetCourseImage(_selectitem.CoursePicture);
                    OnPropertyChanged(nameof(newitem));
                }
            }
        }


        public Dictionary<int, string> StatusOptions { get; } = new Dictionary<int, string>
{
    { 1, "Active" },
    { 2, "Inactive" }
};


        public BitmapImage CourseImage
        {
            get => _courseImage;
            set
            {
                _courseImage = value;
                OnPropertyChanged();
            }
        }





        private void SetCourseImage(byte[]? imageBytes)
        {
            if (imageBytes != null && imageBytes.Length > 0)
            {
                using var ms = new MemoryStream(imageBytes);
                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                CourseImage = image;
            }

            else
            {

                CourseImage = new BitmapImage(new Uri("pack://application:,,,/BrainStormEra_WPF;component/img/user-img/coursenull.jpg"));
            }
        }


        private string GenerateNewCourseId()
        {
            using (var context = new PrnDbFpContext())
            {

                var maxId = context.Courses
                                   .Where(c => c.CourseId.StartsWith("CO"))
                                   .OrderByDescending(c => c.CourseId)
                                   .Select(c => c.CourseId)
                                   .FirstOrDefault();


                if (maxId == null)
                {
                    return "CO001";
                }


                int numberPart = int.Parse(maxId.Substring(2));


                string newIdNumber = (numberPart + 1).ToString("D3");


                return $"CO{newIdNumber}";
            }
        }


        private void ViewFeedback(object obj)
        {
            if (selectitem != null)
            {

                using (var context = new PrnDbFpContext())
                {
                    var account = context.Accounts.FirstOrDefault(a => a.UserId == _userId);
                    if (account != null)
                    {

                        var feedbackPage = new ViewFeedback(selectitem, account);
                        HomePageInstructor.MainFrameInstance.Navigate(feedbackPage);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy tài khoản người dùng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một khóa học để xem phản hồi.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OpenChapterPage(object obj)
        {

            if (selectitem == null)
            {
                MessageBox.Show("Please select a course to view its chapters.", "No Course Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var chapterPage = new ChapterInstructor();
            var chapterViewModel = new ChapterViewModel
            {
                CourseId = selectitem.CourseId
            };
            chapterPage.DataContext = chapterViewModel;


            HomePageInstructor.MainFrameInstance.Navigate(chapterPage);
        }


    }
}
