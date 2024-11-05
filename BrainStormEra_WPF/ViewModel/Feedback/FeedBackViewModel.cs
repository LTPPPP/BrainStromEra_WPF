using BrainStormEra_WPF.Models;
using BrainStormEra_WPF.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace BrainStormEra_WPF.ViewModel.Feedback
{
    public class FeedBackViewModel : BaseViewModel
    {
        public ObservableCollection<Models.Feedback> Feedbacks { get; set; }
        private readonly PrnDbFpContext _context;
        private readonly Models.Course _course;
        private readonly Models.Account _account;

        private Models.Feedback _newFeedback;
        public Models.Feedback NewFeedback
        {
            get => _newFeedback;
            set
            {
                if (value.Rating < 1 || value.Rating > 5)
                {
                    MessageBox.Show("Rating phải nằm trong khoảng từ 1 đến 5.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                _newFeedback = value;
                OnPropertyChanged(nameof(NewFeedback));
            }
        }

        private Models.Feedback _selectedFeedback;
        public Models.Feedback SelectedFeedback
        {
            get => _selectedFeedback;
            set
            {
                _selectedFeedback = value;
                OnPropertyChanged(nameof(SelectedFeedback));
                if (SelectedFeedback != null)
                {
                    NewFeedback.Rating = _selectedFeedback.Rating;
                    NewFeedback.Comment = _selectedFeedback.Comment;
                }
            }
        }

        public FeedBackViewModel(Models.Course course, Models.Account account)
        {
            _context = new PrnDbFpContext();
            Feedbacks = new ObservableCollection<Models.Feedback>();
            NewFeedback = new Models.Feedback();

            _course = course;
            _account = account;

            LoadFeedbacks(_course.CourseId);

            CreateFeedbackCommand = new RelayCommand(AddFeedback);
            UpdateFeedbackCommand = new RelayCommand(UpdateFeedback);
            DeleteFeedbackCommand = new RelayCommand(DeleteFeedback);
            ViewYourFeedbackCommand = new RelayCommand(ViewYourFeedback);

        }

        private void ViewYourFeedback(object obj)
        {
            var userFeedbacks = _context.Feedbacks
                                .Where(f => f.CourseId == _course.CourseId && f.UserId == _account.UserId)
                                .Include(u => u.User)
                                .ToList();

            if (userFeedbacks == null || userFeedbacks.Count == 0)
            {
                MessageBox.Show("Không có phản hồi nào từ bạn trong khóa học này.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                Feedbacks.Clear();

                foreach (var feedback in userFeedbacks)
                {
                    Feedbacks.Add(feedback);
                }
            }
            OnPropertyChanged(nameof(Feedbacks));
        }

        public ICommand CreateFeedbackCommand { get; set; }
        public ICommand UpdateFeedbackCommand { get; set; }
        public ICommand DeleteFeedbackCommand { get; set; }
        public ICommand ViewYourFeedbackCommand { get; set; }


        private void LoadFeedbacks(string courseId)
        {
            var feedbacks = _context.Feedbacks
                            .Where(c => c.CourseId == courseId)
                            .Include(u => u.User)
                            .ToList();

            if (feedbacks != null)
            {
                Feedbacks.Clear();
                foreach (var feedback in feedbacks)
                {
                    Feedbacks.Add(feedback);
                }
            }
            OnPropertyChanged(nameof(Feedbacks));
        }

        private void AddFeedback(object obj)
        {
            if (NewFeedback != null && _account.UserRole != 0)
            {
                if (NewFeedback.Rating < 1 || NewFeedback.Rating > 5)
                {
                    MessageBox.Show("Rating phải nằm trong khoảng từ 1 đến 5.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;

                }

                if (NewFeedback.Rating == null)
                {
                    MessageBox.Show("Rating phải nằm trong khoảng từ 1 đến 5.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;

                }
                bool isEnrolled = _context.Enrollments
                    .Any(e => e.CourseId == _course.CourseId && e.UserId == _account.UserId);

                if (!isEnrolled && _account.UserRole != 1)
                {
                    MessageBox.Show("Bạn cần đăng ký khóa học trước khi thêm phản hồi.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                Models.Feedback feedback = new Models.Feedback
                {
                    FeedbackId = GenerateNewFeedbackId(),
                    Rating = NewFeedback.Rating,
                    Comment = NewFeedback.Comment,
                    CourseId = _course.CourseId,
                    UserId = _account.UserId,
                    CreatedAt = DateTime.Now
                };

                _context.Feedbacks.Add(feedback);
                _context.SaveChanges();
                Feedbacks.Add(feedback);
                OnPropertyChanged(nameof(Feedbacks));
                NewFeedback = new Models.Feedback();
            }
            else
            {
                MessageBox.Show("Bạn không có quyền thêm feedback.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateFeedback(object obj)
        {
            if (SelectedFeedback == null || SelectedFeedback.UserId != _account.UserId)
            {
                MessageBox.Show("Bạn chỉ có thể sửa phản hồi của mình.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (NewFeedback.Rating < 1 || NewFeedback.Rating > 5)
            {
                MessageBox.Show("Rating phải nằm trong khoảng từ 1 đến 5.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelectedFeedback.Rating = NewFeedback.Rating;
            SelectedFeedback.Comment = NewFeedback.Comment;

            _context.Feedbacks.Update(SelectedFeedback);
            _context.SaveChanges();

            LoadFeedbacks(SelectedFeedback.CourseId);
            NewFeedback = new Models.Feedback();
        }

        private void DeleteFeedback(object obj)
        {
            if (SelectedFeedback == null || SelectedFeedback.UserId != _account.UserId)
            {
                MessageBox.Show("Bạn chỉ có thể xóa phản hồi của mình.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _context.Feedbacks.Remove(SelectedFeedback);
            _context.SaveChanges();
            Feedbacks.Remove(SelectedFeedback);
            OnPropertyChanged(nameof(Feedbacks));

            SelectedFeedback = null;
            NewFeedback = new Models.Feedback();
        }


        private string GenerateNewFeedbackId()
        {
            using (var context = new PrnDbFpContext())
            {
                // Lấy FeedbackId lớn nhất bắt đầu với tiền tố "FB"
                var maxId = context.Feedbacks
                                   .Where(f => f.FeedbackId.StartsWith("FE"))
                                   .OrderByDescending(f => f.FeedbackId)
                                   .Select(f => f.FeedbackId)
                                   .FirstOrDefault();

                // Nếu chưa có FeedbackId nào, bắt đầu từ "FB001"
                if (maxId == null)
                {
                    return "FE001";
                }

                // Lấy phần số của FeedbackId hiện tại
                int numberPart = int.Parse(maxId.Substring(2));

                // Tạo phần số mới tăng thêm 1 và định dạng với 3 chữ số
                string newIdNumber = (numberPart + 1).ToString("D3");

                // Trả về FeedbackId mới với tiền tố "FB"
                return $"FE{newIdNumber}";
            }
        }

    }
}
