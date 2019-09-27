using CoreApp.BusinessModels;
using CoreApp.IServices;
using DesktopApp.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopApp.Models
{
    public class EnrolmentUpdateableModel : UpdateableModelBase
    {
        private Grade _finalGrade;
        public Grade FinalGrade 
        {
            get => _finalGrade;
            set => SetProperty(ref _finalGrade, value);
        }

        [DependsOnProperty(nameof(FinalGrade))]
        public string GradeDescription
        {
            get => FinalGrade.GetEnumDescription();
        }

        private EnrolmentUpdateList _enrolment;
        public EnrolmentUpdateList Enrolment
        {
            get => _enrolment;
            set => SetProperty(ref _enrolment, value);
        }
    }
}
