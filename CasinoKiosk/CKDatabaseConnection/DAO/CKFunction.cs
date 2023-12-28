using CKDatabaseConnection.EF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CKDatabaseConnection.DAO
{
    public class CKFunction
    {


        //List<RedeemLog> logList = new List<RedeemLog>();
        //CKdbContext db = new CKdbContext();

        //db casinokiosk
        string connectionString = ConfigurationManager.ConnectionStrings["CKdbContext"].ConnectionString;

        //db playermanagement
        string connectionString1 = ConfigurationManager.ConnectionStrings["PlayerManagementConStr"].ConnectionString;
        public string getPlayerName(int ID)
        {
            string playerName = "";
            DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString1))
            {
                using (SqlCommand cmd = new SqlCommand("Proc_GetPlayerName", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@@PlayerID", ID));
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
                            playerName = ds.Tables[0].Rows[0]["PlayerName"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return playerName;
        }
        public int getPlayerPointsBalance(int ID)
        {

            int playerPoints = 0;
            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString1))
            {
                using (SqlCommand cmd = new SqlCommand("Proc_GetPlayerPointBalance", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@@nPlayerID", ID));
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
                            playerPoints = Convert.ToInt32(ds.Tables[0].Rows[0]["PlayerBalance"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return playerPoints;

        }

        //CasinoKiosUsers
        public void updateLastLoginCasinoKioskUsers(string UserName)
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("CasinoKiosk_UpdateLastLogin", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@UserName", UserName));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void updateLastLoginFOPatronUsers(string UserName)
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("FOPatron_UpdateLastLogin", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@UserName", UserName));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
        }
        //Daily Bonus    
        public int selectPlayerPointsDaily(int playerID)
        {
            int points = 0;
            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFDailyBonus_spSelectPlayerPoints", con))
                {
                    con.Open();
                    cmd.Connection = con;

                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
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
                            points = Convert.ToInt32(ds.Tables[0].Rows[0]["DailyPoints"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return points;
        }
        public DataSet getStatusItemDailyByPID(int PlayerID)
        {


            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFDailyBonus_spGetItemsDailyStatusByPlayerID", con))
                {
                    con.Open();
                    cmd.Connection = con;

                    cmd.Parameters.Add(new SqlParameter("@PlayerID", PlayerID));
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
                    return ds;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;

        }
        public DateTime getMFDBCurrentDay()
        {

            DateTime now = DateTime.Now;
            var today = DateTime.Today;
            DateTime currentDay;

            int currentTime = Int32.Parse(now.Hour.ToString());
            if (currentTime < 6)
            {
                currentDay = today.AddDays(-1);

            }
            else
            {
                currentDay = today;

            }

            return currentDay;
        }

        //Add 20231212 Hantt start
        //Add 20231212 Hantt end


        public void insertItemsDaily(int itemPoints, int playerID)
        {

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFDailyBonus_ItemsInsert_New", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@ItemPoints", itemPoints));
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
        }        
        public void insertUpdateItemsYesterdayLog(int PlayerID, DateTime ActionTime, string Description)
        {

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFDailyBonus_InsertUpdateItemsYesterdayLog", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    //cmd.Parameters.Add(new SqlParameter("@ItemName", itemName));
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", PlayerID));
                    cmd.Parameters.Add(new SqlParameter("@ActionTime", ActionTime));
                    cmd.Parameters.Add(new SqlParameter("@Description", Description));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void insertPlayersDaily(int playerID, string playerName, int dailyPoints, int slotDailyPoints, int tableDailyPoints, DateTime gamingDate)
        {

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFDailyBonus_PlayersInsert", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@PlayerName", playerName));
                    cmd.Parameters.Add(new SqlParameter("@DailyPoints", dailyPoints));
                    cmd.Parameters.Add(new SqlParameter("@SlotDailyPoints", slotDailyPoints));
                    cmd.Parameters.Add(new SqlParameter("@TableDailyPoints", tableDailyPoints));
                    cmd.Parameters.Add(new SqlParameter("@GamingDate", gamingDate));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public int getItemsRowCountDaily(int playerID)
        {

            int count = 0;
            DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFDailyBonus_GetItemsRowCount", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
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
                        if (ds.Tables[0].Rows[0]["RowCount"].ToString() != "0")
                        {
                            count = Convert.ToInt32(ds.Tables[0].Rows[0]["RowCount"]);
                        }

                    }
                }

            }

            else
            {
                count = 0;
            }


            return count;

        }
        public int getItemPointsDaily(int dailyPoints)
        {

            int points = 0;

            if (dailyPoints >= 1000 && dailyPoints < 2000)
            {
                points = 1000;

            }

            if (dailyPoints >= 2000 && dailyPoints < 4000)
            {
                points = 2000;

            }

            if (dailyPoints >= 4000 && dailyPoints < 6000)
            {
                points = 4000;

            }

            if (dailyPoints >= 6000 && dailyPoints < 8000)
            {
                points = 6000;

            }

            //Change 20230619 Hantt start
            //if (dailyPoints >= 8000)
            //{
            //    points = 8000;

            //}
            if (dailyPoints >= 8000 && dailyPoints < 20000)
            {
                points = 8000;

            }

            if (dailyPoints >= 20000)
            {
                points = 20000;
            }

            //Change 20230619 Hantt end

                return points;
        }
        public int getPlayersRowCountDaily(int playerID)
        {

            int count = 0;
            DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFDailyBonus_GetPlayersRowCount", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
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
                        if (ds.Tables[0].Rows[0]["RowCount"].ToString() != "0")
                        {
                            count = Convert.ToInt32(ds.Tables[0].Rows[0]["RowCount"]);
                        }

                    }
                }

            }

            else
            {
                count = 0;
            }

            return count;
        }
        public int getPlayerPointsDaily(int ID, DateTime Date)
        {

            int playerPointsDaily = 0;
            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                //using (SqlCommand cmd = new SqlCommand("MFDailyBonus_GetPlayerPointsDaily", con))
                using (SqlCommand cmd = new SqlCommand("MFDailyBonus_GetPlayerPointsDaily_CasinoKiosk", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", ID));
                    cmd.Parameters.Add(new SqlParameter("@Date", Date));
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
                            playerPointsDaily = Convert.ToInt32(ds.Tables[0].Rows[0]["DailyPointsEarned"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return playerPointsDaily;

        }
        //public int getPlayerPointsYesterday(int ID, DateTime Date)
        //{

        //    int playerPointsYesterday = 0;
        //    DataSet ds;

        //    using (SqlConnection con = new SqlConnection(connectionString))
        //    {
        //        using (SqlCommand cmd = new SqlCommand("MFDailyBonus_GetPlayerPointsDaily", con))
        //        {
        //            con.Open();
        //            cmd.Connection = con;
        //            cmd.Parameters.Add(new SqlParameter("@PlayerID", ID));
        //            cmd.Parameters.Add(new SqlParameter("@Date", Date));
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        //            {

        //                ds = new DataSet();
        //                da.Fill(ds);
        //            }
        //        }
        //    }
        //    try
        //    {
        //        if (ds != null)
        //        {
        //            if (ds.Tables.Count > 0)
        //            {
        //                if (ds.Tables[0].Rows.Count > 0)
        //                {
        //                    playerPointsDaily = Convert.ToInt32(ds.Tables[0].Rows[0]["DailyPointsEarned"].ToString());
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return playerPointsDaily;

        //}
        public int getTwoDaysPoints(int ID, DateTime Date1, DateTime Date2)
        {

            int twoDaysPoints = 0;
            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFDailyBonus_Get2DaysPointsDaily", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", ID));
                    cmd.Parameters.Add(new SqlParameter("@Date1", Date1));
                    cmd.Parameters.Add(new SqlParameter("@Date2", Date2));
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
                            twoDaysPoints = Convert.ToInt32(ds.Tables[0].Rows[0]["TwoDayPointsEarned"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return twoDaysPoints;

        }
        public DateTime getPlayerGamingDateDaily(int playerID)
        {

            DateTime gamingdate = getMFDBCurrentDay();
            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFDailyBonus_spGetPlayerGamingDate", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));

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
                            gamingdate = Convert.ToDateTime(ds.Tables[0].Rows[0]["GamingDate"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return gamingdate;

        }
        public DateTime getPlayerGamingDateDailyPSBundle(int playerID)
        {

            DateTime gamingdate = getMFDBCurrentDay();
            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DailyPSBundle_spGetPlayerGamingDate", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));

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
                            gamingdate = Convert.ToDateTime(ds.Tables[0].Rows[0]["GamingDate"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return gamingdate;

        }
        public int getStatusItemDaily(int ID, int PlayerID)
        {

            int status = 0;
            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFDailyBonus_spGetItemsDailyStatusByID", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@ID", ID));
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", PlayerID));
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
                            status = Convert.ToInt32(ds.Tables[0].Rows[0]["Status"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return status;

        }
        public int getStatusItemYesterday(int ID, int PlayerID)
        {

            int status = 0;
            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFDailyBonus_spGetItemsYesterdayStatusByID", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@ID", ID));
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", PlayerID));
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
                            status = Convert.ToInt32(ds.Tables[0].Rows[0]["Status"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return status;

        }
       
        public DataSet setItemNameOnPoints(int? itemPoints, string itemName)
        {
            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFDailyBonus_spSetItemNameOnPoints", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@itemPoints", itemPoints));
                    cmd.Parameters.Add(new SqlParameter("@itemName", itemName));
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {

                        ds = new DataSet();
                        da.Fill(ds);
                    }
                }
            }
            //try
            //{
            //    if (ds != null)
            //    {
            //        if (ds.Tables.Count > 0)
            //        {
            //            if (ds.Tables[0].Rows.Count > 0)
            //            {
            //                status = Convert.ToInt32(ds.Tables[0].Rows[0]["Status"].ToString());
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            return ds;

        }
        public int getStatusItemDailyByPoints(int ItemPoints, int PlayerID)
        {

            int status = 0;
            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFDailyBonus_spGetItemsDailyStatusByPoints", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@ItemPoints", ItemPoints));
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", PlayerID));
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
                            status = Convert.ToInt32(ds.Tables[0].Rows[0]["Status"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return status;

        }
        public void updateItemDailyStatusFirst(int playerID, int status, int itemPoints)
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFDailyBonus_UpdateItemStatusFirst", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@playerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@status", status));
                    cmd.Parameters.Add(new SqlParameter("@itemPoints", itemPoints));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }
        // public void updateItemDailyStatusYesterday(int playerID, DateTime playerGamingDate)
        public void updateItemDailyStatusYesterday(int playerID)
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFDailyBonus_UpdateItemStatusYesterday", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@playerID", playerID));
                   // cmd.Parameters.Add(new SqlParameter("@gamingDate", playerGamingDate.ToString("dd/MM/yyyy").Replace("-", "/")));
                    //cmd.Parameters.Add(new SqlParameter("@itemPoints", itemPoints));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }
        public void updateItemDailyStatusAll(int playerID, int status)
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFDailyBonus_UpdateItemStatusAll", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@playerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@status", status));
                    //cmd.Parameters.Add(new SqlParameter("@itemPoints", itemPoints));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }
        public void updateItemDailyStatusYesterdayAll(int playerID, int status)
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFDailyBonus_UpdateItemStatusYesterdayAll", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@playerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@status", status));
                    //cmd.Parameters.Add(new SqlParameter("@itemPoints", itemPoints));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }

        ////Add 20230301 Hantt start
        //public IEnumerable<MFBonus_spSelectDailyLogs_Result> ListAllPagingDailyLog(int page, int pageSize, int playerID)
        //{
        //    List<SpReport_MarketingAuthorizer_Result> list = context.SpReport_MarketingAuthorize().ToList();
        //    return list.OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
        //}
        ////Add 20230301 Hantt end

        public void updateItemDailyStatusNew(int playerID, int itemPoints)
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFDailyBonus_UpdateItemStatusNew", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@ItemPoints", itemPoints));
                    //cmd.Parameters.Add(new SqlParameter("@GamingDate", gamingDate.ToString("dd/MM/yyyy").Replace("-", "/")));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }

        public void updatePlayerDailyPoints(int playerID, int dailyPoints)
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFDailyBonus_UpdatePlayerDailyPoints", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@DailyPoints", dailyPoints));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }
        public void updatePlayerGamingDate(int playerID, DateTime gamingDate)
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFDailyBonus_UpdatePlayerGamingDate", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@GamingDate", gamingDate));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }
        public void updatePlayerGamingDatePSBundle(int playerID, DateTime gamingDate)
        {
            //DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DailyPSBundle_UpdatePlayerGamingDate", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@GamingDate", gamingDate));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void updateItemDailyStatusSecond(int playerID, int itemID, int status, string type)
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFDailyBonus_UpdateItemStatusSecond", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@playerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@itemID", itemID));
                    cmd.Parameters.Add(new SqlParameter("@status", status));
                    cmd.Parameters.Add(new SqlParameter("@type", type));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }
        public void updateItemDailyStatusSecondNew(int playerID, int itemPoints, int status, string type)
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFDailyBonus_UpdateItemStatusSecondNew", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@playerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@itemPoints", itemPoints));
                    cmd.Parameters.Add(new SqlParameter("@status", status));
                    cmd.Parameters.Add(new SqlParameter("@type", type));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }
        //Play&Stay Promotion
        public void updateItemDailyPSBundleAll(int playerID, int status)
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DailyPSBundle_UpdateItemStatusAll", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@playerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@status", status));
                    //cmd.Parameters.Add(new SqlParameter("@itemPoints", itemPoints));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }
        public void updateItemDailyPSBundleRemain(int playerID, int status, int ID)
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DailyPSBundle_UpdateItemStatusRemain", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@playerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@status", status));
                    cmd.Parameters.Add(new SqlParameter("@ID", ID));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }
        public void insertDailyPSBundleItems(int playerID)
        {

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DailyPSBundle_ItemsInsert", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
        }
        //Weekly Bonus  
        public void updateItemWeeklyStatusByID(int status, int ID)
        {

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFWeeklyBonus_UpdateItemStatusByID", con))
                {
                    con.Open();
                    cmd.Connection = con;

                    cmd.Parameters.Add(new SqlParameter("@Status", status));
                    cmd.Parameters.Add(new SqlParameter("@ID", ID));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }
        public DataSet getTopItemIDByPIDWeekly(int PlayerID)
        {
            //int status = 0;
            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFWeeklyBonus_spGetTopItemIDByPID", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    //cmd.Parameters.Add(new SqlParameter("@number", number));
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", PlayerID));
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
                    return ds;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }
        public void updatePlayerGamingDateWeekly(int playerID, DateTime gamingDate)
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFWeeklyBonus_UpdatePlayerGamingDate", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@GamingDate", gamingDate));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }
        public DataSet getItemIDByPointsWeekly(int ItemPoints, int PlayerID)
        {
            //int status = 0;
            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFWeeklyBonus_spGetIDByPoints", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@Points", ItemPoints));
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", PlayerID));
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
                    return ds;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }
        public DataSet getStatusItemWeeklyByPID(int PlayerID)
        {


            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFWeeklyBonus_spGetItemsWeeklyStatusByPlayerID", con))
                {
                    con.Open();
                    cmd.Connection = con;

                    cmd.Parameters.Add(new SqlParameter("@PlayerID", PlayerID));
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
                    return ds;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;

        }
        public DateTime getMFWBCurrentDay()
        {

            DateTime now = DateTime.Now;
            var today = DateTime.Today;
            DateTime currentDay;

            int currentTime = Int32.Parse(now.Hour.ToString());
            if (currentTime < 6)
            {
                currentDay = today.AddDays(-1);

            }
            else
            {
                currentDay = today;

            }

            return currentDay;
        }
        public int getPlayersRowCountWeekly(int playerID)
        {

            int count = 0;
            DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFWeeklyBonus_GetPlayersRowCount", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
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
                        if (ds.Tables[0].Rows[0]["RowCount"].ToString() != "0")
                        {
                            count = Convert.ToInt32(ds.Tables[0].Rows[0]["RowCount"]);
                        }

                    }
                }
            }
            else
            {
                count = 0;
            }

            return count;

        }
        public int getItemsRowCountWeekly(int playerID)
        {

            int count = 0;
            DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFWeeklyBonus_GetItemsRowCount", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
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
                        if (ds.Tables[0].Rows[0]["RowCount"].ToString() != "0")
                        {
                            count = Convert.ToInt32(ds.Tables[0].Rows[0]["RowCount"]);
                        }

                    }
                }



            }


            else
            {
                count = 0;
            }


            return count;

        }
        public DateTime getPlayerGamingDateWeekly(int playerID)
        {
            DateTime gamingdate = DateTime.Now;

            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFWeeklyBonus_spGetPlayerGamingDate", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));

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
                            gamingdate = Convert.ToDateTime(ds.Tables[0].Rows[0]["GamingDate"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return gamingdate;

        }
        public void updateItemWeeklyStatusFirst(int playerID, int status, int itemPoints)
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFWeeklyBonus_UpdateItemStatusFirst", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@playerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@itemPoints", itemPoints));
                    cmd.Parameters.Add(new SqlParameter("@status", status));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }
        public void updateItemWeeklyStatusSecond(int playerID, int itemID, int status)
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFWeeklyBonus_UpdateItemStatusSecond", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@playerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@itemID", itemID));
                    cmd.Parameters.Add(new SqlParameter("@status", status));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }
        public int getStatusItemWeeklyByPoints(int ItemPoints, int PlayerID)
        {

            int status = 0;
            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFWeeklyBonus_spGetItemsStatusByPoints", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@ItemPoints", ItemPoints));
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", PlayerID));
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
                            status = Convert.ToInt32(ds.Tables[0].Rows[0]["Status"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return status;

        }

        //Change 20230619 Hantt start
        //public void switchWeeklyStatus(int weeklyPoints, int playerID)
        //{
        //    DataSet ds = getStatusItemWeeklyByPID(playerID);
        //    int statusMPV1000 = Convert.ToInt32(ds.Tables[0].Rows[0]["Status"].ToString());
        //    int statusMPV2000 = Convert.ToInt32(ds.Tables[0].Rows[1]["Status"].ToString());
        //    int statusMPV3000 = Convert.ToInt32(ds.Tables[0].Rows[2]["Status"].ToString());
        //    int statusMPV4000 = Convert.ToInt32(ds.Tables[0].Rows[3]["Status"].ToString());
        //    int statusMPV5000 = Convert.ToInt32(ds.Tables[0].Rows[4]["Status"].ToString());
        //    int statusMPV6000 = Convert.ToInt32(ds.Tables[0].Rows[5]["Status"].ToString());
        //    int statusMPV7000 = Convert.ToInt32(ds.Tables[0].Rows[6]["Status"].ToString());


        //    int statusSFP1000 = Convert.ToInt32(ds.Tables[0].Rows[7]["Status"].ToString());
        //    int statusSFP2000 = Convert.ToInt32(ds.Tables[0].Rows[8]["Status"].ToString());
        //    int statusSFP3000 = Convert.ToInt32(ds.Tables[0].Rows[9]["Status"].ToString());
        //    int statusSFP4000 = Convert.ToInt32(ds.Tables[0].Rows[10]["Status"].ToString());
        //    int statusSFP5000 = Convert.ToInt32(ds.Tables[0].Rows[11]["Status"].ToString());
        //    int statusSFP6000 = Convert.ToInt32(ds.Tables[0].Rows[12]["Status"].ToString());
        //    int statusSFP7000 = Convert.ToInt32(ds.Tables[0].Rows[13]["Status"].ToString());

        //    switch (weeklyPoints)
        //    {

        //        case 5000:

        //            if (statusMPV1000 != 2 && statusSFP1000 != 2 && statusMPV1000 != 3 && statusSFP1000 != 3)
        //            {
        //                updateItemWeeklyStatusFirst(playerID, 1, 5000);
        //            }


        //            break;
        //        case 8000:

        //            if (statusMPV2000 != 2 && statusSFP2000 != 2 && statusMPV2000 != 3 && statusSFP2000 != 3)
        //            {
        //                updateItemWeeklyStatusFirst(playerID, 1, 8000);
        //            }

        //            break;
        //        case 10000:

        //            if (statusMPV3000 != 2 && statusSFP3000 != 2 && statusMPV3000 != 3 && statusSFP3000 != 3)
        //            {
        //                updateItemWeeklyStatusFirst(playerID, 1, 10000);
        //            }
        //            break;
        //        case 12000:

        //            if (statusMPV4000 != 2 && statusSFP4000 != 2 && statusMPV4000 != 3 && statusSFP4000 != 3)
        //            {
        //                updateItemWeeklyStatusFirst(playerID, 1, 12000);
        //            }

        //            break;
        //        case 15000:

        //            if (statusMPV5000 != 2 && statusSFP5000 != 2 && statusMPV5000 != 3 && statusSFP5000 != 3)
        //            {

        //                updateItemWeeklyStatusFirst(playerID, 1, 15000);
        //            }
        //            break;
        //        case 25000:

        //            if (statusMPV6000 != 2 && statusSFP6000 != 2 && statusMPV6000 != 3 && statusSFP6000 != 3)
        //            {
        //                updateItemWeeklyStatusFirst(playerID, 1, 25000);
        //            }
        //            break;
        //        case 50000:

        //            if (statusMPV7000 != 2 && statusSFP7000 != 2 && statusMPV7000 != 3 && statusSFP7000 != 3)
        //            {
        //                updateItemWeeklyStatusFirst(playerID, 1, 50000);
        //            }
        //            break;


        //        default:
        //            updateItemWeeklyStatusFirst(playerID, 0, 5000);
        //            updateItemWeeklyStatusFirst(playerID, 0, 8000);
        //            updateItemWeeklyStatusFirst(playerID, 0, 10000);
        //            updateItemWeeklyStatusFirst(playerID, 0, 12000);
        //            updateItemWeeklyStatusFirst(playerID, 0, 15000);
        //            updateItemWeeklyStatusFirst(playerID, 0, 25000);
        //            updateItemWeeklyStatusFirst(playerID, 0, 50000);

        //            break;

        //    }
        //}
        //public void switchWeeklyPoints(int weeklyItemPoints, int playerID)
        //{
        //    switch (weeklyItemPoints)
        //    {
        //        case 5000:
        //            insertItemsWeekly("$250 SBV - 5000 pts", 5000, 1, playerID);
        //            insertItemsWeekly("$400 SBV - 8000 pts", 8000, 0, playerID);
        //            insertItemsWeekly("$500 SBV - 10000 pts", 10000, 0, playerID);
        //            insertItemsWeekly("$600 SBV - 12000 pts", 12000, 0, playerID);
        //            insertItemsWeekly("$700 SBV - 15000 pts", 15000, 0, playerID);
        //            insertItemsWeekly("$1500 SBV - 25000 pts", 25000, 0, playerID);
        //            insertItemsWeekly("$3000 SBV - 50000 pts", 50000, 0, playerID);

        //            insertItemsWeekly("$250 SFP - 5000 pts", 5000, 1, playerID);
        //            insertItemsWeekly("$400 SFP - 8000 pts", 8000, 0, playerID);
        //            insertItemsWeekly("$500 SFP - 10000 pts", 10000, 0, playerID);
        //            insertItemsWeekly("$600 SFP - 12000 pts", 12000, 0, playerID);
        //            insertItemsWeekly("$700 SFP - 15000 pts", 15000, 0, playerID);
        //            insertItemsWeekly("$1500 SFP - 25000 pts", 25000, 0, playerID);
        //            insertItemsWeekly("$3000 SFP - 50000 pts", 50000, 0, playerID);

        //            break;
        //        case 8000:
        //            insertItemsWeekly("$250 SBV - 5000 pts", 5000, 0, playerID);
        //            insertItemsWeekly("$400 SBV - 8000 pts", 8000, 1, playerID);
        //            insertItemsWeekly("$500 SBV - 10000 pts", 10000, 0, playerID);
        //            insertItemsWeekly("$600 SBV - 12000 pts", 12000, 0, playerID);
        //            insertItemsWeekly("$700 SBV - 15000 pts", 15000, 0, playerID);
        //            insertItemsWeekly("$1500 SBV - 25000 pts", 25000, 0, playerID);
        //            insertItemsWeekly("$3000 SBV - 50000 pts", 50000, 0, playerID);

        //            insertItemsWeekly("$250 SFP - 5000 pts", 5000, 0, playerID);
        //            insertItemsWeekly("$400 SFP - 8000 pts", 8000, 1, playerID);
        //            insertItemsWeekly("$500 SFP - 10000 pts", 10000, 0, playerID);
        //            insertItemsWeekly("$600 SFP - 12000 pts", 12000, 0, playerID);
        //            insertItemsWeekly("$700 SFP - 15000 pts", 15000, 0, playerID);
        //            insertItemsWeekly("$1500 SFP - 25000 pts", 25000, 0, playerID);
        //            insertItemsWeekly("$3000 SFP - 50000 pts", 50000, 0, playerID);

        //            break;
        //        case 10000:
        //            insertItemsWeekly("$250 SBV - 5000 pts", 5000, 0, playerID);
        //            insertItemsWeekly("$400 SBV - 8000 pts", 8000, 0, playerID);
        //            insertItemsWeekly("$500 SBV - 10000 pts", 10000, 1, playerID);
        //            insertItemsWeekly("$600 SBV - 12000 pts", 12000, 0, playerID);
        //            insertItemsWeekly("$700 SBV - 15000 pts", 15000, 0, playerID);
        //            insertItemsWeekly("$1500 SBV - 25000 pts", 25000, 0, playerID);
        //            insertItemsWeekly("$3000 SBV - 50000 pts", 50000, 0, playerID);

        //            insertItemsWeekly("$250 SFP - 5000 pts", 5000, 0, playerID);
        //            insertItemsWeekly("$400 SFP - 8000 pts", 8000, 0, playerID);
        //            insertItemsWeekly("$500 SFP - 10000 pts", 10000, 1, playerID);
        //            insertItemsWeekly("$600 SFP - 12000 pts", 12000, 0, playerID);
        //            insertItemsWeekly("$700 SFP - 15000 pts", 15000, 0, playerID);
        //            insertItemsWeekly("$1500 SFP - 25000 pts", 25000, 0, playerID);
        //            insertItemsWeekly("$3000 SFP - 50000 pts", 50000, 0, playerID);

        //            break;
        //        case 12000:
        //            insertItemsWeekly("$250 SBV - 5000 pts", 5000, 0, playerID);
        //            insertItemsWeekly("$400 SBV - 8000 pts", 8000, 0, playerID);
        //            insertItemsWeekly("$500 SBV - 10000 pts", 10000, 0, playerID);
        //            insertItemsWeekly("$600 SBV - 12000 pts", 12000, 1, playerID);
        //            insertItemsWeekly("$700 SBV - 15000 pts", 15000, 0, playerID);
        //            insertItemsWeekly("$1500 SBV - 25000 pts", 25000, 0, playerID);
        //            insertItemsWeekly("$3000 SBV - 50000 pts", 50000, 0, playerID);

        //            insertItemsWeekly("$250 SFP - 5000 pts", 5000, 0, playerID);
        //            insertItemsWeekly("$400 SFP - 8000 pts", 8000, 0, playerID);
        //            insertItemsWeekly("$500 SFP - 10000 pts", 10000, 0, playerID);
        //            insertItemsWeekly("$600 SFP - 12000 pts", 12000, 1, playerID);
        //            insertItemsWeekly("$700 SFP - 15000 pts", 15000, 0, playerID);
        //            insertItemsWeekly("$1500 SFP - 25000 pts", 25000, 0, playerID);
        //            insertItemsWeekly("$3000 SFP - 50000 pts", 50000, 0, playerID);

        //            break;
        //        case 15000:
        //            insertItemsWeekly("$250 SBV - 5000 pts", 5000, 0, playerID);
        //            insertItemsWeekly("$400 SBV - 8000 pts", 8000, 0, playerID);
        //            insertItemsWeekly("$500 SBV - 10000 pts", 10000, 0, playerID);
        //            insertItemsWeekly("$600 SBV - 12000 pts", 12000, 0, playerID);
        //            insertItemsWeekly("$700 SBV - 15000 pts", 15000, 1, playerID);
        //            insertItemsWeekly("$1500 SBV - 25000 pts", 25000, 0, playerID);
        //            insertItemsWeekly("$3000 SBV - 50000 pts", 50000, 0, playerID);

        //            insertItemsWeekly("$250 SFP - 5000 pts", 5000, 0, playerID);
        //            insertItemsWeekly("$400 SFP - 8000 pts", 8000, 0, playerID);
        //            insertItemsWeekly("$500 SFP - 10000 pts", 10000, 0, playerID);
        //            insertItemsWeekly("$600 SFP - 12000 pts", 12000, 0, playerID);
        //            insertItemsWeekly("$700 SFP - 15000 pts", 15000, 1, playerID);
        //            insertItemsWeekly("$1500 SFP - 25000 pts", 25000, 0, playerID);
        //            insertItemsWeekly("$3000 SFP - 50000 pts", 50000, 0, playerID);

        //            break;
        //        case 25000:
        //            insertItemsWeekly("$250 SBV - 5000 pts", 5000, 0, playerID);
        //            insertItemsWeekly("$400 SBV - 8000 pts", 8000, 0, playerID);
        //            insertItemsWeekly("$500 SBV - 10000 pts", 10000, 0, playerID);
        //            insertItemsWeekly("$600 SBV - 12000 pts", 12000, 0, playerID);
        //            insertItemsWeekly("$700 SBV - 15000 pts", 15000, 0, playerID);
        //            insertItemsWeekly("$1500 SBV - 25000 pts", 25000, 1, playerID);
        //            insertItemsWeekly("$3000 SBV - 50000 pts", 50000, 0, playerID);

        //            insertItemsWeekly("$250 SFP - 5000 pts", 5000, 0, playerID);
        //            insertItemsWeekly("$400 SFP - 8000 pts", 8000, 0, playerID);
        //            insertItemsWeekly("$500 SFP - 10000 pts", 10000, 0, playerID);
        //            insertItemsWeekly("$600 SFP - 12000 pts", 12000, 0, playerID);
        //            insertItemsWeekly("$700 SFP - 15000 pts", 15000, 0, playerID);
        //            insertItemsWeekly("$1500 SFP - 25000 pts", 25000, 1, playerID);
        //            insertItemsWeekly("$3000 SFP - 50000 pts", 50000, 0, playerID);

        //            break;
        //        case 50000:
        //            insertItemsWeekly("$250 SBV - 5000 pts", 5000, 0, playerID);
        //            insertItemsWeekly("$400 SBV - 8000 pts", 8000, 0, playerID);
        //            insertItemsWeekly("$500 SBV - 10000 pts", 10000, 0, playerID);
        //            insertItemsWeekly("$600 SBV - 12000 pts", 12000, 0, playerID);
        //            insertItemsWeekly("$700 SBV - 15000 pts", 15000, 0, playerID);
        //            insertItemsWeekly("$1500 SBV - 25000 pts", 25000, 0, playerID);
        //            insertItemsWeekly("$3000 SBV - 50000 pts", 50000, 1, playerID);

        //            insertItemsWeekly("$250 SFP - 5000 pts", 5000, 0, playerID);
        //            insertItemsWeekly("$400 SFP - 8000 pts", 8000, 0, playerID);
        //            insertItemsWeekly("$500 SFP - 10000 pts", 10000, 0, playerID);
        //            insertItemsWeekly("$600 SFP - 12000 pts", 12000, 0, playerID);
        //            insertItemsWeekly("$700 SFP - 15000 pts", 15000, 0, playerID);
        //            insertItemsWeekly("$1500 SFP - 25000 pts", 25000, 0, playerID);
        //            insertItemsWeekly("$3000 SFP - 50000 pts", 50000, 1, playerID);

        //            break;
        //        default:
        //            insertItemsWeekly("$250 SBV - 5000 pts", 5000, 0, playerID);
        //            insertItemsWeekly("$400 SBV - 8000 pts", 8000, 0, playerID);
        //            insertItemsWeekly("$500 SBV - 10000 pts", 10000, 0, playerID);
        //            insertItemsWeekly("$600 SBV - 12000 pts", 12000, 0, playerID);
        //            insertItemsWeekly("$700 SBV - 15000 pts", 15000, 0, playerID);
        //            insertItemsWeekly("$1500 SBV - 25000 pts", 25000, 0, playerID);
        //            insertItemsWeekly("$3000 SBV - 50000 pts", 50000, 0, playerID);

        //            insertItemsWeekly("$250 SFP - 5000 pts", 5000, 0, playerID);
        //            insertItemsWeekly("$400 SFP - 8000 pts", 8000, 0, playerID);
        //            insertItemsWeekly("$500 SFP - 10000 pts", 10000, 0, playerID);
        //            insertItemsWeekly("$600 SFP - 12000 pts", 12000, 0, playerID);
        //            insertItemsWeekly("$700 SFP - 15000 pts", 15000, 0, playerID);
        //            insertItemsWeekly("$1500 SFP - 25000 pts", 25000, 0, playerID);
        //            insertItemsWeekly("$3000 SFP - 50000 pts", 50000, 0, playerID);

        //            break;
        //    }
        //}



        public void switchWeeklyStatus(int weeklyPoints, int playerID)
        {
            DataSet ds = getStatusItemWeeklyByPID(playerID);
            int status8000 = Convert.ToInt32(ds.Tables[0].Rows[0]["Status"].ToString());
            int status10000 = Convert.ToInt32(ds.Tables[0].Rows[1]["Status"].ToString());
            int status20000 = Convert.ToInt32(ds.Tables[0].Rows[2]["Status"].ToString());
            int status30000 = Convert.ToInt32(ds.Tables[0].Rows[3]["Status"].ToString());
            int status60000 = Convert.ToInt32(ds.Tables[0].Rows[4]["Status"].ToString());


            switch (weeklyPoints)
            {

                case 8000:

                    if (status8000 != 2 && status8000 != 3)
                    {
                        updateItemWeeklyStatusFirst(playerID, 1, 8000);
                    }
                    break;
                case 10000:

                    if (status10000 != 2 && status10000 != 3)
                    {
                        updateItemWeeklyStatusFirst(playerID, 1, 10000);
                    }
                    break;
                case 20000:

                    if (status20000 != 2 && status20000 != 3)
                    {
                        updateItemWeeklyStatusFirst(playerID, 1, 20000);
                    }

                    break;
                case 30000:

                    if (status30000 != 2 && status30000 != 3)
                    {

                        updateItemWeeklyStatusFirst(playerID, 1, 30000);
                    }
                    break;
                case 60000:

                    if (status60000 != 2 && status60000 != 3)
                    {
                        updateItemWeeklyStatusFirst(playerID, 1, 60000);
                    }
                    break;


                default:
                    updateItemWeeklyStatusFirst(playerID, 0, 8000);
                    updateItemWeeklyStatusFirst(playerID, 0, 10000);
                    updateItemWeeklyStatusFirst(playerID, 0, 20000);
                    updateItemWeeklyStatusFirst(playerID, 0, 30000);
                    updateItemWeeklyStatusFirst(playerID, 0, 60000);

                    break;

            }
        }
        public void switchWeeklyPoints(int weeklyItemPoints, int playerID)
        {
            switch (weeklyItemPoints)
            {
                case 8000:
                    insertItemsWeekly("$200 SBV + $200 SFP - 8000 pts", 8000, 1, playerID);
                    insertItemsWeekly("$250 SBV + $250 SFP - 10000 pts", 10000, 0, playerID);
                    insertItemsWeekly("$400 SBV + $400 SFP - 20000 pts", 20000, 0, playerID);
                    insertItemsWeekly("$800 SBV + $700 SFP - 30000 pts", 30000, 0, playerID);
                    insertItemsWeekly("$2,000 SBV + $1,500 SFP - 60000 pts", 60000, 0, playerID);

                    insertItemsWeekly("$400 SBV - 8000 pts", 8000, 1, playerID);
                    insertItemsWeekly("$500 SBV - 10000 pts", 10000, 0, playerID);
                    insertItemsWeekly("$800 SBV - 20000 pts", 20000, 0, playerID);
                    insertItemsWeekly("$1,500 SBV - 30000 pts", 30000, 0, playerID);
                    insertItemsWeekly("$3,500 SBV - 60000 pts", 60000, 0, playerID);

                    break;

                case 10000:
                    insertItemsWeekly("$200 SBV + $200 SFP - 8000 pts", 8000, 0, playerID);
                    insertItemsWeekly("$250 SBV + $250 SFP - 10000 pts", 10000, 1, playerID);
                    insertItemsWeekly("$400 SBV + $400 SFP - 20000 pts", 20000, 0, playerID);
                    insertItemsWeekly("$800 SBV + $700 SFP - 30000 pts", 30000, 0, playerID);
                    insertItemsWeekly("$2,000 SBV + $1,500 SFP - 60000 pts", 60000, 0, playerID);

                    insertItemsWeekly("$400 SBV - 8000 pts", 8000, 0, playerID);
                    insertItemsWeekly("$500 SBV - 10000 pts", 10000, 1, playerID);
                    insertItemsWeekly("$800 SBV - 20000 pts", 20000, 0, playerID);
                    insertItemsWeekly("$1,500 SBV - 30000 pts", 30000, 0, playerID);
                    insertItemsWeekly("$3,500 SBV - 60000 pts", 60000, 0, playerID);

                    break;
                case 20000:
                    insertItemsWeekly("$200 SBV + $200 SFP - 8000 pts", 8000, 0, playerID);
                    insertItemsWeekly("$250 SBV + $250 SFP - 10000 pts", 10000, 0, playerID);
                    insertItemsWeekly("$400 SBV + $400 SFP - 20000 pts", 20000, 1, playerID);
                    insertItemsWeekly("$800 SBV + $700 SFP - 30000 pts", 30000, 0, playerID);
                    insertItemsWeekly("$2,000 SBV + $1,500 SFP - 60000 pts", 60000, 0, playerID);

                    insertItemsWeekly("$400 SBV - 8000 pts", 8000, 0, playerID);
                    insertItemsWeekly("$500 SBV - 10000 pts", 10000, 0, playerID);
                    insertItemsWeekly("$800 SBV - 20000 pts", 20000, 1, playerID);
                    insertItemsWeekly("$1,500 SBV - 30000 pts", 30000, 0, playerID);
                    insertItemsWeekly("$3,500 SBV - 60000 pts", 60000, 0, playerID);

                    break;
                case 30000:
                    insertItemsWeekly("$200 SBV + $200 SFP - 8000 pts", 8000, 0, playerID);
                    insertItemsWeekly("$250 SBV + $250 SFP - 10000 pts", 10000, 0, playerID);
                    insertItemsWeekly("$400 SBV + $400 SFP - 20000 pts", 20000, 0, playerID);
                    insertItemsWeekly("$800 SBV + $700 SFP - 30000 pts", 30000, 1, playerID);
                    insertItemsWeekly("$2,000 SBV + $1,500 SFP - 60000 pts", 60000, 0, playerID);

                    insertItemsWeekly("$400 SBV - 8000 pts", 8000, 0, playerID);
                    insertItemsWeekly("$500 SBV - 10000 pts", 10000, 0, playerID);
                    insertItemsWeekly("$800 SBV - 20000 pts", 20000, 0, playerID);
                    insertItemsWeekly("$1,500 SBV - 30000 pts", 30000, 1, playerID);
                    insertItemsWeekly("$3,500 SBV - 60000 pts", 60000, 0, playerID);

                    break;
                case 60000:
                    insertItemsWeekly("$200 SBV + $200 SFP - 8000 pts", 8000, 0, playerID);
                    insertItemsWeekly("$250 SBV + $250 SFP - 10000 pts", 10000, 0, playerID);
                    insertItemsWeekly("$400 SBV + $400 SFP - 20000 pts", 20000, 0, playerID);
                    insertItemsWeekly("$800 SBV + $700 SFP - 30000 pts", 30000, 0, playerID);
                    insertItemsWeekly("$2,000 SBV + $1,500 SFP - 60000 pts", 60000, 1, playerID);

                    insertItemsWeekly("$400 SBV - 8000 pts", 8000, 0, playerID);
                    insertItemsWeekly("$500 SBV - 10000 pts", 10000, 0, playerID);
                    insertItemsWeekly("$800 SBV - 20000 pts", 20000, 0, playerID);
                    insertItemsWeekly("$1,500 SBV - 30000 pts", 30000, 0, playerID);
                    insertItemsWeekly("$3,500 SBV - 60000 pts", 60000, 1, playerID);
                    break;

                default:
                    insertItemsWeekly("$200 SBV + $200 SFP - 8000 pts", 8000, 0, playerID);
                    insertItemsWeekly("$250 SBV + $250 SFP - 10000 pts", 10000, 0, playerID);
                    insertItemsWeekly("$400 SBV + $400 SFP - 20000 pts", 20000, 0, playerID);
                    insertItemsWeekly("$800 SBV + $700 SFP - 30000 pts", 30000, 0, playerID);
                    insertItemsWeekly("$2,000 SBV + $1,500 SFP - 60000 pts", 60000, 0, playerID);

                    insertItemsWeekly("$400 SBV - 8000 pts", 8000, 0, playerID);
                    insertItemsWeekly("$500 SBV - 10000 pts", 10000, 0, playerID);
                    insertItemsWeekly("$800 SBV - 20000 pts", 20000, 0, playerID);
                    insertItemsWeekly("$1,500 SBV - 30000 pts", 30000, 0, playerID);
                    insertItemsWeekly("$3,500 SBV - 60000 pts", 60000, 0, playerID);

                    break;
            }
        }
        //Change 20230619 Hantt end

        public void updatePlayerWeeklyPoints(int playerID, int weeklyPoints)
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFWeeklyBonus_UpdatePlayerWeeklyPoints", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@WeeklyPoints", weeklyPoints));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }
        public int getItemPointsWeekly(int weeklyItemPoints)
        {

            int points = 0;

            //Change 20230619 Hantt start
            //if (weeklyItemPoints >= 5000 && weeklyItemPoints < 8000)
            //{
            //    points = 5000;

            //}

            //if (weeklyItemPoints >= 8000 && weeklyItemPoints < 10000)
            //{
            //    points = 8000;

            //}

            //if (weeklyItemPoints >= 10000 && weeklyItemPoints < 12000)
            //{
            //    points = 10000;

            //}

            //if (weeklyItemPoints >= 12000 && weeklyItemPoints < 15000)
            //{
            //    points = 12000;

            //}

            //if (weeklyItemPoints >= 15000 && weeklyItemPoints < 25000)
            //{
            //    points = 15000;

            //}

            //if (weeklyItemPoints >= 25000 && weeklyItemPoints < 50000)
            //{
            //    points = 25000;

            //}

            //if (weeklyItemPoints >= 50000)
            //{
            //    points = 50000;

            //}

            if (weeklyItemPoints >= 8000 && weeklyItemPoints < 10000)
            {
                points = 8000;

            }

            if (weeklyItemPoints >= 10000 && weeklyItemPoints < 20000)
            {
                points = 10000;

            }

            if (weeklyItemPoints >= 20000 && weeklyItemPoints < 30000)
            {
                points = 20000;

            }

            if (weeklyItemPoints >= 30000 && weeklyItemPoints < 60000)
            {
                points = 30000;

            }

            if (weeklyItemPoints >= 60000)
            {
                points = 60000;

            }

            //Change 20230619 Hantt end

            return points;
        }

        public int selectPlayerPointsWeekly(int playerID)
        {
            int status = 0;
            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFWeeklyBonus_spSelectPlayerPoints", con))
                {
                    con.Open();
                    cmd.Connection = con;

                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
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
                            status = Convert.ToInt32(ds.Tables[0].Rows[0]["WeeklyPoints"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return status;
        }
        public void insertItemsWeekly(string itemName, int itemPoints, int status, int playerID)
        {

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFWeeklyBonus_ItemsInsert", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@ItemName", itemName));
                    cmd.Parameters.Add(new SqlParameter("@ItemPoints", itemPoints));
                    cmd.Parameters.Add(new SqlParameter("@Status", status));
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void insertPlayersWeekly(int playerID, string playerName, int weeklyPoints, int slotWeeklyPoints, int tableWeeklyPoints, DateTime gamingDate)
        {

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFWeeklyBonus_PlayersInsert", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@PlayerName", playerName));
                    cmd.Parameters.Add(new SqlParameter("@WeeklyPoints", weeklyPoints));
                    cmd.Parameters.Add(new SqlParameter("@SlotWeeklyPoints", slotWeeklyPoints));
                    cmd.Parameters.Add(new SqlParameter("@TableWeeklyPoints", tableWeeklyPoints));
                    cmd.Parameters.Add(new SqlParameter("@GamingDate", gamingDate));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int getStatusItemWeekly(int ID, int PlayerID)
        {

            int status = 0;
            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFWeeklyBonus_spGetItemsStatusByID", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@ID", ID));
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", PlayerID));
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
                            status = Convert.ToInt32(ds.Tables[0].Rows[0]["Status"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return status;

        }

        public int getPlayerPointsWeekly(int ID)
        {

            int playerPointsWeekly = 0;
            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFWeeklyBonus_GetPlayerPointsWeekly", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", ID));
                    //cmd.Parameters.Add(new SqlParameter("@Date1", lastThursday));
                    //cmd.Parameters.Add(new SqlParameter("@Date2", lastSunday));
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
                            playerPointsWeekly = Convert.ToInt32(ds.Tables[0].Rows[0]["WeeklyPointsEarned"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return playerPointsWeekly;

        }

        // Friday Bonus

        public void updatePlayerGamingDateFriday(int playerID, DateTime gamingDate)
        {
            //DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFFridayBonus_UpdatePlayerGamingDate", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@GamingDate", gamingDate));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public DataSet getTopItemIDByPIDFriday(int PlayerID)
        {
            //int status = 0;
            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFFridayBonus_spGetTopItemIDByPID", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    //cmd.Parameters.Add(new SqlParameter("@number", number));
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", PlayerID));
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
                    return ds;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

        public DataSet getItemIDByPointsFriday(int ItemPoints, int PlayerID)
        {
            //int status = 0;
            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFFridayBonus_spGetIDByPoints", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@Points", ItemPoints));
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", PlayerID));
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
                    return ds;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

        public void updateItemFridayStatusByID(int status, int ID)
        {

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFFridayBonus_UpdateItemStatusByID", con))
                {
                    con.Open();
                    cmd.Connection = con;

                    cmd.Parameters.Add(new SqlParameter("@Status", status));
                    cmd.Parameters.Add(new SqlParameter("@ID", ID));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }
        public DataSet getStatusItemFridayByPID(int PlayerID)
        {


            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFFridayBonus_spGetItemsStatusByPID", con))
                {
                    con.Open();
                    cmd.Connection = con;

                    cmd.Parameters.Add(new SqlParameter("@PlayerID", PlayerID));
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
                    return ds;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;

        }
        public DateTime getPlayerGamingDateFriday(int playerID)
        {
            DateTime gamingdate = DateTime.Now;

            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFFridayBonus_spGetPlayerGamingDate", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));

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
                            gamingdate = Convert.ToDateTime(ds.Tables[0].Rows[0]["GamingDate"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return gamingdate;

        }
        public int getStatusItemFriday(int ID, int PlayerID)
        {

            int status = 0;
            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFFridayBonus_spGetItemsStatusByID", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@ID", ID));
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", PlayerID));
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
                            status = Convert.ToInt32(ds.Tables[0].Rows[0]["Status"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return status;

        }
        public void updateItemFridayStatusSecond(int playerID, int itemID, int status)
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFFridayBonus_UpdateItemStatusSecond", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@playerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@itemID", itemID));
                    cmd.Parameters.Add(new SqlParameter("@status", status));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }
        public int selectPlayerPointsFriday(int playerID)
        {
            int status = 0;
            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFFridayBonus_spSelectPlayerPoints", con))
                {
                    con.Open();
                    cmd.Connection = con;

                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
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
                            status = Convert.ToInt32(ds.Tables[0].Rows[0]["FridayPoints"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return status;
        }
        public void updateItemFridayStatusFirst(int playerID, int status, int itemPoints)
        {

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFFridayBonus_UpdateItemStatusFirst", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@playerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@status", status));
                    cmd.Parameters.Add(new SqlParameter("@itemPoints", itemPoints));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }
        public int getStatusItemFridayByPoints(int ItemPoints, int PlayerID)
        {

            int status = 0;
            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFFridayBonus_spGetItemsStatusByPoints", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@ItemPoints", ItemPoints));
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", PlayerID));
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
                            status = Convert.ToInt32(ds.Tables[0].Rows[0]["Status"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return status;

        }
        public void switchFridayStatus(int fridayPoints, int playerID)
        {
            DataSet ds = getStatusItemFridayByPID(playerID);
            int statusMPV1000 = Convert.ToInt32(ds.Tables[0].Rows[0]["Status"].ToString());
            int statusMPV2000 = Convert.ToInt32(ds.Tables[0].Rows[1]["Status"].ToString());
            int statusMPV3000 = Convert.ToInt32(ds.Tables[0].Rows[2]["Status"].ToString());
            int statusMPV4000 = Convert.ToInt32(ds.Tables[0].Rows[3]["Status"].ToString());
            int statusMPV5000 = Convert.ToInt32(ds.Tables[0].Rows[4]["Status"].ToString());
            int statusMPV6000 = Convert.ToInt32(ds.Tables[0].Rows[5]["Status"].ToString());
            int statusMPV7000 = Convert.ToInt32(ds.Tables[0].Rows[6]["Status"].ToString());
            int statusMPV8000 = Convert.ToInt32(ds.Tables[0].Rows[7]["Status"].ToString());
            int statusMPV9000 = Convert.ToInt32(ds.Tables[0].Rows[8]["Status"].ToString());
            int statusMPV10000 = Convert.ToInt32(ds.Tables[0].Rows[9]["Status"].ToString());
            int statusMPV11000 = Convert.ToInt32(ds.Tables[0].Rows[10]["Status"].ToString());
            int statusMPV12000 = Convert.ToInt32(ds.Tables[0].Rows[11]["Status"].ToString());
            int statusMPV15000 = Convert.ToInt32(ds.Tables[0].Rows[12]["Status"].ToString());
            int statusMPV25000 = Convert.ToInt32(ds.Tables[0].Rows[13]["Status"].ToString());

            int statusSFP1000 = Convert.ToInt32(ds.Tables[0].Rows[14]["Status"].ToString());
            int statusSFP2000 = Convert.ToInt32(ds.Tables[0].Rows[15]["Status"].ToString());
            int statusSFP3000 = Convert.ToInt32(ds.Tables[0].Rows[16]["Status"].ToString());
            int statusSFP4000 = Convert.ToInt32(ds.Tables[0].Rows[17]["Status"].ToString());
            int statusSFP5000 = Convert.ToInt32(ds.Tables[0].Rows[18]["Status"].ToString());
            int statusSFP6000 = Convert.ToInt32(ds.Tables[0].Rows[19]["Status"].ToString());
            int statusSFP7000 = Convert.ToInt32(ds.Tables[0].Rows[20]["Status"].ToString());
            int statusSFP8000 = Convert.ToInt32(ds.Tables[0].Rows[21]["Status"].ToString());
            int statusSFP9000 = Convert.ToInt32(ds.Tables[0].Rows[22]["Status"].ToString());
            int statusSFP10000 = Convert.ToInt32(ds.Tables[0].Rows[23]["Status"].ToString());
            int statusSFP11000 = Convert.ToInt32(ds.Tables[0].Rows[24]["Status"].ToString());
            int statusSFP12000 = Convert.ToInt32(ds.Tables[0].Rows[25]["Status"].ToString());
            int statusSFP15000 = Convert.ToInt32(ds.Tables[0].Rows[26]["Status"].ToString());
            int statusSFP25000 = Convert.ToInt32(ds.Tables[0].Rows[27]["Status"].ToString());


            switch (fridayPoints)
            {

                case 1000:

                    if (statusMPV1000 != 2 && statusSFP1000 != 2 && statusMPV1000 != 3 && statusSFP1000 != 3)
                    {
                        updateItemFridayStatusFirst(playerID, 1, 1000);
                    }


                    break;
                case 2000:

                    if (statusMPV2000 != 2 && statusSFP2000 != 2 && statusMPV2000 != 3 && statusSFP2000 != 3)
                    {
                        updateItemFridayStatusFirst(playerID, 1, 2000);
                    }

                    break;
                case 3000:

                    if (statusMPV3000 != 2 && statusSFP3000 != 2 && statusMPV3000 != 3 && statusSFP3000 != 3)
                    {
                        updateItemFridayStatusFirst(playerID, 1, 3000);
                    }
                    break;
                case 4000:

                    if (statusMPV4000 != 2 && statusSFP4000 != 2 && statusMPV4000 != 3 && statusSFP4000 != 3)
                    {
                        updateItemFridayStatusFirst(playerID, 1, 4000);
                    }

                    break;
                case 5000:

                    if (statusMPV5000 != 2 && statusSFP5000 != 2 && statusMPV5000 != 3 && statusSFP5000 != 3)
                    {
                        updateItemFridayStatusFirst(playerID, 1, 5000);
                    }
                    break;
                case 6000:

                    if (statusMPV6000 != 2 && statusSFP6000 != 2 && statusMPV6000 != 3 && statusSFP6000 != 3)
                    {
                        updateItemFridayStatusFirst(playerID, 1, 6000);
                    }
                    break;
                case 7000:

                    if (statusMPV7000 != 2 && statusSFP7000 != 2 && statusMPV7000 != 3 && statusSFP7000 != 3)
                    {
                        updateItemFridayStatusFirst(playerID, 1, 7000);
                    }
                    break;
                case 8000:

                    if (statusMPV8000 != 2 && statusSFP8000 != 2 && statusMPV8000 != 3 && statusSFP8000 != 3)
                    {
                        updateItemFridayStatusFirst(playerID, 1, 8000);
                    }
                    break;
                case 9000:

                    if (statusMPV9000 != 2 && statusSFP9000 != 2 && statusMPV9000 != 3 && statusSFP9000 != 3)
                    {
                        updateItemFridayStatusFirst(playerID, 1, 9000);
                    }
                    break;
                case 10000:

                    if (statusMPV10000 != 2 && statusSFP10000 != 2 && statusMPV10000 != 3 && statusSFP10000 != 3)
                    {
                        updateItemFridayStatusFirst(playerID, 1, 10000);
                    }
                    break;
                case 11000:

                    if (statusMPV11000 != 2 && statusSFP11000 != 2 && statusMPV11000 != 3 && statusSFP11000 != 3)
                    {
                        updateItemFridayStatusFirst(playerID, 1, 11000);
                    }
                    break;
                case 12000:

                    if (statusMPV12000 != 2 && statusSFP12000 != 2 && statusMPV12000 != 3 && statusSFP12000 != 3)
                    {
                        updateItemFridayStatusFirst(playerID, 1, 12000);
                    }
                    break;
                case 15000:

                    if (statusMPV15000 != 2 && statusSFP15000 != 2 && statusMPV15000 != 3 && statusSFP15000 != 3)
                    {
                        updateItemFridayStatusFirst(playerID, 1, 15000);
                    }
                    break;
                case 25000:

                    if (statusMPV25000 != 2 && statusSFP25000 != 2 && statusMPV25000 != 3 && statusSFP25000 != 3)
                    {
                        updateItemFridayStatusFirst(playerID, 1, 25000);
                    }
                    break;

                default:
                    updateItemFridayStatusFirst(playerID, 0, 1000);
                    updateItemFridayStatusFirst(playerID, 0, 2000);
                    updateItemFridayStatusFirst(playerID, 0, 3000);
                    updateItemFridayStatusFirst(playerID, 0, 4000);
                    updateItemFridayStatusFirst(playerID, 0, 5000);
                    updateItemFridayStatusFirst(playerID, 0, 6000);
                    updateItemFridayStatusFirst(playerID, 0, 7000);
                    updateItemFridayStatusFirst(playerID, 0, 8000);
                    updateItemFridayStatusFirst(playerID, 0, 9000);
                    updateItemFridayStatusFirst(playerID, 0, 10000);
                    updateItemFridayStatusFirst(playerID, 0, 11000);
                    updateItemFridayStatusFirst(playerID, 0, 12000);
                    updateItemFridayStatusFirst(playerID, 0, 15000);
                    updateItemFridayStatusFirst(playerID, 0, 25000);
                    break;
            }
        }
        public void insertItemsFriday(string itemName, int itemPoints, int status, int playerID)
        {

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFFridayBonus_ItemsInsert", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@ItemName", itemName));
                    cmd.Parameters.Add(new SqlParameter("@ItemPoints", itemPoints));
                    cmd.Parameters.Add(new SqlParameter("@Status", status));
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void switchFridayPoints(int fridayItemPoints, int playerID)
        {
            switch (fridayItemPoints)
            {
                case 1000:
                    insertItemsFriday("$50 SBV - 1000 pts", 1000, 1, playerID);
                    insertItemsFriday("$100 SBV - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SBV - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SBV - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SBV - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SBV - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SBV - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SBV - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SBV - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SBV - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SBV - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SBV - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SBV - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SBV - 25000 pts", 25000, 0, playerID);

                    insertItemsFriday("$50 SFP - 1000 pts", 1000, 1, playerID);
                    insertItemsFriday("$100 SFP - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SFP - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SFP - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SFP - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SFP - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SFP - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SFP - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SFP - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SFP - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SFP - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SFP - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SFP - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SFP - 25000 pts", 25000, 0, playerID);

                    break;
                case 2000:
                    insertItemsFriday("$50 SBV - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SBV - 2000 pts", 2000, 1, playerID);
                    insertItemsFriday("$150 SBV - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SBV - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SBV - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SBV - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SBV - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SBV - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SBV - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SBV - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SBV - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SBV - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SBV - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SBV - 25000 pts", 25000, 0, playerID);

                    insertItemsFriday("$50 SFP - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SFP - 2000 pts", 2000, 1, playerID);
                    insertItemsFriday("$150 SFP - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SFP - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SFP - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SFP - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SFP - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SFP - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SFP - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SFP - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SFP - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SFP - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SFP - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SFP - 25000 pts", 25000, 0, playerID);

                    break;
                case 3000:
                    insertItemsFriday("$50 SBV - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SBV - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SBV - 3000 pts", 3000, 1, playerID);
                    insertItemsFriday("$200 SBV - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SBV - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SBV - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SBV - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SBV - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SBV - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SBV - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SBV - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SBV - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SBV - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SBV - 25000 pts", 25000, 0, playerID);

                    insertItemsFriday("$50 SFP - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SFP - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SFP - 3000 pts", 3000, 1, playerID);
                    insertItemsFriday("$200 SFP - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SFP - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SFP - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SFP - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SFP - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SFP - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SFP - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SFP - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SFP - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SFP - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SFP - 25000 pts", 25000, 0, playerID);

                    break;
                case 4000:
                    insertItemsFriday("$50 SBV - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SBV - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SBV - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SBV - 4000 pts", 4000, 1, playerID);
                    insertItemsFriday("$250 SBV - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SBV - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SBV - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SBV - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SBV - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SBV - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SBV - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SBV - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SBV - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SBV - 25000 pts", 25000, 0, playerID);

                    insertItemsFriday("$50 SFP - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SFP - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SFP - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SFP - 4000 pts", 4000, 1, playerID);
                    insertItemsFriday("$250 SFP - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SFP - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SFP - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SFP - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SFP - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SFP - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SFP - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SFP - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SFP - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SFP - 25000 pts", 25000, 0, playerID);

                    break;
                case 5000:
                    insertItemsFriday("$50 SBV - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SBV - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SBV - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SBV - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SBV - 5000 pts", 5000, 1, playerID);
                    insertItemsFriday("$300 SBV - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SBV - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SBV - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SBV - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SBV - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SBV - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SBV - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SBV - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SBV - 25000 pts", 25000, 0, playerID);

                    insertItemsFriday("$50 SFP - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SFP - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SFP - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SFP - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SFP - 5000 pts", 5000, 1, playerID);
                    insertItemsFriday("$300 SFP - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SFP - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SFP - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SFP - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SFP - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SFP - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SFP - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SFP - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SFP - 25000 pts", 25000, 0, playerID);

                    break;
                case 6000:
                    insertItemsFriday("$50 SBV - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SBV - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SBV - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SBV - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SBV - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SBV - 6000 pts", 6000, 1, playerID);
                    insertItemsFriday("$350 SBV - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SBV - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SBV - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SBV - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SBV - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SBV - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SBV - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SBV - 25000 pts", 25000, 0, playerID);

                    insertItemsFriday("$50 SFP - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SFP - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SFP - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SFP - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SFP - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SFP - 6000 pts", 6000, 1, playerID);
                    insertItemsFriday("$350 SFP - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SFP - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SFP - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SFP - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SFP - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SFP - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SFP - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SFP - 25000 pts", 25000, 0, playerID);

                    break;
                case 7000:
                    insertItemsFriday("$50 SBV - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SBV - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SBV - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SBV - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SBV - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SBV - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SBV - 7000 pts", 7000, 1, playerID);
                    insertItemsFriday("$400 SBV - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SBV - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SBV - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SBV - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SBV - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SBV - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SBV - 25000 pts", 25000, 0, playerID);

                    insertItemsFriday("$50 SFP - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SFP - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SFP - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SFP - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SFP - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SFP - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SFP - 7000 pts", 7000, 1, playerID);
                    insertItemsFriday("$400 SFP - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SFP - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SFP - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SFP - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SFP - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SFP - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SFP - 25000 pts", 25000, 0, playerID);

                    break;
                case 8000:
                    insertItemsFriday("$50 SBV - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SBV - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SBV - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SBV - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SBV - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SBV - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SBV - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SBV - 8000 pts", 8000, 1, playerID);
                    insertItemsFriday("$450 SBV - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SBV - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SBV - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SBV - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SBV - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SBV - 25000 pts", 25000, 0, playerID);

                    insertItemsFriday("$50 SFP - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SFP - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SFP - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SFP - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SFP - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SFP - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SFP - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SFP - 8000 pts", 8000, 1, playerID);
                    insertItemsFriday("$450 SFP - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SFP - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SFP - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SFP - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SFP - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SFP - 25000 pts", 25000, 0, playerID);

                    break;
                case 9000:
                    insertItemsFriday("$50 SBV - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SBV - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SBV - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SBV - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SBV - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SBV - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SBV - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SBV - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SBV - 9000 pts", 9000, 1, playerID);
                    insertItemsFriday("$500 SBV - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SBV - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SBV - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SBV - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SBV - 25000 pts", 25000, 0, playerID);

                    insertItemsFriday("$50 SFP - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SFP - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SFP - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SFP - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SFP - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SFP - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SFP - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SFP - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SFP - 9000 pts", 9000, 1, playerID);
                    insertItemsFriday("$500 SFP - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SFP - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SFP - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SFP - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SFP - 25000 pts", 25000, 0, playerID);

                    break;
                case 10000:
                    insertItemsFriday("$50 SBV - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SBV - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SBV - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SBV - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SBV - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SBV - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SBV - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SBV - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SBV - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SBV - 10000 pts", 10000, 1, playerID);
                    insertItemsFriday("$550 SBV - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SBV - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SBV - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SBV - 25000 pts", 25000, 0, playerID);

                    insertItemsFriday("$50 SFP - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SFP - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SFP - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SFP - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SFP - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SFP - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SFP - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SFP - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SFP - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SFP - 10000 pts", 10000, 1, playerID);
                    insertItemsFriday("$550 SFP - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SFP - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SFP - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SFP - 25000 pts", 25000, 0, playerID);

                    break;
                case 11000:
                    insertItemsFriday("$50 SBV - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SBV - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SBV - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SBV - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SBV - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SBV - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SBV - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SBV - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SBV - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SBV - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SBV - 11000 pts", 11000, 1, playerID);
                    insertItemsFriday("$600 SBV - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SBV - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SBV - 25000 pts", 25000, 0, playerID);

                    insertItemsFriday("$50 SFP - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SFP - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SFP - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SFP - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SFP - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SFP - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SFP - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SFP - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SFP - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SFP - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SFP - 11000 pts", 11000, 1, playerID);
                    insertItemsFriday("$600 SFP - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SFP - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SFP - 25000 pts", 25000, 0, playerID);

                    break;
                case 12000:
                    insertItemsFriday("$50 SBV - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SBV - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SBV - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SBV - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SBV - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SBV - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SBV - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SBV - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SBV - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SBV - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SBV - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SBV - 12000 pts", 12000, 1, playerID);
                    insertItemsFriday("$750 SBV - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SBV - 25000 pts", 25000, 0, playerID);

                    insertItemsFriday("$50 SFP - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SFP - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SFP - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SFP - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SFP - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SFP - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SFP - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SFP - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SFP - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SFP - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SFP - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SFP - 12000 pts", 12000, 1, playerID);
                    insertItemsFriday("$750 SFP - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SFP - 25000 pts", 25000, 0, playerID);

                    break;
                case 15000:
                    insertItemsFriday("$50 SBV - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SBV - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SBV - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SBV - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SBV - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SBV - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SBV - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SBV - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SBV - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SBV - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SBV - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SBV - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SBV - 15000 pts", 15000, 1, playerID);
                    insertItemsFriday("$1500 SBV - 25000 pts", 25000, 0, playerID);

                    insertItemsFriday("$50 SFP - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SFP - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SFP - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SFP - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SFP - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SFP - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SFP - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SFP - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SFP - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SFP - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SFP - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SFP - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SFP - 15000 pts", 15000, 1, playerID);
                    insertItemsFriday("$1500 SFP - 25000 pts", 25000, 0, playerID);

                    break;
                case 25000:
                    insertItemsFriday("$50 SBV - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SBV - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SBV - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SBV - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SBV - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SBV - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SBV - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SBV - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SBV - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SBV - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SBV - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SBV - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SBV - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SBV - 25000 pts", 25000, 1, playerID);

                    insertItemsFriday("$50 SFP - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SFP - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SFP - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SFP - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SFP - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SFP - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SFP - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SFP - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SFP - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SFP - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SFP - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SFP - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SFP - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SFP - 25000 pts", 25000, 1, playerID);

                    break;
                default:
                    insertItemsFriday("$50 SBV - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SBV - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SBV - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SBV - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SBV - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SBV - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SBV - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SBV - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SBV - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SBV - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SBV - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SBV - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SBV - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SBV - 25000 pts", 25000, 0, playerID);

                    insertItemsFriday("$50 SFP - 1000 pts", 1000, 0, playerID);
                    insertItemsFriday("$100 SFP - 2000 pts", 2000, 0, playerID);
                    insertItemsFriday("$150 SFP - 3000 pts", 3000, 0, playerID);
                    insertItemsFriday("$200 SFP - 4000 pts", 4000, 0, playerID);
                    insertItemsFriday("$250 SFP - 5000 pts", 5000, 0, playerID);
                    insertItemsFriday("$300 SFP - 6000 pts", 6000, 0, playerID);
                    insertItemsFriday("$350 SFP - 7000 pts", 7000, 0, playerID);
                    insertItemsFriday("$400 SFP - 8000 pts", 8000, 0, playerID);
                    insertItemsFriday("$450 SFP - 9000 pts", 9000, 0, playerID);
                    insertItemsFriday("$500 SFP - 10000 pts", 10000, 0, playerID);
                    insertItemsFriday("$550 SFP - 11000 pts", 11000, 0, playerID);
                    insertItemsFriday("$600 SFP - 12000 pts", 12000, 0, playerID);
                    insertItemsFriday("$750 SFP - 15000 pts", 15000, 0, playerID);
                    insertItemsFriday("$1500 SFP - 25000 pts", 25000, 0, playerID);

                    break;
            }
        }
        public void updatePlayerFridayPoints(int playerID, int fridayPoints)
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFFridayBonus_UpdatePlayerPoints", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@FridayPoints", fridayPoints));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }
        public void insertPlayersFriday(int playerID, string playerName, int fridayPoints, int slotFridayPoints, int tableFridayPoints, DateTime gamingDate)
        {

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFFridayBonus_PlayersInsert", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@PlayerName", playerName));
                    cmd.Parameters.Add(new SqlParameter("@FridayPoints", fridayPoints));
                    cmd.Parameters.Add(new SqlParameter("@SlotFridayPoints", slotFridayPoints));
                    cmd.Parameters.Add(new SqlParameter("@TableFridayPoints", tableFridayPoints));
                    cmd.Parameters.Add(new SqlParameter("@GamingDate", gamingDate));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public int getItemPointsFriday(int fridayItemPoints)
        {

            int points = 0;

            if (fridayItemPoints >= 1000 && fridayItemPoints < 2000)
            {
                points = 1000;

            }

            if (fridayItemPoints >= 2000 && fridayItemPoints < 3000)
            {
                points = 2000;

            }

            if (fridayItemPoints >= 3000 && fridayItemPoints < 4000)
            {
                points = 3000;

            }

            if (fridayItemPoints >= 4000 && fridayItemPoints < 5000)
            {
                points = 4000;

            }

            if (fridayItemPoints >= 5000 && fridayItemPoints < 6000)
            {
                points = 5000;

            }

            if (fridayItemPoints >= 6000 && fridayItemPoints < 7000)
            {
                points = 6000;

            }

            if (fridayItemPoints >= 7000 && fridayItemPoints < 8000)
            {
                points = 7000;

            }

            if (fridayItemPoints >= 8000 && fridayItemPoints < 9000)
            {
                points = 8000;

            }

            if (fridayItemPoints >= 9000 && fridayItemPoints < 10000)
            {
                points = 9000;

            }

            if (fridayItemPoints >= 10000 && fridayItemPoints < 11000)
            {
                points = 10000;

            }

            if (fridayItemPoints >= 11000 && fridayItemPoints < 12000)
            {
                points = 11000;

            }

            if (fridayItemPoints >= 12000 && fridayItemPoints < 15000)
            {
                points = 12000;

            }

            if (fridayItemPoints >= 15000 && fridayItemPoints < 25000)
            {
                points = 15000;

            }

            if (fridayItemPoints >= 25000)
            {
                points = 25000;

            }

            return points;
        }
        public int getPlayerPointsFriday(int ID, DateTime lastSunday, DateTime lastThursday)
        {
            int playerPointsFriday = 0;
            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFFridayBonus_GetPlayerPointsFriday", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", ID));
                    cmd.Parameters.Add(new SqlParameter("@Date1", lastSunday));
                    cmd.Parameters.Add(new SqlParameter("@Date2", lastThursday));
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
                            playerPointsFriday = Convert.ToInt32(ds.Tables[0].Rows[0]["FridayPointsEarned"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return playerPointsFriday;
        }
        public int getItemsRowCountFriday(int playerID)
        {

            int count = 0;
            DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFFridayBonus_GetItemsRowCount", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
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
                        if (ds.Tables[0].Rows[0]["RowCount"].ToString() != "0")
                        {
                            count = Convert.ToInt32(ds.Tables[0].Rows[0]["RowCount"]);
                        }

                    }
                }



            }


            else
            {
                count = 0;
            }


            return count;

        }
        public int getPlayersRowCountFriday(int playerID)
        {

            int count = 0;
            DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFFridayBonus_GetPlayersRowCount", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
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
                        if (ds.Tables[0].Rows[0]["RowCount"].ToString() != "0")
                        {
                            count = Convert.ToInt32(ds.Tables[0].Rows[0]["RowCount"]);
                        }

                    }
                }
            }
            else
            {
                count = 0;
            }

            return count;

        }

        // Points Redemption
        public int getItemPoints(int ID)
        {
            int itemPoints = 0;
            DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("CasinoKiosk_spGetItemPoints", con))
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
                            itemPoints = Convert.ToInt32(ds.Tables[0].Rows[0]["ItemPoints"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return itemPoints;
        }
        public string getItemName(int ID)
        {
            string itemName = "";
            DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("CasinoKiosk_spGetItemNameByID", con))
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
                            itemName = ds.Tables[0].Rows[0]["ItemName"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return itemName;
        }

        public string getTierName(int ID)
        {
            string tierName = "";
            DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SpCheckPlayerTier", con))
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
                            tierName = ds.Tables[0].Rows[0]["Tier"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return tierName;
        }

        public int getItemQuantity(int ID)
        {
            int quan = 0;
            DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("CasinoKiosk_spGetItemQuantityByID", con))
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
                            quan = Convert.ToInt32(ds.Tables[0].Rows[0]["Quantity"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return quan;
        }

        public string getItemImageUrl(int ID)
        {
            string imageUrl = "";
            DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("CasinoKiosk_spGetItemImageUrl", con))
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
                            imageUrl = ds.Tables[0].Rows[0]["imageUrl"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return imageUrl;
        }

        public void updateIsActive(int ID, bool isActive)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("CasinoKiosk_spSetIsActiveByID", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@ID", ID));
                    cmd.Parameters.Add(new SqlParameter("@isActive", isActive));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void updateQuantity(int ID, int Quantity)
        {


            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("CasinoKiosk_spSetQuantityByID", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@ID", ID));
                    cmd.Parameters.Add(new SqlParameter("@Quantity", Quantity));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }


        }

        public void returnVoidedPoints(int playerID, int voidedPoints)
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Proc_PlayerPointReturnVoidPoints", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@@nPlayerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@@nVoidPoints", voidedPoints));
                    cmd.Parameters.Add(new SqlParameter("@@nReasonNumber", 1));
                    cmd.Parameters.Add(new SqlParameter("@@szComment", "Returned voided points for" + playerID.ToString() + "- Amount: " + voidedPoints.ToString()));
                    cmd.Parameters.Add(new SqlParameter("@@nUserID", 31580));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();

                }
            }
        }

        public void updateVoidedStatusByID(int ID, string voidedStatus)
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CkdbContext"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("CasinoKiosk_spSetVoidedStatus", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@ID", ID));
                    cmd.Parameters.Add(new SqlParameter("@voidedStatus", voidedStatus));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }

        public string getStatusRedemptionByID(int ID)
        {
            string voidedStatus = "";
            DataSet ds;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CkdbContext"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("MFRedemption_spGetLogVoidedStatusByID", con))
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
                            voidedStatus = ds.Tables[0].Rows[0]["voidedStatus"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return voidedStatus;
        }
        //Add 20230109 Hantt end
        public void updateRedeemtionVoidedStatusByID(int ID, string voidedStatus)
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CkdbContext"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("MFRedemption_spSetVoidedStatus", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@ID", ID));
                    cmd.Parameters.Add(new SqlParameter("@voidedStatus", voidedStatus));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }

        //Change 20230109 Hantt start
        public void updateVoidedPersonByID(int ID, string voidedPerson)
        //Change 20230109 Hantt end
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CkdbContext"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("CasinoKiosk_spSetVoidedPerson", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@ID", ID));
                    cmd.Parameters.Add(new SqlParameter("@voidedPerson", voidedPerson));
                    //Add 20230109 Hantt start
                    cmd.Parameters.Add(new SqlParameter("@voidedTime", DateTime.Now.ToString()));
                    //Add 20230109 Hantt end
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }
        public void updateRedeemtionVoidedPersonByID(int ID, string voidedPerson)
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CkdbContext"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("MFRedemption_spSetVoidedPerson", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@ID", ID));
                    cmd.Parameters.Add(new SqlParameter("@voidedPerson", voidedPerson));
                    //Add 20230109 Hantt start
                    cmd.Parameters.Add(new SqlParameter("@voidedTime", DateTime.Now.ToString()));
                    //Add 20230109 Hantt end
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
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

        public string getLogVoidedStatusByID(int ID)
        {
            string voidedStatus = "";
            DataSet ds;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CkdbContext"].ToString()))
            {
                //Change 20230109 Hantt start
                //using (SqlCommand cmd = new SqlCommand("CasinoKiosk_spGetVoidedPersonByIDMFPoint", con))
                using (SqlCommand cmd = new SqlCommand("CasinoKiosk_spGetStatus", con))
                //Change 20230109 Hantt end
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
                            voidedStatus = ds.Tables[0].Rows[0]["voidedStatus"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return voidedStatus;
        }
        

        public string getVoidedPersonByID(int ID)
        {
            string voidedPerson = "";
            DataSet ds;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CkdbContext"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("CasinoKiosk_spGetVoidedPersonByID", con))
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
                            voidedPerson = ds.Tables[0].Rows[0]["voidedPerson"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return voidedPerson;
        }

        //Add 20230206 Hantt start
        public string getVoidedPersonByIDMFPoint(int ID)
        {
            string voidedPerson = "";
            DataSet ds;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CkdbContext"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("CasinoKiosk_spGetVoidedPersonByIDMFPoint", con))
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
                            voidedPerson = ds.Tables[0].Rows[0]["voidedPerson"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return voidedPerson;
        }
        //Add 20230206 Hantt end


        public void insertPlayersPoints(int playerID, int dailyPoints, int weeklyPoints, int fridayPoints, DateTime gamingDate)
        {

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFBonus_InsertPlayerPoints", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@DailyPoints", dailyPoints));
                    cmd.Parameters.Add(new SqlParameter("@WeeklyPoints", weeklyPoints));
                    cmd.Parameters.Add(new SqlParameter("@FridayPoints", fridayPoints));

                    cmd.Parameters.Add(new SqlParameter("@GamingDate", gamingDate));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void updatePlayerPoints(int playerID, int dailyPoints, int weeklyPoints, int fridayPoints, DateTime gamingDate)
        {

            //DataSet ds;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CkdbContext"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("MFBonus_UpdatePlayerPoints", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@playerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@dailyPoints", dailyPoints));
                    cmd.Parameters.Add(new SqlParameter("@weeklyPoints", weeklyPoints));
                    cmd.Parameters.Add(new SqlParameter("@fridayPoints", fridayPoints));

                    cmd.Parameters.Add(new SqlParameter("@gamingDate", gamingDate));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }

        public int getPlayerPointsRowCount(int playerID)
        {

            int count = 0;
            DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFBonus_GetPlayerPointsRowCount", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
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
                        if (ds.Tables[0].Rows[0]["RowCount"].ToString() != "0")
                        {
                            count = Convert.ToInt32(ds.Tables[0].Rows[0]["RowCount"]);
                        }

                    }
                }
            }
            else
            {
                count = 0;
            }

            return count;

        }

        // Slot Bonus

        public int getSlotPointsDaily(int playerID, DateTime Date)
        {

            int playerPoints = 0;
            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFBonus_GetSlotPointsDaily", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@Date", Date));
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
                            playerPoints = Convert.ToInt32(ds.Tables[0].Rows[0]["DailySlotPointsEarned"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return playerPoints;

        }

        public int getSlotPointsRedeemed(int playerID, DateTime Date)
        {

            int playerPoints = 0;
            DataSet ds;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFBonus_GetSlotRedeemedPoints", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
                    cmd.Parameters.Add(new SqlParameter("@Date", Date));
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
                            playerPoints = Convert.ToInt32(ds.Tables[0].Rows[0]["SlotRedeemedPoints"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return playerPoints;

        }


        public int getPlayersRowCountSlot(int playerID)
        {

            int count = 0;
            DataSet ds;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MFBonus_SlotDailyPlayers", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
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
                        if (ds.Tables[0].Rows[0]["RowCount"].ToString() != "0")
                        {
                            count = Convert.ToInt32(ds.Tables[0].Rows[0]["RowCount"]);
                        }

                    }
                }



            }


            else
            {
                count = 0;
            }


            return count;
        }
      
    }
}
