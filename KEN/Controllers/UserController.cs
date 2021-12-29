using KEN_DataAccess;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KEN.Models;
using AutoMapper;
using KEN.Interfaces;
using KEN.Interfaces.Iservices;
using KEN.Interfaces.Repository;
using KEN.AppCode;

namespace KEN.Controllers
{
    public class UserController : Controller
    {
        ResponseViewModel response = new ResponseViewModel();
        private readonly ILoginService _baseService;
        KENNEWEntities dbcontext = new KENNEWEntities();
        public UserController(ILoginService baseService)
        {
            _baseService = baseService;
        }
     
        [Route("~/Login")]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            return Redirect("Login");
        }
        [HttpPost]
        [Route("~/login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(KENLoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var EncriptPassword = DataBaseCon.Encrypt(model.Password);
                var GetData = _baseService.GetByUsername(model.Email, EncriptPassword);
                if (GetData != null)
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
                   Session["UserEmail"] = GetData.email;
                    // baans change 16th November for saving the user profile
                    var CurrentUser = 100000;
                    var ActiveUser = DataBaseCon.ActiveUser();
                    var User = dbcontext.tblusers.Where(_ => _.email == ActiveUser && _.UserRole == "Account Manager").FirstOrDefault();
                    var IsAdmin = dbcontext.tblusers.Where(_ => _.email == ActiveUser && _.admin == true).FirstOrDefault();
                    if (User != null)
                    {
                        CurrentUser = User.id;
                    }
                    if(User == null && IsAdmin != null)
                    {
                        CurrentUser = 0;
                    }

                    
                    Session.Add("MyUser", CurrentUser);
                    Session.Add("EventType", "LoadEvent");
                    
                    // baans end 16th November
                    return RedirectToAction("OpportunityList", "Opportunity", new {  id = UrlParameter.Optional });
                   
                }
                else
                {

                    ModelState.AddModelError("Username", "Invalid username or password.");

                }
            }
            return View(model);
        }

        // baans change 4th July for Remember Me
        public ActionResult SetValueFromCookies(string Emailid, string password, bool RememberMe)
        {
            if (Request.Cookies[Emailid] != null)
            {
                var user = dbcontext.tblusers.Where(_ => _.email == Emailid).FirstOrDefault();
                password = DataBaseCon.Decrypt(user.hashed_password);
                RememberMe = true;
            }
            return Json(password, JsonRequestBehavior.AllowGet);
        }
        // baans end 4th July

        [HttpPost]
        [Route("~/login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = dbcontext.tblusers.Where(_ => _.email == model.Email).FirstOrDefault();
                if(user != null)
                {
                    return RedirectToAction("Login", "User");
                }
                else
                {
                    ModelState.AddModelError("Username", "Invalid username or password.");
                }
            }
            return View(model);
        }

        public ActionResult ForgotPasswordSendMail(string EmailId)
        {
            try
            {
                var getMailId = Mapper.Map<KENLoginViewModel>(dbcontext.tblusers.Where(_ => _.email == EmailId).FirstOrDefault());
                if (getMailId != null)
                {

                    if (ModelState.IsValid)
                    {
                        string To = getMailId.Email;
                        string subject = "For Testing";
                        string Body = "Hello," + "<br><br>" + "Based on your request the password for logging has been reset to 'ken123'. ";
                        DataBaseCon.SendEmailWithName(To, subject, Body);
                    }
                    var updatePassword = dbcontext.tblusers.Where(_ => _.email == EmailId).FirstOrDefault();
                    updatePassword.hashed_password = DataBaseCon.Encrypt("ken123");
                    dbcontext.SaveChanges();
                    response.Result = ResponseType.Success;
                    response.Message = "The Mail has been sent to your Email Id";
                }
                else
                {
                    response.Result = ResponseType.Warning;
                    response.Message = "The Email Id you have entered is not Valid. please try with the valid mail id";
                }
                
            }
            catch(Exception e)
            {
                response.Result = ResponseType.Error;
                response.Message = "Some Error Occured. Please Contract Your admin !!!";
            }
            return Json(response, JsonRequestBehavior.AllowGet);

        }


    }

}