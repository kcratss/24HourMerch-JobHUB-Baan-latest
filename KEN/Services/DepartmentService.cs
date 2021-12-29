using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KEN_DataAccess;
using KEN.Interfaces;
using KEN.Interfaces.Iservices;
using KEN.Interfaces.Repository;
using KEN.Models;

namespace KEN.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IRepository<tbldepartment> _st_departmentRepository;

        public DepartmentService(IRepository<tbldepartment> st_departmentRepository)
        {
            _st_departmentRepository = st_departmentRepository;
        }
        public bool Add(tbldepartment entity)
        {
            throw new NotImplementedException();
        }

        public ResponseViewModel BatchTransaction(tbldepartment entitity, BatchOperation operation)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<tbldepartment> GetAll()
        {
            throw new NotImplementedException();
        }

        public tbldepartment GetById(int id)
        {
            throw new NotImplementedException();
        }

       
        public IEnumerable<tbldepartment> GetAllDepartmentList()
        {
            // baans change 19th September for Status Acive
             return _st_departmentRepository.Get();
           //return _st_departmentRepository.Get(_ => _.Status == "Active");
            // baans end 19th Sept
        }

        public ResponseViewModel Update(tbldepartment entity)
        {
            throw new NotImplementedException();
        }
    }
}