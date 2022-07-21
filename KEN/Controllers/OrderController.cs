using KEN.Interfaces.Iservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using KEN.Filters;
using KEN.AppCode;
using KEN_DataAccess;
using KEN.Models;
using AutoMapper;
using Newtonsoft.Json;

namespace KEN.Controllers
{
    [UserAuthenticationFilter]
    public class OrderController : Controller
    {
        private readonly IOrderService _baseService;
        //// GET: Order
         KENNEWEntities dbcontext = new KENNEWEntities();
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
            /* var optionList = _baseService.GetAllOrderList(activeClientId);*/
            var checkOrder = dbcontext.tblDraftOrders.Where(x => x.UserId == activeClientId && x.IsDeleted == true).ToList();


            return View(Mapper.Map<List<ClientOptionViewModel>>(checkOrder));

        }

        public ActionResult ClientOrder()
        {
            var activeClientId = DataBaseCon.ActiveClientId();
            /* var optionList = _baseService.GetAllOrderList(activeClientId);*/
            var checkOrder = dbcontext.tblDraftOrders.Where(x => x.UserId == activeClientId && x.IsDeleted == true).ToList();
            /* foreach(var item in checkOrder)
             {
                 item.tblDraftOrderItems = null;
                 item.tbluser = null;
             }*/
            var u = JsonConvert.SerializeObject(Mapper.Map<List<ClientOptionViewModel>>(checkOrder));
            return Json(Mapper.Map<List<ClientOptionViewModel>>(checkOrder),JsonRequestBehavior.AllowGet);

        }
        public ActionResult ClientOrderItemList(int id)
        {
            try
            {               
                var order = dbcontext.tblDraftOrderItems.Where(x => x.OrderId == id && x.IsDeleted == false).ToList();
                var cartList = Mapper.Map<List<DraftOrdersItemViewModel>>(order);
                var materProcessList = dbcontext.tblApplicationProcesses.ToList();              
                foreach (var item in cartList)
                {
                    var userItem = dbcontext.tblDraftQuoteItems.Include(x => x.tblUserItem).Where(x => x.Id == item.QuotesItem_Id).FirstOrDefault();
                    item.FrontImage = userItem.tblUserItem?.FrontImageSource ?? string.Empty;
                    var selectedProcess = materProcessList.FirstOrDefault(x => x.Id == item.Process);
                    item.ProcessValue = selectedProcess.Name;
                    var selectedColor = dbcontext.tblPrintColors.FirstOrDefault(x => x.Id == item.Color);
                    item.ColorValue = selectedColor.Name;
                    var selectedSize = dbcontext.tblSizeMasters.FirstOrDefault(x => x.Id == item.Stitches);
                    item.StitchesValue = selectedSize.Size;
                }
                return Json(cartList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

            }
            return View();
        }

        public ActionResult ClientOrderFilter(string order)
        {
            var activeClientId = DataBaseCon.ActiveClientId();
            if (order == "oneweek")
            {
                var lastweek = DateTime.UtcNow.AddDays(-7);
                var checkOrder = dbcontext.tblDraftOrders.Where(x => x.UserId == activeClientId && x.IsDeleted == true && x.OrderDate>=lastweek).ToList();
                return Json(Mapper.Map<List<ClientOptionViewModel>>(checkOrder), JsonRequestBehavior.AllowGet);
            }
           else if (order == "month")
            {
                var lastweek = DateTime.UtcNow.AddDays(-30);
                var checkOrder = dbcontext.tblDraftOrders.Where(x => x.UserId == activeClientId && x.IsDeleted == true && x.OrderDate >= lastweek).ToList();
                return Json(Mapper.Map<List<ClientOptionViewModel>>(checkOrder), JsonRequestBehavior.AllowGet);
            }
            else if (order == "2021")
            {
                var lastyear = DateTime.UtcNow.AddYears(-1);
                var year = lastyear.Year;
                var checkOrder = dbcontext.tblDraftOrders.Where(x => x.UserId == activeClientId && x.IsDeleted == true && x.OrderDate.Value.Year == year).ToList();
                return Json(Mapper.Map<List<ClientOptionViewModel>>(checkOrder), JsonRequestBehavior.AllowGet);
            }
            return View();

        }

    }
}