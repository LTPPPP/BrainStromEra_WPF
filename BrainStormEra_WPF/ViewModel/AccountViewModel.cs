using System;
using System.IO;
using System.Windows.Media.Imaging;
using BrainStormEra_WPF.Models;

namespace BrainStormEra_WPF.ViewModel
{
    public class AccountViewModel : BaseViewModel
    {
        private string _fullName;
        private BitmapImage _userPicture;

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

        public AccountViewModel(Account account)
        {
            FullName = account.FullName;
            SetUserPicture(account.UserPicture);
        }

        private void SetUserPicture(byte[]? pictureBytes)
        {
            if (pictureBytes != null && pictureBytes.Length > 0)
            {
                using (var ms = new MemoryStream(pictureBytes))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = ms;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    UserPicture = image;
                }
            }
            else
            {
                // Nếu không có hình ảnh, hiển thị hình mặc định
                UserPicture = new BitmapImage(new Uri("pack://application:,,,/BrainStormEra_WPF;component/img/user-img/default_user.png"));
            }
        }
    }
}
