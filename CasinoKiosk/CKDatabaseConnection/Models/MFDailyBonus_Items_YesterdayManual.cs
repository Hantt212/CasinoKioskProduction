using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CKDatabaseConnection.Models
{
    public class MFDailyBonus_Items_YesterdayManual
    {
        public int ID { get; set; }
        public string ItemName { get; set; }
        public Nullable<int> ItemPoints { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> PlayerID { get; set; }
        public string Type { get; set; }
        public string Kind { get; set; }
    }
}