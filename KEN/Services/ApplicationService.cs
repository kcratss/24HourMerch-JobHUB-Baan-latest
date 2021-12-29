using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KEN.Interfaces;
using KEN.Interfaces.Iservices;
using KEN_DataAccess;
using KEN.Interfaces.Repository;
using KEN.Models;
using KEN.AppCode;
using AutoMapper;

namespace KEN.Services
{
    public class ApplicationService : IApplicationService
    {
        ResponseViewModel response = new ResponseViewModel();
        KENNEWEntities dbcontext = new KENNEWEntities();

        private readonly IRepository<tblApplicationStatu> _tblApplicationStatus;
        private readonly IRepository<tblApplicationArtSuppplier> _tblApplicationArtSuppplier;
        private readonly IRepository<tblApplicationType> _tblApplicationType;
        private readonly IRepository<tblApplicationArt> _tblApplicationArt;
        private readonly IRepository<tblApplicationDesigner> _tblApplicationDesigner;
        private readonly IRepository<TblApplication> _tblApplication;
        private readonly IRepository<TblApplicationColour> _tblApplicationColours;
        private readonly IRepository<TblApplicationColoursMapping> _tblApplicationColoursMapping;
        private readonly IRepository<tblPantoneMaster> _tblPantoneMaster;
        private readonly IRepository<TblApplicationCustomInfo> _tblApplicationCustomInfo;
        private readonly IRepository<tblApplicationCustomInfoMapping> _tblApplicationCustomInfoMapping;

        public ApplicationService(IRepository<tblApplicationStatu> tblApplicationStatus, IRepository<tblApplicationArtSuppplier> tblApplicationArtSuppplier, IRepository<tblApplicationType> tblApplicationType, IRepository<tblApplicationArt> tblApplicationArt, IRepository<tblApplicationDesigner> tblApplicationDesigner, IRepository<TblApplication> tblApplication, IRepository<TblApplicationColour> tblApplicationColours, IRepository<TblApplicationColoursMapping> tblApplicationColoursMapping, IRepository<tblPantoneMaster> tblPantoneMaster, IRepository<TblApplicationCustomInfo> tblApplicationCustomInfo, IRepository<tblApplicationCustomInfoMapping> tblApplicationCustomInfoMapping)
        {
            _tblApplicationStatus = tblApplicationStatus;
            _tblApplicationArtSuppplier = tblApplicationArtSuppplier;
            _tblApplicationType = tblApplicationType;
            _tblApplicationArt = tblApplicationArt;
            _tblApplicationDesigner = tblApplicationDesigner;
            _tblApplication = tblApplication;
            _tblApplicationColours = tblApplicationColours;
            _tblApplicationColoursMapping = tblApplicationColoursMapping;
            _tblPantoneMaster = tblPantoneMaster;
            _tblApplicationCustomInfo = tblApplicationCustomInfo;
            _tblApplicationCustomInfoMapping = tblApplicationCustomInfoMapping;
        }
        public bool Add(TblApplication entity)
        {
            throw new NotImplementedException();
        }

        public ResponseViewModel BatchTransaction(TblApplication entitity, BatchOperation operation)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TblApplication> GetAll()
        {
            throw new NotImplementedException();
        }

        public TblApplication GetById(int id)
        {
            throw new NotImplementedException();
        }

        public ResponseViewModel Update(TblApplication entity)
        {
            throw new NotImplementedException();
        }

        public List<tblApplicationStatu> GetApplicationStatus()
        {
            return _tblApplicationStatus.Get(_ => _.Status == "Active").OrderBy(_ => _.ApplicationStatus).ToList();
        }
        public List<tblApplicationArtSuppplier> GetApplicationArtSupplier()
        {
            return _tblApplicationArtSuppplier.Get(_ => _.Status == "Active").OrderBy(_ => _.SupplierName).ToList();
        }
        public ResponseViewModel SaveArtSupplier(string ArtSupplierName)
        {
            tblApplicationArtSuppplier entity = new tblApplicationArtSuppplier();

            entity.SupplierName = ArtSupplierName;
            entity.CreatedBy = DataBaseCon.ActiveUser();
            entity.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
            entity.Status = "Active";
            _tblApplicationArtSuppplier.Insert(entity);
            _tblApplicationArtSuppplier.Save();

            response.Message = ResponseMessage.SuccessMessage;
            response.ID = entity.Id;
            response.Result = entity.SupplierName;
            return response;
        }

        public List<tblApplicationType> GetApplicationType()
        {
            return _tblApplicationType.Get(_ => _.Status == "Active").OrderBy(_ => _.ApplicationType).ToList();
        }

        public List<tblApplicationArt> GetApplicationArt()
        {
            return _tblApplicationArt.Get(_ => _.Status == "Active").OrderBy(_ => _.Art).ToList();
        }

        public List<tblApplicationDesigner> GetApplicationDesigner()
        {
            return _tblApplicationDesigner.Get(_ => _.Status == "Active").OrderBy(_ => _.DesignerName).ToList();
        }

        public ResponseViewModel ApplicationBatchTransaction(TblApplication Entity, BatchOperation operation)
        {
            try
            {
                switch (operation)
                {
                    case BatchOperation.Insert:
                        {
                            Entity.DecorationDate = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                            Entity.CreatedBy = DataBaseCon.ActiveUser();
                            Entity.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));

                            _tblApplication.Insert(Entity);
                            _tblApplication.Save();

                            response.Message = ResponseMessage.SuccessMessage;
                            response.ID = Entity.ApplicationId;
                            response.Result = ResponseType.Success;
                            response.tblName = TableNames.tblApplication;

                            break;
                        }
                    case BatchOperation.Delete:
                        {
                            break;
                        }
                    default:
                        {
                            var entity = _tblApplication.Get(_ => _.ApplicationId == Entity.ApplicationId).FirstOrDefault();

                            if(entity != null)
                            {
                                entity.AppName = Entity.AppName;
                                entity.AppType = Entity.AppType;
                                entity.AppWidth = Entity.AppWidth;
                                entity.Art = Entity.Art;
                                entity.ArtNotes = Entity.ArtNotes;
                                entity.Bill = Entity.Bill;
                                entity.Production = Entity.Production;
                                entity.ProductionNotes = Entity.ProductionNotes;
                                entity.ArtSupplier = Entity.ArtSupplier;
                                entity.Designer = Entity.Designer;
                                entity.DesignerNotes = Entity.DesignerNotes;
                                entity.AppStatus = Entity.AppStatus;
                                entity.ProofNotes = Entity.ProofNotes;
                                entity.AcctMgrId = Entity.AcctMgrId;
                                entity.UpdatedBy = DataBaseCon.ActiveUser();
                                entity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                entity.Link = Entity.Link;

                                _tblApplication.Update(entity);
                                _tblApplication.Save();
                            }
                            response.Message = ResponseMessage.SuccessMessage;
                            response.ID = Entity.ApplicationId;
                            response.Result = ResponseType.Success;
                            response.tblName = TableNames.tblApplication;

                            break;
                        }
                }
            }
            catch(Exception Ex)
            {
                response.Message = Ex.Message;
                response.Result = ResponseType.Error;
                response.ErrorCode = Ex.HResult;
                response.tblName = TableNames.tblApplication;
            }
            return response;
        }
        public ApplicationViewModel GetApplicationById(int ApplicationId)
        {
            return Mapper.Map<ApplicationViewModel>(_tblApplication.Get(_ => _.ApplicationId == ApplicationId).FirstOrDefault());
        }

        public ResponseViewModel ApplicationColoursBatchTransaction(TblApplicationColour Entity, int ApplicationId, BatchOperation operation)
        {
            try
            {
                switch (operation)
                {
                    case BatchOperation.Insert:
                        {
                            Entity.CreatedBy = DataBaseCon.ActiveUser();
                            Entity.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                            _tblApplicationColours.Insert(Entity);
                            _tblApplicationColours.Save();

                            TblApplicationColoursMapping data = new TblApplicationColoursMapping();

                            data.ApplicationId = ApplicationId;
                            data.ApplicationColourId = Entity.ApplicationColourId;
                            data.CreatedBy = DataBaseCon.ActiveUser();
                            data.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));

                            _tblApplicationColoursMapping.Insert(data);
                            _tblApplicationColoursMapping.Save();

                            response.Message = ResponseMessage.SuccessMessage;
                            response.ID = Entity.ApplicationColourId;
                            response.Result = ResponseType.Success;
                            response.tblName = TableNames.tblApplictaionColours;
                            break;
                        }
                    case BatchOperation.Delete:
                        {
                            if(Entity.ApplicationColourId > 0)
                            {
                                var entity = _tblApplicationColours.Get(_ => _.ApplicationColourId == Entity.ApplicationColourId).FirstOrDefault();

                                if(entity != null)
                                {
                                    var data = _tblApplicationColoursMapping.Get(_ => _.ApplicationColourId == Entity.ApplicationColourId && _.ApplicationId == ApplicationId).FirstOrDefault();
                                    if(data != null)
                                    {
                                        _tblApplicationColoursMapping.Delete(data);
                                        _tblApplicationColoursMapping.Save();
                                    }
                                    _tblApplicationColours.Delete(entity);
                                    _tblApplicationColours.Save();

                                    response.Message = ResponseMessage.SuccessMessage;
                                    //response.ID = Entity.ApplicationColourId;
                                    response.Result = ResponseType.Success;
                                    response.tblName = TableNames.tblApplictaionColours;
                                }
                            }
                            break;
                        }
                    default:
                        {
                            var entity = _tblApplicationColours.Get(_ => _.ApplicationColourId == Entity.ApplicationColourId).FirstOrDefault();

                            if(entity != null)
                            {
                                entity.ApplicationColourId = Entity.ApplicationColourId;
                                entity.ColourWayNo = Entity.ColourWayNo;
                                entity.GarmentColour = Entity.GarmentColour;
                                entity.InkId = Entity.InkId;
                                entity.InkColour = Entity.InkColour;
                                entity.ThreadId = Entity.ThreadId;
                                entity.ThreadColour = Entity.ThreadColour;
                                entity.Pantone = Entity.Pantone;
                                entity.PrintOrder = Entity.PrintOrder;
                                entity.Mesh = Entity.Mesh;
                                entity.Flash = Entity.Flash;
                                entity.TransferColour = Entity.TransferColour;
                                entity.Substrate = Entity.Substrate;
                                entity.ColourNotes = Entity.ColourNotes;
                                entity.UpdatedBy = DataBaseCon.ActiveUser();
                                entity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));

                                _tblApplicationColours.Update(entity);
                                _tblApplicationColours.Save();
                            }
                            response.Message = ResponseMessage.SuccessMessage;
                            response.ID = Entity.ApplicationColourId;
                            response.Result = ResponseType.Success;
                            response.tblName = TableNames.tblApplictaionColours;
                            break;
                        }
                }
            }
            catch (Exception Ex)
            {
                response.Message = Ex.Message;
                response.Result = ResponseType.Error;
                response.ErrorCode = Ex.HResult;
                response.tblName = TableNames.tblApplictaionColours;
            }
            return response;
        }
        public List<ApplicationColourViewModel> GetApplicationColoursGrid(int ApplicationId)
        {
            List<ApplicationColourViewModel> AppColors = new List<ApplicationColourViewModel>();

            var data = _tblApplicationColoursMapping.Get(_ => _.ApplicationId == ApplicationId).ToList();
            if(data.Count > 0)
            {
                foreach(var item in data)
                {
                    var colours = Mapper.Map<ApplicationColourViewModel>(_tblApplicationColours.Get(_ => _.ApplicationColourId == item.ApplicationColourId).FirstOrDefault());

                    AppColors.Add(colours);
                }
            }
            return AppColors;
        }
        public List<ApplicationViewModel> GetApplicationGridData(string ApplicationType)
        {
            List<ApplicationViewModel> Applicationlist = new List<ApplicationViewModel>();

            //by sandeep InActive Condition (24Sept)
            if (ApplicationType == "All")
            {
                Applicationlist = Mapper.Map<List<ApplicationViewModel>>(_tblApplication.Get(_ =>_.AppStatus != "InActive").OrderByDescending(_ => _.ApplicationId));
            }
            else if(ApplicationType == "Primo Colour")
            {
                Applicationlist = Mapper.Map<List<ApplicationViewModel>>(_tblApplication.Get(_ => _.AppType == "Supa Colour Transfer" || _.AppType == "Ultra ColourTransfer" && _.AppStatus != "InActive").OrderByDescending(_ => _.ApplicationId));
            }
            else if(ApplicationType == "Vinyl Transfer")
            {
                Applicationlist = Mapper.Map<List<ApplicationViewModel>>(_tblApplication.Get(_ => _.AppType == "Sports Number" || _.AppType == "Vinyl Cut Transfer" && _.AppStatus != "InActive").OrderByDescending(_ => _.ApplicationId));
            }
            else if (ApplicationType == "InActive")
            {
                Applicationlist = Mapper.Map<List<ApplicationViewModel>>(_tblApplication.Get(_ => _.AppStatus == ApplicationType).OrderByDescending(_ => _.ApplicationId));
            }
            else
            {
                Applicationlist = Mapper.Map<List<ApplicationViewModel>>(_tblApplication.Get(_ => _.AppType == ApplicationType && _.AppStatus != "InActive").OrderByDescending(_ => _.ApplicationId));
            }
            return Applicationlist;
        }
        public List<ApplicationViewModel> GetApplicationCustomData(string CustomText, string TableName)
        {
            var CustomApplicationData = Mapper.Map<List<ApplicationViewModel>>(dbcontext.Database.SqlQuery<ApplicationViewModel>("exec Pro_Search '" + TableName + "','" + CustomText + "'").ToList()).ToList();

            return CustomApplicationData;
        }
        public List<ApplicationJobListViewModel> ApplicationJobsList(int ApplicationId)
        {
            List<ApplicationJobListViewModel> JobsList = Mapper.Map<List<ApplicationJobListViewModel>>(dbcontext.Pro_AppplicationJobsDetail(ApplicationId));
            return JobsList;
        }

        public IEnumerable<PantoneMasterViewModel> GetPantone(string Prefix)
        {
            List<PantoneMasterViewModel> Data = Mapper.Map<List<PantoneMasterViewModel>>(_tblPantoneMaster.Get(_ => _.Pantone.Contains(Prefix))).ToList();
            return Data;
        }
        public int SavePantone(tblPantoneMaster Entity)
        {
            var PantoneId = 0;

            var data = _tblPantoneMaster.Get(_ => _.Pantone == Entity.Pantone && _.Hexvalue == Entity.Hexvalue).FirstOrDefault();

            if(data != null)
            {
                data.Pantone = Entity.Pantone;
                //entity.BucketId = Entity.BucketId;
                data.Hexvalue = Entity.Hexvalue;
                data.UpdatedBy = DataBaseCon.ActiveUser();
                data.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                _tblPantoneMaster.Update(data);
                _tblPantoneMaster.Save();

                PantoneId = data.Id;
            }
            else
            {
                Entity.CreatedBy = DataBaseCon.ActiveUser();
                Entity.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                _tblPantoneMaster.Insert(Entity);
                _tblPantoneMaster.Save();

                PantoneId = Entity.Id;
            }
            return PantoneId;
        }
        public ResponseViewModel SaveCustomInfo(TblApplicationCustomInfo Entity, int ApplicationId, BatchOperation operation)
        {
            try
            {
                switch (operation)
                {
                    case BatchOperation.Insert:
                        {
                            Entity.CreatedBy = DataBaseCon.ActiveUser();
                            Entity.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                            _tblApplicationCustomInfo.Insert(Entity);
                            _tblApplicationCustomInfo.Save();

                            tblApplicationCustomInfoMapping CustomMapping = new tblApplicationCustomInfoMapping();
                            CustomMapping.ApplicationId = ApplicationId;
                            CustomMapping.CustomInfoId = Entity.CustomInfoId;
                            CustomMapping.CreatedBy = DataBaseCon.ActiveUser();
                            CustomMapping.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));

                            _tblApplicationCustomInfoMapping.Insert(CustomMapping);
                            _tblApplicationCustomInfoMapping.Save();

                            response.Message = ResponseMessage.SuccessMessage;
                            response.ID = Entity.CustomInfoId;
                            response.Result = ResponseType.Success;
                            response.tblName = TableNames.ApplicationCustom;
                            break;
                        }
                    case BatchOperation.Delete:
                        {
                            var entity = _tblApplicationCustomInfo.Get(_ => _.CustomInfoId == Entity.CustomInfoId).FirstOrDefault();
                            if (entity != null)
                            {
                                var data = _tblApplicationCustomInfoMapping.Get(_ => _.CustomInfoId == Entity.CustomInfoId).FirstOrDefault();
                                if(data != null)
                                {
                                    _tblApplicationCustomInfoMapping.Delete(data);
                                    _tblApplicationCustomInfoMapping.Save();
                                }

                                _tblApplicationCustomInfo.Delete(entity);
                                _tblApplicationCustomInfo.Save();
                            }

                            response.Message = ResponseMessage.SuccessMessage;
                            //response.ID = Entity.CustomInfoId;
                            response.Result = ResponseType.Success;
                            response.tblName = TableNames.ApplicationCustom;
                            break;
                        }
                    default:
                        {
                            var entity = _tblApplicationCustomInfo.Get(_ => _.CustomInfoId == Entity.CustomInfoId).FirstOrDefault();

                            if(entity != null)
                            {
                                entity.FirstName = Entity.FirstName;
                                entity.LastName = Entity.LastName;
                                entity.NickName = Entity.NickName;
                                entity.JersyNumber = Entity.JersyNumber;
                                entity.CustomNotes = Entity.CustomNotes;
                                entity.Garment = Entity.Garment;
                                entity.Garmentcolour = Entity.Garmentcolour;
                                entity.GarmentSize = Entity.GarmentSize;
                                entity.UpdatedBy = DataBaseCon.ActiveUser();
                                entity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));

                                _tblApplicationCustomInfo.Update(entity);
                                _tblApplicationCustomInfo.Save();

                                response.Message = ResponseMessage.SuccessMessage;
                                response.ID = Entity.CustomInfoId;
                                response.Result = ResponseType.Success;
                                response.tblName = TableNames.ApplicationCustom;
                            }
                            break;
                        }
                }
            }
            catch(Exception Ex)
            {
                response.Message = Ex.Message;
                response.Result = ResponseType.Error;
                response.ErrorCode = Ex.HResult;
                response.tblName = TableNames.ApplicationCustom;
            }

            return response;
        }

        public List<ApplicationCustomInfoViewModel> GetApplicationCustomGrid(int ApplicationId)
        {

            List<ApplicationCustomInfoViewModel> CustomDecoration = new List<ApplicationCustomInfoViewModel>();

            var data = _tblApplicationCustomInfoMapping.Get(_ => _.ApplicationId == ApplicationId).ToList();
            if (data.Count > 0)
            {
                foreach (var item in data)
                {
                    var CustomDec = Mapper.Map<ApplicationCustomInfoViewModel>(_tblApplicationCustomInfo.Get(_ => _.CustomInfoId == item.CustomInfoId).FirstOrDefault());

                    CustomDecoration.Add(CustomDec);
                }
            }
            return CustomDecoration;
        }
    }
}