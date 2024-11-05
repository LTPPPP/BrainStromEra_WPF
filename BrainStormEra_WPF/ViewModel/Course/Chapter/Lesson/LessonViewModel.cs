using BrainStormEra_WPF.Models;
using BrainStormEra_WPF.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows;
using Microsoft.EntityFrameworkCore; // Thêm thư viện này để mở tệp


namespace BrainStormEra_WPF.ViewModel.Course.Chapter.Lesson
{
    internal class LessonViewModel : BaseViewModel
    {
        // Observable Collection for Lessons
        public ObservableCollection<BrainStormEra_WPF.Models.Lesson> LessonList { get; set; }

        // Observable Collection for Lesson Types
        public ObservableCollection<LessonType> LessonTypeList { get; set; }
        public string ChapterId { get; private set; }


        // Commands
        public ICommand OpenFileCommand { get; }
        public ICommand ClearCommand { get; }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand UploadDocCommand { get; }
        public ICommand UploadPdfCommand { get; }

        // New Lesson to be added/edited
        private BrainStormEra_WPF.Models.Lesson _newItem;
        public BrainStormEra_WPF.Models.Lesson NewItem
        {
            get { return _newItem; }
            set
            {
                _newItem = value;
                OnPropertyChanged(nameof(NewItem));
            }
        }

        // Selected Lesson from the list view
        private BrainStormEra_WPF.Models.Lesson _selectedItem;
        public BrainStormEra_WPF.Models.Lesson SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));

                if (_selectedItem != null)
                {
                    // Update NewItem properties to match SelectedItem
                    NewItem.LessonId = _selectedItem.LessonId;
                    NewItem.ChapterId = _selectedItem.ChapterId;
                    NewItem.LessonName = _selectedItem.LessonName;
                    NewItem.LessonDescription = _selectedItem.LessonDescription;
                    NewItem.LessonContent = _selectedItem.LessonContent;
                    NewItem.LessonOrder = _selectedItem.LessonOrder;
                    NewItem.LessonTypeId = _selectedItem.LessonTypeId;
                    NewItem.LessonStatus = _selectedItem.LessonStatus;
                    NewItem.LessonCreatedAt = _selectedItem.LessonCreatedAt;

                    // Reset the file path and file name when a new item is selected
                    LessonFilePath = string.Empty;
                    LessonFileName = string.Empty;

                    OnPropertyChanged(nameof(NewItem));
                }
            }
        }

        // File path to store the uploaded file path temporarily (not in DB)
        private string _lessonFilePath;
        public string LessonFilePath
        {
            get { return _lessonFilePath; }
            set
            {
                _lessonFilePath = value;
                OnPropertyChanged(nameof(LessonFilePath));
            }
        }



        private void OpenFile(object parameter)
        {
            if (parameter is string filePath && !string.IsNullOrEmpty(filePath))
            {
                try
                {
                    // Tạo đường dẫn đầy đủ từ đường dẫn tương đối
                    string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);

                    // Kiểm tra xem file có tồn tại không
                    if (File.Exists(fullPath))
                    {
                        // Mở file sử dụng chương trình mặc định của hệ điều hành
                        Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });
                    }
                    else
                    {
                        // Thông báo nếu file không tồn tại
                        Console.WriteLine("File không tồn tại: " + fullPath);
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý ngoại lệ nếu có lỗi xảy ra khi mở file
                    Console.WriteLine("Có lỗi xảy ra khi mở file: " + ex.Message);
                }
            }
        }

        // File name to display in the UI
        private string _lessonFileName;
        public string LessonFileName
        {
            get { return _lessonFileName; }
            set
            {
                _lessonFileName = value;
                OnPropertyChanged(nameof(LessonFileName));
            }
        }

        // Directory to save lessons in project
        private readonly string lessonDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Lessons");

        // Constructor
        public LessonViewModel(string chapterId)
        {
            NewItem = new BrainStormEra_WPF.Models.Lesson();
            LessonList = new ObservableCollection<BrainStormEra_WPF.Models.Lesson>();
            LessonTypeList = new ObservableCollection<LessonType>();

            LoadLessonTypes();
            ChapterId = chapterId;
            LessonList = new ObservableCollection<BrainStormEra_WPF.Models.Lesson>();
            LoadLessons(chapterId);

            // Command bindings
            OpenFileCommand = new RelayCommand(OpenFile);
            ClearCommand = new RelayCommand(ClearForm);

            AddCommand = new RelayCommand(Add);
            EditCommand = new RelayCommand(Edit);
            DeleteCommand = new RelayCommand(Delete);
            UploadDocCommand = new RelayCommand(UploadDoc);
            UploadPdfCommand = new RelayCommand(UploadPdf);
        }

        public void LoadLessons(string chapterId)
        {
            if (string.IsNullOrEmpty(chapterId)) return; // Nếu chapterId rỗng, thoát khỏi phương thức

            using (var context = new PrnDbFpContext())
            {
                var lessons = context.Lessons
                    .Include(l => l.LessonType)
                    .Where(l => l.ChapterId == chapterId) // Lọc theo chapterId
                    .ToList();

                LessonList = new ObservableCollection<BrainStormEra_WPF.Models.Lesson>(lessons);
                OnPropertyChanged(nameof(LessonList)); // Thông báo cập nhật dữ liệu
            }
        }

        // Load Lesson Types from the database
        public void LoadLessonTypes()
        {
            using (var context = new PrnDbFpContext())
            {
                var types = context.LessonTypes.ToList();
                LessonTypeList = new ObservableCollection<LessonType>(types);
                OnPropertyChanged(nameof(LessonTypeList));
            }
        }




        //ADD
        private void Add(object obj)
        {
            // Kiểm tra nếu các trường bắt buộc bị bỏ trống
            if (string.IsNullOrWhiteSpace(NewItem.LessonName) || string.IsNullOrWhiteSpace(NewItem.LessonDescription))
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new PrnDbFpContext())
            {
                // Kiểm tra nếu tên bài học đã tồn tại trong cùng ChapterId
                bool lessonNameExists = context.Lessons
                                               .Any(l => l.ChapterId == ChapterId && l.LessonName == NewItem.LessonName);

                if (lessonNameExists)
                {
                    MessageBox.Show("Lesson name already exists in this chapter. Please choose a different name.",
                                    "Duplicate Lesson Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return; // Dừng lại nếu tên bài học đã tồn tại
                }

                // Loại bỏ thực thể đã được theo dõi nếu có
                var trackedEntity = context.ChangeTracker.Entries<BrainStormEra_WPF.Models.Lesson>()
                                           .FirstOrDefault(e => e.Entity.LessonId == NewItem.LessonId);

                if (trackedEntity != null)
                {
                    context.Entry(trackedEntity.Entity).State = EntityState.Detached;
                }

                // Tạo một LessonId duy nhất
                var latestLessonId = context.Lessons
                                            .Where(l => l.LessonId.StartsWith("LE"))
                                            .OrderByDescending(l => l.LessonId)
                                            .Select(l => l.LessonId)
                                            .FirstOrDefault();

                int nextIdNumber = 1;
                if (!string.IsNullOrEmpty(latestLessonId) && int.TryParse(latestLessonId.Substring(2), out int currentMax))
                {
                    nextIdNumber = currentMax + 1;
                }

                // Định dạng LessonId mới, ví dụ: "LE001", "LE002"
                NewItem.LessonId = $"LE{nextIdNumber:000}";

                // Xác định thứ tự LessonOrder tiếp theo trong ChapterId hiện tại
                var lessonsInChapter = context.Lessons
                                              .Where(l => l.ChapterId == ChapterId)
                                              .ToList();

                int nextLessonOrder = lessonsInChapter.Any()
                    ? lessonsInChapter.Max(l => l.LessonOrder) + 1
                    : 1;

                // Thiết lập các thuộc tính cho NewItem
                NewItem.LessonOrder = nextLessonOrder;
                NewItem.LessonStatus = 1;  // Đặt trạng thái mặc định
                NewItem.ChapterId = ChapterId; // Sử dụng ChapterId đã truyền vào


                context.Lessons.Add(NewItem);
                context.SaveChanges();


                LessonList.Add(NewItem);


                NewItem = new BrainStormEra_WPF.Models.Lesson();
                LessonFileName = string.Empty;
            }
        }




        private void Edit(object obj)
        {

            if (string.IsNullOrWhiteSpace(NewItem.LessonName) || string.IsNullOrWhiteSpace(NewItem.LessonDescription))
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new PrnDbFpContext())
            {

                bool lessonNameExists = context.Lessons
                                               .Any(l => l.ChapterId == ChapterId
                                                      && l.LessonName == NewItem.LessonName
                                                      && l.LessonId != SelectedItem.LessonId);

                if (lessonNameExists)
                {
                    MessageBox.Show("A lesson with this name already exists in the same chapter. Please choose a different name.",
                                    "Duplicate Lesson Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                var item = context.Lessons.Find(SelectedItem.LessonId);
                if (item != null && item.ChapterId == ChapterId)
                {
                    item.LessonName = NewItem.LessonName;
                    item.ChapterId = ChapterId;
                    item.LessonDescription = NewItem.LessonDescription;
                    item.LessonContent = NewItem.LessonContent;
                    item.LessonOrder = NewItem.LessonOrder;
                    item.LessonTypeId = NewItem.LessonTypeId;
                    item.LessonStatus = NewItem.LessonStatus;
                    item.LessonCreatedAt = NewItem.LessonCreatedAt;

                    context.SaveChanges();


                    int index = LessonList.IndexOf(SelectedItem);
                    if (index >= 0)
                    {
                        LessonList[index] = item;
                    }

                    // Reset NewItem để chuẩn bị cho các thao tác tiếp theo
                    NewItem = new BrainStormEra_WPF.Models.Lesson();
                    LessonFileName = string.Empty; // Reset tên file hiển thị
                }
            }
        }


        // Delete a lesson
        private void Delete(object obj)
        {
            // Kiểm tra nếu chưa chọn bất kỳ mục nào
            if (SelectedItem == null)
            {
                MessageBox.Show("Please select a lesson to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            using (var context = new PrnDbFpContext())
            {
                context.Lessons.Remove(SelectedItem);
                context.SaveChanges();
                LessonList.Remove(SelectedItem);
            }
        }



        //ClearForm
        private void ClearForm(object parameter = null)
        {
            NewItem = new BrainStormEra_WPF.Models.Lesson(); // Reinitialize NewItem to a new instance
            LessonFilePath = string.Empty;                   // Reset the file path
            LessonFileName = string.Empty;                   // Reset the displayed file name
        }


        // Upload a DOC file
        private void UploadDoc(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Word Documents (*.doc;*.docx)|*.doc;*.docx"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                LessonFilePath = selectedFilePath;

                // Display the file name
                LessonFileName = Path.GetFileName(selectedFilePath);

                // Ensure the directory exists
                if (!Directory.Exists(lessonDirectory))
                {
                    Directory.CreateDirectory(lessonDirectory);
                }

                string fileName = Path.GetFileName(selectedFilePath);
                string destinationPath = Path.Combine(lessonDirectory, fileName);

                // Copy file to project directory
                File.Copy(selectedFilePath, destinationPath, true);

                // Save the relative path in LessonContent
                NewItem.LessonContent = Path.Combine("Resources", "Lessons", fileName);
            }
        }

        // Upload a PDF file
        private void UploadPdf(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                LessonFilePath = selectedFilePath;

                // Display the file name
                LessonFileName = Path.GetFileName(selectedFilePath);

                // Ensure the directory exists
                if (!Directory.Exists(lessonDirectory))
                {
                    Directory.CreateDirectory(lessonDirectory);
                }

                string fileName = Path.GetFileName(selectedFilePath);
                string destinationPath = Path.Combine(lessonDirectory, fileName);

                // Copy file to project directory
                File.Copy(selectedFilePath, destinationPath, true);

                // Save the relative path in LessonContent
                NewItem.LessonContent = Path.Combine("Resources", "Lessons", fileName);
            }
        }
    }
}
