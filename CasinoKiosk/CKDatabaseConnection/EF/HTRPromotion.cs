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
    using System.Collections.Generic;
    
    public partial class HTRPromotion
    {
        public int PromotionId { get; set; }
        public string PromotionName { get; set; }
        public Nullable<bool> IsActived { get; set; }
        public string PromotionContent { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedTime { get; set; }
        public string Condition { get; set; }
        public Nullable<bool> IsDisplayLuckyDate { get; set; }
    }
}
