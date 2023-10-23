using CKDatabaseConnection.DAO;
using CKDatabaseConnection.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CasinoKiosk.Controllers
{
    public class FOPatronController : Controller
    {
        UserDao dao = new UserDao();
        // GET: FOPatron

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public JsonResult selectListFOPatron()
        {
            return Json(dao.ListAllFOPatronUser(), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public JsonResult GetbyID(int ID)
        {        
            var user = dao.ListAllFOPatronUser().Find(x => x.ID.Equals(ID));
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public JsonResult Add(FOPatron_SelectAll_Result user)
        {
            return Json(dao.InsertFOPatron(user), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public JsonResult Update(FOPatron_SelectAll_Result user)
        {
            return Json(dao.UpdateFOPatron(user), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public JsonResult Delete(int ID)
        {
            return Json(dao.DeleteFOPatron(ID), JsonRequestBehavior.AllowGet);
        }
    }
}