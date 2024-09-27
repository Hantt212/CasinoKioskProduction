using CKDatabaseConnection.DAO;
using CKDatabaseConnection.EF;
using CKDatabaseConnection.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml;

namespace WebServiceApp
{
    /// <summary>
    /// Summary description for WebServiceCasino
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class WebServiceCasino : System.Web.Services.WebService
    {

        List<Items> itemList = new List<Items>();
        List<ItemsDaily> itemListDaily = new List<ItemsDaily>();

        //List<RedeemLog> logList = new List<RedeemLog>();
        CKdbContext db = new CKdbContext();
        CKFunction function = new CKFunction();
        FBCasino fbFunction = new FBCasino();

        //db casinokiosk
        string connectionString = ConfigurationManager.ConnectionStrings["CKdbContext"].ConnectionString;
        
        //Redemption Webservices

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        //Change 20230109 Hantt start
        //public RedeemLog RedeemProcess(int ID, int playerID)
        public RedeemLog RedeemProcess(int ID, int playerID, string location)
        //Change 20230109 Hantt end
        {
            CasinoKiosk_Item item = new CasinoKiosk_Item();
            ItemDao dao = new ItemDao();
            CasinoKiosk_Log log = new CasinoKiosk_Log();

            int ok = 0;
            //int redeemBalance = 0;
            DataSet ds;

            int maxID = 0;
            int playerPoints = function.getPlayerPointsBalance(playerID);
            int currentPlayerPoints = 0;
            int itemPoints = function.getItemPoints(ID);
            string itemName = function.getItemName(ID);


            int quan = function.getItemQuantity(ID);
            string imageUrl = function.getItemImageUrl(ID);


            if (playerPoints >= itemPoints)
            {


                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("Proc_PlayerPointRedeem", con))
                    {
                        con.Open();
                        cmd.Connection = con;
                        cmd.Parameters.Add(new SqlParameter("@@nPlayerID", playerID));
                        cmd.Parameters.Add(new SqlParameter("@@nRedeemPoints", itemPoints));
                        cmd.Parameters.Add(new SqlParameter("@@nReasonNumber", 1));
                        cmd.Parameters.Add(new SqlParameter("@@szComment", "Gift points redeemed for" + playerID.ToString() + "- Amount: " + itemPoints.ToString()));
                        cmd.Parameters.Add(new SqlParameter("@@nUserID", 31580));
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {

                            ds = new DataSet();
                            da.Fill(ds);
                        }
                    }
                }
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            currentPlayerPoints = Convert.ToInt32(ds.Tables[0].Rows[0]["NewPlayerBalance"].ToString());

                            ok = 1;

                            log.LogName = itemName;

                            DateTime now = DateTime.Now;
                            var today = DateTime.Today;

                            int currentTime = Int32.Parse(now.Hour.ToString());
                            if (currentTime < 6)
                            {
                                var yesterday = today.AddDays(-1);
                                log.gamingDate = yesterday;
                            }
                            else
                            {
                                log.gamingDate = today;
                            }

                            log.createdDate = DateTime.Now.ToString("dd-MM-yyyy");
                            log.createdTime = DateTime.Now.ToString("HH:mm:ss");

                            log.PlayerID = playerID;
                            log.PlayerName = function.getPlayerName(playerID);
                            log.PromotionName = "Points Redemption";
                            log.ItemPoints = itemPoints;
                            log.Status = ok;
                            log.CurrentPlayerPoints = currentPlayerPoints;
                            log.voidedStatus = "printed";
                            //Add 20230109 Hantt start
                            log.Location = location;
                            //Add 20230109 Hantt end

                            if (quan > 1)
                            {
                                log.Quantity = quan - 1;

                                function.updateIsActive(ID, true);
                                function.updateQuantity(ID, quan - 1);
                            }
                            else
                            {
                                log.Quantity = 0;

                                function.updateIsActive(ID, false);
                                function.updateQuantity(ID, 0);
                            }

                            try
                            {
                                db.CasinoKiosk_Log.Add(log);
                                db.SaveChanges();



                                maxID = log.ID;

                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                    }
                }
            }

            else
            {
                ok = 0;
            }

            var redeemLog = new RedeemLog()
            {
                TicketNo = maxID,
                LogName = itemName,
                issuedDate = DateTime.Now.ToString("dd-MM-yyyy"),
                issuedTime = DateTime.Now.ToString("HH:mm"),
                PlayerID = playerID,
                PlayerName = function.getPlayerName(playerID),
                PromotionName = "Points Redemption",
                ItemPoints = itemPoints,
                Status = ok,
                CurrentPlayerPoints = currentPlayerPoints,
                Quantity = function.getItemQuantity(ID),
                voidedStatus = "printed"

            };
            return redeemLog;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<Items> GetItemListByIsActive()
        {
            Items item = new Items();



            DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("CasinoKiosk_spGetItemByIsActive", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    //cmd.Parameters.Add(new SqlParameter("@ID", ID));
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {

                        ds = new DataSet();
                        da.Fill(ds);
                    }
                }
            }
            try
            {
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                itemList.Add(new Items()
                                {
                                    ID = Convert.ToInt32(dr["ID"]),
                                    ItemName = dr["ItemName"].ToString(),
                                    ItemPoints = Convert.ToInt32(dr["ItemPoints"].ToString()),
                                    isActive = Convert.ToBoolean(dr["isActive"].ToString()),
                                    //Change 20230111 Hantt start
                                    //imageURL = "http://casinokiosk/Assets/Images/" + dr["imageURL"].ToString(),
                                    imageURL = "http://10.21.10.1:8222/Assets/Images/" + dr["imageURL"].ToString(),
                                    //Change 20230111 Hantt end
                                    Quantity = Convert.ToInt32(dr["Quantity"].ToString()),


                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return itemList;


        }

        public List<Items> GetItemListByID(int ID)
        {
            //Item item = new Item();
            //List<Item> itemList = new List<Item>();
            DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("CasinoKiosk_spGetItemByID", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@ID", ID));
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {

                        ds = new DataSet();
                        da.Fill(ds);
                    }
                }
            }
            try
            {
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                itemList.Add(new Items()
                                {
                                    ID = Convert.ToInt32(dr["ID"]),
                                    ItemName = dr["ItemName"].ToString(),
                                    ItemPoints = Convert.ToInt32(dr["ItemPoints"].ToString()),
                                    isActive = Convert.ToBoolean(dr["isActive"].ToString()),
                                    imageURL = dr["imageURL"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return itemList;
        }

        public List<Items> getItemList()
        {
            Items item = new Items();

            DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("CasinoKiosk_spGetItem", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {

                        ds = new DataSet();
                        da.Fill(ds);
                    }
                }
            }
            try
            {
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                itemList.Add(new Items()
                                {
                                    ID = Convert.ToInt32(dr["ID"]),
                                    ItemName = dr["ItemName"].ToString(),
                                    ItemPoints = Convert.ToInt32(dr["ItemPoints"].ToString()),
                                    isActive = Convert.ToBoolean(dr["isActive"].ToString()),
                                    imageURL = "10.30.7.25/MyWebSite/Assets/Images" + dr["imageURL"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return itemList;
        }

        //Daily Play & Stay Bundle Promotion webservices

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public RedeemLogDailyPSbundleFirst RedeemDailyPSBundleFirst(int playerID)
        {
            ITHoTram_CustomReportEntities entity = new ITHoTram_CustomReportEntities();
            DailyPSBundle_Players player = new DailyPSBundle_Players();
            DailyPSBundle_Items item = new DailyPSBundle_Items();

            string playerName = function.getPlayerName(playerID);

            DateTime currentDay;

            bool isItemExisted = entity.DailyPSBundle_Items.Any(i => i.PlayerID == playerID);
            bool isPlayerExisted = entity.DailyPSBundle_Players.Any(i => i.PlayerID == playerID);

            DateTime playerGamingDate = function.getPlayerGamingDateDailyPSBundle(playerID);

            currentDay = function.getMFDBCurrentDay();

            //insert items and players here.            
            if (!isPlayerExisted)
            {
                player.PlayerID = playerID;
                player.PlayerName = playerName;
                player.GamingDate = currentDay;

                entity.DailyPSBundle_Players.Add(player);
                entity.SaveChanges();
            }

            //if items not existed.
            if (!isItemExisted)
            {
                player = entity.DailyPSBundle_Players.SingleOrDefault(p => p.PlayerID == playerID);
                function.updatePlayerGamingDatePSBundle(playerID, currentDay);
                function.insertDailyPSBundleItems(playerID);
            }
            //if items existed.
            else
            {
                //compare last player gaming date vs current gaming date/yesterday
                if (playerGamingDate < currentDay)
                {
                    function.updateItemDailyPSBundleAll(playerID, 1);
                    function.updatePlayerGamingDatePSBundle(playerID, currentDay);
                }
            }

            var dailyLog = new RedeemLogDailyPSbundleFirst()
            {
                PlayerID = playerID,
                PlayerName = function.getPlayerName(playerID),
                PromotionName = "Play & Stay Bundle",
                GamingDate = currentDay.ToString("dd/MM/yyyy"),
                Items = entity.DailyPSBundle_Items.Where(i => i.PlayerID == playerID).ToList()
            };
            return dailyLog;
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public RedeemLogDailyPSbundleSecond RedeemDailyPSBundleSecond(int itemID, int playerID)
        {
            ITHoTram_CustomReportEntities entity = new ITHoTram_CustomReportEntities();

            DailyPSBundle_Items item = new DailyPSBundle_Items();

            DailyPSBundle_Logs log = new DailyPSBundle_Logs();

            int maxID = 0;

            item = entity.DailyPSBundle_Items.SingleOrDefault(x => x.ID == itemID);

            item.Status = 2;

            function.updateItemDailyPSBundleRemain(playerID, 0, item.ID);

            log.ItemName = item.ItemName;
            log.ItemID = itemID;
            log.GamingDate = function.getMFWBCurrentDay().ToString("dd/MM/yyyy");
            log.IssueDate = DateTime.Now.ToString("dd/MM/yyyy");
            log.IssueTime = DateTime.Now.ToString("HH:mm:ss");
            log.PlayerID = playerID;
            log.PlayerName = function.getPlayerName(playerID);
            log.PromotionName = "Play & Stay Bundle";
            log.Status = 2;
            log.Type = "T";
            log.voidedStatus = "printed";

            try
            {
                entity.DailyPSBundle_Logs.Add(log);
                entity.SaveChanges();

                maxID = log.ID;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var dailyLog = new RedeemLogDailyPSbundleSecond()
            {
                TicketNo = maxID,
                PlayerID = playerID,
                PlayerName = function.getPlayerName(playerID),
                ItemName = item.ItemName,
                PromotionName = "Play & Stay Bundle",
                IssuedDate = DateTime.Now.ToString("dd/MM/yyyy"),
                IssuedTime = DateTime.Now.ToString("HH:mm:ss"),
                GamingDate = function.getMFDBCurrentDay().ToString("dd/MM/yyyy"),
                Status = 1,
                Items = entity.DailyPSBundle_Items.Where(i => i.PlayerID == playerID).ToList()
            };
            return dailyLog;
        }

        //New Points Redemption webservices

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<MFPointsRedemption_Items> RedeemPointsFirst()
        {
            ITHoTram_CustomReportEntities entity = new ITHoTram_CustomReportEntities();
            List<MFPointsRedemption_Items> Items = entity.MFPointsRedemption_Items.ToList();
           return Items;
    }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //Change 20230109 Hantt start
        //public RedeemPointsLog RedeemPointsSecond(int itemID, int playerID)
        public RedeemPointsLog RedeemPointsSecond(int itemID, int playerID, string location)
        //Change 20230109 Hantt end
        {
            ITHoTram_CustomReportEntities entity = new ITHoTram_CustomReportEntities();

            MFPointsRedemption_Items item = new MFPointsRedemption_Items();

            MFPointsRedemption_Logs log = new MFPointsRedemption_Logs();

            int ok = 0;
            //int redeemBalance = 0;
            DataSet ds;

            int maxID = 0;
            int playerPoints = function.getPlayerPointsBalance(playerID);
            int currentPlayerPoints = 0;

            item = entity.MFPointsRedemption_Items.SingleOrDefault(x => x.ID == itemID);

            if (playerPoints >= item.ItemPoints)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("Proc_PlayerPointRedeem", con))
                    {
                        con.Open();
                        cmd.Connection = con;
                        cmd.Parameters.Add(new SqlParameter("@@nPlayerID", playerID));
                        cmd.Parameters.Add(new SqlParameter("@@nRedeemPoints", item.ItemPoints));
                        cmd.Parameters.Add(new SqlParameter("@@nReasonNumber", 1));
                        cmd.Parameters.Add(new SqlParameter("@@szComment", "Gift points redeemed for " + playerID.ToString() + " - Amount: " + item.ItemPoints.ToString() + " - ItemName: " + item.ItemName));
                        cmd.Parameters.Add(new SqlParameter("@@nUserID", 31580));
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                    }
                }
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            currentPlayerPoints = Convert.ToInt32(ds.Tables[0].Rows[0]["NewPlayerBalance"].ToString());

                            ok = 1;

                            log.LogName = item.ItemName;

                            log.gamingDate = function.getMFWBCurrentDay();

                            log.createdDate = DateTime.Now.ToString("dd-MM-yyyy");
                            log.createdTime = DateTime.Now.ToString("HH:mm:ss");

                            log.PlayerID = playerID;
                            log.PlayerName = function.getPlayerName(playerID);
                            log.PromotionName = "Points Redemption";
                            log.ItemPoints = item.ItemPoints;
                            log.Status = ok;
                            log.CurrentPlayerPoints = currentPlayerPoints;
                            log.voidedStatus = "printed";
                            //Add 20230109 Hantt start
                            log.Location = location;
                            //Add 20230109 Hantt end;

                            try
                            {
                                entity.MFPointsRedemption_Logs.Add(log);
                                entity.SaveChanges();

                                maxID = log.ID;
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                    }
                }
            }

            else
            {
                ok = 0;
            }

            var redeemPointsLog = new RedeemPointsLog()
            {
                TicketNo = maxID,
                LogName = item.ItemName,
                //Change 20230118 Hantt start
                //issuedDate = DateTime.Now.ToString("dd-MM-yyyy"),
                //issuedTime = DateTime.Now.ToString("HH:mm:ss"),
                IssuedDate = DateTime.Now.ToString("dd-MM-yyyy"),
                IssuedTime = DateTime.Now.ToString("HH:mm:ss"),
                //Change 20230118 Hantt end
                PlayerID = playerID,
                PlayerName = function.getPlayerName(playerID),
                PromotionName = "Points Redemption",
                ItemPoints = (int)item.ItemPoints,
                Status = ok,
                CurrentPlayerPoints = currentPlayerPoints,
                voidedStatus = "printed"
            };
            return redeemPointsLog;
        }

        //Add 20231212 Hantt end
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public RedeemLog8DragonsFirst Redeem8DragonsFirst(int playerID)
        {
            List<MF8DragonBuffetBonus_Items> dragonItemList = new List<MF8DragonBuffetBonus_Items>();
            MF8DragonBuffetBonus_Items dragonItem;
            MF8DragonBuffetBonus_Players dragonPlayer;
            string playerName = function.getPlayerName(playerID);

            DateTime currentDay;
            DateTime newDay;
            DateTime yesterday;
            bool isPass = false;

            //count Item of Player
            int countItems = db.MF8DragonBuffetBonus_Items.Where(item => item.PlayerID == playerID).Count();
            int countPlayers = db.MF8DragonBuffetBonus_Players.Where(item => item.PlayerID == playerID).Count();

            DateTime playerGamingDate = function.getPlayerGamingDateDaily(playerID);
            currentDay = function.getMFDBCurrentDay();
            yesterday = currentDay.AddDays(-1);
            newDay = DateTime.Now;

            //Lấy điểm từ System
            int playerPointsDaily = function.getPlayerPointsDaily(playerID, currentDay);
            int playerPointsYesterday = function.getPlayerPointsDaily(playerID, yesterday);
            int combinedPoints = playerPointsDaily + playerPointsYesterday;
            int balancePoints = function.getPlayerPointsBalance(playerID);

            //Dựa vào điểm, insert Item tương ứng
            isPass = false;
            string tierName = function.getTierName(playerID);


            //Insert or update Player
            if (countPlayers == 0)
            {
                dragonPlayer = new MF8DragonBuffetBonus_Players();
                dragonPlayer.PlayerID = playerID;
                dragonPlayer.PlayerName = playerName;
                dragonPlayer.DailyPoints = playerPointsDaily;
                dragonPlayer.YesterdayPoints = playerPointsYesterday;
                dragonPlayer.CombinedPoints = combinedPoints;
                dragonPlayer.BalancePoints = balancePoints;
                dragonPlayer.GamingDate = playerGamingDate;
                dragonPlayer.Quantity = 3; // maximum 3 ticket
                db.MF8DragonBuffetBonus_Players.Add(dragonPlayer);
            }
            else
            {
                dragonPlayer = db.MF8DragonBuffetBonus_Players.Where(item => item.PlayerID == playerID).FirstOrDefault();
                dragonPlayer.DailyPoints = playerPointsDaily;
                dragonPlayer.YesterdayPoints = playerPointsYesterday;
                dragonPlayer.CombinedPoints = combinedPoints;
                dragonPlayer.BalancePoints = balancePoints;
            }

            if (((tierName == "Classic" || tierName == "Fortune") && combinedPoints >= 20) || ((tierName == "Classic" || tierName == "Fortune") && balancePoints >= 1000))
            {
                isPass = true;
            }

            //update GamingDate
            dragonPlayer.GamingDate = currentDay;

            //Insert or update Items
            if (countItems == 0)
            {
                //Insert Item
                dragonItem = new MF8DragonBuffetBonus_Items();
                dragonItem.ItemName = "8Dragons Buffet Ticket";
                dragonItem.Status = isPass == true ? 1 : 0;
                dragonItem.PlayerID = playerID;
                dragonItem.DateInserted = DateTime.Now;
                db.MF8DragonBuffetBonus_Items.Add(dragonItem);
                db.SaveChanges();
                db.MF8DragonBuffetBonus_Items.Add(dragonItem);
                db.SaveChanges();
                db.MF8DragonBuffetBonus_Items.Add(dragonItem);
                db.SaveChanges();
            }
            //if items existed.
            else
            {
                List<MF8DragonBuffetBonus_Items> itemList = new List<MF8DragonBuffetBonus_Items>();
                itemList = db.MF8DragonBuffetBonus_Items.Where(item => item.PlayerID == playerID).ToList();

                //compare last player gaming date vs current gaming date/yesterday
                foreach (var item in itemList)
                {
                    dragonItem = db.MF8DragonBuffetBonus_Items.Find(item.ID);

                    DateTime dateInsert = item.DateInserted;

                    int currentTime = Int32.Parse(dateInsert.Hour.ToString());
                    if (currentTime < 6)
                    {
                        dateInsert = dateInsert.AddDays(-1);

                    }

                    // if (item.DateInserted < currentDay)

                    if (dateInsert < currentDay)
                    {
                        dragonItem.Status = isPass == true ? 1 : 0;
                        dragonItem.DateInserted = DateTime.Now;
                    }
                    else
                    {
                        if (isPass == true)
                        {
                            if (item.Status == 0)
                            {
                                dragonItem.Status = 1;
                                dragonItem.DateInserted = DateTime.Now;
                            }
                        }
                        else
                        {
                            dragonItem.Status = 0;
                            dragonItem.DateInserted = DateTime.Now;
                        }
                    }
                    db.SaveChanges();
                }


            }

            List<MF8DragonBuffetBonus_Items> list = db.MF8DragonBuffetBonus_Items.Where(item => item.PlayerID == playerID).ToList();
            int count = list.Where(item => item.Status == 1).Count();
            var dragonInfo = new RedeemLog8DragonsFirst()
            {
                PlayerID = playerID,
                PlayerName = playerName,
                PromotionName = "8Dragon Buffet Bonus",
                Points = playerPointsDaily,
                GamingDate = currentDay.ToString("dd/MM/yyyy"),
                Items = list,
                EnableCount = count,
            };

            return dragonInfo;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public RedeemLog8DragonsSecond Redeem8DragonsSecond(int playerID, int itemID, string location)
        {
            MF8DragonBuffetBonus_Items dragonItem = new MF8DragonBuffetBonus_Items();
            List<ItemsDaily> listItemsDaily = new List<ItemsDaily>();


            DataSet ds;
            int maxID = 0;
            int ok = 0;
            string playerName = function.getPlayerName(playerID);
            MF8DragonBuffetBonus_Logs log = new MF8DragonBuffetBonus_Logs();


            if (playerID > 0 )
            {
                if (itemID > 0)
                {
                    dragonItem = db.MF8DragonBuffetBonus_Items.Where(item => item.PlayerID == playerID && item.ID == itemID).FirstOrDefault();
                }else
                {
                    //Print from Casinoloyaltykiosk not Promotion
                    dragonItem = db.MF8DragonBuffetBonus_Items.Where(item => item.PlayerID == playerID && item.Status == 1).FirstOrDefault();
                }
                
            }

            DateTime currentDay;
            DateTime yesterday;

            currentDay = function.getMFDBCurrentDay();
            yesterday = currentDay.AddDays(-1);
            int playerPointsDaily = function.getPlayerPointsDaily(playerID, currentDay);
            int playerPointYesterday = function.getPlayerPointsDaily(playerID, yesterday);
            int balancePoint = function.getPlayerPointsBalance(playerID);
            //insert items and players here.

            if (dragonItem.ID > 0)
            {
                dragonItem.Status = 2;

                log.ID = dragonItem.ID;
                log.PlayerID = playerID;
                log.PlayerName = playerName;

                log.PromotionName = "8Dragon Buffet Bonus";
                log.DailyPoints = playerPointsDaily;
                log.YesterdayPoints = playerPointYesterday;
                log.CombinedPoints = playerPointsDaily + playerPointYesterday;
                log.BalancePoints = balancePoint;
                log.GamingDate = function.getPlayerGamingDateDaily(playerID).ToString("dd/MM/yyyy");

                log.ItemID = dragonItem.ID;
                log.ItemName = dragonItem.ItemName;
                log.IssueDate = DateTime.Now.ToString("dd/MM/yyyy");
                log.IssueTime = DateTime.Now.ToString("HH:mm:ss");
                log.Location = location;
                log.voidedStatus = "printed";

                //update GamingDate
                MF8DragonBuffetBonus_Players dragonPlayer = db.MF8DragonBuffetBonus_Players.Where(item => item.PlayerID == playerID).FirstOrDefault();
                dragonPlayer.GamingDate = currentDay;
                dragonPlayer.Quantity = dragonPlayer.Quantity - 1;


                try
                {
                    ok = 1;
                    db.MF8DragonBuffetBonus_Logs.Add(log);
                    db.SaveChanges();
                    maxID = log.ID;

                    function.insertUpdateItemsYesterdayLog(playerID, DateTime.Now, "ItemID: " + dragonItem.ID + " has been redeemed at" + DateTime.Now.ToString("HH:mm:ss"));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            var dailyLog = new RedeemLog8DragonsSecond()
            {
                TicketNo = maxID,
                PlayerID = playerID,
                PlayerName = function.getPlayerName(playerID),
                ItemName = dragonItem.ItemName,
                PromotionName = "8Dragon Buffet Bonus",
                TodayPoints = playerPointsDaily,
                YesterdayPoints = playerPointYesterday,
                BalancePoints = balancePoint,
                IssuedDate = DateTime.Now.ToString("dd/MM/yyyy"),
                IssuedTime = DateTime.Now.ToString("HH:mm:ss"),
                GamingDate = log.GamingDate,
                Status = ok,
                Items = db.MF8DragonBuffetBonus_Items.Where(item => item.PlayerID == playerID).ToList()
            };
            return dailyLog;
        }

        //Add 20231212 Hantt end

        //Daily
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public RedeemLogDailyFirst RedeemDailyFirst(int playerID)
        {
            ItemDetail itemDetail = new ItemDetail();
            List<ItemDetail> listItemDetail = new List<ItemDetail>();

            string playerName = function.getPlayerName(playerID);

            DateTime currentDay;
            DateTime newDay;
            DateTime yesterday;
            int countItems = function.getItemsRowCountDaily(playerID); //10
            int countPlayers = function.getPlayersRowCountDaily(playerID);

            //06/07/2023
            DateTime playerGamingDate = function.getPlayerGamingDateDaily(playerID);

            //cur 26
            currentDay = function.getMFDBCurrentDay();
            //yes
            yesterday = currentDay.AddDays(-1);
            newDay = DateTime.Now;

            //Lấy điểm từ System
            //int playerPointsDaily = function.getTwoDaysPoints(playerID, yesterday, currentDay);
            int playerPointsDaily = function.getPlayerPointsDaily(playerID, currentDay);//91 points

            int playerPointsYesterday = function.getPlayerPointsDaily(playerID, yesterday);//10699 points


            //Dựa vào điểm, insert Item tương ứng
            //insert items and players here.
            int dailyPoints = function.getItemPointsDaily(playerPointsDaily);//8000

            int yesterdayPoints = function.getItemPointsDaily(playerPointsYesterday);//0

            if (countPlayers == 0)
            {
                function.insertPlayersDaily(playerID, playerName, playerPointsDaily, 0, 0, currentDay);
            }
            else
            {
                function.updatePlayerDailyPoints(playerID, playerPointsDaily);
            }

            //if items not existed.
            if (countItems == 0)
            {
                function.insertUpdateItemsYesterdayLog(playerID, DateTime.Now, "Last actived Date: " + playerGamingDate);
                function.updatePlayerGamingDate(playerID, currentDay);
                switch (dailyPoints)
                {
                    case 1000:
                        function.insertItemsDaily(1000, playerID);
                        break;
                    case 2000:
                        function.insertItemsDaily(2000, playerID);
                        break;
                    case 4000:
                        function.insertItemsDaily(4000, playerID);
                        break;
                    case 6000:
                        function.insertItemsDaily(6000, playerID);
                        break;
                    case 8000:
                        function.insertItemsDaily(8000, playerID);
                        break;
                    //Add 20230619 Hantt start
                    case 20000:
                        function.insertItemsDaily(20000, playerID);
                        break;
                    //Add 20230619 Hantt end
                    default:
                        function.insertItemsDaily(0, playerID);
                        break;
                }
            }
            //if items existed.
            else
            {
                //compare last player gaming date vs current gaming date/yesterday

                function.insertUpdateItemsYesterdayLog(playerID, DateTime.Now, "CurrentDate: " + currentDay.ToString("dd/MM/yyyy") + " itemPoints: " + dailyPoints.ToString());
                function.insertUpdateItemsYesterdayLog(playerID, DateTime.Now, "GamingDate: " + playerGamingDate.ToString("dd/MM/yyyy"));
                if (playerGamingDate < currentDay)
                {
                    if (playerGamingDate == yesterday)
                    {
                        //Move Items from Today to Yesterday
                        //function.updateItemDailyStatusYesterday(playerID, playerGamingDate);
                        function.updateItemDailyStatusYesterday(playerID);
                        function.updateItemDailyStatusAll(playerID, 0);
                        function.updatePlayerGamingDate(playerID, currentDay);

                        //WriteLog
                        function.insertUpdateItemsYesterdayLog(playerID, DateTime.Now, "YesterDay: " + yesterday.ToString("dd/MM/yyyy") + " itemPoints: " + yesterdayPoints.ToString());
                        List<MFDailyBonus_Items_Yesterday> yesterdayList = db.MFDailyBonus_Items_Yesterdays.Where(item => item.PlayerID == playerID).ToList();
                        yesterdayList.ForEach(item =>
                        {
                            function.insertUpdateItemsYesterdayLog(playerID, DateTime.Now, "ItemName: " + item.ItemName + " Status: " + item.Status.ToString());
                        });
                    }
                    else
                    {
                        function.insertUpdateItemsYesterdayLog(playerID, DateTime.Now, "NotYesterDay: " + currentDay.ToString("dd/MM/yyyy"));
                        function.updateItemDailyStatusYesterdayAll(playerID, 0);

                        function.updateItemDailyStatusAll(playerID, 0);
                        function.updatePlayerGamingDate(playerID, currentDay);
                    }
                }


                if (dailyPoints > 0)
                {
                    this.function.updateItemDailyStatusNew(playerID, dailyPoints);
                }
                else
                {
                    this.function.updateItemDailyStatusAll(playerID, 0);
                }

            }
            var dailyLog = new RedeemLogDailyFirst()
            {
                PlayerID = playerID,
                PlayerName = function.getPlayerName(playerID),
                PromotionName = "Mass Floor Daily Bonus",
                Points = playerPointsDaily,
                GamingDate = currentDay.ToString("dd/MM/yyyy"),
                Items = GetItemListDailyByPlayerID(playerID)
            };

            return dailyLog;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //Change 20230109 Hantt start
        //public RedeemLogDailySecond RedeemDailySecond(int playerID, int itemID, string type)
        public RedeemLogDailySecond RedeemDailySecond(int playerID, int itemID, string location, string type)
        //Change 20230109 Hantt end
        {

            ItemsDaily itemsDaily = new ItemsDaily();
            ItemDetail itemDetail = new ItemDetail();
            List<ItemsDaily> listItemsDaily = new List<ItemsDaily>();


            DataSet ds;
            int maxID = 0;
            int ok = 0;
            //int currentPlayerPoints = 0;

            string playerName = function.getPlayerName(playerID);

            MFDailyBonus_SecondLogs log = new MFDailyBonus_SecondLogs();


            if (playerID > 0 && itemID > 0)
            {
                itemDetail = GetRedeemItemDetailDailyByID(playerID, itemID, type);
            }

            string name2 = "";
            string name1 = "";

            DateTime currentDay;
            DateTime yesterday;

            currentDay = function.getMFDBCurrentDay();
            yesterday = currentDay.AddDays(-1);
            //int playerPointsDaily = function.getTwoDaysPoints(playerID,yesterday, currentDay);
            int playerPointsDaily = function.getPlayerPointsDaily(playerID, currentDay);

            //insert items and players here.
            int dailyPoints = function.getItemPointsDaily(playerPointsDaily);

            if (itemDetail.ID > 0)
            {
                function.updateItemDailyStatusSecondNew(playerID, itemDetail.ItemPoints, 0, type);
                function.updateItemDailyStatusSecond(playerID, itemDetail.ID, 2, type);
                itemDetail = GetRedeemItemDetailDailyByID(playerID, itemID, type);


                log.ID = itemDetail.ID;
                log.PlayerID = playerID;
                log.PlayerName = playerName;

                log.PromotionName = "Mass Floor Daily Bonus";
                log.DailyPoints = playerPointsDaily;
                log.SlotDailyPoints = 0;
                log.TableDailyPoints = 0;
                if (type == "T")
                {
                    log.Status = function.getStatusItemDaily(itemDetail.ID, playerID);
                    log.GamingDate = currentDay.ToString("dd/MM/yyyy");
                }
                else
                {
                    log.Status = function.getStatusItemYesterday(itemDetail.ID, playerID);
                    log.GamingDate = yesterday.ToString("dd/MM/yyyy");
                }

                log.ItemID = itemDetail.ID;
                log.ItemName = itemDetail.ItemName;
                log.Type = itemDetail.Type;
                log.ItemPoints = itemDetail.ItemPoints;
                log.IssueDate = DateTime.Now.ToString("dd/MM/yyyy");
                log.IssueTime = DateTime.Now.ToString("HH:mm:ss");
                //Change 20230109 Hantt start
                log.Location = location;
                //Change 20230109 Hantt end
                log.voidedStatus = "printed";

                function.updatePlayerGamingDate(playerID, currentDay);

                try
                {
                    ok = 1;
                    db.MFDailyBonus_SecondLogs.Add(log);
                    db.SaveChanges();
                    maxID = log.ID;

                    function.insertUpdateItemsYesterdayLog(playerID, DateTime.Now, "ItemID: " + itemDetail.ID + " has been redeemed at" + DateTime.Now.ToString("HH:mm:ss"));

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            ds = function.setItemNameOnPoints(itemDetail.ItemPoints, itemDetail.ItemName);

            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        name1 = ds.Tables[0].Rows[0]["name1"].ToString();
                        name2 = ds.Tables[0].Rows[0]["name2"].ToString();
                    }
                }
            }
            var dailyLog = new RedeemLogDailySecond()
            {

                TicketNo = maxID,
                PlayerID = playerID,
                PlayerName = function.getPlayerName(playerID),
                ItemName = name1,
                ItemNameFB = name2,
                PromotionName = "Mass Floor Daily Bonus",
                Points = playerPointsDaily,
                IssuedDate = DateTime.Now.ToString("dd/MM/yyyy"),
                IssuedTime = DateTime.Now.ToString("HH:mm:ss"),
                GamingDate = log.GamingDate,
                Status = ok,
                Items = GetItemListDailyByPlayerID(playerID)
            };

            // add by Doc Ly 2021/05/26: insert to fb item
            //insertFBTicket(dailyLog.TicketNo, dailyLog.PlayerID, dailyLog.ItemNameFB, "test", log.ItemPoints.ToString());

            return dailyLog;
        }


        public List<ItemsDaily> GetItemListDailyByPlayerID(int ID)
        {
            List<ItemsDaily> listItemsDaily = new List<ItemsDaily>();
            DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFDailyBonus_spGetItemsDailyByPlayerID", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", ID));
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        ds = new DataSet();
                        da.Fill(ds);
                    }
                }
            }
            try
            {
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                listItemsDaily.Add(new ItemsDaily()
                                {
                                    ID = Convert.ToInt32(dr["ID"]),
                                    ItemName = dr["ItemName"].ToString(),
                                    ItemPoints = Convert.ToInt32(dr["ItemPoints"].ToString()),
                                    PlayerID = Convert.ToInt32(dr["PlayerID"].ToString()),
                                    Status = Convert.ToInt32(dr["Status"].ToString()),
                                    YStatus = Convert.ToInt32(dr["YStatus"].ToString()),
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listItemsDaily;
        }

        public ItemDetail GetRedeemItemDetailDailyByID(int playerID, int itemID, string type)
        {
            ItemDetail itemDetail = new ItemDetail();
            DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFDailyBonus_spGetRedeemItemsDailyByID", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@ItemID", itemID));
                    cmd.Parameters.Add(new SqlParameter("@Type", type));
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        ds = new DataSet();
                        da.Fill(ds);
                    }
                }
            }
            try
            {
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = ds.Tables[0].Rows[0];

                            itemDetail.ID = Convert.ToInt32(dr["ID"]);
                            itemDetail.ItemName = dr["ItemName"].ToString();
                            itemDetail.ItemPoints = Convert.ToInt32(dr["ItemPoints"].ToString());
                            itemDetail.Status = Convert.ToInt32(dr["Status"].ToString());
                            itemDetail.Type = dr["Type"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return itemDetail;
        }

        // Mass Floor Daily Slot Webservices

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<MFBonus_SlotDailyItems> RedeemPointsSlotFirst(int playerID)
        {
            string playerName = function.getPlayerName(playerID);
            //int playerPoints = function.getPlayerPointsBalance(playerID);
            DateTime currentDay;
            currentDay = function.getMFDBCurrentDay();
            int slotDailyPoints = function.getSlotPointsDaily(playerID, currentDay);

            ITHoTram_CustomReportEntities entity = new ITHoTram_CustomReportEntities();
            MFBonus_SlotDailyPlayers player = new MFBonus_SlotDailyPlayers();

            //int count = function.getPlayersRowCountSlot(playerID);

            player = entity.MFBonus_SlotDailyPlayers.SingleOrDefault(x => x.PlayerID == playerID);
            int redeemPoints = 0;

            redeemPoints = function.getSlotPointsRedeemed(playerID, currentDay);

            if (player == null)
            {
                MFBonus_SlotDailyPlayers p = new MFBonus_SlotDailyPlayers();

                p.PlayerID = playerID;
                p.PlayerName = playerName;
                p.SlotDailyPoints = slotDailyPoints;
                p.TableDailyPoints = 0;
                p.vSlotDailyPoints = slotDailyPoints;
                p.vTableDailyPoints = 0;
                p.GamingDate = currentDay;
                p.RedeemedSlotPoints = 0;
                entity.MFBonus_SlotDailyPlayers.Add(p);
                entity.SaveChanges();
            }
            else
            {
                if (player.GamingDate < currentDay)
                {
                    player.GamingDate = currentDay;
                    player.vSlotDailyPoints = slotDailyPoints;
                    player.RedeemedSlotPoints = 0;
                    player.SlotDailyPoints = slotDailyPoints;
                    entity.SaveChanges();
                }
                else
                {

                    player.SlotDailyPoints = slotDailyPoints;
                    player.vSlotDailyPoints = slotDailyPoints - redeemPoints;
                    player.RedeemedSlotPoints = redeemPoints;
                    entity.SaveChanges();
                }
            }

            var itemList = entity.MFBonus_SlotDailyItems.ToList();

            return itemList;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public DailyRedeemSlotLog RedeemPointsSlotSecond(int itemID, int playerID)
        {
            ITHoTram_CustomReportEntities entity = new ITHoTram_CustomReportEntities();

            MFBonus_SlotDailyItems item = new MFBonus_SlotDailyItems();

            MFBonus_SlotDailyPlayers player = new MFBonus_SlotDailyPlayers();

            MFBonus_SlotDailyLogs log = new MFBonus_SlotDailyLogs();

            DateTime currentDay;

            currentDay = function.getMFDBCurrentDay();

            int ok = 0;

            int maxID = 0;
            int slotDailyPoints = function.getSlotPointsDaily(playerID, currentDay);
            int currentSlotPoints = 0;

            item = entity.MFBonus_SlotDailyItems.SingleOrDefault(x => x.ID == itemID);

            player = entity.MFBonus_SlotDailyPlayers.SingleOrDefault(x => x.PlayerID == playerID);

            int redeemedPoints = player.RedeemedSlotPoints;
            int vSlotPoints = player.vSlotDailyPoints;
            int itemPoints = item.ItemPoints;

            if (vSlotPoints > itemPoints)
            {
                ok = 1;

                currentSlotPoints = vSlotPoints - itemPoints;

                player.vSlotDailyPoints = currentSlotPoints;
                player.GamingDate = currentDay;
                player.RedeemedSlotPoints = redeemedPoints + itemPoints;

                log.ItemID = item.ID;
                log.ItemName = item.ItemName;
                log.ItemPoints = item.ItemPoints;
                log.IssueDate = DateTime.Now.ToString("dd-MM-yyyy");
                log.IssueTime = DateTime.Now.ToString("HH:mm:ss");
                log.PlayerID = playerID;
                log.PlayerName = player.PlayerName;
                log.PromotionName = "Slot Daily Points Redemption";
                log.SlotDailyPoints = slotDailyPoints;
                log.Status = ok;
                log.vSlotDailyPoints = currentSlotPoints;

                entity.MFBonus_SlotDailyLogs.Add(log);
                entity.SaveChanges();

                maxID = log.ID;
            }

            else
            {
                ok = 0;
            }

            var redeemPointsLog = new DailyRedeemSlotLog()
            {
                TicketNo = maxID,
                LogName = item.ItemName,
                issuedDate = DateTime.Now.ToString("dd-MM-yyyy"),
                issuedTime = DateTime.Now.ToString("HH:mm"),
                PlayerID = playerID,
                PlayerName = player.PlayerName,
                PromotionName = "Slot Daily Points Redemption",
                ItemPoints = item.ItemPoints,
                Status = ok,
                DailySlotPoints = slotDailyPoints,
                vDailySlotPoints = currentSlotPoints,

            };
            return redeemPointsLog;
        }

        // Mass Floor Weekly Bonus Webservices

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public RedeemLogWeeklyFirst RedeemWeeklyFirst(int playerID)
        {
            string playerName = function.getPlayerName(playerID);

            int countItems = function.getItemsRowCountWeekly(playerID);
            int countPlayers = function.getPlayersRowCountWeekly(playerID);

            //TimeSpan ts = new TimeSpan(0, 0, 0);
            DateTime playerGamingDate = function.getMFWBCurrentDay();

            ////Get last Thursday
            //DateTime lastThursday = DateTime.Now;
            //while (lastThursday.DayOfWeek != DayOfWeek.Thursday)
            //    lastThursday = lastThursday.AddDays(-1);
            //if (lastThursday.Hour < 6)
            //{
            //    lastThursday = lastThursday.AddDays(-1);

            //    while (lastThursday.DayOfWeek != DayOfWeek.Thursday)
            //        lastThursday = lastThursday.AddDays(-1).Date;


            //    lastThursday = lastThursday.Date + ts;
            //}

            ////TimeSpan ts = new TimeSpan(12, 0, 0);
            //lastThursday = lastThursday.Date + ts;

            ////Get last Saturday
            //DateTime lastSaturday = DateTime.Today;
            //while (lastSaturday.DayOfWeek != DayOfWeek.Saturday)
            //    lastSaturday = lastSaturday.AddDays(-1);

            int playerPointsWeekly = function.getPlayerPointsWeekly(playerID);

            //insert items and players here.
            int weeklyPoints = function.getItemPointsWeekly(playerPointsWeekly);

            if (countPlayers == 0)
            {
                function.insertPlayersWeekly(playerID, playerName, playerPointsWeekly, 0, 0, playerGamingDate);
            }

            if (countPlayers > 0)
            {
                function.updatePlayerWeeklyPoints(playerID, playerPointsWeekly);
            }

            //if items not existed.
            if (countItems == 0)
            {
                function.switchWeeklyPoints(weeklyPoints, playerID);

                DataSet ds1 = function.getItemIDByPointsWeekly(weeklyPoints, playerID);
                int count = ds1.Tables[0].Rows.Count;

                if (ds1 != null && count > 0)
                {
                    //get itemID SBV + SFP
                    int itemID1 = Convert.ToInt32(ds1.Tables[0].Rows[0]["ID"].ToString());
                    //get itemID SBV
                    int itemID2 = Convert.ToInt32(ds1.Tables[0].Rows[1]["ID"].ToString());
                    if (itemID1 > 1)
                    {
                        DataSet ds = function.getTopItemIDByPIDWeekly(playerID);

                        for (int i = Convert.ToInt32(ds.Tables[0].Rows[0]["ID"].ToString()); i <= itemID1 - 1; i++)
                        {
                            function.updateItemWeeklyStatusByID(3, i);
                        }

                        //Del 20230719 Hantt start
                        for (int j = Convert.ToInt32(ds.Tables[0].Rows[5]["ID"].ToString()); j <= itemID2 - 1; j++)
                        {
                            function.updateItemWeeklyStatusByID(3, j);
                        }
                        //Del 20230719 Hantt end
                    }
                }
            }

            //if items existed.
            if (countItems > 0)
            {
                DataSet ds2 = function.getStatusItemWeeklyByPID(playerID);
                bool flag = false;

                for (int i = 0; i <= Convert.ToInt32(ds2.Tables[0].Rows.Count - 1); i++)
                {
                    if (Convert.ToInt32(ds2.Tables[0].Rows[i]["Status"].ToString()) == 2)
                    {
                        flag = true;
                        break;
                    }

                }

                DateTime currenDay = function.getMFWBCurrentDay();
                function.updatePlayerGamingDateWeekly(playerID, currenDay);

                if (!flag)
                {

                    function.switchWeeklyStatus(weeklyPoints, playerID);

                    DataSet ds1 = function.getItemIDByPointsWeekly(weeklyPoints, playerID);
                    int count = ds1.Tables[0].Rows.Count;
                    if (ds1 != null && count > 0)
                    {

                        int itemID1 = Convert.ToInt32(ds1.Tables[0].Rows[0]["ID"].ToString());
                        int itemID2 = Convert.ToInt32(ds1.Tables[0].Rows[1]["ID"].ToString());

                        if (itemID1 > 1)
                        {
                            DataSet ds = function.getTopItemIDByPIDWeekly(playerID);

                            for (int i = Convert.ToInt32(ds.Tables[0].Rows[0]["ID"].ToString()); i <= itemID1 - 1; i++)
                            {
                                function.updateItemWeeklyStatusByID(3, i);

                            }

                            //Del 20230719 Hantt start
                            for (int j = Convert.ToInt32(ds.Tables[0].Rows[5]["ID"].ToString()); j <= itemID2 - 1; j++)
                            {
                                function.updateItemWeeklyStatusByID(3, j);
                            }
                            //Del 20230719 Hantt start
                        }
                    }
                }
            }

            //Write Log Weekly
            string path = ConfigurationManager.AppSettings["pathWeeklyLog"];



            int points = function.getPlayerPointsWeekly(playerID);
            string gamingDate = playerGamingDate.ToString("dd/MM/yyyy");
            List<ItemDetail> lstItem = new List<ItemDetail>();


            //WriteLogWeekly
            string filename = DateTime.Now.ToString("yyyy_MM_dd");
            string strpath = path + "Weekly_" + filename + ".txt";
            try
            {

                FileStream stream = null;
                if (File.Exists(strpath))
                {
                    stream = new FileStream(strpath, FileMode.Append, FileAccess.Write);
                }
                else
                {
                    stream = new FileStream(strpath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                }



                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine("======================START:  " + DateTime.Now + "   =================" + "\n");
                    writer.WriteLine("PlayerID: " + playerID);
                    writer.WriteLine("GamingDate: " + gamingDate);
                    writer.WriteLine("Points: " + points.ToString());

                    lstItem = GetItemListWeeklyByPlayerID(playerID, writer);
                    List<ItemDetail> lstItemFirst = new List<ItemDetail>();
                    List<ItemDetail> lstItemSecond = new List<ItemDetail>();
                    int size = lstItem.Count();
                    lstItemFirst = lstItem.GetRange(0, size/2);
                    lstItemSecond = lstItem.GetRange(size / 2, size / 2);

                    lstItemSecond.AddRange(lstItemFirst);
                    lstItem = lstItemSecond;
                    writer.WriteLine("========================END==========================================" + "\n");
                    writer.WriteLine("\n\n");
                }


            }
            catch (Exception exp)
            {
                Console.Write(exp.Message);
            }


            var weeklyLog = new RedeemLogWeeklyFirst()
            {
                PlayerID = playerID,
                PlayerName = function.getPlayerName(playerID),
                PromotionName = "Weekly Bonus",
                Points = points,
                GamingDate = gamingDate,
                Items = lstItem
            };


            return weeklyLog;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //Change 20230109 Hantt start
        //public RedeemLogWeeklySecond RedeemWeeklySecond(int playerID, int itemID)
        public RedeemLogWeeklySecond RedeemWeeklySecond(int playerID, int itemID, string location)
        //Change 20230109 Hantt end
        {
            int maxID = 0;
            int ok = 0;
            //int currentPlayerPoints = 0;
            ItemDetail itemDetail = new ItemDetail();

            string playerName = function.getPlayerName(playerID);

            MFWeeklyBonus_Logs log = new MFWeeklyBonus_Logs();

            ////Get last Thursday
            //DateTime lastThursday = DateTime.Today;
            //while (lastThursday.DayOfWeek != DayOfWeek.Thursday)
            //    lastThursday = lastThursday.AddDays(-1);

            ////Get last Saturday
            //DateTime lastSaturday = DateTime.Today;
            //while (lastSaturday.DayOfWeek != DayOfWeek.Saturday)
            //    lastSaturday = lastSaturday.AddDays(-1);

            int weeklyPoints = function.getPlayerPointsWeekly(playerID);

            if (playerID > 0 && itemID > 0)
            {
                itemDetail = GetRedeemItemDetailWeeklyByID(playerID, itemID);
            }

            if (itemDetail.ID > 0)
            {

                function.updateItemWeeklyStatusFirst(playerID, 0, itemDetail.ItemPoints);
                function.updateItemWeeklyStatusSecond(playerID, itemDetail.ID, 2);

                itemDetail = GetRedeemItemDetailWeeklyByID(playerID, itemID);
                log.ID = itemDetail.ID;
                log.PlayerID = playerID;
                log.PlayerName = playerName;
                //Add 20230109 Hantt start
                log.Location = location;
                //Add 20230109 Hantt end

                log.PromotionName = "Weekly Bonus";
                log.WeeklyPoints = weeklyPoints;
                log.SlotWeeklyPoints = 0;
                log.TableWeeklyPoints = 0;
                log.Status = function.getStatusItemWeekly(itemDetail.ID, playerID);
                log.ItemID = itemDetail.ID;
                log.ItemName = itemDetail.ItemName;
                log.ItemPoints = itemDetail.ItemPoints;
                log.IssueDate = DateTime.Now.ToString("dd/MM/yyyy");
                log.IssueTime = DateTime.Now.ToString("HH:mm:ss");
                log.GamingDate = function.getPlayerGamingDateWeekly(playerID).ToString();
                log.voidedStatus = "printed";
                ok = 1;
                try
                {
                   // db.MFWeeklyBonus_Logs.Add(log);
                    db.SaveChanges();
                    maxID = log.ID;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            string itemName = itemDetail.ItemName;
            //Add 20230619 Hantt start
            //int idxCharAdd = itemName.IndexOf("+");
            //int idxCharSub = itemName.IndexOf("-");
            //string sbvName = itemName.Substring(0, idxCharAdd - 1);
            //string sfpName = itemName.Substring(idxCharAdd + 1, idxCharSub - idxCharAdd - 2);
            //string points = itemName.Substring(idxCharSub + 1, itemName.Length - idxCharSub - 1);
            //Add 20230619 Hantt end

            List<ItemDetail> lstItem = GetItemListWeeklyByPlayerID(playerID, null);
            List<ItemDetail> lstItemFirst = new List<ItemDetail>();
            List<ItemDetail> lstItemSecond = new List<ItemDetail>();
            int size = lstItem.Count();
            lstItemFirst = lstItem.GetRange(0, size / 2);
            lstItemSecond = lstItem.GetRange(size / 2, size / 2);

            lstItemSecond.AddRange(lstItemFirst);

            var weeklyLog = new RedeemLogWeeklySecond()
            {

                TicketNo = maxID,
                PlayerID = playerID,
                PlayerName = function.getPlayerName(playerID),
                //Change 20230619 Hantt start
                ItemName = itemName,
                //ItemNameSBV = sbvName + " - " + points,
                //ItemNameSFP = sfpName + " - " + points,
                //Add 20230619 Hantt end
                PromotionName = "Weekly Bonus",
                Points = weeklyPoints,
                IssuedDate = DateTime.Now.ToString("dd/MM/yyyy"),

                IssuedTime = DateTime.Now.ToString("HH:mm:ss"),
                Status = ok,
                Items = lstItemSecond
            };

            return weeklyLog;
        }
        public List<ItemDetail> GetItemListWeeklyByPlayerID(int ID, StreamWriter writer)
        {
            List<ItemDetail> listItemDetail = new List<ItemDetail>();
            DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFWeeklyBonus_spGetItemsByPlayerID", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", ID));
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {

                        ds = new DataSet();
                        da.Fill(ds);
                    }
                }
            }
            try
            {
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            if (writer != null)
                            {
                                writer.Write("All Item Detail \n");
                            }
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {

                                int id = Convert.ToInt32(dr["ID"]);
                                string itemName = dr["ItemName"].ToString();


                                int status = Convert.ToInt32(dr["Status"].ToString());

                                listItemDetail.Add(new ItemDetail()
                                {
                                    ID = id,
                                    ItemName = itemName,
                                    ItemPoints = Convert.ToInt32(dr["ItemPoints"].ToString()),
                                    Status = status,

                                });

                                //Write Weely Log
                                if (writer != null)
                                {
                                    writer.Write("ItemName: " + itemName + " Status: " + status.ToString() + "\n");
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listItemDetail;
        }
        public ItemDetail GetRedeemItemDetailWeeklyByID(int playerID, int itemID)
        {
            ItemDetail itemDetail = new ItemDetail();
            DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFWeeklyBonus_spGetRedeemItemsByID", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@ItemID", itemID));
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {

                        ds = new DataSet();
                        da.Fill(ds);
                    }
                }
            }
            try
            {
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = ds.Tables[0].Rows[0];

                            itemDetail.ID = Convert.ToInt32(dr["ID"]);
                            itemDetail.ItemName = dr["ItemName"].ToString();
                            itemDetail.ItemPoints = Convert.ToInt32(dr["ItemPoints"].ToString());
                            itemDetail.Status = Convert.ToInt32(dr["Status"].ToString());
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return itemDetail;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public Points GetPoints(int playerID)
        {
            Points p = new Points();

            int dailyPoints = 0;
            int weeklyPoints = 0;
            int fridayPoints = 0;
            int balancePoints = 0;
            int count = 0;

            //Get last Thursday
            DateTime lastThursday = DateTime.Now;
            while (lastThursday.DayOfWeek != DayOfWeek.Thursday)
                lastThursday = lastThursday.AddDays(-1);
            if (lastThursday.Hour < 6)
            {
                lastThursday = lastThursday.AddDays(-1);

                while (lastThursday.DayOfWeek != DayOfWeek.Thursday)
                    lastThursday = lastThursday.AddDays(-1).Date;
            }
            //Get last Saturday
            DateTime lastSaturday = DateTime.Today;
            while (lastSaturday.DayOfWeek != DayOfWeek.Saturday)
                lastSaturday = lastSaturday.AddDays(-1);

            //Get last Wednesday
            DateTime lastWednesday = DateTime.Today;
            while (lastWednesday.DayOfWeek != DayOfWeek.Wednesday)
                lastWednesday = lastWednesday.AddDays(-1);

            //Get last Sunday
            DateTime lastSunday = DateTime.Today;
            while (lastSunday.DayOfWeek != DayOfWeek.Sunday)
                lastSunday = lastSunday.AddDays(-1);

            DateTime currentDay = function.getMFDBCurrentDay();
            DateTime yesterday = currentDay.AddDays(-1);
            DateTime gamingDate = function.getMFWBCurrentDay();

            count = function.getPlayerPointsRowCount(playerID);

            //dailyPoints = function.getTwoDaysPoints(playerID, yesterday, currentDay);
            dailyPoints = function.getPlayerPointsDaily(playerID, currentDay);
            weeklyPoints = function.getPlayerPointsWeekly(playerID);
            fridayPoints = function.getPlayerPointsFriday(playerID, lastSunday, lastWednesday);
            balancePoints = function.getPlayerPointsBalance(playerID);

            if (count > 0)
            {
                function.updatePlayerPoints(playerID, dailyPoints, weeklyPoints, fridayPoints, gamingDate);
            }
            else
            {
                function.insertPlayersPoints(playerID, dailyPoints, weeklyPoints, fridayPoints, gamingDate);
            }

            p.DailyPoints = dailyPoints;
            p.WeeklyPoints = weeklyPoints;
            p.FridayPoints = fridayPoints;
            p.BalancePoints = balancePoints;
            p.CombinePoints = dailyPoints + function.getPlayerPointsDaily(playerID, yesterday);
            p.PromotionEnd = function.getPromotionEnd();
            return p;
        }

        // Mass Floor Lucky Friday Webservices
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public RedeemLogFridayFirst RedeemFridayFirst(int playerID)
        {
            string playerName = function.getPlayerName(playerID);

            int countItems = function.getItemsRowCountFriday(playerID);
            int countPlayers = function.getPlayersRowCountFriday(playerID);

            DateTime playerGamingDate = function.getMFWBCurrentDay();

            //Get last Wednesday
            DateTime lastWednesday = DateTime.Today;
            while (lastWednesday.DayOfWeek != DayOfWeek.Wednesday)
                lastWednesday = lastWednesday.AddDays(-1);

            //Get last Sunday
            DateTime lastSunday = DateTime.Today;
            while (lastSunday.DayOfWeek != DayOfWeek.Sunday)
                lastSunday = lastSunday.AddDays(-1);

            int playerPointsFriday = function.getPlayerPointsFriday(playerID, lastSunday, lastWednesday);

            //insert items and players here.
            int fridayPoints = function.getItemPointsFriday(playerPointsFriday);

            if (countPlayers == 0)
            {
                function.insertPlayersFriday(playerID, playerName, playerPointsFriday, 0, 0, playerGamingDate);
            }

            if (countPlayers > 0)
            {
                function.updatePlayerFridayPoints(playerID, playerPointsFriday);
            }

            //if items not existed.
            if (countItems == 0)
            {

                function.switchFridayPoints(fridayPoints, playerID);

                DataSet ds1 = function.getItemIDByPointsFriday(fridayPoints, playerID);
                int count = ds1.Tables[0].Rows.Count;

                if (ds1 != null && count > 0)
                {

                    int itemID1 = Convert.ToInt32(ds1.Tables[0].Rows[0]["ID"].ToString());
                    int itemID2 = Convert.ToInt32(ds1.Tables[0].Rows[1]["ID"].ToString());
                    if (itemID1 > 1)
                    {
                        DataSet ds = function.getTopItemIDByPIDFriday(playerID);

                        for (int i = Convert.ToInt32(ds.Tables[0].Rows[0]["ID"].ToString()); i <= itemID1 - 1; i++)
                        {
                            function.updateItemFridayStatusByID(3, i);


                        }

                        for (int j = Convert.ToInt32(ds.Tables[0].Rows[14]["ID"].ToString()); j <= itemID2 - 1; j++)
                        {

                            function.updateItemFridayStatusByID(3, j);
                        }
                    }


                }
            }

            //if items existed.
            if (countItems > 0)
            {
                DateTime currenDay = function.getMFWBCurrentDay();
                function.updatePlayerGamingDateFriday(playerID, currenDay);

                function.switchFridayStatus(fridayPoints, playerID);

                DataSet ds1 = function.getItemIDByPointsFriday(fridayPoints, playerID);
                int count = ds1.Tables[0].Rows.Count;
                if (ds1 != null && count > 0)
                {

                    int itemID1 = Convert.ToInt32(ds1.Tables[0].Rows[0]["ID"].ToString());
                    int itemID2 = Convert.ToInt32(ds1.Tables[0].Rows[1]["ID"].ToString());
                    if (itemID1 > 1)
                    {
                        DataSet ds = function.getTopItemIDByPIDFriday(playerID);

                        for (int i = Convert.ToInt32(ds.Tables[0].Rows[0]["ID"].ToString()); i <= itemID1 - 1; i++)
                        {
                            function.updateItemFridayStatusByID(3, i);


                        }

                        for (int j = Convert.ToInt32(ds.Tables[0].Rows[14]["ID"].ToString()); j <= itemID2 - 1; j++)
                        {

                            function.updateItemFridayStatusByID(3, j);
                        }
                    }
                }

            }

            var fridayLog = new RedeemLogFridayFirst()
            {
                PlayerID = playerID,
                PlayerName = function.getPlayerName(playerID),
                PromotionName = "Lucky Friday Bonus",
                Points = playerPointsFriday,
                GamingDate = playerGamingDate.ToString("dd/MM/yyyy"),
                Items = GetItemListFridayByPlayerID(playerID)
            };

            return fridayLog;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public RedeemLogFridaySecond RedeemFridaySecond(int playerID, int itemID)
        {
            int maxID = 0;
            int ok = 0;
            //int currentPlayerPoints = 0;
            ItemDetail itemDetail = new ItemDetail();

            string playerName = function.getPlayerName(playerID);

            MFFridayBonus_Logs log = new MFFridayBonus_Logs();

            int fridayPoints = function.selectPlayerPointsFriday(playerID);


            if (playerID > 0 && itemID > 0)
            {
                itemDetail = GetRedeemItemDetailFridayByID(playerID, itemID);
            }

            if (itemDetail.ID > 0)
            {
                function.updateItemFridayStatusFirst(playerID, 0, itemDetail.ItemPoints);
                function.updateItemFridayStatusSecond(playerID, itemDetail.ID, 2);

                itemDetail = GetRedeemItemDetailFridayByID(playerID, itemID);
                log.ID = itemDetail.ID;
                log.PlayerID = playerID;
                log.PlayerName = playerName;

                log.PromotionName = "Lucky Friday Bonus";
                log.FridayPoints = fridayPoints;
                log.SlotFridayPoints = 0;
                log.TableFridayPoints = 0;
                log.Status = function.getStatusItemFriday(itemDetail.ID, playerID);
                log.ItemID = itemDetail.ID;
                log.ItemName = itemDetail.ItemName;
                log.ItemPoints = itemDetail.ItemPoints;
                log.IssueDate = DateTime.Now.ToString("dd/MM/yyyy");
                log.IssueTime = DateTime.Now.ToString("HH:mm:ss");
                log.GamingDate = function.getPlayerGamingDateFriday(playerID).ToString();
                log.voidedStatus = "printed";

                ok = 1;
                try
                {
                    db.MFFridayBonus_Logs.Add(log);
                    db.SaveChanges();
                    maxID = log.ID;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }


            var fridayLog = new RedeemLogFridaySecond()
            {

                TicketNo = maxID,
                PlayerID = playerID,
                PlayerName = function.getPlayerName(playerID),
                ItemName = itemDetail.ItemName,
                PromotionName = "Lucky Friday Bonus",
                Points = fridayPoints,
                IssuedDate = DateTime.Now.ToString("dd/MM/yyyy"),
                IssuedTime = DateTime.Now.ToString("HH:mm:ss"),
                Status = ok,
                Items = GetItemListFridayByPlayerID(playerID)
            };

            return fridayLog;
        }
        public List<ItemDetail> GetItemListFridayByPlayerID(int ID)
        {
            List<ItemDetail> listItemDetail = new List<ItemDetail>();
            DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFFridayBonus_spGetItemsByPlayerID", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", ID));
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {

                        ds = new DataSet();
                        da.Fill(ds);
                    }
                }
            }
            try
            {
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                listItemDetail.Add(new ItemDetail()
                                {
                                    ID = Convert.ToInt32(dr["ID"]),
                                    ItemName = dr["ItemName"].ToString(),
                                    ItemPoints = Convert.ToInt32(dr["ItemPoints"].ToString()),
                                    Status = Convert.ToInt32(dr["Status"].ToString()),

                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listItemDetail;
        }
        public ItemDetail GetRedeemItemDetailFridayByID(int playerID, int itemID)
        {
            ItemDetail itemDetail = new ItemDetail();
            DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFFridayBonus_spGetRedeemItemsByID", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@ItemID", itemID));
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {

                        ds = new DataSet();
                        da.Fill(ds);
                    }
                }
            }
            try
            {
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = ds.Tables[0].Rows[0];


                            itemDetail.ID = Convert.ToInt32(dr["ID"]);
                            itemDetail.ItemName = dr["ItemName"].ToString();
                            itemDetail.ItemPoints = Convert.ToInt32(dr["ItemPoints"].ToString());
                            itemDetail.Status = Convert.ToInt32(dr["Status"].ToString());
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return itemDetail;
        }
        // Redemption Class

        #region FB Business

        //get fb items by playerid
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<FBItem> getFBItems(int playerID)
        {
            return fbFunction.getFBItems(playerID);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public bool updateStatusFBTicket(int ticketId, int playerID)
        {
            try
            {
                var dt = new DataTable();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("spUpdateStatusCasinoKiosk_FBTickets", con))
                    {
                        con.Open();
                        cmd.Connection = con;
                        cmd.Parameters.Add(new SqlParameter("@ID", ticketId));
                        //cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            { }
            return false;
        }

        private bool insertFBTicket(int TicketID, int PlayerID, string ItemName, string IssuedBy, string Points)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("spInsertCasinoKiosk_FBTicketsOnPoints", con))
                    {
                        con.Open();
                        cmd.Connection = con;
                        cmd.Parameters.Add(new SqlParameter("@TicketID", TicketID));
                        cmd.Parameters.Add(new SqlParameter("@PlayerID", PlayerID));
                        cmd.Parameters.Add(new SqlParameter("@ItemName", ItemName));
                        cmd.Parameters.Add(new SqlParameter("@IssuedBy", IssuedBy));
                        //cmd.Parameters.Add(new SqlParameter("@Status", 1));
                        cmd.Parameters.Add(new SqlParameter("@Points", Points));
                        //cmd.Parameters.Add(new SqlParameter("@IssuedDate", IssuedDate));
                        //cmd.Parameters.Add(new SqlParameter("@IssuedTime", IssuedTime));

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            { }
            return false;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public int compFB(int playerId, int itemNumber, int itemValue, string hostName, string userName, bool isKiosk = false)
        {
            try
            {
                if (isKiosk)
                    userName = "nghiad.tran";
                var uid = getUserIDByUserName(userName);
                var dt = new DataTable();
                var compid = -1;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    //var strCommand = string.Format("exec Proc_Comp '{0}','{1}',1,NULL,{2},0,1,2,{3},31261,31261,0,'{4}'", playerId, DateTime.Now.ToString("yyyy-MM-dd"), itemNumber, itemValue, hostName);
                    var strCommand = string.Format("exec Proc_Comp '{0}','{1}',1,NULL,{2},0,1,2,{3},{5},{5},0,'{4}'", playerId, DateTime.Now.ToString("yyyy-MM-dd"), itemNumber, itemValue, hostName, uid);

                    using (SqlCommand cmd = new SqlCommand(strCommand, con))
                    {
                        con.Open();
                        cmd.Connection = con;
                        cmd.CommandType = CommandType.Text;
                        var rs = cmd.ExecuteScalar();
                        compid = int.Parse(rs.ToString());
                    }
                }
                return compid;
            }
            catch (Exception ex)
            {
            }
            return -1;
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public int getUserIDByUserName(string UserName)
        {
            var uid = -1;
            try
            {
                var dt = new DataTable();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    var strCommand = string.Format("exec spGetUserIDCasinoKiosk_FBTickets '{0}'", UserName);

                    using (SqlCommand cmd = new SqlCommand(strCommand, con))
                    {
                        con.Open();
                        cmd.Connection = con;
                        cmd.CommandType = CommandType.Text;
                        //using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        //{
                        //    da.Fill(dt);
                        //}
                        var rs = cmd.ExecuteScalar();
                        uid = int.Parse(rs.ToString());
                        //uid =int.Parse(dt.Rows[0][0].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return uid;
        }
        #endregion

        #region move from 10.21.1.26
        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public PlayerInfo GetPlayerInfo(string cardNumber)
        //{
        //    var _sqlHelper = new SqlHelper();
        //    PlayerInfo P = new PlayerInfo();
        //    String strConnString = ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString;
        //    string command = "exec pro_IT_PlayPointInfo '{0}'";

        //    SqlConnection cnn = new SqlConnection(strConnString);

        //    try
        //    {
        //        cnn.Open();
        //        string cmdText = String.Format(command, cardNumber);
        //        SqlCommand cmd = new SqlCommand(cmdText, cnn);

        //        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //        DataSet ds = new DataSet();
        //        adapter.Fill(ds);
        //        cnn.Close();

        //        //Fill data to PlayerInfo object
        //        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //        {
        //            DataRow r = ds.Tables[0].Rows[0];
        //            P.ID = int.Parse(r["PlayerID"].ToString());
        //            P.FirstName = r["FirstName"].ToString();
        //            P.LastName = r["LastName"].ToString();
        //            DateTime dt = DateTime.Parse(r["BirthDay"].ToString());
        //            P.DOB = dt.ToString("dd-MMM-yyyy");
        //            P.Status = r["Status"].ToString();
        //            P.Rank = r["Rank"].ToString();
        //            P.Points = int.Parse(r["Points"].ToString());
        //            P.TodayPoints = int.Parse(r["TodayPoint"].ToString());
        //            P.PeriodPoints = int.Parse(r["PeriodPoint"].ToString());
        //            P.ThursdayPoint = int.Parse(r["ThursdayPoint"].ToString());

        //            P.NumDayEnrolled = int.Parse(r["NumDayEnrolled"].ToString());
        //            P.GamingDate = DateTime.Parse(r["AccountingDate"].ToString()).ToString("dd-MMM-yyyy");
        //            P.SegmentationCode = r["SegmentationCode"].ToString();
        //            string TotalRedeemedTicketNewYear_today = getRedeemTicketByPromotionByDay("11", r["PlayerID"].ToString(), P.GamingDate);
        //            P.TotalRedeemedTicketNewYear_today = int.Parse(TotalRedeemedTicketNewYear_today);

        //            string TotalRedeemedTicketFB8Dragon_today = getRedeemTicketByPromotionByDay("12", r["PlayerID"].ToString(), P.GamingDate);
        //            P.TotalRedeemedTicketFb8Dragon_today = int.Parse(TotalRedeemedTicketFB8Dragon_today);

        //            string TotalRedeemedTicketGrandBaccaratTournament = getRedeemTicketByPromotionByDay("13", r["PlayerID"].ToString(), P.GamingDate);
        //            P.TotalRedeemedTicketGrandBaccaratTournament = int.Parse(TotalRedeemedTicketGrandBaccaratTournament);

        //            P.TotalRedeemedTicketBountyHuntingSilver = int.Parse(getRedeemTicketByPromotionByDay("14", r["PlayerID"].ToString(), P.GamingDate));
        //            P.TotalRedeemedTicketBountyHuntingGolden = int.Parse(getRedeemTicketByPromotionByDay("15", r["PlayerID"].ToString(), P.GamingDate));

        //            P.TotalRedeemedTicketBountyHuntingSilverChestAllPlayer = int.Parse(getRedeemTicketBountyHunting("14"));

        //            P.TotalRedeemedTicketBountyHuntingGoldenChestAllPlayer = int.Parse(getRedeemTicketBountyHunting("15"));
        //            P.isShowBountyHunting = 0;
        //            if (P.TotalRedeemedTicketBountyHuntingGoldenChestAllPlayer < 50 || P.TotalRedeemedTicketBountyHuntingSilverChestAllPlayer < 100)
        //            {

        //                // if (DateTime.Now.DayOfWeek == DayOfWeek.Thursday )
        //                if (DateTime.Now.DayOfWeek == DayOfWeek.Wednesday)

        //                //if (DateTime.Now.DayOfWeek == DayOfWeek.Thursday && (DateTime.Now.Hour >= 19 && DateTime.Now.Hour <= 23) && (DateTime.Now.Minute >= 0 && DateTime.Now.Minute <= 59) && (DateTime.Now.Second >= 0 && DateTime.Now.Second <= 59))
        //                {
        //                    P.isShowBountyHunting = 1;
        //                }
        //            }

        //            _sqlHelper.ExecSQL("insert into KioskSwipeCardLog(CardNumber, PlayerID,PlayerName,TodayPoint,EventPoint) values('" + cardNumber + "','" + P.ID + "','" + P.LastName + "-" + P.FirstName + "','" + P.TodayPoints + "','" + P.Promotion1688RedeemedTicket + "')");

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    finally
        //    {
        //        if (cnn != null && cnn.State == ConnectionState.Open)
        //            cnn.Close();
        //    }

        //    return P;
        //}

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        string getRedeemTicketByPromotionByDay(string PromotionID, string PlayerID, string GamingDate)
        {
            String strConnString = ConfigurationManager.ConnectionStrings["CasinoLoyaltyKiosk"].ConnectionString;

            string command = "select count(*) as TotalTicket from Promotion_TicketIssued where PromotionID='{0}' and PlayerID='{1}' and GamingDate= '{2}'";
            if (PromotionID == "13")
            {
                command = "select count(*) as TotalTicket from Promotion_TicketIssued where PromotionID='13' and PlayerID='{1}'";
            }

            if (PromotionID == "14")
            {
                command = "select count(*) as TotalTicket from Promotion_TicketIssued where (PromotionID='14' or PromotionID='15') and PlayerID='{1}' and IssuedTime between dateadd(dd,datediff(dd,0,getdate())/7 * 7 + 2,'17:00:00') and dateadd(dd,datediff(dd,0,getdate())/7 * 7 + 2,'23:59:59')";
            }
            if (PromotionID == "15")
            {
                command = "select count(*) as TotalTicket from Promotion_TicketIssued where (PromotionID='14' or PromotionID='15') and PlayerID='{1}' and IssuedTime between dateadd(dd,datediff(dd,0,getdate())/7 * 7 + 2,'17:00:00') and dateadd(dd,datediff(dd,0,getdate())/7 * 7 + 2,'23:59:59')";
            }
            SqlConnection cnn = new SqlConnection(strConnString);
            string Result = "";
            try
            {
                cnn.Open();
                string cmdText = String.Format(command, PromotionID, PlayerID, GamingDate);
                SqlCommand cmd = new SqlCommand(cmdText, cnn);
                string TotalTicket = "0";
                var resultObj = cmd.ExecuteScalar();
                if (resultObj != null)
                {
                    TotalTicket = cmd.ExecuteScalar().ToString();
                }
                cnn.Close();

                Result = TotalTicket;

            }
            catch (Exception ex)
            {
                Result = "error:" + ex.Message;
            }
            finally
            {
                if (cnn != null && cnn.State == ConnectionState.Open)
                    cnn.Close();
            }
            return Result;
        }

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //string getRedeemTicketBountyHunting(string PromotionId)
        //{
        //    String strConnString = ConfigurationManager.ConnectionStrings["CasinoLoyaltyKiosk"].ConnectionString;
        //    string command = "select count(*) as TotalTicket from Promotion_TicketIssued where PromotionID='{0}' and IssuedTime between dateadd(dd,datediff(dd,0,getdate())/7 * 7 + 2,'17:00:00') and dateadd(dd,datediff(dd,0,getdate())/7 * 7 + 2,'23:59:59') ";

        //    SqlConnection cnn = new SqlConnection(strConnString);
        //    string Result = "";
        //    try
        //    {
        //        cnn.Open();
        //        string cmdText = String.Format(command, PromotionId);
        //        SqlCommand cmd = new SqlCommand(cmdText, cnn);
        //        string TotalTicket = "0";
        //        var resultObj = cmd.ExecuteScalar();
        //        if (resultObj != null)
        //        {
        //            TotalTicket = cmd.ExecuteScalar().ToString();
        //        }
        //        cnn.Close();

        //        Result = TotalTicket;


        //    }
        //    catch (Exception ex)
        //    {
        //        Result = "error:" + ex.Message;
        //    }
        //    finally
        //    {
        //        if (cnn != null && cnn.State == ConnectionState.Open)
        //            cnn.Close();
        //    }
        //    return Result;
        //}


        [WebMethod]
        public string GetLevyTimeRemaining(string @PlayerID)
        {
            string result = "";
            string command = "[spGetPlayerLevyRemainingTime]";
            String LevyConnString = ConfigurationManager.ConnectionStrings["LevyEntryStr"].ConnectionString;
            SqlConnection cnn = new SqlConnection(LevyConnString);

            try
            {
                cnn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = cnn;
                cmd.CommandText = command;
                cmd.Parameters.AddWithValue("@PlayerID", @PlayerID);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                cnn.Close();

                //Fill data to PlayerInfo object
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    double RemainingSecond = double.Parse(ds.Tables[0].Rows[0]["RemainingSecond"].ToString());
                    var ts = TimeSpan.FromSeconds(RemainingSecond);

                    // result=string.Format("H: {0} M:{1} S:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                    result = ds.Tables[0].Rows[0]["RemainingSecond"].ToString();
                }

            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (cnn != null && cnn.State == ConnectionState.Open)
                    cnn.Close();
            }
            return result;
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string IGTPlayerPinVerify(string PlayerID, string PlayerPin)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath("/Models/PlayerPinVerify.xml");
            string xml = System.IO.File.ReadAllText(path);
            StringBuilder xmlvalue = new StringBuilder(xml);
            xmlvalue.Replace("@Player_ID", PlayerID);
            xmlvalue.Replace("@Player_PIN", PlayerPin);

            string verifyResult = postXMLData("http://10.70.1.50:8090", xmlvalue.ToString());

            StringBuilder output = new StringBuilder();

            string result = "";
            using (XmlReader reader = XmlReader.Create(new StringReader(verifyResult)))
            {
                reader.ReadToFollowing("Operation");
                reader.MoveToAttribute("Success");
                result = "PinMatched=" + reader.Value;
            }

            return result;

        }
        public string postXMLData(string destinationUrl, string requestXml)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
            byte[] bytes;
            bytes = System.Text.Encoding.ASCII.GetBytes(requestXml);
            request.ContentType = "text/xml; encoding='utf-8'";
            request.ContentLength = bytes.Length;
            request.Method = "POST";
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response;
            response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = response.GetResponseStream();
                string responseStr = new StreamReader(responseStream).ReadToEnd();
                return responseStr;
            }
            return null;
        }

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public List<GrandEventTicket> getPromotionTicketList(string PromotionID, string PlayerID, string cardNumber, string KioskNo)
        //{
        //    var _sqlHelper = new SqlHelper();
        //    List<GrandEventTicket> results = new List<GrandEventTicket>();
        //    PlayerInfo player = new PlayerInfo();
        //    player = GetPlayerInfo(cardNumber);
        //    int totalTicketAvailable = 0;

        //    string PromotionName = _sqlHelper.ExecSQL_GetFirstValue("select promotionName from Promotion_List where PromotionID=" + PromotionID);
        //    PromotionName = PromotionName.ToUpper();
        //    if (PromotionID == "1")
        //    {
        //        int DragonOfferRedeemedTicket = player.TotalRedeemedTicketDragonOffer;
        //        totalTicketAvailable = (player.PointEarnFor8DragonOffer / 500) - player.TotalRedeemedTicketDragonOffer;

        //        if (DragonOfferRedeemedTicket < 1)
        //        {
        //            if (player.PointEarnFor8DragonOffer >= 500)
        //            {
        //                totalTicketAvailable = 1;
        //            }
        //            else
        //            {
        //                totalTicketAvailable = 0;
        //            }
        //        }
        //        else
        //        {
        //            totalTicketAvailable = 0;
        //        }
        //    }
        //    else if (PromotionID == "2")
        //    {
        //        int RedeemedTicket = player.TotalRedeemedTicketSoundWave;

        //        totalTicketAvailable = (player.PointEarnForSoundWave / 888) - player.TotalRedeemedTicketSoundWave;
        //    }
        //    else if (PromotionID == "3")
        //    {
        //        int RedeemedTicket = player.TotalRedeemedTicketDoubleProsperity;

        //        totalTicketAvailable = (player.PointEarnForDoubleProsperity / 888) - player.TotalRedeemedTicketDoubleProsperity;
        //    }
        //    else if (PromotionID == "4")
        //    {
        //        int RedeemedTicket = player.TotalRedeemedTicketSoundWave;

        //        totalTicketAvailable = (player.PointEarnForSoundWave / 688) - player.TotalRedeemedTicketSoundWave;
        //    }
        //    else if (PromotionID == "5")
        //    {
        //        int RedeemedTicket1688 = 0;
        //        if (player.Promotion1688RedeemedTicket != null)
        //        {
        //            RedeemedTicket1688 = int.Parse(player.Promotion1688RedeemedTicket);
        //        }


        //        totalTicketAvailable = (player.TodayPoints / 1688) - RedeemedTicket1688;


        //    }
        //    else if (PromotionID == "7")
        //    {
        //        int RedeemedTicket = 0;
        //        if (player.TotalRedeemedTicketSlotTournament != 0)
        //        {
        //            RedeemedTicket = player.TotalRedeemedTicketSlotTournament;
        //        }


        //        totalTicketAvailable = (player.PointEarnForSlotTournament / 500) - RedeemedTicket;
        //        int MaxSlotTournamentTicket = 8 - RedeemedTicket;

        //        if (totalTicketAvailable > MaxSlotTournamentTicket)
        //        {
        //            totalTicketAvailable = MaxSlotTournamentTicket;
        //        }


        //    }
        //    else if (PromotionID == "8")
        //    {
        //        int RedeemedTicket = 0;
        //        if (player.PromotionAnniversary_RedeemedTicket != 0)
        //        {
        //            RedeemedTicket = player.PromotionAnniversary_RedeemedTicket;
        //        }


        //        totalTicketAvailable = (player.PointEarnForAnniversary / 888) - RedeemedTicket;



        //    }
        //    else if (PromotionID == "9")
        //    {
        //        int RedeemedTicket = 0;
        //        if (player.TheGrandLuckyDraw_RedeemedTicket != 0)
        //        {
        //            RedeemedTicket = player.TheGrandLuckyDraw_RedeemedTicket;
        //        }


        //        totalTicketAvailable = (player.PointForTheGrandLuckyDraw / 1000) - RedeemedTicket;

        //    }
        //    else if (PromotionID == "11")
        //    {
        //        int RedeemedTicket = player.TotalRedeemedTicketNewYear_today;

        //        int maxTicket = player.TodayPoints / 188;
        //        if (maxTicket >= 5)
        //        {
        //            maxTicket = 5;
        //        }

        //        totalTicketAvailable = maxTicket - player.TotalRedeemedTicketNewYear_today;
        //        if (totalTicketAvailable <= 0)
        //        {
        //            totalTicketAvailable = 0;
        //        }

        //    }
        //    else if (PromotionID == "12")
        //    {
        //        int RedeemedTicket = player.TotalRedeemedTicketFb8Dragon_today;

        //        int maxTicket = player.TodayPoints / 10;
        //        if (maxTicket >= 2)
        //        {
        //            maxTicket = 2;
        //        }

        //        totalTicketAvailable = maxTicket - player.TotalRedeemedTicketFb8Dragon_today;
        //        if (totalTicketAvailable <= 0)
        //        {
        //            totalTicketAvailable = 0;
        //        }

        //    }
        //    else if (PromotionID == "13")
        //    {
        //        int RedeemedTicket = player.TotalRedeemedTicketGrandBaccaratTournament;

        //        int maxTicket = player.PeriodPoints / 1000;
        //        if (maxTicket >= 1)
        //        {
        //            maxTicket = 1;
        //        }

        //        totalTicketAvailable = maxTicket - player.TotalRedeemedTicketGrandBaccaratTournament;
        //        if (totalTicketAvailable <= 0)
        //        {
        //            totalTicketAvailable = 0;
        //        }

        //    }
        //    else if (PromotionID == "14")
        //    {
        //        int RedeemedTicket = player.TotalRedeemedTicketBountyHuntingSilver;
        //        int maxTicket = 0;
        //        if (player.ThursdayPoint < 3000)
        //        {
        //            maxTicket = 1;
        //        }

        //        totalTicketAvailable = maxTicket - RedeemedTicket;
        //        if (totalTicketAvailable <= 0)
        //        {
        //            totalTicketAvailable = 0;
        //        }

        //    }
        //    else if (PromotionID == "15")
        //    {
        //        int RedeemedTicket = player.TotalRedeemedTicketBountyHuntingGolden;
        //        int maxTicket = 0;
        //        if (player.ThursdayPoint >= 3000)
        //        {
        //            maxTicket = 1;
        //        }

        //        totalTicketAvailable = maxTicket - RedeemedTicket;
        //        if (totalTicketAvailable <= 0)
        //        {
        //            totalTicketAvailable = 0;
        //        }

        //    }

        //    String strConnString = ConfigurationManager.ConnectionStrings["CasinoLoyaltyKiosk"].ConnectionString;
        //    string cmdText = "";
        //    int issue_ticket = 0;
        //    if (issue_ticket < totalTicketAvailable)
        //    {
        //        issue_ticket = 1;
        //    }
        //    for (int i = 1; i <= issue_ticket; i++)
        //    {
        //        SqlConnection cnn = new SqlConnection(strConnString);
        //        string Result = "";
        //        try
        //        {
        //            cnn.Open();
        //            cmdText = "sp_issued_ticket";
        //            SqlCommand cmd = new SqlCommand(cmdText, cnn);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.Add("@PromotionID", SqlDbType.VarChar).Value = PromotionID;
        //            cmd.Parameters.Add("@PlayerID", SqlDbType.VarChar).Value = PlayerID;
        //            cmd.Parameters.Add("@PlayerName", SqlDbType.VarChar).Value = player.FirstName + " " + player.LastName;
        //            cmd.Parameters.Add("@KioskNo", SqlDbType.VarChar).Value = KioskNo;
        //            cmd.Parameters.Add("@GamingDate", SqlDbType.VarChar).Value = player.GamingDate;


        //            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //            DataSet ds = new DataSet();
        //            adapter.Fill(ds);
        //            cnn.Close();
        //            foreach (DataRow row in ds.Tables[0].Rows)
        //            {
        //                var tk = new GrandEventTicket();
        //                tk.TicketNo = row["TicketNo"].ToString();
        //                tk.PlayerID = player.ID.ToString();
        //                tk.PlayerName = player.FirstName + " " + player.LastName;
        //                tk.IssuedDate = DateTime.Parse(row["IssuedTime"].ToString()).ToString("dd-MM-yyyy");
        //                tk.IssuedTime = DateTime.Parse(row["IssuedTime"].ToString()).ToString("HH:mm");
        //                tk.KioskNo = row["KioskNo"].ToString();
        //                tk.PromotionName = PromotionName;
        //                results.Add(tk);
        //            }



        //        }
        //        catch (Exception ex)
        //        {
        //            Result = "error:" + ex.Message;
        //        }
        //        finally
        //        {
        //            if (cnn != null && cnn.State == ConnectionState.Open)
        //                cnn.Close();
        //        }
        //    }

        //    //  string cmdText = String.Format(command, PlayerID);

        //    return results;
        //}
        #endregion

        public class Items
        {
            public int ID { get; set; }
            public string ItemName { get; set; }
            public int ItemPoints { get; set; }
            public bool isActive { get; set; }
            public string imageURL { get; set; }
            public int Quantity { get; set; }

        }
        public class RedeemLog
        {
            public int TicketNo { get; set; }
            public string LogName { get; set; }
            public string issuedTime { get; set; }
            public string issuedDate { get; set; }
            public int PlayerID { get; set; }
            public string PlayerName { get; set; }
            public string PromotionName { get; set; }
            public int ItemPoints { get; set; }
            public int Status { get; set; }
            public int CurrentPlayerPoints { get; set; }
            public int Quantity { get; set; }
            public string voidedStatus { get; set; }
            //Add 20230109 Hantt start
            public string Location { get; set; }
            //Add 20230109 Hantt start

        }
        // Daily Bonus Class
        public class ItemDetail
        {
            public int ID { get; set; }
            public int ItemPoints { get; set; }
            public string ItemName { get; set; }
            public int Status { get; set; }
            public string Type { get; set; }

        }
        public class ItemsDaily
        {
            public int ID { get; set; }
            public int ItemPoints { get; set; }
            public string ItemName { get; set; }
            public int PlayerID { get; set; }
            public int Status { get; set; }
            public int YStatus { get; set; }
        }
        //Add 20231212 Hantt start
        public class RedeemLog8DragonsFirst
        {
            public int PlayerID { get; set; }
            public string PlayerName { get; set; }
            public string GamingDate { get; set; }
            public string PromotionName { get; set; }
            public int Points { get; set; }
            public int CombinedPoints { get; set; }
            public int BalancePoints { get; set; }
            public int EnableCount { get; set; }
            public List<MF8DragonBuffetBonus_Items> Items { get; set; }
        }

        public class RedeemLog8DragonsSecond
        {
            public int TicketNo { get; set; }
            public int PlayerID { get; set; }
            public string PlayerName { get; set; }
            public string ItemName { get; set; }
            public string IssuedDate { get; set; }
            public string IssuedTime { get; set; }
            public string GamingDate { get; set; }
            public string PromotionName { get; set; }
            public int YesterdayPoints { get; set; }
            public int TodayPoints { get; set; }
            public int CombinedPoints { get; set; }
            public int BalancePoints { get; set; }
            public int Status { get; set; }
            public List<MF8DragonBuffetBonus_Items> Items { get; set; }
        }
        //Add 20231212 Hantt end

        public class RedeemLogDailyFirst
        {
            public int PlayerID { get; set; }
            public string PlayerName { get; set; }
            public string GamingDate { get; set; }
            public string PromotionName { get; set; }
            public int Points { get; set; }
            public int SlotPoints { get; set; }
            public int TablePoints { get; set; }
            public List<ItemsDaily> Items { get; set; }
        }
        public class RedeemLogDailySecond
        {
            public int TicketNo { get; set; }
            public int PlayerID { get; set; }
            public string PlayerName { get; set; }
            public string ItemName { get; set; }
            public string ItemNameFB { get; set; }
            public string IssuedDate { get; set; }
            public string IssuedTime { get; set; }
            public string GamingDate { get; set; }
            public string Yesterday { get; set; }
            public string PromotionName { get; set; }
            public int Points { get; set; }
            public int SlotPoints { get; set; }
            public int TablePoints { get; set; }
            public int Status { get; set; }
            public List<ItemsDaily> Items { get; set; }
        }

        // Weekly Bonus Class
        public class RedeemLogWeeklyFirst
        {
            public int PlayerID { get; set; }
            public string PlayerName { get; set; }
            public string GamingDate { get; set; }
            public string PromotionName { get; set; }
            public int Points { get; set; }
            public int SlotPoints { get; set; }
            public int TablePoints { get; set; }
            public List<ItemDetail> Items { get; set; }
        }

        public class RedeemLogWeeklySecond
        {
            public int TicketNo { get; set; }
            public int PlayerID { get; set; }
            public string PlayerName { get; set; }
            //Change 20230619 Hantt start
            public string ItemName { get; set; }
            //public string ItemNameSBV { get; set; }
            //public string ItemNameSFP { get; set; }
            //Change 20230619 Hantt end
            public string IssuedDate { get; set; }
            public string IssuedTime { get; set; }
            public string PromotionName { get; set; }
            public int Points { get; set; }
            public int SlotPoints { get; set; }
            public int TablePoints { get; set; }
            public int Status { get; set; }
            public List<ItemDetail> Items { get; set; }
        }

        public class Points
        {
            public int DailyPoints { get; set; }
            public int WeeklyPoints { get; set; }
            public int FridayPoints { get; set; }

            public int BalancePoints { get; set; }
            public int CombinePoints { get; set; }

            public string PromotionEnd { get; set;}
        }

        // Lucky Firday Class

        public class RedeemLogFridayFirst
        {


            public int PlayerID { get; set; }
            public string PlayerName { get; set; }
            public string GamingDate { get; set; }
            public string PromotionName { get; set; }
            public int Points { get; set; }
            public int SlotPoints { get; set; }
            public int TablePoints { get; set; }

            public List<ItemDetail> Items { get; set; }



        }
        public class RedeemLogFridaySecond
        {
            public int TicketNo { get; set; }
            public int PlayerID { get; set; }
            public string PlayerName { get; set; }
            public string ItemName { get; set; }
            public string IssuedDate { get; set; }
            public string IssuedTime { get; set; }
            public string PromotionName { get; set; }
            public int Points { get; set; }
            public int SlotPoints { get; set; }
            public int TablePoints { get; set; }
            public int Status { get; set; }
            public List<ItemDetail> Items { get; set; }

        }

        // Points Redemption Class

        public class RedeemPointsLog
        {
            public int TicketNo { get; set; }
            public string LogName { get; set; }
            //Change 20230118 Hantt start
            //public string issuedTime { get; set; }
            //public string issuedDate { get; set; }
            public string IssuedTime { get; set; }
            public string IssuedDate { get; set; }
            //Change 20230118 Hantt end
            public int PlayerID { get; set; }
            public string PlayerName { get; set; }
            public string PromotionName { get; set; }
            public int ItemPoints { get; set; }
            public int Status { get; set; }
            public int CurrentPlayerPoints { get; set; }
            public int Quantity { get; set; }
            public string voidedStatus { get; set; }
        }

        // Slot Daily class

        public class DailyRedeemSlotLog
        {
            public int TicketNo { get; set; }
            public string LogName { get; set; }
            public string issuedTime { get; set; }
            public string issuedDate { get; set; }
            public int PlayerID { get; set; }
            public string PlayerName { get; set; }
            public string PromotionName { get; set; }
            public int Status { get; set; }
            public int ItemPoints { get; set; }
            public int DailySlotPoints { get; set; }
            public int vDailySlotPoints { get; set; }
        }

        // Play & Stay Bundle class

        public class RedeemLogDailyPSbundleFirst
        {
            public int PlayerID { get; set; }
            public string PlayerName { get; set; }
            public string GamingDate { get; set; }
            public string PromotionName { get; set; }
            public List<DailyPSBundle_Items> Items { get; set; }
        }

        public class RedeemLogDailyPSbundleSecond
        {
            public int TicketNo { get; set; }
            public int PlayerID { get; set; }
            public string PlayerName { get; set; }
            public string ItemName { get; set; }
            public string ItemNameFB { get; set; }
            public string IssuedDate { get; set; }
            public string IssuedTime { get; set; }
            public string GamingDate { get; set; }
            public string Yesterday { get; set; }
            public string PromotionName { get; set; }
            public int? Status { get; set; }
            public List<DailyPSBundle_Items> Items { get; set; }
        }

    }

}

