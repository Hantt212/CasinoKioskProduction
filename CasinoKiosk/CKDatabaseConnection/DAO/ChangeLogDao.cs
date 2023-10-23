using CKDatabaseConnection.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKDatabaseConnection.DAO
{
    public class ChangeLogDao
    {

        ITHoTram_CustomReportEntities context = null;
        public ChangeLogDao()
        {
            context = new ITHoTram_CustomReportEntities();
        }

        public void Insert(CasinoKiosk_ChangeLog changeLog)
        {
            
            context.CasinoKiosk_ChangeLog.Add(changeLog);
            context.SaveChanges();

            //var updateQuantity = item.UpdateItemQuantity(i, i.Quantity - 1);

            
        }
    }
}
