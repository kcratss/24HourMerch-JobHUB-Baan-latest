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

    public class DecorationCostService : IDecorationCostService
    {
        private readonly IRepository<tblDecorationCost> _decorationCost;
        private readonly IRepository<tblCommonData> _CommonData;
        KENNEWEntities dbContext = new KENNEWEntities();
        ResponseViewModel response = new ResponseViewModel();
        // GET: DecorationCost
        public DecorationCostService(IRepository<tblDecorationCost> decorationCost, IRepository<tblCommonData> CommonData)
        {
            _decorationCost = decorationCost;
            _CommonData = CommonData;

        }
        public bool Add(tblDecorationCost entity)
        {
            throw new NotImplementedException();
        }

        public ResponseViewModel BatchTransaction(tblDecorationCost entitity, BatchOperation operation)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<tblDecorationCost> GetAll()
        {
            throw new NotImplementedException();
        }

        public tblDecorationCost GetById(int id)
        {
            throw new NotImplementedException();
        }

        // 03 Oct 2018 (N)
        //public List<tblDecorationCost> GetDecorationCostList()
        //{
        //    return _decorationCost.Get().ToList();
        //}
        // 03 Oct 2018 (N)

        public ResponseViewModel Update(tblDecorationCost entity)
        {
            throw new NotImplementedException();
        }
        public ResponseViewModel DecorationBatchTransaction(tblDecorationCost Entity, BatchOperation operation)

        {
            try
            {

                
                switch (operation)
                {
                    case BatchOperation.Insert:
                        {

                            //KEN_DataAccess.tblDecorationCost Entity = new KEN_DataAccess.tblDecorationCost();
                            //Entity.Dec_Desc = Dec_Desc;
                            //Entity.Quantity = Quantity;
                            //Entity.Cost = Convert.ToDecimal(Cost);
                            //model.Cost= Convert.ToDecimal(model.Cost);

                            bool IsValid = true;
                            var data = _decorationCost.Get(_ => _.Dec_Desc == Entity.Dec_Desc && _.Quantity == Entity.Quantity).FirstOrDefault();
                            if (data != null)
                            {
                                IsValid = false;
                            }

                            if (IsValid)
                            {
                                Entity.CreatedBy = DataBaseCon.ActiveUser();
                                Entity.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                _decorationCost.Insert(Entity);
                                _decorationCost.Save();
                                response.Message = ResponseMessage.SuccessMessage;
                                response.ID = Entity.DecCostId;
                                response.Result = ResponseType.Success;
                            }
                            else
                            {
                                response.Message = "Cannot insert duplicate data, please insert unique values!!!";
                                response.Result = ResponseType.Error;
                                //response.tblName = TableNames.tblOption;
                                response.ErrorCode = 122;
                                //response.Message = "Cannot insert duplicate data, please insert unique values!!!";
                                //response.Result = ResponseType.Error;
                            }

                            break;
                        }
                    case BatchOperation.Delete:
                        {

                            var entity = _decorationCost.Get(_ => _.DecCostId == Entity.DecCostId).FirstOrDefault();
                            if (entity != null)
                            {
                                //entity.Dec_Desc = Entity.Dec_Desc;
                                //entity.Quantity = Entity.Quantity;
                                //entity.Cost = Convert.ToDecimal(Entity.Cost);
                                entity.Status = Entity.Status;
                                entity.UpdatedBy = DataBaseCon.ActiveUser();
                                entity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                _decorationCost.Update(entity);
                                _decorationCost.Save();
                                response.Message = ResponseMessage.SuccessMessage;
                                response.ID = entity.DecCostId;
                                response.Result = ResponseType.Success;
                            }
                            break;
                        }
                    default:
                        {
                            var entity = _decorationCost.Get(_ => _.DecCostId == Entity.DecCostId).FirstOrDefault();
                            if (entity != null)
                            {
                                entity.Dec_Desc = Entity.Dec_Desc;
                                entity.Quantity = Entity.Quantity;
                                entity.Cost = Convert.ToDecimal(Entity.Cost);
                                entity.Status = Entity.Status;
                                entity.UpdatedBy = DataBaseCon.ActiveUser();
                                entity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                _decorationCost.Update(entity);
                                _decorationCost.Save();
                            response.Message = ResponseMessage.SuccessMessage;
                            response.ID = entity.DecCostId;
                            response.Result = ResponseType.Success;
                            }
                            break;
                        }
                }
            
            }
            catch (Exception ex)
            {

            }
            return response;
        }
         public List<DecorationCostMasterViewModel> GetDecorationList(string Status,string Type)
         {
            
            List<DecorationCostMasterViewModel> decdata = new List<DecorationCostMasterViewModel>();
            if (Type == "Digital")
            {
                if (Status == "All")
                {
                    var data = _decorationCost.Get(_ => _.MainCategory == Type).ToList();
                    decdata = Mapper.Map<List<DecorationCostMasterViewModel>>(data).ToList();
                }
                else
                {
                    var data = _decorationCost.Get(_ => _.Status == Status && _.MainCategory == Type).ToList();
                    decdata = Mapper.Map<List<DecorationCostMasterViewModel>>(data).ToList();
                }
            }
            else if (Type == "Screen Print")
            {
                if (Status == "All")
                {
                    var data = _decorationCost.Get(_ => _.MainCategory == Type).ToList();
                    decdata = Mapper.Map<List<DecorationCostMasterViewModel>>(data).ToList();
                }
                else
                {
                    var data = _decorationCost.Get(_ => _.Status == Status && _.MainCategory == Type).ToList();
                    decdata = Mapper.Map<List<DecorationCostMasterViewModel>>(data).ToList();
                }
            }
            else if (Type == "Embroidery")
            {
                if (Status == "All")
                {
                    var data = _decorationCost.Get(_ => _.MainCategory == Type).ToList();
                    decdata = Mapper.Map<List<DecorationCostMasterViewModel>>(data).ToList();
                }
                else
                {
                    var data = _decorationCost.Get(_ => _.Status == Status && _.MainCategory == Type).ToList();
                    decdata = Mapper.Map<List<DecorationCostMasterViewModel>>(data).ToList();
                }
            }
            else
            {
                if (Status == "All")
                {
                    var data = _decorationCost.Get().ToList();
                    decdata = Mapper.Map<List<DecorationCostMasterViewModel>>(data).ToList();
                }
                else
                {
                    var data = _decorationCost.Get(_ => _.Status == Status).ToList();
                    decdata = Mapper.Map<List<DecorationCostMasterViewModel>>(data).ToList();
                }
            }

            return decdata;

         }

        //tarun 15/09/2018   Common Data View
        public List<tblCommonData> GetCommonDataList()
        {
            return _CommonData.Get().ToList();
        }
        //end

        //public List<DecorationCostExportViewModel> ExportData()
        //{
        //    var  data= Mapper.Map<List<DecorationCostExportViewModel>>(dbContext.Database.SqlQuery<DecorationCostExportViewModel>("exec Pro_DecorationExport").ToList()).ToList();
        //    return data;
        //}
    }

}