using BrainStormEra_WPF.Models;
using BrainStormEra_WPF.Utilities;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace BrainStormEra_WPF.ViewModel.Login_Register
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly PrnDbFpContext _dbContext;
        private string _username;
        private string _password;
        private string _errorMessage;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand CancelCommand { get; }

        public LoginViewModel()
        {
            _dbContext = new PrnDbFpContext();
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
            CancelCommand = new RelayCommand(ExecuteCancel);
        }

        private bool CanExecuteLogin(object parameter) => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);

        private void ExecuteLogin(object parameter)
        {
            string hashedPassword = ComputeMD5Hash(Password);
            var account = _dbContext.Accounts
                .FirstOrDefault(a => a.Username == Username && a.Password == hashedPassword);

            if (account != null)
            {
                ErrorMessage = string.Empty;
                OpenUserPage(account);
            }
            else
            {
                ErrorMessage = "Invalid username or password.";
            }
        }

        private void ExecuteCancel(object parameter)
        {
            Username = string.Empty;
            Password = string.Empty;
            ErrorMessage = string.Empty;
        }

        private string ComputeMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private void OpenUserPage(Models.Account account)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Window targetWindow = null;

                // Determine which window to open based on the user role
                switch (account.UserRole)
                {
                    case 1:
                        var homePageAdmin = new HomePageAdmin(account);
                        homePageAdmin.Show();
                        break;
                    case 2:
                        var homePageInstructor = new HomePageInstructor(account);
                        homePageInstructor.Show();
                        break;
                    case 3:
                        var homePageLearner = new HomePageLearner(account);
                        homePageLearner.Show();
                        break;
                }

                // Close the current login window after opening the target window
                Application.Current.MainWindow.Close();
            });
        }

    }
}
