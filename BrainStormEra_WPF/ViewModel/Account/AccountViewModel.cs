using System;
using System.IO;
using System.Windows.Media.Imaging;
using BrainStormEra_WPF.Models;

namespace BrainStormEra_WPF.ViewModel.Account
{
    public class AccountViewModel : BaseViewModel
    {
        private string _UserId;
        private string _fullName;
        private BitmapImage _userPicture;
        private DateOnly? _dateOfBirth;
        private string _gender;
        private string _phoneNumber;
        private string _userAddress;

        public string UserId
        {
            get => _UserId;
            set
            {
                _UserId = value;
                OnPropertyChanged();
            }
        }

        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage UserPicture
        {
            get => _userPicture;
            set
            {
                _userPicture = value;
                OnPropertyChanged();
            }
        }

        public byte[]? UserPictureBytes { get; set; }

        public DateOnly? DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                _dateOfBirth = value;
                OnPropertyChanged();
            }
        }

        public string Gender
        {
            get => _gender;
            set
            {
                _gender = value;
                OnPropertyChanged();
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();
            }
        }

        public string UserAddress
        {
            get => _userAddress;
            set
            {
                _userAddress = value;
                OnPropertyChanged();
            }
        }

        public AccountViewModel(Models.Account account)
        {
            FullName = account.FullName ?? string.Empty;
            DateOfBirth = account.DateOfBirth;
            Gender = account.Gender ?? "Other";
            PhoneNumber = account.PhoneNumber ?? string.Empty;
            UserAddress = account.UserAddress ?? string.Empty;
            SetUserPicture(account.UserPicture);
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
    }
}
