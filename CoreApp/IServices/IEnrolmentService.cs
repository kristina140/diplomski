using CoreApp.BusinessModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreApp.IServices
{
    public interface IEnrolmentService
    {
        Task<List<EnrolmentBase>> GetAll();
        Task<List<EnrolmentList>> GetForCourseInstance(int courseId, int semesterId);
        Task<List<EnrolmentList>> GetForCourseInstance(CourseInstanceBase courseInstance);
        Task<List<EnrolmentList>> GetAvailableForCourseInstance(CourseInstanceBase courseInstance, int examId);
        Task<List<EnrolmentUpdateList>> GetUpdateable();
        Task<EnrolmentUpdate> Create(EnrolmentCreate model);
        Task<EnrolmentUpdate> CreateBase(EnrolmentBaseCreate model);
        Task<List<EnrolmentUpdate>> CreateSafe(List<EnrolmentCreate> models);
        Task UpdateAndCreate(List<EnrolmentCreate> models);
        Task<EnrolmentUpdate> Update(int enrolmentId, EnrolmentUpdate model);
        Task Delete(int enrolmentId);
    }
}
