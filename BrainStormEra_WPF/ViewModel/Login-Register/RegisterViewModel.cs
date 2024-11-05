using BrainStormEra_WPF.Models;
using BrainStormEra_WPF.Utilities;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using BrainStormEra_WPF.Common;

namespace BrainStormEra_WPF.ViewModel.Login_Register
{
    public class RegisterViewModel : BaseViewModel
    {
        private string _username;
        private string _email;
        private string _password;
        private string _fullName;
        private DateTime? _dateOfBirth;
        private string _selectedGender;
        private string _phoneNumber;
        private string _address;
        private string _selectedRole;
        private BitmapImage _profilePicture;

        // Validation error messages
        private string _usernameError;
        private string _emailError;
        private string _passwordError;
        private string _fullNameError;
        private string _dateOfBirthError;
        private string _genderError;
        private string _phoneNumberError;
        private string _addressError;

        // Commands
        public ICommand RegisterCommand { get; }
        public ICommand UploadPictureCommand { get; }
        public ICommand ReturnCommand { get; }

        // Lists for ComboBoxes
        public List<string> Genders { get; } = new List<string> { "Male", "Female", "Other" };
        public List<string> Roles { get; } = new List<string> { "Student", "Instructor" };

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(ExecuteRegister, CanExecuteRegister);
            UploadPictureCommand = new RelayCommand(ExecuteUploadPicture);
            ReturnCommand = new RelayCommand(ExecuteReturn);
        }

        // Properties with validation
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                ValidateUsername();
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                ValidateEmail();
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                ValidatePassword();
                OnPropertyChanged();
            }
        }

        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                ValidateFullName();
                OnPropertyChanged();
            }
        }

        public DateTime? DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                _dateOfBirth = value;
                ValidateDateOfBirth();
                OnPropertyChanged();
            }
        }

        public string SelectedGender
        {
            get => _selectedGender;
            set
            {
                _selectedGender = value;
                ValidateGender();
                OnPropertyChanged();
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                ValidatePhoneNumber();
                OnPropertyChanged();
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                ValidateAddress();
                OnPropertyChanged();
            }
        }

        public string SelectedRole
        {
            get => _selectedRole;
            set
            {
                _selectedRole = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage ProfilePicture
        {
            get => _profilePicture;
            set
            {
                _profilePicture = value;
                OnPropertyChanged();
            }
        }

        // Error properties
        public string UsernameError
        {
            get => _usernameError;
            set
            {
                _usernameError = value;
                OnPropertyChanged();
            }
        }

        public string EmailError
        {
            get => _emailError;
            set
            {
                _emailError = value;
                OnPropertyChanged();
            }
        }

        public string PasswordError
        {
            get => _passwordError;
            set
            {
                _passwordError = value;
                OnPropertyChanged();
            }
        }

        public string FullNameError
        {
            get => _fullNameError;
            set
            {
                _fullNameError = value;
                OnPropertyChanged();
            }
        }

        public string DateOfBirthError
        {
            get => _dateOfBirthError;
            set
            {
                _dateOfBirthError = value;
                OnPropertyChanged();
            }
        }

        public string GenderError
        {
            get => _genderError;
            set
            {
                _genderError = value;
                OnPropertyChanged();
            }
        }

        public string PhoneNumberError
        {
            get => _phoneNumberError;
            set
            {
                _phoneNumberError = value;
                OnPropertyChanged();
            }
        }

        public string AddressError
        {
            get => _addressError;
            set
            {
                _addressError = value;
                OnPropertyChanged();
            }
        }

        // Validation methods
        private void ValidateUsername()
        {
            if (string.IsNullOrEmpty(Username))
            {
                UsernameError = "Username cannot be empty.";
                return;
            }

            if (Username.Length < 3)
            {
                UsernameError = "Username must be at least 3 characters long.";
                return;
            }

            if (!Regex.IsMatch(Username, "^[a-zA-Z0-9_]+$"))
            {
                UsernameError = "Username can only contain letters, numbers, and underscores.";
                return;
            }

            UsernameError = string.Empty;
        }

        private void ValidateEmail()
        {
            if (string.IsNullOrEmpty(Email))
            {
                EmailError = "Email cannot be empty.";
                return;
            }

            if (!IsEmailValid(Email))
            {
                EmailError = "Invalid email address.";
                return;
            }

            EmailError = string.Empty;
        }

        private void ValidatePassword()
        {
            if (string.IsNullOrEmpty(Password))
            {
                PasswordError = "Password cannot be empty.";
                return;
            }

            if (Password.Length < 6)
            {
                PasswordError = "Password must be at least 6 characters long.";
                return;
            }

            if (!Regex.IsMatch(Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$"))
            {
                PasswordError = "Password must contain at least one uppercase letter, one lowercase letter, and one number.";
                return;
            }

            PasswordError = string.Empty;
        }

        private void ValidateFullName()
        {
            if (string.IsNullOrEmpty(FullName))
            {
                FullNameError = "Full name cannot be empty.";
                return;
            }

            if (FullName.Length < 2)
            {
                FullNameError = "Full name must be at least 2 characters long.";
                return;
            }

            if (!Regex.IsMatch(FullName, @"^[a-zA-Z\s]+$"))
            {
                FullNameError = "Full name can only contain letters and spaces.";
                return;
            }

            FullNameError = string.Empty;
        }

        private void ValidateDateOfBirth()
        {
            if (!DateOfBirth.HasValue)
            {
                DateOfBirthError = "Date of birth is required.";
                return;
            }

            if (DateOfBirth.Value > DateTime.Now)
            {
                DateOfBirthError = "Date of birth cannot be in the future.";
                return;
            }

            var age = DateTime.Now.Year - DateOfBirth.Value.Year;
            if (DateOfBirth.Value > DateTime.Now.AddYears(-age))
            {
                age--;
            }

            if (age < 13)
            {
                DateOfBirthError = "You must be at least 13 years old to register.";
                return;
            }

            DateOfBirthError = string.Empty;
        }

        private void ValidateGender()
        {
            if (string.IsNullOrEmpty(SelectedGender))
            {
                GenderError = "Please select a gender.";
                return;
            }

            if (!Genders.Contains(SelectedGender))
            {
                GenderError = "Invalid gender selection.";
                return;
            }

            GenderError = string.Empty;
        }

        private void ValidatePhoneNumber()
        {
            if (string.IsNullOrEmpty(PhoneNumber))
            {
                PhoneNumberError = "Phone number cannot be empty.";
                return;
            }

            if (!IsPhoneNumberValid(PhoneNumber))
            {
                PhoneNumberError = "Invalid phone number format.";
                return;
            }

            if (PhoneNumber.Length < 10 || PhoneNumber.Length > 11)
            {
                PhoneNumberError = "Phone number must be 10 or 11 digits.";
                return;
            }

            PhoneNumberError = string.Empty;
        }

        private void ValidateAddress()
        {
            if (string.IsNullOrEmpty(Address))
            {
                AddressError = "Address cannot be empty.";
                return;
            }

            if (Address.Length < 5)
            {
                AddressError = "Address must be at least 5 characters long.";
                return;
            }

            AddressError = string.Empty;
        }

        // Helper methods
        private bool IsEmailValid(string email)
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return !string.IsNullOrEmpty(email) && Regex.IsMatch(email, emailPattern);
        }

        private bool IsPhoneNumberValid(string phoneNumber)
        {
            return !string.IsNullOrEmpty(phoneNumber) && phoneNumber.All(char.IsDigit);
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

        private string GenerateUserId(string role, PrnDbFpContext context)
        {
            string prefix = role == "Student" ? "ST" : "IN";
            int userCount = context.Accounts.Count();
            return prefix + (userCount + 1).ToString("D2");
        }

        // Command execution methods
        private bool CanExecuteRegister(object parameter)
        {
            return !string.IsNullOrEmpty(Username) &&
                   !string.IsNullOrEmpty(Email) &&
                   !string.IsNullOrEmpty(Password) &&
                   !string.IsNullOrEmpty(FullName) &&
                   DateOfBirth.HasValue &&
                   !string.IsNullOrEmpty(SelectedGender) &&
                   !string.IsNullOrEmpty(PhoneNumber) &&
                   !string.IsNullOrEmpty(Address) &&
                   !string.IsNullOrEmpty(SelectedRole) &&
                   string.IsNullOrEmpty(UsernameError) &&
                   string.IsNullOrEmpty(EmailError) &&
                   string.IsNullOrEmpty(PasswordError) &&
                   string.IsNullOrEmpty(FullNameError) &&
                   string.IsNullOrEmpty(DateOfBirthError) &&
                   string.IsNullOrEmpty(GenderError) &&
                   string.IsNullOrEmpty(PhoneNumberError) &&
                   string.IsNullOrEmpty(AddressError);
        }

        private void ExecuteRegister(object parameter)
        {
            if (!ValidateAll()) return;

            try
            {
                using (var context = new PrnDbFpContext())
                {
                    // Check if username already exists
                    if (context.Accounts.Any(a => a.Username == Username))
                    {
                        MessageBox.Show("Username already exists. Please choose a different username.", "Registration Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Check if email already exists
                    if (context.Accounts.Any(a => a.UserEmail == Email))
                    {
                        MessageBox.Show("Email already exists. Please use a different email address.", "Registration Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    string hashedPassword = HashPasswordMD5(Password);
                    int userRole = SelectedRole == "Student" ? 3 : 2;
                    string userId = GenerateUserId(SelectedRole, context);

                    var newUser = new Models.Account
                    {
                        UserId = userId,
                        Username = Username,
                        UserEmail = Email,
                        Password = hashedPassword,
                        UserRole = userRole,
                        FullName = FullName,
                        DateOfBirth = DateOfBirth.HasValue ? DateOnly.FromDateTime(DateOfBirth.Value) : null,
                        Gender = SelectedGender,
                        PhoneNumber = PhoneNumber,
                        UserAddress = Address,
                        UserPicture = ProfilePicture != null ? Convert.FromBase64String(ConvertImageToBase64(ProfilePicture)) : null,
                        AccountCreatedAt = DateTime.Now
                    };

                    context.Accounts.Add(newUser);
                    context.SaveChanges();

                    MessageBox.Show("Registration successful!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Navigate to login page
                    var loginWindow = new LoginPage();
                    Application.Current.MainWindow.Close();
                    Application.Current.MainWindow = loginWindow;
                    loginWindow.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Registration failed: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteUploadPicture(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    ProfilePicture = new BitmapImage(new Uri(openFileDialog.FileName));

                    // Validate image size (max 5MB)
                    FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
                    if (fileInfo.Length > 5 * 1024 * 1024)
                    {
                        MessageBox.Show("Image size must be less than 5MB.", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        ProfilePicture = null;
                        return;
                    }

                    MessageBox.Show("Image uploaded successfully.", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error uploading image: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    ProfilePicture = null;
                }
            }
        }

        private void ExecuteReturn(object parameter)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to return? Any unsaved information will be lost.",
                "Confirm Return",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var homePageGuest = new HomePageGuest();
                Application.Current.MainWindow.Close();
                Application.Current.MainWindow = homePageGuest;
                homePageGuest.Show();
            }
        }

        private bool ValidateAll()
        {
            ValidateUsername();
            ValidateEmail();
            ValidatePassword();
            ValidateFullName();
            ValidateDateOfBirth();
            ValidateGender();
            ValidatePhoneNumber();
            ValidateAddress();

            // Check if role is selected
            if (string.IsNullOrEmpty(SelectedRole))
            {
                MessageBox.Show("Please select a role (Student or Instructor).", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Combine all validation errors
            var errors = new List<string>();

            if (!string.IsNullOrEmpty(UsernameError)) errors.Add(UsernameError);
            if (!string.IsNullOrEmpty(EmailError)) errors.Add(EmailError);
            if (!string.IsNullOrEmpty(PasswordError)) errors.Add(PasswordError);
            if (!string.IsNullOrEmpty(FullNameError)) errors.Add(FullNameError);
            if (!string.IsNullOrEmpty(DateOfBirthError)) errors.Add(DateOfBirthError);
            if (!string.IsNullOrEmpty(GenderError)) errors.Add(GenderError);
            if (!string.IsNullOrEmpty(PhoneNumberError)) errors.Add(PhoneNumberError);
            if (!string.IsNullOrEmpty(AddressError)) errors.Add(AddressError);

            // If there are any validation errors, show them to the user
            if (errors.Any())
            {
                string errorMessage = string.Join("\n", errors);
                MessageBox.Show(errorMessage, "Validation Errors",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        // Optional: Method to clear all fields
        public void ClearFields()
        {
            Username = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            FullName = string.Empty;
            DateOfBirth = null;
            SelectedGender = null;
            PhoneNumber = string.Empty;
            Address = string.Empty;
            SelectedRole = null;
            ProfilePicture = null;

            // Clear all error messages
            UsernameError = string.Empty;
            EmailError = string.Empty;
            PasswordError = string.Empty;
            FullNameError = string.Empty;
            DateOfBirthError = string.Empty;
            GenderError = string.Empty;
            PhoneNumberError = string.Empty;
            AddressError = string.Empty;
        }
    }
}