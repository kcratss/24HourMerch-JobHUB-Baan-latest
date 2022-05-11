using KEN.Interfaces.Iservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KEN.Filters;
using KEN.AppCode;

namespace KEN.Controllers
{
    [UserAuthenticationFilter]
    public class OrderController : Controller
    {
        private readonly IOrderService _baseService;
        //// GET: Order
        public OrderController(IOrderService baseService)
        {
            _baseService = baseService;
        }
        public ActionResult OrderList()
        {
           
            return View();
        }
        //22 Nov 2018 (N)
        public ActionResult GetOrderList(string Type, String Department, string UserProfile, string StartDate, string EndDate)
        {
            var OrderDetails = _baseService.GetOrdersDetails(Type, Department, UserProfile, StartDate, EndDate);
            var Jsonresult = Json(OrderDetails, JsonRequestBehavior.AllowGet);
            Jsonresult.MaxJsonLength = int.MaxValue;
            return Jsonresult;
        }
        //22 Nov 2018 (N)

        [UserAuthorize("Online")]
        public ActionResult ClientOrderList()
        {
            var activeClientId = DataBaseCon.ActiveClientId();

            var optionList = _baseService.GetAllOrderList(activeClientId);

            return View(optionList);

        }
    }
}