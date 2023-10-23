using CKDatabaseConnection.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace CasinoKiosk.Areas.Admin.Infrastructure
{
    public class CustomRoleProvider : RoleProvider
    {
        public override bool IsUserInRole(string username, string roleName)
        {
            using (var usersContext = new ITHoTram_CustomReportEntities())
            {
                var user = usersContext.CasinoKioskUsers.SingleOrDefault(u => u.CasinoKiosk_UserName == username);
                if (user == null)
                    return false;
                return user.CasinoKioskUserRoles != null && user.CasinoKioskUserRoles.Select(u => u.CasinoKioskRole).Any(r => r.RoleName == roleName);
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            using (var usersContext = new ITHoTram_CustomReportEntities())
            {
                var user = usersContext.CasinoKioskUsers.SingleOrDefault(u => u.CasinoKiosk_UserName == username);
                if (user == null)
                    return new string[] { };
                return user.CasinoKioskUserRoles == null ? new string[] { } : user.CasinoKioskUserRoles.Select(u => u.CasinoKioskRole).Select(u => u.RoleName).ToArray();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            using (var usersContext = new ITHoTram_CustomReportEntities())
            {
                return usersContext.CasinoKioskRoles.Select(r => r.RoleName).ToArray();
            }
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName { get; set; }
    }
}