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
    
    public partial class MFBonus_PlayerPoints
    {
        public int ID { get; set; }
        public Nullable<int> PlayerID { get; set; }
        public Nullable<int> DailyPoints { get; set; }
        public Nullable<int> WeeklyPoints { get; set; }
        public Nullable<int> FridayPoints { get; set; }
        public Nullable<System.DateTime> GamingDate { get; set; }
    }
}
