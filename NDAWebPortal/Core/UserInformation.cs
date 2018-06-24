using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NDAWebPortal.Core.EnumLib;

namespace NDAWebPortal.Core
{
    public static class UserInformation
    {
        public static void ActivateSession(Credential userData)
        {
            HttpContext.Current.Session["UserData"] = userData;
            var data = new NDAPortalDatabaseEntities();
            CurrentSession = data.Sessions.FirstOrDefault(x => x.IsCurrentSession);

            if (userData.Id == 0)
            {
                Route = UserRoles.SystemAdministrator.ToString();
                RoleName = UserRoles.SystemAdministrator.DisplayName();
            }
            else
            {
                Route = "";
                RoleName = "";
                CredentialAreas =
                    data.CredentialAreas.Where(x => x.IsDeleted == false && x.CredentialId == userData.Id)
                        .OrderBy(x => x.AreaName)
                        .ToList();

                if (CredentialAreas.Any(x => x.AreaName == UserRoles.Students.ToString()))
                {
                    Admission =
                        data.Admissions.FirstOrDefault(
                            x => x.CredentialId == userData.Id && x.SessionId == CurrentSession.Id && !x.IsDeleted);
                }
            }
            data.Dispose();
        }

        public static bool IsSessionValid => HttpContext.Current.Session["UserData"] != null;

        public static Credential UserInformationCredential => ((Credential)HttpContext.Current.Session["UserData"]);

        public static void DeactivateSession()
        {
            HttpContext.Current.Session["UserData"] = null;
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.RemoveAll();
        }

        public static string Route { get; private set; }

        public static string RoleName { get; private set; }

        public static List<CredentialArea> CredentialAreas { get; private set; }

        public static Session CurrentSession { get; private set; }

        public static Admission Admission { get; private set; }
    }
}
