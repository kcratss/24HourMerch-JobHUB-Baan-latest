using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KEN_DataAccess;
using KEN.Interfaces;
using KEN.Interfaces.Iservices;
using KEN.Interfaces.Repository;
using KEN.Models;
using AutoMapper;
using KEN.AppCode;

namespace KEN.Services
{
    public class CommonMastersService : ICommonMastersService
    {
        private readonly IRepository<tbldepartment> _tbldepartmentRepository;
        private readonly IRepository<tblband> _tblbrandRepository;
        private readonly IRepository<tblitem> _tblitemRepository;
        private readonly IRepository<tblOptionCode> _tblOptionCodeRepository;
        ResponseViewModel response = new ResponseViewModel();

        public CommonMastersService(IRepository<tbldepartment> tbldepartmentRepository, IRepository<tblband> tblbrandRepository, IRepository<tblitem> tblitemRepository,IRepository<tblOptionCode> tblOptionCodeRepository)
        {            
            _tbldepartmentRepository = tbldepartmentRepository;
            _tblbrandRepository = tblbrandRepository;
            _tblitemRepository = tblitemRepository;
            _tblOptionCodeRepository = tblOptionCodeRepository;
        }

        public bool Add(tblband entity)
        {
            throw new NotImplementedException();
        }

        public ResponseViewModel BatchTransaction(tblband entitity, BatchOperation operation)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<tblband> GetAll()
        {
            throw new NotImplementedException();
        }

        public tblband GetById(int id)
        {
            throw new NotImplementedException();
        }

        public ResponseViewModel Update(tblband entity)
        {
            throw new NotImplementedException();
        }
        public List<DepartmentViewModel> DepartmentListForMasters(string ddlstatusvalue)
        {
            List<DepartmentViewModel> data = new List<DepartmentViewModel>();

            if(ddlstatusvalue == "All")
            {
                var griddata = _tbldepartmentRepository.Get().ToList();
                data = Mapper.Map<List<DepartmentViewModel>>(griddata).ToList();
            }
            else if (ddlstatusvalue == "Active")
            {
                var griddata = _tbldepartmentRepository.Get(_ => _.Status == "Active").ToList();
                data = Mapper.Map<List<DepartmentViewModel>>(griddata).ToList();
            }
            else if (ddlstatusvalue == "InActive")
            {
                var griddata = _tbldepartmentRepository.Get(_ => _.Status == "InActive").ToList();
                data = Mapper.Map<List<DepartmentViewModel>>(griddata).ToList();
            }
            return data;
        }
        public List<BrandViewModel> BrandListForMasters(string ddlstatusvalue)
        {
            List<BrandViewModel> data = new List<BrandViewModel>();

            if (ddlstatusvalue == "All")
            {
                var griddata = _tblbrandRepository.Get().ToList();
                data = Mapper.Map<List<BrandViewModel>>(griddata).ToList();
            }
            else if (ddlstatusvalue == "Active")
            {
                var griddata = _tblbrandRepository.Get(_ => _.Status == "Active").ToList();
                data = Mapper.Map<List<BrandViewModel>>(griddata).ToList();
            }
            else if (ddlstatusvalue == "InActive")
            {
                var griddata = _tblbrandRepository.Get(_ => _.Status == "InActive").ToList();
                data = Mapper.Map<List<BrandViewModel>>(griddata).ToList();
            }

            return data;
        }
        public List<ItemViewModel> ItemsListForMasters(string ddlstatusvalue)
        {
            List<ItemViewModel> data = new List<ItemViewModel>();

            if (ddlstatusvalue == "All")
            {
                var griddata = _tblitemRepository.Get().ToList();
                data = Mapper.Map<List<ItemViewModel>>(griddata).ToList();
            }
            else if (ddlstatusvalue == "Active")
            {
                var griddata = _tblitemRepository.Get(_ => _.Status == "Active").ToList();
                data = Mapper.Map<List<ItemViewModel>>(griddata).ToList();
            }
            else if (ddlstatusvalue == "InActive")
            {
                var griddata = _tblitemRepository.Get(_ => _.Status == "InActive").ToList();
                data = Mapper.Map<List<ItemViewModel>>(griddata).ToList();
            }
            return data;
        }

        public List<OptionCodeBrandItemViewModel> OptionCodeListMasters()
        {
            List<OptionCodeBrandItemViewModel> data = new List<OptionCodeBrandItemViewModel>();
            var griddata = _tblOptionCodeRepository.Get().ToList();
            data = Mapper.Map<List<OptionCodeBrandItemViewModel>>(griddata).ToList();
            return data;
        }

        public ResponseViewModel MasterBatchTransaction(int id, string name, string status, string table, BatchOperation operation)
        {
            try
            {
                switch (operation)
                {
                    case BatchOperation.Insert:
                        {
                            if(table == "tbldepartment")
                            {
                                KEN_DataAccess.tbldepartment Entity = new KEN_DataAccess.tbldepartment();
                                var Depname = _tbldepartmentRepository.Get(_ => _.department == name).FirstOrDefault();
                                if (Depname == null)
                                {
                                    Entity.department = name;
                                    Entity.Status = status;
                                    Entity.CreatedBy = DataBaseCon.ActiveUser();
                                    Entity.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                    _tbldepartmentRepository.Insert(Entity);
                                    _tbldepartmentRepository.Save();
                                    response.Message = ResponseMessage.SuccessMessage;
                                    response.Result = ResponseType.Success;
                                    break;
                                }
                                response.Message = "Cannot insert duplicate value.please insert unique value!";
                                response.Result = ResponseType.Error;
                                break;
                            }
                            else if (table == "tblband")
                            {
                                KEN_DataAccess.tblband Entity = new KEN_DataAccess.tblband();
                                var Brandname = _tblbrandRepository.Get(_ => _.name == name).FirstOrDefault();
                                if (Brandname == null)
                                {
                                    Entity.name = name;
                                    Entity.Status = status;
                                    Entity.CreatedBy = DataBaseCon.ActiveUser();
                                    Entity.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                    _tblbrandRepository.Insert(Entity);
                                    _tblbrandRepository.Save();
                                    response.Message = ResponseMessage.SuccessMessage;
                                    response.Result = ResponseType.Success;
                                    break;
                                }
                                response.Message = "Cannot insert duplicate value.please insert unique value!";
                                response.Result = ResponseType.Error;
                                break;
                            }
                            else if(table == "tblitem")
                            {
                                KEN_DataAccess.tblitem Entity = new KEN_DataAccess.tblitem();
                                var Itemname = _tblitemRepository.Get(_ => _.name == name).FirstOrDefault();
                                if (Itemname == null)
                                {
                                    Entity.name = name;
                                    Entity.Status = status;
                                    Entity.CreatedBy = DataBaseCon.ActiveUser();
                                    Entity.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                    _tblitemRepository.Insert(Entity);
                                    _tblitemRepository.Save();
                                    response.Message = ResponseMessage.SuccessMessage;
                                    response.Result = ResponseType.Success;
                                    break;
                                }
                                response.Message = "Cannot insert duplicate value.please insert unique value!";
                                response.Result = ResponseType.Error;
                                break;
                            }
                            break;
                        }
                    case BatchOperation.Delete:
                        {
                            break;
                        }
                    default:
                        {
                            if(table == "tbldepartment")
                            {
                                var entity = _tbldepartmentRepository.Get(_ => _.id == id).FirstOrDefault();

                                entity.department = name;
                                entity.Status = status;
                                entity.UpdatedBy = DataBaseCon.ActiveUser();
                                entity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                _tbldepartmentRepository.Update(entity);
                                _tbldepartmentRepository.Save();
                            }
                            else if(table == "tblband")
                            {
                                var entity = _tblbrandRepository.Get(_ => _.id == id).FirstOrDefault();

                                entity.name = name;
                                entity.Status = status;
                                entity.UpdatedBy = DataBaseCon.ActiveUser();
                                entity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                _tblbrandRepository.Update(entity);
                                _tblbrandRepository.Save();
                            }
                            else if (table == "tblitem")
                            {
                                var entity = _tblitemRepository.Get(_ => _.id == id).FirstOrDefault();

                                entity.name = name;
                                entity.Status = status;
                                entity.UpdatedBy = DataBaseCon.ActiveUser();
                                entity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                _tblitemRepository.Update(entity);
                                _tblitemRepository.Save();
                            }
                            response.Message = ResponseMessage.SuccessMessage;
                            //response.ID = Entity.PurchaseId;
                            response.Result = ResponseType.Success;
                            //response.tblName = TableNames.tbl;
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Result = ResponseType.Error;
                response.ErrorCode = ex.HResult;
            }
            return response;
        }
        
        public ResponseViewModel OptionCodeTransaction(tblOptionCode model,BatchOperation operation)
        {
            try
            {
                switch (operation)
                {
                    case BatchOperation.Insert:
                        {
                            KEN_DataAccess.tblOptionCode Entity = new KEN_DataAccess.tblOptionCode();
                            var optionCode = _tblOptionCodeRepository.Get(_ => _.Code == model.Code).FirstOrDefault();
                            if (optionCode == null)
                            {
                                Entity.Code = model.Code;
                                Entity.itemId = model.itemId;
                                Entity.BrandId = model.BrandId;
                                Entity.Link = model.Link;
                                Entity.cost = model.cost;
                                Entity.Status = model.Status;
                                Entity.CreatedBy = DataBaseCon.ActiveUser();
                                Entity.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                _tblOptionCodeRepository.Insert(Entity);
                                _tblOptionCodeRepository.Save();
                                response.Message = ResponseMessage.SuccessMessage;
                                response.Result = ResponseType.Success;
                                break;
                            }
                            response.Message = "Cannot insert duplicate value.please insert unique value!";
                            response.Result = ResponseType.Error;
                            break;
                        }
                    case BatchOperation.Update:
                        {
                            var entity = _tblOptionCodeRepository.Get(_ => _.id == model.id).FirstOrDefault();

                            entity.Code = model.Code;
                            entity.itemId = model.itemId;
                            entity.BrandId = model.BrandId;
                            entity.Link = model.Link;
                            entity.cost = model.cost;
                            entity.Status = model.Status;
                            entity.UpdatedBy = DataBaseCon.ActiveUser();
                            entity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                            _tblOptionCodeRepository.Update(entity);
                            _tblOptionCodeRepository.Save();
                            response.Message = ResponseMessage.SuccessMessage;
                            response.Result = ResponseType.Success;
                            break;
                        }
                    case BatchOperation.Delete:
                        {
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Result = ResponseType.Error;
                response.ErrorCode = ex.HResult;
            }
            return response;
        }
    }
}