using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using BrainStormEra_WPF.Models;
using BrainStormEra_WPF.ViewModel.Account;
using Microsoft.EntityFrameworkCore;

namespace BrainStormEra_WPF
{
    public partial class Profile : Window
    {
        private readonly AccountViewModel _accountViewModel;

        public Profile(AccountViewModel accountViewModel)
        {
            InitializeComponent();
            DataContext = accountViewModel;
            Console.WriteLine(DataContext);
            // Load profile image from ViewModel
            if (accountViewModel.UserPicture != null)
            {
                ProfileImage.ImageSource = accountViewModel.UserPicture;
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateAccount();
                MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating profile: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChangePicture_Click(object sender, RoutedEventArgs e)
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
                ProfileImage.ImageSource = image;

                // Update image in ViewModel
                using var memoryStream = new MemoryStream();
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(memoryStream);
                _accountViewModel.UserPictureBytes = memoryStream.ToArray();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UpdateAccount()
        {
            using var context = new PrnDbFpContext();
            var existingAccount = context.Accounts.Find(_accountViewModel.UserId);

            if (existingAccount != null)
            {
                existingAccount.FullName = _accountViewModel.FullName;
                existingAccount.DateOfBirth = _accountViewModel.DateOfBirth;
                existingAccount.Gender = _accountViewModel.Gender;
                existingAccount.PhoneNumber = _accountViewModel.PhoneNumber;
                existingAccount.UserAddress = _accountViewModel.UserAddress;
                existingAccount.UserPicture = _accountViewModel.UserPictureBytes;

                // Explicitly mark the entity as modified
                context.Entry(existingAccount).State = EntityState.Modified;
                context.SaveChanges();
            }
            else
            {
                MessageBox.Show("User account not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
