using System.Web.Mvc;

namespace NDAWebPortal.Areas.Bank
{
    public class BankAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Bank";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Bank_default",
                "Bank/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "NDAWebPortal.Areas.Bank.Controllers" }
            );
        }
    }
}