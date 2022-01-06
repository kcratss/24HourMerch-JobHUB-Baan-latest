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
using System.Text.RegularExpressions;


namespace KEN.Services
{
    public class OpportunityService : IOpportunityService
    {
        private readonly IRepository<tblOpportunity> _tblOpportunityRepository;
        private readonly IRepository<Vw_tblOpportunity> _VW_tblOpportunityRepository;
        private readonly IRepository<tblOppContactMapping> _tblOppContactMappingRepository;
        private readonly IRepository<tbloption> _st_optionRepository;
        private readonly IRepository<tblcontact> _st_contactRepository;
        private readonly IRepository<tblDecorationCost> _tblDecorationCostRepository;
        //private readonly IRepository<tbldecoration> _st_DecorationRepository;     //13 July 2019 (N)
        private readonly IRepository<TblApplication> _tblApplication;       //13 July 2019 (N)
        private readonly IRepository<tblInquiry> _tblInquiryRepository;
        private readonly IRepository<tblEmailContent> _tblEmailContentRepository;
        private readonly IRepository<tblPayment> _tblPaymentRepository;
        private readonly IRepository<tblCommonData> _tblCommonData;
        private readonly IRepository<tbluser> _tblUsers;
        // baans change 13th Sept for New Brand
        private readonly IRepository<tblband> _tblBand;
        // baans end 13th Sept
        private readonly IRepository<tblitem> _tblItem;
        private readonly IRepository<tblPurchase> _tblPurchase;         //28 Aug 2018 (N)
        private readonly IRepository<tblPurchaseDetail> _tblPurchaseDetail;         //29 Aug 2018 (N)
        private readonly IRepository<tblOrganisation> _tblOrganisation;     //13 Nov 2018(N)
        private readonly IRepository<tblOptionCode> _tblOptionCodeRepository;     //10 Jan 2019(P)

        ResponseViewModel response = new ResponseViewModel();

        KENNEWEntities DbContext = new KENNEWEntities();

        public OpportunityService(IRepository<tblitem> tblItem,IRepository<tblOpportunity> tblOpportunityRepository, IRepository<tblOppContactMapping> tblOppContactMappingRepository, IRepository<tbloption> st_optionRepository, IRepository<tblcontact> st_contactRepository, IRepository<tblDecorationCost> tblDecorationCostRepository, /*IRepository<tbldecoration> st_DecorationRepository,*/ IRepository<tblInquiry> tblInquiryRepository, IRepository<Vw_tblOpportunity> VW_tblOpportunityRepository, IRepository<tblEmailContent> tblEmailContentRepository, IRepository<tblPayment> tblPaymentRepository, IRepository<tblCommonData> tblCommonData, IRepository<tbluser> tblUsers, IRepository<tblPurchase> tblPurchase, IRepository<tblPurchaseDetail> tblPurchaseDetail /*baans change 13th Sept*/, IRepository<tblband> tblBand /*baans end 13th Sept*/, IRepository<tblOrganisation> tblOrganisation/*13 Nov 2018 (N)*/, IRepository<tblOptionCode> tblOptionCodeRepository, IRepository<TblApplication> tblApplication)
        {
            _tblOpportunityRepository = tblOpportunityRepository;
            _tblOppContactMappingRepository = tblOppContactMappingRepository;
            _st_optionRepository = st_optionRepository;
            _st_contactRepository = st_contactRepository;
            _tblDecorationCostRepository = tblDecorationCostRepository;
            //_st_DecorationRepository = st_DecorationRepository;       //13 July 2019 (N)
            _tblApplication = tblApplication;       //13 July 2019 (N)
            _tblInquiryRepository = tblInquiryRepository;
            _VW_tblOpportunityRepository = VW_tblOpportunityRepository;
            _tblEmailContentRepository = tblEmailContentRepository;
            _tblPaymentRepository = tblPaymentRepository;
            _tblCommonData = tblCommonData;
            _tblUsers = tblUsers;
            _tblPurchase = tblPurchase;         //28 Aug 2018 (N)
            _tblPurchaseDetail = tblPurchaseDetail;         //29 Aug 2018 (N)
            // baans change 13th Sept for New Brand
            _tblBand = tblBand;
            // baans end 13th Sept
            _tblOrganisation = tblOrganisation;     //13 Nov 2018(N)
            _tblOptionCodeRepository = tblOptionCodeRepository;     //10 Jan 2019(P)
            _tblItem = tblItem;

        }

        public bool Add(tblOpportunity entity)
        {
            throw new NotImplementedException();
        }


        
        // baans change 13th Sept for New Brand in option
        public ResponseViewModel SaveNewBrand(tblband Entity, string OptionBrand)
        {
            if (OptionBrand == "")
            {
                response.Result = ResponseType.Warning;
                response.Message = "Please Fill the Brand Name";
                return response;
            }
            Entity.CreatedBy = DataBaseCon.ActiveUser();
            Entity.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
            Entity.name = OptionBrand;
            Entity.Status = "Active";
            _tblBand.Insert(Entity);
            _tblBand.Save();
            response.Message = ResponseMessage.SuccessMessage;
            response.ID = Entity.id;
            response.Result = Entity.name;
            return response;
            //var getData = DbContext.tblbands.ToList().OrderBy(_ => _.name);

            //return getData;
            //var data = DbContext.tblbands
        }
        // baans end 13th Sept

        public ResponseViewModel SaveNewItem(string itemName)
        {
            tblitem entity = new tblitem();
            var status = "Active";
            if (itemName == "")
            {
                response.Result = ResponseType.Warning;
                response.Message = "Please Fill the Item Name";
                return response;
            }
            entity.CreatedBy = DataBaseCon.ActiveUser();
            entity.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
            entity.name = itemName;
            entity.Status = status;
            _tblItem.Insert(entity);
            _tblItem.Save();
            response.Message = ResponseMessage.SuccessMessage;
            response.ID = entity.id;
            response.Result = entity.name;
            return response;
           
        }


        public ResponseViewModel OppBatchTransaction(tblOpportunity Entity, string PageSource, BatchOperation operation)
        {
            try
            {

                switch (operation)
                {
                    case BatchOperation.Insert:
                        {
                            decimal? gst = 0;
                            var GSTdata = DbContext.tblCommonDatas.Where(_ => _.FieldName == "Gst").FirstOrDefault();
                            if (GSTdata != null)
                            {
                                gst = Convert.ToDecimal(GSTdata.FieldValue);
                            }
                            if (gst > 0)
                            {
                                Entity.CreatedBy = DataBaseCon.ActiveUser();
                                Entity.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                Entity.Stage = "Opportunity";
                                if (Entity.RepeatJobId == 0)
                                    Entity.RepeatJobId = null;
                                Entity.GSTValue = gst;
                                // baans change 12th September for Opp Date
                                Entity.OppDate = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                // baans end 12th Sept
                                //Entity.Lost = null;
                                //Entity.Cancelled = null;
                                
                                _tblOpportunityRepository.Insert(Entity);

                                _tblOpportunityRepository.Save();

                                response.Message = ResponseMessage.SuccessMessage;
                                response.ID = Entity.OpportunityId;
                                response.Result = ResponseType.Success;
                                response.tblName = TableNames.tblOpportunity;
                            }
                            else
                            {
                                response.Message = "Add GST first";
                                response.ID = Entity.OpportunityId;
                                response.Result = ResponseType.Warning;
                                response.tblName = TableNames.tblOpportunity;
                            }

                            break;
                        }
                    case BatchOperation.Delete:
                        {

                            var entity = _tblOpportunityRepository.Get(_ => _.OpportunityId == Entity.OpportunityId).FirstOrDefault();
                            if (entity != null)
                            {
                                _tblOpportunityRepository.Delete(entity);
                                _tblOpportunityRepository.Save();
                            }
                            response.Message = ResponseMessage.SuccessMessage;
                            response.ID = entity.OpportunityId;
                            response.Result = ResponseType.Success;
                            response.tblName = TableNames.tblOpportunity;
                            break;
                        }
                    default:
                        {

                            var entity = _tblOpportunityRepository.Get(_ => _.OpportunityId == Entity.OpportunityId).FirstOrDefault();
                            if (entity != null)
                            {
                                switch (Entity.Stage)
                                {
                                    case "Opportunity":
                                        {
                                            entity.OppNotes = Entity.OppNotes;
                                            break;
                                        }
                                    case "Quote":
                                        {
                                            entity.QuoteNotes = Entity.QuoteNotes;
                                            break;
                                        }
                                    case "Order":
                                        {
                                            entity.OrderNotes = Entity.OrderNotes;
                                            break;
                                        }
                                    case "Job":
                                        {
                                            entity.JobNotes = Entity.JobNotes;
                                            break;
                                        }
                                    case "Packing":
                                        {
                                            entity.PackingNotes = Entity.PackingNotes;
                                            break;
                                        }
                                    case "Invoicing":
                                        {
                                            entity.InvoicingNotes = Entity.InvoicingNotes;
                                            break;

                                        }
                                    case "Shipping":
                                        {
                                            entity.ShippingNotes = Entity.ShippingNotes;
                                            break;
                                        }
                                    case "Complete":
                                        {
                                            entity.CompleteNotes = Entity.CompleteNotes;
                                            break;
                                        }
                                }
                                entity.OppName = Entity.OppName;
                                entity.Quantity = Entity.Quantity;
                                entity.ReqDate = Entity.ReqDate;
                                entity.Source = Entity.Source;
                                //  entity.EventId = Entity.EventId;
                                //Adding Job_department Feild to Get Department Ids
                                entity.job_department = Entity.job_department;
                                entity.DepartmentName = Entity.DepartmentName;
                                entity.Shipping = Entity.Shipping;
                                entity.AcctManagerId = Entity.AcctManagerId;
                                entity.Price = Entity.Price;
                                entity.ShippingTo = Entity.ShippingTo;
                                entity.DepositReqDate = Entity.DepositReqDate;
                                entity.DeliveryDate = Entity.DeliveryDate;
                                if (PageSource == "Opportunity")
                                {
                                    entity.Compaign = Entity.Compaign;
                                    entity.Declined = Entity.Declined;
                                }
                                if (PageSource == "QuoteDetails")
                                {
                                    entity.Lost = Entity.Lost;
                                }
                                if (PageSource == "OrderDetails")
                                {
                                    entity.Cancelled = Entity.Cancelled;
                                }
                                if (PageSource == "JobDetails")
                                {
                                    entity.ConfirmedDate = Entity.ConfirmedDate;
                                }
                                entity.UpdatedBy = DataBaseCon.ActiveUser();
                                //entity.UpdatedOn = DateTime.Now;
                                entity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));

                                _tblOpportunityRepository.Update(entity);
                                _tblOpportunityRepository.Save();

                            }
                            response.Message = ResponseMessage.SuccessMessage;
                            response.ID = entity.OpportunityId;
                            response.Result = ResponseType.Success;
                            response.tblName = TableNames.tblOpportunity;
                            //}
                            //else
                            //{
                            //    response.Message = "Primary contact already Exist for this opportunity";
                            //    response.Result = ResponseType.Warning;
                            //}
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Result = ResponseType.Error;
                response.ErrorCode = ex.HResult;
                response.tblName = TableNames.tblOpportunity;
            }

            return response;
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<tblOpportunity> GetAll()
        {
            throw new NotImplementedException();
        }

        public tblOpportunity GetById(int id)
        {

            throw new NotImplementedException();

        }
        public Vw_tblOpportunity GetOppById(int id)
        {

            return _VW_tblOpportunityRepository.Get(_ => _.OpportunityId == id).FirstOrDefault();

        }

        public ResponseViewModel Update(tblOpportunity entity)
        {
            throw new NotImplementedException();
        }
        public List<OptionViewModel> GetOptionGrid(int OpportunityID, string Status)
        {
            var data = _st_optionRepository.Get(_ => _.OpportunityId == OpportunityID && _.OptionStage == Status);
            var OptionData = Mapper.Map<List<OptionViewModel>>(data).ToList();
            return OptionData;


        }
        public double TotalPaidBalance(int OpportunityID)
        {
            decimal Totalsum = _tblPaymentRepository.Get(t => t.OpportunityId == OpportunityID)
   .Select(t => t.AmtReceived ?? 0).Sum();
            return Convert.ToDouble( Totalsum);


        }
        
        public List<DecorationViewModel> GetDecorationList()
        {
            // baans change 19th Sept for Active Decoration
            List<DecorationViewModel> DecorationList = new List<DecorationViewModel>();
            //var data = _tblDecorationCostRepository.Get().Select(_ => _.Dec_Desc).Distinct();
            var data = _tblDecorationCostRepository.Get(_ => _.Status == "Active").Select(_ => _.Dec_Desc).Distinct();
            // baans end 19th Sept
            foreach (var item in data)
            {
                DecorationViewModel Decoration = new DecorationViewModel();
                Decoration.Dec_Desc = item;
                DecorationList.Add(Decoration);
            }
            return DecorationList;
        }
        public List<DecorationCostViewModel> GetDecorationCost(string DecorationDesc)
        {
            var data = _tblDecorationCostRepository.Get(_ => _.Dec_Desc == DecorationDesc);
            var ModelData = Mapper.Map<List<DecorationCostViewModel>>(data).ToList();
            return ModelData;
        }
        public IEnumerable<DecorationCostViewModel> GetDecorationCostByQty(string Prefix, string Decoration)
        {
           
            var data = _tblDecorationCostRepository.Get(_ => _.Dec_Desc == Decoration && _.Quantity.Contains(Prefix));
            
            var newdata = data.Select(item => new DecorationCostViewModel
            {
                Quantity = item.Quantity,
                Cost = item.Cost
            }
           ).ToList();
            return newdata;

        }
        public IEnumerable<DecorationViewModel> GetDecorationByDesc(string Prefix)
        {
            List<DecorationViewModel> DecorationList = new List<DecorationViewModel>();
            var data = _tblDecorationCostRepository.Get(_ => _.Dec_Desc.Contains(Prefix) && _.Status == "Active").Select(_ => _.Dec_Desc).Distinct();
            foreach (var item in data)
            {
                DecorationViewModel Decoration = new DecorationViewModel();
                Decoration.Dec_Desc = item;
                DecorationList.Add(Decoration);
            }
            return DecorationList;

        }

        public ResponseViewModel OptionBatchTransaction(tbloption Entity, BatchOperation operation)
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
                            _st_optionRepository.Insert(Entity);
                            _st_optionRepository.Save();
                            response.Message = ResponseMessage.SuccessMessage;
                            response.ID = Entity.id;
                            response.Result = ResponseType.Success;
                            response.tblName = TableNames.tblOption;

                            break;
                        }
                    case BatchOperation.Delete:
                        {

                            var entity = _st_optionRepository.Get(_ => _.id == Entity.id).FirstOrDefault();
                            if (entity != null)
                            {
                                _st_optionRepository.Delete(entity);
                                _st_optionRepository.Save();
                            }
                            response.Message = ResponseMessage.SuccessMessage;
                            response.ID = entity.id;
                            response.Result = ResponseType.Success;
                            response.tblName = TableNames.tblOption;
                            break;
                        }
                    default:
                        {
                            var entity = _st_optionRepository.Get(_ => _.id == Entity.id).FirstOrDefault();
                            if (entity != null)
                            {
                                decimal? gst = 1;
                                var GSTdata = DbContext.tblOpportunities.Where(_ => _.OpportunityId == Entity.OpportunityId).FirstOrDefault();
                                if (GSTdata != null)
                                    gst = GSTdata.GSTValue;


                                entity.Back_decCost = Entity.Back_decCost;
                                entity.Back_decDesign = Entity.Back_decDesign;
                                entity.back_decoration = Entity.back_decoration;
                                entity.Back_decQuantity = Entity.Back_decQuantity;
                                entity.band_id = Entity.band_id;
                                entity.code = Entity.code;
                                entity.colour = Entity.colour;
                                entity.comment = Entity.comment;
                                entity.Cost = Entity.Cost;
                                entity.Declined = Entity.Declined;
                                entity.Extra_decCost = Entity.Extra_decCost;
                                entity.Extra_decDesign = Entity.Extra_decDesign;
                                entity.extra_decoration = Entity.extra_decoration;
                                entity.Extra_decQuantity = Entity.Extra_decQuantity;
                                entity.Front_decCost = Entity.Front_decCost;
                                entity.Front_decDesign = Entity.Front_decDesign;
                                entity.front_decoration = Entity.front_decoration;
                                entity.Front_decQuantity = Entity.Front_decQuantity;
                                entity.include_job = Entity.include_job;
                                entity.InitialSizes = Entity.InitialSizes;
                                entity.SizesPacked = Entity.SizesPacked;
                                entity.item_id = Entity.item_id;
                                entity.Left_decCost = Entity.Left_decCost;
                                entity.Left_decDesign = Entity.Left_decDesign;
                                entity.left_decoration = Entity.left_decoration;
                                entity.Left_decQuantity = Entity.Left_decQuantity;
                                entity.Link = Entity.Link;
                                entity.Margin = Entity.Margin;
                                entity.OtherCost = Entity.OtherCost;
                                entity.OtherDesc = Entity.OtherDesc;
                                entity.quantity = Entity.quantity;
                                entity.Right_decCost = Entity.Right_decCost;
                                entity.Right_decDesign = Entity.Right_decDesign;
                                entity.right_decoration = Entity.right_decoration;
                                entity.Right_decQuantity = Entity.Right_decQuantity;
                                entity.Service = Entity.Service;
                                entity.SizeGrid = Entity.SizeGrid;
                                entity.uni_price = Entity.uni_price;
                                entity.UpdatedBy = DataBaseCon.ActiveUser();
                                entity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                entity.UnitInclGST = Math.Round((Convert.ToDecimal(Entity.uni_price) * Convert.ToDecimal(gst)), 2);
                                entity.ExtExGST = Math.Round((Convert.ToDecimal(Entity.uni_price) * Convert.ToDecimal(Entity.quantity)), 2);
                                entity.ExtInclGST = Math.Round((Convert.ToDecimal(Entity.uni_price) * Convert.ToDecimal(Entity.quantity) * Convert.ToDecimal(gst)), 2);
                                entity.ProofSent = Entity.ProofSent;//17 Oct 2019 (N) ProofSent

                                _st_optionRepository.Update(entity);
                                _st_optionRepository.Save();

                            }
                            response.Message = ResponseMessage.SuccessMessage;
                            response.ID = entity.id;
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
        public List<opportunityViewModel> GetOpportunityData(string oppt, string startDate, string EndDate, int UserProfile)
        {
            var FromDate = Convert.ToDateTime(startDate);
            var ToDate = Convert.ToDateTime(EndDate).AddDays(2);
            List<opportunityViewModel> opptdata = new List<opportunityViewModel>();
            // baans change 19th november
            var IsAdmin = false;
            var ActiveUser = DataBaseCon.ActiveUser();
            var Data = DbContext.tblusers.Where(_ => _.email == ActiveUser).FirstOrDefault();
            if(Data.admin == true && UserProfile == Data.id)
            {
                IsAdmin = true;
            }
            // baans end 
            if (oppt == "All")
            {
                if(!IsAdmin)
                { 
                    var data = _VW_tblOpportunityRepository.Get(_ => _.OppDate >= FromDate && _.OppDate <= ToDate && _.AcctManagerId == UserProfile);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
                else
                {
                    var data = _VW_tblOpportunityRepository.Get(_ => _.OppDate >= FromDate && _.OppDate <= ToDate);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
            }
            else if (oppt == "Lost")
            {
                if (!IsAdmin)
                {
                    var data = _VW_tblOpportunityRepository.Get(_ => _.Lost == "Yes" && _.OppDate >= FromDate && _.OppDate <= ToDate && _.AcctManagerId == UserProfile);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
                else
                {
                    var data = _VW_tblOpportunityRepository.Get(_ => _.Lost == "Yes" && _.OppDate >= FromDate && _.OppDate <= ToDate);
                    // var data = _VW_tblOpportunityRepository.Get(_ => _.Stage == oppt);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
            }
            else if (oppt == "Declined")
            {
                if (!IsAdmin)
                {
                    var data = _VW_tblOpportunityRepository.Get(_ => _.Declined == "Yes" && _.OppDate >= FromDate && _.OppDate <= ToDate && _.AcctManagerId == UserProfile);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
                else
                {
                    var data = _VW_tblOpportunityRepository.Get(_ => _.Declined == "Yes" && _.OppDate >= FromDate && _.OppDate <= ToDate);
                    // var data = _VW_tblOpportunityRepository.Get(_ => _.Stage == oppt);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
            }
            else if (oppt == "Cancelled")
            {
                if (!IsAdmin)
                {
                    var data = _VW_tblOpportunityRepository.Get(_ => _.Cancelled == "Yes" && _.OppDate >= FromDate && _.OppDate <= ToDate && _.AcctManagerId == UserProfile);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
                else
                {
                    var data = _VW_tblOpportunityRepository.Get(_ => _.Cancelled == "Yes" && _.OppDate >= FromDate && _.OppDate <= ToDate);
                    // var data = _VW_tblOpportunityRepository.Get(_ => _.Stage == oppt);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
            }
            // baans change 20th Sept
            
            else if (oppt == "Packing")
            {
                if (!IsAdmin)
                {
                    var data = _VW_tblOpportunityRepository.Get(_ => (_.Stage == "Stock Decorated") && _.OppDate >= FromDate && _.OppDate <= ToDate && _.AcctManagerId == UserProfile);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
                else
                {
                    var data = _VW_tblOpportunityRepository.Get(_ => (_.Stage == "Stock Decorated") && _.OppDate >= FromDate && _.OppDate <= ToDate);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
            }
            else if (oppt == "Invoicing")
            {
                if (!IsAdmin)
                {
                    var data = _VW_tblOpportunityRepository.Get(_ => (_.Stage == "Order Packed" || _.Stage == "Order Invoiced") && _.OppDate >= FromDate && _.OppDate <= ToDate && _.AcctManagerId == UserProfile);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
                else
                {
                    var data = _VW_tblOpportunityRepository.Get(_ => (_.Stage == "Order Packed" || _.Stage == "Order Invoiced") && _.OppDate >= FromDate && _.OppDate <= ToDate);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
            }

            else if (oppt == "Job")
            {
                if (!IsAdmin)
                {
                    var data = _VW_tblOpportunityRepository.Get(_ => (_.Stage == "Order Confirmed" || _.Stage == "Job") && _.OppDate >= FromDate && _.OppDate <= ToDate && _.AcctManagerId == UserProfile);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
                else
                {
                    var data = _VW_tblOpportunityRepository.Get(_ => (_.Stage == "Order Confirmed" || _.Stage == "Job") && _.OppDate >= FromDate && _.OppDate <= ToDate);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
            }

            // baans end 20th Sept

            else if (oppt == "Quote")
            {
                if (!IsAdmin)
                {
                    var data = _VW_tblOpportunityRepository.Get(_ => (_.Stage == "Quote") && _.OppDate >= FromDate && _.OppDate <= ToDate && _.AcctManagerId == UserProfile);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
                else
                {
                    var data = _VW_tblOpportunityRepository.Get(_ => (_.Stage == "Quote") && _.OppDate >= FromDate && _.OppDate <= ToDate);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
            }
            else if (oppt == "Order")
            {
                if (!IsAdmin)
                {
                    var data = _VW_tblOpportunityRepository.Get(_ => (_.Stage == "Order") && _.OppDate >= FromDate && _.OppDate <= ToDate && _.AcctManagerId == UserProfile);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
                else
                {
                    var data = _VW_tblOpportunityRepository.Get(_ => (_.Stage == "Order") && _.OppDate >= FromDate && _.OppDate <= ToDate);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
            }
            else if (oppt == "Opportunity")
            {
                if (!IsAdmin)
                {
                    var data = _VW_tblOpportunityRepository.Get(_ => (_.Stage == "Opportunity") && _.OppDate >= FromDate && _.OppDate <= ToDate && _.AcctManagerId == UserProfile);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
                else
                {
                    var data = _VW_tblOpportunityRepository.Get(_ => (_.Stage == "Opportunity") && _.OppDate >= FromDate && _.OppDate <= ToDate);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
            }
            else if (oppt == "Complete")
            {
                if (!IsAdmin)
                {
                    var data = _VW_tblOpportunityRepository.Get(_ => (_.Stage == "Complete") && _.OppDate >= FromDate && _.OppDate <= ToDate && _.AcctManagerId == UserProfile);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
                else
                {
                    var data = _VW_tblOpportunityRepository.Get(_ => (_.Stage == "Complete") && _.OppDate >= FromDate && _.OppDate <= ToDate);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
            }
            else if (oppt == "Shipping")
            {
                if (!IsAdmin)
                {
                    var data = _VW_tblOpportunityRepository.Get(_ => (_.Stage == "Order Shipped" || _.Stage == "Paid") && _.OppDate >= FromDate && _.OppDate <= ToDate && _.AcctManagerId == UserProfile);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
                else
                {
                    var data = _VW_tblOpportunityRepository.Get(_ => (_.Stage == "Order Shipped" || _.Stage == "Paid") && _.OppDate >= FromDate && _.OppDate <= ToDate);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
            }
            else
            {
                if (!IsAdmin)
                {
                    var data = _VW_tblOpportunityRepository.Get(_ => _.Stage == oppt && _.OppDate >= FromDate && _.OppDate <= ToDate && _.AcctManagerId == UserProfile);
                    // var data = _VW_tblOpportunityRepository.Get(_ => _.Stage == oppt);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
                else
                {
                    var data = _VW_tblOpportunityRepository.Get(_ => _.Stage == oppt && _.OppDate >= FromDate && _.OppDate <= ToDate);
                    // var data = _VW_tblOpportunityRepository.Get(_ => _.Stage == oppt);
                    opptdata = Mapper.Map<List<opportunityViewModel>>(data).OrderByDescending(_ => _.OpportunityId).ToList();
                }
            }

            return opptdata;
        }


        public ResponseViewModel BatchTransaction(tblOpportunity entitity, BatchOperation operation)
        {
            throw new NotImplementedException();
        }
        //13 JUly 2019 (N)
        //public List<st_DecorationViewModel> GetDecorationImagesList(string keyword)
        //{
        //    List<st_DecorationViewModel> DecImageData = new List<st_DecorationViewModel>();

        //    if (keyword == "All")
        //    {
        //        var data = _st_DecorationRepository.Get();
        //        DecImageData = Mapper.Map<List<st_DecorationViewModel>>(data).ToList();
        //    }
        //    else
        //    {
        //        var data = _st_DecorationRepository.Get(_ => _.name.Contains(keyword));
        //        DecImageData = Mapper.Map<List<st_DecorationViewModel>>(data).ToList();
        //    }

        //    return DecImageData;
        //}
        public List<ApplicationViewModel> GetDecorationImagesList(string keyword)
        {
            List<ApplicationViewModel> DecImageData = new List<ApplicationViewModel>();
            
            if (keyword == "All")
            {
                //by sandeep InActive Condition (24Sept)
                var data = _tblApplication.Get(_ =>_.AppStatus != "InActive").OrderByDescending(_ => _.ApplicationId);
                DecImageData = Mapper.Map<List<ApplicationViewModel>>(data).ToList();
            }
            else
            {
                //by sandeep InActive Condition (24Sept)
                var data = _tblApplication.Get(_ => _.AppName.Contains(keyword) && _.AppStatus != "InActive").OrderByDescending(_ => _.ApplicationId);
                DecImageData = Mapper.Map<List<ApplicationViewModel>>(data).ToList();
            }

            return DecImageData;
        }
        //13 JUly 2019 (N)
        public StageChangeResponseViewModel ChangeStageByOppoID(int OppId, string Stage)
        {
            DateTime CurrentDate = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
            bool IsstageUpdate = true;
            StageChangeResponseViewModel Resp = new StageChangeResponseViewModel();
            try
            {
                var OppData = _tblOpportunityRepository.Get(_ => _.OpportunityId == OppId).FirstOrDefault();
                if (OppData != null)
                {

                    switch (Stage)
                    {

                        case "Quote":
                            {
                                OppData.QuoteDate = CurrentDate;
                                break;
                            }
                        case "Order":
                            {
                                OppData.Orderdate = CurrentDate;
                                break;
                            }
                        case "Job":
                            {
                                OppData.JobDate = CurrentDate;
                                break;
                            }
                        case "Order Confirmed":
                            {
                                OppData.OrderConfirmedDate = CurrentDate;
                                break;
                            }
                        case "Art Ordered":
                            {

                                OppData.ArtOrderedDate = CurrentDate;
                                IsstageUpdate = false;
                                break;
                            }
                        case "Proof Approved":
                            {
                                OppData.ApprovedDate = CurrentDate;
                                IsstageUpdate = false;
                                break;
                            }
                        case "Film/Digi Ready":
                            {

                                OppData.ArtReadyDate = CurrentDate;
                               IsstageUpdate = false;
                                break;
                            }
                        case "Stock Ordered":
                            {
                                OppData.StockOrderedDate = CurrentDate;
                                IsstageUpdate = false;

                                break;
                            }
                        case "Stock In":
                            {
                                OppData.ReceivedDate = CurrentDate;
                                IsstageUpdate = false;
                                break;
                            }
                        case "Stock Checked":
                            {
                                OppData.Checkeddate = CurrentDate;                            
                                IsstageUpdate = false;
                                break;
                            }
                        case "Stock Decorated":
                            {
                                OppData.DecoratedDate = CurrentDate;
                                break;
                            }
                        case "Order Packed":
                            {

                                OppData.PackingDate = CurrentDate;
                                break;
                            }
                        case "Order Invoiced":
                            {
                                OppData.InvoicingDate = CurrentDate;
                                break;
                            }
                        case "Paid":
                            {
                                OppData.PaidDate = CurrentDate;
                                break;
                            }
                        case "Order Shipped":
                            {
                                OppData.ShippedDate = CurrentDate;
                                break;
                            }
                        case "Complete":
                            {
                                OppData.CompleteDate = CurrentDate;
                                break;
                            }
                        case "Job Accepted":
                            {
                                OppData.JobAcceptedDate = CurrentDate;
                                break;
                            }
                        case "Proof Created":
                            {
                                OppData.ProofCreatedDate = CurrentDate;
                                break;
                            }
                        case "Proof Sent":
                            {
                                OppData.ProofSentdate = CurrentDate;
                                break;
                            }
                    }
                    if(IsstageUpdate)
                    OppData.Stage = Stage;

                    OppData.UpdatedBy = DataBaseCon.ActiveUser();
                    OppData.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                    _tblOpportunityRepository.Update(OppData);
                    _tblOpportunityRepository.Save();
                }
                Resp.ChangeDate = CurrentDate;
                response.ID = OppData.OpportunityId;
                response.Message = ResponseMessage.SuccessMessage;
                response.Result = "Success";
                Resp.response = response;
            }
            catch (Exception ex)
            {
                response.ID = 0;
                response.Message = ex.Message;
                response.Result = "Error";
                Resp.response = response;
            }
            return Resp;
        }
        // baans change 03rd November for ChangeConfirmedDateOppoID


        public StageChangeResponseViewModel ResetStageByOppoID(int oppId, string stage)
        {
            StageChangeResponseViewModel resp = new StageChangeResponseViewModel();
            try
            {
                var OppData = _tblOpportunityRepository.Get(_ => _.OpportunityId == oppId).FirstOrDefault();
                if (OppData != null)
                {
                    switch (stage)
                    {
                       
                        case "Quote":
                            {
                                OppData.QuoteDate = null;
                                OppData.QuoteMail = null;
                                OppData.QuoteNotes = null;
                                OppData.Stage = "Opportunity";
                                var optionData = _st_optionRepository.Get(x => x.OpportunityId == OppData.OpportunityId && x.OptionStage == "Opp").ToList();
                                foreach (var item in optionData)
                                {
                                    _st_optionRepository.Delete(item);
                                    _st_optionRepository.Save();
                                }
                                break;
                            }
                        case "Order":
                            {
                                OppData.Orderdate = null;
                                OppData.OrderMailDate = null;
                                OppData.OrderNotes = null;
                                OppData.Stage = "Quote";

                                var optionData = _st_optionRepository.Get(x=>x.OpportunityId==OppData.OpportunityId && x.OptionStage== "Order").ToList();
                                foreach (var item in optionData)
                                {
                                    _st_optionRepository.Delete(item);
                                    _st_optionRepository.Save();
                                }
                                break;
                            }
                        case "Job":
                            {
                                OppData.JobDate = null;
                                OppData.JobNotes = null;
                                OppData.JobAcceptedDate = null;
                                OppData.AddressId = null;
                                OppData.Stage = "Order";
                                var paymetData = _tblPaymentRepository.Get(x=>x.OpportunityId==OppData.OpportunityId).ToList();
                                foreach (var item in paymetData)
                                {
                                    _tblPaymentRepository.Delete(item);
                                    _tblPaymentRepository.Save();
                                }
                                break;
                            }
                        case "Order Confirmed":
                            {
                                
                                OppData.Stage = "Job";
                                OppData.ConfirmedDate = null;
                                OppData.OrderConfirmedDate = null;
                                OppData.ConfirmMailDate = null;
                                break;
                            }
                        case "Art Ordered":
                            {

                                OppData.ArtOrderedDate = null;
                                OppData.Stage = "Job Accepted";
                                break;
                            }
                        case "Proof Approved":
                            {
                                OppData.ApprovedDate = null;
                                break;
                            }
                        case "Film/Digi Ready":
                            {

                                OppData.ArtReadyDate = null;
                                break;
                            }
                        case "Stock Ordered":
                            {
                                OppData.StockOrderedDate = null;
                                break;
                            }
                        case "Stock In":
                            {
                                OppData.ReceivedDate = null;
                                break;
                            }
                        case "Stock Checked":
                            {
                                OppData.Checkeddate = null;
                                break;
                            }
                        case "Stock Decorated":
                            {
                                OppData.DecoratedDate = null;
                                OppData.Stage = "Proof Sent";
                                break;
                            }
                        case "Order Packed":
                            {
                                OppData.PackingNotes = null;
                                OppData.PackingDate = null;
                                OppData.Stage = "Stock Decorated";
                                break;
                            }
                        case "Order Invoiced":
                            {
                                OppData.InvoicingDate = null;
                                OppData.InvoiceMailDate = null;
                                OppData.InvoicingNotes = null;
                                OppData.Stage = "Order Packed";
                                break;
                            }
                        case "Paid":
                            {
                                OppData.PaidDate = null;
                                OppData.PackingNotes = null;
                                OppData.Stage = "Order Invoiced";
                                break;
                            }
                        case "Order Shipped":
                            {
                                OppData.ShippedDate = null;
                                OppData.Stage = "Paid";
                                break;
                            }
                        case "Complete":
                            {
                                OppData.CompleteDate = null;
                                OppData.Stage = "Order Shipped";
                                OppData.PackedInSet1 =null;
                                OppData.PackedInSet2 = null;
                                OppData.ConsigNoteNo = null;
                                OppData.PacagingNotes = null;
                                break;
                            }
                        case "Job Accepted":
                            {
                                OppData.JobAcceptedDate = null;
                                OppData.Stage = "Order Confirmed";
                                break;
                            }
                        case "Proof Created":
                            {
                                OppData.ProofCreatedDate = null;
                                OppData.Stage = "Art Ordered";
                                break;
                            }
                        case "Proof Sent":
                            {
                                OppData.ProofSentdate = null;
                                OppData.Stage = "Proof Created";
                                break;
                            }
                    }
                    
                       
                    OppData.UpdatedBy = DataBaseCon.ActiveUser();
                    OppData.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                    _tblOpportunityRepository.Update(OppData);
                    _tblOpportunityRepository.Save();
                }
                
                response.ID = OppData.OpportunityId;
                response.Message = ResponseMessage.revertMessage;
                response.Result = "Success";
                resp.response = response;
            }
            catch (Exception ex)
            {
                response.ID = 0;
                response.Message = ex.Message;
                response.Result = "Error";
                resp.response = response;
            }
            return resp;
        }

        public ResponseViewModel ChangeConfirmedDateOppoID(int OppId, string ConfirmedDate)
        {
            DateTime NewConfirmDate = Convert.ToDateTime(ConfirmedDate);
            var OppData = _tblOpportunityRepository.Get(_ => _.OpportunityId == OppId).FirstOrDefault();
            if(OppData != null)
            {
                OppData.ConfirmedDate = NewConfirmDate;
                OppData.UpdatedBy = DataBaseCon.ActiveUser();
                OppData.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                _tblOpportunityRepository.Update(OppData);
                _tblOpportunityRepository.Save();

                response.ID = OppId;
                response.Message = ResponseMessage.SuccessMessage;
                response.Result = "Success";

            }
            return response;
        }
            // baans end 03rd November
        ResponseViewModel IOpportunityService.UploadOppImage(string filename, int OppId, string field, string path)
        {
            var entity = _tblInquiryRepository.Get(_ => _.OpportunityId == OppId).FirstOrDefault();
            tblInquiry data = new tblInquiry();
            var oldFileName = "";

            if (entity != null)
            {
                if (field == "front")
                {
                    oldFileName = entity.FrontPrintArt;
                    entity.FrontPrintArt = filename;
                }
                if (field == "back")
                {
                    oldFileName = entity.BackPrintArt;
                    entity.BackPrintArt = filename;
                }
                if (field == "left")
                {
                    oldFileName = entity.LeftPrintArt;
                    entity.LeftPrintArt = filename;
                }
                if (field == "right")
                {
                    oldFileName = entity.RighPrintArt;
                    entity.RighPrintArt = filename;
                }

                entity.UpdatedBy = DataBaseCon.ActiveUser();
                entity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));

                _tblInquiryRepository.Update(entity);
                _tblInquiryRepository.Save();
                if (oldFileName != null && oldFileName != "")
                {
                    if (System.IO.File.Exists(path + oldFileName))
                    {
                        System.IO.File.Delete(path + oldFileName);
                    }
                }
                response.Message = "Image saved successfully";
                response.ID = OppId;
                response.Result = ResponseType.Success;
            }
            else
            {
                if (field == "front")
                {
                    data.OpportunityId = OppId;
                    data.FrontPrintArt = filename;
                }
                if (field == "back")
                {
                    data.OpportunityId = OppId;
                    data.BackPrintArt = filename;
                }
                if (field == "left")
                {
                    data.OpportunityId = OppId;
                    data.LeftPrintArt = filename;
                }
                if (field == "right")
                {
                    data.OpportunityId = OppId;
                    data.RighPrintArt = filename;
                }

                data.CreatedBy = DataBaseCon.ActiveUser();
                data.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));

                _tblInquiryRepository.Insert(data);
                _tblInquiryRepository.Save();

                response.Message = "Image saved successfully";
                response.ID = Convert.ToInt32(data.OpportunityId);
                response.Result = ResponseType.Success;
            }

            return response;
        }
        public InquiryViewModel GetInquiryData(int OppId)
        {
            var data = _tblInquiryRepository.Get().Where(_ => _.OpportunityId == OppId).FirstOrDefault();
            var newData = Mapper.Map<InquiryViewModel>(data);
            return newData;
        }
        public ResponseViewModel UpdateOppInquiry(tblInquiry Entity)
        {
            try
            {
                var entity = _tblInquiryRepository.Get(_ => _.OpportunityId == Entity.OpportunityId).FirstOrDefault();

                if (entity != null)
                {
                    entity.UpdatedBy = DataBaseCon.ActiveUser();
                    entity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));

                    entity.FrontPrintArt = Entity.FrontPrintArt;
                    entity.FrontPrintNotes = Entity.FrontPrintNotes;
                    entity.BackPrintArt = Entity.BackPrintArt;
                    entity.BackPrintNotes = Entity.BackPrintNotes;
                    entity.LeftPrintArt = Entity.LeftPrintArt;
                    entity.LEftPrintNotes = Entity.LEftPrintNotes;
                    entity.RighPrintArt = Entity.RighPrintArt;
                    entity.RightPrintNotes = Entity.RightPrintNotes;
                    entity.ItemColours = Entity.ItemColours;
                    entity.PrefBrandsAndStyle = Entity.PrefBrandsAndStyle;
                    entity.GeneralNotes = Entity.GeneralNotes;
                    entity.UpdatedBy = DataBaseCon.ActiveUser();
                    entity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                    _tblInquiryRepository.Update(entity);
                    _tblInquiryRepository.Save();

                    response.Message = ResponseMessage.SuccessMessage;
                    response.ID = Convert.ToInt32(entity.OpportunityId);
                    response.Result = ResponseType.Success;
                    response.tblName = TableNames.tblInquiry;
                }
                else
                {
                    Entity.CreatedBy = DataBaseCon.ActiveUser();
                    Entity.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));

                    _tblInquiryRepository.Insert(Entity);
                    _tblInquiryRepository.Save();

                    response.Message = ResponseMessage.SuccessMessage;
                    response.ID = Convert.ToInt32(Entity.OpportunityId);
                    response.Result = ResponseType.Success;
                    response.tblName = TableNames.tblInquiry;
                }
            }
            catch(Exception ex)
            {
                response.Message = ex.Message;
                response.tblName = TableNames.tblInquiry;
                response.Result = ResponseType.Error;
                response.ErrorCode = ex.HResult;
            }

            return response;
        }

        //27 May 2019 (N)
        //public ResponseViewModel SendEmail(EmailViewModel model, string path, int OpportunityId, string PathPdf)
        //{
        //    try
        //    {
        //        var EmailContent = _tblEmailContentRepository.Get(_ => _.Purpose == model.OptionStatus).FirstOrDefault();
        //        var contactDetails = _tblOppContactMappingRepository.Get(_ => _.OpportunityId == OpportunityId && _.IsPrimary == true).FirstOrDefault().tblcontact;
        //        string ContactFullname = "", ContactEmail = "", Organisation = "", AccountManageName = "", ContactName = "", AcctDesignation = "", AcctEmail = "";

        //        // baans change 26th October for Dynamic Designation
        //        var CurrentAccountManagerDetail = DbContext.tblOpportunities.Where(_ => _.OpportunityId == OpportunityId).Select(_ => _.AcctManagerId).FirstOrDefault();
        //        if (CurrentAccountManagerDetail != null)
        //        {

        //            AccountManageName = DbContext.tblusers.Where(_ => _.id == CurrentAccountManagerDetail).Select(_ => _.firstname + " " + _.lastname).FirstOrDefault();
        //            AcctDesignation = DbContext.tblusers.Where(_ => _.id == CurrentAccountManagerDetail).Select(_ => _.title).FirstOrDefault();
        //            AcctEmail = DbContext.tblusers.Where(_ => _.id == CurrentAccountManagerDetail).Select(_ => _.email).FirstOrDefault();
        //        }
        //        // baans end 26th October

        //        if (contactDetails != null)
        //        {
        //            ContactFullname = contactDetails.first_name + " " + contactDetails.last_name;
        //            ContactEmail = contactDetails.email;
        //            ContactName = contactDetails.first_name;
        //            if (contactDetails.tblOrganisation != null)
        //                Organisation = contactDetails.tblOrganisation.OrgName;



        //        }
        //        if (model.MailMessage2 != null)
        //        {
        //            model.MailMessage2 = Regex.Replace(model.MailMessage2,"\n", "<br>");
        //            //var MailMsg2 = model.MailMessage2.Split('\n');
        //            //if (MailMsg2.Length > 0)
        //            //{
        //            //    model.MailMessage2 = "";
        //            //    for (var h = 0; h < MailMsg2.Length; h++)
        //            //    {
        //            //        model.MailMessage2 += MailMsg2[h] + "<br>";
        //            //    }
        //            //}
        //        }
        //        else
        //        {
        //            model.MailMessage2 = "";
        //        }


        //        string Msg1 = "", Msg2 = "", Msg3 = "", Msg4 = "";


        //        if (EmailContent != null)
        //        {
        //            // Baans change 15th November for msg from body1 pop up and 2
        //            Msg1 = EmailContent.Body1;
        //            Msg2 = EmailContent.Body2;
        //            //Msg1 = model.MailMessage1;
        //            //Msg2 = model.MailMessage3;
        //            // Baans end 15th November
        //            Msg3 = EmailContent.Body3;
        //        }
        //        // baans change 20th November for changing the content
        //        if (AccountManageName.Contains("Kenneth"))
        //        {
        //            Msg4 = "http://au.linkedin.com/in/kennethswan";
        //        }
        //        // baans end 20th November
        //        LinkedResource Img = null;
        //        string str = "";
        //        if (model.OptionStatus == "Opp")
        //        {
        //            // 04 Oct 2018(N)
        //            //    Img = new LinkedResource(path, MediaTypeNames.Image.Jpeg);
        //            //    Img.ContentId = "MyImage";
        //            //     str = @"<table style='width:100%'><tr>  
        //            //    <td colspan='3'>  
        //            //      <img src=cid:MyImage  id='img' alt='' width='100%' height='100px'/>   
        //            //    </td>  
        //            //</tr><tr><td colspan='2' style='width:90%'>" + ContactFullname + "</td><td style= 'text-align: right'> " + Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime())).ToString("dd/MM/yyyy") + " </td></tr><tr><td colspan='2' style='width:90%'> " + ContactEmail + " </td><td style= 'text-align: right'> Quote No: " + OpportunityId + "  </td></tr><tr><td>Organisation </td><td> </td><td></td></tr><tr><td>" + Organisation + "</td><td> </td><td></td></tr></table><br><br><br>Dear " + ContactName + ",<br><br>" + Msg1 + "<br><br>" + model.MailMessage2 + "<br><br>" + Msg2 + "<br><br><br> Best Regards,<br><br>" + AccountManageName + "<br>Account Manager<br>" + Msg3;


        //            Img = new LinkedResource(path, MediaTypeNames.Image.Jpeg);
        //            Img.ContentId = "MyImage";
        //            // baans change 17th October for change the Signature
        //            //str = @"<br>Hi " + ContactName + ",<br><br>" + model.MailMessage2 + "<br><br>Regards,<br><br>" + "<b>" + AccountManageName + "</b>" + "<br>" + Msg3 + "<br><br><b>" + AcctDesignation + "</b><br> 24 Hour Merchandise <br><br>http://au.linkedin.com/in/kennethswan" + "<br><br><table style ='width:100%'><tr><td colspan='3'><a href ='https://24hourt-shirts.com.au/'><img src = cid:MyImage id = 'img' alt = '' style='width:82%;height:auto;' /></a></td></tr></table>";

        //            str = @"<br>Hi " + ContactName + ",<br><br>" + model.MailMessage2 + "<br><br>Regards,<br><br>" + AccountManageName + "<br>" + AcctDesignation + "<br><b> 24 Hour Merchandise </b><br>" + Msg3 + "<br><br>" + Msg4 + "<br><br><table style ='width:100%'><tr><td colspan='3'><a href ='https://24hourt-shirts.com.au/'><img src = cid:MyImage id = 'img' alt = '' style='width:82%;height:auto;' /></a></td></tr></table>";
        //            // baans end 17th October
        //        }
        //        // baans change 07 august for email through job
        //        else if (model.OptionStatus == "Confirm")
        //        {
        //            Img = new LinkedResource(path, MediaTypeNames.Image.Jpeg);
        //            Img.ContentId = "MyImage";
        //            // baans change 22nd October for Email from Confirm Screen
        //            //    str = @"<table style='width:100%'><tr>  
        //            //    <td colspan='3'>  
        //            //      <img src=cid:MyImage  id='img' alt='' width='100%' height='100px'/>   
        //            //    </td>  
        //            //</tr><tr><td colspan='2' style='width:85%'></td><td style= 'text-align: right'>" + Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime())).ToString("dd/MM/yyyy") + " </td></tr><tr><td colspan='2' style='width:85%'></td><td style= 'text-align: right'>Order/Invoice No: " + OpportunityId + "  </td></tr><tr><td></td><td> </td><td></td></tr><tr><td></td><td> </td><td></td></tr></table><br><br><br>Hi " + ContactName + ",<br><br>" + Msg1 + "<br><br>" + model.MailMessage2 + "<br><br>" + Msg2 + "<br><br><br> Best Regards,<br><br>" + AccountManageName + "<br>Account Manager<br>" + Msg3;

        //            //str = @"<br>Hi " + ContactName + ",<br><br>" + model.MailMessage2 + "<br><br>Regards,<br><br>" + "<b>" + AccountManageName + "</b>" + "<br>" + Msg3 + "<br><br><b>" + AcctDesignation + "</b><br> 24 Hour Merchandise <br><br>http://au.linkedin.com/in/kennethswan" + "<br><br><table style ='width:100%'><tr><td colspan='3'><a href ='https://24hourt-shirts.com.au/'><img src = cid:MyImage id = 'img' alt = '' style='width:82%;height:auto;' /></a></td></tr></table>";

        //            str = @"<br>Hi " + ContactName + ",<br><br>" + model.MailMessage2 + "<br><br>Regards,<br><br>" + AccountManageName + "<br>" + AcctDesignation + "<br><b> 24 Hour Merchandise </b><br>" + Msg3 + "<br><br>" + Msg4 + "<br><br><table style ='width:100%'><tr><td colspan='3'><a href ='https://24hourt-shirts.com.au/'><img src = cid:MyImage id = 'img' alt = '' style='width:82%;height:auto;' /></a></td></tr></table>";
        //            // baans end 22nd Octber
        //        }

        //        else if (model.OptionStatus == "Invoice")
        //        {
        //            //    Img = new LinkedResource(path, MediaTypeNames.Image.Jpeg);
        //            //    Img.ContentId = "MyImage";
        //            //    str = @"<table style='width:100%'><tr>  
        //            //    <td colspan='3'>  
        //            //      <img src=cid:MyImage  id='img' alt='' width='100%' height='100px'/>   
        //            //    </td>  
        //            //</tr><tr><td colspan='2' style='width:85%'></td><td style= 'text-align: right'>" + Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime())).ToString("dd/MM/yyyy") + " </td></tr><tr><td colspan='2' style='width:85%'></td><td style= 'text-align: right'>Order/Invoice No: " + OpportunityId + "  </td></tr><tr><td></td><td> </td><td></td></tr><tr><td></td><td> </td><td></td></tr></table><br><br><br>Hi " + ContactName + ",<br><br>" + Msg1 + "<br><br>" + model.MailMessage2 + "<br><br>" + Msg2 + "<br><br><br> Best Regards,<br><br>" + AccountManageName + "<br>Account Manager<br>" + Msg3;


        //            Img = new LinkedResource(path, MediaTypeNames.Image.Jpeg);
        //            Img.ContentId = "MyImage";
        //            //str = @"<br>Hi " + ContactName + ",<br><br>" + model.MailMessage2 + "<br><br>Regards,<br><br>" + "<b>" + AccountManageName + "</b>" + "<br>" + Msg3 + "<br><br><b>" + AcctDesignation + "</b><br> 24 Hour Merchandise <br><br>http://au.linkedin.com/in/kennethswan" + "<br><br><table style ='width:100%'><tr><td colspan='3'><a href ='https://24hourt-shirts.com.au/'><img src = cid:MyImage id = 'img' alt = '' style='width:82%;height:auto;' /></a></td></tr></table>";

        //            str = @"<br>Hi " + ContactName + ",<br><br>" + model.MailMessage2 + "<br><br>Regards,<br><br>" + AccountManageName + "<br>" + AcctDesignation + "<br><b> 24 Hour Merchandise </b><br>" + Msg3 + "<br><br>" + Msg4 + "<br><br><table style ='width:100%'><tr><td colspan='3'><a href ='https://24hourt-shirts.com.au/'><img src = cid:MyImage id = 'img' alt = '' style='width:82%;height:auto;' /></a></td></tr></table>";
        //        }
        //        // baans end 07 august
        //        else
        //        {       //20 Sep 2018 (N)
        //                //    Img = new LinkedResource(path, MediaTypeNames.Image.Jpeg);
        //                //    Img.ContentId = "MyImage";
        //                //    str = @"<table style='width:100%'><tr>  
        //                //        <td colspan='3'>  
        //                //          <img src=cid:MyImage  id='img' alt='' width='100%' height='100px'/>   
        //                //        </td>  
        //                //    </tr><tr><td colspan='2' style='width:85%'></td><td style= 'text-align: right'>" + Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime())).ToString("dd/MM/yyyy") + " </td></tr><tr><td colspan='2' style='width:85%'></td><td style= 'text-align: right'>Order/Invoice No: " + OpportunityId + "  </td></tr><tr><td></td><td> </td><td></td></tr><tr><td></td><td> </td><td></td></tr></table><br><br><br>Hi " + ContactName + ",<br><br>" + Msg1 + "<br>" + model.MailMessage2 + "<br>" + Msg2 + "<br><br>Best Regards,<br><br>" + AccountManageName + "<br><br>" + Msg3;

        //            Img = new LinkedResource(path, MediaTypeNames.Image.Jpeg);
        //            Img.ContentId = "MyImage";
        //            // baans change 17th October for Change in the Signature
        //            //str = @"<br>Hi " + ContactName + ",<br><br>" + model.MailMessage2 + "<br><br>Regards,<br><br>" + "<b>" + AccountManageName + "</b>" + "<br>" + Msg3 + "<br><br><b>" + AcctDesignation + "</b><br> 24 Hour Merchandise <br><br>http://au.linkedin.com/in/kennethswan" + "<br><br><table style ='width:100%'><tr><td colspan='3'><a href ='https://24hourt-shirts.com.au/'><img src = cid:MyImage id = 'img' alt = '' style='width:82%;height:auto;' /></a></td></tr></table>";

        //            str = @"<br>Hi " + ContactName + ",<br><br>" + model.MailMessage2 + "<br><br>Regards,<br><br>" + AccountManageName + "<br>" + AcctDesignation + "<br><b> 24 Hour Merchandise </b><br>" + Msg3 + "<br><br>" + Msg4 + "<br><br><table style ='width:100%'><tr><td colspan='3'><a href ='https://24hourt-shirts.com.au/'><img src = cid:MyImage id = 'img' alt = '' style='width:82%;height:auto;' /></a></td></tr></table>";

        //            //baans end 17th October
        //        }

        //        AlternateView AV =
        //        AlternateView.CreateAlternateViewFromString(str, null, MediaTypeNames.Text.Html);
        //        AV.LinkedResources.Add(Img);
        //        string To = model.Email;
        //        // baans change 16th January for subject with OpportunityId
        //        //string subject = model.Subject;
        //        string subject = model.Subject;
        //        // baans end 16th January
        //        DataBaseCon.SendEmail(To, subject, AV, path, PathPdf, AcctEmail);
        //        var Oppodata = _tblOpportunityRepository.Get(_ => _.OpportunityId == OpportunityId).FirstOrDefault();
        //        Oppodata.QuoteMail = model.MailMessage2;
        //        // baans change 11 august
        //        if (model.OptionStatus == "Opp")
        //            Oppodata.QuoteMailDate = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));

        //        if (model.OptionStatus == "Order")
        //            Oppodata.OrderMailDate = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
        //        //if (model.OptionStatus == "Invoice")
        //        //    Oppodata = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));

        //        if (model.OptionStatus != "Confirm")
        //        {
        //            Oppodata.UpdatedBy = DataBaseCon.ActiveUser();
        //            Oppodata.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
        //        }
        //        // baans end 11 august

        //        _tblOpportunityRepository.Update(Oppodata);
        //        _tblOpportunityRepository.Save();
        //        response.Result = ResponseType.Success;
        //        response.Message = "Mail Sent successfully";
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Message = ex.Message;
        //        //response.Message = "error occurred";
        //    }
        //    return response;
        //}
        //27 May 2019 (N)

        public ResponseViewModel OpportunityContactMapping(OppContactMappingViewModel model)
        {
            try
            {
                var MappingData = _tblOppContactMappingRepository.Get(_ => _.ContactId == model.ContactID && _.OpportunityId == model.MappingID).FirstOrDefault();

                var PrimaryMappingData = _tblOppContactMappingRepository.Get(_ => _.OpportunityId == model.MappingID && _.IsPrimary==true).FirstOrDefault();
                if (MappingData != null)
                {
                    if (!model.isPrimary)
                    {
                        MappingData.IsPrimary = model.isPrimary;
                        MappingData.UpdatedBy = DataBaseCon.ActiveUser();
                        MappingData.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                        _tblOppContactMappingRepository.Update(MappingData);
                        _tblOppContactMappingRepository.Save();
                        response.Message = ResponseMessage.SuccessMessage;
                        response.Result = ResponseType.Success;
                    }
                    else
                    {
                        if(PrimaryMappingData!=null)
                        {
                            if(model.ContactID!=PrimaryMappingData.ContactId)
                            {
                                response.Message ="Primary contact with this opportunity already exists";
                                response.Result = ResponseType.Warning;
                            }
                        }
                        else
                        {
                            MappingData.IsPrimary = model.isPrimary;
                            MappingData.UpdatedBy = DataBaseCon.ActiveUser();
                            MappingData.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                            _tblOppContactMappingRepository.Update(MappingData);
                            _tblOppContactMappingRepository.Save();
                            response.Message = ResponseMessage.SuccessMessage;
                            response.Result = ResponseType.Success;
                        }

                    }
                }
                else
                {
                    response.Message = "Contact Does not linked with this opportunity";
                    response.Result = ResponseType.Warning;

                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Result = ResponseType.Error;
            }
            return response;
        }
        public ResponseViewModel DeleteOppImage(InquiryImageDeleteViewModel model,string path)
        {
            try
            {
                var oldFileName = "";
                var Inquieydata = _tblInquiryRepository.Get(_ => _.OpportunityId == model.OppId).FirstOrDefault();
                if(Inquieydata!=null)
                {
                    switch(model.Type)
                    {
                        case "front":
                            {
                                oldFileName = Inquieydata.FrontPrintArt;
                                Inquieydata.FrontPrintArt = null;
                                break;
                            }
                        case "back":
                            {
                                oldFileName = Inquieydata.BackPrintArt;
                                Inquieydata.BackPrintArt = null;
                                break;
                            }
                        case "left":
                            {
                                oldFileName = Inquieydata.LeftPrintArt;
                                Inquieydata.LeftPrintArt = null;
                                break;
                            }
                        case "right":
                            {
                                oldFileName = Inquieydata.RighPrintArt;
                                Inquieydata.RighPrintArt = null;
                                break;
                            }
                    }
                    Inquieydata.UpdatedBy = DataBaseCon.ActiveUser();
                    Inquieydata.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                    _tblInquiryRepository.Update(Inquieydata);
                    _tblInquiryRepository.Save();
                    if (oldFileName != null && oldFileName != "")
                    {
                        if (System.IO.File.Exists(path + oldFileName))
                        {
                            System.IO.File.Delete(path + oldFileName);
                        }
                    }
                    response.ID = model.OppId;
                    response.Result = ResponseType.Success;
                    response.Message = "Image deleted successfully";

                }
            }
            catch(Exception ex)
            {
                response.Message = ex.Message;
                response.Message = ex.Message;
            }
            return response;
        }
     public ImageResponseViewModel UploadOppThumbnail(string filename, int OppId,string path)
        {
            var oldFileName = "";
            ImageResponseViewModel res = new ImageResponseViewModel();
            try
            {
                var entity = _tblOpportunityRepository.Get(_ => _.OpportunityId == OppId).FirstOrDefault();
                if (entity != null)
                {
                    oldFileName = entity.OppThumbnail;
                    entity.OppThumbnail = filename;
                    entity.UpdatedBy = DataBaseCon.ActiveUser();
                    entity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                    _tblOpportunityRepository.Update(entity);
                    _tblOpportunityRepository.Save();
                    if(oldFileName!=null && oldFileName!="")
                    {
                        if (System.IO.File.Exists(path+ oldFileName))
                        {
                            System.IO.File.Delete(path + oldFileName);
                        }
                    }
                    res.Message = "Image saved successfully";
                    res.ID = OppId;
                    res.Result = ResponseType.Success;
                    res.FileName = filename;

                }
            }
            catch(Exception ex)
            {
                res.Message = ex.Message;
                res.Result = ResponseType.Error;
            }
            return res;
        }
       
        public EmailContentViewModel GetMailMessage(string OptionStatus)
        {
            var EmailMessage =Mapper.Map<EmailContentViewModel>( _tblEmailContentRepository.Get(_ => _.Purpose == OptionStatus).FirstOrDefault());
            return EmailMessage;
        }

        public List<opportunityViewModel> GetCustomOppList(string CustomText, string TableName)
        {
            //  var ddd = DbContext.Pro_Search(TableName, CustomText);
           var CustomOppData = Mapper.Map<List<opportunityViewModel>>(DbContext.Database.SqlQuery<Vw_tblOpportunity>("exec Pro_Search '" + TableName + "','" + CustomText + "'").ToList()).ToList();
            //  DbContext.Pro_Search
            return CustomOppData;
        }
        public List<Pro_HistoryOfPayments_Result> GetPaymentHistory(int id)
        {
            // 27 Aug 2018 (N)

            //var historydata = _tblPaymentRepository.Get(_ => _.OpportunityId == id).ToList();
            //var data = Mapper.Map<List<Pro_HistoryOfPayments_Result>>(historydata).ToList();
            //return data;

            var HistoryGrid = new List<Pro_HistoryOfPayments_Result>();
            HistoryGrid = DbContext.Pro_HistoryOfPayments(id).ToList();
            return HistoryGrid;

            // 27 Aug 2018 (N)
        }
        public ResponseViewModel UpdateOppPackin(OpportunityPackInViewModel model)
        {
            try
            {
                if (model.OpportunityId > 0)
                {
                    var OpportunityForUpdate = _tblOpportunityRepository.Get(_ => _.OpportunityId == model.OpportunityId).FirstOrDefault();
                    OpportunityForUpdate.PackedInSet1 = model.PackedInSet1;
                    OpportunityForUpdate.PackedInSet2 = model.PackedInSet2;
                    OpportunityForUpdate.ConsigNoteNo = model.ConsigNoteNo;
                    OpportunityForUpdate.PacagingNotes = model.PacagingNotes;
                    _tblOpportunityRepository.Update(OpportunityForUpdate);
                    _tblOpportunityRepository.Save();
                    response.Message = ResponseMessage.SuccessMessage;
                    response.Result = ResponseType.Success;
                    response.ID = model.OpportunityId;



                }
            }
            catch(Exception ex)
            {
                response.Message = ex.Message;
                response.Result = ResponseType.Error;
                response.ErrorCode = ex.HResult;
            }
            return response;
        }
        public bool GetPackinDetails(int OppId)
        {
            var result = false;
            var oppdata = _tblOpportunityRepository.Get(_ => _.OpportunityId == OppId).FirstOrDefault();
            if(oppdata!=null)
            {
                //Stopped the check for the packed in set 1 24th November
                //if(oppdata.PackedInSet1!=null && oppdata.PackedInSet1 != "")
                //{
                //    result = true;
                //}
                // baans end 24th November
                result = true;
            }
            return result;
        }
        public bool GetShipDetails(int OppId)
        {
            var result = false;
            var oppdata = _tblOpportunityRepository.Get(_ => _.OpportunityId == OppId).FirstOrDefault();
            if (oppdata != null)
            {
                if (oppdata.ConsigNoteNo != null && oppdata.ConsigNoteNo != "")
                {
                    result = true;
                }
            }
            return result;
        }
        
        public List<Pro_PayHistory_Result> GetPaymentList(int OrgId)
        {

            var PaymentGrid = new List<Pro_PayHistory_Result>();

            if (OrgId != 0)
            {
                try
                {

                    //int OrgId1 = Int32.Parse(OrgId);
                    PaymentGrid = DbContext.Pro_PayHistory(OrgId).ToList();
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    response.Result = "Error";
                }
            }

            return PaymentGrid;
        }

        public ResponseViewModel PaymentBatchTransaction(tblPayment Entity, PaymentViewModel model, BatchOperation operation)
        {
            try
            {
                switch (operation)
                {
                    case BatchOperation.Insert:
                        {
                            // Baans end 26th November for CreatedBy and CreatedOn
                            Entity.CreatedBy = DataBaseCon.ActiveUser();
                            Entity.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                            // baans end 26th November
                            _tblPaymentRepository.Insert(Entity);
                            _tblPaymentRepository.Save();
                            response.Message = ResponseMessage.SuccessMessage;
                            response.Result = ResponseType.Success;
                            break;
                        }

                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Result = "Error";
            }

            return response;
        }

        public tblCommonData GetCommandDataForPaymentDescription()
        {
            int OppId = 2;
            var data = _tblCommonData.Get(_ => _.Id == OppId).FirstOrDefault();
            return data;
        }
        // baans change 22nd August for Getting the AccountManagerName
        public tbluser GetAccountManagerById(int Id)
        {
            var Data = _tblUsers.Get(_ => _.id == Id).FirstOrDefault();
            return Data;
        }
        // baans end 22nd August

        //20 Aug 2018 (N)
        public IEnumerable<OppoDropdownViewModel> GetOpportunityByOppName(string prefix)
        {
            var data = _VW_tblOpportunityRepository.Get(_ => _.OppName.Contains(prefix));
            var newdata = data.Select(item => new OppoDropdownViewModel
            {
                OpportunityId = item.OpportunityId,
                OppName = item.OppName,
                Quantity = item.Quantity,
                ReqDate = item.ReqDate == null ? "" : Convert.ToDateTime(item.ReqDate).ToString("dd/MM/yyyy"),
                DepositReqDate = item.DepositReqDate == null ? "" : Convert.ToDateTime(item.DepositReqDate).ToString("dd/MM/yyyy"),
                Source = item.Source,
                Compaign = item.Compaign,
                Stage = item.Stage,
                job_department = item.job_department,
                ShippingTo = item.ShippingTo,
                Shipping = item.Shipping,
                Price = item.Price,
                Service = item.Service,
                OppNotes = item.OppNotes,
                AcctManagerId = item.AcctManagerId,
                AccountManagerFullName = item.AccountManagerFullName == null ? "" : item.AccountManagerFullName,
                Status = item.Stage == "Opportunity" ? item.Declined : item.Stage == "Quote" ? item.Lost : item.Cancelled
            }
            ).ToList();
            return newdata;
        }
        //20 Aug 2018 (N)


        //28 Aug 2018 (N)
        public tblPurchase GetPurchaseData(int Id)
        {
            var data = _tblPurchase.Get(_ => _.OpportunityId == Id).FirstOrDefault();
            return data;
        }

        //28 Aug 2018 (N)
        public List<PurchaseDetailViewModel> OptionDescForPurchase(int Id)
        {
            var data = _tblPurchaseDetail.Get(_ => _.OpportunityId == Id).ToList();              /*&& _.OptionStage == "Order"*/
            var ItemDesc = Mapper.Map<List<PurchaseDetailViewModel>>(data).ToList();
            return ItemDesc;
        }
        //28 Aug 2018 (N)

        //baans change 31 Aug start
        public List<tblCommonData> AuthdataListByDesc()
        {
            return _tblCommonData.Get(_ => _.FieldDescription == "Authentication").ToList();
        }
        public List<OptionViewModel> getOptionsForInvoice(int OpportunityID, string Status)
        {
            var data = _st_optionRepository.Get(_ => _.OpportunityId == OpportunityID && _.OptionStage == Status && _.include_job == true);
            var OptionData = Mapper.Map<List<OptionViewModel>>(data).ToList();
            return OptionData;


        }
        public bool UpdateTOken(string Token, string Type)
        {
            bool result = false;
            try
            {
                var tokendata = _tblCommonData.Get(_ => _.FieldName == Type).FirstOrDefault();
                if (tokendata != null)
                {
                    tokendata.FieldValue = Token;
                    _tblCommonData.Update(tokendata);
                    _tblCommonData.Save();
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }


            return result;
        }
        //baans change 31 Aug end

        //25 Sep 2018 (N)
        public bool GetOptionStatus(int Oppid)
        {
            var result = false;
            var oppdata = _st_optionRepository.Get(_ => _.OpportunityId == Oppid).FirstOrDefault();
            if (oppdata != null)
            {
                result = true;
            }
            return result;
        }
        //25 Sep 2018 (N)
        // baans change 23rd October for Deleting the Option
        public ResponseViewModel DeleteOption(int Id)
        {
            KEN_DataAccess.tbloption Entity = new KEN_DataAccess.tbloption();
            var IsOption = _st_optionRepository.Get(_ => _.id == Id).FirstOrDefault();
            if (IsOption != null)
            {
                _st_optionRepository.Delete(IsOption);
                _st_optionRepository.Save();
                response.Message = "Data Deleted Successfully";
                response.Result = ResponseType.Success;
            }
            return response;
        }
        // baans end 23rd October

        // 13 Nov 2018 (N)
        public bool OrgData(int OrgId)
        {
            var result = false;
            var orgdata = _tblOrganisation.Get(_ => _.OrgId == OrgId).FirstOrDefault();
            if (orgdata.OrgName != "Not Assigned")
            {
                result = true;
            }
            return result;
        }
        // 13 Nov 2018 (N)

        // baans change 10th January for checking Opportunity Status
        public bool StatusChkByOppIdForMakeRepeat(int OppId)
        {
            bool Result = false;
            var Stage = DbContext.tblOpportunities.Where(_ => _.OpportunityId == OppId).Select(_ => _.Stage).FirstOrDefault();
            if (Stage == "Complete")
            {
                Result = true;
            }
            return Result;
        }
        public ResponseViewModel MakeRepeatOrder(int OppId)
        {
            var OpportunityId = DbContext.Pro_MakeRepeatOrder(OppId).FirstOrDefault();
            response.Message = ResponseMessage.SuccessMessage;
            response.ID = Convert.ToInt32(OpportunityId);
            response.Result = ResponseType.Success;
            response.tblName = TableNames.tblOpportunity;

            return response;

        }
        // baans end 10th January
        //baans change 16th Jan
        public List<OptionCodeBrandItemViewModel> GetOptionByPrefix(string Prefix)
        {
            List<OptionCodeBrandItemViewModel> Data = new List<OptionCodeBrandItemViewModel>();
            Data = Mapper.Map<List<OptionCodeBrandItemViewModel>>(_tblOptionCodeRepository.Get(_ => _.Code.Contains(Prefix))).ToList();
            return Data;

        }
        //baans end 16th Jan
        // 29 April NotesEditing List
        public ResponseViewModel UpdateStatusNotes(int id, string stage, string notes)
        {
            var entity = _tblOpportunityRepository.Get(_ => _.OpportunityId == id).FirstOrDefault();

            switch (entity.Stage)
            {
                case "Opportunity":
                    {
                        entity.OppNotes = notes;
                        break;
                    }
                case "Declined":
                    {
                        entity.OppNotes = notes;
                        break;
                    }
                case "Quote":
                    {
                        entity.QuoteNotes = notes;
                        break;
                    }
                case "Lost":
                    {
                        entity.QuoteNotes = notes;
                        break;
                    }
                case "Order":
                    {
                        entity.OrderNotes = notes;
                        break;
                    }
                case "Cancelled":
                    {
                        entity.OrderNotes = notes;
                        break;
                    }
                case "Job":
                    {
                        entity.JobNotes = notes;
                        break;
                    }
                case "Order Confirmed":
                    {
                        entity.JobNotes = notes;
                        break;
                    }
                case "Order Packed":
                    {
                        entity.PackingNotes = notes;
                        break;
                    }
                case "Order Invoiced":
                    {
                        entity.InvoicingNotes = notes;
                        break;
                    }
                case "Complete":
                    {
                        entity.CompleteNotes = notes;
                        break;
                    }
                case "Shipping":
                    {
                        entity.ShippingNotes = notes;
                        break;
                    }
            }
            entity.UpdatedBy = DataBaseCon.ActiveUser();
            entity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));

            _tblOpportunityRepository.Update(entity);
            _tblOpportunityRepository.Save();
            response.Message = ResponseMessage.SuccessMessage;
            response.ID = entity.OpportunityId;
            response.Result = ResponseType.Success;
            response.tblName = TableNames.tblOpportunity;
            return response;
        }
        // 29 April NotesEditing List

        //29 April Stage Change List
        public bool GetJobStatusByOppId(int OppId, string lblDate)
        {
            var result = false;
            switch (lblDate)
            {
                case "OrderConfirmedDate":
                    {
                        var Data = DbContext.tblOpportunities.Where(_ => _.OpportunityId == OppId).FirstOrDefault();
                        if (Data.OrderConfirmedDate != null)
                        {
                            result = true;
                        }
                        break;
                    }
                case "ArtOrderedDate":
                    {
                        var Data = DbContext.tblOpportunities.Where(_ => _.OpportunityId == OppId).FirstOrDefault();
                        if (Data.ArtOrderedDate != null)
                        {
                            result = true;
                        }
                        break;
                    }
                case "ApprovedDate":
                    {
                        var Data = DbContext.tblOpportunities.Where(_ => _.OpportunityId == OppId).FirstOrDefault();
                        if (Data.ApprovedDate != null)
                        {
                            result = true;
                        }
                        break;
                    }
                case "JobDate":
                    {
                        var Data = DbContext.tblOpportunities.Where(_ => _.OpportunityId == OppId).FirstOrDefault();
                        if (Data.JobDate != null || Data.ArtReadyDate != null)
                        {
                            result = true;
                        }
                        break;
                    }
                case "StockOrderedDate":
                    {
                        var Data = DbContext.tblOpportunities.Where(_ => _.OpportunityId == OppId).FirstOrDefault();
                        if (Data.StockOrderedDate != null)
                        {
                            result = true;
                        }
                        break;
                    }
                case "ReceivedDate":
                    {
                        var Data = DbContext.tblOpportunities.Where(_ => _.OpportunityId == OppId).FirstOrDefault();
                        if (Data.ReceivedDate != null)
                        {
                            result = true;
                        }
                        break;
                    }
            }
            return result;
        }
        //29 April Stage Change List
    }

}