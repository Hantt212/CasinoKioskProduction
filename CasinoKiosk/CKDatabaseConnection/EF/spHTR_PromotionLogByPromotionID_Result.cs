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
    
    public partial class spHTR_PromotionLogByPromotionID_Result
    {
        public int ID { get; set; }
        public int PromotionId { get; set; }
        public Nullable<int> PlayerID { get; set; }
        public string PlayerName { get; set; }
        public Nullable<System.DateTime> PrintedDate { get; set; }
        public string PrintedBy { get; set; }
        public Nullable<bool> isPrinted { get; set; }
        public string ReprintedBy { get; set; }
        public Nullable<System.DateTime> ReprintedDate { get; set; }
        public Nullable<bool> isVoided { get; set; }
        public Nullable<System.DateTime> VoidedDate { get; set; }
        public string VoidedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
