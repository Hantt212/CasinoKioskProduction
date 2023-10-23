using CasinoKiosk.Common;
using CKDatabaseConnection.Common;
using CKDatabaseConnection.DAO;
using CKDatabaseConnection.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CasinoKiosk.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users
        UserDao dao = new UserDao();


        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public JsonResult selectList()
        {
            var RoleID = Convert.ToInt32(Session["RoleID"]);
            return Json(dao.ListAllRoles(RoleID), JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public JsonResult List()
        {
            var RoleID = Convert.ToInt32(Session["RoleID"]);
            return Json(dao.ListAllCKUsers(RoleID), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public JsonResult GetbyID(int ID)
        {
            var RoleID = Convert.ToInt32(Session["RoleID"]);
            var user = dao.ListAllCKUsers(RoleID).Find(x => x.UserID.Equals(ID));
            return Json(user, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public JsonResult Add(CasinoKioskUsers_SelectAll_Result user)
        {
            return Json(dao.Insert(user), JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public JsonResult Update(CasinoKioskUsers_SelectAll_Result user)
        {
            return Json(dao.Update(user), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public JsonResult Delete(int ID)
        {
            return Json(dao.Delete(ID), JsonRequestBehavior.AllowGet);
        }
    }
}