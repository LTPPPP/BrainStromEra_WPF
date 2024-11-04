using BrainStormEra_WPF.ViewModel.Login_Register;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BrainStormEra_WPF.Common.Login_Register
{
    public partial class LoginPage : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginPage()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
            DataContext = _viewModel;
        }

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInput();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ValidateInput();
        }

        private void ValidateInput()
        {
            if (LoginButton != null)
            {
                LoginButton.IsEnabled = !string.IsNullOrWhiteSpace(UsernameTextBox?.Text)
                                      && !string.IsNullOrWhiteSpace(PasswordBox?.Password);
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (UsernameTextBox != null && PasswordBox != null)
                {
                    _viewModel.Username = UsernameTextBox.Text;
                    _viewModel.Password = PasswordBox.Password;

                    if (_viewModel.LoginCommand.CanExecute(null))
                    {
                        _viewModel.LoginCommand.Execute(null);
                        this.Close();
                        UpdateErrorMessage(_viewModel.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateErrorMessage($"An error occurred: {ex.Message}");
            }
        }

        private void UpdateErrorMessage(string message)
        {
            if (ErrorMessageTextBlock != null)
            {
                ErrorMessageTextBlock.Text = message;
                ErrorMessageTextBlock.Visibility = string.IsNullOrEmpty(message)
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _viewModel.CancelCommand?.Execute(null);

                if (UsernameTextBox != null && PasswordBox != null && ErrorMessageTextBlock != null)
                {
                    UsernameTextBox.Text = string.Empty;
                    PasswordBox.Password = string.Empty;
                    ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                UpdateErrorMessage($"An error occurred while canceling: {ex.Message}");
            }
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var homePageGuest = new HomePageGuest();
                homePageGuest.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                UpdateErrorMessage($"An error occurred while returning: {ex.Message}");
            }
        }
    }
}