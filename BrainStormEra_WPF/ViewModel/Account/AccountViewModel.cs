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

        public ICommand SaveCommand { get; }
        public ICommand ChangePictureCommand { get; }
        public ICommand CancelCommand { get; }

        public AccountViewModel(Models.Account account)
        {
            _originalAccount = account;
            _userId = account.UserId;
            FullName = account.FullName ?? string.Empty;
            DateOfBirth = account.DateOfBirth;
            Gender = account.Gender switch
            {
                "male" => "male",
                "female" => "female",
                "other" => "other",
                _ => "other"
            };
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
            try
            {
                using var context = new PrnDbFpContext();
                var existingAccount = await context.Accounts.FindAsync(UserId);

                if (existingAccount != null)
                {
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving changes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                UserPicture = image;

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
