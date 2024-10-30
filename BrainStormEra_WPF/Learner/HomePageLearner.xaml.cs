using BrainStormEra_WPF.Models;
using BrainStormEra_WPF.ViewModel;
using System.Windows;

namespace BrainStormEra_WPF
{
    public partial class HomePageLearner : Window
    {
        private AccountViewModel _accountViewModel;

        public HomePageLearner(Account account)
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
