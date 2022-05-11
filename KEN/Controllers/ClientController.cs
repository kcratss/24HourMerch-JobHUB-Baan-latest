using AutoMapper;
using Intuit.Ipp.Data;
using KEN.AppCode;
using KEN.Interfaces.Iservices;
using KEN.Interfaces.Repository;
using KEN.Models;
using KEN_DataAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace KEN.Controllers
{
    public class ClientController : Controller
    {

        private readonly IRepository<tbluser> _tblUsersRepository;
       

        ResponseMessageViewModel response = new ResponseMessageViewModel();

       
        KENNEWEntities dbcontext = new KENNEWEntities();
        public ClientController(IRepository<tbluser> tblUsersRepository)
        {
            _tblUsersRepository = tblUsersRepository;

           
        }
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult Register(ClientRegisterViewModel model)
        {
            try
            {
                var user = _tblUsersRepository.Get(x => x.email == model.Email).FirstOrDefault();
                if (user != null)
                {
                    response.IsSuccess = false;
                    response.Message = "Email already exists";

                    return Json(response, JsonRequestBehavior.AllowGet);
                }


                var tblUserEntity = Mapper.Map<tbluser>(model);

                tblUserEntity.status = "active";
                tblUserEntity.admin = false;
                tblUserEntity.CreatedBy = DataBaseCon.ActiveUser();
                tblUserEntity.created = DateTime.Now;
                tblUserEntity.title = "Online User";
                tblUserEntity.UserRole = "Online"; //Account Manager
                tblUserEntity.UserId = Guid.NewGuid();
                tblUserEntity.IsVerified = false;
                tblUserEntity.access = "";
                tblUserEntity.hashed_password = DataBaseCon.Encrypt(model.Password);

                _tblUsersRepository.Insert(tblUserEntity);
                _tblUsersRepository.Save();


                var rootPath = HostingEnvironment.ApplicationPhysicalPath;

                var pathToFile = rootPath + @"Templates\EmailVerification.html";

                string body = System.IO.File.ReadAllText(pathToFile);

                string rootUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string link = rootUrl + "/Client/VerifyEmail/" + tblUserEntity.UserId.ToString();
                body = System.IO.File.ReadAllText(pathToFile);
                body = body.Replace("[USERNAME]", tblUserEntity.firstname + " " + tblUserEntity.lastname);
                body = body.Replace("[LINK]", link);


                var status = DataBaseCon.SendEmailWithName(model.Email, "Verification Email", body);
                if (status == true)
                {
                    response.IsSuccess = true;
                    response.Message = "Thanks For Registering. we have sent you an email please verify that.";

                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Registration not success.";

                    return Json(response, JsonRequestBehavior.AllowGet);
                }

            }

            catch (Exception)
            {

                response.IsSuccess = false;
                response.Message = "something went wrong.Please fill values for register.";
                return Json(response, JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult VerifyEmail(string id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "Something went wrong");
                return View();
            }
            var userId = Guid.Parse(id);
            var user = _tblUsersRepository.Get(x => x.UserId == userId && x.IsVerified == false).FirstOrDefault();
            if (user != null)
            {
                user.IsVerified = true;
                _tblUsersRepository.Update(user);
                _tblUsersRepository.Save();
            }

            return View();

        }
        public JsonResult SetValueFromCookies(string Emailid, string password, bool RememberMe)
        {
            if (Request.Cookies[Emailid] != null)
            {
                var user = dbcontext.tblusers.Where(_ => _.email == Emailid).FirstOrDefault();
                password = DataBaseCon.Decrypt(user.hashed_password);
                RememberMe = true;
            }
            return Json(password, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(ClientLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var encriptPassword = DataBaseCon.Encrypt(model.Password);
                var getData = dbcontext.tblusers.Where(x => x.hashed_password == encriptPassword && x.email == model.Email).FirstOrDefault();
                if (getData != null)
                {
                    if (model.RememberMe == true)
                    {
                        HttpCookie sessionCookie = new HttpCookie("UserSettings");

                        Response.Cookies[model.Email].Expires = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime())).AddDays(30);
                        Response.Cookies[model.Password].Expires = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime())).AddDays(30);
                    }
                    else
                    {
                        Response.Cookies[model.Email].Expires = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime())).AddDays(-1);
                        Response.Cookies[model.Password].Expires = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime())).AddDays(-1);
                    }

                    Session["UserEmail"] = getData.email;

                   
                    var activeClient = DataBaseCon.ActiveUser();

                    var user = dbcontext.tblusers.Where(x => x.email == activeClient && x.admin == false).FirstOrDefault();


                    if (user != null)
                    {
                        Session["MyClientId"] = user.id;
                        if (user.IsProfileCompleted == true)
                        {
                            //return RedirectToAction("ClientOpportunityList", "Client")
                                 return RedirectToAction("ClientOrderList", "Order");
                        }


                        //return RedirectToAction("EditContactProfile", "Client");
                        return RedirectToAction("EditProfile", "Contact");
                    }
                    else
                    {

                        ModelState.AddModelError("", "Account does not exists.");
                        return View();
                    }


                }
                else
                {

                    ModelState.AddModelError("", "Invalid username or password.");
                    return View();

                }
            }
            return RedirectToAction("EditProfile", "Contact");
        }

        public ActionResult ForgotPassword()

        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(ClientForgotPasswordViewModel model)
        {
            try
            {
                var user = _tblUsersRepository.Get(x => x.email == model.Email).FirstOrDefault();
                if (user == null)
                {
                    ModelState.AddModelError("", "Email does not exists.");
                    return View();
                }
                var rootPath = HostingEnvironment.ApplicationPhysicalPath;

                var pathToFile = rootPath + @"Templates\ForgotPassword.html";

                string body = System.IO.File.ReadAllText(pathToFile);

                string rootUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string link = rootUrl + "/Client/ResetPassword/" + user.UserId.ToString();
                body = System.IO.File.ReadAllText(pathToFile);
                body = body.Replace("[USERNAME]", user.firstname + " " + user.lastname);
                body = body.Replace("[LINK]", link);


                var status = DataBaseCon.SendEmailWithName(model.Email, "Verification Email", body);
                if (status == true)
                {
                    return RedirectToAction("ForgotPasswordConfirm", "Client");
                }

                else
                {
                    ModelState.AddModelError("", "Email does not exists.");
                }
            }
            catch (Exception)
            {

                ModelState.AddModelError("", "Email does not exists.");
            }

            return View();
        }

        public ActionResult ForgotPasswordConfirm()
        {
            return View();
        }


        public ActionResult ResetPassword(string id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "Something went wrong");
                return View();
            }
            var userId = Guid.Parse(id);
            var userTemp = _tblUsersRepository.Get(x => x.UserId == userId).FirstOrDefault();
            var viewUser = Mapper.Map<ClientResetPasswordViewModel>(userTemp);
            return View(viewUser);
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResetPassword(ClientResetPasswordViewModel model)
        {
            var user = _tblUsersRepository.Get(x => x.UserId == model.UserId && x.email == model.Email).FirstOrDefault();
            if (user != null)
            {
                user.hashed_password = DataBaseCon.Encrypt(model.Password);
                _tblUsersRepository.Update(user);
                _tblUsersRepository.Save();
                return RedirectToAction("ResetPasswordConfirm", "Client");
            }

            return View();
        }
        public ActionResult ResetPasswordConfirm()
        {
            return View();
        }


        [AllowAnonymous]
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login", "Client");
        }

    }
}
