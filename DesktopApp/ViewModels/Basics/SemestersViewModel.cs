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
    public class SemestersViewModel : UpdateableViewModelBase<SemesterUpdateableModel, SemesterCreateModel>
    {
        private readonly ISemesterService _semesterService;

        public SemestersViewModel(
            ISemesterService semesterService)
        {
            _semesterService = semesterService;
        }

        public List<SemesterUpdate> Semesters { get; set; }

        public async Task Load()
        {
            Semesters = await _semesterService.GetUpdateable();
            Semesters.ForEach(_ => _.AcademicYear = SemesterConverterService.GetAcademicYear(_.StartDate, _.IsWinter));

            ItemsObservable = new ObservableCollection<SemesterUpdateableModel>(
                Semesters.Select(_ => Mapper.MapSemesterUpdateableModel(_)));

        }

        protected override async void OnDeleteItem(object commandParameter)
        {
            ValidationErrors = null;
            if (SelectedItem != null && SelectedItemIndex.HasValue)
            {
                try
                {
                    await _semesterService.Delete(SelectedItem.Id);

                    Semesters.RemoveAt(SelectedItemIndex.Value);
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
                var oldValues = Semesters.FirstOrDefault(_ => SelectedItem.Id == _.Id);

                ItemsObservable[SelectedItemIndex.Value] = Mapper.MapSemesterUpdateableModel(oldValues);
            }
        }

        protected override void OnStartAdd(object commandParameter)
        {
            InAddMode = true;
            NewItem = new SemesterCreateModel
            {
                StartDate = DateTime.Now
            };
        }

        protected override async void OnSaveAdd(object commandParameter)
        {
            ValidationErrors = null;
            try
            {
                await _semesterService.Create(Mapper.MapSemesterCreate(NewItem));

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
                    var newValues = await _semesterService.UpdateBasic(SelectedItem.Id, Mapper.MapSemesterUpdate(SelectedItem));

                    Semesters[SelectedItemIndex.Value] = newValues;

                    ItemsObservable[SelectedItemIndex.Value].MapSemesterUpdateableModel(newValues);
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
