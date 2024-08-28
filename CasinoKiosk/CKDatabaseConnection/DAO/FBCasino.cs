using CKDatabaseConnection.DAO;
using CKDatabaseConnection.EF;
using CKDatabaseConnection.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKDatabaseConnection.DAO
{
    public class FBCasino
    {
        #region FB Business

        //get fb items by playerid
        CKdbContext db = new CKdbContext();
        CKFunction function = new CKFunction();

        //db casinokiosk
        string connectionString = ConfigurationManager.ConnectionStrings["CKdbContext"].ConnectionString;
        //string connectionString = ConfigurationManager.ConnectionStrings["CKdbContexttest"].ConnectionString;


        //db playermanagement
        //string connectionString1 = ConfigurationManager.ConnectionStrings["PlayerManagementConStr"].ConnectionString;
        public List<FBItem> getFBItems(int playerID, string fromdate = "", string todate = "")
        {
            string playerName = function.getPlayerName(playerID);

            var fbitemlist = new List<FBItem>();
            try
            {
                var dt = new DataTable();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("spCheckExpiredCasinoKiosk_FBTickets", con))
                    {
                        con.Open();
                        cmd.Connection = con;
                        cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
                //fbitemlist=dt.AsEnumerable().Where(p => ((playerID > 0) ? p["PlayerID"] == playerID : true) && ((fromdate != "" && todate != "") ? (
                //DateTime.ParseExact(p["issuedDate"], "dd-MM-yyyy", CultureInfo.CurrentCulture) >= DateTime.Parse(fromdate, CultureInfo.CurrentCulture)
                //&&
                //DateTime.ParseExact(p.createdDate, "dd-MM-yyyy", CultureInfo.CurrentCulture) <= DateTime.Parse(todate, CultureInfo.CurrentCulture)
                //) : true)
                //).OrderByDescending(x => x.ID).ToList();
                fbitemlist = dt.AsEnumerable().Select(
                    s => new FBItem()
                    {
                        ID = int.Parse(s["ID"].ToString()),
                        ItemName = s["ItemName"].ToString(),
                        itemValue = s["TicketValue"].ToString(),
                        issuedDate = s["issuedDate"].ToString(),
                        //playerID = s["PlayerID"].ToString(),
                        status = bool.Parse(s["status"].ToString())

                    }).ToList();
            }
            catch (Exception ex)
            { }
            return fbitemlist;
        }

        public List<FBItem> getAllFBItems(int playerID = 0, string fromdate = "", string todate = "")
        {
            if (fromdate == "" || todate == "")
            {
                fromdate = DateTime.Today.AddDays(-30).ToString("yyyy-MM-dd");
                todate = DateTime.Today.ToString("yyyy-MM-dd");
            }
            var fbitemlist = new List<FBItem>();
            try
            {
                var dt = new DataTable();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("spSelectAllCasinoKiosk_FBTickets", con))
                    {
                        con.Open();
                        cmd.Connection = con;
                        cmd.Parameters.Add(new SqlParameter("@PlayerID", playerID));
                        cmd.Parameters.Add(new SqlParameter("@FromDate", fromdate));
                        cmd.Parameters.Add(new SqlParameter("@ToDate", todate));

                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
                fbitemlist = dt.AsEnumerable().Select(
                    s => new FBItem()
                    {
                        ID = int.Parse(s["ID"].ToString()),
                        ItemName = s["ItemName"].ToString(),
                        itemValue = s["TicketValue"].ToString(),
                        issuedDate = s["issuedDate"].ToString(),
                        playerID = int.Parse(s["PlayerID"].ToString()),
                        strstatus = s["status"].ToString(),
                        isSplit = s["isSplit"].ToString()

                    }).ToList();
            }
            catch (Exception ex)
            { }
            return fbitemlist;
        }

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

        public bool insertFBTicket(int TicketID, int PlayerID, string ItemName, string IssuedBy, string Points, string IssuedDate, string IssuedTime)
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

        public int checkPointByID(string FBID, string splitValule)
        {
            var uid = 0;
            try
            {
                var dt = new DataTable();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    var strCommand = string.Format("exec spCheckValueZeroCasinoKiosk_FBTickets {0}, {1}", FBID, splitValule);

                    using (SqlCommand cmd = new SqlCommand(strCommand, con))
                    {
                        con.Open();
                        cmd.Connection = con;
                        cmd.CommandType = CommandType.Text;
                        var rs = cmd.ExecuteScalar();
                        uid = int.Parse(rs.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return uid;
        }

        // Management
        public List<FBItem> getFBItemsByDate(string playerID, string fromdate = "", string todate = "")
        {
            if (fromdate == "" || todate == "")
            {
                fromdate = DateTime.Today.AddDays(-30).ToString("yyyy-MM-dd");
                todate = DateTime.Today.ToString("yyyy-MM-dd");
            }
            var fbitemlist = new List<FBItem>();
            try
            {
                var dt = new DataTable();
                var cmdCommand = string.Format("exec spSelectAllCasinoKiosk_FBTickets '{0}','{1}','{2}'", playerID, fromdate, todate);
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(cmdCommand, con))
                    {
                        con.Open();
                        cmd.Connection = con;
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
                fbitemlist = dt.AsEnumerable().Select(
                    s => new FBItem()
                    {
                        ID = int.Parse(s["ID"].ToString()),
                        ticketID = s["TicketID"].ToString(),
                        itemValue = s["TicketValue"].ToString(),
                        issuedDate = s["issuedDate"].ToString(),
                        issuedBy = s["IssuedBy"].ToString(),
                        playerID = int.Parse(s["PlayerID"].ToString()),
                        strstatus = s["status"].ToString(),
                        isSplit = s["isSplit"].ToString()

                    }).ToList();
            }
            catch (Exception ex)
            { }
            return fbitemlist;
        }

        public List<FBSplitItem> getFBSplitItems(int ticketid)
        {
            var fbitemlist = new List<FBSplitItem>();
            try
            {
                var dt = new DataTable();
                var cmdCommand = string.Format("exec spSelectAllCasinoKiosk_FBSplitTickets '{0}'", ticketid);
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(cmdCommand, con))
                    {
                        con.Open();
                        cmd.Connection = con;
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
                fbitemlist = dt.AsEnumerable().Select(
                    s => new FBSplitItem()
                    {
                        ID = int.Parse(s["ID"].ToString()),
                        SplitID = s["SplitID"].ToString(),
                        SplitValue = s["SplitValue"].ToString(),
                        RemainedValue = s["RemainedValue"].ToString(),
                        SplitDate = s["SplitDate"].ToString(),
                        SplitBy = s["SplitBy"].ToString()

                    }).ToList();
            }
            catch (Exception ex)
            { }
            return fbitemlist;
        }


        public bool splitItem(int ticketId, int splitValue, string splitBy)
        {
            try
            {
                var dt = new DataTable();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("spUpdateStatusCasinoKiosk_FBTicketsOnValue", con))
                    {
                        con.Open();
                        cmd.Connection = con;
                        cmd.Parameters.Add(new SqlParameter("@ID", ticketId));
                        cmd.Parameters.Add(new SqlParameter("@SplitValue", splitValue));
                        cmd.Parameters.Add(new SqlParameter("@SplitBy", splitBy));

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
        #endregion
    }
}
