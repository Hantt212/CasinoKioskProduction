using CKDatabaseConnection.DAO;
using CKDatabaseConnection.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CasinoKiosk.Controllers
{
    public class ActivityTrackingController : Controller
    {
        ITHoTram_CustomReportEntities entity = new ITHoTram_CustomReportEntities();
        UserDao dao = new UserDao();
        // GET: ActivityTracking
        public ActionResult Index()
        {            
            return View();
        }
        [Authorize(Roles = "SuperAdmin, PDTeam")]
        public JsonResult listActivity()
        {
            return Json(dao.getListActivityByUserName(), JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "SuperAdmin, PDTeam")]
        public JsonResult Add(CasinoTrackingActivity ac)
        {
            return Json(dao.InsertActivity(ac), JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "SuperAdmin, PDTeam")]
        public JsonResult GetbyID(int ID)
        {
            var user = dao.GetActivityByID(ID);
            return Json(user, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "SuperAdmin, PDTeam")]
        public JsonResult Update(CasinoTrackingActivity ac)
        {
            return Json(dao.UpdateActivity(ac), JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "SuperAdmin, PDTeam")]
        public JsonResult GetNameByPID(string PID)
        {
            var playerName = "";

            if (PID != null)
            {
                if (PID.Trim() != "")
                {
                    playerName = dao.GetPlayerNameByPID(Int32.Parse(PID.Trim()));                  
                }               
            }
            return Json(playerName, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "SuperAdmin, PDTeam")]
        public ActionResult Report(int page = 1)
        {
            var dao = new LogDao();
            this.Session["page"] = page;
            var model1 = dao.ListAllPagingActivity(page, 50);
            return View(model1);
        }

        [Authorize(Roles = "SuperAdmin, PDTeam")]
        public ActionResult ReportByDate()
        {
            var dao = new LogDao();
            IEnumerable<CasinoTrackingActivity> model = null;


            if (HttpContext.Request.Form["dfromdate"] != "" && HttpContext.Request.Form["dtodate"] != "")
            {
                var fdate = Convert.ToString(HttpContext.Request.Form["dfromdate"]);
                var tdate = Convert.ToString(HttpContext.Request.Form["dtodate"]);
                model = dao.ListAllActivityLog(fdate, tdate);
            }
            else
            {
                model = dao.ListAllActivityLog("", "").Take(100);
            }

            return View("Report", model);

        }
    }
}