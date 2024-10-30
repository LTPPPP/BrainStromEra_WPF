using System;
using System.Windows;
using System.Windows.Media.Imaging;
using BrainStormEra_WPF.Models;

namespace BrainStormEra_WPF
{
    /// <summary>
    /// Interaction logic for HomePageInstructor.xaml
    /// </summary>
    public partial class HomePageInstructor : Window
    {
        private Account _currentAccount;

        public HomePageInstructor(Account account)
        {
            InitializeComponent();
            _currentAccount = account;

            // Hiển thị tên và hình ảnh của người dùng
            DisplayUserInfo();
        }

        private void DisplayUserInfo()
        {
            // Hiển thị tên người dùng
            UserNameTextBlock.Text = _currentAccount.FullName;

            // Hiển thị hình ảnh người dùng
            if (!string.IsNullOrEmpty(_currentAccount.UserPicture))
            {
                UserImage.Source = new BitmapImage(new Uri(_currentAccount.UserPicture, UriKind.RelativeOrAbsolute));
            }
            else
            {
                // Nếu không có hình ảnh, có thể hiển thị hình ảnh mặc định
                UserImage.Source = new BitmapImage(new Uri("pack://application:,,,/BrainStormEra_WPF;component/img/user-img/default_user.png"));
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmLogoutDialog confirmDialog = new ConfirmLogoutDialog();
            confirmDialog.Owner = this;
            confirmDialog.ShowDialog();

            if (confirmDialog.IsConfirmed)
            {
                HomePageGuest guestPage = new HomePageGuest();
                guestPage.Show();
                this.Close();
            }
        }
    }
}
