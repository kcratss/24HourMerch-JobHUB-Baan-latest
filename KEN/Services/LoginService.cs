using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KEN.Interfaces;
using KEN.Interfaces.Iservices;
using KEN_DataAccess;
using AutoMapper;
using KEN.Interfaces.Repository;
using KEN.Models;

namespace KEN.Services
{
    public class LoginService:ILoginService
    {
        private readonly IRepository<tbluser> _tblUsers;
        public LoginService(IRepository<tbluser> tblUsers)
        {
            _tblUsers = tblUsers;
        }

        public bool Add(tbluser entity)
        {
            throw new NotImplementedException();
        }

        public ResponseViewModel BatchTransaction(tbluser entitity, BatchOperation operation)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<tbluser> GetAll()
        {
            throw new NotImplementedException();
        }

        public tbluser GetById(int id)
        {
            throw new NotImplementedException();
        }

        public tbluser GetByUsername(string email, string hashed_password)
        {
            var data = _tblUsers.Get(x => x.email == email && x.hashed_password == hashed_password && x.status == "active").FirstOrDefault();
            return data;
        }

        public ResponseViewModel Update(tbluser entity)
        {
            throw new NotImplementedException();
        }
    }
}