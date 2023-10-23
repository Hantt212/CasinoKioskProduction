using CKDatabaseConnection.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CasinoKiosk.Controllers
{
    public class PlayerCardController : Controller
    {

        // GET: PlayerCard
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult RegisterPlayer(int fcardId, string patronId, bool isVisitor)
        {
            var dao = new PlayerDao();
            long exist = dao.InsertFCardID(fcardId.ToString(), patronId, isVisitor);
            return Json(exist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getInitData()
        {
            var dao = new PlayerDao();
            var fcardList = (from item in dao.getFCardInfoList()
                             select new
                             {
                                 ID = item.ID,
                                 FCardID = item.FCardID,
                                 PID = item.PID,
                                 IsActive = item.IsActive,
                                 IsVisitor = item.Remark,
                                 DateInserted = item.DateInserted.ToString(),
                                 DateUpdated = item.DateUpdated.ToString(),
                                 UpdatedBy = item.UpdatedBy
                             });
            return Json(fcardList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCardInfoByID(int Id)
        {
            var dao = new PlayerDao();
            var result = dao.getFCardID(Id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateCardInfoByID(int Id, string fCardID, bool isVisitor)
        {
            var dao = new PlayerDao();
            //Mode == 2 is Edit
            return Json(dao.updateFCardID(Id, fCardID, isVisitor, true, 2), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DelCardInfoByID(int Id)
        {
            var dao = new PlayerDao();
            //Mode == 3 is Delete
            return Json(dao.updateFCardID(Id, null, false, false, 3), JsonRequestBehavior.AllowGet);
        }

    }
}