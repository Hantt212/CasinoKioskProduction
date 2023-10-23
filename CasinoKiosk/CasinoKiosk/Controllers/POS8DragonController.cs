using CKDatabaseConnection.DAO;
using CKDatabaseConnection.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CasinoKiosk.Controllers
{
    public class POS8DragonController : Controller
    {
        // GET: POS8Dragon
        UserDao dao = new UserDao();

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public JsonResult selectListPOS8DragonUser()
        {
            return Json(dao.ListAllPOS8DragonUser(), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public JsonResult GetbyID(int ID)
        {
            var user = dao.ListAllPOS8DragonUser().Find(x => x.ID.Equals(ID));
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public JsonResult Add(POSPatronInfoUser user)
        {
            return Json(dao.InsertPOS8DragonUser(user), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public JsonResult Update(POSPatronInfoUser user)
        {
            return Json(dao.UpdatePOS8DragonUser(user), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public JsonResult Delete(int ID)
        {
            return Json(dao.DeletePOS8DragonUser(ID), JsonRequestBehavior.AllowGet);
        }
    }
}