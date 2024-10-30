using BrainStormEra_WPF.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BrainStormEra_WPF
{
    public partial class LoginPage : Window
    {
        private readonly PrnDbFpContext _dbContext;

        public LoginPage()
        {
            InitializeComponent();
            _dbContext = new PrnDbFpContext();

            // Gắn sự kiện khi nhấn phím Enter trên TextBox và PasswordBox
            UsernameTextBox.KeyDown += OnEnterPressed;
            PasswordBox.KeyDown += OnEnterPressed;
        }

        private void OnEnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(sender, e);
            }
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            HomePageGuest homePageGuest = new HomePageGuest();
            homePageGuest.Show();
            this.Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ErrorMessageTextBlock.Text = "Username and Password cannot be empty.";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                string hashedPassword = ComputeMD5Hash(password);

                var account = _dbContext.Accounts
                    .FirstOrDefault(a => a.Username == username && a.Password == hashedPassword);

                if (account != null)
                {
                    ErrorMessageTextBlock.Visibility = Visibility.Collapsed;

                    switch (account.UserRole)
                    {
                        case 1: // Admin
                            var adminPage = new HomePageAdmin(account);
                            adminPage.Show();
                            this.Close();
                            break;
                        case 2: // Instructor
                            var instructorPage = new HomePageInstructor(account);
                            instructorPage.Show();
                            this.Close();
                            break;
                        case 3: // Learner
                            var learnerPage = new HomePageLearner(account);
                            learnerPage.Show();
                            this.Close();
                            break;
                    }
                }
                else
                {
                    ErrorMessageTextBlock.Text = "Invalid username or password.";
                    ErrorMessageTextBlock.Visibility = Visibility.Visible;

                    // Clear the Username and Password fields when login fails
                    UsernameTextBox.Clear();
                    PasswordBox.Clear();
                }
            }
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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            UsernameTextBox.Clear();
            PasswordBox.Clear();
            ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
        }
    }
}
