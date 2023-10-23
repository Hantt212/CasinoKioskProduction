using CKDatabaseConnection.DAO;
using CKDatabaseConnection.EF;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CasinoKiosk.Controllers
{
    public class ManageController : Controller
    {
        // GET: Manage
       
            // GET: Manage
            private readonly ITHoTram_CustomReportEntities _usersContext;
            private readonly UserDao userDao;

        public ManageController()
            {
                _usersContext = new ITHoTram_CustomReportEntities();
            }

            [Authorize(Roles = "SuperAdmin")]
            public ActionResult Index()
            {
                return View(_usersContext.CasinoKioskUsers.Where(u => u.UserID > 1).ToList());
            }

            [Authorize(Roles = "SuperAdmin")]
            public JsonResult UpdateRole(int userId, short roleId)
            {
               userDao.AddUserRole(new CasinoKioskUserRole { UserId = userId, RoleId = roleId });
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        public static class ListProvider
        {
            public static List<SelectListItem> Roles = new List<SelectListItem>
                                                   {
                                                       new SelectListItem { Text = "Super Admin", Value = "0" },
                                                       new SelectListItem { Text = "ITAdmin", Value = "1" },
                                                       new SelectListItem { Text = "CasinoAudit", Value = "2" },
                                                       new SelectListItem { Text = "HTRAdmin", Value = "3" },
                                                       new SelectListItem { Text = "HTRStaff", Value = "4" },
                                                   };

            public static List<SelectListItem> GetRoles(short roleId)
            {
                Roles.ForEach(r => r.Selected = false);
                var role = Roles.Single(r => r.Value == roleId.ToString(CultureInfo.InvariantCulture));
                role.Selected = true;
                return Roles;
            }
        
    }
}