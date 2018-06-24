using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NDAWebPortal.Core.EnumLib;

namespace NDAWebPortal
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            using (var data = new NDAPortalDatabaseEntities())
            {
                var adminCredo = data.Credentials.FirstOrDefault(x => x.Username == "administrator@ndawebportal.com");
                if (adminCredo != null)
                    return;

                var passwordSalt = Core.ClassLib.Encryption.GetUniqueKey(8);
                adminCredo = data.Credentials.Add(new Credential()
                {
                    Username = "administrator@ndawebportal.com", 
                    Surname = "System", 
                    OtherNames = "", 
                    IsDeleted = false,
                    FirstName = "Administrator", 
                    LasLogIn = DateTime.Now, 
                    Password = Core.ClassLib.Encryption.SaltEncrypt(Core.ClassLib.Encryption.GetUniqueKey(8),  passwordSalt), 
                    PasswordSalt = passwordSalt, 
                    PhoneNumber = "000-0000-000", 
                    UserState = (int)UserStates.Active
                });
                data.SaveChanges();

                data.CredentialAreas.Add(new CredentialArea()
                {
                    IsDeleted = false, 
                    AreaName = UserRoles.SystemAdministrator.ToString(), 
                    CredentialId = adminCredo.Id
                });
                data.SaveChanges();
            }
        }
    }
}
