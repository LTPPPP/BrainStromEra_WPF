using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using BrainStormEra_WPF.Models;
using BrainStormEra_WPF.ViewModel;

namespace BrainStormEra_WPF
{
    public partial class Profile : Page
    {
        private AccountViewModel _accountViewModel;

        public Profile(AccountViewModel accountViewModel)
        {
            InitializeComponent();
            _accountViewModel = accountViewModel;
            DataContext = _accountViewModel;

            // Gán ảnh profile từ ViewModel
            if (_accountViewModel.UserPicture != null)
            {
                ProfileImage.ImageSource = _accountViewModel.UserPicture;
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            UpdateAccount();
            MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ChangePicture_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                Title = "Select a Profile Picture"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var selectedImagePath = openFileDialog.FileName;
                var image = new BitmapImage(new Uri(selectedImagePath));
                ProfileImage.ImageSource = image;

                // Cập nhật ảnh trong ViewModel
                using var memoryStream = new MemoryStream();
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(memoryStream);
                _accountViewModel.UserPictureBytes = memoryStream.ToArray();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Quay lại trang trước đó
            NavigationService.GoBack();
        }

        private void UpdateAccount()
        {
            // Cập nhật vào cơ sở dữ liệu
            using (var context = new PrnDbFpContext())
            {
                var existingAccount = context.Accounts.Find(_accountViewModel.UserId);
                if (existingAccount != null)
                {
                    existingAccount.FullName = _accountViewModel.FullName;
                    existingAccount.DateOfBirth = _accountViewModel.DateOfBirth;
                    existingAccount.Gender = _accountViewModel.Gender;
                    existingAccount.PhoneNumber = _accountViewModel.PhoneNumber;
                    existingAccount.UserAddress = _accountViewModel.UserAddress;
                    existingAccount.UserPicture = _accountViewModel.UserPictureBytes;

                    context.SaveChanges();
                }
            }
        }
    }
}
