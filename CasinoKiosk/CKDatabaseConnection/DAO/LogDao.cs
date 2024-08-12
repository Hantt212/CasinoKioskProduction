using CKDatabaseConnection.EF;
using CKDatabaseConnection.Supports;
using Microsoft.Reporting.WebForms;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CKDatabaseConnection.DAO
{
    public class LogDao
    {

        ITHoTram_CustomReportEntities context = null;
        public LogDao()
        {

            context = new ITHoTram_CustomReportEntities();
        }

        public long Insert(int itemID)
        {
            CasinoKiosk_Log log = new CasinoKiosk_Log();

            ItemDao item = new ItemDao();
            CasinoKiosk_Item i = item.GetItemByID(itemID);

            log.ID = itemID;

            //log.createdDate = System.DateTime.Now;
            log.LogName = i.ItemName;
            //log.remainingQuantity = i.Quantity - 1;
            context.CasinoKiosk_Log.Add(log);
            context.SaveChanges();

            //var updateQuantity = item.UpdateItemQuantity(i, i.Quantity - 1);

            return log.ID;
        }

        public IEnumerable<CasinoKiosk_Log> ListAllPaging(int page, int pageSize)
        {
            //return context.CasinoKiosk_Log.OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
            List<CasinoKiosk_Log> list = context.CasinoKiosk_Log.ToList();
            List<CasinoKiosk_Log> result = new List<CasinoKiosk_Log>();
            var last3month = DateTime.Today.AddMonths(-3).ToString("yyyy-MM-dd");
            list.ForEach(item =>
            {
                string dd = item.createdDate.Substring(0, 2);
                string mm = item.createdDate.Substring(3, 2);
                string yyyy = item.createdDate.Substring(6, 4);

                dd = yyyy + '-' + mm + '-' + dd;
                if (String.Compare(dd, last3month) >= 0)
                {
                    result.Add(item);
                }
            });
            return result.OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
        }

        //add by Doc Ly 2021/06/01
        public IEnumerable<CasinoKiosk_Log> ListAllByDate(string fromdate, string todate, int playerID)
        {
            List<CasinoKiosk_Log> list = new List<CasinoKiosk_Log>();
            try
            {
                list = context.CasinoKiosk_Log.ToList()
                .Where(p => ((playerID > 0) ? p.PlayerID == playerID : true) && ((fromdate != "" && todate != "") ? (
                DateTime.ParseExact(p.createdDate, "dd-MM-yyyy", CultureInfo.CurrentCulture) >= DateTime.Parse(fromdate, CultureInfo.CurrentCulture)
                &&
                DateTime.ParseExact(p.createdDate, "dd-MM-yyyy", CultureInfo.CurrentCulture) <= DateTime.Parse(todate, CultureInfo.CurrentCulture)
                ) : true)
                ).OrderByDescending(x => x.ID).ToList();
            }
            catch (Exception ex)
            { }
            return list;
        }
        //End
        public CasinoKiosk_Log GetLogByID(int ID)
        {
            return context.CasinoKiosk_Log.SingleOrDefault(x => x.ID == ID);
        }

        //add by Doc Ly: 2021/04/15
        public IEnumerable<MFBonus_spSelectDailyLogs_Result> ListAllDailyLog(string fromdate, string todate, int playerID)
        {
            List<MFBonus_spSelectDailyLogs_Result> list = context.MFBonus_spSelectDailyLogs_Result(playerID)
                .Where(p => (fromdate != "" && todate != "") ? (
                DateTime.ParseExact(p.IssueDate, "dd/MM/yyyy", CultureInfo.CurrentCulture) >= DateTime.Parse(fromdate, CultureInfo.CurrentCulture)
                &&
                DateTime.ParseExact(p.IssueDate, "dd/MM/yyyy", CultureInfo.CurrentCulture) <= DateTime.Parse(todate, CultureInfo.CurrentCulture)
                ) : true
                ).ToList();
            return list.OrderByDescending(x => x.ID);
        }


        public spHTR_PromotionByPatron_Result getPromotionByPatron(int promotion,int playerID)
        {
            spHTR_PromotionByPatron_Result result = context.spHTR_PromotionByPatron(playerID, promotion).FirstOrDefault();
            return result;
        }

        public List<spHTR_PromotionLogByPromotionID_Result> getHTRPromotionLog(int promotionID)
        {
            List<spHTR_PromotionLogByPromotionID_Result> list = context.spHTR_PromotionLogByPromotionID(promotionID).ToList();
            return list;
        }

        public HTRPromotion getPromotionById(int promotionID)
        {
            HTRPromotion result = context.HTRPromotions.Find(promotionID);
            return result;
        }

        public long PrintHTRPromotion(int PlayerID, int PromotionID)
        {
            var ticket = new HTRPromotionLog();
            string userLogin = HttpContext.Current.Session["UserName"].ToString();
            HTRPromotionPlayer player = context.HTRPromotionPlayers.OrderByDescending(item => item.InsertDate).FirstOrDefault(item => item.PlayerID == PlayerID && item.PromotionId == PromotionID);
            if (player != null)
            {
                if (player.Quantity == 1)
                {
                    player.Quantity = 0;
                    player.PrintedQty += 1;

                    //write log
                    ticket.PlayerID = PlayerID;
                    ticket.PlayerName = player.PlayerName;
                    ticket.PromotionId = PromotionID;
                    ticket.isPrinted = true;
                    ticket.PrintedDate = DateTime.Now;
                    ticket.PrintedBy = userLogin;
                    context.HTRPromotionLogs.Add(ticket);
                    context.SaveChanges();
                    return ticket.ID;
                }
            }
            return 0;
        }

        public string ReprintHTRPromotion(int ID)
        {
            string userLogin = HttpContext.Current.Session["UserName"].ToString();
            HTRPromotionLog ticket = context.HTRPromotionLogs.Find(ID);

            ticket.ReprintedBy = userLogin;
            ticket.ReprintedDate = DateTime.Now;
            context.SaveChanges();
            string filename = DateTime.Now.ToString("ddMMyyyy_HHmmss");
            return filename;

        }

        public long VoidHTRPromotion(int id)
        {
            var ticket = new HTRPromotionLog();

            ticket = context.HTRPromotionLogs.FirstOrDefault(x => x.ID == id);


            string username = HttpContext.Current.Session["UserName"].ToString();
            ticket.VoidedBy = username;
            ticket.isVoided = true;
            ticket.VoidedDate = DateTime.Now;



            int patronId = ticket.PromotionId;
            HTRPromotionPlayer promotion = context.HTRPromotionPlayers
                                            .Where(item => item.PromotionId == ticket.PromotionId && item.PlayerID == ticket.PlayerID)
                                            .OrderByDescending(item => item.InsertDate).FirstOrDefault();
            promotion.Quantity = 1;
            context.SaveChanges();
            return ticket.ID;
        }

        //Mid Autime end

        public HTRPromotionLog getPromotionLogById(int logID)
        {
            HTRPromotionLog result = context.HTRPromotionLogs.Find(logID);
            return result;
        }
        

        public List<HTRPromotion> getHTRPromotionList()
        {
            List<HTRPromotion> result = context.HTRPromotions.ToList();
            return result;
        }

        public int saveHTRPromotion(HTRPromotion input)
        {
            try
            {
                if (input.PromotionId == 0)
                {
                    context.HTRPromotions.Add(input);
                }
                else
                {
                    HTRPromotion promotion = context.HTRPromotions.Find(input.PromotionId);
                    promotion.PromotionName = input.PromotionName;
                    promotion.PromotionContent = input.PromotionContent;
                    promotion.Condition = input.Condition;
                    promotion.IsDisplayLuckyDate = input.IsDisplayLuckyDate;
                    promotion.IsActived = input.IsActived;
                    promotion.UpdatedBy = input.UpdatedBy;
                    promotion.UpdatedTime = input.UpdatedTime;
                }
                context.SaveChanges();
                return 0;
            }
            catch(Exception e)
            {
                return 1;
            }
        }

        public int savePromotionLog(int logID, string playerName, string updatedBy)
        {
            try
            {
                HTRPromotionLog log = context.HTRPromotionLogs.Find(logID);
                log.PlayerName = playerName;
                log.UpdatedDate = DateTime.Now;
                log.UpdatedBy = updatedBy;

                HTRPromotionPlayer player = context.HTRPromotionPlayers.Where(item => item.PlayerID == log.PlayerID).FirstOrDefault();
                player.PlayerName = playerName;
               
                context.SaveChanges();
                return 0;
            }
            catch (Exception e)
            {
                return 1;
            }
        }

        //Daily
        public IEnumerable<MFBonus_spSelectDailyLogs_Result> ListAllPagingDailyLog(int page, int pageSize, int playerID)
        {
            List<MFBonus_spSelectDailyLogs_Result> list = context.MFBonus_spSelectDailyLogs_Result(playerID).ToList();
            return list.OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
        }
        
        public IEnumerable<MFBonus_spSelectDailyLogs_Result> ListAllDailyLog(int playerID)
        {
            List<MFBonus_spSelectDailyLogs_Result> list = context.MFBonus_spSelectDailyLogs_Result(playerID).ToList();
            return list.OrderByDescending(x => x.ID);
        }

        //Weekly
        public IEnumerable<MFBonus_spSelectWeeklyLogs_Result> ListAllPagingWeeklyLog(int page, int pageSize, int playerID)
        {
            List<MFBonus_spSelectWeeklyLogs_Result> list = context.MFBonus_spSelectWeeklyLogs_Result(playerID).ToList();
            return list.OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
        }
       
        public IEnumerable<MFBonus_spSelectWeeklyLogs_Result> ListAllWeeklyLog(string fromdate, string todate, int playerID)
        {
            List<MFBonus_spSelectWeeklyLogs_Result> list = context.MFBonus_spSelectWeeklyLogs_Result(playerID)
                .Where(p => (fromdate != "" && todate != "") ? (
                DateTime.ParseExact(p.IssueDate, "dd/MM/yyyy", CultureInfo.CurrentCulture) >= DateTime.Parse(fromdate, CultureInfo.CurrentCulture)
                &&
                DateTime.ParseExact(p.IssueDate, "dd/MM/yyyy", CultureInfo.CurrentCulture) <= DateTime.Parse(todate, CultureInfo.CurrentCulture)
                ) : true
                ).ToList();
            return list.OrderByDescending(x => x.ID);
        }

        //Bufet 8Dragons start
        public IEnumerable<MF8DragonBuffetBonus_Logs> ListAllPagingMF8DragonLog(int page, int pageSize, int playerID)
        {
            List<MF8DragonBuffetBonus_Logs> list = context.MF8DragonBuffetBonus_Logs.Where(item => item.PlayerID == (playerID > 0 ? playerID : item.PlayerID)).ToList();
            return list.OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
        }

        public IEnumerable<MF8DragonBuffetBonus_Logs> ListAllMF8DragonLog(string fromdate, string todate, int playerID)
        {
            List<MF8DragonBuffetBonus_Logs> list = context.MF8DragonBuffetBonus_Logs.Where(item => item.PlayerID == (playerID > 0 ? playerID : item.PlayerID)).ToList();
            if (fromdate != "" && todate != "")
            {
                list = list.Where(p => (DateTime.ParseExact(p.IssueDate, "dd/MM/yyyy", CultureInfo.CurrentCulture) >= DateTime.Parse(fromdate, CultureInfo.CurrentCulture)
                &&
                DateTime.ParseExact(p.IssueDate, "dd/MM/yyyy", CultureInfo.CurrentCulture) <= DateTime.Parse(todate, CultureInfo.CurrentCulture)
                )).ToList();
            }
                
            return list.OrderByDescending(x => x.ID);
        }

        //Bufet 8Dragons end

        public IEnumerable<MFBonus_spSelectFridayLogs_Result> ListAllPagingFridayLog(int page, int pageSize, int playerID)
        {
            List<MFBonus_spSelectFridayLogs_Result> list = context.MFBonus_spSelectFridayLogs_Result(playerID).ToList();
            return list.OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
        }

        // add by Doc Ly 2021/05/28
        public IEnumerable<MFBonus_spSelectFridayLogs_Result> ListAllFridayLog(string fromdate, string todate, int playerID)
        {
            List<MFBonus_spSelectFridayLogs_Result> list = context.MFBonus_spSelectFridayLogs_Result(playerID)
                .Where(p => (fromdate != "" && todate != "") ? (
                DateTime.ParseExact(p.IssueDate, "dd/MM/yyyy", CultureInfo.CurrentCulture) >= DateTime.Parse(fromdate, CultureInfo.CurrentCulture)
                &&
                DateTime.ParseExact(p.IssueDate, "dd/MM/yyyy", CultureInfo.CurrentCulture) <= DateTime.Parse(todate, CultureInfo.CurrentCulture)
                ) : true
                ).ToList();
            return list.OrderByDescending(x => x.ID);
        }
        //End
        public IEnumerable<MFBonus_spSelectSlotDailyLogs_Result> ListAllPagingSlotDailyLog(int page, int pageSize, int playerID)
        {
            List<MFBonus_spSelectSlotDailyLogs_Result> list = context.MFBonus_spSelectSlotDailyLogs(playerID).ToList();
            return list.OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
        }
        
        public IEnumerable<MFBonus_spSelectSlotDailyLogs_Result> ListAllSlotDailyLog(string fromdate, string todate, int playerID)
        {
            List<MFBonus_spSelectSlotDailyLogs_Result> list = context.MFBonus_spSelectSlotDailyLogs(playerID)
                .Where(p => (fromdate != "" && todate != "") ? (
                DateTime.ParseExact(p.IssueDate, "dd/MM/yyyy", CultureInfo.CurrentCulture) >= DateTime.Parse(fromdate, CultureInfo.CurrentCulture)
                &&
                DateTime.ParseExact(p.IssueDate, "dd/MM/yyyy", CultureInfo.CurrentCulture) <= DateTime.Parse(todate, CultureInfo.CurrentCulture)
                ) : true
                ).ToList();
            return list.OrderByDescending(x => x.ID);
        }

        public IEnumerable<FOPatronUserActionLog_SelectAll_Result> ListAllFOPatronUserLog(string fromdate, string todate)
        {
            List<FOPatronUserActionLog_SelectAll_Result> list = context.FOPatronUserActionLog_SelectAll()
                .Where(p => (fromdate.ToString() != "" && todate.ToString() != "") ? (
                p.ActionDate >= DateTime.Parse(fromdate, CultureInfo.CurrentCulture) && p.ActionDate <= DateTime.Parse(todate, CultureInfo.CurrentCulture)) : true).ToList();
            return list.OrderByDescending(x => x.ID);
        }

        public IEnumerable<FOPatronUserActionLog_SelectAll_Result> ListAllPagingFOPatronUserLog(int page, int pageSize)
        {
            List<FOPatronUserActionLog_SelectAll_Result> list = context.FOPatronUserActionLog_SelectAll().ToList();
            return list.OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
        }

        public IEnumerable<POSPatron8DragonUserActionLog> ListAllPagingPOS8DragonUserLog(int page, int pageSize)
        {
            List<POSPatron8DragonUserActionLog> list = context.POSPatron8DragonUserActionLog.ToList();
            return list.OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
        }

        public IEnumerable<POSPatron8DragonUserActionLog> ListAllPOS8DragonUserLog(string fromdate, string todate)
        {
            List<POSPatron8DragonUserActionLog> list = context.POSPatron8DragonUserActionLog.ToList()
                .Where(p => (fromdate.ToString() != "" && todate.ToString() != "") ? (
                p.ActionDate >= DateTime.Parse(fromdate, CultureInfo.CurrentCulture) && p.ActionDate <= DateTime.Parse(todate, CultureInfo.CurrentCulture)) : true).ToList();
            return list.OrderByDescending(x => x.ID);
        }

        public IEnumerable<CurrencyConverterUserActionLog> ListAllPagingCCUserLog(int page, int pageSize)
        {
            List<CurrencyConverterUserActionLog> list = context.CurrencyConverterUserActionLogs.ToList();
            return list.OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
        }

        public IEnumerable<CurrencyConverterUserActionLog> ListAllCCUserLog(string fromdate, string todate)
        {
            List<CurrencyConverterUserActionLog> list = context.CurrencyConverterUserActionLogs.ToList()
                .Where(p => (fromdate.ToString() != "" && todate.ToString() != "") ? (
                p.ActionDate >= DateTime.Parse(fromdate, CultureInfo.CurrentCulture) && p.ActionDate <= DateTime.Parse(todate, CultureInfo.CurrentCulture)) : true).ToList();
            return list.OrderByDescending(x => x.ID);
        }
        public IEnumerable<DailyPSBundle_Logs> ListAllPSBundleLog(string fromdate, string todate)
        {
            List<DailyPSBundle_Logs> list = context.DailyPSBundle_Logs.ToList()
                .Where(p => (fromdate.ToString() != "" && todate.ToString() != "") ? (
                DateTime.Parse(p.IssueDate) >= DateTime.Parse(fromdate, CultureInfo.CurrentCulture) && DateTime.Parse(p.IssueDate) <= DateTime.Parse(todate, CultureInfo.CurrentCulture)) : true).ToList();
            return list.OrderByDescending(x => x.ID);
        }
        public IEnumerable<DailyPSBundle_Logs> ListAllPagingPSBundleLog(int page, int pageSize)
        {
            List<DailyPSBundle_Logs> list = context.DailyPSBundle_Logs.ToList();
            return list.OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
        }
        public IEnumerable<CasinoTrackingActivity> ListAllPagingActivity(int page, int pageSize)
        {
            List<CasinoTrackingActivity> list = context.CasinoTrackingActivities.ToList();
            return list.OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
        }
        public IEnumerable<CasinoTrackingActivity> ListAllActivityLog(string fromdate, string todate)
        {
            List<CasinoTrackingActivity> list = context.CasinoTrackingActivities.ToList()
                .Where(p => (fromdate.ToString() != "" && todate.ToString() != "") ? (
                DateTime.Parse(p.CreatedDate) >= DateTime.Parse(fromdate, CultureInfo.CurrentCulture) && DateTime.Parse(p.CreatedDate) <= DateTime.Parse(todate, CultureInfo.CurrentCulture)) : true).ToList();
            return list.OrderByDescending(x => x.ID);
        }
        public IEnumerable<KioskUserActionLog_SelectAll_Result> ListAllKioskUserLog(string fromdate, string todate)
        {
            List<KioskUserActionLog_SelectAll_Result> list = context.KioskUserActionLog_SelectAll()
                .Where(p => (fromdate.ToString() != "" && todate.ToString() != "") ? (
                p.ActionDate >= DateTime.Parse(fromdate, CultureInfo.CurrentCulture) && p.ActionDate <= DateTime.Parse(todate, CultureInfo.CurrentCulture)) : true).ToList();
            return list.OrderByDescending(x => x.ID);
        }

        public IEnumerable<KioskUserActionLog_SelectAll_Result> ListAllPagingKioskUserLog(int page, int pageSize)
        {
            List<KioskUserActionLog_SelectAll_Result> list = context.KioskUserActionLog_SelectAll().ToList();
            return list.OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
        }

        public IEnumerable<MFBonus_spSelectRedemptionLogs_Result> ListAllPagingRedemptionLog(int page, int pageSize, int playerID)
        {
            List<MFBonus_spSelectRedemptionLogs_Result> list = context.MFBonus_spSelectRedemptionLogs_Result(playerID).ToList();
            //return list.OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
            List<MFBonus_spSelectRedemptionLogs_Result> result = new List<MFBonus_spSelectRedemptionLogs_Result>();
            var last3month = DateTime.Today.AddMonths(-3).ToString("yyyy-MM-dd");
            list.ForEach(item =>
            {
                string dd = item.createdDate.Substring(0, 2);
                string mm = item.createdDate.Substring(3, 2);
                string yyyy = item.createdDate.Substring(6, 4);

                dd = yyyy + '-' + mm + '-' + dd;
                if (String.Compare(dd, last3month) >= 0)
                {
                    result.Add(item);
                }
            });
            return result.OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
        }
        // add by Doc Ly 2021/05/28
        public IEnumerable<MFBonus_spSelectRedemptionLogs_Result> ListAllRedemptionLog(string fromdate, string todate, int playerID)
        {
            List<MFBonus_spSelectRedemptionLogs_Result> list = context.MFBonus_spSelectRedemptionLogs_Result(playerID)
                .Where(p => (fromdate != "" && todate != "") ? (
                DateTime.ParseExact(p.createdDate, "dd-MM-yyyy", CultureInfo.CurrentCulture) >= DateTime.Parse(fromdate, CultureInfo.CurrentCulture)
                &&
                DateTime.ParseExact(p.createdDate, "dd-MM-yyyy", CultureInfo.CurrentCulture) <= DateTime.Parse(todate, CultureInfo.CurrentCulture)
                ) : true
                ).ToList();
            return list.OrderByDescending(x => x.ID);
        }
        //End
        
    }
}

