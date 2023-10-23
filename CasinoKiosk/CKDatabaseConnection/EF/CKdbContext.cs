namespace CKDatabaseConnection.EF
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class CKdbContext : DbContext
    {
        public CKdbContext()
            : base("name=CKdbContext")
        {
        }

        public virtual DbSet<CasinoKiosk_Item> CasinoKiosk_Item { get; set; }
        public virtual DbSet<CasinoKiosk_Log> CasinoKiosk_Log { get; set; }
        public virtual DbSet<CasinoKioskUser> CasinoKioskUser { get; set; }
        public virtual DbSet<CIFE_Players> CIFE_Players { get; set; }
        public virtual DbSet<CasinoKiosk_ChangeLog> CasinoKiosk_ChangeLog { get; set; }
        public virtual DbSet<MFDailyBonus_Items> MFDailyBonus_Items { get; set; }
        public virtual DbSet<MFDailyBonus_Items_Yesterday> MFDailyBonus_Items_Yesterdays { get; set; }
        public virtual DbSet<MFDailyBonus_YesterdayItemsManual_Log> MFDailyBonus_YesterdayItemsManual_Logs { get; set; }
        public virtual DbSet<MFDailyBonus_Players> MFDailyBonus_Players { get; set; }
        
        public virtual DbSet<MFDailyBonus_SecondLogs> MFDailyBonus_SecondLogs { get; set; }
        public virtual DbSet<MFWeeklyBonus_Items> MFWeeklyBonus_Items { get; set; }
        public virtual DbSet<MFWeeklyBonus_Logs> MFWeeklyBonus_Logs { get; set; }
        public virtual DbSet<MFWeeklyBonus_Players> MFWeeklyBonus_Players { get; set; }
        public virtual DbSet<MFFridayBonus_Items> MFFridayBonus_Items { get; set; }
        public virtual DbSet<MFFridayBonus_Logs> MFFridayBonus_Logs { get; set; }
        public virtual DbSet<MFFridayBonus_Players> MFFridayBonus_Players { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
        }
    }
}
