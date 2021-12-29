using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KEN.Models;
using KEN_DataAccess;
using AutoMapper;
using KEN.Filters;
using KEN.Interfaces.Iservices;
using System.IO;

namespace KEN.Controllers
{
    [UserAuthenticationFilter]
    public class ApplicationController : Controller
    {
        private readonly IApplicationService _baseService;

        ResponseViewModel response = new ResponseViewModel();
        KENNEWEntities dbContext = new KENNEWEntities();

        public ApplicationController(IApplicationService baseService)
        {
            _baseService = baseService;
        }
        public ActionResult ApplicationList()
        {
            ViewBag.ProfileList = getProfileList();
            return View();
        }
        public ActionResult ApplicationDetails(int Id, string PageType, string OptionId)
        {
            if(Id != 0) {
                var AppType = dbContext.tblApplicationCustomInfoMappings.Where(_ => _.ApplicationId == Id).FirstOrDefault();
                if(AppType == null)
                {
                    PageType = "Default";
                }
                else
                {
                    PageType = "Custom";
                }
            }
            ViewBag.ApplicationId = Id;
            ViewBag.PageType = PageType;
            ViewBag.OptionId = OptionId;
            ViewBag.ProfileList = getProfileList();
            ViewBag.ApplicationStatus = GetApplicationStatus();
            ViewBag.ArtSupplier = GetApplicationArtSupplier();
            ViewBag.ApplicationType = GetApplicationType();
            ViewBag.ApplicationArt = GetApplicationArt();
            ViewBag.ApplicationDesigner = GetApplicationDesigner();
            ViewBag.ApplicationProduction = GetApplicationProduction();
            return View();
        }
        public List<AccountManagerDropdownViewModel> getProfileList()
        {
            var getData = Mapper.Map<List<AccountManagerDropdownViewModel>>(dbContext.tblusers
                .Where(_ => _.UserRole == "Account Manager" && _.status == "Active").ToList().OrderBy(_ => _.firstname)).OrderBy(_ => _.AccountManagerFullName).ToList();
            return getData;
        }
        public IEnumerable<tblApplicationStatu> GetApplicationStatus()
        {
            var ApplicationStatus = _baseService.GetApplicationStatus();
            return ApplicationStatus;
        }
        public IEnumerable<tblApplicationArtSuppplier> GetApplicationArtSupplier()
        {
            var ArtSupplier = _baseService.GetApplicationArtSupplier();
            return ArtSupplier;
        }
        public IEnumerable<tblApplicationType> GetApplicationType()
        {
            var ApplicationType = _baseService.GetApplicationType();
            return ApplicationType;
        }
        public IEnumerable<tblApplicationArt> GetApplicationArt()
        {
            var Art = _baseService.GetApplicationArt();
            return Art;
        }
        public IEnumerable<tblApplicationDesigner> GetApplicationDesigner()
        {
            var Designer = _baseService.GetApplicationDesigner();
            return Designer;
        }
        public List<EnumViewModel> GetApplicationProduction()
        {
            var getData = (from ApplicaionProduction e in Enum.GetValues(typeof(ApplicaionProduction))
                           select new { Name = e.ToString() }).ToList();
            var newdata = getData.Select(item => new EnumViewModel
            {
                Name = item.Name
            }).OrderBy(_ => _.Name).ToList();

            return newdata;
        }
        public ActionResult GetSupplierId()
        {
            var data = dbContext.tblApplicationArtSupppliers.Where(_ => _.SupplierName == "Add New").FirstOrDefault();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveArtSupplier(string ArtSupplierName)
        {
            response = _baseService.SaveArtSupplier(ArtSupplierName);

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveApplication(ApplicationViewModel Model)
        {
            var Entity = Mapper.Map<TblApplication>(Model);
            if (Model != null)
            {
                if(Model.ApplicationId > 0)
                {
                    response = _baseService.ApplicationBatchTransaction(Entity, Interfaces.BatchOperation.Update);
                }
                else
                {
                    response = _baseService.ApplicationBatchTransaction(Entity, Interfaces.BatchOperation.Insert);
                }
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetApplicationById(int ApplicationId)
        {
            ApplicationViewModel data = new ApplicationViewModel();
            if(ApplicationId != 0)
            {
                data = _baseService.GetApplicationById(ApplicationId);
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveApplicationColours(ApplicationColourViewModel Model, int ApplicationId, PantoneMasterViewModel PantoneModel)
        {
            var exceptionMessage = "";
            try
            {
                if(PantoneModel.Hexvalue != "NA" || PantoneModel.Hexvalue != "TBC")
                {
                    var Code = System.Drawing.ColorTranslator.FromHtml("#" + Convert.ToString(PantoneModel.Hexvalue));
                }
            }
            catch (Exception Ex)
            {
                exceptionMessage = Ex.InnerException.Message.ToString();
            }

            if(exceptionMessage == "")
            {
                var PantoneEntity = Mapper.Map<tblPantoneMaster>(PantoneModel);
                var PantoneId = _baseService.SavePantone(PantoneEntity);

                var Entity = Mapper.Map<TblApplicationColour>(Model);
                Entity.Pantone = PantoneId;
                if (Model != null)
                {
                    if (Model.ApplicationColourId > 0)
                    {
                        response = _baseService.ApplicationColoursBatchTransaction(Entity, ApplicationId, Interfaces.BatchOperation.Update);
                    }
                    else
                    {
                        response = _baseService.ApplicationColoursBatchTransaction(Entity, ApplicationId, Interfaces.BatchOperation.Insert);
                    }
                }
            }
            var result = new { exceptionMessage = exceptionMessage, response = response };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetApplicationColoursGrid(int ApplicationId)
        {
            var data = _baseService.GetApplicationColoursGrid(ApplicationId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetApplicationGridData(string ApplicationType)
        {
            var data = _baseService.GetApplicationGridData(ApplicationType);
            var Jsonresult = Json(data, JsonRequestBehavior.AllowGet);
            Jsonresult.MaxJsonLength = int.MaxValue;
            return Jsonresult;
        }
        public ActionResult GetApplicationCustomData(string CustomText)
        {
            var TableName = "TblApplication";
            var CustomData = _baseService.GetApplicationCustomData(CustomText, TableName);
            var Jsonresult = Json(CustomData, JsonRequestBehavior.AllowGet);
            Jsonresult.MaxJsonLength = int.MaxValue;
            return Jsonresult;
        }
        public ActionResult DeleteApplicationColours(ApplicationColourViewModel Model, int ApplicationId)
        {
            var Entity = Mapper.Map<TblApplicationColour>(Model);
            response = _baseService.ApplicationColoursBatchTransaction(Entity, ApplicationId, Interfaces.BatchOperation.Delete);

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ApplicationJobsList(int ApplicationId)
        {
            var data = _baseService.ApplicationJobsList(ApplicationId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPantone(string prefix)
        {
            var data = _baseService.GetPantone(prefix).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ApplicationFiles(int ApplicationId, string OptionId, HttpPostedFileBase srcimgFile, HttpPostedFileBase imgFile, HttpPostedFileBase vectorFile, HttpPostedFileBase mockimgFile)
        {
            var data = dbContext.TblApplications.Where(_ => _.ApplicationId == ApplicationId).FirstOrDefault();
            if(data != null)
            {
                if (srcimgFile != null)
                {
                    if (srcimgFile.ContentLength > 0)
                    {
                        if(data.SourceImage != null && data.SourceImage != "")
                        {
                            var oldfile = Path.Combine(Server.MapPath("~/Content/uploads/Application/") + data.SourceImage);

                            if (System.IO.File.Exists(oldfile))
                            {
                                System.IO.File.Delete(oldfile);
                            }
                        }

                        var filename = Path.GetFileName(srcimgFile.FileName);
                        var File = filename.Split('.');
                        var newFileName = File[0] + "_" + ApplicationId + "." + File[1];
                        var ServerSavePath = Path.Combine(Server.MapPath("~/Content/uploads/Application/") + newFileName);
                        srcimgFile.SaveAs(ServerSavePath);

                        data.SourceImage = newFileName;
                    }
                }

                if (imgFile != null)
                {
                    if (imgFile.ContentLength > 0)
                    {
                        if (data.AppImage != null && data.AppImage != "")
                        {
                            var oldfile = Path.Combine(Server.MapPath("~/Content/uploads/Application/") + data.AppImage);

                            if (System.IO.File.Exists(oldfile))
                            {
                                System.IO.File.Delete(oldfile);
                            }
                        }

                        var filename = Path.GetFileName(imgFile.FileName);
                        var File = filename.Split('.');
                        var newFileName = File[0] + "_" + ApplicationId + "." + File[1];
                        var ServerSavePath = Path.Combine(Server.MapPath("~/Content/uploads/Application/") + newFileName);
                        imgFile.SaveAs(ServerSavePath);

                        data.AppImage = newFileName;
                    }
                }

                if(mockimgFile != null)
                {
                    if(mockimgFile.ContentLength > 0)
                    {
                        if (data.MockUpImage != null && data.MockUpImage != "")
                        {
                            var oldfile = Path.Combine(Server.MapPath("~/Content/uploads/Application/") + data.MockUpImage);

                            if (System.IO.File.Exists(oldfile))
                            {
                                System.IO.File.Delete(oldfile);
                            }
                        }

                        var filename = Path.GetFileName(mockimgFile.FileName);
                        var File = filename.Split('.');
                        var newFileName = File[0] + "_" + ApplicationId + "." + File[1];
                        var ServerSavePath = Path.Combine(Server.MapPath("~/Content/uploads/Application/") + newFileName);
                        mockimgFile.SaveAs(ServerSavePath);

                        data.MockUpImage = newFileName;
                    }
                }

                if(vectorFile != null)
                {
                    if (vectorFile.ContentLength > 0)
                    {
                        if (data.AppVector != null && data.AppVector != "")
                        {
                            var oldfile = Path.Combine(Server.MapPath("~/Content/uploads/Application/") + data.AppVector);

                            if (System.IO.File.Exists(oldfile))
                            {
                                System.IO.File.Delete(oldfile);
                            }
                        }

                        var filename = Path.GetFileName(vectorFile.FileName);
                        var File = filename.Split('.');
                        var newFileName = File[0] + "_" + ApplicationId + "." + File[1];
                        var ServerSavePath = Path.Combine(Server.MapPath("~/Content/uploads/Application/") + newFileName);
                        vectorFile.SaveAs(ServerSavePath);

                        data.AppVector = newFileName;
                    }
                }
            }
            dbContext.SaveChanges();

            return RedirectToAction("ApplicationDetails", new { Id = ApplicationId, PageType = "Default", OptionId = OptionId });
        }
        public ActionResult SaveCustomInfo(ApplicationCustomInfoViewModel Model, int ApplicationId)
        {
            var Entity = Mapper.Map<TblApplicationCustomInfo>(Model);
            if (Entity != null)
            {
                if (Entity.CustomInfoId > 0)
                {
                    response = _baseService.SaveCustomInfo(Entity, ApplicationId, Interfaces.BatchOperation.Update);
                }
                else
                {
                    response = _baseService.SaveCustomInfo(Entity, ApplicationId, Interfaces.BatchOperation.Insert);
                }
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetApplicationCustomGrid(int ApplicationId)
        {
            var data = _baseService.GetApplicationCustomGrid(ApplicationId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteApplicationCustomInfo(ApplicationCustomInfoViewModel Model, int ApplicationId)
        {
            var Entity = Mapper.Map<TblApplicationCustomInfo>(Model);
            response = _baseService.SaveCustomInfo(Entity, ApplicationId, Interfaces.BatchOperation.Delete);

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetOptionInfo(string OptionId)
        {
            var Id = Convert.ToInt32(OptionId);
            var data = Mapper.Map<OptionViewModel>(dbContext.tbloptions.Where(_ => _.id == Id).FirstOrDefault());

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public FileResult DownloadApplicationFiles(string fileName)
        {
            var ServerSavePath = Path.Combine(Server.MapPath("~/Content/uploads/Application/") + fileName);
            string ReportURL = ServerSavePath;
            byte[] FileBytes = System.IO.File.ReadAllBytes(ReportURL);
            return File(FileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}