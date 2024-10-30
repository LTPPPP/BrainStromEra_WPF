using BrainStormEra_WPF.ViewModel;
using System;
using System.Windows;
using BrainStormEra_WPF.Models;
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
    }
}
