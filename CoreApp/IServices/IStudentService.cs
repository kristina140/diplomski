using CoreApp.BusinessModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreApp.IServices
{
    public interface IStudentService
    {
        Task<List<StudentBase>> GetAll();
        Task<List<StudentList>> GetList();
        Task<List<StudentUpdate>> GetUpdateable();
        Task<StudentBase> GetById(int studentId);
        Task<StudentCard> GetStudentCard(int studentId);
        Task<StudentUpdate> Create(StudentCreate model);
        Task<List<StudentUpdate>> Create(List<StudentCreate> models);
        Task<StudentUpdate> UpdateBasic(int studentId, StudentUpdate model);
        Task Delete(int studentId);
    }
}
