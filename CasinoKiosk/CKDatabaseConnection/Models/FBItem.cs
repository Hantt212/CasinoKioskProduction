using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CKDatabaseConnection.Models
{
    public class FBItem
    {
        public int ID { get; set; }
        public string ItemName { get; set; }
        public string itemValue { get; set; }
        public string issuedDate { get; set; }
        public string issuedBy { get; set; }
        public string ticketID { get; set; }


        public string issuedTime { get; set; }
        public bool status { get; set; }
        public int playerID { get; set; }
        public string strstatus { get; set; }
        public string isSplit { set; get; }



    }
}