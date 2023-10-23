using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CasinoKiosk.Assets.Reports
{
    public partial class CifePlayers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ReportViewerPlayers.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;

                ReportViewerPlayers.ServerReport.ReportServerUrl = new Uri("http://10.70.1.53/ReportServer");

                ReportViewerPlayers.ServerReport.ReportPath = "/Custom Reports/CIFE Program";
                // ReportViewer1.ServerReport.ReportPath = "/ELearning/2021/LearningProgressByEmployee_2021";
                ReportViewerPlayers.ServerReport.Refresh();
            }
        }
    }
}