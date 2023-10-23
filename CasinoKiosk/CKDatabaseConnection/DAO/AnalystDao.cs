using CKDatabaseConnection.EF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKDatabaseConnection.DAO
{
    public class AnalystDao
    {
        ITHoTram_CustomReportEntities context = null;
        SqlConnection pmCon = new SqlConnection("Data Source=10.70.1.53;Initial Catalog=ITHoTram_CustomReport;User Id=casinokiosk.user; Password=P@ssword1;");
        CKFunction function = new CKFunction();

        public DataSet getCompSettleTypeRedeemOutlet()
        {

            DataSet ds = new DataSet();

            using (pmCon)
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "spFPA_CompSettleTypeRedeemOutlet";
                    cmd.Connection = pmCon;
                    cmd.CommandType = CommandType.StoredProcedure;

                    pmCon.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    adapter.Fill(ds);

                    pmCon.Close();
                }
            }

            return ds;
        }
        public List<POSPatronInfoUser> ListAllPOS8DragonUser()
        {
            context.Configuration.ProxyCreationEnabled = false;
            return context.POSPatronInfoUsers.Where(u => u.isShow == true).OrderByDescending(X => X.ID).ToList();
        }

    }
}
