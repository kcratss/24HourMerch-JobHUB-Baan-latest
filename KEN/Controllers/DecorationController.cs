using AutoMapper;
using KEN.Models;
using KEN_DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KEN.Filters;

namespace KEN.Controllers
{
    [UserAuthenticationFilter]
    public class DecorationController : Controller
    {
        KENNEWEntities dbContext = new KENNEWEntities();
        // GET: Decoration
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DecorationDetails()
        {
            ViewBag.SizeType = GetAllState();
            ViewBag.DepartmentList = GetDepartments();
            ViewBag.ManagerList = GetAccountManagers();
            ViewBag.CampaignList = GetCampaign();
            ViewBag.ContactTypes = GetContactType();
           // ViewBag.ContactRoles = GetContactRoles();
            //ViewBag.StageList = GetStage();
            ViewBag.SourceList = GetSource();
            ViewBag.Cycles = GetCycles();
            //ViewBag.SizeType = GetAllState();
            //ViewBag.DecorationList = GetDecorationList();
            ViewBag.BrandList = GetBrandList();
            ViewBag.ItemList = GetItemList();
            ViewBag.ShippingList = GetShipping();
            ViewBag.StateList = GetAllStateList();
            return View();
        }
        public List<sizeType> GetAllState()
        {
            List<sizeType> objstate = new List<sizeType>();
            objstate.Add(new sizeType { value = "", typeName = "--Select--" });
            objstate.Add(new sizeType { value = "Mens", typeName = "Mens" });
            objstate.Add(new sizeType { value = "Shirts", typeName = "Shirts" });
            objstate.Add(new sizeType { value = "Shorts/Pants", typeName = "Shorts/Pants" });
            objstate.Add(new sizeType { value = "Toddlers/Youth", typeName = "Toddlers/Youth" });
            objstate.Add(new sizeType { value = "Womens", typeName = "Womens" });
            objstate.Add(new sizeType { value = "Youths", typeName = "Youths" });
            objstate.Add(new sizeType { value = "Custom", typeName = "Custom" });
            return objstate.OrderBy(_ => _.value).ToList();

        }
        public List<StateList> GetAllStateList()
        {
            List<StateList> objState = new List<StateList>();
            //objState.Add(new StateList { stateName = "New South Wales" });
            //objState.Add(new StateList { stateName = "Queensland south" });
            //objState.Add(new StateList { stateName = "South Australia" });
            //objState.Add(new StateList { stateName = "Tasmania" });
            //objState.Add(new StateList { stateName = "Victoria" });
            //objState.Add(new StateList { stateName = "Western Australia" });
            //objState.Add(new StateList { stateName = "South west" });
            //objState.Add(new StateList { stateName = "South west india" });
            objState.Add(new StateList { stateName = "ACT" }); //Commente and added by baans 19Sep2020
            objState.Add(new StateList { stateName = "NSW" });
            objState.Add(new StateList { stateName = "NT" });
            objState.Add(new StateList { stateName = "QLD" });
            objState.Add(new StateList { stateName = "SA" });
            objState.Add(new StateList { stateName = "TAS" });
            objState.Add(new StateList { stateName = "VIC" });
            objState.Add(new StateList { stateName = "WA" });

            return objState;
        }

        public List<AccountManagerDropdownViewModel> GetAccountManagers()
        {
            var getData = Mapper.Map<List<AccountManagerDropdownViewModel>>(dbContext.tblusers.Where(_ => _.title == "Account Manager").ToList().OrderBy(_ => _.title)).OrderBy(_ => _.AccountManagerFullName).ToList();
            return getData;
        }
        public IEnumerable<tbldepartment> GetDepartments()
        {
            var getData = dbContext.tbldepartments.ToList().OrderBy(_ => _.department);
            return getData;
        }
        public IEnumerable<tblitem> GetItemList()
        {
            var getData = dbContext.tblitems.ToList().OrderBy(_ => _.name);
            return getData;
        }
        public IEnumerable<tblband> GetBrandList()
        {
            var getData = dbContext.tblbands.ToList().OrderBy(_ => _.name);
            return getData;
        }
        public IEnumerable<tblCampaign> GetCampaign()
        {
            var getData = dbContext.tblCampaigns.ToList().OrderBy(_ => _.Campaign);
            return getData;
        }

        //public List<EnumViewModel> GetContactRoles()
        //{
        //    var getData = (from ContactRole e in Enum.GetValues(typeof(ContactRole))
        //                   select new { Name = e.ToString() }).ToList();
        //    var newdata = getData.Select(item => new EnumViewModel
        //    {
        //        Name = item.Name,
        //    }
        //    ).OrderBy(_ => _.Name).ToList();
        //    return newdata;
        //}
        public List<EnumViewModel> GetContactType()
        {
            var getData = (from ContactType e in Enum.GetValues(typeof(ContactType))
                           select new { Name = e.ToString() }).ToList();
            var newdata = getData.Select(item => new EnumViewModel
            {
                Name = item.Name,
            }
            ).OrderBy(_ => _.Name).ToList();
            return newdata;
        }
        public List<EnumViewModel> GetSource()
        {

            var getData = (from SourceEnum e in Enum.GetValues(typeof(SourceEnum))
                           select new { Name = e.ToString() }).ToList();
            var newdata = getData.Select(item => new EnumViewModel
            {
                Name = item.Name,
            }
            ).OrderBy(_ => _.Name).ToList();
            return newdata;
        }
        public List<EnumViewModel> GetCycles()
        {
            var getData = (from Cycles e in Enum.GetValues(typeof(Cycles))
                           select new { Name = e.ToString() }).ToList();
            var newdata = getData.Select(item => new EnumViewModel
            {
                Name = item.Name,
            }
            ).OrderBy(_ => _.Name).ToList();
            return newdata;
        }
        public List<EnumViewModel> GetShipping()
        {
            var getData = (from Shipping e in Enum.GetValues(typeof(Cycles))
                           select new { Name = e.ToString() }).ToList();
            var newdata = getData.Select(item => new EnumViewModel
            {
                Name = item.Name,
            }
            ).OrderBy(_ => _.Name).ToList();
            return newdata;
        }

    }
}