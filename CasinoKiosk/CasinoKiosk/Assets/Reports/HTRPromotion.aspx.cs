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
    public partial class HTRPromotion : System.Web.UI.Page
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
                SqlCommand cmd = new SqlCommand("spHTR_PromotionLogByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", id);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                
            }
            return dt;
        }
        private void showReport()
        {
            int id = 0;
            DataTable dt = new DataTable();
            if (!String.IsNullOrEmpty(Request.QueryString["id"]))
            {

                id = Convert.ToInt32(Request.QueryString["id"]);
            }
            dt = GetData(id);

            if (dt != null)
            {
                ReportDataSource rds = new ReportDataSource("Promotion", dt);

                ReportPromotion.LocalReport.DataSources.Clear();
                ReportPromotion.LocalReport.DataSources.Add(rds);
                ReportPromotion.LocalReport.ReportPath = Server.MapPath("~/Assets/Reports/HTRPromotion.rdlc");

                ReportPromotion.DataBind();
                ReportPromotion.ShowToolBar = false;
                ReportPromotion.LocalReport.Refresh();

            }

        }

        private void MessageBox(string message, string title = "title")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), title, "alert('" + message + "');", true);
        }
    }
}
