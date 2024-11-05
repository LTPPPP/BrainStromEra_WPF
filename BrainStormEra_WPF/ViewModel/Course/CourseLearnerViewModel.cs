using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BrainStormEra_WPF.Common.Feedback;
using BrainStormEra_WPF.Models;
using BrainStormEra_WPF.Utilities;
using Microsoft.VisualBasic.ApplicationServices;

namespace BrainStormEra_WPF.ViewModel.Learner
{
    public class CourseLearnerViewModel : BaseViewModel
    {
        public ObservableCollection<Models.Course> ActiveCourseList { get; set; }
        private BitmapImage _courseImage;
        private string _userId;

        public ICommand FeedbackLearnerCommand { get; }

        public CourseLearnerViewModel(string userId)
        {
            _userId = userId;
            newitem = new Models.Course();
            ActiveCourseList = new ObservableCollection<Models.Course>();
            LoadActiveCourses();
            FeedbackLearnerCommand = new RelayCommand(ViewFeedback);
        }

        private void LoadActiveCourses()
        {
            using (var context = new PrnDbFpContext())
            {
                var activeCourses = context.Courses
                    .Where(c => c.CourseStatus == 1)
                    .Select(c => new
                    {
                        Course = c,
                        CreatorName = context.Accounts
                        .Where(a => a.UserId == c.CreatedBy)
                            .Select(a => a.FullName)
                            .FirstOrDefault()
                    })
                    .ToList();

                ActiveCourseList.Clear();
                foreach (var item in activeCourses)
                {
                    item.Course.CreatedBy = item.CreatorName;
                    ActiveCourseList.Add(item.Course);
                }
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

                    SetCourseImage(_selectitem.CoursePicture);
                    OnPropertyChanged(nameof(newitem));
                }
            }
        }

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
                        HomePageLearner.MainFrameInstance.Navigate(feedbackPage);
                    }
                    else
                    {
                        MessageBox.Show("User account not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a course to view feedback.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}
