using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CKDatabaseConnection.Models
{
    public class PlayerInfo
    {
        public int ID
        { get; set; }

        public string FirstName
        { get; set; }

        public string MiddleName
        { get; set; }

        public string LastName
        { get; set; }

        public string DOB
        { get; set; }

        public string Rank
        { get; set; }

        public string Status
        { get; set; }

        public int Points
        { get; set; }

        public int TodayPoints
        { get; set; }
        public int PeriodPoints// for GRAND BACCARAT 01/05/2020
        { get; set; }

        public int ThursdayPoint // for Bounty Hunting 23/07/2020
        { get; set; }
        public int NumDayEnrolled
        { get; set; }

        public int isShowBountyHunting { get; set; }

        public string GamingDate
        { get; set; }
        public string SegmentationCode { get; set; }
        public int TotalRedeemedTicket
        { get; set; }

        public int GrandEventRedeemedTicket
        { get; set; }

        public int PointEarnForGrandEvent
        { get; set; }

        public int PointEarnForProxyNightEvent { get; set; }

        public int ProxyNighEventRedeemedTicket { get; set; }

        public int ProxyNightEventRedeemedTicket { get; set; }

        public string Promotion1688RedeemedTicket { get; set; }

        public int PointEarnForMidAutumn { get; set; }

        public int TotalRedeemedTicketMidAutumn { get; set; }



        public int PointEarnForSoundWave { get; set; }

        public int TotalRedeemedTicketSoundWave { get; set; }

        public int PointEarnFor8DragonOffer { get; set; }

        public int TotalRedeemedTicketDragonOffer { get; set; }

        public int PointEarnForDoubleProsperity { get; set; }

        public int TotalRedeemedTicketDoubleProsperity { get; set; }

        public int TotalRedeemedTicketDoubleProsperity_Today { get; set; }

        public int PointEarnForRedEnvelope { get; set; }

        public int PromotionRedEnvelopeRedeemedTicket { get; set; }

        public int PointEarnForSlotTournament { get; set; }

        public int TotalRedeemedTicketSlotTournament { get; set; }

        public int PointEarnForAnniversary { get; set; }

        public int PromotionAnniversary_RedeemedTicket { get; set; }

        public int TheGrandLuckyDraw_RedeemedTicket { get; set; }

        public int PointForTheGrandLuckyDraw { get; set; }

        public int TotalRedeemedTicketNewYear_today { get; set; }

        public int TotalRedeemedTicketFb8Dragon_today { get; set; }

        public int TotalRedeemedTicketGrandBaccaratTournament { get; set; }



        public int TotalRedeemedTicketBountyHuntingSilver { get; set; }

        public int TotalRedeemedTicketBountyHuntingGolden { get; set; }

        public int TotalRedeemedTicketBountyHuntingGoldenChestAllPlayer { get; set; }

        public int TotalRedeemedTicketBountyHuntingSilverChestAllPlayer { get; set; }
    }
}