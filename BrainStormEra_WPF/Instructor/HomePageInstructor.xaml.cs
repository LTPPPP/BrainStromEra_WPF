using System;
using System.Windows;
using System.Windows.Media.Imaging;
using BrainStormEra_WPF.Instructor;
using BrainStormEra_WPF.Models;
using BrainStormEra_WPF.ViewModel.Account;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BrainStormEra_WPF
{
    /// <summary>
    /// Interaction logic for HomePageInstructor.xaml
    /// </summary>
    public partial class HomePageInstructor : Window
    {
        private AccountViewModel _accountViewModel;

        public HomePageInstructor(Account account)
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


        private void Click_Course(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new CourseIntructor();
        }


    }
}
