using System;
using System.Linq;
using System.Web.Mvc;
using NDAWebPortal.Core;
using NDAWebPortal.Core.ClassLib;
using NDAWebPortal.Core.EnumLib;

namespace NDAWebPortal.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                if (!UserInformation.IsSessionValid)
                {
                    filterContext.Result = RedirectToAction("Index", "Home", new { area = "" });
                    return;
                }

                var area = filterContext.RouteData.DataTokens["area"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(area))
                {
                    if (UserInformation.CredentialAreas.FirstOrDefault(x => x.AreaName == area) == null)
                        if (area != UserInformation.Route)
                        {
                            filterContext.Result = RedirectToAction("Index", "Home", new { area = "" });
                            UserInformation.DeactivateSession();
                            return;
                        }
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                filterContext.Result = RedirectToAction("Index", "Home", new { area = "" });
                return;
            }
        }
    }
}