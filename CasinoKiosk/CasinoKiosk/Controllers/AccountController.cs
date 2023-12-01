using CasinoKiosk.Areas.Admin.Interfaces;
using CasinoKiosk.Areas.Admin.Services;
using CasinoKiosk.Common;
using CKDatabaseConnection.Common;
using CKDatabaseConnection.DAO;
using CKDatabaseConnection.EF;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using static CasinoKiosk.Areas.Admin.Models.AccountModels;

namespace CasinoKiosk.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        // GET: Admin/Account

        public IMembershipService MembershipService { get; set; }

        public ITHoTram_CustomReportEntities _usersContext = null;

        public CKFunction function = null;

        public UserDao userDao = null;

        
        protected override void Initialize(RequestContext requestContext)
        {
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        //
        // GET: /Account/LogIn
        public ActionResult LogIn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogIn(LogOnModel model, string returnUrl)
        {
           
            if (ModelState.IsValid)
            {
                function = new CKFunction();
                var res1 = isWindowsAccount(model.UserName, model.Password);

                if (res1 == 1)
                {
                    SetupFormsAuthTicket(model.UserName, model.RememberMe);
                    function.updateLastLoginCasinoKioskUsers(model.UserName);
                    function.updateLastLoginFOPatronUsers(model.UserName);

                    var cu = _usersContext.CasinoKioskUsers.SingleOrDefault(x => x.CasinoKiosk_UserName == model.UserName);
                    var cur = _usersContext.CasinoKioskUserRoles.SingleOrDefault(x => x.UserId == cu.UserID);

                    Session["UserID"] = cu.UserID.ToString();
                    Session["UserName"] = cu.CasinoKiosk_UserName.ToString();
                    Session["RoleID"] = cur.RoleId.ToString();

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                    
                }
                else if (res1 == 0)
                {
                    ModelState.AddModelError("", "Authentication failed.");
                }
                else if (res1 == -1)
                {
                    ModelState.AddModelError("", "The account is deactivated.");
                }
                else if (res1 == -2)
                {
                    ModelState.AddModelError("", "The password is incorrect.");
                }
            }
            // ModelState.AddModelError("", "The user name is incorrect.");

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOn

        //public ActionResult LogOn()
        //{
        //    return View();
        //}

        ////
        //// POST: /Account/LogOn

        //[HttpPost]
        //public ActionResult LogOn(LogOnModel model, string returnUrl)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (MembershipService.ValidateUser(model.UserName, model.Password))
        //        {
        //            SetupFormsAuthTicket(model.UserName, model.RememberMe);

        //            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
        //                && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
        //            {
        //                return Redirect(returnUrl);
        //            }
        //            return RedirectToAction("Index", "Home");
        //        }
        //        ModelState.AddModelError("", "The user name or password provided is incorrect.");
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //
        // GET: /Account/LogOff

        public int isWindowsAccount(string username, string password)
        {
            _usersContext = new ITHoTram_CustomReportEntities();
            
            var res = _usersContext.CasinoKioskUsers.SingleOrDefault(x => x.CasinoKiosk_UserName == username);
            var ur = new CasinoKioskUserRole();

            if (res == null)
            {
                return 0;
            }
            else if (res.isActive == true)
            {

                using (DirectoryEntry entry = new DirectoryEntry())
                {

                    entry.Username = username;
                    entry.Password = password;

                    DirectorySearcher searcher = new DirectorySearcher(entry);

                    searcher.Filter = "(objectclass=user)";
                    try
                    {
                        SearchResult sr = searcher.FindOne();
                        if (sr != null)
                        {                           

                            return 1;                        
                       }
                        else
                            return 0;

                    }
                    catch (COMException ex)
                    {
                        return -2;
                    }
                }

            }
            else
                return -1;
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("LogIn", "Account");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                var createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    return RedirectToAction("LogOn", "Account");
                }
                ModelState.AddModelError("", ErrorCodeToString(createStatus));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        // ChangePassword method not implemented in CustomMembershipProvider.cs
        // Feel free to update!

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        private CasinoKioskUser SetupFormsAuthTicket(string userName, bool persistanceFlag)
        {
            CasinoKioskUser user;
            userDao = new UserDao();

            //var userContext = new CustomMembershipWithRolesEntities();        
            user = userDao.GetUser(userName);
            
            var userId = user.UserID;
            var userData = userId.ToString(CultureInfo.InvariantCulture);
            var authTicket = new FormsAuthenticationTicket(1, //version
                                                        userName, // user name
                                                        DateTime.Now,             //creation
                                                        DateTime.Now.AddMinutes(120), //Expiration
                                                        persistanceFlag, //Persistent
                                                        userData);

            var encTicket = FormsAuthentication.Encrypt(authTicket);
            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
            return user;
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
