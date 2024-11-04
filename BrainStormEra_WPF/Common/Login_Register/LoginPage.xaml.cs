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

            // Optional: Add input validation
            UsernameTextBox.TextChanged += (s, e) => ValidateInput();
            PasswordBox.PasswordChanged += (s, e) => ValidateInput();
        }

        private void ValidateInput()
        {
            LoginButton.IsEnabled = !string.IsNullOrWhiteSpace(UsernameTextBox.Text)
                                  && !string.IsNullOrWhiteSpace(PasswordBox.Password);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the values from the UI controls
                _viewModel.Username = UsernameTextBox.Text;
                _viewModel.Password = PasswordBox.Password;

                // Execute the login command if it can execute
                if (_viewModel.LoginCommand.CanExecute(null))
                {
                    _viewModel.LoginCommand.Execute(null);

                    // Update error message visibility
                    UpdateErrorMessage(_viewModel.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                UpdateErrorMessage($"An error occurred: {ex.Message}");
            }
        }

        private void UpdateErrorMessage(string message)
        {
            ErrorMessageTextBlock.Text = message;
            ErrorMessageTextBlock.Visibility = string.IsNullOrEmpty(message)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Execute the cancel command
                _viewModel.CancelCommand?.Execute(null);

                // Clear the UI controls
                UsernameTextBox.Text = string.Empty;
                PasswordBox.Password = string.Empty;
                ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
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