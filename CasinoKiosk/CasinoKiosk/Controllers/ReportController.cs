using CKDatabaseConnection.DAO;
using CKDatabaseConnection.EF;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CasinoKiosk.Areas.Admin.Controllers
{
    public class ReportController : Controller
    {

        CKdbContext db = new CKdbContext();
        ITHoTram_CustomReportEntities entity = new ITHoTram_CustomReportEntities();
        // GET: Admin/Report

        [Authorize(Roles = "SuperAdmin, CasinoAudit")]
        public ActionResult Index()
        {
            return Redirect("~/Assets/Reports/RedeemLog.aspx");
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public ActionResult CIFEReport()
        {
            return Redirect("http://10.70.1.53/Reports/Pages/Report.aspx?ItemPath=%2fCustom+Reports%2fCIFE+Program");
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin")]
        public ActionResult Reprint(int id)
        {
            var dao = new LogDao();
            var log = new CasinoKiosk_Log();

            string s1 = Session["userName"].ToString();

            updateReprintedPersonByID(id, Session["userName"].ToString());

            var model = dao.ListAllPaging(1,50);
            return Redirect("~/Assets/Reports/Tickets.aspx");
            
        }

        
        public MFDailyBonus_SecondLogs GetDailyLogByID(int ID)
        {
            return db.MFDailyBonus_SecondLogs.SingleOrDefault(x => x.ID == ID);
        }      

        
        public MFWeeklyBonus_Logs GetWeeklyLogByID(int ID)
        {
            return db.MFWeeklyBonus_Logs.SingleOrDefault(x => x.ID == ID);
        }

        
        public MFFridayBonus_Logs GetFridayLogByID(int ID)
        {
            return db.MFFridayBonus_Logs.SingleOrDefault(x => x.ID == ID);
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin")]
        public ActionResult DailyReprint(int id, int playerID, string promoName)
        {
            var dao = new LogDao();
           
            MFBonus_ReprintLog reprintLog = new MFBonus_ReprintLog();
            var dailyLog = GetDailyLogByID(id);
            string s1 = Session["userName"].ToString();

            var countDailyLog = entity.MFBonus_ReprintLog.Where(x=>x.ID == id && x.PlayerID == playerID && x.PromotionName == promoName);
            
            if (countDailyLog.Count() == 0)
            {
                reprintLog.TicketNo = id;
                reprintLog.PlayerID = playerID;
                reprintLog.PlayerName = dailyLog.PlayerName;
                reprintLog.PromotionName = dailyLog.PromotionName;
                reprintLog.ItemName = dailyLog.ItemName;
                reprintLog.PrintedBy = s1;
                reprintLog.PrintedDate = DateTime.Now.ToString("dd/MM/yyyy");

                entity.MFBonus_ReprintLog.Add(reprintLog);
                entity.SaveChanges();
            }
            else {
                reprintLog.PrintedBy = s1;
                reprintLog.PrintedDate = DateTime.Now.ToString("dd/MM/yyyy");
                entity.SaveChanges();
            }
                                             
            //updateReprintedPersonByID(id, Session["userName"].ToString());

            var model = dao.ListAllPagingDailyLog(1, 50, 0);
            string url = "~/Assets/Reports/DailyTickets.aspx?id=" + id + "&playerID=" + playerID + "&promoName" + promoName;
            return Redirect(url);
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin")]
        public ActionResult WeeklyReprint(int id, int playerID, string promoName)
        {
            var dao = new LogDao();

            MFBonus_ReprintLog reprintLog = new MFBonus_ReprintLog();
            var weeklyLog = GetWeeklyLogByID(id);
            string s1 = Session["userName"].ToString();

            var countWeeklyLog = entity.MFBonus_ReprintLog.Where(x => x.ID == id && x.PlayerID == playerID && x.PromotionName == promoName);

            if (countWeeklyLog.Count() == 0)
            {
                reprintLog.TicketNo = id;
                reprintLog.PlayerID = weeklyLog.PlayerID;
                reprintLog.PlayerName = weeklyLog.PlayerName;
                reprintLog.PromotionName = weeklyLog.PromotionName;
                reprintLog.ItemName = weeklyLog.ItemName;
                reprintLog.PrintedBy = s1;
                reprintLog.PrintedDate = DateTime.Now.ToString("dd/MM/yyyy");

                entity.MFBonus_ReprintLog.Add(reprintLog);
                entity.SaveChanges();
            }
            else
            {
                reprintLog.PrintedBy = s1;
                reprintLog.PrintedDate = DateTime.Now.ToString("dd/MM/yyyy");
                entity.SaveChanges();
            }

            //updateReprintedPersonByID(id, Session["userName"].ToString());

            var model = dao.ListAllPagingWeeklyLog(1, 50, 0);
            string url = "~/Assets/Reports/WeeklyTickets.aspx?id=" + id + "&playerID=" + playerID +"&promoName" + promoName;
            return Redirect(url);
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin")]
        public ActionResult FridayReprint(int id, int playerID, string promoName)
        {
            var dao = new LogDao();

            MFBonus_ReprintLog reprintLog = new MFBonus_ReprintLog();
            var fridayLog = GetFridayLogByID(id);
            string s1 = Session["userName"].ToString();

            var countFridayLog = entity.MFBonus_ReprintLog.Where(x => x.ID == id && x.PlayerID == playerID && x.PromotionName == promoName);

            if (countFridayLog.Count() == 0)
            {
                reprintLog.TicketNo = id;
                reprintLog.PlayerID = fridayLog.PlayerID;
                reprintLog.PlayerName = fridayLog.PlayerName;
                reprintLog.PromotionName = fridayLog.PromotionName;
                reprintLog.ItemName = fridayLog.ItemName;
                reprintLog.PrintedBy = s1;
                reprintLog.PrintedDate = DateTime.Now.ToString("dd/MM/yyyy");

                entity.MFBonus_ReprintLog.Add(reprintLog);
                entity.SaveChanges();
            }
            else
            {
                reprintLog.PrintedBy = s1;
                reprintLog.PrintedDate = DateTime.Now.ToString("dd/MM/yyyy");
                entity.SaveChanges();
            }

            //updateReprintedPersonByID(id, Session["userName"].ToString());

            var model = dao.ListAllPagingFridayLog(1, 50, 0);
            string url = "~/Assets/Reports/FridayTickets.aspx?id=" + id + "&playerID=" + playerID + "&promoName" + promoName;
            return Redirect(url);
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin")]
        public ActionResult MFRedemptionReprint(int id)
        {
            var dao = new LogDao();

            MFPointsRedemption_Logs log = new MFPointsRedemption_Logs();
            
            string s1 = Session["userName"].ToString();

            log = entity.MFPointsRedemption_Logs.FirstOrDefault(x=>x.ID == id);

            log.reprintedPerson = s1;
            //Add 20230109 Hantt start
            log.reprintedTime = DateTime.Now;
            //Add 20230109 Hantt end
            log.voidedStatus = "re-printed";

            entity.SaveChanges();

            var model = dao.ListAllPagingFridayLog(1, 50, 0);
            string url = "~/Assets/Reports/TicketMFRedemption.aspx?id=" + id;
            return Redirect(url);
        }

        
        public void updateReprintedPersonByID(int ID, string reprintedPerson)
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CkdbContext"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("CasinoKiosk_spSetReprintedPerson", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@ID", ID));
                    cmd.Parameters.Add(new SqlParameter("@reprintedPerson", reprintedPerson));
                    //Add 20230109 Hantt start
                    cmd.Parameters.Add(new SqlParameter("@reprintedTime", DateTime.Now.ToString()));
                    //Add 20230109 Hantt end
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }

    }
}