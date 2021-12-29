using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KEN.Interfaces;
using KEN.Interfaces.Iservices;
using KEN_DataAccess;
using KEN.Interfaces.Repository;
using AutoMapper;
using KEN.AppCode;
using KEN.Models;
using System.Text.RegularExpressions;

namespace KEN.Services
{
    public class QuickBooksService : IQuickBooksService
    {
        private readonly IRepository<tblCommonData> _tblCommonDataRepository;
        public QuickBooksService(IRepository<tblCommonData> tblCommonDataRepository)
        {
            _tblCommonDataRepository = tblCommonDataRepository;
        }

        public List<tblCommonData> AuthdataListByDesc()
        {
           return _tblCommonDataRepository.Get(_ => _.FieldDescription == "Authentication").ToList();
        }
        public bool UpdateTOken(string Token,string Type)
        {
            bool result = false;
            try
            {
               var tokendata= _tblCommonDataRepository.Get(_ => _.FieldName == Type).FirstOrDefault();
                if(tokendata!=null)
                {
                    tokendata.FieldValue = Token;
                    _tblCommonDataRepository.Update(tokendata);
                    _tblCommonDataRepository.Save();
                }
                
                result = true;
            }
            catch(Exception ex)
            {
                result = false;
            }


            return result;
        }
        public bool Add(tblCommonData entity)
        {
            throw new NotImplementedException();
        }

        public ResponseViewModel BatchTransaction(tblCommonData entitity, BatchOperation operation)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<tblCommonData> GetAll()
        {
            throw new NotImplementedException();
        }

        public tblCommonData GetById(int id)
        {
            throw new NotImplementedException();
        }

        public ResponseViewModel Update(tblCommonData entity)
        {
            throw new NotImplementedException();
        }
    }
}