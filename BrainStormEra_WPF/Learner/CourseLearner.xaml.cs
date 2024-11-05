using BrainStormEra_WPF.Instructor;
using BrainStormEra_WPF.ViewModel;
using BrainStormEra_WPF.ViewModel.Course.Chapter;
using BrainStormEra_WPF.ViewModel.Learner;
using Microsoft.VisualBasic.ApplicationServices;
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

namespace BrainStormEra_WPF.Learner
{
    /// <summary>
    /// Interaction logic for CourseLearner.xaml
    /// </summary>
    public partial class CourseLearner : Page
    {
        public CourseLearner(string userId)
        {
            InitializeComponent();
            DataContext = new CourseLearnerViewModel(userId);
        }

        private void ManageChapter_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as CourseLearnerViewModel;
            if (viewModel?.selectitem == null) return;

            var chapterPage = new ChapterLearner();
            var chapterViewModel = new ChapterViewModel
            {
                CourseId = viewModel.selectitem.CourseId
            };
            chapterPage.DataContext = chapterViewModel;


            NavigationService.GetNavigationService(this)?.Navigate(chapterPage);
        }

        private void CustomerListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
