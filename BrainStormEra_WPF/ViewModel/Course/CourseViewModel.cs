using BrainStormEra_WPF.Instructor;
using BrainStormEra_WPF.Models;
using BrainStormEra_WPF.Utilities;
using BrainStormEra_WPF.ViewModel.Course.Chapter;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace BrainStormEra_WPF.ViewModel
{
    public class CourseViewModel : BaseViewModel
    {
        public ObservableCollection<Models.Course> courselist { get; set; }
        public ICommand Addcomand { get; }
        public ICommand Searchcomand { get; }
        public ICommand Deletecomand { get; }
       


        public CourseViewModel()
        {
            newitem = new Models.Course();
            courselist = new ObservableCollection<Models.Course>();
            Load();
            Addcomand = new RelayCommand(Add);
            Searchcomand = new RelayCommand(Search);
            Deletecomand = new RelayCommand(Delete);
           
        }

        private void Delete(object obj)
        {
            throw new NotImplementedException();
        }

        private void Search(object obj)
        {
            throw new NotImplementedException();
        }

        private void Add(object obj)
        {
            throw new NotImplementedException();
        }

        private void Load()
        {

            using (var context = new PrnDbFpContext())
            {
                var item = context.Courses.ToList();
                courselist = new ObservableCollection<Models.Course>(item);
            }
        }

        private Models.Course _newitem;
        public Models.Course newitem
        {
            get { return _newitem; }
            set
            {
                _newitem = value;
                OnPropertyChanged(nameof(newitem));
            }
        }


        private Models.Course _selectitem;
        public Models.Course selectitem
        {
            get { return _selectitem; }
            set
            {
                _selectitem = value;
                OnPropertyChanged(nameof(selectitem));

                if (_selectitem != null)
                {
                    newitem.CourseId = _selectitem.CourseId;
                    newitem.CourseName = _selectitem.CourseName;
                    newitem.CourseDescription = _selectitem.CourseDescription;
                    newitem.CourseStatus = _selectitem.CourseStatus;
                    newitem.CoursePicture = _selectitem.CoursePicture;
                    newitem.Price = _selectitem.Price;
                    newitem.CourseCreatedAt = _selectitem.CourseCreatedAt;
                    newitem.CreatedBy = _selectitem.CreatedBy;

                    OnPropertyChanged(nameof(newitem));
                }
            }
        }


     

    }
}