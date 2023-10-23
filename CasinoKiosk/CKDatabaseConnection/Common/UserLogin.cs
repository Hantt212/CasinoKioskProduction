using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CKDatabaseConnection.Common
{
    [Serializable]
    public static class UserLogin
    {
        public static int AdminRoleID { get; set; }
        public static int StaffRoleID { get; set; }
        public static string Username { get; set; }

        public static int UserID { get; set; }
    }
}