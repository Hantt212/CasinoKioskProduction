using CKDatabaseConnection.DAO;
using CKDatabaseConnection.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CasinoKiosk.Controllers
{
    public class HTRTicketPromotionController : Controller
    {
        ITHoTram_CustomReportEntities entity = new ITHoTram_CustomReportEntities();
        UserDao dao = new UserDao();
        // GET: ActivityTracking
        public ActionResult Index()
        {
            return View();
            //return Redirect("TicketPromotion.aspx");
        }
        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public JsonResult listTicketPromotion()
        {
            return Json(dao.getListPromotion(), JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public JsonResult Add(HTRTicketPromotion ticket)
        {
            return Json(dao.InsertTicketPromotion(ticket), JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public JsonResult Void(int ID)
        {
            return Json(dao.TicketVoid(ID), JsonRequestBehavior.AllowGet);
        }

        //[Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        //public JsonResult Reprint(int ID)
        //{
        //    return Json(dao.TicketReprint(ID), JsonRequestBehavior.AllowGet);
        //}

        //Add 20230301 Hantt start
        [Authorize(Roles = "SuperAdmin, HTRAdmin")]
        public ActionResult MarketAuth()
        {
            var dao = new PlayerDao();
            var list = dao.ListAllPagingMarketAuth();
            list = list.GroupBy(item => item.Authorizer).Select(s => new SpReport_MarketingAuthorizer_Result
            {
                Authorizer = s.First().Authorizer,
                Status = s.First().Status,
                PlayerID = s.Count()
            }).ToList();
            return View(list);
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin")]
        public JsonResult  DetailMarket(string authorizer)
        {
            var dao = new PlayerDao();
            List<SpReport_MarketingAuthorizer_Result> list = new List<SpReport_MarketingAuthorizer_Result>();
            if ("All" == authorizer)
            {
                list = dao.ListAllPagingMarketAuth().ToList();
            }
            else
            {
                list = dao.ListAllPagingMarketAuth().Where(item => item.Authorizer == authorizer).ToList();
            }
              

            return Json(list, JsonRequestBehavior.AllowGet);

        }
        //Add 20230301 Hantt end

        //Add 20230413 Hantt start

        [Authorize(Roles = "SuperAdmin, HTRAdmin,HTRStaff")]
        public ActionResult GoldenHour()
        {
            var dao = new LogDao();

            spHTR_ThursdayGoldenHourDraw_Result patronInfo = new spHTR_ThursdayGoldenHourDraw_Result();
            if (Request["PatronID"] != null)
            {

                int patronID = Int32.Parse(Request["PatronID"]);
                patronInfo = dao.getPatronThursdayGolderHour(patronID);
            }

            List<HTR_ThursdayGoldenHourPromotion> patronList = dao.getLogThursdayGolderHour();


            ViewBag.patronInfo = patronInfo;
            ViewBag.patronList = patronList;
            return View();
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin,HTRStaff")]
        public ActionResult PrintGoldenHour(int PlayerID)
        {
            string url = "";
            var dao = new UserDao();
            var ID = dao.TicketGoldenHourPrint(PlayerID);
            if (ID != 0) {
                url = "~/Assets/Reports/GoldenHourTicket.aspx?id=" + ID;
                return Redirect(url);
            }
            return Redirect(url);


        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin,HTRStaff")]
        public ActionResult ReprintGoldenHour(int ID)
        {
            var dao = new UserDao();
            dao.TicketGoldenHourRePrint(ID);
            string url = "~/Assets/Reports/GoldenHourTicket.aspx?id=" + ID;
            return Redirect(url);
        }
        //Add 20230413 Hantt end

        // Sunday Promotion start
        [Authorize(Roles = "SuperAdmin, HTRAdmin,HTRStaff")]
        public ActionResult SundayPromotion()
        {
            var dao = new LogDao();

            spHTR_SundayGoldenHourDraw_Result patronInfo = new spHTR_SundayGoldenHourDraw_Result();
            if (Request["PatronID"] != null)
            {

                int patronID = Int32.Parse(Request["PatronID"]);
                patronInfo = dao.getPatronSundayGolderHour(patronID);

            }

            List<HTR_SundayGoldenHourLog> patronList = dao.getSundayGolderHourLog();


            ViewBag.patronInfo = patronInfo;
            ViewBag.patronList = patronList;
            return View();
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin,HTRStaff")]
        public ActionResult PrintSundayPromotion(int PlayerID)
        {
            string url = "";
            var dao = new UserDao();
            var ID = dao.PrintSundayPromotion(PlayerID);
            if (ID != 0)
            {
                url = "~/Assets/Reports/SundayPromotion.aspx?id=" + ID;
                return Redirect(url);
            }
            return Redirect(url);
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin,HTRStaff")]
        public ActionResult ReprintSundayPromotion(int ID)
        {
            var dao = new UserDao();
            dao.ReprintSundayPromotion(ID);
            string url = "~/Assets/Reports/SundayPromotion.aspx?id=" + ID;
            return Redirect(url);
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin")]
        public JsonResult VoidSundayPromotion(int ID)
        {
            return Json(dao.VoidSundayPromotion(ID), JsonRequestBehavior.AllowGet);
        }
        // Sunday Promotion end


        //MidAutume Promotion start
        [Authorize(Roles = "SuperAdmin, HTRAdmin,HTRStaff")]
        public ActionResult MidAutumePromotion()
        {
            var dao = new LogDao();

            spHTR_MidAutumeBySearchPatron_Result patronInfo = new spHTR_MidAutumeBySearchPatron_Result();
            if (Request["PatronID"] != null)
            {

                int patronID = Int32.Parse(Request["PatronID"]);
                patronInfo = dao.getPatronMidAutume(patronID);

            }

            List<HTR_MidAutumeLog> patronList = dao.getMidAutumeLog();
            
            ViewBag.patronInfo = patronInfo;
            ViewBag.patronList = patronList;
            return View();
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin,HTRStaff")]
        public ActionResult PrintMidAutumePromotion(int PlayerID)
        {
            string url = "";
            var dao = new LogDao();
            var ID = dao.PrintMidAutumePromotion(PlayerID);
            if (ID != 0)
            {
                url = "~/Assets/Reports/MidAutumePromotion.aspx?id=" + ID;
                return Redirect(url);
            }
            return Redirect(url);
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin,HTRStaff")]
        public ActionResult ReprintMidAutumePromotion(int ID)
        {
            var dao = new LogDao();
            dao.ReprintMidAutumePromotion(ID);
            string url = "~/Assets/Reports/MidAutumePromotion.aspx?id=" + ID;
            return Redirect(url);
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin")]
        public JsonResult VoidMidAutumePromotion(int ID)
        {
            var dao = new LogDao();
            return Json(dao.VoidMidAutumePromotion(ID), JsonRequestBehavior.AllowGet);
        }

        
        //MidAutume Promotion end

        //Add 20230809 Hantt start
        [Authorize(Roles = "SuperAdmin, HTRAdmin,HTRStaff")]
        public ActionResult MiniBuffet()
        {
            var dao = new LogDao();
            MiniBuffet_GetNewClassicPlayer_Result patronInfo = new MiniBuffet_GetNewClassicPlayer_Result();
            List<MiniBuffet_GetNewClassicPlayerLogs> patronList = dao.getMiniBuffetLogs();
            if (Request["PatronID"] != null)
            {
                patronInfo = dao.getMiniBuffet_GetNewClassicPlayer(Int32.Parse(Request["PatronID"]));
                if (patronInfo != null)
                {
                    //MiniBuffet_GetNewClassicPlayerLogs log  = dao.getMiniBuffetLogs()
                    //                                               .Where(item => item.PlayerID == patronInfo.PlayerID && item.PrintedDate.ToString().Contains(DateTime.Now.ToShortDateString()))
                    //                                               .OrderByDescending(item => item.ID)
                    //                                               .FirstOrDefault();

                    //if (log == null || log.isVoided == true)
                    //{
                    //    patronInfo.isPrint = 1;
                    //}

                    bool flgDisplayBtnPrint = dao.getMiniBuffetLogs().Exists(item => item.isPrinted == true && item.PlayerID == patronInfo.PlayerID);
                    if (flgDisplayBtnPrint == false)
                    {
                        patronInfo.isPrint = 1;
                    }
                }

            }
            

            ViewBag.patronInfo = patronInfo;
            ViewBag.patronList = patronList;
            return View();
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin,HTRStaff")]
        public ActionResult MiniBuffetPrint(int PlayerID, string PlayerName)
        {
            string url = "";
            var dao = new UserDao();
            var ID = dao.MiniBuffetPrint(PlayerID, PlayerName);
            if (ID != 0)
            {
                url = "~/Assets/Reports/MiniBuffetTicket.aspx?id=" + ID;
                return Redirect(url);
            }
            return Redirect(url);


        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin")]
        public ActionResult ReprintMiniBuffet(int ID)
        {
            var dao = new UserDao();
            dao.MiniBuffetRePrint(ID);
            string url = "~/Assets/Reports/MiniBuffetTicket.aspx?id=" + ID;
            return Redirect(url);
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin")]
        public JsonResult VoidMiniBuffet(int ID)
        {
            return Json(dao.MiniBuffetVoid(ID), JsonRequestBehavior.AllowGet);
        }
        //Add 20230809 Hantt end


        public JsonResult VoidGoldenHour(int ID)
        {
            // return Json(dao.TicketGoldenHourVoid(ID), JsonRequestBehavior.AllowGet);
            return Json(dao.TicketGoldenHourVoid(ID), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Reprint(int ID)
        {
            var result = dao.TicketReprint(ID);
            return RedirectToAction("~/TicketPromotion.aspx");
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public JsonResult GetbyID(int ID)
        {
            var user = dao.GetActivityByID(ID);
            return Json(user, JsonRequestBehavior.AllowGet);
        }      
        
        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
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

        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public ActionResult Report(int page = 1)
        {
            var dao = new LogDao();
            this.Session["page"] = page;
            var model1 = dao.ListAllPagingActivity(page, 50);
            return View(model1);
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
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