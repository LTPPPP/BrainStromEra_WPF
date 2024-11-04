using BrainStormEra_WPF.ViewModel;
using BrainStormEra_WPF.ViewModel.Course;
using BrainStormEra_WPF.ViewModel.Course.Chapter;
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
namespace BrainStormEra_WPF.Instructor
{
    /// <summary>
    /// Interaction logic for CourseIntructor.xaml
    /// </summary>
    public partial class CourseIntructor : Page
    {
        public CourseIntructor()
        {
            InitializeComponent();
            DataContext = new CourseViewModel();

        }

        private void ManageChapter_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as CourseViewModel;
            if (viewModel?.selectitem == null) return;

            var chapterPage = new ChapterInstructor();
            var chapterViewModel = new ChapterViewModel
            {
                CourseId = viewModel.selectitem.CourseId
            };
            chapterPage.DataContext = chapterViewModel;

            // Sử dụng NavigationService từ chính Page
            NavigationService.GetNavigationService(this)?.Navigate(chapterPage);
        }

    }
}
