using System;
using System.Linq;
using System.Web.Mvc;
using ReportManagement;
using SiteBlue.Data.EightHundred;

namespace SiteBlue.Controllers
{
    public abstract class SiteBlueBaseController : PdfViewController
    {
        private SessionContainer _userInfo;
        public SessionContainer UserInfo
        {
            get
            {
                //If user is no longer authenticated, force pulling
                //the session container again to prevent using old user info.
                if (!HttpContext.User.Identity.IsAuthenticated && _userInfo != null)
                    _userInfo = null;

                return _userInfo ?? (_userInfo = SessionContainer.GetInstance());
            }
        }

        protected static EightHundredEntities GetContext()
        {
            return new EightHundredEntities(SessionContainer.GetInstance().UserKey);
        }

        public PartialViewResult FranchiseSelectionHeader()
        {
            return PartialView("FranchiseSelectionHeader", this.UserInfo);
        }

        [HttpPost]
        public JsonResult SetCurrentFranchise(int id, bool? showInactiveFranchises)
        {
            UserInfo.ShowInactiveFranchises = showInactiveFranchises.GetValueOrDefault();

            if (id != 0)
            {
                try
                {
                    UserInfo.CurrentFranchise = UserInfo.Franchises.SingleOrDefault(f => f.FranchiseID == id);
                }
                catch (Exception)
                {
                    //intentoinally swallow exception...this is not critical.
                }
            }
            return Json(new {Success = true});
        }
    }
}
