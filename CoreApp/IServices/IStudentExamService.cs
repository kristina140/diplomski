using CoreApp.BusinessModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreApp.IServices
{
    public interface IStudentExamService
    {
        Task<ExistingStudentOralExam> GetStudentOralExam(int examId, int enrolmentId);
        Task<OralExamList> GetOralExam(Guid id);
        Task<List<StudentExamBaseList>> GetStudentExamList();
        Task<List<StudentExamList>> GetStudentExamList(int studentId, Guid studentExamId, int courseId);
        Task<List<StudentExamUpdateList>> GetStudentExamsForExam(int examId);
        Task<StudentExamsExport> GetStudentExamsExport(int courseId, int semesterId);
        Task Create(List<StudentExamCreate> models);
        Task Create(StudentExamsCreate model);
        Task UpdateOralExam(OralExamUpdate model);
        Task Update(StudentExamUpdate model);
        Task Update(List<StudentExamUpdate> models);
        Task Delete(int enrolmentId, int examId);
    }
}
