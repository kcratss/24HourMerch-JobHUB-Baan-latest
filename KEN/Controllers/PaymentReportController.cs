using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KEN.Models;
using KEN_DataAccess;
using AutoMapper;
using System.IO;
using KEN.Filters;
namespace KEN.Controllers
{
    [UserAuthenticationFilter]
    public class PaymentReportController : Controller
    {
        KENNEWEntities dbContext = new KENNEWEntities();
        // GET: PaymentReport
        public ActionResult PaymentReport()
        {
            ViewBag.ProfileList = getProfileList();
            return View();
        }
        // baans change 13th December for Sales Report
        public ActionResult SalesReport()
        {
            ViewBag.ProfileList = getProfileList();
            return View();
        }
        // baans end 13th December

        public ActionResult ManagerStageWiseReport()
        {
            ViewBag.ProfileList = getProfileList();
            return View();
        }
        public ActionResult ValueConversionReport()
        {
            ViewBag.ProfileList = getProfileList();
            return View();
        }
        public ActionResult OpportunityValueConversionReport()
        {
            ViewBag.ProfileList = getProfileList();
            return View();
        }
        public List<AccountManagerDropdownViewModel> getProfileList()
        {
            var getData = Mapper.Map<List<AccountManagerDropdownViewModel>>(dbContext.tblusers
                .Where(_ => _.UserRole == "Account Manager" && _.status == "Active").ToList().OrderBy(_ => _.firstname)).OrderBy(_ => _.AccountManagerFullName).ToList();
            return getData;
        }
        //Added by baans 23Sep2020
        public ActionResult InvoicedReport()
        {
            ViewBag.ProfileList = getProfileList();
            return View();
        }
    }
}