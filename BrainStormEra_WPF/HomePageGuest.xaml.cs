using System.Security.AccessControl;
using System.Windows;

namespace BrainStormEra_WPF
{
    public partial class HomePageGuest : Window
    {
        public HomePageGuest()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Register register = new Register();
            register.Show();
            this.Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginPage loginPage = new LoginPage();
            loginPage.Show();
            this.Close();
        }
    }
}
