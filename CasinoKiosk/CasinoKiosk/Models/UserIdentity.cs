﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace CasinoKiosk.Areas.Admin.Models
{

    [Serializable()]
    public class UserIdentity : IIdentity, IPrincipal
        {
            private readonly FormsAuthenticationTicket _ticket;

            public UserIdentity(FormsAuthenticationTicket ticket)
            {
                _ticket = ticket;
            }

            public string AuthenticationType
            {
                get { return "User"; }
            }

            public bool IsAuthenticated
            {
                get { return true; }
            }

            public string Name
            {
                get { return _ticket.Name; }
            }

            public string UserId
            {
                get { return _ticket.UserData; }
            }

            public bool IsInRole(string role)
            {
                return Roles.IsUserInRole(role);
            }

            public IIdentity Identity
            {
                get { return this; }
            }
        }
    }
