using CoreApp.BusinessModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreApp.IServices
{
    public interface ICourseService
    {
        Task<bool> CourseInstanceExists(int courseId, int semesterId);
        Task<List<CourseList>> GetList();
        Task<List<CourseInstanceList>> GetInastancesList();
        Task<List<CourseInstanceList>> GetInastancesListForCourse(int courseId);
        Task<List<CourseUpdate>> GetUpdateable();
        Task<List<CourseUpdateModel>> GetUpdateableModels();
        Task<List<CourseInstanceUpdate>> GetInstancesUpdateable();
        Task<CourseBase> GetById(int courseId);
        Task<CourseUpdate> CreateBasic(CourseCreate model);
        Task CreateInstance(int courseId, int semesterId);
        Task<CourseUpdate> UpdateBasic(int courseId, CourseUpdate model);
        Task Delete(int courseId);
        Task DeleteWithInstances(int courseId);
        Task DeleteInstance(int courseId, int semesterId);
    }
}
