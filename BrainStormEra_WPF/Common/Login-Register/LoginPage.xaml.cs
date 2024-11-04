using BrainStormEra_WPF.ViewModel.Login;
using System.Windows;

namespace BrainStormEra_WPF
{
    public partial class LoginPage : Window
    {
        private LoginViewModel _viewModel;

        public LoginPage()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
            DataContext = _viewModel;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the values from the UI controls
            _viewModel.Username = UsernameTextBox.Text;
            _viewModel.Password = PasswordBox.Password;

            // Execute the login command if it can execute
            if (_viewModel.LoginCommand.CanExecute(null))
            {
                _viewModel.LoginCommand.Execute(null);

                // Update error message visibility
                ErrorMessageTextBlock.Text = _viewModel.ErrorMessage;
                ErrorMessageTextBlock.Visibility =
                    string.IsNullOrEmpty(_viewModel.ErrorMessage)
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Execute the cancel command
            _viewModel.CancelCommand.Execute(null);

            // Clear the UI controls
            UsernameTextBox.Text = string.Empty;
            PasswordBox.Password = string.Empty;
            ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            var homePageGuest = new HomePageGuest();
            homePageGuest.Show();
            this.Close();
        }
    }
}