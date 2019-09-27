using CoreApp;
using CoreApp.BusinessModels;
using CoreApp.IServices;
using DesktopApp.Models;
using DesktopApp.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopApp.ViewModels.Basics
{
    public class StudentsViewModel : UpdateableViewModelBase<StudentUpdateableModel, StudentCreate>
    {
        private readonly IStudentService _studentService;

        public StudentsViewModel(
            IStudentService studentService)
        {
            _studentService = studentService;
        }

        public List<StudentUpdate> Students { get; set; }

        public async Task Load()
        {
            Students = await _studentService.GetUpdateable();
            ItemsObservable = new ObservableCollection<StudentUpdateableModel>(
                Students.Select(_ => Mapper.MapStudentUpdateableModel(_)));

        }

        protected override async void OnDeleteItem(object commandParameter)
        {
            ValidationErrors = null;
            if (SelectedItem != null && SelectedItemIndex.HasValue)
            {
                try
                {
                    await _studentService.Delete(SelectedItem.Student.Id);

                    Students.RemoveAt(SelectedItemIndex.Value);
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
                var oldValues = Students.FirstOrDefault(_ => SelectedItem.Student.Id == _.Id);

                ItemsObservable[SelectedItemIndex.Value] = Mapper.MapStudentUpdateableModel(oldValues);
            }
        }


        protected override async void OnSaveAdd(object commandParameter)
        {
            ValidationErrors = null;
            try
            {
                await _studentService.Create(NewItem);

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

        protected override async void OnSaveEdit(object commandParameter)
        {
            if (SelectedItem != null && SelectedItemIndex.HasValue)
            {
                ValidationErrors = null;
                try
                {
                    var newValues = await _studentService.UpdateBasic(SelectedItem.Student.Id, Mapper.MapStudentUpdate(SelectedItem));

                    Students[SelectedItemIndex.Value] = newValues;

                    ItemsObservable[SelectedItemIndex.Value].MapStudentUpdateableModel(newValues);
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

    }
}
