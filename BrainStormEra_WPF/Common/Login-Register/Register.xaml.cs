using BrainStormEra_WPF.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace BrainStormEra_WPF
{
    public partial class Register : Window
    {
        private BitmapImage? UploadedPicture;

        public Register()
        {
            InitializeComponent();
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            var homePageGuest = new HomePageGuest();
            homePageGuest.Show();
            this.Close();
        }

        // Chuyển đổi hình ảnh BitmapImage thành chuỗi Base64
        private string? ConvertImageToBase64(BitmapImage image)
        {
            if (image == null) return null;

            using (var ms = new MemoryStream())
            {
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(ms);
                byte[] imageBytes = ms.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }

        public void RegisterNewUser(string username, string email, string password, string fullName, DateTime? dateOfBirth, string gender, string phoneNumber, string address, BitmapImage? picture)
        {
            using (var context = new PrnDbFpContext())
            {
                string hashedPassword = HashPasswordMD5(password);
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
                    UserPicture = picture != null ? Convert.FromBase64String(ConvertImageToBase64(picture)) : null,
                    AccountCreatedAt = DateTime.Now
                };

                context.Accounts.Add(newUser);
                context.SaveChanges();
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            ClearValidationErrors();

            string username = UsernameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;
            string fullName = FullNameTextBox.Text;
            DateTime? dateOfBirth = DateOfBirthPicker.SelectedDate;
            string gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string phoneNumber = PhoneNumberTextBox.Text;
            string address = AddressTextBox.Text;

            bool hasError = ValidateInput(username, email, password, fullName, dateOfBirth, gender, phoneNumber, address);
            if (hasError) return;

            RegisterNewUser(username, email, password, fullName, dateOfBirth, gender, phoneNumber, address, UploadedPicture);
            MessageBox.Show("Register successful!");

            var loginWindow = new LoginPage();
            loginWindow.Show();
            this.Close();
        }

        private bool ValidateInput(string username, string email, string password, string fullName, DateTime? dateOfBirth, string gender, string phoneNumber, string address)
        {
            bool hasError = false;

            if (string.IsNullOrEmpty(username)) { SetError(UsernameTextBox, UsernameErrorTextBlock, "Username can't be empty."); hasError = true; }
            if (string.IsNullOrEmpty(email) || !IsEmailValid(email)) { SetError(EmailTextBox, EmailErrorTextBlock, "Invalid Email."); hasError = true; }
            if (string.IsNullOrEmpty(phoneNumber) || !IsPhoneNumberValid(phoneNumber)) { SetError(PhoneNumberTextBox, PhoneNumberErrorTextBlock, "Invalid phone number."); hasError = true; }
            if (string.IsNullOrEmpty(password) || password.Length < 6) { SetError(PasswordBox, PasswordErrorTextBlock, "Password must be at least 6 characters."); hasError = true; }
            if (string.IsNullOrEmpty(fullName)) { SetError(FullNameTextBox, FullNameErrorTextBlock, "Fullname can't be empty."); hasError = true; }
            if (!dateOfBirth.HasValue) { SetError(DateOfBirthPicker, DateOfBirthErrorTextBlock, "Invalid date of birth."); hasError = true; }
            if (string.IsNullOrEmpty(gender)) { SetError(GenderComboBox, GenderErrorTextBlock, "Please select gender."); hasError = true; }
            if (string.IsNullOrEmpty(address)) { SetError(AddressTextBox, AddressErrorTextBlock, "Address cannot be empty."); hasError = true; }

            return hasError;
        }

        private void ClearValidationErrors()
        {
            UsernameErrorTextBlock.Text = string.Empty;
            EmailErrorTextBlock.Text = string.Empty;
            PasswordErrorTextBlock.Text = string.Empty;
            FullNameErrorTextBlock.Text = string.Empty;
            DateOfBirthErrorTextBlock.Text = string.Empty;
            GenderErrorTextBlock.Text = string.Empty;
            PhoneNumberErrorTextBlock.Text = string.Empty;
            AddressErrorTextBlock.Text = string.Empty;

            UsernameTextBox.BorderBrush = Brushes.Gray;
            EmailTextBox.BorderBrush = Brushes.Gray;
            PasswordBox.BorderBrush = Brushes.Gray;
            FullNameTextBox.BorderBrush = Brushes.Gray;
            DateOfBirthPicker.BorderBrush = Brushes.Gray;
            GenderComboBox.BorderBrush = Brushes.Gray;
            PhoneNumberTextBox.BorderBrush = Brushes.Gray;
            AddressTextBox.BorderBrush = Brushes.Gray;
        }

        private void SetError(Control control, TextBlock errorTextBlock, string errorMessage)
        {
            errorTextBlock.Text = errorMessage;
            control.BorderBrush = Brushes.Red;
            control.Focus();
        }

        private string HashPasswordMD5(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return Convert.ToHexString(hashBytes);
            }
        }

        private string GenerateUserId(string role, PrnDbFpContext context)
        {
            string prefix = role == "Student" ? "ST" : "IN";
            int userCount = context.Accounts.Count();
            return prefix + (userCount + 1).ToString("D2");
        }

        public bool IsEmailValid(string email)
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        public bool IsPhoneNumberValid(string phoneNumber)
        {
            return phoneNumber.All(char.IsDigit);
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                UploadedPicture = new BitmapImage(new Uri(openFileDialog.FileName));
                ProfilePictureImage.Source = UploadedPicture;
                MessageBox.Show("Image uploaded successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
