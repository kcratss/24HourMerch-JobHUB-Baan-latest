using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KEN_DataAccess;
using KEN.Interfaces;
using KEN.Interfaces.Iservices;
using AutoMapper;
using KEN.Interfaces.Repository;
using KEN.Models;
using System.Text.RegularExpressions;
using KEN.AppCode;

namespace KEN.Services
{
    public class OrganisationService : IOrganisationService
    {
        private readonly IRepository<tblOrganisation> _tblOrganisationRepository;
        private readonly IRepository<Vw_tblOrganisation> _VW_tblOrganisationRepository;
        private readonly IRepository<tblAddress> _tblAddressRepository;
        private readonly IRepository<tblcontact> _tblcontactRepository;
        private readonly IRepository<tblPurchase> _tblPurchaseRepository;   //1 Sep 2018 (N)
        KENNEWEntities DbContext = new KENNEWEntities();  //tarun 08/09/2018
        private readonly IRepository<tblOpportunity> _tblOpportunityRepository; //tarun 18Sept
        ResponseViewModel response = new ResponseViewModel();

        public OrganisationService(IRepository<tblOrganisation> tblOrganisationRepository, IRepository<tblAddress> tblAddressRepository, IRepository<tblOpportunity> tblOpportunityRepository, IRepository<Vw_tblOrganisation> VW_tblOrganisationRepository, IRepository<tblcontact> tblcontactRepository, IRepository<tblPurchase> tblPurchaseRepository)
        {
            _tblOrganisationRepository = tblOrganisationRepository;
            _tblAddressRepository = tblAddressRepository;
            _VW_tblOrganisationRepository = VW_tblOrganisationRepository;
            _tblcontactRepository = tblcontactRepository;
            _tblOpportunityRepository = tblOpportunityRepository;
            _tblPurchaseRepository = tblPurchaseRepository;     //1 Sep 2018 (N)
        }
        public bool Add(tblOrganisation entity)
        {
            throw new NotImplementedException();
        }
        public ResponseViewModel BatchTransaction(tblOrganisation Entity, BatchOperation operation)
        {
            throw new NotImplementedException();
        }
        public ResponseViewModel OrgBatchTransaction(tblOrganisation Entity, string PageSource, int ContactID, BatchOperation operation, int PurchaseId)  //1 Sep 2018 (N)
        {
            string tblName = "";
            try
            {

                switch (operation)
                {
                    case BatchOperation.Insert:
                        {
                            tblName = TableNames.tblOrganisation;
                            Entity.CreatedBy = DataBaseCon.ActiveUser();
                            Entity.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                            if (Entity.TradingName == "" || Entity.TradingName == null)
                            {
                                Entity.TradingName = Entity.OrgName;
                            }
                            if (Entity.MainPhone != null && Entity.MainPhone != "")
                            {
                                Entity.MainPhone = Regex.Replace(Entity.MainPhone, @"[^0-9]", "");
                            }

                            _tblOrganisationRepository.Insert(Entity);

                            _tblOrganisationRepository.Save();
                            if (PageSource != "OrganisationDetail")
                            {
                                tblName = TableNames.tblContact;
                                var Contactdata = _tblcontactRepository.Get(_ => _.id == ContactID).FirstOrDefault();
                                if (Contactdata != null)
                                {
                                    Contactdata.OrgId = Entity.OrgId;
                                    Contactdata.UpdatedBy = DataBaseCon.ActiveUser();
                                    Contactdata.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                    _tblcontactRepository.Update(Contactdata);
                                    _tblcontactRepository.Save();
                                }
                            }
                            //1 Sep 2018 (N)
                            if (PageSource == "PurchaseDetails")
                            {
                                var entity = _tblPurchaseRepository.Get(_ => _.PurchaseId == PurchaseId).FirstOrDefault();
                                //var entity = _tblOrganisationRepository.Get(_ => _.OrgId == Entity.OrgId).FirstOrDefault();
                                entity.OrgId = Entity.OrgId;
                                _tblPurchaseRepository.Update(entity);
                                _tblPurchaseRepository.Save();
                            }
                            //1 Sep 2018 (N)

                            response.ID = Entity.OrgId;
                            response.Message = ResponseMessage.SuccessMessage;
                            response.Result = ResponseType.Success;
                            response.tblName = TableNames.tblOrganisation;
                            break;
                        }

                    case BatchOperation.Delete:
                        {
                            var entity = _tblOrganisationRepository.Get(_ => _.OrgId == Entity.OrgId).FirstOrDefault();

                            if (entity != null)
                            {
                                _tblOrganisationRepository.Delete(entity);
                                _tblOrganisationRepository.Save();
                            }
                            response.ID = Entity.OrgId;
                            response.Message = ResponseMessage.SuccessMessage;
                            response.Result = ResponseType.Success;
                            response.tblName = TableNames.tblOrganisation;
                            break;
                        }
                    default:
                        {

                            tblName = TableNames.tblOrganisation;
                            var entity = _tblOrganisationRepository.Get(_ => _.OrgId == Entity.OrgId).FirstOrDefault();

                            if (entity != null)
                            {
                                entity.OrgName = Entity.OrgName;
                                entity.TradingName = Entity.TradingName;
                                entity.ABN = Entity.ABN;
                                if (Entity.MainPhone != null && Entity.MainPhone != "")
                                {
                                    entity.MainPhone = Regex.Replace(Entity.MainPhone, @"[^0-9]", "");
                                }
                                entity.WebAddress = Entity.WebAddress;
                                entity.Industry = Entity.Industry;
                                entity.OrgNotes = Entity.OrgNotes;
                                entity.AcctMgrId = Entity.AcctMgrId;
                                entity.OrganisationType = Entity.OrganisationType;
                                entity.UpdatedBy = DataBaseCon.ActiveUser();
                                entity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                entity.EmailAddress = Entity.EmailAddress;


                                _tblOrganisationRepository.Update(entity);
                                _tblOrganisationRepository.Save();
                            }

                            //1 Sep 2018 (N)
                            if (PageSource == "PurchaseDetails")
                            {
                                var Purchasedata = _tblPurchaseRepository.Get(_ => _.PurchaseId == PurchaseId).FirstOrDefault();
                                if (entity != null)
                                {
                                    //var entity = _tblOrganisationRepository.Get(_ => _.OrgId == Entity.OrgId).FirstOrDefault();
                                    Purchasedata.UpdatedBy = DataBaseCon.ActiveUser();
                                    Purchasedata.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                    Purchasedata.OrgId = Entity.OrgId;
                                    _tblPurchaseRepository.Update(Purchasedata);
                                    _tblPurchaseRepository.Save();
                                }
                                else
                                {
                                    response.Message = "Please Save the Purchase First";
                                    response.Result = ResponseType.Error;
                                    //response.tblName = tblName;
                                    response.ErrorCode = 101;
                                    break;


                                }
                            }
                            //1 Sep 2018 (N)

                            else
                            {
                                tblName = TableNames.tblContact;
                                var Contactdata = _tblcontactRepository.Get(_ => _.id == ContactID).FirstOrDefault();
                                if (Contactdata != null)
                                {
                                    Contactdata.OrgId = Entity.OrgId;
                                    Contactdata.UpdatedBy = DataBaseCon.ActiveUser();
                                    Contactdata.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                    _tblcontactRepository.Update(Contactdata);
                                    _tblcontactRepository.Save();
                                }
                            }
                            response.ID = Entity.OrgId;
                            response.Message = ResponseMessage.SuccessMessage;
                            response.Result = ResponseType.Success;
                            response.tblName = TableNames.tblOrganisation;
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Result = ResponseType.Error;
                response.tblName = tblName;
                response.ErrorCode = ex.HResult;
            }

            return response;
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<tblOrganisation> GetAll()
        {
            throw new NotImplementedException();
        }

        public tblOrganisation GetById(int id)
        {
            throw new NotImplementedException();
        }



        public ResponseViewModel Update(tblOrganisation entity)
        {
            throw new NotImplementedException();
        }

        ResponseViewModel IOrganisationService.OrganisationCheck(tblOrganisation entity, string PageSource, int ContactID, int PurchaseId)       //1 Sep 2018(N)
        {
            if (entity != null)
            {
                //var GetOrganisationData = _tblOrganisationRepository.Get(_ => _.TradingName == entity.TradingName && _.OrgId == entity.OrgId).FirstOrDefault();
                var GetOrganisationData = _tblOrganisationRepository.Get(_ => _.OrgId == entity.OrgId).FirstOrDefault();
                if (GetOrganisationData != null)
                {
                    response = OrgBatchTransaction(entity, PageSource, ContactID, BatchOperation.Update, PurchaseId);     //1 Sep 2018 (N)
                }
                else
                {
                    response = OrgBatchTransaction(entity, PageSource, ContactID, BatchOperation.Insert, PurchaseId);        //1 Sep 2018 (N)
                }
            }

            return response;
        }

        ResponseViewModel IOrganisationService.UpdateAddress(tblAddress entity, string PageSource, int PurchaseId, int OppId)      //tarun 18/09/2018
        {
            try
            {
                if (entity != null)
                {
                    var Entity = _tblAddressRepository.Get(_ => _.OrgId == entity.OrgId && _.DeliveryAddress == entity.DeliveryAddress).FirstOrDefault();
                    if (Entity != null)
                    {
                        //if (Entity != null)
                        //{
                        Entity.TradingName = entity.TradingName;
                        Entity.Attention = entity.Attention;
                        Entity.DeliveryAddress = entity.DeliveryAddress;
                        Entity.Address1 = entity.Address1;
                        Entity.Address2 = entity.Address2;
                        Entity.State = entity.State;
                        Entity.Postcode = entity.Postcode;
                        Entity.AddNotes = entity.AddNotes;
                        Entity.UpdatedBy = DataBaseCon.ActiveUser();
                        Entity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));

                        _tblAddressRepository.Update(Entity);
                        _tblAddressRepository.Save();

                        //tarun 06/09/2018
                        if (PageSource == "PurchaseDetails")
                        {
                            var AddressEntity = _tblPurchaseRepository.Get(_ => _.PurchaseId == PurchaseId).FirstOrDefault();
                            //var entity = _tblOrganisationRepository.Get(_ => _.OrgId == Entity.OrgId).FirstOrDefault();
                            AddressEntity.UpdatedBy = DataBaseCon.ActiveUser();
                            AddressEntity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                            AddressEntity.DeliveryToId = entity.AddressId;
                            _tblPurchaseRepository.Update(AddressEntity);
                            _tblPurchaseRepository.Save();
                        }
                        //end

                        //tarun 18/09/2018
                        if (PageSource == "InvoicingDetails" || PageSource == "PackingDetails" || PageSource == "JobDetails" || PageSource == "CompleteDetails" || PageSource == "ShippingDetails")
                        {
                            var AddressEntity = _tblOpportunityRepository.Get(_ => _.OpportunityId == OppId).FirstOrDefault();
                            //var entity = _tblOrganisationRepository.Get(_ => _.OrgId == Entity.OrgId).FirstOrDefault();
                            AddressEntity.UpdatedBy = DataBaseCon.ActiveUser();
                            AddressEntity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                            AddressEntity.AddressId = entity.AddressId;
                            _tblOpportunityRepository.Update(AddressEntity);
                            _tblOpportunityRepository.Save();
                        }
                        //end
                        response.ID = Entity.AddressId;
                        response.Message = ResponseMessage.SuccessMessage;
                        response.Result = ResponseType.Success;
                        response.tblName = "Address";
                    }
                    else
                    {
                        entity.CreatedBy = DataBaseCon.ActiveUser();
                        entity.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                        _tblAddressRepository.Insert(entity);
                        _tblAddressRepository.Save();
                        //tarun 12/09/2018
                        if (PageSource == "PurchaseDetails")
                        {
                            var AddressEntity = _tblPurchaseRepository.Get(_ => _.PurchaseId == PurchaseId).FirstOrDefault();
                            //var entity = _tblOrganisationRepository.Get(_ => _.OrgId == Entity.OrgId).FirstOrDefault();
                            AddressEntity.UpdatedBy = DataBaseCon.ActiveUser();
                            AddressEntity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                            AddressEntity.DeliveryToId = entity.AddressId;
                            _tblPurchaseRepository.Update(AddressEntity);
                            _tblPurchaseRepository.Save();
                        }
                        //end

                        //tarun 18/09/2018
                        if (PageSource == "InvoicingDetails" || PageSource == "PackingDetails" || PageSource == "JobDetails" || PageSource == "CompleteDetails" || PageSource == "ShippingDetails")
                        {
                            var AddressEntity = _tblOpportunityRepository.Get(_ => _.OpportunityId == OppId).FirstOrDefault();
                            //var entity = _tblOrganisationRepository.Get(_ => _.OrgId == Entity.OrgId).FirstOrDefault();
                            AddressEntity.UpdatedBy = DataBaseCon.ActiveUser();
                            AddressEntity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                            AddressEntity.AddressId = entity.AddressId;
                            _tblOpportunityRepository.Update(AddressEntity);
                            _tblOpportunityRepository.Save();
                        }
                        //end

                        response.ID = entity.AddressId;
                        response.Message = ResponseMessage.SuccessMessage;
                        response.Result = ResponseType.Success;
                        response.tblName = "Address";

                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Result = ResponseType.Error;
                response.ErrorCode = ex.HResult;
                response.tblName = "Address";
            }

            return response;
        }

        // baans change 27th September for Autocomplete  by type
        public IEnumerable<OrganisationViewModel> GetOrganisationByPrefix(string Prefix, string OrgType)
        {
            IEnumerable<tblOrganisation> data;
            //if (PageSource == "PurchaseDetails")
            //{
            //     data = _tblOrganisationRepository.Get(_ => _.OrgName.Contains(Prefix) && _.OrganisationType == "Supplier");
            //}
            //else
            //{
            data = _tblOrganisationRepository.Get(_ => _.OrgName.Contains(Prefix) && _.OrganisationType == OrgType);
            // baans end 27th September
            // }
            var newdata = data.Select(item => new OrganisationViewModel
            {
                OrgId = item.OrgId,
                OrgName = item.OrgName,
                TradingName = item.TradingName,
                ABN = item.ABN,
                MainPhone = item.MainPhone,
                WebAddress = item.WebAddress,
                Brand = item.Brand == null ? "" : item.Brand,
                OrgNotes = item.OrgNotes,
                OrganisationType = item.OrganisationType,
                AcctMgrId = item.AcctMgrId,
                EmailAddress = item.EmailAddress
            }).ToList();

            return newdata;
        }

        public tblOrganisation GetOrganisationById(int OrgId)
        {
            return _tblOrganisationRepository.Get(_ => _.OrgId == OrgId).FirstOrDefault();
        }
        public List<OrganisationViewModel> GetOrganisationList(string Type)
        {
            List<OrganisationViewModel> list = new List<OrganisationViewModel>();
            if (Type == "All")
            {
                list = Mapper.Map<List<OrganisationViewModel>>(_VW_tblOrganisationRepository.Get()).ToList();
            }
            else
            {
                list = Mapper.Map<List<OrganisationViewModel>>(_VW_tblOrganisationRepository.Get()).ToList();
                list = list.Where(e => e.OrganisationType == Type).ToList();
            }
            return list;


        }

        //tarun 22/08/2018
        public tblOrganisation GetOrgByType()
        {
            var OrgType = "JobHub24Hour";   //tarun 26/09/2018
            var data = _tblOrganisationRepository.Get(_ => _.OrganisationType == OrgType).FirstOrDefault();
            //var newdata = _tblAddressRepository.Get(_ => _.OrgId == data.OrgId).FirstOrDefault();
            return data;
        }
        //end

        //tarun 08/09/2018
        public List<OrganisationViewModel> GetCustomOrganisationList(string CustomText, string TableName)
        {
            var CustomContact = Mapper.Map<List<OrganisationViewModel>>(DbContext.Database.SqlQuery<Vw_tblOrganisation>("exec Pro_Search '" + TableName + "','" + CustomText + "'").ToList()).ToList();
            //  DbContext.Pro_Search
            return CustomContact;
        }
        //end

        //27 Dec 2018 (N)
        public ResponseViewModel AddIntuitID(int orgid, string IntutitId)
        {
            var entity = _tblOrganisationRepository.Get(_ => _.OrgId == orgid).FirstOrDefault();

            if (entity != null)
            {
                entity.IntuitID = Convert.ToInt32(IntutitId);
                entity.UpdatedBy = DataBaseCon.ActiveUser();
                entity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
            }


            _tblOrganisationRepository.Update(entity);
            _tblOrganisationRepository.Save();

            //response.ID = Entity.OrgId;
            response.Message = ResponseMessage.SuccessMessage;
            //response.Result = ResponseType.Success;
            //response.tblName = TableNames.tblOrganisation;

            return response;
        }
        //27 Dec 2018 (N)
        public tblAddress GetOrganisationAddress(int OrgId)
        {
            return _tblAddressRepository.Get(_ => _.OrgId == OrgId && _.DeliveryAddress == "Delivery1").FirstOrDefault();
        }

    }
}