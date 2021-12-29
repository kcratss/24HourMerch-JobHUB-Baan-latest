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

namespace KEN.Controllers
{
    [UserAuthenticationFilter]
    public class KanBanController : Controller
    {
        private readonly IKanBanService _baseService;
        ResponseViewModel response = new ResponseViewModel();

        public KanBanController(IKanBanService baseService)
        {
            _baseService = baseService;
        }

        KENNEWEntities dbContext = new KENNEWEntities();
        // GET: KanBan
        public ActionResult KanBan()
        {
            // baans change 15th November
            ViewBag.ProfileList = getProfileList();
            // baans end 15th November
            ViewBag.GetDepartmentList = GetDepartmentList();
            return View();
        }
        
        public ActionResult updateKanBan(KanBanViewModel model)
        {
            var entity = Mapper.Map<tblkanban>(model);
            
            response = _baseService.KanbanCheck(entity);

            return Json(response, JsonRequestBehavior.AllowGet); 
        }
        
        public List<DepartmentSelectViewModel> GetDepartmentList()
        {
            var data = _baseService.GetAllDepartmentList().ToList();
            var newdata = data.Select(item => new DepartmentSelectViewModel
            {
               id = item.id,
               text = item.department
            }).ToList();

            return newdata;
        }

        public ActionResult GetSidebarKanbanOpportunity(string DeptId)
        {
            var unassignKanban = _baseService.GetSidebarunassignKanban(DeptId);
            var UncompleteKanBan = _baseService.GetSideBarUncompleteKanBan(DeptId);

            var result = Json(new { param1 = unassignKanban, param2 = UncompleteKanBan });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
             

        public ActionResult GetAllKanbanJobs(string DeptId)
        {
            var data = _baseService.GetAllKanbanJobs(DeptId);
            var GetAllHolidays = Mapper.Map<List<VW_GetHolidaysViewModel>>(dbContext.VW_GetHolidays).ToList();

            var result = new { res1 = data, res2=GetAllHolidays};
            return Json(result, JsonRequestBehavior.AllowGet);
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