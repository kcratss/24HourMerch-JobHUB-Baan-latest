using KEN.AppCode;
using KEN.Interfaces.Iservices;
using KEN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KEN.Controllers
{
    public class AddressController : Controller
    {
        private readonly IAddressService _addressService;
        ResponseMessageViewModel response = new ResponseMessageViewModel();
        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        public ActionResult AddressList()
        {
            ViewBag.state = _addressService.GetAllState();
            var userId = DataBaseCon.ActiveClientId();
            var addressList = _addressService.GetAddressList(userId);
            if (addressList != null)
            {
                return View(addressList);
            }
            return View();

        }

        [HttpPost]
        public ActionResult AddUpdateAddress(ClientAddressViewModel model)
        {
            try
            {
                bool result;
                if (model.AddressId > 0)
                {
                    result = _addressService.UpdateAddress(model);
                    if (result)
                    {
                        response.IsSuccess = true;
                        response.Message = "Address Updated";
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    result = _addressService.AddAddress(model);
                    if (result)
                    {
                        response.IsSuccess = true;
                        response.Message = "Address Added";
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                }

                response.IsSuccess = false;
                response.Message = "Address not Added";
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.ToString();
                return Json(response, JsonRequestBehavior.AllowGet);
            }


        }


        [HttpPost]
        public ActionResult DeleteAddress(int addressId)
        {
            var result = _addressService.DeleteAddress(addressId);
            if (result)
            {
                response.IsSuccess = true;
                response.Message = "Address Deleted";
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            response.IsSuccess = false;
            response.Message = "Address not Deleted";
            return Json(response, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetAddressById(int addressId)
        {
            ViewBag.state = _addressService.GetAllState();
            var addressmodel = _addressService.GetAddressById(addressId);
            return PartialView("_UpdateAddressModel", addressmodel);


        }
    }
}