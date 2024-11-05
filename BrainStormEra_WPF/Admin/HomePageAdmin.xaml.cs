using System;
using System.Windows;
using BrainStormEra_WPF.Models;
using System.Windows.Input;
using System.Windows.Navigation;
using BrainStormEra_WPF.ViewModel.Account;
namespace BrainStormEra_WPF
{
    /// <summary>
    /// Interaction logic for HomePageAdmin.xaml
    /// </summary>
    public partial class HomePageAdmin : Window
    {
        private AccountViewModel _accountViewModel;
        public HomePageAdmin(Account account)
        {
            InitializeComponent();
            _accountViewModel = new AccountViewModel(account);
            DataContext = _accountViewModel;
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
        private void UserPicture_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Open the profile edit window when the user clicks on their profile picture
            var editWindow = new Profile(_accountViewModel); // Pass the current AccountViewModel
            editWindow.Owner = this;  // Optional: Set the owner for modal behavior
            editWindow.ShowDialog();
        }


    }
}
