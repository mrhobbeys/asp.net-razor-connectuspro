using System;
using System.Web.Mvc;

namespace SiteBlue.Controllers
{
    [OutputCache(Duration = 1, VaryByParam = "*")] 
    public partial class SecurityGuardBaseController : Controller
    {
        /// <summary>
        /// This provides simple feedback to the modelstate in the case of errors.
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Result is RedirectToRouteResult)
            {
                try
                {
                    // put the ModelState into TempData
                    TempData.Add("_MODELSTATE", ModelState);
                }
                catch (Exception)
                {
                    TempData.Clear();
                    // swallow exception
                }
            }
            else if (filterContext.Result is ViewResult && TempData.ContainsKey("_MODELSTATE"))
            {
                // merge modelstate from TempData
                var modelState = TempData["_MODELSTATE"] as ModelStateDictionary;
                foreach (var item in modelState)
                {
                    if (!ModelState.ContainsKey(item.Key))
                        ModelState.Add(item);
                }
            }
            base.OnActionExecuted(filterContext);
        }
    }
}
