using BrainStormEra_WPF.ViewModel;
using BrainStormEra_WPF.ViewModel.Course.Chapter.Lesson;
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

namespace BrainStormEra_WPF.Lesson
{
    /// <summary>
    /// Interaction logic for ManagementLesson.xaml
    /// </summary>
    public partial class ManagementLesson : Page
    {
        public ManagementLesson(string chapterId)
        {
            InitializeComponent();
            DataContext = new LessonViewModel(chapterId);
        }




    }
}
