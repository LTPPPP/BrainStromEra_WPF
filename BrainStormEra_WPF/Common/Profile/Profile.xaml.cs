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
			try
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
			catch (DbUpdateException dbEx)
			{
				// This handles database-specific issues, such as connectivity problems or SQL errors.
				MessageBox.Show("There was an error saving to the database. Please check your connection and try again.\n\nDetails: " + dbEx.Message,
					"Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			catch (ArgumentException argEx)
			{
				// This handles invalid arguments that might be passed, potentially during property validation.
				MessageBox.Show("One or more profile fields are invalid. Please review your input.\n\nDetails: " + argEx.Message,
					"Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			catch (InvalidOperationException invOpEx)
			{
				// This handles invalid operations, such as issues with data binding or incorrect state.
				MessageBox.Show("An operation error occurred. Please try again or restart the application.\n\nDetails: " + invOpEx.Message,
					"Operation Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			catch (Exception ex)
			{
				// Catch any other exceptions that do not fit into the above categories.
				MessageBox.Show("An unexpected error occurred while updating your profile. Please contact support.\n\nDetails: " + ex.Message,
					"Unexpected Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

	}
}
