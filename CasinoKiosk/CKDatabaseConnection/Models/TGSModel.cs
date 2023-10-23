using CKDatabaseConnection.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKDatabaseConnection.Models
{
    public class TGSModel
    {
        public int? PID { get; set; }
        public string PlayerName { get; set; }
        public List<CasinoTheGrandSignaturePlayerQualified> CasinoTGSPlayers { get;set;}
    }
}
