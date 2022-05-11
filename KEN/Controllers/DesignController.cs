using AutoMapper;
using KEN.AppCode;
using KEN.Models;
using KEN_DataAccess;
using Newtonsoft.Json;
using PostmarkDotNet;
using PostmarkDotNet.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace KEN.Controllers
{
    public class DesignController : Controller
    {
        public static string PostmarkToken = ConfigurationManager.AppSettings["PostmarkToken"].ToString();

        KENNEWEntities dbcontext = new KENNEWEntities();

        // GET: Design
        public JsonResult GetUserLogos()
        {
            var userId = DataBaseCon.ActiveClientId();
            var imagesrc = dbcontext.tblUserLogoes.Where(x => x.UserId == userId).ToList();
            return Json(Mapper.Map<List<DesignViewModel>>(imagesrc), JsonRequestBehavior.AllowGet);

        }


        public ActionResult DesignerTool()
        {
            var items = dbcontext.tblitems.ToList();
            var colors = dbcontext.tblColors.ToList();
            var fabrics = dbcontext.tblFabrics.ToList();
            var process = dbcontext.tblProcesses.ToList();
            ViewBag.Items = items;
            ViewBag.Colors = colors;
            ViewBag.Fabrics = fabrics;
            ViewBag.Process = process;
            /*ViewBag.Image = imagesrc;*/
            return View();
        }
        public JsonResult FilterGarments(int? ItemId, int? ColorId, int? FabricId, int? Gender, int? Order)
        {
            var response = dbcontext.Pro_FilterGarments(ItemId, ColorId, FabricId, Gender, Order).ToList();

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetImage(int? ImageId)
        {
            var response = dbcontext.tblOptionProperties.Where(x => x.Id == ImageId).FirstOrDefault();

            return Json(Mapper.Map<OptionPropertiesViewModel>(response), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLogo(int? LogoId)
        {
            var logo = dbcontext.tblUserLogoes.Where(x => x.Id == LogoId).FirstOrDefault();
            return Json(Mapper.Map<DesignViewModel>(logo), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LogoUpload()
        {
            tblUserLogo userlogo = new tblUserLogo();
            var fileExtension = "";
            var fileName = "";

            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    fileName = Path.GetFileName(file.FileName);
                    fileExtension = Path.GetExtension(file.FileName);
                    var path = "/Content/uploads/logos/" + fileName;
                    string fullPath = Path.Combine(Server.MapPath(path));
                    userlogo.CreatedOn = DateTime.Now;
                    userlogo.IsDeleted = false;
                    userlogo.LogoUrl = path;
                    userlogo.UserId = DataBaseCon.ActiveClientId();

                    dbcontext.tblUserLogoes.Add(userlogo);
                    dbcontext.SaveChanges();
                    file.SaveAs(fullPath);
                }
            }
            return Json(new { fileName = fileName });

        }

        [HttpPost]
        public async Task SendEmail(string to, string imageData, string subject, string description)
        {
            string str = "Testing";

            var message = new PostmarkMessage()
            {
                To = to,
                From = DataBaseCon.FromEmailID,
                TrackOpens = true,
                Subject = subject,
                TextBody = description,
                //HtmlBody = str,
                Tag = "business-message",
                Headers = new HeaderCollection{
                    { "Name", "24 Hour Merchandise"},
                }
            };

            byte[] data = Convert.FromBase64String(imageData);

            message.AddAttachment(data, "Test", "image/jpeg");

            var client = new PostmarkClient(PostmarkToken);
            var sendResult = await client.SendMessageAsync(message);
        }



        [HttpPost]
        public ContentResult UploadImage(string imageData)
        {
            var fileName = Guid.NewGuid().ToString();
            try
            {
                string fileNameWithPath = Server.MapPath($"~/Content/uploads/Items/{fileName}.jpg");
                if (!Directory.Exists(fileNameWithPath))
                {
                    using (FileStream fs = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        using (BinaryWriter bw = new BinaryWriter(fs))
                        {
                            byte[] data = Convert.FromBase64String(imageData);
                            bw.Write(data);
                            bw.Close();
                        }
                    }
                }
            }
            catch (Exception ex) { }
            return Content(fileName);
        }

        public ActionResult SaveToRange(UserItemsViewModel items)
        {
            var ImagePath = "/Content/uploads/Items/" + items.FrontImageSource;
            tblUserItem userItem = new tblUserItem
            {
                UserId = DataBaseCon.ActiveClientId(),
                ImageId = items.ImageId,
                BackLogoId = items.BackLogoId,
                FrontLogoId = items.FrontLogoId,
                FrontLogoWidth = items.FrontLogoWidth,
                FrontLogoheight = items.FrontLogoheight,
                FrontLogoPositionTop = items.FrontLogoPositionTop,
                FrontLogoPositionLeft = items.FrontLogoPositionLeft,
                BackLogoheight = items.BackLogoheight,
                BackLogoWidth = items.BackLogoWidth,
                BackLogoPositionTop = items.BackLogoPositionTop,
                BackLogoPositionLeft = items.BackLogoPositionLeft,
                FrontImageSource = ImagePath
            };
            dbcontext.tblUserItems.Add(userItem);
            dbcontext.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }

        public ActionResult MyItems()
        {
            var userId = DataBaseCon.ActiveClientId();
            var myItems = dbcontext.tblUserItems.Where(x => x.UserId == userId).ToList();

            return Json(Mapper.Map<List<UserItemsViewModel>>(myItems), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSelectedItem(int id)
        {
            var item = dbcontext.tblUserItems.Include(x => x.tblOptionProperty).Where(x => x.Id == id).FirstOrDefault();
            return Json(Mapper.Map<UserItemsViewModel>(item), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddToCart(int itemId, float price, int process, int quantity)
        {
            
            
            
            var gst = 1;
            var total = quantity * price;
            var item = dbcontext.tblUserItems.FirstOrDefault(x => x.Id == itemId);
            tblDraftOrderItem draftOrders = new tblDraftOrderItem
            {
               
                Quantity = quantity,
                Process = process,
                Price = price,    
                TotalPrice = total,
                
            };
            dbcontext.tblDraftOrderItems.Add(draftOrders);
            dbcontext.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }

        public ActionResult Cart()
        {
            return View();
        }


        public ActionResult MyCart()
        {
            var cart = dbcontext.tblDraftOrderItems.ToList();
            var cartList = Mapper.Map<List<DraftOrdersViewModel>>(cart);
            var materProcessList = dbcontext.tblProcesses.ToList();


            foreach (var item in cartList)
            {
                var selectedProcess = materProcessList.FirstOrDefault(x => x.id == item.Process);
                item.ProcessValue = selectedProcess.name;
            }
            return Json(cartList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
       /* public ActionResult UpdateCart(List<CartDataListViewModel> cartDataList )
        {
            for (int i = 0; i < cartDataList.Count; i++)
            {
                var item = dbcontext.tblDraftOrderItems.Find(cartDataList[i].Id);
                item.Total = cartDataList[i].TotalPrice;
                item.Quantity = cartDataList[i].Quantity;
                dbcontext.Entry(item).State = EntityState.Modified;
                dbcontext.SaveChanges();


            }
            return Content("ok");
        }*/

        public ActionResult addAddress()
        {
            int userid = DataBaseCon.ActiveClientId();
            var userAddres = dbcontext.tblUserAddressMappings.Include(x => x.tbluser).Include(x => x.tblAddress).FirstOrDefault(x => x.UserId == userid);
            //var userAddressList = dbcontext.tblAddresses.FirstOrDefault(x => x.AddressId == userAddres.AddressId);
            userAddres.tblAddress.tblUserAddressMappings = null;
            userAddres.tbluser = null;
          
            return Json(userAddres, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CheckOut()
        {
            return View();
        }

        /*public ActionResult OrderDetail()
        {
            var cart = dbcontext.tblDraftOrderItems.ToList();
            for(int i=0; i< cart.Count; i++)
            {
                tblOrderDetail tblorder = new tblOrderDetail
                {
                    Quantity = cart[i].Quantity,
                    Process = cart[i].Process,
                    UserId = cart[i].UserId,
                    UserItemId = cart[i].UserItemId,
                    createdBY = DataBaseCon.ActiveClientId(),
                    createdOn = DateTime.UtcNow,
            };
                dbcontext.tblOrderDetails.Add(tblorder);
                dbcontext.SaveChanges();
            }
            return Json("thanku buy product");
        }*/

        
        public ActionResult OrderAddress()
        {

            return View();
        }


    }

}