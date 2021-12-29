using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KEN.Interfaces;
using KEN.Interfaces.Iservices;
using KEN_DataAccess;
using KEN.Interfaces.Repository;
using KEN.Models;
using AutoMapper;
using KEN.AppCode;
using System.Net.Mail;
using System.Net.Mime;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web.Mvc;

namespace KEN.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IRepository<tblPurchase> _tblPurchase;
        private readonly IRepository<tblPurchaseDetail> _tblPurchaseDetail;
        private readonly IRepository<tbloption> _tbloptionRepository;
        private readonly IRepository<Vw_tblOpportunity> _VW_tblOpportunityRepository;
        private readonly IRepository<Vw_tblPurchase> _VW_tblPurchase;
        private readonly IRepository<tblOrganisation> _tblOrganisation;
        private readonly IRepository<tblEmailContent> _tblEmailContentRepository;

        ResponseViewModel response = new ResponseViewModel();
        KENNEWEntities DbContext = new KENNEWEntities();
        public PurchaseService(IRepository<tblPurchase> tblPurchase, IRepository<tblPurchaseDetail> tblPurchaseDetail, IRepository<Vw_tblOpportunity> VW_tblOpportunityRepository, IRepository<tbloption> tbloptionRepository,IRepository<Vw_tblPurchase> VW_tblPurchase, IRepository<tblOrganisation> tblOrganisation, IRepository<tblEmailContent> tblEmailContent)
        {
            _tblPurchase = tblPurchase;
            _tblPurchaseDetail = tblPurchaseDetail;
            _VW_tblOpportunityRepository = VW_tblOpportunityRepository;
            _tbloptionRepository = tbloptionRepository;
            _VW_tblPurchase = VW_tblPurchase;
            _tblOrganisation = tblOrganisation;
            _tblEmailContentRepository = tblEmailContent;
        }

        public bool Add(tblPurchase entity)
        {
            throw new NotImplementedException();
        }

        public ResponseViewModel BatchTransaction(tblPurchase entitity, BatchOperation operation)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<tblPurchase> GetAll()
        {
            throw new NotImplementedException();
        }

        public tblPurchase GetById(int id)
        {
            throw new NotImplementedException();
        }

        public List<PurchaseDetailViewModel> GetPurchaseDetailOptionGrid(int PurchaseId)
        {
            var data = _tblPurchaseDetail.Get(_ => _.PurchaseId == PurchaseId);
            var OptionData = Mapper.Map<List<PurchaseDetailViewModel>>(data).ToList();
            return OptionData;
        }

        public ResponseViewModel Update(tblPurchase entity)
        {
            throw new NotImplementedException();
        }
        //tarun 27/08/2018
        public tblPurchase GetPurchById(int Id)
        {
            return _tblPurchase.Get(_ => _.PurchaseId == Id).FirstOrDefault();
        }
        //end
        public Vw_tblOpportunity GetPurchEmailById(int OppId)
        {
            return _VW_tblOpportunityRepository.Get(_ => _.OpportunityId == OppId).FirstOrDefault();
        }
        public ResponseViewModel OptionBatchTransaction(tblPurchaseDetail Entity, BatchOperation operation)
        {
            try
            {

                switch (operation)
                {
                    case BatchOperation.Insert:
                        {
                            decimal? gst = 1;
                            var GSTdata = DbContext.tblOpportunities.Where(_ => _.OpportunityId == Entity.OpportunityId).FirstOrDefault();
                            if (GSTdata != null)
                                gst = GSTdata.GSTValue;

                            Entity.UnitInclGST = Math.Round((Convert.ToDecimal(Entity.uni_price) * Convert.ToDecimal(gst)), 2);
                            Entity.ExtExGST = Math.Round((Convert.ToDecimal(Entity.uni_price) * Convert.ToDecimal(Entity.quantity)), 2);
                            Entity.ExtInclGST = Math.Round((Convert.ToDecimal(Entity.uni_price) * Convert.ToDecimal(Entity.quantity) * Convert.ToDecimal(gst)), 2);
                            Entity.CreatedBy = DataBaseCon.ActiveUser();
                            Entity.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));

                            _tblPurchaseDetail.Insert(Entity);
                            _tblPurchaseDetail.Save();
                            response.Message = ResponseMessage.SuccessMessage;
                            response.ID = Entity.PurchaseDetailId;
                            response.Result = ResponseType.Success;
                            response.tblName = TableNames.tblOption;

                            break;
                        }
                    case BatchOperation.Delete:
                        {

                            var entity = _tblPurchaseDetail.Get(_ => _.PurchaseDetailId == Entity.PurchaseDetailId).FirstOrDefault();
                            if (entity != null)
                            {
                                _tblPurchaseDetail.Delete(entity);
                                _tblPurchaseDetail.Save();
                            }
                            response.Message = ResponseMessage.SuccessMessage;
                            response.ID = entity.PurchaseDetailId;
                            response.Result = ResponseType.Success;
                            response.tblName = TableNames.tblOption;
                            break;
                        }
                    default:
                        {
                            var entity = _tblPurchaseDetail.Get(_ => _.PurchaseDetailId == Entity.PurchaseDetailId).FirstOrDefault();
                            if (entity != null)
                            {
                                decimal? gst = 1;
                                var GSTdata = DbContext.tblOpportunities.Where(_ => _.OpportunityId == Entity.OpportunityId).FirstOrDefault();
                                if (GSTdata != null)
                                    gst = GSTdata.GSTValue;


                                //entity.Back_decCost = Entity.Back_decCost;
                                //entity.Back_decDesign = Entity.Back_decDesign;
                                //entity.back_decoration = Entity.back_decoration;
                                //entity.Back_decQuantity = Entity.Back_decQuantity;
                                entity.band_id = Entity.band_id;
                                entity.code = Entity.code;
                                entity.colour = Entity.colour;
                                entity.comment = Entity.comment;
                                entity.Cost = Entity.Cost;

                                //entity.Extra_decCost = Entity.Extra_decCost;
                                //entity.Extra_decDesign = Entity.Extra_decDesign;
                                //entity.extra_decoration = Entity.extra_decoration;
                                //entity.Extra_decQuantity = Entity.Extra_decQuantity;
                                //entity.Front_decCost = Entity.Front_decCost;
                                //entity.Front_decDesign = Entity.Front_decDesign;
                                //entity.front_decoration = Entity.front_decoration;
                                //entity.Front_decQuantity = Entity.Front_decQuantity;
                                entity.include_job = Entity.include_job;
                                entity.InitialSizes = Entity.InitialSizes;
                                entity.item_id = Entity.item_id;
                                //entity.Left_decCost = Entity.Left_decCost;
                                //entity.Left_decDesign = Entity.Left_decDesign;
                                //entity.left_decoration = Entity.left_decoration;
                                //entity.Left_decQuantity = Entity.Left_decQuantity;
                                entity.Link = Entity.Link;
                                //entity.Margin = Entity.Margin;
                                //TARUN 08/31/2018
                                entity.ItemNotes = Entity.ItemNotes;
                               
                                //END
                                entity.quantity = Entity.quantity;
                                //entity.Right_decCost = Entity.Right_decCost;
                                //entity.Right_decDesign = Entity.Right_decDesign;
                                //entity.right_decoration = Entity.right_decoration;
                                //entity.Right_decQuantity = Entity.Right_decQuantity;
                                //entity.Service = Entity.Service;
                                entity.SizeGrid = Entity.SizeGrid;
                                entity.uni_price = Entity.uni_price;
                                entity.UpdatedBy = DataBaseCon.ActiveUser();
                                entity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                entity.UnitInclGST = Math.Round((Convert.ToDecimal(Entity.uni_price) * Convert.ToDecimal(gst)), 2);
                                entity.ExtExGST = Math.Round((Convert.ToDecimal(Entity.uni_price) * Convert.ToDecimal(Entity.quantity)), 2);
                                entity.ExtInclGST = Math.Round((Convert.ToDecimal(Entity.uni_price) * Convert.ToDecimal(Entity.quantity) * Convert.ToDecimal(gst)), 2);
                                _tblPurchaseDetail.Update(entity);
                                _tblPurchaseDetail.Save();

                            }
                            response.Message = ResponseMessage.SuccessMessage;
                            response.ID = entity.PurchaseDetailId;
                            response.Result = ResponseType.Success;
                            response.tblName = TableNames.tblOption;

                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Result = ResponseType.Error;
                response.tblName = TableNames.tblOption;
                response.ErrorCode = ex.HResult;
            }

            return response;
        }
        // tarun change 05th September

        // baans end 05th September
        public ResponseViewModel PurchaseBatchTransaction(tblPurchase Entity, BatchOperation operation)
        {
            try
            {
                switch (operation)
                {
                    case BatchOperation.Insert:
                        {
                            Entity.CreatedBy = DataBaseCon.ActiveUser();
                            Entity.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                            _tblPurchase.Insert(Entity);
                            _tblPurchase.Save();

                            response.Message = ResponseMessage.SuccessMessage;
                            response.ID = Entity.PurchaseId;
                            //response.OppId = Entity.OpportunityId;

                            var optiontopurchase = Mapper.Map<List<PurchaseDetailViewModel>>(DbContext.Database.SqlQuery<PurchaseDetailViewModel>("exec pro_optiontopurchasedetail '" + Entity.OpportunityId + "','" + Entity.PurchaseId + "'").ToList()).ToList();

                            response.Result = ResponseType.Success;
                            response.tblName = TableNames.tblPurchase;
                            break;
                        }
                    default:
                        {
                            var entity = _tblPurchase.Get(_ => _.PurchaseId == Entity.PurchaseId).FirstOrDefault();
                            //tarun 4/9/2018
                            entity.QuantityRequired = Entity.QuantityRequired;
                            entity.Depts = Entity.Depts;
                            entity.Purchasedate = Entity.Purchasedate;
                            entity.RequiredByDate = Entity.RequiredByDate;
                            entity.ShippingIn = Entity.ShippingIn;
                            entity.ShippingCharge = Entity.ShippingCharge;
                            entity.PurchStatus = Entity.PurchStatus;
                            entity.BillNo = Entity.BillNo;
                            entity.BillDate = Entity.BillDate;
                            entity.PurchaseNotes = Entity.PurchaseNotes;
                            entity.WebOrderNo = Entity.WebOrderNo;
                            entity.DeliveryToId = Entity.DeliveryToId;
                            entity.UpdatedBy = DataBaseCon.ActiveUser();
                            entity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                            _tblPurchase.Update(entity);
                            _tblPurchase.Save();

                        }
                        response.Message = ResponseMessage.SuccessMessage;
                        response.ID = Entity.PurchaseId;
                        response.Result = ResponseType.Success;
                        response.tblName = TableNames.tblPurchase;

                        break;

                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Result = ResponseType.Error;
                response.ErrorCode = ex.HResult;
                response.tblName = TableNames.tblPurchase;
            }
            return response;
        }
        
        //public List<PurchaseDetailViewModel> GetOptionToPurchaseDetail(int PurchaseId, int OppId)
        //{
        //    //  var ddd = DbContext.Pro_Search(TableName, CustomText);
        //    var optiontopurchase = Mapper.Map<List<PurchaseDetailViewModel>>(DbContext.Database.SqlQuery<PurchaseDetailViewModel>("exec pro_optiontopurchasedetail '" + PurchaseId + "','" + OppId + "'").ToList()).ToList();
        //    //  DbContext.Pro_Search
        //    return optiontopurchase;
        //}

        //public tblPurchase GetSupplierByPurchaseId(int PurchaseId)
        //{
        //    var data = _tblPurchase.Get(_ => _.PurchaseId == PurchaseId).FirstOrDefault();
        //    return data;
        //}
        //Prashant
        public IEnumerable<OrganisationViewModel> GetSupplierName()
        {
            //List<PurchaseViewModel> model = new List<PurchaseViewModel>();
            //model = Mapper.Map<List<PurchaseViewModel>>(_tblPurchase.Get().ToList());
            //return model;
            var data = _tblOrganisation.Get(_ => _.OrganisationType == "Supplier");
            var SupplierName = data.Select(item => new OrganisationViewModel { OrgName = item.OrgName }).ToList();
            return SupplierName;
        }

        List<PurchaseViewModel> IPurchaseService.GetPurchaseList(string PurchaseTabs, string DateFrom, string DateTo, string Supplier)
        {
            var FromDate = Convert.ToDateTime(DateFrom);
            var ToDate = Convert.ToDateTime(DateTo);

            List<PurchaseViewModel> PurchaseList = new List<PurchaseViewModel>();

            if (Supplier != null && Supplier != "")
            {
                if (PurchaseTabs == "All")
                {
                    PurchaseList = Mapper.Map<List<PurchaseViewModel>>(_VW_tblPurchase.Get(_ => _.Purchasedate >= FromDate && _.Purchasedate <= ToDate && _.OrgName == Supplier).OrderByDescending(_ => _.PurchaseId));
                }
                else if (PurchaseTabs == "PurchaseOrders")
                {
                    PurchaseList = Mapper.Map<List<PurchaseViewModel>>(_VW_tblPurchase.Get(_ => _.Purchasedate >= FromDate && _.Purchasedate <= ToDate && _.PurchStatus == "Ordered" && _.OrgName == Supplier).OrderByDescending(_ => _.PurchaseId));
                }
                else if (PurchaseTabs == "OpenBills")
                {
                    PurchaseList = Mapper.Map<List<PurchaseViewModel>>(_VW_tblPurchase.Get(_ => _.Purchasedate >= FromDate && _.Purchasedate <= ToDate && _.BillNo == null && _.OrgName == Supplier && _.PurchStatus != "Deleted").OrderByDescending(_ => _.PurchaseId));
                }
                else if (PurchaseTabs == "ClosedBills")
                {
                    PurchaseList = Mapper.Map<List<PurchaseViewModel>>(_VW_tblPurchase.Get(_ => _.Purchasedate >= FromDate && _.Purchasedate <= ToDate && _.BillNo != null && _.OrgName == Supplier && _.PurchStatus == "Billed").OrderByDescending(_ => _.PurchaseId));
                }
                else if (PurchaseTabs == "Deleted")
                {
                    PurchaseList = Mapper.Map<List<PurchaseViewModel>>(_VW_tblPurchase.Get(_ => _.Purchasedate >= FromDate && _.Purchasedate <= ToDate && _.PurchStatus == "Deleted" && _.OrgName == Supplier).OrderByDescending(_ => _.PurchaseId));
                }
            }
            else
            {
                if (PurchaseTabs == "All")
                {
                    PurchaseList = Mapper.Map<List<PurchaseViewModel>>(_VW_tblPurchase.Get(_ => _.Purchasedate >= FromDate && _.Purchasedate <= ToDate).OrderByDescending(_ => _.PurchaseId));
                }
                else if (PurchaseTabs == "PurchaseOrders")
                {
                    PurchaseList = Mapper.Map<List<PurchaseViewModel>>(_VW_tblPurchase.Get(_ => _.Purchasedate >= FromDate && _.Purchasedate <= ToDate && _.PurchStatus == "Ordered").OrderByDescending(_ => _.PurchaseId));
                }
                else if (PurchaseTabs == "OpenBills")
                {
                    PurchaseList = Mapper.Map<List<PurchaseViewModel>>(_VW_tblPurchase.Get(_ => _.Purchasedate >= FromDate && _.Purchasedate <= ToDate && _.BillNo == null && _.PurchStatus != "Deleted").OrderByDescending(_ => _.PurchaseId));
                }
                else if (PurchaseTabs == "ClosedBills")
                {
                    PurchaseList = Mapper.Map<List<PurchaseViewModel>>(_VW_tblPurchase.Get(_ => _.Purchasedate >= FromDate && _.Purchasedate <= ToDate && _.BillNo != null && _.PurchStatus == "Billed").OrderByDescending(_ => _.PurchaseId));
                }
                else if (PurchaseTabs == "Deleted")
                {
                    PurchaseList = Mapper.Map<List<PurchaseViewModel>>(_VW_tblPurchase.Get(_ => _.Purchasedate >= FromDate && _.Purchasedate <= ToDate && _.PurchStatus == "Deleted").OrderByDescending(_ => _.PurchaseId));
                }

            }
            return PurchaseList;
        }
        //Prashant
        //8 Sep 2018 (N)
        public List<PurchaseViewModel> GetCustomPurchaseList(string CustomText, string TableName)
        {
            //  var ddd = DbContext.Pro_Search(TableName, CustomText);
            var CustomOppData = Mapper.Map<List<PurchaseViewModel>>(DbContext.Database.SqlQuery<Vw_tblPurchase>("exec Pro_Search '" + TableName + "','" + CustomText + "'").ToList()).ToList();
            //  DbContext.Pro_Search
            return CustomOppData;
        }
        //8 Sep 2018 (N)
        public EmailContentViewModel GetPurchaseEmailContent()
        {
            var EmailContent = Mapper.Map<EmailContentViewModel>(_tblEmailContentRepository.Get(_ => _.Purpose == "PurchaseOrder").FirstOrDefault());
            return EmailContent;
        }
        public tblPurchase GetPurchaseByOpportunityId(int OpportunityId)
        {
            return _tblPurchase.Get(_ => _.OpportunityId == OpportunityId).FirstOrDefault();
        }

        public ResponseViewModel SetPurchaseStatus(int PurchaseId)
        {
            var entity = _tblPurchase.Get(_ => _.PurchaseId == PurchaseId).FirstOrDefault();

            if(entity != null)
            {
                entity.PurchStatus = "Billed";

                _tblPurchase.Update(entity);
                _tblPurchase.Save();

                response.Message = ResponseMessage.SuccessMessage;
                response.ID = entity.PurchaseId;
                response.Result = ResponseType.Success;
                response.tblName = TableNames.tblPurchase;
            }
            return response;
        }
    }
}