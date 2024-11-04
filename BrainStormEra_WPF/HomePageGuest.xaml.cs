using System.Security.AccessControl;
using System.Windows;
using BrainStormEra_WPF.Common;
using BrainStormEra_WPF.Common.Login_Register;
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
