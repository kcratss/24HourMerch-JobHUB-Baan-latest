using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KEN.Models;
using KEN_DataAccess;
using AutoMapper;
using KEN.AppCode;
using KEN.Interfaces.Iservices;
using KEN.Interfaces;
using System.IO;
using KEN.Filters;
using System.Collections;
using System.Net;
using System.Data.SqlClient;
using OfficeOpenXml;
using System.Data;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;

namespace KEN.Controllers
{
    [UserAuthenticationFilter]
    public class CommonMastersController : Controller
    {
        // baans change 11th October
        SqlCommand cmd = null;
        SqlConnection con = null;
        string FlagQuantity;
        // baans end 11th october
        private readonly ICommonMastersService _baseService;
        ResponseViewModel response = new ResponseViewModel();

        public CommonMastersController(ICommonMastersService baseService)
        {
            _baseService = baseService;
        }
        KENNEWEntities dbContext = new KENNEWEntities();
        // GET: CommonMasters
        public ActionResult CommonMastersDetails()
        {
            // baans change 15th November
            ViewBag.ProfileList = getProfileList();
            // baans end 15th November
            return View();
        }
        // baans change 11th October
        [HttpPost]
        public ActionResult CommonMastersDetails(HttpPostedFileBase postedFile)
        {
            // baans change 11th october
            var ActiveUser = DataBaseCon.ActiveUser();
            var User = dbContext.tblusers.Where(_ => _.email == ActiveUser).FirstOrDefault();
            var CurrentUser = User.id;
            // baans end 11th OCTOBER
            var cnnstring=ConfigurationManager.ConnectionStrings["KENNEWEntities"].ConnectionString;
            EntityConnectionStringBuilder builder = new EntityConnectionStringBuilder(cnnstring);
            builder.Metadata = null;
            cnnstring = builder.ProviderConnectionString;
            //con = new SqlConnection(ConfigurationManager.ConnectionStrings["bwavenueEntities"].ConnectionString);
            if (postedFile != null)
            {
                string FileExtension = postedFile.FileName;
                if (FileExtension != "")
                {
                    var ext = FileExtension.Split('.');
                    var extension = ext[1];

                    if (extension.ToUpper() == "xlsx".ToUpper())
                    {
                        string filename = postedFile.FileName;
                        string Filename = filename;

                        string filePath = Server.MapPath(Filename);
                        filePath = Server.MapPath("~/Content/uploads/Opportunity/") + filename;
                        postedFile.SaveAs(filePath);
                        // baans change 11th Oct for Temp Table

                        //DataTable tableTemp = new DataTable();
                        //DataColumn column = new DataColumn("Department", typeof(string));
                        //DataColumn column = new DataColumn("Status", typeof(string));
                        //DataColumn column = new DataColumn("Department", typeof(string));
                        //DataColumn column = new DataColumn("Department", typeof(string));
                        //DataColumn column = new DataColumn("Department", typeof(string));
                        //DataColumn column = new DataColumn("Department", typeof(string));
                        //DataColumn column = new DataColumn("Department", typeof(string));
                        //tableTemp.Columns.Add(column);
                        // baans end 11th October
                        FileInfo workBook = new FileInfo(filePath);
                        using (ExcelPackage xlPackage = new ExcelPackage(workBook))
                        {
                            //Fetch the worksheet to insert the data
                            ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1]; //ContactImportTemplate
                            int totalRows = worksheet.Dimension.End.Row;

                            int totalCols = worksheet.Dimension.End.Column;
                            DataTable dtExcelData = new DataTable(worksheet.Name);
                            for (int n = 1; n <= totalCols; n++)
                            {
                                string ColName = worksheet.Cells[1, n].Value.ToString().Trim();
                                dtExcelData.Columns.Add(ColName);
                            }
                            dtExcelData.Columns.Add("UserId", typeof(System.Int32));
                            DataRow dr = null;
                            for (int i = 2; i <= totalRows; i++)
                            {
                                dr = dtExcelData.NewRow();
                                for (int j = 1; j <= totalCols; j++)
                                {
                                    if (worksheet.Cells[i, j].Value != null)
                                    {
                                        dr[j - 1] = worksheet.Cells[i, j].Value.ToString().Trim();
                                    }
                                    else
                                    {
                                        dr[j - 1] = null;
                                    }
                                }
                                dtExcelData.Rows.Add(dr);

                                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                                {
                                    foreach (DataRow row in dtExcelData.Rows)
                                    {
                                        
                                        row["UserId"] = CurrentUser;
                                    }
                                    sqlBulkCopy.DestinationTableName = "tblTempDepartmentExport";
                                    
                                    sqlBulkCopy.ColumnMappings.Add("Department", "Department");
                                    sqlBulkCopy.ColumnMappings.Add("Status", "Status");
                                    sqlBulkCopy.ColumnMappings.Add("CreatedBy", "CreatedBy");
                                    sqlBulkCopy.ColumnMappings.Add("CreatedOn", "CreatedOn");
                                    sqlBulkCopy.ColumnMappings.Add("UpdatedBy", "UpdatedBy");
                                    sqlBulkCopy.ColumnMappings.Add("UpdatedOn", "UpdatedOn");
                                    sqlBulkCopy.ColumnMappings.Add(CurrentUser, "UserId");
                                    con.Open();
                                    sqlBulkCopy.WriteToServer(dtExcelData);
                                    con.Close();
                                    dtExcelData.Clear();
                                }
                            }


                        }
                    }
                }
            }
                return View();
            
        }
        // baans end 11th October

        public ActionResult ViewMaster(string ddlvalue, string ddlstatusvalue)
        {
            //var Mastergrid = new List<MasterViewModels>();
            if (ddlvalue == "tbldepartment")

            {
                //var MasterModelList = entity.tbldepartments.ToList().OrderBy(_ => _.department);
                //Mastergrid = Mapper.Map<List<MasterViewModels>>(MasterModelList).ToList();

                var data = _baseService.DepartmentListForMasters(ddlstatusvalue);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else if (ddlvalue == "tblband")
            {
                //var MasterModelList = entity.tblbands.ToList().OrderBy(_ => _.name);
                //Mastergrid = Mapper.Map<List<MasterViewModels>>(MasterModelList).ToList();

                var data = _baseService.BrandListForMasters(ddlstatusvalue);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else if(ddlvalue == "tblitem")
            {
                var data = _baseService.ItemsListForMasters(ddlstatusvalue);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return View();
            //return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateData(int id,string name,string status,string table)
        {
            if(id > 0)
            {
                response = _baseService.MasterBatchTransaction(id,name,status,table,BatchOperation.Update);
            }
            else
            {
                response = _baseService.MasterBatchTransaction(id, name, status, table, BatchOperation.Insert);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        // baans change 15th November
        public List<AccountManagerDropdownViewModel> getProfileList()
        {
            var getData = Mapper.Map<List<AccountManagerDropdownViewModel>>(dbContext.tblusers
                .Where(_ => _.UserRole == "Account Manager" && _.status == "Active").ToList().OrderBy(_ => _.firstname)).OrderBy(_ => _.AccountManagerFullName).ToList();
            return getData;
        }
        // baans end 15th November
        public ActionResult OptionCodeMaster()
        {
            ViewBag.ProfileList = getProfileList();
            ViewBag.ItemList = getItemList();
            ViewBag.BrandList = getBrandList();
            return View();
        }
        public ActionResult GetOptionCodeList()
        {
            var data = _baseService.OptionCodeListMasters().OrderBy(_ => _.Code).OrderBy(_ => _.Status).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveOptionCodeData(tblOptionCode model)
        {
            if (model.id > 0)
            {
                response = _baseService.OptionCodeTransaction(model, BatchOperation.Update);
            }
            else
            {
                response = _baseService.OptionCodeTransaction(model, BatchOperation.Insert);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public List<tblitem> getItemList()
        {
            var getData = Mapper.Map<List<tblitem>>(dbContext.tblitems
                .Where(_ => _.Status == "Active").ToList());
            return getData;
        }
        public List<tblband> getBrandList()
        {
            var getData = Mapper.Map<List<tblband>>(dbContext.tblbands
                .Where(_ => _.Status == "Active").ToList());
            return getData;
        }
    }
}