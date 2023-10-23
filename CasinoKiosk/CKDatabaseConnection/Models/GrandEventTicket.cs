using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CKDatabaseConnection.Models
{
    public class GrandEventTicket
    {
        public string PromotionName { get; set; }
        public string TicketNo { get; set; }
        public string PlayerID { get; set; }
        public string PlayerName { get; set; }
        public string IssuedDate { get; set; }
        public string KioskNo { get; set; }
        public string IssuedTime { get; set; }
        public string Amount { get; set; }

    }
}