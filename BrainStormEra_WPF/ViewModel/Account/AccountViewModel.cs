using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BrainStormEra_WPF.Models;
using Microsoft.Win32;
using Microsoft.EntityFrameworkCore;
using BrainStormEra_WPF.Utilities;
using System.Windows;

namespace BrainStormEra_WPF.ViewModel.Account
{
    public class AccountViewModel : BaseViewModel
    {
        private readonly Models.Account _originalAccount;

        private string _userId;
        private string _fullName;
        private BitmapImage _userPicture;
        private DateOnly? _dateOfBirth;
        private string _gender;
        private string _phoneNumber;
        private string _userAddress;
        private bool _isMale;
        private bool _isFemale;
        private bool _isOther;
		private string _fullNameError;
		private string _dateOfBirthError;
		private string _genderError;
		private string _phoneNumberError;
		private string _addressError;

		public string UserId
        {
            get => _userId;
            set { _userId = value; OnPropertyChanged(); }
        }

        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(); }
        }

        public BitmapImage UserPicture
        {
            get => _userPicture;
            set { _userPicture = value; OnPropertyChanged(); }
        }

        public byte[]? UserPictureBytes { get; set; }

        public DateOnly? DateOfBirth
        {
            get => _dateOfBirth;
            set { _dateOfBirth = value; OnPropertyChanged(); OnPropertyChanged(nameof(DateOfBirthDateTime)); }
        }

        public DateTime? DateOfBirthDateTime
        {
            get => DateOfBirth.HasValue ? DateOfBirth.Value.ToDateTime(new TimeOnly(0, 0)) : (DateTime?)null;
            set
            {
                DateOfBirth = value.HasValue ? DateOnly.FromDateTime(value.Value) : null;
                OnPropertyChanged();
            }
        }


        public string Gender
        {
            get => _gender;
            set
            {
                _gender = value == "male" || value == "female" || value == "other" ? value : "other";
                OnPropertyChanged();
            }
        }
        public bool IsMale
        {
            get => _isMale;
            set
            {
                if (value)
                {
                    IsFemale = IsOther = false; // Uncheck others
                    Gender = "male";
                }
                _isMale = value;
                OnPropertyChanged();
            }
        }

        public bool IsFemale
        {
            get => _isFemale;
            set
            {
                if (value)
                {
                    IsMale = IsOther = false; // Uncheck others
                    Gender = "female";
                }
                _isFemale = value;
                OnPropertyChanged();
            }
        }

        public bool IsOther
        {
            get => _isOther;
            set
            {
                if (value)
                {
                    IsMale = IsFemale = false; // Uncheck others
                    Gender = "other";
                }
                _isOther = value;
                OnPropertyChanged();
            }
        }
        public string PhoneNumber
        {
            get => _phoneNumber;
            set { _phoneNumber = value; OnPropertyChanged(); }
        }

        public string UserAddress
        {
            get => _userAddress;
            set { _userAddress = value; OnPropertyChanged(); }
        }
		public string FullNameError
		{
			get => _fullNameError;
			set { _fullNameError = value; OnPropertyChanged(); }
		}

		public string DateOfBirthError
		{
			get => _dateOfBirthError;
			set { _dateOfBirthError = value; OnPropertyChanged(); }
		}

		public string GenderError
		{
			get => _genderError;
			set { _genderError = value; OnPropertyChanged(); }
		}

		public string PhoneNumberError
		{
			get => _phoneNumberError;
			set { _phoneNumberError = value; OnPropertyChanged(); }
		}

		public string AddressError
		{
			get => _addressError;
			set { _addressError = value; OnPropertyChanged(); }
		}

		// Add validation methods
		private void ValidateFullName()
		{
			FullNameError = string.IsNullOrEmpty(FullName) || FullName.Length < 2
				? "Full name must be at least 2 characters and contain only letters and spaces."
				: string.Empty;
		}

		private void ValidateDateOfBirth()
		{
			if (!DateOfBirth.HasValue)
				DateOfBirthError = "Date of birth is required.";
			else if (DateOfBirth.Value > DateOnly.FromDateTime(DateTime.Now))
				DateOfBirthError = "Date of birth cannot be in the future.";
			else
				DateOfBirthError = string.Empty;
		}

		private void ValidateGender()
		{
			GenderError = string.IsNullOrEmpty(Gender) || !(Gender == "male" || Gender == "female" || Gender == "other")
				? "Please select a valid gender."
				: string.Empty;
		}

		private void ValidatePhoneNumber()
		{
			PhoneNumberError = string.IsNullOrEmpty(PhoneNumber) || !PhoneNumber.All(char.IsDigit) || PhoneNumber.Length < 10 || PhoneNumber.Length > 11
				? "Phone number must be 10-11 digits and contain only numbers."
				: string.Empty;
		}

		private void ValidateAddress()
		{
			AddressError = string.IsNullOrEmpty(UserAddress) || UserAddress.Length < 5
				? "Address must be at least 5 characters long."
				: string.Empty;
		}
		public ICommand SaveCommand { get; }
        public ICommand ChangePictureCommand { get; }
        public ICommand CancelCommand { get; }

        public AccountViewModel(Models.Account account)
        {
            _originalAccount = account;
            _userId = account.UserId;
            FullName = account.FullName ?? string.Empty;
            DateOfBirth = account.DateOfBirth;
            switch (account.Gender)
            {
                case "male":
                    IsMale = true;
                    break;
                case "female":
                    IsFemale = true;
                    break;
                case "other":
                    IsOther = true;
                    break;
                default:
                    IsOther = true; // default case
                    break;
            }
            PhoneNumber = account.PhoneNumber ?? string.Empty;
            UserAddress = account.UserAddress ?? string.Empty;
            SetUserPicture(account.UserPicture);

            SaveCommand = new RelayCommand(async param => await SaveChangesAsync());
            ChangePictureCommand = new RelayCommand(ChangePicture);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void SetUserPicture(byte[]? pictureBytes)
        {
            if (pictureBytes != null && pictureBytes.Length > 0)
            {
                using var ms = new MemoryStream(pictureBytes);
                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                UserPicture = image;
            }
            else
            {
                UserPicture = new BitmapImage(new Uri("pack://application:,,,/BrainStormEra_WPF;component/img/user-img/default_user.png"));
            }
            UserPictureBytes = pictureBytes;
        }

		private async Task SaveChangesAsync()
		{
			// Trigger all validations before attempting to save
			ValidateFullName();
			ValidateDateOfBirth();
			ValidateGender();
			ValidatePhoneNumber();
			ValidateAddress();

			// Collect all error messages into a single string
			string errorMessages = string.Join("\n",
				new[]
				{
			FullNameError,
			DateOfBirthError,
			GenderError,
			PhoneNumberError,
			AddressError
				}.Where(error => !string.IsNullOrEmpty(error))); // Only include non-empty error messages

			// Show all errors in a MessageBox if there are any
			if (!string.IsNullOrEmpty(errorMessages))
			{
				MessageBox.Show($"Please correct the following errors before saving:\n\n{errorMessages}",
								"Validation Errors",
								MessageBoxButton.OK,
								MessageBoxImage.Warning);
				return;
			}

			try
			{
				using var context = new PrnDbFpContext();
				var existingAccount = await context.Accounts.FindAsync(UserId);

				if (existingAccount != null)
				{
					// Update account properties
					existingAccount.FullName = FullName;
					existingAccount.DateOfBirth = DateOfBirth;
					existingAccount.Gender = Gender;
					existingAccount.PhoneNumber = PhoneNumber;
					existingAccount.UserAddress = UserAddress;
					existingAccount.UserPicture = UserPictureBytes;

					context.Entry(existingAccount).State = EntityState.Modified;
					await context.SaveChangesAsync();

					MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				else
				{
					MessageBox.Show("User account not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			catch (DbUpdateException dbEx)
			{
				// Handle database update errors
				MessageBox.Show("A database error occurred while saving your profile. Please check your connection and try again.\n\nDetails: " + dbEx.Message,
					"Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			catch (ArgumentException argEx)
			{
				// Handle invalid arguments, often due to incorrect values
				MessageBox.Show("There is an issue with one or more profile fields. Please review your inputs.\n\nDetails: " + argEx.Message,
					"Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			catch (InvalidOperationException invOpEx)
			{
				// Handle invalid operations
				MessageBox.Show("An unexpected error occurred during the operation. Please try again or restart the application.\n\nDetails: " + invOpEx.Message,
					"Operation Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			catch (Exception ex)
			{
				// Catch all other unexpected errors
				MessageBox.Show("An unexpected error occurred while updating your profile. Please contact support.\n\nDetails: " + ex.Message,
					"Unexpected Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}


		private void ChangePicture(object parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                Title = "Select a Profile Picture"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var selectedImagePath = openFileDialog.FileName;
                var image = new BitmapImage(new Uri(selectedImagePath));
                UserPicture = image; // This will automatically update the ImageBrush binding in the UI

                using var memoryStream = new MemoryStream();
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(memoryStream);
                UserPictureBytes = memoryStream.ToArray();
            }
        }


        private void Cancel(object parameter)
        {
            FullName = _originalAccount.FullName;
            DateOfBirth = _originalAccount.DateOfBirth;
            Gender = _originalAccount.Gender ?? "other";
            PhoneNumber = _originalAccount.PhoneNumber;
            UserAddress = _originalAccount.UserAddress;
            SetUserPicture(_originalAccount.UserPicture);
        }
    }
}
