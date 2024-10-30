using System.Windows;

namespace BrainStormEra_WPF
{
    public partial class ConfirmLogoutDialog : Window
    {
        public bool IsConfirmed { get; private set; }

        public ConfirmLogoutDialog()
        {
            InitializeComponent();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = true;
            this.Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            this.Close();
        }
    }
}
