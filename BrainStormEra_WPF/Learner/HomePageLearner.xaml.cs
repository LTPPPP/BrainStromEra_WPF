using BrainStormEra_WPF.Instructor;
using BrainStormEra_WPF.Learner;
using BrainStormEra_WPF.Models;
using BrainStormEra_WPF.ViewModel.Account;
using System.Windows;
using System.Windows.Controls;

namespace BrainStormEra_WPF
{
    public partial class HomePageLearner : Window
    {
        private AccountViewModel _accountViewModel;
        public static Frame MainFrameInstance { get; private set; }

        public HomePageLearner(Account account)
        {
            InitializeComponent();
            _accountViewModel = new AccountViewModel(account);
            DataContext = _accountViewModel;
            MainFrameInstance = MainFrame;
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


        private void Click_CourseLearner(object sender, RoutedEventArgs e)
        {
            string userId = _accountViewModel.UserId;
            var courseLearnerPage = new CourseLearner(userId);

            MainFrame.Content = courseLearnerPage;



        }
    }
}
