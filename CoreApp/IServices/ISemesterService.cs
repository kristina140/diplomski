using CoreApp.BusinessModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreApp.IServices
{
    public interface ISemesterService
    {
        Task<List<SemesterBase>> GetAll();
        Task<List<SemesterList>> GetList();
        Task<List<SemesterUpdate>> GetUpdateable();
        Task<SemesterBase> GetById(int semesterId);
        Task<SemesterUpdate> Create(SemesterCreate model);
        Task<List<SemesterUpdate>> Create(List<SemesterCreate> models);
        Task<SemesterUpdate> UpdateBasic(int semesterId, SemesterUpdate model);
        Task Delete(int semesterId);
    }
}
