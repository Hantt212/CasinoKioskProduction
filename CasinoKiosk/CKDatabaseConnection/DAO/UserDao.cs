using CKDatabaseConnection.Common;
using CKDatabaseConnection.EF;
using CKDatabaseConnection.Models;
using CKDatabaseConnection.Supports;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CKDatabaseConnection.DAO
{
    public class UserDao
    {
        ITHoTram_CustomReportEntities context = null;
        SqlConnection pmCon = new SqlConnection("Data Source=10.70.1.53;Initial Catalog=ITHoTram_CustomReport;User Id=casinokiosk.user; Password=P@ssword1;");
        CKFunction function = new CKFunction();
        LocalReport report = new LocalReport();

        public UserDao()
        {
            context = new ITHoTram_CustomReportEntities();
        }

        public List<CasinoKioskRoles_SelectAll_Result> ListAllRoles(int RoleID)
        {
            context.Configuration.ProxyCreationEnabled = false;
            var role = context.CasinoKioskRoles_SelectAll(RoleID).ToList();
            return role;
        }

        public long Insert(CasinoKioskUsers_SelectAll_Result ckUser)
        {
            var cu = new CasinoKioskUser();
            var cur = new CasinoKioskUserRole();
            var existUser = new CasinoKioskUser();
            var actionLog = new CasinoKioskUserActionLog();
            var cr = new CasinoKioskRole();
            var name = ckUser.CasinoKiosk_UserName.Trim();
            var isExist = context.CasinoKioskUsers.Any(x => x.CasinoKiosk_UserName == name);

            if (!isExist)
            {
                cu.CasinoKiosk_UserName = name;
                cu.isActive = ckUser.isActive;
                cu.UserEmailAddress = ckUser.UserEmailAddress;
                cu.CreatedTime = DateTime.Now;
                cu.isShow = true;

                context.CasinoKioskUsers.Add(cu);
                context.SaveChanges();

                cur.RoleId = ckUser.RoleId;
                cur.UserId = cu.UserID;
                context.CasinoKioskUserRoles.Add(cur);

                actionLog.UserID = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                actionLog.UserName = HttpContext.Current.Session["UserName"].ToString();
                actionLog.ActionType = "Insert";
                actionLog.Description = "Insert new user";
                actionLog.ValueBefore = "";
                actionLog.ValueAfter = "UserName: " + name + " - " + "RoleName: " + ckUser.RoleName + " - " + "Email: " + ckUser.UserEmailAddress + " - " + "isActive: " + ckUser.isActive.ToString();
                actionLog.ActionDate = DateTime.Now;

                context.CasinoKioskUserActionLogs.Add(actionLog);

                context.SaveChanges();

                return cu.UserID;
            }
            else
            {
                existUser = GetByName(name);
                cur = context.CasinoKioskUserRoles.FirstOrDefault(x => x.UserId == existUser.UserID);
                cr = context.CasinoKioskRoles.FirstOrDefault(x => x.RoleId == cur.RoleId);

                actionLog.ValueBefore = "UserName: " + name + " - " + "RoleName: " + cr.RoleName + " - " + "Email: " + existUser.UserEmailAddress + " - " + "isDisabled: " + existUser.isShow.ToString();

                existUser.isShow = true;
                existUser.isActive = ckUser.isActive;
                existUser.UserEmailAddress = ckUser.UserEmailAddress;
                cur.RoleId = ckUser.RoleId;

                actionLog.UserID = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                actionLog.UserName = HttpContext.Current.Session["UserName"].ToString();
                actionLog.ActionType = "Reactivate";
                actionLog.Description = "Activate a retired user";
                actionLog.ValueAfter = "UserName: " + name + " - " + "RoleName: " + ckUser.RoleName + " - " + "Email: " + ckUser.UserEmailAddress + " - " + "isDisabled: " + "True";
                actionLog.ActionDate = DateTime.Now;

                context.CasinoKioskUserActionLogs.Add(actionLog);

                context.SaveChanges();
                return existUser.UserID;
            }

        }

        public long InsertFOPatron(FOPatron_SelectAll_Result user)
        {
            var name = user.Username.Trim();
            var isExist = context.FOPatronUsers.Any(x => x.Username == name);
            var patronUser = new FOPatronUser();
            var existUser = new FOPatronUser();
            var actionLog = new FOPatronUserActionLog();

            if (!isExist)
            {
                patronUser.Username = name;
                patronUser.isActive = user.isActive;
                patronUser.isShow = true;

                actionLog.UserID = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                actionLog.UserName = HttpContext.Current.Session["UserName"].ToString();
                actionLog.ActionType = "Insert";
                actionLog.Description = "Insert new user";
                actionLog.ValueBefore = "";
                actionLog.ValueAfter = "UserName: " + name + " - " + "isActive: " + user.isActive.ToString();
                actionLog.ActionDate = DateTime.Now;

                context.FOPatronUserActionLogs.Add(actionLog);
                context.FOPatronUsers.Add(patronUser);
                context.SaveChanges();

                return patronUser.ID;
            }
            else
            {
                existUser = GetFOPatronByName(name);

                actionLog.ValueBefore = "UserName: " + name + " - " + "isDisabled: " + existUser.isShow.ToString();

                existUser.isShow = true;
                existUser.isActive = user.isActive;

                actionLog.UserID = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                actionLog.UserName = HttpContext.Current.Session["UserName"].ToString();
                actionLog.ActionType = "Reactivate";
                actionLog.Description = "Activate a retired user";
                actionLog.ValueAfter = "UserName: " + name + " - " + "isDisabled: " + "True";
                actionLog.ActionDate = DateTime.Now;

                context.FOPatronUserActionLogs.Add(actionLog);

                context.SaveChanges();
                return existUser.ID;
            }

        }

        public long InsertCCUser(CurrencyConverter_User user)
        {
            var name = user.UserName.Trim();
            var isExist = context.CurrencyConverter_User.Any(x => x.UserName == name);
            var ccUser = new CurrencyConverter_User();
            var existUser = new CurrencyConverter_User();
            var actionLog = new CurrencyConverterUserActionLog();

            if (!isExist)
            {
                ccUser.UserName = name;
                ccUser.isActive = user.isActive;
                ccUser.CreatedTime = DateTime.Now;
                ccUser.Role = "Finance";
                ccUser.isShow = true;

                actionLog.UserID = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                actionLog.UserName = HttpContext.Current.Session["UserName"].ToString();
                actionLog.ActionType = "Insert";
                actionLog.Description = "Insert new user";
                actionLog.ValueBefore = "";
                actionLog.ValueAfter = "UserName: " + name + " - " + "isActive: " + user.isActive.ToString();
                actionLog.ActionDate = DateTime.Now;

                context.CurrencyConverterUserActionLogs.Add(actionLog);
                context.CurrencyConverter_User.Add(ccUser);
                context.SaveChanges();

                return ccUser.ID;
            }
            else
            {
                existUser = GetCCUserByName(name);

                actionLog.ValueBefore = "UserName: " + name + " - " + "isDisabled: " + existUser.isShow.ToString();

                existUser.isShow = true;
                existUser.isActive = user.isActive;

                actionLog.UserID = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                actionLog.UserName = HttpContext.Current.Session["UserName"].ToString();
                actionLog.ActionType = "Reactivate";
                actionLog.Description = "Activate a retired user";
                actionLog.ValueAfter = "UserName: " + name + " - " + "isDisabled: " + "True";
                actionLog.ActionDate = DateTime.Now;

                context.CurrencyConverterUserActionLogs.Add(actionLog);

                context.SaveChanges();
                return existUser.ID;
            }

        }

        public long InsertPOS8DragonUser(POSPatronInfoUser user)
        {
            var name = user.Username.Trim();
            var password = user.Password.Trim();
            var isExist = context.POSPatronInfoUsers.Any(x => x.Username == name);
            var posUser = new POSPatronInfoUser();
            var existUser = new POSPatronInfoUser();
            var actionLog = new POSPatron8DragonUserActionLog();

            if (!isExist)
            {
                posUser.Username = name;
                posUser.isActived = user.isActived;
                posUser.CreatedTime = DateTime.Now;
                posUser.Password = user.Password;
                posUser.isShow = true;

                actionLog.UserID = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                actionLog.UserName = HttpContext.Current.Session["UserName"].ToString();
                actionLog.ActionType = "Insert";
                actionLog.Description = "Insert new user";
                actionLog.ValueBefore = "";
                actionLog.ValueAfter = "UserName: " + name + " - " + "Password: " + password + " - " + "isActive: " + user.isActived.ToString();
                actionLog.ActionDate = DateTime.Now;

                context.POSPatron8DragonUserActionLog.Add(actionLog);
                context.POSPatronInfoUsers.Add(posUser);
                context.SaveChanges();

                return posUser.ID;
            }
            else
            {
                existUser = GetPOS8DragonUserByName(name);

                actionLog.ValueBefore = "UserName: " + name + " - " + "Password: " + existUser.Password + " - " + "isDisabled: " + existUser.isShow.ToString();

                existUser.isShow = true;
                existUser.Password = password;
                existUser.isActived = user.isActived;

                actionLog.UserID = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                actionLog.UserName = HttpContext.Current.Session["UserName"].ToString();
                actionLog.ActionType = "Reactivate";
                actionLog.Description = "Activate a retired user";
                actionLog.ValueAfter = "UserName: " + name + " - " + "Password: " + password + " - " + "isDisabled: " + "True";
                actionLog.ActionDate = DateTime.Now;

                context.POSPatron8DragonUserActionLog.Add(actionLog);

                context.SaveChanges();
                return existUser.ID;
            }

        }

        public List<FOPatron_SelectAll_Result> ListAllFOPatronUser()
        {
            context.Configuration.ProxyCreationEnabled = false;
            return context.FOPatron_SelectAll().OrderByDescending(x => x.ID).ToList();

        }

        public List<CurrencyConverter_User> ListAllCCUser()
        {
            context.Configuration.ProxyCreationEnabled = false;
            return context.CurrencyConverter_User.Where(u => u.isShow == true).OrderByDescending(X => X.ID).ToList();
        }

        public List<POSPatronInfoUser> ListAllPOS8DragonUser()
        {
            context.Configuration.ProxyCreationEnabled = false;
            return context.POSPatronInfoUsers.Where(u => u.isShow == true).OrderByDescending(X => X.ID).ToList();
        }

        public List<CasinoKioskUsers_SelectAll_Result> ListAllCKUsers(int RoleID)
        {

            context.Configuration.ProxyCreationEnabled = false;
            return context.CasinoKioskUsers_SelectAll(RoleID).OrderByDescending(x => x.UserID).ToList();
        }
        public CasinoTheGrandSignaturePlayerQualified FindTGSPlayerByID(int id)
        {
            return context.CasinoTheGrandSignaturePlayerQualifieds.Find(id);
        }
        public void AddUser(CasinoKioskUser user)
        {
            context.CasinoKioskUsers.Add(user);
            context.SaveChanges();
        }

        public CasinoKioskUser GetByName(string userName)
        {
            return context.CasinoKioskUsers.FirstOrDefault(x => x.CasinoKiosk_UserName == userName);
        }
        public FOPatronUser GetFOPatronByName(string userName)
        {
            return context.FOPatronUsers.FirstOrDefault(x => x.Username == userName);
        }

        public CurrencyConverter_User GetCCUserByName(string userName)
        {
            return context.CurrencyConverter_User.FirstOrDefault(x => x.UserName == userName);
        }

        public POSPatronInfoUser GetPOS8DragonUserByName(string userName)
        {
            return context.POSPatronInfoUsers.FirstOrDefault(x => x.Username == userName);
        }

        public CasinoKioskUser GetUser(string userName, string password)
        {
            var user = context.CasinoKioskUsers.FirstOrDefault(u => u.CasinoKiosk_UserName == userName && u.Password == password);
            return user;
        }

        public CasinoKioskUser GetUser(string userName)
        {
            var user = context.CasinoKioskUsers.FirstOrDefault(u => u.CasinoKiosk_UserName == userName);
            return user;
        }

        public void AddUserRole(CasinoKioskUserRole userRole)
        {
            var roleEntry = context.CasinoKioskUserRoles.FirstOrDefault(r => r.UserId == userRole.UserId);
            if (roleEntry != null)
            {
                context.CasinoKioskUserRoles.Remove(roleEntry);
                context.SaveChanges();
            }
            context.CasinoKioskUserRoles.Add(userRole);
            context.SaveChanges();
        }

        public CasinoKioskUser ViewDetail(int id)
        {
            return context.CasinoKioskUsers.Find(id);
        }

        public bool Update(CasinoKioskUsers_SelectAll_Result entity)
        {

            var cu = context.CasinoKioskUsers.Find(entity.UserID);
            var actionLog = new CasinoKioskUserActionLog();
            var cur = context.CasinoKioskUserRoles.Find(entity.UserRoleId);
            var cr = context.CasinoKioskRoles.Find(cur.RoleId);

            try
            {
                actionLog.ValueBefore = "UserName: " + cu.CasinoKiosk_UserName + " - " + "RoleName: " + cr.RoleName + " - " + "Email: " + cu.UserEmailAddress + " - " + "isActive: " + cu.isActive.ToString();

                cu.CasinoKiosk_UserName = entity.CasinoKiosk_UserName;
                cu.isActive = entity.isActive;
                cu.UserEmailAddress = entity.UserEmailAddress;

                cur.RoleId = entity.RoleId;
                cur.UserId = entity.UserID;

                actionLog.UserID = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                actionLog.UserName = HttpContext.Current.Session["UserName"].ToString();
                actionLog.ActionType = "Update";
                actionLog.Description = "Update an existing user";
                actionLog.ValueAfter = "UserName: " + entity.CasinoKiosk_UserName + " - " + "RoleName: " + entity.RoleName + " - " + "Email: " + entity.UserEmailAddress + " - " + "isActive: " + entity.isActive.ToString();
                actionLog.ActionDate = DateTime.Now;

                context.CasinoKioskUserActionLogs.Add(actionLog);

                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateFOPatron(FOPatron_SelectAll_Result entity)
        {
            var actionLog = new FOPatronUserActionLog();
            var user = context.FOPatronUsers.Find(entity.ID);

            try
            {
                actionLog.ValueBefore = "UserName: " + user.Username + " - " + "isActive: " + user.isActive.ToString();

                user.Username = entity.Username;
                user.isActive = entity.isActive;

                actionLog.UserID = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                actionLog.UserName = HttpContext.Current.Session["UserName"].ToString();
                actionLog.ActionType = "Update";
                actionLog.Description = "Update an existing user";
                actionLog.ValueAfter = "UserName: " + entity.Username + " - " + "isActive: " + entity.isActive.ToString();
                actionLog.ActionDate = DateTime.Now;

                context.FOPatronUserActionLogs.Add(actionLog);

                context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateCCUser(CurrencyConverter_User entity)
        {
            var actionLog = new CurrencyConverterUserActionLog();
            var user = context.CurrencyConverter_User.Find(entity.ID);

            try
            {
                actionLog.ValueBefore = "UserName: " + user.UserName + " - " + "isActive: " + user.isActive.ToString();

                user.UserName = entity.UserName;
                user.isActive = entity.isActive;

                actionLog.UserID = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                actionLog.UserName = HttpContext.Current.Session["UserName"].ToString();
                actionLog.ActionType = "Update";
                actionLog.Description = "Update an existing user";
                actionLog.ValueAfter = "UserName: " + entity.UserName + " - " + "isActive: " + entity.isActive.ToString();
                actionLog.ActionDate = DateTime.Now;

                context.CurrencyConverterUserActionLogs.Add(actionLog);

                context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool RedemmTGS(CasinoTheGrandSignaturePlayerQualified player)
        {
            var log = new CasinoTheGrandSignatureRedeemLog();
            var redeemedPlayer = context.CasinoTheGrandSignaturePlayerQualifieds.SingleOrDefault(p => p.ID == player.ID);
            var localReport = new LocalReport();

            try
            {
                int? remainingQty = 0;
                var redeemedQty = player.Redeemded;

                if (redeemedQty > 0)
                {
                    if (redeemedPlayer.Remaining > 0 && redeemedPlayer.Remaining >= redeemedQty)
                    {
                        remainingQty = redeemedPlayer.Remaining - redeemedQty;
                    }
                }

                redeemedPlayer.Redeemded += redeemedQty;
                redeemedPlayer.Remaining = remainingQty;

                log.PlayerID = redeemedPlayer.PID;
                log.PlayerName = redeemedPlayer.PlayerName;
                log.IssueDate = DateTime.Now.ToString("dd/MM/yyyy");
                log.IssueTime = DateTime.Now.ToString("HH:mm:ss");
                log.PromotionName = "Play & Stay Bundle";
                log.ItemID = redeemedPlayer.ID;
                log.ItemName = redeemedPlayer.ItemName;
                log.Quantity = (int)redeemedPlayer.Qty;
                log.RedeemedQty = (int)redeemedQty;
                log.RemainingQty = (int)remainingQty;
                log.redeemedPerson = HttpContext.Current.Session["UserName"].ToString();
                log.GamingDate = function.getMFDBCurrentDay().ToString();
                log.Status = "R";
                log.Type = "T";
                log.voidedPerson = "";

                context.CasinoTheGrandSignatureRedeemLogs.Add(log);

                context.SaveChanges();

                string filename = log.PlayerID.ToString() + "_" + log.ItemName + "_" + log.ItemID.ToString() + "_" + DateTime.Now.ToString("ddMMyyyy_HHmmss");
                this.printRedeemedTGSTicket(report, log.ID, filename);

                return true;
            }
            catch
            {
                return false;
            }
        }
        public void printRedeemedTGSTicket(LocalReport report, int ID, string filename)
        {
            report = PrintExtention.ExportLocalReport(report, ID);
            PrintExtention.Print(report);
            PrintExtention.SavePDF(report, filename);
        }
        public void printTicketPromotion(LocalReport report, int ID, string filename)
        {
            report = PrintExtention.ExportLocalReportTicketPromotion(report, ID);
            PrintExtention.Print(report);
            PrintExtention.SavePDF(report, filename);
        }

        

        public DataTable GetData(int id)
        {
            DataTable dt = new DataTable();
            if (pmCon.State == ConnectionState.Open) pmCon.Close();
            using (SqlCommand cmd = new SqlCommand("CasinoTGS_spSelectLog", pmCon))
            {
                pmCon.Open();
                cmd.Connection = pmCon;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", id);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            return dt;
        }
        public DataTable GetDataTicketPromotion(int id)
        {
            DataTable dt = new DataTable();
            if (pmCon.State == ConnectionState.Open) pmCon.Close();
            using (SqlCommand cmd = new SqlCommand("TicketPromotion_spSelectByID", pmCon))
            {
                pmCon.Open();
                cmd.Connection = pmCon;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", id);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            return dt;
        }

       

        public bool UpdatePOS8DragonUser(POSPatronInfoUser entity)
        {
            var actionLog = new POSPatron8DragonUserActionLog();
            var user = context.POSPatronInfoUsers.Find(entity.ID);

            try
            {
                actionLog.ValueBefore = "UserName: " + user.Username + " - " + "Password: " + user.Password + " - " + "isActive: " + user.isActived.ToString();

                user.Username = entity.Username;
                user.Password = entity.Password;
                user.isActived = entity.isActived;

                actionLog.UserID = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                actionLog.UserName = HttpContext.Current.Session["UserName"].ToString();
                actionLog.ActionType = "Update";
                actionLog.Description = "Update an existing user";
                actionLog.ValueAfter = "UserName: " + entity.Username + " - " + "Password: " + entity.Password + " - " + "isActive: " + entity.isActived.ToString();
                actionLog.ActionDate = DateTime.Now;

                context.POSPatron8DragonUserActionLog.Add(actionLog);

                context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public long Delete(int id)
        {
            var ckUser = context.CasinoKioskUsers.Find(id);
            var actionLog = new CasinoKioskUserActionLog();

            ckUser.isShow = false;
            ckUser.isActive = false;

            actionLog.UserID = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
            actionLog.UserName = HttpContext.Current.Session["UserName"].ToString();
            actionLog.ActionType = "Delete";
            actionLog.Description = "Delete an existing user";
            actionLog.ValueBefore = "UserName: " + ckUser.CasinoKiosk_UserName + " - " + "isDisabled: " + "True";
            actionLog.ValueAfter = "UserName: " + ckUser.CasinoKiosk_UserName + " - " + "isDisabled: " + "False";
            actionLog.ActionDate = DateTime.Now;

            context.CasinoKioskUserActionLogs.Add(actionLog);

            context.SaveChanges();

            return ckUser.UserID;
        }

        public long DeleteFOPatron(int id)
        {
            var user = context.FOPatronUsers.Find(id);
            var actionLog = new FOPatronUserActionLog();

            user.isShow = false;
            user.isActive = false;

            actionLog.UserID = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
            actionLog.UserName = HttpContext.Current.Session["UserName"].ToString();
            actionLog.ActionType = "Delete";
            actionLog.Description = "Delete an existing user";
            actionLog.ValueBefore = "UserName: " + user.Username + " - " + "isDisabled: " + "True";
            actionLog.ValueAfter = "UserName: " + user.Username + " - " + "isDisabled: " + "False";
            actionLog.ActionDate = DateTime.Now;

            context.FOPatronUserActionLogs.Add(actionLog);

            context.SaveChanges();

            return user.ID;
        }

        public long DeleteCCUser(int id)
        {
            var user = context.CurrencyConverter_User.Find(id);
            var actionLog = new CurrencyConverterUserActionLog();

            user.isShow = false;
            user.isActive = false;

            actionLog.UserID = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
            actionLog.UserName = HttpContext.Current.Session["UserName"].ToString();
            actionLog.ActionType = "Delete";
            actionLog.Description = "Delete an existing user";
            actionLog.ValueBefore = "UserName: " + user.UserName + " - " + "isDisabled: " + "True";
            actionLog.ValueAfter = "UserName: " + user.UserName + " - " + "isDisabled: " + "False";
            actionLog.ActionDate = DateTime.Now;

            context.CurrencyConverterUserActionLogs.Add(actionLog);

            context.SaveChanges();

            return user.ID;
        }

        public long DeletePOS8DragonUser(int id)
        {
            var user = context.POSPatronInfoUsers.Find(id);
            var actionLog = new POSPatron8DragonUserActionLog();

            user.isShow = false;
            user.isActived = false;

            actionLog.UserID = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
            actionLog.UserName = HttpContext.Current.Session["UserName"].ToString();
            actionLog.ActionType = "Delete";
            actionLog.Description = "Delete an existing user";
            actionLog.ValueBefore = "UserName: " + user.Username + " - " + "isDisabled: " + "True";
            actionLog.ValueAfter = "UserName: " + user.Username + " - " + "isDisabled: " + "False";
            actionLog.ActionDate = DateTime.Now;

            context.POSPatron8DragonUserActionLog.Add(actionLog);

            context.SaveChanges();

            return user.ID;
        }

        public List<CasinoTrackingActivity> getListActivityByUserName()
        {
            context.Configuration.ProxyCreationEnabled = false;
            string username = HttpContext.Current.Session["UserName"].ToString();
            var currentDateTime = DateTime.Now;
            var currentDate = currentDateTime.Date; 

            var listActivity =  context.CasinoTrackingActivities.Where(a => a.UserName == username && a.isShow == true).OrderByDescending(a => a.CreatedDate).ToList();

            foreach (var cta in listActivity) {

                var ctaCreatedDate = DateTime.ParseExact(cta.CreatedDate, @"dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                var ctaDate = ctaCreatedDate.Date;

                if (ctaDate < currentDate) {
                    cta.isShow = false;
                    context.SaveChanges();
                }
            }
            return context.CasinoTrackingActivities.Where(a => a.UserName == username && a.isShow == true).OrderByDescending(a => a.CreatedDate).ToList();
        }
        public List<HTRTicketPromotion> getListPromotion()
        {
            context.Configuration.ProxyCreationEnabled = false;
            //string username = HttpContext.Current.Session["UserName"].ToString();           
            return context.HTRTicketPromotions.OrderByDescending(x => x.ID).ToList();
        }
        public long InsertTicketPromotion(HTRTicketPromotion ticket)
        {
            var htrTicket = new HTRTicketPromotion();
            string username = HttpContext.Current.Session["UserName"].ToString();           

            var PID = ticket.GuestPID;
            var isExist = context.HTRTicketPromotions.Any(x => x.GuestPID == PID);

            if (!isExist)
            {
                htrTicket.GuestName = ticket.GuestName;
                htrTicket.GuestPID = ticket.GuestPID;
                htrTicket.isPrinted = true;
                htrTicket.IssuedDate = DateTime.Now.ToString("dd/MM/yyyy");
                htrTicket.IssuedTime = DateTime.Now.ToString("HH:mm:ss");
                htrTicket.PrintedQty = 1;
                htrTicket.PrintedBy = username;
                htrTicket.isReprinted = false;
                htrTicket.isVoided = false;
                htrTicket.NumberOfReprint = 0;


                context.HTRTicketPromotions.Add(htrTicket);
                context.SaveChanges();

                //string filename = htrTicket.GuestPID.ToString() + "_" + htrTicket.ID.ToString() + "_" + DateTime.Now.ToString("ddMMyyyy_HHmmss");
                //this.printTicketPromotion(report, htrTicket.ID, filename);

                return 1;
            }
            else {
                return 0;
            }          
        }

        public void InsertTicketPromotionTest(int PID)
        {
            var htrTicket = new HTRTicketPromotion();
            string username = HttpContext.Current.Session["UserName"].ToString();
          
            var isExist = context.HTRTicketPromotions.Any(x => x.GuestPID == PID);
            var name = GetPlayerNameByPID(PID);
            var ticket = context.HTRTicketPromotions.FirstOrDefault(x => x.GuestPID == PID);

            if (!isExist)
            {
                htrTicket.GuestName = name;
                htrTicket.GuestPID = PID ;
                htrTicket.isPrinted = true;
                htrTicket.IssuedDate = DateTime.Now.ToString("dd/MM/yyyy");
                htrTicket.IssuedTime = DateTime.Now.ToString("HH:mm:ss");
                htrTicket.PrintedQty = 1;
                htrTicket.PrintedBy = username;
                htrTicket.isReprinted = false;
                htrTicket.isVoided = false;
                htrTicket.NumberOfReprint = 0;


                context.HTRTicketPromotions.Add(htrTicket);
                context.SaveChanges();

                //string filename = htrTicket.GuestPID.ToString() + "_" + htrTicket.ID.ToString() + "_" + DateTime.Now.ToString("ddMMyyyy_HHmmss");
                //this.printTicketPromotion(report, htrTicket.ID, filename);

                
            }
            else
            {
               
            }
        }
        public long TicketVoid(int id)
        {
            var ticket = new HTRTicketPromotion();

            ticket = context.HTRTicketPromotions.FirstOrDefault(x => x.ID == id);

            string username = HttpContext.Current.Session["UserName"].ToString();

            if (ticket.isPrinted == true)
            {
                if (ticket.isVoided == false)
                {
                    ticket.VoidedBy = username;
                    ticket.isVoided = true;
                    ticket.PrintedQty = 0;
                    ticket.VoidedDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                }
            }
            context.SaveChanges();

            return ticket.ID;
        }

        public long TicketReprint(int id)
        {
            var ticket = new HTRTicketPromotion();

            ticket = context.HTRTicketPromotions.FirstOrDefault(x => x.ID == id);

            string username = HttpContext.Current.Session["UserName"].ToString();

            if(ticket.isPrinted == true)
            {                
                    ticket.isReprintedBy = username;
                    ticket.isReprinted = true;
                    ticket.PrintedQty = 1;
                    ticket.IssuedDate = DateTime.Now.ToString("dd/MM/yyyy");
                    ticket.IssuedTime = DateTime.Now.ToString("HH:mm:ss");
                    ticket.NumberOfReprint += 1;
            }
            context.SaveChanges();
           // string filename = ticket.GuestPID.ToString() + "_" + ticket.ID.ToString() + "_" + DateTime.Now.ToString("ddMMyyyy_HHmmss");
            //this.printTicketPromotion(report, ticket.ID, filename);

            return ticket.ID;
        }

        public long InsertActivity(CasinoTrackingActivity ac)
        {
            var activity = new CasinoTrackingActivity();
            string username = HttpContext.Current.Session["UserName"].ToString();
          
            activity.UserName = username;
            activity.GuestName = ac.GuestName;
            activity.GuestPID = ac.GuestPID;
            activity.isShow = true;
            activity.MeetRecord = ac.MeetRecord;
            activity.ChatRecord = ac.ChatRecord;
            activity.CallRecord = ac.CallRecord;            
            activity.CreatedDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            
            context.CasinoTrackingActivities.Add(activity);            
            context.SaveChanges();

            return activity.ID;
        }      
        public CasinoTrackingActivity GetActivityByID(int ID)
        {
            context.Configuration.ProxyCreationEnabled = false;
            return context.CasinoTrackingActivities.SingleOrDefault(a=>a.ID == ID);

        }
        public bool UpdateActivity(CasinoTrackingActivity ac)
        {
            var activity = new CasinoTrackingActivity();
            var existAc = context.CasinoTrackingActivities.Find(ac.ID);
            var actionLog = new CasinoTrackingActivityEditLog();

            try
            {
                actionLog.ValueBefore = "PlayerID: " + existAc.GuestPID.ToString() + " - " + "PlayerName: " + existAc.GuestName + " - " + "Call: " + existAc.CallRecord + " - " + "Chat: " 
                                         + existAc.ChatRecord + " - " + "Meet: " + existAc.MeetRecord;
                existAc.GuestPID = ac.GuestPID;
                existAc.GuestName = ac.GuestName;                
                existAc.MeetRecord = ac.MeetRecord;
                existAc.CallRecord = ac.CallRecord;
                existAc.ChatRecord = ac.ChatRecord;              
                
                actionLog.ValueAfter = "PlayerID: " + existAc.GuestPID.ToString() + " - " + "PlayerName: " + existAc.GuestName + " - " + "Call: " + existAc.CallRecord + " - " + "Chat: "
                                         + existAc.ChatRecord + " - " + "Meet: " + existAc.MeetRecord;
                actionLog.ActionDate = DateTime.Now;
                actionLog.UserName = existAc.UserName;

                context.CasinoTrackingActivityEditLogs.Add(actionLog);

                context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
        public string GetPlayerNameByPID(int PID) {            
            return function.getPlayerName(PID);
        }    
    
    }
}

