using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CasinoKiosk.Assets.Reports
{
    public partial class FridayTickets : System.Web.UI.Page
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
                SqlCommand cmd = new SqlCommand("MFBonus_spSelectFridayLogsByID", cn);
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
                string name = dt.Rows[0]["ItemName"].ToString();
                ReportParameter[] reportParameters = new ReportParameter[1];
                ReportDataSource rds = new ReportDataSource("DataSet5", dt);

                //string s = rds.DataMember.ToString();
                ReportViewerFridayBonus.LocalReport.DataSources.Clear();
                ReportViewerFridayBonus.LocalReport.DataSources.Add(rds);
                ReportViewerFridayBonus.LocalReport.ReportPath = Server.MapPath("~/Assets/Reports/TicketMFFridayBonus.rdlc");

                if (chkMPV.Checked)
                {
                    if (name.Contains("MPV"))
                    {
                        reportParam1 = new ReportParameter("ReportParameter1", name);
                        reportParameters[0] = reportParam1;
                    }
                    else if (name.Contains("SFP"))
                    {
                        name = name.Replace("SFP", "Promo MPV");
                        reportParam1 = new ReportParameter("ReportParameter1", name);
                        reportParameters[0] = reportParam1;
                    }
                    this.ReportViewerFridayBonus.LocalReport.SetParameters(reportParameters);
                }
                else
                {
                    if (name.Contains("MPV"))
                    {
                        name = name.Replace("Promo MPV", "SFP");
                        reportParam1 = new ReportParameter("ReportParameter1", name);
                        reportParameters[0] = reportParam1;
                    }
                    else if (name.Contains("SFP"))
                    {
                        reportParam1 = new ReportParameter("ReportParameter1", name);
                        reportParameters[0] = reportParam1;
                    }
                    this.ReportViewerFridayBonus.LocalReport.SetParameters(reportParameters);
                }

                ReportViewerFridayBonus.DataBind();
                ReportViewerFridayBonus.ShowToolBar = false;
                ReportViewerFridayBonus.LocalReport.Refresh();

            }

        }

        private void MessageBox(string message, string title = "title")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), title, "alert('" + message + "');", true);
        }
    }
}
