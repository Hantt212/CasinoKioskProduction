using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CasinoKiosk.Assets.Reports
{
    public partial class DailyTickets : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

      
        protected void btnShow_Click(object sender, EventArgs e)
        {
            showReport();
        }

        private DataTable GetData(int id)
        {
            DataTable dt = new DataTable();
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["CKdbContext"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("MFBonus_spSelectDailyLogsByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", id);
                //cmd.Parameters.AddWithValue("@toDate", toDate);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            return dt;
        }
        private void showReport()
        {

            int id = 0;
            
           
            ReportParameter reportParam1;

            if (!String.IsNullOrEmpty(Request.QueryString["id"]))
            {
                
                id = Convert.ToInt32(Request.QueryString["id"]);
            }
           

                DataTable dt = GetData(id);

            if (dt != null)
            {

                dt.TableName = "RedeemLogDatatable";

                int points = Convert.ToInt32(dt.Rows[0]["ItemPoints"].ToString());
                string name = dt.Rows[0]["ItemName"].ToString();
                //Add 20230214 Hantt start
                string issueDate = dt.Rows[0]["IssueDate"].ToString();
                //Add 20230214 Hantt end

                ReportParameter[] reportParameters = new ReportParameter[1];

                ReportDataSource rds = new ReportDataSource("DataSet3", dt);

                //string s = rds.DataMember.ToString();
                ReportViewerDailyBonus.LocalReport.DataSources.Clear();
                ReportViewerDailyBonus.LocalReport.DataSources.Add(rds);
                ReportViewerDailyBonus.LocalReport.ReportPath = Server.MapPath("~/Assets/Reports/TicketMFDailyBonus.rdlc");

                if (chkGaming.Checked && !chkFaB.Checked)
                {

                    //Change 20230214 Hantt start
                    //if (name.Contains("MPV")) {
                    //    if (points == 1000)
                    //    {
                    //        reportParam1 = new ReportParameter("ReportParameter1", "$50 Promo MPV - 1000 pts");
                    //        reportParameters[0] = reportParam1;
                    //    }
                    //    if (points == 2000)
                    //    {
                    //        reportParam1 = new ReportParameter("ReportParameter1", "$50 Promo MPV - 2000 pts");
                    //        reportParameters[0] = reportParam1;
                    //    }
                    //    if (points == 4000)
                    //    {
                    //        reportParam1 = new ReportParameter("ReportParameter1", "$50 Promo MPV - 4000 pts");
                    //        reportParameters[0] = reportParam1;
                    //    }
                    //    if (points == 6000)
                    //    {
                    //        reportParam1 = new ReportParameter("ReportParameter1", "$50 Promo MPV - 6000 pts");
                    //        reportParameters[0] = reportParam1;
                    //    }
                    //    if (points == 8000)
                    //    {
                    //        reportParam1 = new ReportParameter("ReportParameter1", "$50 Promo MPV - 8000 pts");
                    //        reportParameters[0] = reportParam1;
                    //    }
                    //}

                    if (name.Contains("MPV") || name.Contains("SBV"))
                    {
                        reportParam1 = new ReportParameter("ReportParameter1", name);
                        reportParameters[0] = reportParam1;
                    }
                    //Change 20230214 Hantt end

                    if (name.Contains("SFP")) {
                        if (points == 1000)
                        {
                            reportParam1 = new ReportParameter("ReportParameter1", "$30 SFP - 1000 pts");
                            reportParameters[0] = reportParam1;
                        }
                        if (points == 2000)
                        {
                            reportParam1 = new ReportParameter("ReportParameter1", "$50 SFP - 2000 pts");
                            reportParameters[0] = reportParam1;
                        }
                        if (points == 4000)
                        {
                            reportParam1 = new ReportParameter("ReportParameter1", "$50 SFP - 4000 pts");
                            reportParameters[0] = reportParam1;
                        }
                        if (points == 6000)
                        {
                            reportParam1 = new ReportParameter("ReportParameter1", "$50 SFP - 6000 pts");
                            reportParameters[0] = reportParam1;
                        }
                        if (points == 8000)
                        {
                            reportParam1 = new ReportParameter("ReportParameter1", "$50 SFP - 8000 pts");
                            reportParameters[0] = reportParam1;
                        }
                        //Add 20230619 Hantt start
                        if (points == 20000)
                        {
                            reportParam1 = new ReportParameter("ReportParameter1", "$100 SFP - 20000 pts");
                            reportParameters[0] = reportParam1;
                        }
                        //Add 20230619 Hantt end
                    }


                    //ReportParameter reportParam2 = new ReportParameter("ReportParameter2", "F&B Redeemed");

                    //reportParameters[1] = reportParam2;
                    this.ReportViewerDailyBonus.LocalReport.SetParameters(reportParameters);
                }

                if (!chkGaming.Checked && chkFaB.Checked)
                {
                    //Change 20230214 Hantt start
                    //if (name.Contains("MPV")) {
                    if (name.Contains("MPV") || name.Contains("SBV")) { 
                    //Change 20230214 Hantt end
                        if (points == 1000)
                        {
                            reportParam1 = new ReportParameter("ReportParameter1", "$10 F&B - 1000 pts");
                            reportParameters[0] = reportParam1;
                        }
                        if (points == 2000)
                        {
                            reportParam1 = new ReportParameter("ReportParameter1", "$10 F&B - 2000 pts");
                            reportParameters[0] = reportParam1;
                        }
                        if (points == 4000)
                        {
                            //Change 20230214 Hantt start
                            //reportParam1 = new ReportParameter("ReportParameter1", "$50 Promo MPV - 4000 pts");
                            if (issueDate.CompareTo(ConfigurationManager.AppSettings["editDate"]) < 0)
                            {
                                 reportParam1 = new ReportParameter("ReportParameter1", "$50 Promo MPV - 4000 pts");
                            }else
                            {
                                reportParam1 = new ReportParameter("ReportParameter1", "$50 SBV - 4000 pts");
                            }
                            //Change 20230214 Hantt end

                            reportParameters[0] = reportParam1;
                        }

                        if (points == 6000)
                        {
                            //Change 20230214 Hantt start
                            //reportParam1 = new ReportParameter("ReportParameter1", "$50 Promo MPV - 6000 pts");
                            if (issueDate.CompareTo(ConfigurationManager.AppSettings["editDate"]) < 0)
                            {
                                reportParam1 = new ReportParameter("ReportParameter1", "$50 Promo MPV - 6000 pts");
                            }
                            else
                            {
                                reportParam1 = new ReportParameter("ReportParameter1", "$50 SBV - 6000 pts");
                            }
                            //Change 20230214 Hantt end
                            reportParameters[0] = reportParam1;
                        }
                        if (points == 8000)
                        {
                            reportParam1 = new ReportParameter("ReportParameter1", "$100 F&B - 8000 pts");
                            reportParameters[0] = reportParam1;
                        }
                        //Add 20230619 Hantt start
                        if(points == 20000)
                        {
                            reportParam1 = new ReportParameter("ReportParameter1", "$100 F&B - 20000 pts");
                            reportParameters[0] = reportParam1;
                        }
                        //Add 20230619 Hantt end

                    }

                    if (name.Contains("SFP"))
                    {
                        if (points == 1000)
                        {
                            reportParam1 = new ReportParameter("ReportParameter1", "$10 F&B - 1000 pts");
                            reportParameters[0] = reportParam1;
                        }
                        if (points == 2000)
                        {
                            reportParam1 = new ReportParameter("ReportParameter1", "$10 F&B - 2000 pts");
                            reportParameters[0] = reportParam1;
                        }
                        if (points == 4000)
                        {
                            reportParam1 = new ReportParameter("ReportParameter1", "$50 SFP - 4000 pts");
                            reportParameters[0] = reportParam1;
                        }
                        if (points == 6000)
                        {
                            reportParam1 = new ReportParameter("ReportParameter1", "$50 SFP - 6000 pts");
                            reportParameters[0] = reportParam1;
                        }
                        if (points == 8000)
                        {
                            reportParam1 = new ReportParameter("ReportParameter1", "$100 F&B - 8000 pts");
                            reportParameters[0] = reportParam1;
                        }
                        //Add 20230619 Hantt start
                        if (points == 20000)
                        {
                            reportParam1 = new ReportParameter("ReportParameter1", "$100 F&B - 20000 pts");
                            reportParameters[0] = reportParam1;
                        }
                        //Add 20230619 Hantt end
                    }

                    //ReportParameter reportParam2 = new ReportParameter("ReportParameter2", "Gaming Redeemed");

                    //reportParameters[1] = reportParam2;
                    this.ReportViewerDailyBonus.LocalReport.SetParameters(reportParameters);
                }

                if ((!chkFaB.Checked && !chkGaming.Checked) || (chkFaB.Checked && chkGaming.Checked))
                {
                   
                    reportParam1 = new ReportParameter("ReportParameter1", name);
                   // ReportParameter reportParam2 = new ReportParameter("ReportParameter2", "  ");
                    reportParameters[0] = reportParam1;
                    //reportParameters[1] = reportParam2;
                    this.ReportViewerDailyBonus.LocalReport.SetParameters(reportParameters);
                }

                ReportViewerDailyBonus.DataBind();
                ReportViewerDailyBonus.ShowToolBar = false;
                ReportViewerDailyBonus.LocalReport.Refresh();
 
                }

        }

        private void MessageBox(string message, string title = "title")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), title, "alert('" + message + "');", true);
        }
    }
}
