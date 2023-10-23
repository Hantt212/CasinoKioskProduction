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
    public partial class Tickets : System.Web.UI.Page
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
                SqlCommand cmd = new SqlCommand("CasinoKiosk_spGetLogByID", cn);
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

            if (txtIDRedemption.Text == "")
            {
                MessageBox("ID could not be empty", "Error");
            }



            else {
                id = Convert.ToInt32(txtIDRedemption.Text.ToString().Trim());

                DataTable dt = GetData(id);

                if (dt != null)
                {
                    //dt.TableName = "RedeemLogDatatable";
                    ReportDataSource rds = new ReportDataSource("DataSet2", dt);

                    //string s = rds.DataMember.ToString();
                    ReportViewerRedempTion.LocalReport.DataSources.Clear();
                    ReportViewerRedempTion.LocalReport.DataSources.Add(rds);
                    ReportViewerRedempTion.LocalReport.ReportPath = Server.MapPath("~/Assets/Reports/TicketSlotTournament.rdlc");

                    ReportViewerRedempTion.DataBind();
                    ReportViewerRedempTion.ShowToolBar = false;
                    ReportViewerRedempTion.LocalReport.Refresh();

                }


            }



        }

        private void MessageBox(string message, string title = "title")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), title, "alert('" + message + "');", true);
        }
    }
}