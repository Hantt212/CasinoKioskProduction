using CKDatabaseConnection.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKDatabaseConnection.Models
{
    public class TrackingActivityViewModel
    {
        
        public string UserName { get; set; }        
        public List<CasinoTrackingActivity> TrackingActivites { get; set; }
    }
}
