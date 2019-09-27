using CoreApp;
using CoreApp.BusinessModels;
using CoreApp.IServices;
using DesktopApp.Commands;
using DesktopApp.Models;
using DesktopApp.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace DesktopApp.ViewModels.Basics
{
    public class CoursesViewModel : UpdateableViewModelBase<CourseUpdateableModel, CourseCreate>
    {
        private readonly ICourseService _courseService;

        public CoursesViewModel(ICourseService courseService)
        {
            _courseService = courseService;

            _toggleDetailsCommand = new DelegateCommand(OnToggleDetails, CanToggleDetails);
        }

        public List<CourseUpdateModel> Courses { get; set; }
        public ListCollectionView CoursesView { get; set; }


        private bool _toggleDetails = false;
        public bool ToggleDetails
        {
            get => _toggleDetails;
            set => SetProperty(ref _toggleDetails, value);
        }

        private readonly DelegateCommand _toggleDetailsCommand;
        public ICommand ToggleDetailsCommand => _toggleDetailsCommand;


        public async Task Load()
        {
            Courses = await _courseService.GetUpdateableModels();
            CoursesView = new ListCollectionView(Courses);
            ItemsObservable = new ObservableCollection<CourseUpdateableModel>(
                Courses.Select(_ => Mapper.MapCourseUpdateableModel(_)));

        }


        protected override async void OnDeleteItem(object commandParameter)
        {
            ValidationErrors = null;
            if (SelectedItem != null && SelectedItemIndex.HasValue)
            {
                try
                {
                    await _courseService.DeleteWithInstances(SelectedItem.Id);

                    Courses.RemoveAt(SelectedItemIndex.Value);
                    ItemsObservable.Remove(SelectedItem);
                }
                catch (ValidationPropertyException vpe)
                {
                    ValidationErrors = vpe.ErrorsList.ToList();
                }
                catch (ValidationException ve)
                {
                    ValidationErrors = ve.Errors.ToList();
                }
            }
        }


        protected override void OnDiscardEdit(object commandParameter)
        {
            if (SelectedItemIndex.HasValue)
            {
                var oldValues = Courses.FirstOrDefault(_ => SelectedItem.Id == _.Id);

                ItemsObservable[SelectedItemIndex.Value] = Mapper.MapCourseUpdateableModel(oldValues);
            }
        }

        protected override async void OnSaveEdit(object commandParameter)
        {
            if(SelectedItem != null && SelectedItemIndex.HasValue)
            {
                ValidationErrors = null;
                try
                {
                    var newValues = await _courseService.UpdateBasic(SelectedItem.Id, Mapper.MapCourseUpdate(SelectedItem));

                    Courses[SelectedItemIndex.Value].Course = newValues;

                    ItemsObservable[SelectedItemIndex.Value].MapCourseUpdateableModel(newValues);
                    ItemsObservable[SelectedItemIndex.Value].InEditMode = false;

                    ValidationErrors = null;
                }
                catch (ValidationPropertyException vpe)
                {
                    ValidationErrors = vpe.ErrorsList.ToList();
                }
                catch (ValidationException ve)
                {
                    ValidationErrors = ve.Errors.ToList();
                }
            }
        }

        protected override async void OnSaveAdd(object commandParameter)
        {
            ValidationErrors = null;
            try
            {
                await _courseService.CreateBasic(NewItem);

                await Load();

                OnDiscardAdd();
            }
            catch (ValidationPropertyException vpe)
            {
                ValidationErrors = vpe.ErrorsList.ToList();
            }
            catch (ValidationException ve)
            {
                ValidationErrors = ve.Errors.ToList();
            }
        }

        protected void OnToggleDetails(object commandParemeter = null)
        {
            ToggleDetails = !ToggleDetails;
        }
        protected bool CanToggleDetails(object commandParameter = null)
        {
            return true;
        }

    }
}
