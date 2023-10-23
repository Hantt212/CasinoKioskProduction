using CKDatabaseConnection.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKDatabaseConnection.DAO
{
    
    public class TestDao
    {
        
        ITHoTram_CustomReportEntities context = null;

        public TestDao()
        {
            
            context = new ITHoTram_CustomReportEntities();
        }


        public IEnumerable<MFBonus_spSelectPlayerPoints_Result> ListAllPagingPoints(int page, int pageSize, int playerID)
        {
            List<MFBonus_spSelectPlayerPoints_Result> list = context.MFBonus_spSelectPlayerPoints_Result(playerID).ToList();
            return list.OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
        }

        public IEnumerable<HTAppGetPlayerInfoByPID_1_Result> ListAllPagingPlayerInfo(int page, int pageSize, int playerID, string idNumber)
        {
            List<HTAppGetPlayerInfoByPID_1_Result> list = context.HTAppGetPlayerInfoByPID_1(playerID, idNumber).ToList();
            return list.OrderByDescending(x => x.PlayerID).ToPagedList(page, pageSize);
        }

    }
}
