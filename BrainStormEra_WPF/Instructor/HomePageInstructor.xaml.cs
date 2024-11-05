using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BrainStormEra_WPF.Instructor;
using BrainStormEra_WPF.Models;
using BrainStormEra_WPF.ViewModel;
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
        public static Frame MainFrameInstance { get; private set; }

        public HomePageInstructor(Account account)
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


        private void Click_Course(object sender, RoutedEventArgs e)
        {
            string userId = _accountViewModel.UserId;
            var courseInstructorPage = new CourseIntructor(userId);

            MainFrame.Content = courseInstructorPage;
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
