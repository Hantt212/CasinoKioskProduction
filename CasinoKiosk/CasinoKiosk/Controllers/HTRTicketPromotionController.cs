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
        LogDao logDao = new LogDao();
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

        // Get All Promotion List
        [Authorize(Roles = "SuperAdmin, HTRAdmin,HTRStaff")]
        public ActionResult HTRPromotionList()
        {
            return View();
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin,HTRStaff")]
        public JsonResult GetHTRPromotionList()
        {
            
            var result = logDao.getHTRPromotionList().Where(item => item.IsActived == true).Select(item =>
                    new {
                        PromotionId = item.PromotionId,
                        PromotionName = item.PromotionName,
                        PromotionContent = item.PromotionContent,
                        Condition = item.Condition,
                        IsLuckyDate = item.IsDisplayLuckyDate,
                        IsActived = item.IsActived,
                        CreatedBy = item.CreatedBy,
                        CreatedTime = item.CreatedTime?.ToString("yyyy-MM-dd"),
                        UpdatedBy = item.UpdatedBy,
                        UpdatedTime = item.UpdatedTime?.ToString("yyyy-MM-dd"),
                    }
            ).OrderBy(pro => pro.PromotionId).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetPIDByPromotion(int PromotionID)
        {
            List<spHTR_GetPatronInfoByPromotionID_Result> playerList = logDao.getHTRPromotionPlayerList(PromotionID);
            return new JsonResult() { Data = playerList, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        public JsonResult GetHTRPromotionLog(int PromotionID)
        {
            if (PromotionID == 0)
            {
                PromotionID = logDao.getHTRPromotionList().Where(item => item.IsActived == true).OrderBy(pro => pro.PromotionId).FirstOrDefault().PromotionId;
            }
            var result = logDao.getHTRPromotionLog(PromotionID).Select(item =>
                    new {
                        PlayerID = item.PlayerID,
                        PlayerName = item.PlayerName,
                        PrintedDate = item.PrintedDate?.ToString("yyyy-MM-dd HH:mm"),
                        PrintedBy = item.PrintedBy,
                        ID = item.ID,
                        isVoided = item.isVoided,
                        ReprintedBy = item.ReprintedBy,
                        ReprintedDate = item.ReprintedDate?.ToString("yyyy-MM-dd HH:mm"),
                        VoidedBy = item.VoidedBy,
                        VoidedDate = item.VoidedDate?.ToString("yyyy-MM-dd HH:mm")
                    }
            ).ToList();
            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        public JsonResult SearchPromotionByPatron(int PromotionID, int PatronID)
        {
            var dao = new LogDao();
            spHTR_PromotionByPatron_Result patronInfo = new spHTR_PromotionByPatron_Result();
            patronInfo = dao.getPromotionByPatron(PromotionID, PatronID);
            
            return Json(patronInfo, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPromotionById(int PromotionID)
        {
            if (PromotionID == 0)
            {
                PromotionID = logDao.getHTRPromotionList().OrderBy(pro => pro.PromotionId).FirstOrDefault().PromotionId;
            }
            var item = logDao.getPromotionById(PromotionID);

            var result = new
            {
                PromotionId = item.PromotionId,
                PromotionName = item.PromotionName,
                Condition = item.Condition,
                IsActived = item.IsActived,
                Content = item.PromotionContent,
                IsLuckyDate = item.IsDisplayLuckyDate,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin,HTRStaff")]
        public JsonResult SaveHTRPromotion(HTRPromotion input)
        {
            if (input.PromotionId == 0)
            {
                input.CreatedBy = Session["UserName"].ToString();
                input.CreatedTime = DateTime.Now;
            }

            input.UpdatedBy = Session["UserName"].ToString();
            input.UpdatedTime = DateTime.Now;


            return Json(logDao.saveHTRPromotion(input), JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "SuperAdmin, HTRAdmin,HTRStaff")]
        public JsonResult SavePIDModify(int proID, string curPidName, string newPidName)
        {
            return Json(logDao.savePidModify(proID, curPidName, newPidName), JsonRequestBehavior.AllowGet);
        }


        //Ticket Promotion start
        [Authorize(Roles = "SuperAdmin, HTRAdmin,HTRStaff")]
        public ActionResult HTRPromotionLog()
        {
            return View();
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin,HTRStaff")]
        public ActionResult PrintHTRPromotion(int PlayerID, int PromotionID)
        {
            string url = "";
            var ID = logDao.PrintHTRPromotion(PlayerID, PromotionID);
            if (ID != 0)
            {
                url = "~/Assets/Reports/HTRPromotion.aspx?id=" + ID;
                return Redirect(url);
            }else
            {
                TempData["ErrorMessage"] = "An error occurred: The PID " + PlayerID .ToString() + " does not meet the requirements for printing.";
                return View("HTRPromotionLog");
            }
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin,HTRStaff")]
        public ActionResult ReprintHTRPromotion(int ID)
        {
            logDao.ReprintHTRPromotion(ID);
            string url = "~/Assets/Reports/HTRPromotion.aspx?id=" + ID;
            return Redirect(url);
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin,HTRStaff")]
        public JsonResult VoidHTRPromotion(int ID)
        {
            return Json(logDao.VoidHTRPromotion(ID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPromotionLogById(int logID)
        {
            return Json(logDao.getPromotionLogById(logID), JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "SuperAdmin, HTRAdmin")]
        public JsonResult SavePromotionLog(int LogID, string PlayerName)
        {
            string updateBy = Session["UserName"].ToString();
            return Json(logDao.savePromotionLog(LogID, PlayerName, updateBy), JsonRequestBehavior.AllowGet);
        }

        //MidAutume Promotion end

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
            this.Session["page"] = page;
            var model1 = logDao.ListAllPagingActivity(page, 50);
            return View(model1);
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public ActionResult ReportByDate()
        {
            IEnumerable<CasinoTrackingActivity> model = null;
            if (HttpContext.Request.Form["dfromdate"] != "" && HttpContext.Request.Form["dtodate"] != "")
            {
                var fdate = Convert.ToString(HttpContext.Request.Form["dfromdate"]);
                var tdate = Convert.ToString(HttpContext.Request.Form["dtodate"]);
                model = logDao.ListAllActivityLog(fdate, tdate);
            }
            else
            {
                model = logDao.ListAllActivityLog("", "").Take(100);
            }
            return View("Report", model);
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public ActionResult NewEnrollments()
        {
            string fromDate = DateTime.Now.ToString("yyyy-MM-dd");
            string toDate = DateTime.Now.ToString("yyyy-MM-dd");
            if (HttpContext.Request.Form["dfromdate"] != null && HttpContext.Request.Form["dfromdate"] != "")
            {
                fromDate = Convert.ToString(HttpContext.Request.Form["dfromdate"]);
            }
            if (HttpContext.Request.Form["dtodate"] != null && HttpContext.Request.Form["dtodate"] != "")
            {
                toDate = Convert.ToString(HttpContext.Request.Form["dtodate"]);
            }

            var result = logDao.getNewEnrollmentsList(DateTime.Parse(fromDate), DateTime.Parse(toDate));
            return View(result);
        }
    }
}