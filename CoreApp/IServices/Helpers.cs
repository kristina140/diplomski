using CoreApp.BusinessModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CoreApp.IServices
{
    public static class Helpers
    {
        public static string GetEnumDescription(this Enum value)
        {
            return
                value
                    .GetType()
                    .GetMember(value.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DescriptionAttribute>()
                    ?.Description
                ?? value.ToString();
        }

        public static bool GetSemesterType(this DateTime startDate)
        {
            if (startDate == null)
                throw new ArgumentNullException("startDate");
            
            var winterSemesterStartMonths = Enum.GetValues(typeof(WinterSemesterStartMonth)).Cast<int>();
            var summerSemesterStartMonths = Enum.GetValues(typeof(SummerSemesterStartMonth)).Cast<int>();

            if (winterSemesterStartMonths.Contains(startDate.Month))
                return true; //winter
            else if (summerSemesterStartMonths.Contains(startDate.Month))
                return false; //summer
            else
                throw new ArgumentException("Invalid DateTime provided.");
        }

        public static bool? TryGetSemesterType(this DateTime startDate)
        {
            if (startDate == null)
                return null;

            var winterSemesterStartMonths = Enum.GetValues(typeof(WinterSemesterStartMonth)).Cast<int>();
            var summerSemesterStartMonths = Enum.GetValues(typeof(SummerSemesterStartMonth)).Cast<int>();

            if (winterSemesterStartMonths.Contains(startDate.Month))
                return true; //winter
            else if (summerSemesterStartMonths.Contains(startDate.Month))
                return false; //summer
            else
                return null;
        }

        public static string GetAcademicYear(this DateTime startDate)
        {
            var isWinter = startDate.GetSemesterType();

            return isWinter ?
                    string.Format(($"{startDate.Year}/{startDate.Year + 1}")) :
                    string.Format(($"{startDate.Year - 1}/{startDate.Year}"));
        }

        public static string TryGetAcademicYear(this DateTime startDate)
        {
            var isWinter = startDate.TryGetSemesterType();
            if (!isWinter.HasValue)
                return null;

            return isWinter.Value ?
                    string.Format(($"{startDate.Year}/{startDate.Year + 1}")) :
                    string.Format(($"{startDate.Year - 1}/{startDate.Year}"));
        }

        public static string GetAcademicYear(this DateTime startDate, bool IsWinter)
        {
            if (startDate == null)
                throw new ArgumentNullException("startDate");

            return IsWinter ?
                    string.Format(($"{startDate.Year}/{startDate.Year + 1}")) :
                    string.Format(($"{startDate.Year - 1}/{startDate.Year}"));
        }

        public static string TryGetAcademicYear(this DateTime startDate, bool IsWinter)
        {
            if (startDate == null)
                return null;

            return IsWinter ?
                    string.Format(($"{startDate.Year}/{startDate.Year + 1}")) :
                    string.Format(($"{startDate.Year - 1}/{startDate.Year}"));
        }

        public static string GetUserFriendlySemester(this DateTime startDate, bool? isWinter = null)
        {
            var semesterType = isWinter ?? startDate.TryGetSemesterType();
            if (!semesterType.HasValue)
                return null;

            return semesterType.Value ?
                   string.Format($"zimski {startDate.Year}/{startDate.Year + 1}") :
                   string.Format($"ljetni {startDate.Year - 1}/{startDate.Year}");
        }

        public static string GetUserFriendlyExam(DateTime? examDate, string courseName, DateTime semesterStartDate, bool? isWinter = null)
        {
            return examDate.HasValue ?
                string.Format($"{examDate.Value.ToString("dd.MM.yyyy hh:mm")} - {courseName} - {semesterStartDate.GetUserFriendlySemester(isWinter)}") :
                string.Format($"{courseName} - {semesterStartDate.GetUserFriendlySemester(isWinter)}");
        }

        public static string GetUserFriendlyExam(ExamType examType, string academicYear)
        {
            return string.Format($"{examType.GetEnumDescription()} {academicYear}");
        }

        public static string GetUserFriendlyStudent(string firstname, string lastname, string jmbag, string indexNmb)
        {
            return string.Format($"{firstname} {lastname} ({jmbag}) {(string.IsNullOrEmpty(indexNmb) ? string.Empty : indexNmb)}");
        }

        public static string GetDateDescription(this DateTime? date)
        {
            return date.HasValue ? date.Value.ToString("dd.MM.yyyy") : " - ";
        }
    }
}
