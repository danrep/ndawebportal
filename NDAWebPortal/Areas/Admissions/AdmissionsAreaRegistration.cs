using System.Web.Mvc;

namespace NDAWebPortal.Areas.Admissions
{
    public class AdmissionsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admissions";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Admissions_default",
                "Admissions/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "NDAWebPortal.Areas.Admissions.Controllers" }
            );
        }
    }
}