using CKDatabaseConnection.DAO;
using CKDatabaseConnection.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CasinoKiosk.Controllers
{
    public class FCController : Controller
    {
        // GET: FC
        UserDao dao = new UserDao();        

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public JsonResult selectListCCUser()
        {
            return Json(dao.ListAllCCUser(), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public JsonResult GetbyID(int ID)
        {
            var user = dao.ListAllCCUser().Find(x => x.ID.Equals(ID));
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public JsonResult Add(CurrencyConverter_User user)
        {
            return Json(dao.InsertCCUser(user), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public JsonResult Update(CurrencyConverter_User user)
        {
            return Json(dao.UpdateCCUser(user), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public JsonResult Delete(int ID)
        {
            return Json(dao.DeleteCCUser(ID), JsonRequestBehavior.AllowGet);
        }
    }
}