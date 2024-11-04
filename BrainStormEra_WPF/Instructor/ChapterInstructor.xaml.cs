using BrainStormEra_WPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BrainStormEra_WPF.ViewModel.Course.Chapter;
namespace BrainStormEra_WPF.Instructor
{
    /// <summary>
    /// Interaction logic for ChapterIntructor.xaml
    /// </summary>
    public partial class ChapterInstructor : Page
    {
        public ChapterInstructor()
        {
            InitializeComponent();

            DataContext = new ChapterViewModel();
        }
    }
}
