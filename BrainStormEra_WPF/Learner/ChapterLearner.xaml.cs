using BrainStormEra_WPF.Lesson;
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

namespace BrainStormEra_WPF.Learner
{
    /// <summary>
    /// Interaction logic for ChapterLearner.xaml
    /// </summary>
    public partial class ChapterLearner : Page
    {
        public ChapterLearner()
        {
            InitializeComponent();
        }

        private void Lesson_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ChapterViewModel;
            if (viewModel?.SelectedChapter == null) return;

            // Lấy chapterId từ chapter đang chọn
            string chapterId = viewModel.SelectedChapter.ChapterId;

            // Tạo instance của trang ManagementLesson và truyền chapterId vào constructor
            var lessonPage = new ManagementLessonLearner(chapterId);

            // Điều hướng đến trang ManagementLesson
            NavigationService.GetNavigationService(this)?.Navigate(lessonPage);
        }
    }
}
