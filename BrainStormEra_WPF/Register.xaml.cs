using BrainStormEra_WPF.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using IOPath = System.IO.Path;


namespace BrainStormEra_WPF
{
    public partial class Register : Window
    {
        // Biến lưu trữ đường dẫn ảnh sau khi người dùng upload ảnh
        private string UploadedPicturePath;

        public Register()
        {
            InitializeComponent();
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            HomePageGuest homePageGuest = new HomePageGuest();
            homePageGuest.Show();
            this.Close();
        }

        // Kiểm tra xem email đã được đăng ký chưa
        public bool IsEmailRegistered(string email)
        {
            using (var context = new PrnDbContext())
            {
                return context.Accounts.Any(a => a.UserEmail == email);
            }
        }

        // Kiểm tra xem số điện thoại đã được đăng ký chưa
        public bool IsPhoneNumberRegistered(string phoneNumber)
        {
            using (var context = new PrnDbContext())
            {
                return context.Accounts.Any(a => a.PhoneNumber == phoneNumber);
            }
        }

        // Đăng ký người dùng mới
        public void RegisterNewUser(string username, string email, string password, string fullName, DateTime? dateOfBirth, string gender, string phoneNumber, string address, string picturePath)
        {
            using (var context = new PrnDbContext())
            {
                string hashedPassword = HashPasswordMD5(password);

                // Tạo UserId với tiền tố ST hoặc IN dựa trên role
                string role = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                int userRole = role == "Student" ? 3 : 2;
                string userId = GenerateUserId(role, context);

                var newUser = new Account
                {
                    UserId = userId,
                    Username = username,
                    UserEmail = email,
                    Password = hashedPassword,
                    UserRole = userRole,
                    FullName = fullName,
                    DateOfBirth = dateOfBirth.HasValue ? DateOnly.FromDateTime(dateOfBirth.Value) : null,
                    Gender = gender,
                    PhoneNumber = phoneNumber,
                    UserAddress = address,
                    UserPicture = picturePath != null ? $"img/user-img/{IOPath.GetFileName(picturePath)}" : null,
                    AccountCreatedAt = DateTime.Now
                };

                context.Accounts.Add(newUser);
                context.SaveChanges();
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Xóa trạng thái lỗi trước khi kiểm tra lại
            ClearValidationErrors();

            string username = UsernameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;
            string fullName = FullNameTextBox.Text;
            DateTime? dateOfBirth = DateOfBirthPicker.SelectedDate;
            string gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string phoneNumber = PhoneNumberTextBox.Text;
            string address = AddressTextBox.Text;
            string picturePath = string.IsNullOrEmpty(UploadedPicturePath) ? null : UploadedPicturePath;  // Kiểm tra nếu không có ảnh

            // Biến lưu trữ thông báo lỗi
            bool hasError = false;

            if (string.IsNullOrEmpty(username))
            {
                SetError(UsernameTextBox, UsernameErrorTextBlock, "Username can't be empty.");
                hasError = true;
            }

            if (string.IsNullOrEmpty(email) || !IsEmailValid(email))
            {
                SetError(EmailTextBox, EmailErrorTextBlock, "Invalid Email.");
                hasError = true;
            }

            if (string.IsNullOrEmpty(phoneNumber) || !IsPhoneNumberValid(phoneNumber))
            {
                SetError(PhoneNumberTextBox, PhoneNumberErrorTextBlock, "Invalid phone number.");
                hasError = true;
            }

            if (IsEmailRegistered(email))
            {
                SetError(EmailTextBox, EmailErrorTextBlock, "Email has been existed.");
                hasError = true;
            }

            if (IsPhoneNumberRegistered(phoneNumber))
            {
                SetError(PhoneNumberTextBox, PhoneNumberErrorTextBlock, "Phone number has been existed.");
                hasError = true;
            }

            if (string.IsNullOrEmpty(password) || password.Length < 6)
            {
                SetError(PasswordBox, PasswordErrorTextBlock, "Password must be at least 6 character.");
                hasError = true;
            }

            if (string.IsNullOrEmpty(fullName))
            {
                SetError(FullNameTextBox, FullNameErrorTextBlock, "Fullname can't be empty.");
                hasError = true;
            }

            if (!dateOfBirth.HasValue)
            {
                SetError(DateOfBirthPicker, DateOfBirthErrorTextBlock, "Invalid date of birth.");
                hasError = true;
            }

            if (string.IsNullOrEmpty(gender))
            {
                SetError(GenderComboBox, GenderErrorTextBlock, "Please select gender.");
                hasError = true;
            }

            if (string.IsNullOrEmpty(address))
            {
                SetError(AddressTextBox, AddressErrorTextBlock, "Address can not be empty.");
                hasError = true;
            }

            // Nếu có lỗi, không tiếp tục đăng ký
            if (hasError)
            {
                return;
            }

            // Nếu không có lỗi, đăng ký người dùng mới
            RegisterNewUser(username, email, password, fullName, dateOfBirth, gender, phoneNumber, address, picturePath);
            MessageBox.Show("Register successfull!");

            var loginWindow = new LoginPage();
            loginWindow.Show();
            this.Close();
        }

        // Hàm đặt thông báo lỗi cho các controls và TextBlock tương ứng
        private void SetError(Control control, TextBlock errorTextBlock, string message)
        {
            control.BorderBrush = Brushes.Red;
            control.ToolTip = message;
            errorTextBlock.Text = message;
            errorTextBlock.Visibility = Visibility.Visible;
        }

        // Hàm xóa tất cả các thông báo lỗi
        private void ClearValidationErrors()
        {
            UsernameTextBox.BorderBrush = null;
            EmailTextBox.BorderBrush = null;
            PasswordBox.BorderBrush = null;
            FullNameTextBox.BorderBrush = null;
            PhoneNumberTextBox.BorderBrush = null;
            AddressTextBox.BorderBrush = null;
            DateOfBirthPicker.BorderBrush = null;
            GenderComboBox.BorderBrush = null;

            EmailErrorTextBlock.Text = "";
            PhoneNumberErrorTextBlock.Text = "";
            FullNameErrorTextBlock.Text = "";
            PasswordErrorTextBlock.Text = "";
            AddressErrorTextBlock.Text = "";
            DateOfBirthErrorTextBlock.Text = "";
            GenderErrorTextBlock.Text = "";

            EmailErrorTextBlock.Visibility = Visibility.Collapsed;
            PhoneNumberErrorTextBlock.Visibility = Visibility.Collapsed;
            FullNameErrorTextBlock.Visibility = Visibility.Collapsed;
            PasswordErrorTextBlock.Visibility = Visibility.Collapsed;
            AddressErrorTextBlock.Visibility = Visibility.Collapsed;
            DateOfBirthErrorTextBlock.Visibility = Visibility.Collapsed;
            GenderErrorTextBlock.Visibility = Visibility.Collapsed;
        }

        // Kiểm tra định dạng email hợp lệ
        public bool IsEmailValid(string email)
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        // Kiểm tra số điện thoại chỉ chứa số
        public bool IsPhoneNumberValid(string phoneNumber)
        {
            return phoneNumber.All(char.IsDigit);
        }

        // Hàm hash mật khẩu bằng MD5
        private string HashPasswordMD5(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return Convert.ToHexString(hashBytes);  // Trả về chuỗi hex của MD5
            }
        }

        // Tạo UserId dựa trên vai trò của người dùng
        private string GenerateUserId(string role, PrnDbContext context)
        {
            string prefix = role == "Student" ? "ST" : "IN";
            int userCount = context.Accounts.Count();
            return prefix + (userCount + 1).ToString("D2");
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            // Tạo và cấu hình hộp thoại chọn file.
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";

            // Hiển thị hộp thoại và kiểm tra xem người dùng có chọn file hay không.
            if (openFileDialog.ShowDialog() == true)
            {
                // Lấy đường dẫn file mà người dùng chọn.
                string sourceFilePath = openFileDialog.FileName;

                // Đường dẫn đích cụ thể trong project, ví dụ: thư mục "img/user-img" trong thư mục gốc của project.
                string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
                string targetDirectory = IOPath.Combine(projectDirectory, "img", "user-img");

                // Tạo thư mục nếu chưa tồn tại.
                Directory.CreateDirectory(targetDirectory);

                // Lấy tên file từ đường dẫn nguồn.
                string fileName = IOPath.GetFileName(sourceFilePath);
                string targetFilePath = IOPath.Combine(targetDirectory, fileName);

                try
                {
                    // Sao chép file từ đường dẫn nguồn vào đường dẫn đích.
                    File.Copy(sourceFilePath, targetFilePath, true);

                    // Lưu trữ đường dẫn của file đã upload để sử dụng sau.
                    UploadedPicturePath = targetFilePath;

                    // Hiển thị ảnh trong Image control (nếu có).
                    ProfilePictureImage.Source = new BitmapImage(new Uri(targetFilePath));

                    MessageBox.Show("Image uploaded successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


    }
}
