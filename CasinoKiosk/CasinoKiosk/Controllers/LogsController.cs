using CKDatabaseConnection.DAO;
using CKDatabaseConnection.EF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CasinoKiosk.Areas.Admin.Controllers
{
    public class LogsController : Controller
    {
        // GET: Admin/Logs
        string connectionString = ConfigurationManager.ConnectionStrings["CKdbContext"].ConnectionString;
        CKFunction function = new CKFunction();
        CKdbContext db = new CKdbContext();
        ITHoTram_CustomReportEntities entity = new ITHoTram_CustomReportEntities();

        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public ActionResult Index(int page = 1, int pageSize = 500)
        {
            var dao = new LogDao();
            var model = dao.ListAllPaging(page, pageSize);
            return View(model);
        }
        //add by Doc Ly 2021/06/01

        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public ActionResult ItemByDate()
        {
            var dao = new LogDao();
            IEnumerable<CasinoKiosk_Log> model = null;
            if (HttpContext.Request.Form["ID"] != "")
            {
                var ID = Convert.ToInt32(HttpContext.Request.Form["ID"].ToString());
                model = dao.ListAllByDate("", "", ID);
            }
            else
            if (HttpContext.Request.Form["dfromdate"] != "" && HttpContext.Request.Form["dtodate"] != "")
            {
                var fdate = Convert.ToString(HttpContext.Request.Form["dfromdate"]);
                var tdate = Convert.ToString(HttpContext.Request.Form["dtodate"]);
                model = dao.ListAllByDate(fdate, tdate, 0);
            }
            else
            {
                model = dao.ListAllByDate("", "", 0).Take(100);
            }
            return View("index", model);
        }
        //End

        [Authorize(Roles = "SuperAdmin, HTRAdmin")]
        public ActionResult Void(int id)
        {
            var dao = new LogDao();
            var log = new CasinoKiosk_Log();

            log = dao.GetLogByID(id);
            string s = function.getLogVoidedStatusByID(id).ToString();
            string s1 = Session["userName"].ToString();
            string s2 = function.getVoidedPersonByID(id);

            if (s == "printed")
            {
                function.updateVoidedStatusByID(id, "voided");
                function.updateVoidedPersonByID(id, Session["userName"].ToString());

                if (s2 == "")
                {
                    function.returnVoidedPoints((int)log.PlayerID, (int)log.ItemPoints);
                }
            }

            //var model = dao.ListAllPaging(1,50);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin")]
        public ActionResult Reprint(int id)
        {
            var dao = new LogDao();
            var log = new CasinoKiosk_Log();

            string s1 = Session["userName"].ToString();

            function.updateReprintedPersonByID(id, Session["userName"].ToString());

            //var model = dao.ListAllPaging(1,50);
            return RedirectToAction("~/Tickets.aspx");
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public ActionResult DailyLog(int page = 1)
        {
            var dao = new LogDao();
            //this.Session["page"] = page;
            var model = dao.ListAllPagingDailyLog(page, 100, 0);
            return View(model);
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        [HttpPost]
        public ActionResult DailyLog(int ID, int page = 1)
        {
            if (HttpContext.Request.Form["ID"] != null)
            {
                ID = Convert.ToInt32(HttpContext.Request.Form["ID"]);

                var dao = new LogDao();
                //this.Session["page"] = page;
                var model = dao.ListAllPagingDailyLog(page, 50, ID);
                return View(model);
            }
            else
            {
                var dao = new LogDao();
                this.Session["page"] = page;
                var model1 = dao.ListAllPagingDailyLog(page, 50, 0);
                return View(model1);
            }

        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin")]
        public ActionResult DailyVoid(int id)
        {
            var dao = new LogDao();
            var log = new MFDailyBonus_SecondLogs();

            log = entity.MFDailyBonus_SecondLogs.FirstOrDefault(x => x.ID == id);

            string s1 = Session["userName"].ToString();

            if (log.voidedStatus == "printed") {
                log.voidedStatus = "voided";
                
                if (log.voidedPerson == null) {

                    DateTime currentDate = function.getMFDBCurrentDay();

                    //DateTime logGamingDate = DateTime.Parse((log.GamingDate));
                    //Change 20230109 Hantt start
                    //DateTime logGamingDate = DateTime.ParseExact(log.GamingDate, @"dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    DateTime logGamingDate = DateTime.ParseExact(log.GamingDate, "dd/MM/yyyy", CultureInfo.CurrentCulture);
                    //Change 20230109 Hantt end

                    DateTime yesterday = currentDate.AddDays(-1);

                    if (currentDate == logGamingDate)
                    {
                        function.updateItemDailyStatusSecondNew((int)log.PlayerID, (int)log.ItemPoints, 1, "T");
                        //log.voidedPerson = s1;
                        //entity.SaveChanges();
                    }
                    else if(logGamingDate == yesterday) {
                        function.updateItemDailyStatusSecondNew((int)log.PlayerID, (int)log.ItemPoints, 1, "Y");
                        //log.voidedPerson = s1;
                        //entity.SaveChanges();
                    }
                    //Add 20230109 Hantt start
                    log.voidedTime = DateTime.Now;
                    //Add 20230109 Hantt end
                    log.voidedPerson = s1;
                    entity.SaveChanges();
                }
            }             
            

            //var model = dao.ListAllPagingDailyLog(1, 50, id);
            return RedirectToAction("DailyLog");

        }

        //add by Doc Ly 2021/05/28 
        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        [HttpPost]
        public ActionResult DailyLogByDate()
        {
            var dao = new LogDao();
            IEnumerable<MFBonus_spSelectDailyLogs_Result> model = null;
            if (HttpContext.Request.Form["ID"] != "")
            {
                var ID = Convert.ToInt32(HttpContext.Request.Form["ID"].ToString());
                model = dao.ListAllDailyLog("", "", ID);
            }
            else
            if (HttpContext.Request.Form["dfromdate"] != "" && HttpContext.Request.Form["dtodate"] != "")
            {
                var fdate = Convert.ToString(HttpContext.Request.Form["dfromdate"]);
                var tdate = Convert.ToString(HttpContext.Request.Form["dtodate"]);
                model = dao.ListAllDailyLog(fdate, tdate, 0);
            }
            else
            {
                model = dao.ListAllDailyLog("", "", 0).Take(100);
            }

            return View("DailyLog", model);

        }
        //End

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        [HttpPost]
        public ActionResult FOPatronUserActionLogByDate()
        {
            var dao = new LogDao();
            IEnumerable<FOPatronUserActionLog_SelectAll_Result> model = null;
            
           
            if (HttpContext.Request.Form["dfromdate"] != "" && HttpContext.Request.Form["dtodate"] != "")
            {
                var fdate = Convert.ToString(HttpContext.Request.Form["dfromdate"]);
                var tdate = Convert.ToString(HttpContext.Request.Form["dtodate"]);
                model = dao.ListAllFOPatronUserLog(fdate, tdate);
            }
            else
            {
                model = dao.ListAllFOPatronUserLog("", "").Take(100);
            }

            return View("FOPatronUserActionLog", model);

        }

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        [HttpPost]
        public ActionResult POS8DragonUserActionLogByDate()
        {
            var dao = new LogDao();
            IEnumerable<POSPatron8DragonUserActionLog> model = null;


            if (HttpContext.Request.Form["dfromdate"] != "" && HttpContext.Request.Form["dtodate"] != "")
            {
                var fdate = Convert.ToString(HttpContext.Request.Form["dfromdate"]);
                var tdate = Convert.ToString(HttpContext.Request.Form["dtodate"]);
                model = dao.ListAllPOS8DragonUserLog(fdate, tdate);
            }
            else
            {
                model = dao.ListAllPOS8DragonUserLog("", "").Take(100);
            }

            return View("POS8DragonUserActionLog", model);

        }

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        [HttpPost]
        public ActionResult CCUserActionLogByDate()
        {
            var dao = new LogDao();
            IEnumerable<CurrencyConverterUserActionLog> model = null;


            if (HttpContext.Request.Form["dfromdate"] != "" && HttpContext.Request.Form["dtodate"] != "")
            {
                var fdate = Convert.ToString(HttpContext.Request.Form["dfromdate"]);
                var tdate = Convert.ToString(HttpContext.Request.Form["dtodate"]);
                model = dao.ListAllCCUserLog(fdate, tdate);
            }
            else
            {
                model = dao.ListAllCCUserLog("", "").Take(100);
            }

            return View("CurrencyConverterUserActionLog", model);

        }

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public ActionResult FOPatronUserActionLog(int page = 1)
        {      
                var dao = new LogDao();
                this.Session["page"] = page;
                var model1 = dao.ListAllPagingFOPatronUserLog(page, 50);
                return View(model1);
        }

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public ActionResult CCUserActionLog(int page = 1)
        {
            var dao = new LogDao();
            this.Session["page"] = page;
            var model1 = dao.ListAllPagingCCUserLog(page, 50);
            return View(model1);
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public ActionResult PSBundleLog(int page = 1)
        {
            var dao = new LogDao();
            this.Session["page"] = page;
            var model1 = dao.ListAllPagingPSBundleLog(page, 50);
            return View(model1);
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public ActionResult PSBundleLogByDate()
        {
            var dao = new LogDao();
            IEnumerable<DailyPSBundle_Logs> model = null;


            if (HttpContext.Request.Form["dfromdate"] != "" && HttpContext.Request.Form["dtodate"] != "")
            {
                var fdate = Convert.ToString(HttpContext.Request.Form["dfromdate"]);
                var tdate = Convert.ToString(HttpContext.Request.Form["dtodate"]);
                model = dao.ListAllPSBundleLog(fdate, tdate);
            }
            else
            {
                model = dao.ListAllPSBundleLog("", "").Take(100);
            }

            return View("DailyPSBundleLogs", model);

        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin")]
        public ActionResult PSBundleVoid(int id)
        {
            var dao = new LogDao();
            var log = new DailyPSBundle_Logs();
            var item = new DailyPSBundle_Items();
            List<DailyPSBundle_Items> items = new List<DailyPSBundle_Items>();

            log = entity.DailyPSBundle_Logs.FirstOrDefault(x => x.ID == id);
            item = entity.DailyPSBundle_Items.FirstOrDefault(i => i.ID == log.ItemID);
            items = entity.DailyPSBundle_Items.Where(i => i.PlayerID == log.PlayerID).ToList();

            string s1 = Session["userName"].ToString();

            if (log.voidedStatus == "printed")
            {
                log.voidedStatus = "voided";

                if (log.voidedPerson == null)
                {
                    DateTime currentDate = function.getMFDBCurrentDay();

                    //DateTime logGamingDate = DateTime.Parse((log.GamingDate));
                    DateTime logGamingDate = DateTime.ParseExact(log.GamingDate, @"dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                    if (logGamingDate == currentDate)
                    {                       
                        for (int j = 0; j < items.Count; j++) {
                            items[j].Status = 1;
                        }
                    }
                   
                    log.voidedPerson = s1;
                }
            }
            entity.SaveChanges();

            //var model = dao.ListAllPagingDailyLog(1, 50, id);
            return RedirectToAction("PSBundleLog");

        }

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public ActionResult POS8DragonUserActionLog(int page = 1)
        {
            var dao = new LogDao();
            this.Session["page"] = page;
            var model1 = dao.ListAllPagingPOS8DragonUserLog(page, 50);
            return View(model1);
        }

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        public ActionResult KioskUserActionLog(int page = 1)
        {
            var dao = new LogDao();
            this.Session["page"] = page;
            var model1 = dao.ListAllPagingKioskUserLog(page, 50);
            return View(model1);
        }

        [Authorize(Roles = "SuperAdmin, ITAdmin")]
        [HttpPost]
        public ActionResult KioskUserActionLogByDate()
        {
            var dao = new LogDao();
            IEnumerable<KioskUserActionLog_SelectAll_Result> model = null;


            if (HttpContext.Request.Form["dfromdate"] != "" && HttpContext.Request.Form["dtodate"] != "")
            {
                var fdate = Convert.ToString(HttpContext.Request.Form["dfromdate"]);
                var tdate = Convert.ToString(HttpContext.Request.Form["dtodate"]);
                model = dao.ListAllKioskUserLog(fdate, tdate);
            }
            else
            {
                model = dao.ListAllKioskUserLog("", "").Take(100);
            }

            return View("FOPatronUserActionLog", model);

        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public ActionResult WeeklyLog(int page = 1)
        {
            var dao = new LogDao();
            var model = dao.ListAllPagingWeeklyLog(page, 50, 0);
            return View(model);
        }

        //add by Doc Ly 2021/05/28 
        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        [HttpPost]
        public ActionResult WeeklyLogByDate()
        {
            var dao = new LogDao();
            IEnumerable<MFBonus_spSelectWeeklyLogs_Result> model = null;
            if (HttpContext.Request.Form["ID"] != "")
            {
                var ID = Convert.ToInt32(HttpContext.Request.Form["ID"].ToString());
                model = dao.ListAllWeeklyLog("", "", ID);
            }
            else
            if (HttpContext.Request.Form["dfromdate"] != "" && HttpContext.Request.Form["dtodate"] != "")
            {
                var fdate = Convert.ToString(HttpContext.Request.Form["dfromdate"]);
                var tdate = Convert.ToString(HttpContext.Request.Form["dtodate"]);
                model = dao.ListAllWeeklyLog(fdate, tdate, 0);
            }
            else
            {
                model = dao.ListAllWeeklyLog("", "", 0).Take(100);
            }

            return View("WeeklyLog", model);

        }
        //End

        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        [HttpPost]
        public ActionResult WeeklyLog(int ID, int page = 1)
        {
            if (HttpContext.Request.Form["ID"] != null)
            {
                ID = Convert.ToInt32(HttpContext.Request.Form["ID"]);

                var dao = new LogDao();
                var model = dao.ListAllPagingWeeklyLog(page, 50, ID);
                return View(model);
            }
            else
            {
                var dao = new LogDao();
                var model1 = dao.ListAllPagingWeeklyLog(page, 50, 0);
                return View(model1);
            }

        }

        //8Dragon Buffet Log start
        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public ActionResult MF8DragonLog(int page = 1)
        {
            var dao = new LogDao();
            var model = dao.ListAllPagingMF8DragonLog(page, 50, 0);
            return View(model);
        }
        
        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        [HttpPost]
        public ActionResult MF8DragonLogByDate()
        {
            var dao = new LogDao();
            IEnumerable<MF8DragonBuffetBonus_Logs> model = null;
            if (HttpContext.Request.Form["ID"] != "")
            {
                var ID = Convert.ToInt32(HttpContext.Request.Form["ID"].ToString());
                model = dao.ListAllMF8DragonLog("", "", ID);
            }
            else
            if (HttpContext.Request.Form["dfromDate"] != "" && HttpContext.Request.Form["dtoDate"] != "")
            {
                var fdate = Convert.ToString(HttpContext.Request.Form["dfromDate"]);
                var tdate = Convert.ToString(HttpContext.Request.Form["dtoDate"]);
                model = dao.ListAllMF8DragonLog(fdate, tdate, 0);
            }
            else
            {
                model = dao.ListAllMF8DragonLog("", "", 0).Take(100);
            }

            return View("MF8DragonLog", model);

        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        [HttpPost]
        public ActionResult MF8DragonLog(int ID, int page = 1)
        {
            if (HttpContext.Request.Form["ID"] != null){
                ID = Convert.ToInt32(HttpContext.Request.Form["ID"]);
            } else {
                ID = 0;
            }

            var dao = new LogDao();
            var model = dao.ListAllPagingMF8DragonLog(page, 50, ID);
            return View(model);
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin")]
        public ActionResult MF8DragonVoid(int id)
        {
            var dao = new LogDao();
            var log = new MF8DragonBuffetBonus_Logs();
            MF8DragonBuffetBonus_Items item;

            log = entity.MF8DragonBuffetBonus_Logs.FirstOrDefault(x => x.ID == id);

            string s1 = Session["userName"].ToString();

            if (log.voidedStatus == "printed") {
                log.voidedStatus = "voided";
                if (log.voidedPerson == null) {
                    item = entity.MF8DragonBuffetBonus_Items.Find(log.ItemID);
                    item.Status = 1;
                    item.DateInserted = DateTime.Now;
                     
                    log.voidedPerson = s1;
                    log.voidedTime = DateTime.Now;
                    entity.SaveChanges();
                }
            }

            entity.SaveChanges();
            return RedirectToAction("MF8DragonLog");

        }
        //8Dragon Buffet Log end

        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public ActionResult FridayLog(int page = 1)
        {
            var dao = new LogDao();
            var model = dao.ListAllPagingFridayLog(page, 50, 0);
            return View(model);
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin")]
        public ActionResult WeeklyVoid(int id)
        {
            var dao = new LogDao();
            var log = new MFWeeklyBonus_Logs();

            log = entity.MFWeeklyBonus_Logs.FirstOrDefault(x => x.ID == id);

            string s1 = Session["userName"].ToString();

            if (log.voidedStatus == "printed")
            {
                log.voidedStatus = "voided";

                if (log.voidedPerson == null)
                {
                    function.updateItemWeeklyStatusFirst((int)log.PlayerID, 1, (int)log.ItemPoints);
                    log.voidedPerson = s1;
                    //Add 20230109 Hantt start
                    log.voidedTime = DateTime.Now;
                    //Add 20230109 Hantt end
                }
            }
            entity.SaveChanges();

            //var model = dao.ListAllPagingDailyLog(1, 50, id);
            return RedirectToAction("WeeklyLog");

        }

        //add by Doc Ly 2021/05/28 
        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        [HttpPost]
        public ActionResult FridayLogByDate()
        {
            var dao = new LogDao();
            IEnumerable<MFBonus_spSelectFridayLogs_Result> model = null;
            if (HttpContext.Request.Form["ID"] != "")
            {
                var ID = Convert.ToInt32(HttpContext.Request.Form["ID"].ToString());
                model = dao.ListAllFridayLog("", "", ID);
            }
            else
            if (HttpContext.Request.Form["dfromdate"] != "" && HttpContext.Request.Form["dtodate"] != "")
            {
                var fdate = Convert.ToString(HttpContext.Request.Form["dfromdate"]);
                var tdate = Convert.ToString(HttpContext.Request.Form["dtodate"]);
                model = dao.ListAllFridayLog(fdate, tdate, 0);
            }
            else
            {
                model = dao.ListAllFridayLog("", "", 0).Take(100);
            }

            return View("FridayLog", model);
        }
        //End

        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        [HttpPost]
        public ActionResult FridayLog(int ID, int page = 1)
        {
            if (HttpContext.Request.Form["ID"] != null)
            {
                ID = Convert.ToInt32(HttpContext.Request.Form["ID"]);

                var dao = new LogDao();
                var model = dao.ListAllPagingFridayLog(page, 50, ID);
                return View(model);
            }
            else
            {
                var dao = new LogDao();
                var model1 = dao.ListAllPagingFridayLog(page, 50, 0);
                return View(model1);
            }

        }

        //add by Doc Ly 2021/05/28 
        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        [HttpPost]
        public ActionResult RedemptionLogByDate()
        {
            var dao = new LogDao();
            IEnumerable<MFBonus_spSelectRedemptionLogs_Result> model = null;
            if (HttpContext.Request.Form["ID"] != "")
            {
                var ID = Convert.ToInt32(HttpContext.Request.Form["ID"].ToString());
                model = dao.ListAllRedemptionLog("", "", ID);
            }
            else
            if (HttpContext.Request.Form["dfromdate"] != "" && HttpContext.Request.Form["dtodate"] != "")
            {
                var fdate = Convert.ToString(HttpContext.Request.Form["dfromdate"]);
                var tdate = Convert.ToString(HttpContext.Request.Form["dtodate"]);
                model = dao.ListAllRedemptionLog(fdate, tdate, 0);
            }
            else
            {
                model = dao.ListAllRedemptionLog("", "", 0).Take(100);
            }

            return View("RedemptionLog", model);
        }
        //End

        [Authorize(Roles = "SuperAdmin, HTRAdmin")]
        public ActionResult FridayVoid(int id)
        {
            var dao = new LogDao();
            var log = new MFFridayBonus_Logs();

            log = entity.MFFridayBonus_Logs.FirstOrDefault(x => x.ID == id);

            string s1 = Session["userName"].ToString();

            if (log.voidedStatus == "printed")
            {
                log.voidedStatus = "voided";

                if (log.voidedPerson == null)
                {
                    function.updateItemFridayStatusFirst((int)log.PlayerID, 1, (int)log.ItemPoints);
                    log.voidedPerson = s1;
                }
            }
            entity.SaveChanges();

            //var model = dao.ListAllPagingDailyLog(1, 50, id);
            return RedirectToAction("FridayLog");

        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public ActionResult RedemptionLog(int page = 1)
        {
            var dao = new LogDao();
            var model = dao.ListAllPagingRedemptionLog(page, 500, 0);
            return View(model);
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        [HttpPost]
        public ActionResult RedemptionLog(int ID, int page = 1)
        {
            if (HttpContext.Request.Form["ID"] != null)
            {
                ID = Convert.ToInt32(HttpContext.Request.Form["ID"]);

                var dao = new LogDao();
                var model = dao.ListAllPagingRedemptionLog(page, 500, ID);
                return View(model);
            }
            else
            {
                var dao = new LogDao();
                var model1 = dao.ListAllPagingRedemptionLog(page, 500, 0);
                return View(model1);
            }

        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin")]
        public ActionResult RedemptionVoid(int id)
        {

            ITHoTram_CustomReportEntities entity = new ITHoTram_CustomReportEntities();
            MFPointsRedemption_Logs log = new MFPointsRedemption_Logs();

            log = entity.MFPointsRedemption_Logs.SingleOrDefault(x => x.ID == id);

            //Change 20230109 Hantt start
            //string s = function.getLogVoidedStatusByID(id).ToString();
            //string s1 = Session["userName"].ToString();
            //string s2 = function.getVoidedPersonByID(id);

            string s = function.getStatusRedemptionByID(id).ToString();
            string s1 = Session["userName"].ToString();
            string s2 = function.getVoidedPersonByIDMFPoint(id);
            log.voidedTime = DateTime.Now;
            //Change 20230109 Hantt end
            
            if (s == "printed")
            {
                function.updateRedeemtionVoidedStatusByID(id, "voided");
                function.updateRedeemtionVoidedPersonByID(id, Session["userName"].ToString());
                if (s2 == "")
                {
                    //Change 20230109 Hantt start
                    function.returnVoidedPoints((int)log.PlayerID, (int)log.ItemPoints);
                    //Change 20230109 Hantt end
                }
            }

            //var model = dao.ListAllPaging(1,50);
            return RedirectToAction("RedemptionLog");
        }

        //Add 20230727 Hantt start
        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public ActionResult DailyManual()
        {
            List<MFDailyBonus_Items_Yesterday> yesItemList = new List<MFDailyBonus_Items_Yesterday>();
            List<MFDailyBonus_Items_Yesterday> yesItemNoCheckList = new List<MFDailyBonus_Items_Yesterday>();
            List<MFDailyBonus_Items_Yesterday> list = new List<MFDailyBonus_Items_Yesterday>();
            ViewBag.PointsEarned = 0;
            ViewBag.PlayerID = "";

            if (Request["PlayerID"] != null)
            {
                 int PlayerID = Int32.Parse(Request["PlayerID"].ToString());

                int playerPointsYesterday = entity.MFDailyBonus_YesterdayItemsManual(PlayerID).First() ?? 0;

                //set 3600 points -> 3000
                int yesterdayPoints = function.getItemPointsDaily(playerPointsYesterday);//0

                yesItemList = db.MFDailyBonus_Items_Yesterdays.Where(item => item.PlayerID == PlayerID).ToList();
               
                for (int i =0; i<= 5; i++)
                {
                    if(yesItemList[i].Status == 0 && yesItemList[i].ItemPoints <= yesterdayPoints
                       && yesItemList[i + 6].Status == 0 && yesItemList[i + 6].ItemPoints <= yesterdayPoints)
                    {
                        yesItemList[i].Status = -1;

                        list.Add(yesItemList[i]);
                    }else
                    {
                        if(yesItemList[i].Status > yesItemList[i + 6].Status)
                        {
                            list.Add(yesItemList[i]);
                        }else
                        {
                            list.Add(yesItemList[i + 6]);
                        }
                    }
                }

                list = list.OrderBy(item => item.ItemPoints).ToList();


                ViewBag.PointsEarned = playerPointsYesterday;
                ViewBag.PlayerID = Request["PlayerID"].ToString();
            }

            return View(list);
        }


        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public JsonResult enableItemYes(int ItemPoints, int PlayerID)
        {
            List<MFDailyBonus_Items_Yesterday> yesterdayLst = db.MFDailyBonus_Items_Yesterdays.Where(item => item.ItemPoints == ItemPoints && item.PlayerID == PlayerID).ToList();

            yesterdayLst.ForEach(item =>
            {
                int playerPointsYesterday = entity.MFDailyBonus_YesterdayItemsManual(item.PlayerID).First() ?? 0;

                MFDailyBonus_YesterdayItemsManual_Log manualLog = new MFDailyBonus_YesterdayItemsManual_Log();
                manualLog.ItemID = item.ID;
                manualLog.ItemName = item.ItemName;
                manualLog.ItemPoints = item.ItemPoints;
                manualLog.PreStatus = item.Status ?? 0;
                manualLog.PlayerID = item.PlayerID;
                manualLog.PointsEarned = playerPointsYesterday;
                manualLog.CreateTime = DateTime.Now;
                manualLog.CreateBy = Session["userName"].ToString();
                db.MFDailyBonus_YesterdayItemsManual_Logs.Add(manualLog);

                item.Status = 1;
            });
            

            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        //Add 20230727 Hantt end
    }

}