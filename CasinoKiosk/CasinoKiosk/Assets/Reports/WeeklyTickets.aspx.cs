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
    public partial class WeeklyTickets : System.Web.UI.Page
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
                SqlCommand cmd = new SqlCommand("MFBonus_spSelectWeeklyLogsByID", cn);
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
                //dt.TableName = "RedeemLogDatatable";
                ReportParameter[] reportParameters = new ReportParameter[1];
                ReportDataSource rds = new ReportDataSource("DataSet4", dt);
                string name = dt.Rows[0]["ItemName"].ToString();
                //Add 20230214 Hantt start
                string issueDate = dt.Rows[0]["IssueDate"].ToString();
                //Add 20230214 Hantt end

                //string s = rds.DataMember.ToString();
                ReportViewerWeeklyBonus.LocalReport.DataSources.Clear();
                ReportViewerWeeklyBonus.LocalReport.DataSources.Add(rds);
                ReportViewerWeeklyBonus.LocalReport.ReportPath = Server.MapPath("~/Assets/Reports/TicketMFWeeklyBonus.rdlc");

                if (chkMPV.Checked)
                {
                    //Change 20230214 Hantt start
                    //if (name.Contains("MPV"))
                    if (name.Contains("MPV") || name.Contains("SBV"))
                    //Change 20230214 Hantt end
                    {
                        reportParam1 = new ReportParameter("ReportParameter1", name);
                        reportParameters[0] = reportParam1;
                    }
                    else if (name.Contains("SFP"))
                    {
                        //Change 20230214 Hantt start
                        //name = name.Replace("SFP", "Promo MPV");
                        if (issueDate.CompareTo(ConfigurationManager.AppSettings["editDate"]) < 0)
                        {
                            name = name.Replace("SFP", "Promo MPV");
                        }else
                        {
                            name = name.Replace("SFP", "SBV");
                        } 
                        //Change 20230214 Hantt end
                        reportParam1 = new ReportParameter("ReportParameter1", name);
                        reportParameters[0] = reportParam1;
                    }
                    this.ReportViewerWeeklyBonus.LocalReport.SetParameters(reportParameters);
                }
                else
                {
                    //Change 20230214 Hantt start
                    //if (name.Contains("MPV"))
                    //{
                    //    name = name.Replace("Promo MPV", "SFP");
                    if (name.Contains("MPV") || name.Contains("SBV"))
                    {
                        if (issueDate.CompareTo(ConfigurationManager.AppSettings["editDate"]) < 0)
                        {
                            name = name.Replace("Promo MPV", "SFP");
                        }else
                        {
                            name = name.Replace("SBV", "SFP");
                        }
                           
                    //Change 20230214 Hantt end
                        reportParam1 = new ReportParameter("ReportParameter1", name);
                        reportParameters[0] = reportParam1;
                    }
                    else if (name.Contains("SFP"))
                    {
                        reportParam1 = new ReportParameter("ReportParameter1", name);
                        reportParameters[0] = reportParam1;
                    }
                    this.ReportViewerWeeklyBonus.LocalReport.SetParameters(reportParameters);
                }

                ReportViewerWeeklyBonus.DataBind();
                ReportViewerWeeklyBonus.ShowToolBar = false;
                ReportViewerWeeklyBonus.LocalReport.Refresh();

            }

        }

        private void MessageBox(string message, string title = "title")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), title, "alert('" + message + "');", true);
        }
    }
}
