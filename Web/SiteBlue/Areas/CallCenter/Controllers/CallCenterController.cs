using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SecurityGuard.Interfaces;
using SecurityGuard.Services;
using SiteBlue.Controllers;

namespace SiteBlue.Areas.CallCenter.Controllers
{
    public class CallCenterController : SiteBlueBaseController
    {
        protected IMembershipService MembershipService;
        protected IAuthenticationService AuthenticationService;

        public CallCenterController()
        {
            MembershipService = new MembershipService(Membership.Provider);
            AuthenticationService = new AuthenticationService(MembershipService, new FormsAuthenticationService());
        }

    }
}
