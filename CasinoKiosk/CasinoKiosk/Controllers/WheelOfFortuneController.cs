using CKDatabaseConnection.DAO;
using CKDatabaseConnection.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CasinoKiosk.Controllers
{
    public class WheelOfFortuneController : Controller
    {
        PlayerDao dao = new PlayerDao();
        // GET: PlayerCard
        public ActionResult Index()
        {
            return View();
        }
        
        public JsonResult getInitData()
        {
            List<WoF_GetWheelOfFortuneLog_Result> logs = dao.getWheelOfFortuneLog();
            return new JsonResult() { Data = logs, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        public ActionResult PrizeInfo()
        {
            return View();
        }

        public JsonResult getInitPrizeInfo()
        {
            List<WoF_Prize> infos = dao.getPrizeInfo();
            return new JsonResult() { Data = infos, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        public JsonResult getPrizeInfoByID(int Id)
        {
            WoF_Prize prize = dao.getPrizeInfoByID(Id);
            return new JsonResult() { Data = prize, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        public JsonResult UpdatePrizeInfoByID(int Id, string PrizeName, int TotalQty, bool IsSelected)
        {
            return Json(dao.UpdatePrizeInfoByID(Id, PrizeName, TotalQty, IsSelected), JsonRequestBehavior.AllowGet);
        }

        public JsonResult updPosition(List<WoF_Prize> inputList)
        {
            return Json(dao.updPosition(inputList), JsonRequestBehavior.AllowGet);
        }
    }
}