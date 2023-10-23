using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CasinoKiosk.Assets.Reports
{
    public partial class RedeemLog1 : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            //txtFromDate.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy");
            //txtToDate.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy");

            //ReportViewer1.LocalReport.Refresh();


        }

        protected void btnShow_Click(object sender, EventArgs e)
        {
            showReport();
        }

        private DataTable GetData(DateTime fromDate, DateTime toDate, string logName)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = null;

            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["CKdbContext"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(connStr))
            {
                if (logName == "Daily Bonus")
                {
                    cmd = new SqlCommand("CasinoKiosk_spGetDailyLogByDate", cn);
                }
                else if (logName == "Weekly Bonus")
                {
                    cmd = new SqlCommand("CasinoKiosk_spGetWeeklyLogByDate", cn);
                }
                else if (logName == "Friday Bonus")
                {
                    cmd = new SqlCommand("CasinoKiosk_spGetFridayLogByDate", cn);
                }
                else if (logName == "Points Redemption")
                {
                    cmd = new SqlCommand("CasinoKiosk_spGetPointsRedemptionLogByDate", cn);
                }
                //Add 20230526 Hantt start
                else if (logName == "FO Patron Log")
                {
                    cmd = new SqlCommand("CasinoKiosk_spGetFOPatronLogByDate", cn);
                }
                //Add 20230526 Hantt end
                else
                {
                    cmd = new SqlCommand("CasinoKiosk_spGetLogByDate", cn);
                }

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fromDate", fromDate);
                cmd.Parameters.AddWithValue("@toDate", toDate);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            return dt;
        }

        private void showReport()
        {
            string s = "";

            if (txtFromDate.Text == "" || txtToDate.Text == "")
            {
                MessageBox("Date could not be empty", "Error");
            }
            else
            {


                DateTime d1 = DateTime.Parse(txtFromDate.Text.ToString());
                string date1 = d1.ToString("yyyy-MM-dd");
                //string date3 = d1.ToString("dd-MM-yyy");

                DateTime d2 = DateTime.Parse(txtToDate.Text.ToString());
                string date2 = d2.ToString("yyyy-MM-dd");
                //string date4 = d1.ToString("dd-MM-yyy");

                if (ddlReports.Items[ddlReports.SelectedIndex].Text != null)
                {
                    s = ddlReports.Items[ddlReports.SelectedIndex].Text;
                }

                DataTable dt = GetData(d1, d2, s);
                if (dt != null)
                {
                    ReportDataSource rds = new ReportDataSource("DataSet1", dt);

                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.LocalReport.DataSources.Add(rds);

                    if (s == "Daily Bonus")
                    {
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Assets/Reports/DailyBonusLog.rdlc");
                    }
                    else if (s == "Weekly Bonus")
                    {
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Assets/Reports/WeeklyBonusLog.rdlc");
                    }
                    else if (s == "Friday Bonus")
                    {
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Assets/Reports/FridayBonusLog.rdlc");
                    }
                    else if (s == "Points Redemption")
                    {
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Assets/Reports/PointsRedemptionLog.rdlc");
                    }
                    //Add 20230526 Hantt start
                    else if (s == "FO Patron Log")
                    {
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Assets/Reports/FOPatronLog.rdlc");
                    }
                    //Add 20230526 Hantt end
                    else
                    {
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Assets/Reports/RedeemLogReport.rdlc");
                    }

                    ReportViewer1.DataBind();
                    ReportViewer1.LocalReport.Refresh();
                }
            }
        }

        private void MessageBox(string message, string title = "title")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), title, "alert('" + message + "');", true);
        }



    }
}