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
        PlayerDao dao = new PlayerDao();
        // GET: PlayerCard
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult RegisterPlayer(string fcardId, string patronId,string passportId, bool isVisitor)
        {
            var dao = new PlayerDao();
            long exist = dao.InsertFCardID(fcardId, patronId, passportId, isVisitor);
            return Json(exist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getInitData(string query)
        {
            var dao = new PlayerDao();
            var fcardList = (from item in dao.getFCardInfoList(query)
                             select new
                             {
                                 ID = item.ID,
                                 FCardID = item.FCardID,
                                 PID = item.PID,
                                 PassportID = item.PassportID,
                                 IsActive = item.IsActive,
                                 IsVisitor = item.Remark,
                                 DateInserted = item.DateInserted.ToString(),
                                 DateUpdated = item.DateUpdated.ToString(),
                                 UpdatedBy = item.UpdatedBy
                             });

           
           
            return new JsonResult() { Data = fcardList, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }


        public JsonResult GetCardInfoByID(int Id)
        {
            var dao = new PlayerDao();
            var result = dao.getFCardID(Id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateCardInfoByID(int Id, string fCardID, string passport, bool isVisitor)
        {
            var dao = new PlayerDao();
            //Mode == 2 is Edit
            return Json(dao.updateFCardID(Id, fCardID, passport, isVisitor, true, 2), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DelCardInfoByID(int Id)
        {
            var dao = new PlayerDao();
            //Mode == 3 is Delete
            return Json(dao.updateFCardID(Id, null, null, false, false, 3), JsonRequestBehavior.AllowGet);
        }


        public ActionResult CasinoDoorLog()
        {
            var result = dao.getCasinoDoorLog("");
            return View(result);
        }

    }
}