using System.Web.Mvc;

namespace NDAWebPortal.Areas.SystemAdministrator
{
    public class SystemAdministratorAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SystemAdministrator";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SystemAdministrator_default",
                "SystemAdministrator/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "NDAWebPortal.Areas.SystemAdministrator.Controllers" }
            );
        }
    }
}