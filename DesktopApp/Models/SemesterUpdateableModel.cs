using CoreApp.BusinessModels;
using DesktopApp.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopApp.Models
{
    public class SemesterCreateModel : UpdateableModelBase
    {
        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        [DependsOnProperty(nameof(StartDate))]
        public bool IsWinter
        {
            get => SemesterConverterService.GetSemester(StartDate);
        }

        [DependsOnProperty(nameof(StartDate))]
        public string AcademicYear
        {
            get => SemesterConverterService.GetAcademicYear(StartDate);
        }
    }

    public class SemesterUpdateableModel : SemesterCreateModel
    {
        public SemesterUpdateableModel()
        {
        }

        private int _id;
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

    }
}
