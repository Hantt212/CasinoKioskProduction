using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKDatabaseConnection.Models
{
    public class FBSplitItem
    {
        public int ID { get; set; }
        public string SplitID { get; set; }
        public string SplitValue { get; set; }
        public string RemainedValue { get; set; }
        public string SplitDate { get; set; }
        public string SplitBy { get; set; }
    }

    public class requestDetailID
    {
        public int ticketid { get; set; }
        public int splitvalue { get; set; }

    }

}
