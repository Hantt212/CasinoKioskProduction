
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
    
public partial class CasinoKiosk_Log
{

    public int ID { get; set; }

    public string LogName { get; set; }

    public string createdDate { get; set; }

    public Nullable<int> PlayerID { get; set; }

    public string PlayerName { get; set; }

    public string PromotionName { get; set; }

    public Nullable<int> ItemPoints { get; set; }

    public Nullable<int> Status { get; set; }

    public string createdTime { get; set; }

    public Nullable<int> CurrentPlayerPoints { get; set; }

    public int Quantity { get; set; }

    public string voidedStatus { get; set; }

    public Nullable<System.DateTime> gamingDate { get; set; }

    public string voidedPerson { get; set; }

    public string reprintedPerson { get; set; }

    public Nullable<System.DateTime> reprintedTime { get; set; }

    public Nullable<System.DateTime> voidedTime { get; set; }

    public string Location { get; set; }

}

}
