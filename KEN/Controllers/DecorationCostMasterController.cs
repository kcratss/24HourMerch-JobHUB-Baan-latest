using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KEN.Models;
using KEN_DataAccess;
using AutoMapper;
using KEN.Interfaces.Iservices;
using KEN.Interfaces;
using KEN.Filters;
using ClosedXML.Excel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace KEN.Controllers
{
    [UserAuthenticationFilter]
    public class DecorationCostMasterController : Controller
    {
        ResponseViewModel response = new ResponseViewModel();
        private readonly IDecorationCostService _decorationCost;
        KENNEWEntities dbContext = new KENNEWEntities();

        public object data { get; private set; }

        // GET: DecorationCost
        public DecorationCostMasterController(IDecorationCostService decorationCost)
        {     
            _decorationCost = decorationCost;
          
        }
        public ActionResult DecorationCostMaster()
        {
            // baans change 15th November
            ViewBag.ProfileList = getProfileList();
            // baans end 15th November
            return View();
        }

        // 03 Oct 2018 (N)
        //public ActionResult GetDecorationCostList(string Status)
        //{
        //    var data = _decorationCost.GetDecorationCostList();
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        // 03 Oct 2018 (N)
        public ActionResult SaveDecoration(DecorationCostMasterViewModel model)
        {
            if (model != null)
            {
                var Entity = Mapper.Map<tblDecorationCost>(model);
                if (model.DecCostId > 0)
                {
                    response = _decorationCost.DecorationBatchTransaction(Entity, BatchOperation.Update);
                }
                else
                {
                    response = _decorationCost.DecorationBatchTransaction(Entity, BatchOperation.Insert);
                }
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteDecoration(DecorationCostMasterViewModel model)
        {
            if (model != null)
            {
                var Entity = Mapper.Map<tblDecorationCost>(model);
                if (Entity.DecCostId != 0)
                {
                    response = _decorationCost.DecorationBatchTransaction(Entity, BatchOperation.Delete);
                }
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        
        }
        
        // 03 Oct 2018 (N)
        public ActionResult GetDecorationList(string Status, string Type)   
        {
            var data = _decorationCost.GetDecorationList(Status,Type);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        // 03 Oct 2018 (N)

        //Common Data View
        public ActionResult CommonDataMaster()
        {
            return View();
        }
        public ActionResult GetCommonDataList()
        {
            var data = _decorationCost.GetCommonDataList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //End
        public ActionResult ExportToExcel()
        {
            var gv = new GridView();
            //gv.DataSource = this._decorationCost.GetDecorationList("All","AllType");
            //var exportdata = dbContext.Pro_DecorationExport().ToList();
            //var data = exportdata.ToList();
            //gv.DataSource = this._decorationCost.ExportData();
            gv.DataSource = this.dbContext.Pro_DecorationExport().ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=DecorationCost.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();

            return View();
        }
        // baans change 15th November
        public List<AccountManagerDropdownViewModel> getProfileList()
        {
            var getData = Mapper.Map<List<AccountManagerDropdownViewModel>>(dbContext.tblusers
                .Where(_ => _.UserRole == "Account Manager" && _.status == "Active").ToList().OrderBy(_ => _.firstname)).OrderBy(_ => _.AccountManagerFullName).ToList();
            return getData;
        }
        // baans end 15th November
    }

}