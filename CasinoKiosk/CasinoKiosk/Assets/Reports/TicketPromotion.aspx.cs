  using CKDatabaseConnection.DAO;
using CKDatabaseConnection.EF;
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
    public partial class TicketPromotion : System.Web.UI.Page
    {
        UserDao dao = new UserDao();
        ITHoTram_CustomReportEntities context = new ITHoTram_CustomReportEntities();
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
                SqlCommand cmd = new SqlCommand("TicketPromotion_spSelectByPID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PID", id);
                //cmd.Parameters.AddWithValue("@toDate", toDate);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            return dt;
        }
        private void showReport()
        {

            int id = 0;

            DataTable dt = new DataTable();

            if (txtPID1.Text.Trim() != "")
            {
                id = Int32.Parse(txtPID1.Text.Trim());
                var exist = context.HTRTicketPromotions.Any(x => x.GuestPID == id);
                if (!exist)
                {
                    dao.InsertTicketPromotionTest(id);
                    dt = GetData(id);
                }
                else
                {
                    MessageBox("This Player has already had ticket printed.", "Ticket existed.");
                    return;
                }


            }
            if (dt != null)
            {               

                //dt.TableName = "RedeemLogDatatable";
                //ReportParameter[] reportParameters = new ReportParameter[1];
                ReportDataSource rds = new ReportDataSource("TicketPromotionByPID", dt);


                //string s = rds.DataMember.ToString();


                ReportViewerTicketPromotion.LocalReport.DataSources.Clear();
                ReportViewerTicketPromotion.LocalReport.DataSources.Add(rds);
                ReportViewerTicketPromotion.LocalReport.ReportPath = Server.MapPath("~/Assets/Reports/HTRTicketPromotion.rdlc");


                //ReportViewerTicketPromotion.LocalReport.SetParameters(reportParameters);
                ReportViewerTicketPromotion.DataBind();
                ReportViewerTicketPromotion.ShowToolBar = false;
                ReportViewerTicketPromotion.LocalReport.Refresh();

            }




        }

        private void MessageBox(string message, string title = "title")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), title, "alert('" + message + "');", true);
        }
    }
}
