using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using CoreApp.BusinessModels;
using CoreApp.IServices;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace CoreApp.Services
{
    public class ExportService : IExportService
    {
        private readonly string xslsxMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public File ExportStudentExams(StudentExamsExport data, string fileName = null)
        {
            byte[] content = null;

            using (var package = new ExcelPackage())
            {
                var workbook = package.Workbook.Worksheets.Add(data.Course + " - " + data.Semester);

                workbook.Cells[2, 1].Value = "Index";
                workbook.Cells[2, 2].Value = "JMBAG";
                workbook.Cells[2, 3].Value = "Ime";
                workbook.Cells[2, 4].Value = "Prezime";
                workbook.Cells[2, 5].Value = "Konačna ocjena";
                workbook.Cells[2, 6].Value = "Konačna ocjena";
                workbook.Cells[2, 7].Value = "Datum ocjene";

                //students header
                using (var range = workbook.Cells[2, 1, 2, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.CornflowerBlue);
                    range.Style.Font.Color.SetColor(Color.White);
                }

                //students
                for (int i = 0; i < data.Students.Count; i++)
                {
                    var enrolment = data.Students[i];

                    workbook.Cells[i + 3, 1].Value = enrolment.Student.IndexNmb;
                    workbook.Cells[i + 3, 2].Value = enrolment.Student.Jmbag;
                    workbook.Cells[i + 3, 3].Value = enrolment.Student.Firstname;
                    workbook.Cells[i + 3, 4].Value = enrolment.Student.Lastname;
                    workbook.Cells[i + 3, 5].Value = enrolment.FinalGrade.ConvertToGrade().GetEnumDescription();
                    workbook.Cells[i + 3, 6].Value = enrolment.FinalGrade;
                    workbook.Cells[i + 3, 7].Value = enrolment.FinalGradeDate.GetDateDescription();
                }

                //exams
                for (int i = 0; i < data.Exams.Count; i++)
                {
                    var exam = data.Exams[i];

                    workbook.Cells[1, 9 + i * 4, 1, 12 + i * 4].Value = exam.ExamDate.GetDateDescription();
                    workbook.Cells[1, 9 + i * 4, 1, 12 + i * 4].Merge = true;

                    workbook.Cells[2, 9 + i * 4].Value = Helpers.GetUserFriendlyExam(exam.ExamType, exam.Semestar.AcademicYear);
                    workbook.Cells[2, 10 + i * 4].Value = "Bodovi";
                    workbook.Cells[2, 11 + i * 4].Value = "Ocjena";
                    workbook.Cells[2, 12 + i * 4].Value = "Ocjena";

                    for (int j = 0; j < data.Students.Count; j++)
                    {
                        var studentId = data.Students[j].Student.Id;

                        if (data.StudentExams.TryGetValue(new Tuple<int, int>(exam.ExamId, studentId), out StudentExamExport studentExam))
                        {
                            workbook.Cells[j + 3, 9 + i * 4].Value = studentExam.Participated ? "Izašao na ispit" : "Nije pristupio";
                            workbook.Cells[j + 3, 10 + i * 4].Value = studentExam.Score;
                            workbook.Cells[j + 3, 11 + i * 4].Value = studentExam.Grade.ConvertToGrade().GetEnumDescription();
                            workbook.Cells[j + 3, 12 + i * 4].Value = studentExam.Grade;
                        }
                    }
                }

                //exam dates header
                using (var range = workbook.Cells[1, 9, 1, 12 + (data.Exams.Count - 1) * 4])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.CornflowerBlue);
                    range.Style.Font.Color.SetColor(Color.White);
                }

                //exams header
                using (var range = workbook.Cells[2, 9, 2, 12 + (data.Exams.Count - 1) * 4])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                    range.Style.Font.Color.SetColor(Color.White);
                }

                workbook.Cells.AutoFitColumns();

                package.Workbook.Properties.Title = "Popis studenata i ispita";

                content = package.GetAsByteArray();
            }

            return new File(content, string.IsNullOrEmpty(fileName) ? "ispiti.xlsx" : fileName , xslsxMimeType);
        }
    }
}
