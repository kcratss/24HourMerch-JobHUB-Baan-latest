using AutoMapper;
using KEN.AppCode;
using KEN.Interfaces.Iservices;
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
        ResponseMessageViewModel response = new ResponseMessageViewModel();
      
        KENNEWEntities dbcontext = new KENNEWEntities();

        // GET: Design
        public JsonResult GetUserLogos()
        {
            var userId = DataBaseCon.ActiveClientId();
            var imagesrc = dbcontext.tblUserLogoes.Include(x=>x.tblStatu).Where(x => x.UserId == userId).ToList().OrderByDescending(x=>x.Status).ThenByDescending(x => x.Id);
            var userlogo = Mapper.Map<List<DesignViewModel>>(imagesrc);          
            return Json(userlogo, JsonRequestBehavior.AllowGet);

        }


        public ActionResult DesignerTool()
        {
            
            var items = dbcontext.tblitems.ToList();
            var colors = dbcontext.tblColors.ToList();
            var fabrics = dbcontext.tblFabrics.ToList();
            var process = dbcontext.tblApplicationProcesses.ToList();
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
            var UserId = DataBaseCon.ActiveClientId();
            var createdby = dbcontext.tblusers.Where(x=>x.id == UserId).FirstOrDefault();
            var userName = createdby.firstname +"  "+ createdby.lastname;
            tblUserLogo userlogo = new tblUserLogo();
            var fileExtension = "";
            var fileName = "";
            var Process_Id = 0;
            var Color_Id = 0;
            var Size_Id = 0;
            var Width = "";
            var Height = "";
            var Name = "";
            var Complexity = 0;

            if (Request.Files.Count > 0)
            {

                HttpPostedFileBase file = Request.Files[0];

            for(int i =0;i<Request.Form.Count; i++)
                {
                    if (i == 0)
                    {
                        Process_Id = Int32.Parse(Request.Form[i]);
                    }
                    else if(i == 1)
                    {
                        Color_Id = Int32.Parse(Request.Form[i]);
                    }
                    else if(i == 2)
                    {
                        Size_Id = Int32.Parse(Request.Form[i]);
                    }
                    else if (i == 3)
                    {
                        Name =Request.Form[i];
                    }                   
                    else if (i == 4)
                    {
                        Height = Request.Form[i];                        
                    }
                    else if (i == 5)
                    {
                        Width = Request.Form[i];
                    }
                    else if (i == 6)
                    {
                       Complexity = Int32.Parse(Request.Form[i]);
                    }
                }
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
                    userlogo.Status = 2;
                    userlogo.Height = Height;
                    userlogo.Width = Width;
                    dbcontext.tblUserLogoes.Add(userlogo);
                    dbcontext.SaveChanges();
                    file.SaveAs(fullPath);
                }
                tblUserLogoProcess logo = new tblUserLogoProcess();
                logo.Process_Id = Process_Id;
                logo.Stitches_Id = Size_Id;
                logo.Colour_Id = Color_Id;
                logo.UserLogo_Id = userlogo.Id;
                logo.Name = Name;
                logo.Status = true;
                logo.Complexity = Complexity;
                logo.CreatedBy = DataBaseCon.ActiveClientId();
                logo.CreatedOn = DateTime.UtcNow;
                dbcontext.tblUserLogoProcesses.Add(logo);
                dbcontext.SaveChanges();
            }
            return Json(new { fileName = fileName, UserId = userlogo.Id});

        }

        public ActionResult AddLogoMultipleProcess(int logoId,int Process_Id,int Size_Id,int Colour_Id,string Name,int Complexity)
        {
            var userlogo = dbcontext.tblUserLogoes.Where(x => x.Id == logoId).FirstOrDefault();
            tblUserLogoProcess logo = new tblUserLogoProcess();
            logo.Process_Id = Process_Id;
            logo.Stitches_Id = Size_Id;
            logo.Colour_Id = Colour_Id;
            logo.UserLogo_Id = logoId;
            logo.Name = Name;    
            logo.Status = false;
            logo.Complexity = Complexity;
            logo.CreatedBy = DataBaseCon.ActiveClientId();
            logo.CreatedOn = DateTime.UtcNow;
            dbcontext.tblUserLogoProcesses.Add(logo);
            dbcontext.SaveChanges();
            userlogo.Status = 2;
            dbcontext.Entry(userlogo).State = EntityState.Modified;
            dbcontext.SaveChanges();
            response.IsSuccess = true;
            response.Message = "Process added successfully.";
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogoDetails(int id)
        {
            var img = dbcontext.tblUserLogoProcesses.Include(x => x.tblUserLogo).Include(x => x.tblUserLogo.tblStatu).Include(x => x.tblUserLogo.tbluser).Where(x => x.UserLogo_Id == id).ToList().OrderByDescending(x => x.Id);
            var userlogo = Mapper.Map<List<DesignViewModel>>(img);
            foreach (var items in userlogo)
            {
                items.UserName = items.FirstName + " " + items.LastName;
                var proces = dbcontext.tblApplicationProcesses.FirstOrDefault(x => x.Id == items.Process_Id);
                items.ProcessValue = proces.Name;
                var color = dbcontext.tblPrintColors.FirstOrDefault(x => x.Id == items.Color_Id);
                items.ColorValue = color.Name;
                var selectedProcess = dbcontext.tblSizeMasters.FirstOrDefault(x => x.Id == items.Size_Id);
                items.SizeValue = selectedProcess.Size;
            }
            return Json(userlogo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetDefaultSetting(int logoId,int processId)
        {
            var logoList= dbcontext.tblUserLogoProcesses.Where(x => x.UserLogo_Id == logoId).ToList();
            foreach(var item in logoList)
            {
               
                if (item.Id == processId)
                {
                    item.Status = true;
                    dbcontext.Entry(item).State = EntityState.Modified;
                    dbcontext.SaveChanges();
                }
                else
                {
                    item.Status = false;
                    dbcontext.Entry(item).State = EntityState.Modified;
                    dbcontext.SaveChanges();
                }             
            }
            response.IsSuccess = true;
            response.Message = "Successfully set default process.";
            return Json(response, JsonRequestBehavior.AllowGet);

        }
        public ActionResult LogoProcessChange(int id)
        {
            var img = dbcontext.tblUserLogoProcesses.Where(x => x.Id == id).FirstOrDefault();
            var userlogo = Mapper.Map<DesignViewModel>(img);
            
                var proces = dbcontext.tblApplicationProcesses.FirstOrDefault(x => x.Id == userlogo.Process_Id);
                userlogo.ProcessValue = proces.Name;
                var color = dbcontext.tblPrintColors.FirstOrDefault(x => x.Id == userlogo.Color_Id);
                userlogo.ColorValue = color.Name;
                var selectedProcess = dbcontext.tblSizeMasters.FirstOrDefault(x => x.Id == userlogo.Size_Id);
                userlogo.SizeValue = selectedProcess.Size;
            
            return Json(userlogo, JsonRequestBehavior.AllowGet);
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
            if(items.saveId == -1 && items.Id == 0)
            {
                if (items.itemId > 0)
                {
                    var ImageBackPath = "/Content/uploads/Items/" + items.BackImageSource;
                    var ImagePath = "/Content/uploads/Items/" + items.FrontImageSource;
                    var userItem = dbcontext.tblUserItems.Include(x => x.tblOptionProperty).Where(x => x.Id == items.itemId).FirstOrDefault();
                    if ((items.BackLogoId == 0 && userItem.FrontLogoId == null) || (items.FrontLogoId == 0 && userItem.BackLogoId == null))
                    {
                        response.IsSuccess = false;
                        response.Message = "Your design is empty!, please add object";
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                    else if (items.FrontLogoId != 0)
                    {
                        tblUserItem userItems = new tblUserItem();
                        userItems.UserId = DataBaseCon.ActiveClientId();
                        userItems.FrontImageSource = ImagePath;
                        userItems.ImageId = items.ImageId;
                        userItems.FrontLogoId = items.FrontLogoId;
                        userItems.FrontLogoWidth = items.FrontLogoWidth;
                        userItems.FrontLogoheight = items.FrontLogoheight;
                        userItems.UserLogoProcess_id = items.logoProcess_Id;
                        userItems.FrontLogoPositionTop = items.FrontLogoPositionTop;
                        userItems.FrontLogoPositionLeft = items.FrontLogoPositionLeft;
                        userItems.BackLogoId = userItem.BackLogoId;
                        userItems.BackLogoheight = userItem.BackLogoheight;
                        userItems.BackLogoWidth = userItem.BackLogoWidth;
                        userItems.BackLogoPositionTop = userItem.BackLogoPositionTop;
                        userItems.BackLogoPositionLeft = userItem.BackLogoPositionLeft;
                        userItems.BackImageSource = userItem.BackImageSource;
                        if (items.logoProcess_Id == 0)
                        {
                            userItems.UserLogoProcess_id = userItem.UserLogoProcess_id;
                        }
                        else
                        {
                            userItems.UserLogoProcess_id = items.logoProcess_Id;
                        }
                        dbcontext.tblUserItems.Add(userItems);
                        dbcontext.SaveChanges();                       
                        response.IsSuccess = true;
                        response.Message = " Item Successfully added";
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                    else if (items.BackLogoId != 0)
                    {
                        tblUserItem userItems = new tblUserItem();
                        userItems.UserId = DataBaseCon.ActiveClientId();
                        userItems.BackLogoId = items.BackLogoId;
                        userItems.BackLogoheight = items.BackLogoheight;
                        userItems.BackLogoWidth = items.BackLogoWidth;
                        userItems.BackLogoPositionTop = items.BackLogoPositionTop;
                        userItems.BackLogoPositionLeft = items.BackLogoPositionLeft;
                        userItems.BackImageSource = ImageBackPath;
                        userItems.ImageId = items.ImageId;
                        userItems.FrontLogoId = userItem.FrontLogoId;
                        userItems.FrontLogoWidth = userItem.FrontLogoWidth;
                        userItems.FrontLogoheight = userItem.FrontLogoheight;
                        userItems.FrontLogoPositionTop = userItem.FrontLogoPositionTop;
                        userItems.FrontLogoPositionLeft = userItem.FrontLogoPositionLeft;
                        userItems.FrontImageSource = userItem.FrontImageSource;
                        if (items.logoProcess_Id == 0)
                        {
                            userItems.UserLogoProcess_id = userItem.UserLogoProcess_id;
                        }
                        else
                        {
                            userItems.UserLogoProcess_id = items.logoProcess_Id;
                        }
                        dbcontext.tblUserItems.Add(userItems);
                        dbcontext.SaveChanges();                       
                        response.IsSuccess = true;
                        response.Message = " Item Successfully added";
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                    else if (items.FrontLogoId == 0 && userItem.FrontLogoId != 0)
                    {
                        tblUserItem userItems = new tblUserItem();
                        userItems.UserId = DataBaseCon.ActiveClientId();
                        userItems.FrontImageSource = userItem.tblOptionProperty.FrontImage;
                        userItems.ImageId = userItem.ImageId;
                        userItems.UserLogoProcess_id = userItem.UserLogoProcess_id;
                        userItems.BackLogoId = userItem.BackLogoId;
                        userItems.BackLogoheight = userItem.BackLogoheight;
                        userItems.BackLogoWidth = userItem.BackLogoWidth;
                        userItems.BackLogoPositionTop = userItem.BackLogoPositionTop;
                        userItems.BackLogoPositionLeft = userItem.BackLogoPositionLeft;
                        userItems.BackImageSource = userItem.BackImageSource;
                        dbcontext.tblUserItems.Add(userItems);
                        dbcontext.SaveChanges();                        
                        response.IsSuccess = true;
                        response.Message = " Item Successfully added";
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                    else if (items.BackLogoId == 0 && userItem.BackLogoId != 0)
                    {
                        tblUserItem userItems = new tblUserItem();
                        userItems.UserId = DataBaseCon.ActiveClientId();
                        userItems.BackImageSource = userItem.tblOptionProperty.BackImage;
                        userItems.ImageId = userItem.ImageId;
                        userItems.FrontLogoId = userItem.FrontLogoId;
                        userItems.FrontLogoWidth = userItem.FrontLogoWidth;
                        userItems.FrontLogoheight = userItem.FrontLogoheight;
                        userItems.FrontLogoPositionTop = userItem.FrontLogoPositionTop;
                        userItems.FrontLogoPositionLeft = userItem.FrontLogoPositionLeft;
                        userItems.FrontImageSource = userItem.FrontImageSource;
                        userItems.UserLogoProcess_id = userItem.UserLogoProcess_id;
                        dbcontext.tblUserItems.Add(userItems);
                        dbcontext.SaveChanges();                       
                        response.IsSuccess = true;
                        response.Message = " Item Successfully added";
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }

                }

               else if (items.FrontLogoId != 0)
                {
                    var backimage = dbcontext.tblOptionProperties.Where(x => x.Id == items.ImageId).FirstOrDefault();
                    var ImagePath = "/Content/uploads/Items/" + items.FrontImageSource;
                    tblUserItem userItem = new tblUserItem();

                    userItem.UserId = DataBaseCon.ActiveClientId();
                    userItem.ImageId = items.ImageId;
                    userItem.FrontLogoId = items.FrontLogoId;
                    userItem.FrontLogoWidth = items.FrontLogoWidth;
                    userItem.FrontLogoheight = items.FrontLogoheight;
                    userItem.FrontLogoPositionTop = items.FrontLogoPositionTop;
                    userItem.FrontLogoPositionLeft = items.FrontLogoPositionLeft;
                    userItem.UserLogoProcess_id = items.logoProcess_Id;
                    userItem.FrontImageSource = ImagePath;
                    userItem.BackImageSource = backimage.BackImage;
                    dbcontext.tblUserItems.Add(userItem);
                    dbcontext.SaveChanges();
                    response.IsSuccess = true;
                    response.Message = "Item Successfully added";
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Your design is empty!, please add object";
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
            }
           else if (items.Id == 0)
            {
                if(items.itemId>0)
                {
                    var ImageBackPath = "/Content/uploads/Items/" + items.BackImageSource;
                    var ImagePath = "/Content/uploads/Items/" + items.FrontImageSource;
                    var userItem = dbcontext.tblUserItems.Include(x => x.tblOptionProperty).Where(x => x.Id == items.itemId).FirstOrDefault();
                    if(((items.BackLogoId == 0 && items.FrontLogoId == 0) && userItem.FrontLogoId == null) || ((items.FrontLogoId == 0 && items.BackLogoId == 0) && userItem.BackLogoId == null))
                    {
                        response.IsSuccess = false;
                        response.Message = "Your design is empty!, please add object";
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                    else if (items.FrontLogoId != 0)
                    {
                        tblUserItem userItems = new tblUserItem();
                        userItems.UserId = DataBaseCon.ActiveClientId();
                        userItems.FrontImageSource = ImagePath;
                        userItems.ImageId = items.ImageId;
                        userItems.FrontLogoId = items.FrontLogoId;
                        userItems.FrontLogoWidth = items.FrontLogoWidth;
                        userItems.FrontLogoheight = items.FrontLogoheight;
                        userItems.UserLogoProcess_id = items.logoProcess_Id;
                        userItems.FrontLogoPositionTop = items.FrontLogoPositionTop;
                        userItems.FrontLogoPositionLeft = items.FrontLogoPositionLeft;
                        userItems.BackLogoId = userItem.BackLogoId;
                        userItems.BackLogoheight = userItem.BackLogoheight;
                        userItems.BackLogoWidth = userItem.BackLogoWidth;
                        userItems.BackLogoPositionTop = userItem.BackLogoPositionTop;
                        userItems.BackLogoPositionLeft = userItem.BackLogoPositionLeft;
                        userItems.BackImageSource = userItem.BackImageSource;
                        if (items.logoProcess_Id == 0)
                        {
                            userItems.UserLogoProcess_id = userItem.UserLogoProcess_id;
                        }
                        else
                        {
                            userItems.UserLogoProcess_id = items.logoProcess_Id;
                        }
                        dbcontext.tblUserItems.Add(userItems);
                        dbcontext.SaveChanges();
                        response.Id = userItems.Id;
                        response.IsSuccess = true;
                        response.Message = " Item Successfully added";
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                    else if (items.BackLogoId != 0)
                    {
                        tblUserItem userItems = new tblUserItem();
                        userItems.UserId = DataBaseCon.ActiveClientId();
                        userItems.BackLogoId = items.BackLogoId;
                        userItems.BackLogoheight = items.BackLogoheight;
                        userItems.BackLogoWidth = items.BackLogoWidth;
                        userItems.BackLogoPositionTop = items.BackLogoPositionTop;
                        userItems.BackLogoPositionLeft = items.BackLogoPositionLeft;
                        userItems.BackImageSource = ImageBackPath;
                        userItems.ImageId = items.ImageId;
                        userItems.FrontLogoId = userItem.FrontLogoId;
                        userItems.FrontLogoWidth = userItem.FrontLogoWidth;
                        userItems.FrontLogoheight = userItem.FrontLogoheight;
                        userItems.FrontLogoPositionTop = userItem.FrontLogoPositionTop;
                        userItems.FrontLogoPositionLeft = userItem.FrontLogoPositionLeft;
                        userItems.FrontImageSource = userItem.FrontImageSource;
                        if (items.logoProcess_Id == 0)
                        {
                            userItems.UserLogoProcess_id = userItem.UserLogoProcess_id;
                        }
                        else
                        {
                            userItems.UserLogoProcess_id = items.logoProcess_Id;
                        }
                        dbcontext.tblUserItems.Add(userItems);
                        dbcontext.SaveChanges();
                        response.Id = userItems.Id;
                        response.IsSuccess = true;
                        response.Message = " Item Successfully added";
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                    else if(items.FrontLogoId == 0 && userItem.FrontLogoId != 0)
                    {
                        tblUserItem userItems = new tblUserItem();
                        userItems.UserId = DataBaseCon.ActiveClientId();
                        userItems.FrontImageSource = userItem.tblOptionProperty.FrontImage;
                        userItems.ImageId = userItem.ImageId;
                        userItems.UserLogoProcess_id = userItem.UserLogoProcess_id;
                        userItems.BackLogoId = userItem.BackLogoId;
                        userItems.BackLogoheight = userItem.BackLogoheight;
                        userItems.BackLogoWidth = userItem.BackLogoWidth;
                        userItems.BackLogoPositionTop = userItem.BackLogoPositionTop;
                        userItems.BackLogoPositionLeft = userItem.BackLogoPositionLeft;
                        userItems.BackImageSource = userItem.BackImageSource;                       
                        dbcontext.tblUserItems.Add(userItems);
                        dbcontext.SaveChanges();
                        response.Id = userItems.Id;
                        response.IsSuccess = true;
                        response.Message = " Item Successfully added";
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                    else if (items.BackLogoId == 0 && userItem.BackLogoId != 0)
                    {
                        tblUserItem userItems = new tblUserItem();
                        userItems.UserId = DataBaseCon.ActiveClientId();
                        userItems.BackImageSource = userItem.tblOptionProperty.BackImage;
                        userItems.ImageId = userItem.ImageId;
                        userItems.FrontLogoId = userItem.FrontLogoId;
                        userItems.FrontLogoWidth = userItem.FrontLogoWidth;
                        userItems.FrontLogoheight = userItem.FrontLogoheight;
                        userItems.FrontLogoPositionTop = userItem.FrontLogoPositionTop;
                        userItems.FrontLogoPositionLeft = userItem.FrontLogoPositionLeft;
                        userItems.FrontImageSource = userItem.FrontImageSource;
                        userItems.UserLogoProcess_id = userItem.UserLogoProcess_id;                                           
                        dbcontext.tblUserItems.Add(userItems);
                        dbcontext.SaveChanges();
                        response.Id = userItems.Id;
                        response.IsSuccess = true;
                        response.Message = " Item Successfully added";
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                   
                }
               else if (items.BackLogoId != 0 || items.FrontLogoId != 0)
                {
                    var backFrontImage = dbcontext.tblOptionProperties.Where(x => x.Id == items.ImageId).FirstOrDefault();
                    var ImageBackPath = "/Content/uploads/Items/" + items.BackImageSource;
                    var ImagePath = "/Content/uploads/Items/" + items.FrontImageSource;
                    tblUserItem userItem = new tblUserItem();
                    userItem.UserId = DataBaseCon.ActiveClientId();
                    if (items.BackLogoId != 0)
                    {
                        userItem.BackLogoId = items.BackLogoId;
                        userItem.BackLogoheight = items.BackLogoheight;
                        userItem.BackLogoWidth = items.BackLogoWidth;
                        userItem.BackLogoPositionTop = items.BackLogoPositionTop;
                        userItem.ImageId = items.ImageId;
                        userItem.BackLogoPositionLeft = items.BackLogoPositionLeft;
                    }
                   

                    if (items.BackImageSource != null)
                    {
                        userItem.BackImageSource = ImageBackPath;
                        userItem.FrontImageSource = backFrontImage.FrontImage;
                        userItem.UserLogoProcess_id = items.logoProcess_Id;
                    }
                    else
                    {
                        userItem.ImageId = items.ImageId;
                        userItem.FrontLogoId = items.FrontLogoId;
                        userItem.FrontLogoWidth = items.FrontLogoWidth;
                        userItem.FrontLogoheight = items.FrontLogoheight;
                        userItem.UserLogoProcess_id = items.logoProcess_Id;
                        userItem.FrontLogoPositionTop = items.FrontLogoPositionTop;
                        userItem.FrontLogoPositionLeft = items.FrontLogoPositionLeft;
                        userItem.FrontImageSource = ImagePath;
                        userItem.BackImageSource = backFrontImage.BackImage;
                    }
                    dbcontext.tblUserItems.Add(userItem);
                    dbcontext.SaveChanges();
                    response.IsSuccess = true;
                    response.Message = " Item Successfully added";
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Your design is empty!, please add object";
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
            }
            else if(items.saveId == -1 && items.Id >0)
            {              
                var ImagePathFront = "/Content/uploads/Items/" + items.FrontImageSource;
                var userItem = dbcontext.tblUserItems.Include(x=>x.tblOptionProperty).Where(x => x.Id == items.Id).FirstOrDefault();       
                if (items.FrontLogoId == 0 && userItem.BackLogoId == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Your design is empty!, please add object";
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (userItem.FrontLogoId != 0 && items.FrontLogoId != 0)
                    {
                        userItem.FrontLogoId = items.FrontLogoId;
                    }
                    if (userItem.FrontLogoWidth != 0 && items.FrontLogoWidth != 0)
                    {
                        userItem.FrontLogoWidth = items.FrontLogoWidth;
                    }
                    if (userItem.FrontLogoheight != 0 && items.FrontLogoheight != 0)
                    {
                        userItem.FrontLogoheight = items.FrontLogoheight;
                    }
                    if (userItem.FrontLogoPositionTop != 0 && items.FrontLogoPositionTop != 0)
                    {
                        userItem.FrontLogoPositionTop = items.FrontLogoPositionTop;
                    }
                    if (userItem.FrontLogoPositionLeft != 0 && items.FrontLogoPositionLeft != 0)
                    {
                        userItem.FrontLogoPositionLeft = items.FrontLogoPositionLeft;
                    }

                    if (userItem.FrontImageSource != null && userItem.FrontLogoId != 0 && items.FrontImageSource != null && items.FrontLogoId != 0)
                    {
                        userItem.FrontImageSource = ImagePathFront;
                    }
                    else
                    {
                        userItem.FrontImageSource = userItem.tblOptionProperty.FrontImage;
                        userItem.FrontLogoId = null;
                        userItem.FrontLogoPositionLeft = null;
                        userItem.FrontLogoPositionTop = null;
                        userItem.FrontLogoheight = null;
                        userItem.FrontLogoWidth = null;
                      
                    }
                    dbcontext.Entry(userItem).State = EntityState.Modified;
                    dbcontext.SaveChanges();
                    response.IsSuccess = true;
                    response.Message = "Item Successfully Updated ";
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
            }           
            else
            {
                var ImagePath = "/Content/uploads/Items/" + items.BackImageSource;
                var ImagePathFront = "/Content/uploads/Items/" + items.FrontImageSource;
                var userItem = dbcontext.tblUserItems.Include(x=>x.tblOptionProperty).Where(x => x.Id == items.Id).FirstOrDefault();
                if (items.isBack == false)
                {
                    if (userItem.FrontLogoId != 0 && items.FrontLogoId != 0)
                    {
                        userItem.FrontLogoId = items.FrontLogoId;
                        userItem.FrontLogoWidth = items.FrontLogoWidth;
                        userItem.FrontLogoheight = items.FrontLogoheight;
                        userItem.FrontLogoPositionTop = items.FrontLogoPositionTop;
                        userItem.FrontLogoPositionLeft = items.FrontLogoPositionLeft;
                    }


                    if (userItem.FrontImageSource != null && userItem.FrontLogoId != 0 && items.FrontImageSource != null && items.FrontLogoId != 0)
                    {
                        userItem.FrontImageSource = ImagePathFront;
                    }
                    else
                    {
                        userItem.FrontImageSource = userItem.tblOptionProperty.FrontImage;
                        userItem.FrontLogoId = null;
                        userItem.FrontLogoPositionLeft = null;
                        userItem.FrontLogoPositionTop = null;
                        userItem.FrontLogoheight = null;
                        userItem.FrontLogoWidth = null;
                    }
                }
                    if (items.logoProcess_Id == 0)
                    {
                        userItem.UserLogoProcess_id = userItem.UserLogoProcess_id;
                    }
                    else
                    {
                        userItem.UserLogoProcess_id = items.logoProcess_Id;
                    }

                if (items.isBack == true)
                {
                    if (items.BackLogoId != 0)
                    {
                        userItem.BackLogoId = items.BackLogoId;
                        userItem.BackLogoheight = items.BackLogoheight;
                        userItem.BackLogoWidth = items.BackLogoWidth;
                        userItem.BackLogoPositionTop = items.BackLogoPositionTop;
                        userItem.BackLogoPositionLeft = items.BackLogoPositionLeft;
                        userItem.BackImageSource = ImagePath;
                    }
                     else 
                     {
                         userItem.BackImageSource = userItem.tblOptionProperty.BackImage;
                         userItem.BackLogoId = null;
                         userItem.BackLogoPositionLeft = null;
                         userItem.BackLogoPositionTop = null;
                         userItem.BackLogoheight = null;
                         userItem.BackLogoWidth = null;
                     }
                }
               
                dbcontext.Entry(userItem).State = EntityState.Modified;
                dbcontext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Item Successfully added  ";
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            return View();
     
        }

        public ActionResult SaveRange(UserItemsViewModel items)
        {
            var backFrontImage = dbcontext.tblOptionProperties.Where(x => x.Id == items.ImageId).FirstOrDefault();
            var backimage = dbcontext.tblUserItems.Include(x=>x.tblOptionProperty).Where(x => x.Id == items.itemId).FirstOrDefault();
                var ImagePath = "/Content/uploads/Items/" + items.FrontImageSource;
                tblUserItem userItem = new tblUserItem();
            if (items.FrontLogoId == 0 && backimage.BackLogoId == null)
            {
                response.IsSuccess = false;
                response.Message = "Your design is empty!, please add object";
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            else {
                if (items.itemId > 0)
                {
                    if (items.ImageId == 0)
                    {
                        userItem.UserId = DataBaseCon.ActiveClientId();
                        userItem.ImageId = backimage.ImageId;
                        userItem.FrontLogoId = null;
                        userItem.FrontLogoWidth = null;
                        userItem.FrontLogoheight = null;
                        userItem.FrontLogoPositionTop = null;
                        userItem.FrontLogoPositionLeft = null;
                        userItem.FrontImageSource = backimage.tblOptionProperty.FrontImage;
                        userItem.UserLogoProcess_id = backimage.UserLogoProcess_id;
                        userItem.BackLogoId = backimage.BackLogoId;
                        userItem.BackLogoWidth = backimage.BackLogoWidth;
                        userItem.BackLogoheight = backimage.BackLogoheight;
                        userItem.BackLogoPositionTop = backimage.BackLogoPositionTop;
                        userItem.BackLogoPositionLeft = backimage.BackLogoPositionLeft;
                        userItem.BackImageSource = backimage.BackImageSource;
                    }
                    else
                    {
                        userItem.UserId = DataBaseCon.ActiveClientId();
                        userItem.ImageId = items.ImageId;
                        userItem.FrontLogoId = items.FrontLogoId;
                        userItem.FrontLogoWidth = items.FrontLogoWidth;
                        userItem.FrontLogoheight = items.FrontLogoheight;
                        userItem.FrontLogoPositionTop = items.FrontLogoPositionTop;
                        userItem.FrontLogoPositionLeft = items.FrontLogoPositionLeft;
                        userItem.FrontImageSource = ImagePath;
                        userItem.UserLogoProcess_id = backimage.UserLogoProcess_id;
                        userItem.BackLogoId = backimage.BackLogoId;
                        userItem.BackLogoWidth = backimage.BackLogoWidth;
                        userItem.BackLogoheight = backimage.BackLogoheight;
                        userItem.BackLogoPositionTop = backimage.BackLogoPositionTop;
                        userItem.BackLogoPositionLeft = backimage.BackLogoPositionLeft;
                        userItem.BackImageSource = backimage.BackImageSource;
                    }
                    dbcontext.tblUserItems.Add(userItem);
                    dbcontext.SaveChanges();
                    response.Id = userItem.Id;
                    response.IsSuccess = true;
                    response.Message = "Item Successfully added";
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    userItem.UserId = DataBaseCon.ActiveClientId();
                    userItem.ImageId = items.ImageId;
                    userItem.FrontLogoId = items.FrontLogoId;
                    userItem.FrontLogoWidth = items.FrontLogoWidth;
                    userItem.FrontLogoheight = items.FrontLogoheight;
                    userItem.FrontLogoPositionTop = items.FrontLogoPositionTop;
                    userItem.FrontLogoPositionLeft = items.FrontLogoPositionLeft;
                    userItem.FrontImageSource = ImagePath;
                    userItem.UserLogoProcess_id = items.logoProcess_Id;
                    userItem.BackImageSource = backFrontImage.BackImage;
                    dbcontext.tblUserItems.Add(userItem);
                    dbcontext.SaveChanges();
                    response.Id = userItem.Id;
                    response.IsSuccess = true;
                    response.Message = "Item Successfully added";
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
            }
            return View();
            }

            public ActionResult SaveGetQuote(UserItemsViewModel items)
        {
            var UserId = DataBaseCon.ActiveClientId();
            var cartPrice = dbcontext.tblUserItems.Include(x=>x.tblOptionProperty).Include(x => x.tblUserLogoProcess).Where(x => x.Id == items.UserItemId).FirstOrDefault();
            var price = dbcontext.tblPriceLists.Where(_ => _.Process_Id == cartPrice.tblUserLogoProcess.Process_Id && _.Size_Id == cartPrice.tblUserLogoProcess.Stitches_Id && _.Colour_Id == cartPrice.tblUserLogoProcess.Colour_Id && _.MinQty <= items.Quantity && _.MaxQty >=items.Quantity).FirstOrDefault();
            if (price != null)
            {
                var uniPrice = Convert.ToDecimal(cartPrice.tblOptionProperty.CostPrice);
                var pricetshirt = uniPrice + price.Price;
                var totalPrice = pricetshirt * items.Quantity;
                var status = dbcontext.tblStatus.Where(x => x.Name == "Draft").FirstOrDefault();
                var checkQuotes = dbcontext.tblDraftQuotes.Where(x => x.UserId == UserId && x.Status == status.Id).FirstOrDefault();
                if (checkQuotes == null)
                {
                    var order = dbcontext.tblDraftQuotes.Add(new tblDraftQuote
                    {
                        CreatedBy = UserId,
                        CreatedOn = DateTime.UtcNow,
                        isdeleted = false,
                        Status = status.Id,
                        UserId = UserId
                    }) ;
                    dbcontext.SaveChanges();                   
                    tblDraftQuoteItem quotes = new tblDraftQuoteItem();
                    quotes.UserItemId = items.UserItemId;
               /*     quotes.Process_Id = items.Process_Id;
                    quotes.Colour_Id = items.Colour_Id;
                    quotes.Size_Id = items.Size_Id;*/
                    quotes.ImageId = items.ImageId;
                    quotes.Size = items.Size;                   
                    quotes.UserId = DataBaseCon.ActiveClientId();
                    quotes.CreatedBy = DataBaseCon.ActiveClientId();
                    quotes.CreatedOn = DateTime.UtcNow;
                    quotes.Quantity = items.Quantity;
                    quotes.Quotes_Id = order.Id;
                    quotes.Print_Price = price.Price;
                    quotes.Tshirt_Price = Convert.ToInt32(cartPrice.tblOptionProperty.CostPrice);
                    quotes.Unit_Price = pricetshirt;
                    quotes.Totalprice = totalPrice;                   

                    dbcontext.tblDraftQuoteItems.Add(quotes);
                    dbcontext.SaveChanges();
                    var quoteList = dbcontext.tblDraftQuoteItems.Where(x => x.UserId == UserId && x.Quotes_Id == order.Id).ToList();
                    var quoteprice = quoteList.Select(x => x.Totalprice).ToList().Sum();
                    var Quotes = dbcontext.tblDraftQuotes.Where(x => x.UserId == UserId && x.Id == order.Id).FirstOrDefault();
                    Quotes.TotalPrice = quoteprice;
                    dbcontext.Entry(Quotes).State = EntityState.Modified;
                    dbcontext.SaveChanges();
                    response.IsSuccess = true;
                    response.Message = "Item added successfully to quote";
                    return Json(response, JsonRequestBehavior.AllowGet);                   
                }
                else
                {
                    tblDraftQuoteItem quotes = new tblDraftQuoteItem();
                    quotes.UserItemId = items.UserItemId;
                    /*quotes.Process_Id = items.Process_Id;
                    quotes.Colour_Id = items.Colour_Id;
                    quotes.Size_Id = items.Size_Id;*/
                    quotes.ImageId = items.ImageId;
                    quotes.Size = items.Size;                   
                    quotes.UserId = DataBaseCon.ActiveClientId();
                    quotes.CreatedBy = DataBaseCon.ActiveClientId();
                    quotes.CreatedOn = DateTime.UtcNow;
                    quotes.Quantity = items.Quantity;
                    quotes.Quotes_Id = checkQuotes.Id;
                    quotes.Print_Price = price.Price;
                    quotes.Tshirt_Price = Convert.ToInt32(cartPrice.tblOptionProperty.CostPrice);
                    quotes.Unit_Price = pricetshirt;
                    quotes.Totalprice = totalPrice;

                    dbcontext.tblDraftQuoteItems.Add(quotes);
                    dbcontext.SaveChanges();
                    var quoteList = dbcontext.tblDraftQuoteItems.Where(x => x.UserId == UserId && x.Quotes_Id == checkQuotes.Id).ToList();
                    var quoteprice = quoteList.Select(x => x.Totalprice).ToList().Sum();
                    var Quotes = dbcontext.tblDraftQuotes.Where(x => x.UserId == UserId && x.Id == checkQuotes.Id).FirstOrDefault();
                    Quotes.TotalPrice = quoteprice;
                    dbcontext.Entry(Quotes).State = EntityState.Modified;
                    dbcontext.SaveChanges();

                    response.IsSuccess = true;
                    response.Message = "Item added successfully to quote";
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                response.IsSuccess = false;
                response.Message = "Price not found";
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public ActionResult GetQuotes()
        {           
            var UserId = DataBaseCon.ActiveClientId();
            var checkQuotes = dbcontext.tblDraftQuotes.Where(x => x.UserId == UserId && x.Status == 1).FirstOrDefault();
            if (checkQuotes != null)
            {
                var cartList = Mapper.Map<QuotesViewModel>(checkQuotes);
                var statu = dbcontext.tblStatus.Where(x => x.Id == cartList.Status).FirstOrDefault();
                cartList.StatusName = statu.Name;
                return Json(cartList, JsonRequestBehavior.AllowGet);
            }
            return View();
        }
        public ActionResult Quote()
        {
            var UserId = DataBaseCon.ActiveClientId();           
            var status = dbcontext.tblStatus.ToList();
            var checkQuotes = dbcontext.tblDraftQuotes.Where(x => x.UserId == UserId && x.isdeleted == false).ToList().OrderByDescending(x=>x.Id);
            var cartList = Mapper.Map<List<QuotesViewModel>>(checkQuotes);
            foreach (var items in cartList)
            {
                var quotes = dbcontext.tblDraftQuoteItems.Where(x => x.Quotes_Id == items.Id).ToList();
                items.TotalItems = quotes.Count;
                var statu = dbcontext.tblStatus.Where(x => x.Id == items.Status).FirstOrDefault();
                items.StatusName = statu.Name;
            }
            return View(cartList);            
        }

        public ActionResult QuoteList(int id)
        {
            var userId = DataBaseCon.ActiveClientId();
            var quotes = dbcontext.tblDraftQuoteItems.Include(x => x.tblUserItem).Include(x => x.tblUserItem.tblUserLogoProcess).Where(x => x.UserId == userId && x.Quotes_Id == id).ToList();
            var cartList = Mapper.Map<List<UserItemsViewModel>>(quotes);
            foreach (var items in cartList)
            {
                var proces = dbcontext.tblApplicationProcesses.FirstOrDefault(x => x.Id == items.Process_Id);
                items.ProcessValue = proces.Name;
            }
            foreach (var items in cartList)
            {
                var process = dbcontext.tblPrintColors.FirstOrDefault(x => x.Id == items.Colour_Id);
                items.ColorValue = process.Name;
            }

            foreach (var item in cartList)
            {
                var selectedProcess = dbcontext.tblSizeMasters.FirstOrDefault(x => x.Id == item.Size_Id);
                item.SizeValue = selectedProcess.Size;
            }
            return Json(cartList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateQuotes(int id)
        {                      
            var userId = DataBaseCon.ActiveClientId();
            var status = dbcontext.tblStatus.Where(x => x.Name == "Pending").FirstOrDefault();
            var quotesItem = dbcontext.tblDraftQuoteItems.Where(x => x.Quotes_Id == id).ToList();
            var checkQuotes = dbcontext.tblDraftQuotes.Where(x => x.UserId == userId && x.Id == id).FirstOrDefault();
            var price = quotesItem.Select(x => x.Totalprice).ToList().Sum();
            checkQuotes.Status = status.Id;
            checkQuotes.TotalPrice = price;

            dbcontext.Entry(checkQuotes).State = EntityState.Modified;
            dbcontext.SaveChanges();
            response.IsSuccess = true;
            response.Message = "Quote requested successfully.";
            return Json(response, JsonRequestBehavior.AllowGet);           
        }

        public ActionResult MyItems()
        {
            var userId = DataBaseCon.ActiveClientId();
            var myItems = dbcontext.tblUserItems.Where(x => x.UserId == userId).ToList().OrderByDescending(x=>x.Id);

            return Json(Mapper.Map<List<UserItemsViewModel>>(myItems), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSelectedItem(int id)
        {
            var item = dbcontext.tblUserItems.Include(x => x.tblOptionProperty).Where(x => x.Id == id).FirstOrDefault();
            return Json(Mapper.Map<UserItemsViewModel>(item), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetItem(int ImageId)
        {
            var item = dbcontext.tblUserItems.Where(x => x.Id == ImageId).FirstOrDefault();
            return Json(Mapper.Map<UserItemsViewModel>(item), JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddToCart(int id)
        {

            var orderList = dbcontext.tblDraftQuoteItems.Include(x=>x.tblUserItem).Include(x=>x.tblUserItem.tblUserLogoProcess).Where(x => x.Quotes_Id == id).ToList();
            
            var quote = dbcontext.tblDraftQuotes.Where(x => x.Id == id).FirstOrDefault();
            var userId = DataBaseCon.ActiveClientId();
            foreach (var item in orderList)
            {
                var checkOrder = dbcontext.tblDraftOrders.Where(x => x.UserId == userId && x.IsDeleted == false).FirstOrDefault();
                var tshirtPrice = dbcontext.tblOptionProperties.Where(x => x.Id ==item.tblUserItem.ImageId).FirstOrDefault();
                var uniPrice = Convert.ToDecimal(tshirtPrice.CostPrice);
                var price = uniPrice + item.Print_Price;
                var totalPrice = price * item.Quantity;
                if (checkOrder == null)
                {
                    var order = dbcontext.tblDraftOrders.Add(new tblDraftOrder
                    {
                        CreatedBy = userId,
                        CreateOn = DateTime.UtcNow,
                        IsDeleted = false,
                        Status = true,
                        UserId = userId
                    });
                    dbcontext.SaveChanges();

                    tblDraftOrderItem orderItem = new tblDraftOrderItem
                    {
                        Quantity = item.Quantity,
                        LogoPrice = item.Print_Price,
                        CreatedBy = userId,
                        CreatedOn = DateTime.UtcNow,                      
                        Process = item.tblUserItem.tblUserLogoProcess.Process_Id,                        
                        OrderId = order.Id,
                        IsDeleted = false,
                        Color = item.tblUserItem.tblUserLogoProcess.Colour_Id,
                        Stitches = item.tblUserItem.tblUserLogoProcess.Stitches_Id,
                        Tshirt_Price = Convert.ToDecimal(tshirtPrice.CostPrice),
                        Unit_Price = price,
                        Total_Price = totalPrice,
                        QuotesItem_Id = item.Id
                    };
                    dbcontext.tblDraftOrderItems.Add(orderItem);
                    dbcontext.SaveChanges();
                }
                else
                {                                                          
                    tblDraftOrderItem orderItem = new tblDraftOrderItem
                    {
                        Quantity = item.Quantity,
                        LogoPrice = item.Print_Price,
                        CreatedBy = userId,
                        CreatedOn = DateTime.UtcNow,
                        Process = item.tblUserItem.tblUserLogoProcess.Process_Id,
                        OrderId = checkOrder.Id,
                        IsDeleted = false,
                        Color = item.tblUserItem.tblUserLogoProcess.Colour_Id,
                        Stitches = item.tblUserItem.tblUserLogoProcess.Stitches_Id,
                        Tshirt_Price = Convert.ToDecimal(tshirtPrice.CostPrice),
                        Unit_Price = price,
                        Total_Price = totalPrice,
                        QuotesItem_Id = item.Id
                    };
                    dbcontext.tblDraftOrderItems.Add(orderItem);
                    dbcontext.SaveChanges();
                }
            }
            quote.isdeleted = true;
            dbcontext.Entry(quote).State = EntityState.Modified;
            dbcontext.SaveChanges();
            response.IsSuccess = true;
            response.Message = "Add item to Cart";
            return Json(response, JsonRequestBehavior.AllowGet);           
        }

        public ActionResult ReorderCartItem(int id)
        {
            var quotesItem = dbcontext.tblDraftOrderItems.Where(x => x.Id == id).FirstOrDefault();

            var orderList = dbcontext.tblDraftQuoteItems.Include(x => x.tblUserItem).Include(x => x.tblUserItem.tblUserLogoProcess).Where(x => x.Id == quotesItem.QuotesItem_Id).FirstOrDefault();
            var userId = DataBaseCon.ActiveClientId();
           
                var checkOrder = dbcontext.tblDraftOrders.Where(x => x.UserId == userId && x.IsDeleted == false).FirstOrDefault();
            var tshirtPrice = dbcontext.tblOptionProperties.Where(x => x.Id == orderList.ImageId).FirstOrDefault();
            var uniPrice = Convert.ToDecimal(tshirtPrice.CostPrice);
            var price = uniPrice + orderList.Print_Price;
            var totalPrice = price * orderList.Quantity;
            if (checkOrder == null)
                {
            
                var order = dbcontext.tblDraftOrders.Add(new tblDraftOrder
                    {
                        CreatedBy = userId,
                        CreateOn = DateTime.UtcNow,
                        IsDeleted = false,
                        Status = true,
                        UserId = userId
                    });
                    dbcontext.SaveChanges();

                tblDraftOrderItem orderItem = new tblDraftOrderItem
                {
                    Quantity = orderList.Quantity,
                    LogoPrice = orderList.Print_Price,
                    CreatedBy = userId,
                    CreatedOn = DateTime.UtcNow,
                    Process = orderList.tblUserItem.tblUserLogoProcess.Process_Id,
                    OrderId = order.Id,
                    IsDeleted = false,
                    Color = orderList.tblUserItem.tblUserLogoProcess.Colour_Id,
                    Stitches = orderList.tblUserItem.tblUserLogoProcess.Stitches_Id,
                    Tshirt_Price = Convert.ToDecimal(tshirtPrice.CostPrice),
                    Unit_Price = price,
                    Total_Price = totalPrice,
                    QuotesItem_Id = quotesItem.QuotesItem_Id
                    };
                    dbcontext.tblDraftOrderItems.Add(orderItem);
                    dbcontext.SaveChanges();
                }
                else
                {
                    tblDraftOrderItem orderItem = new tblDraftOrderItem
                    {
                        Quantity = orderList.Quantity,
                        LogoPrice = orderList.Print_Price,
                        CreatedBy = userId,
                        CreatedOn = DateTime.UtcNow,
                        Process = orderList.Process_Id,
                        OrderId = checkOrder.Id,
                        IsDeleted = false,
                        Color = orderList.Colour_Id,
                        Stitches = orderList.Size_Id,
                        Tshirt_Price = Convert.ToDecimal(tshirtPrice.CostPrice),
                        Unit_Price = price,
                        Total_Price = totalPrice,
                        QuotesItem_Id = quotesItem.QuotesItem_Id
                    };
                    dbcontext.tblDraftOrderItems.Add(orderItem);
                    dbcontext.SaveChanges();
                }                       
            response.IsSuccess = true;
            response.Message = "Add item to Cart";
            return Json(response, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Cart()
        {
            return View();
        }

        public ActionResult MyCart()
        {
            var userId = DataBaseCon.ActiveClientId();
            var checkOrder = dbcontext.tblDraftOrders.Where(x => x.UserId == userId && x.IsDeleted == false).FirstOrDefault();
            if (checkOrder != null)
            {
                var cart = dbcontext.tblDraftOrderItems.Include(x=>x.tblDraftQuoteItem).Include(x=>x.tblDraftQuoteItem.tblUserItem).Include(x => x.tblDraftQuoteItem.tblUserItem.tblUserLogoProcess).Where(x=>x.OrderId==checkOrder.Id && x.IsDeleted == false).ToList();
                if (cart.Count != 0)
                {
                    var cartList = Mapper.Map<List<DraftOrdersItemViewModel>>(cart);
                    var materProcessList = dbcontext.tblApplicationProcesses.ToList();
                    foreach (var items in cartList)
                    {
                        var userItem = dbcontext.tblDraftQuoteItems.Include(x => x.tblUserItem).Where(x => x.Id == items.QuotesItem_Id).FirstOrDefault();
                        items.FrontImage = userItem.tblUserItem?.FrontImageSource?? string.Empty;
                    }

                    foreach (var item in cartList)
                    {
                        var selectedProcess = materProcessList.FirstOrDefault(x => x.Id == item.Process);
                        item.ProcessValue = selectedProcess.Name;
                        var selectedColor = dbcontext.tblPrintColors.FirstOrDefault(x => x.Id == item.Color);
                        item.ColorValue = selectedColor.Name;
                        var selectedSize = dbcontext.tblSizeMasters.FirstOrDefault(x => x.Id == item.Stitches);
                        item.StitchesValue = selectedSize.Size;
                    }
                    return Json(cartList, JsonRequestBehavior.AllowGet);
                }
            }
            return View();           
        }

        [HttpPost]
        public ActionResult UpdateCart(List<CartDataListViewModel> cartDataList )
        {
            for (int i = 0; i < cartDataList.Count; i++)
            {
                var item = dbcontext.tblDraftOrderItems.Find(cartDataList[i].Id);
                if(cartDataList[i].Quantity == 0)
                {
                    item.Quantity = cartDataList[i].Quantity;
                    item.IsDeleted = true;
                    dbcontext.Entry(item).State = EntityState.Modified;
                }
                else
                {
                    item.Quantity = cartDataList[i].Quantity;
                    dbcontext.Entry(item).State = EntityState.Modified;
                }                
                dbcontext.SaveChanges();
            }
            return Content("ok");
        }

        public ActionResult UpdateCartItem(int id,int quantity)
        {
            var item = dbcontext.tblDraftOrderItems.Find(id);
            if (quantity != 0)
            {
                item.Quantity = quantity;
                item.UpdatedBy = DataBaseCon.ActiveClientId();
                item.UpdatedOn = DateTime.UtcNow;
                dbcontext.Entry(item).State = EntityState.Modified;
                
            }
            else
            {
                item.Quantity = 0;
                item.IsDeleted = true;
                item.UpdatedBy = DataBaseCon.ActiveClientId();
                item.UpdatedOn = DateTime.UtcNow;
                dbcontext.Entry(item).State = EntityState.Modified;
            }
            dbcontext.SaveChanges();
            return Content("ok");
        }

        public ActionResult AddAddress()
        {
            int userid = DataBaseCon.ActiveClientId();
            var userAddres = dbcontext.tblUserAddressMappings.Include(x => x.tbluser).Include(x => x.tblAddress).Where(x => x.UserId == userid).ToList();
            //var userAddressList = dbcontext.tblAddresses.FirstOrDefault(x => x.AddressId == userAddres.AddressId);
            foreach (var item in userAddres)
            {
                item.tblAddress.tblUserAddressMappings = null;
                item.tbluser = null;
            }
          
            return Json(userAddres, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CheckOut()
        {
            var states = dbcontext.tblStates.ToList();
            ViewBag.state = states;
            return View();
        }

        public ActionResult OrderDetail(float shippingCharge,float TotalPrice)
        {
            var userId = DataBaseCon.ActiveClientId();
            var checkOrder = dbcontext.tblDraftOrders.Where(x => x.UserId == userId && x.IsDeleted == false).FirstOrDefault();
            var cart = dbcontext.tblDraftOrderItems.Where(x=>x.OrderId == checkOrder.Id && x.IsDeleted == false ).ToList();     
                
                checkOrder.TotalPrice = Convert.ToDecimal(TotalPrice);
                checkOrder.ShippingCharge = shippingCharge;
                checkOrder.Quantity = cart.Count;
                checkOrder.UpdatedBy = userId;
                checkOrder.UpdatedOn = DateTime.UtcNow;
                checkOrder.Status = false;
                checkOrder.IsDeleted = true;
            checkOrder.OrderDate = DateTime.UtcNow;
                dbcontext.Entry(checkOrder).State = EntityState.Modified;
                dbcontext.SaveChanges();

            response.IsSuccess = true;
            response.Message = "Thank you for buying product";
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult OrderAddress (OrderAddressViewModel addressobj)
        {
            try
            {

            
            var userId = DataBaseCon.ActiveClientId();
            var checkOrder = dbcontext.tblDraftOrders.Where(x => x.UserId == userId && x.IsDeleted == false).FirstOrDefault();
            if (checkOrder != null)
            {
                OrderAddress orderadd = new OrderAddress();
                orderadd.Name = addressobj.Name;
                orderadd.Address = addressobj.Address;
                orderadd.OrderId = checkOrder.Id;
                orderadd.State = addressobj.State;
                orderadd.Postcode = addressobj.PostCode;
                orderadd.createdBy = userId;
                orderadd.createdOn = DateTime.UtcNow;
                dbcontext.OrderAddresses.Add(orderadd);
                dbcontext.SaveChanges();
            }
            }
            catch (Exception ex)
            {

                throw;
            }
            return Json("Thank you for buying product");
        }

        public ActionResult DropDown(int id)
        {
            var color = dbcontext.tblApplicationColorsMappings.Include(x => x.tblPrintColor).Where(_ => _.Process_Id == id).ToList();
            return Json(Mapper.Map<List<PrintColorViewModel>>(color),JsonRequestBehavior.AllowGet);
        }
            
        public ActionResult SizeDropDownList(int id)
        {
            var size = dbcontext.TblSizeApplicationMappings.Include(x => x.tblSizeMaster).Where(_ => _.Process_Id == id).ToList();
            return Json(Mapper.Map<List<SizeViewModel>>(size), JsonRequestBehavior.AllowGet);
        }
        
             public ActionResult PriceList(int process, int color, int size, int quantity)
        {
                       
           var price = dbcontext.tblPriceLists.Include(x=>x.tblApplicationProcess).Include(x=>x.tblPrintColor).Include(x=>x.tblSizeMaster).Where(_=>_.Process_Id == process && _.Size_Id==size && _.Colour_Id==color && _.MinQty<=quantity && _.MaxQty>=quantity).FirstOrDefault();
            if (price != null)
            {
                price.tblApplicationProcess = null;
                price.tblPrintColor = null;
                price.tblSizeMaster = null;
                return Json(price, JsonRequestBehavior.AllowGet);
            }
            else
            {
                response.IsSuccess = true;
                response.Message = "Price not Found";
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public ActionResult Price(int quantity,int cartPriceId, int itemProcess)
        {
            if(cartPriceId > 0)
            {
                var cartPrice = dbcontext.tblUserLogoProcesses.Where(x => x.Id == cartPriceId).FirstOrDefault();
                var price = dbcontext.tblPriceLists.Where(_ => _.Process_Id == cartPrice.Process_Id && _.Size_Id == cartPrice.Stitches_Id && _.Colour_Id == cartPrice.Colour_Id && _.MinQty <= quantity && _.MaxQty >= quantity).FirstOrDefault();
                if (price != null)
                {
                    response.IsSuccess = true;
                    response.Message = "Price Found";
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Price Can't be Found";
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var cartPrice = dbcontext.tblUserItems.Include(x=>x.tblUserLogoProcess).Where(x => x.Id == itemProcess).FirstOrDefault();
                var price = dbcontext.tblPriceLists.Where(_ => _.Process_Id == cartPrice.tblUserLogoProcess.Process_Id && _.Size_Id == cartPrice.tblUserLogoProcess.Stitches_Id && _.Colour_Id == cartPrice.tblUserLogoProcess.Colour_Id && _.MinQty <= quantity && _.MaxQty >= quantity).FirstOrDefault();
                if (price != null)
                {
                    response.IsSuccess = true;
                    response.Message = "Price Found";
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Price Can't be Found";
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

            }
            return View();
        }

        public ActionResult QuotesFilter(string status)
        {
            var userId = DataBaseCon.ActiveClientId();
            var Status = dbcontext.tblStatus.Where(x => x.Name == status).FirstOrDefault();
            var checkQuotes = dbcontext.tblDraftQuotes.Where(x => x.Status == Status.Id && x.UserId == userId && x.isdeleted == false).ToList().OrderByDescending(x=>x.Id);
            var cartList = Mapper.Map<List<QuotesViewModel>>(checkQuotes);

            foreach (var items in cartList)
            {
                var statu = dbcontext.tblStatus.Where(x => x.Id == items.Status).FirstOrDefault();
                items.StatusName = statu.Name;
                var quotes = dbcontext.tblDraftQuoteItems.Where(x => x.Quotes_Id == items.Id).ToList();
                items.TotalItems = quotes.Count;
            }
            return Json(cartList, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult ProcessList()
        {
            var process = dbcontext.tblApplicationProcesses.ToList();
            return Json(Mapper.Map<List<ProcessViewModel>>(process), JsonRequestBehavior.AllowGet);

        }
        public ActionResult ColorList(int processid)
        {
            var color = dbcontext.tblApplicationColorsMappings.Include(x => x.tblPrintColor).Where(_ => _.Process_Id == processid).ToList();
            return Json(Mapper.Map<List<PrintColorViewModel>>(color), JsonRequestBehavior.AllowGet);
        }

        public ActionResult SizeList(int processid)
        {
            var size = dbcontext.TblSizeApplicationMappings.Include(x => x.tblSizeMaster).Where(_ => _.Process_Id == processid).ToList();
            return Json(Mapper.Map<List<SizeViewModel>>(size), JsonRequestBehavior.AllowGet);
        }

    }

}