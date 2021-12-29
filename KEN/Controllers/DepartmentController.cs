using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KEN.Models;
using KEN_DataAccess;
using KEN.Interfaces.Iservices;
using KEN.Interfaces.Repository;
using KEN.Interfaces;
using KEN.Filters;

namespace KEN.Controllers
{
    [UserAuthenticationFilter]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _baseService;

        public DepartmentController(IDepartmentService baseService)
        {
            _baseService = baseService;
        }
        // GET: Department
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDepartmentList(string q)
        {

            var newData = new List<DepartmentSelectViewModel>();
            // var newData = null;
            var data = _baseService.GetAllDepartmentList();
            var data2 = data.Select(u => new {
                id = u.id,
                name = u.department,
            }).OrderBy(_=>_.name).ToList();

            return Json(data2, JsonRequestBehavior.AllowGet);
        }
        // baans change 20th September

    }
}