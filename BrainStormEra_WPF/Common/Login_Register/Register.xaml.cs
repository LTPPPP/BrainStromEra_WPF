using BrainStormEra_WPF.ViewModel.Login_Register;
using System.Windows;

namespace BrainStormEra_WPF
{
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
            DataContext = new RegisterViewModel();
        }
    }
}