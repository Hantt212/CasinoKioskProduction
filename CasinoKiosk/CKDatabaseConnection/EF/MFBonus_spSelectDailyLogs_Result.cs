//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CKDatabaseConnection.EF
{
    using System;
    
    public partial class MFBonus_spSelectDailyLogs_Result
    {
        public int ID { get; set; }
        public Nullable<int> PlayerID { get; set; }
        public string PlayerName { get; set; }
        public string IssueDate { get; set; }
        public string IssueTime { get; set; }
        public string PromotionName { get; set; }
        public Nullable<int> DailyPoints { get; set; }
        public Nullable<int> SlotDailyPoints { get; set; }
        public Nullable<int> TableDailyPoints { get; set; }
        public Nullable<int> ItemID { get; set; }
        public string ItemName { get; set; }
        public Nullable<int> ItemPoints { get; set; }
        public Nullable<int> Status { get; set; }
        public string GamingDate { get; set; }
        public string Type { get; set; }
        public string voidedStatus { get; set; }
        public string voidedPerson { get; set; }
        public string reprintedPerson { get; set; }
        public Nullable<System.DateTime> voidedTime { get; set; }
        public string Location { get; set; }
        public Nullable<System.DateTime> reprintedTime { get; set; }
    }
}
