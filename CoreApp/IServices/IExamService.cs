using CoreApp.BusinessModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreApp.IServices
{
    public interface IExamService
    {
        Task<Guid> CreateOralExam(OralExamCreate oralExamForm);
        Task<bool> HasDate(int examId);
        Task<List<ExamList>> GetOralExamsListForCourseInstance(int courseId, int semesterId);
        Task<List<ExamUpdateList>> GetUpdateable();
        Task<ExamUpdate> Update(int examId, ExamUpdate model);
        Task Create(ExamCreate model);
        Task Delete(int examId);
        Task ForceDelete(int examId);
    }
}
