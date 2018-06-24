using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using NDAWebPortal.Controllers;
using NDAWebPortal.Core;
using NDAWebPortal.Core.ClassLib;
using NDAWebPortal.Core.EnumLib;

namespace NDAWebPortal.Areas.Admissions.Controllers
{
    public class AdmissionManagementController : BaseController
    {
        // GET: Admissions/AdmissionManagement
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApprovedApplications()
        {
            return View();
        }

        public ActionResult DeniedApplications()
        {
            return View();
        }

        public ActionResult AdmissionData(string admissionId, string registrationGuid)
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var registration = data.Registrations.FirstOrDefault(x => !x.IsDeleted &&
                                                                              x.AdmissionId == admissionId &&
                                                                              x.RegistrationGuid == registrationGuid &&
                                                                              x.SessionId ==
                                                                              UserInformation.CurrentSession.Id);

                    if (registration == null)
                        return RedirectToAction("Index");

                    return View(registration);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return RedirectToAction("Index");
            }
        }

        public JsonResult GetRegistrations(int admissionStatus)
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var admissions =
                        data.Admissions.Where(
                            x =>
                                !x.IsDeleted && x.SessionId == UserInformation.CurrentSession.Id &&
                                x.AdmissionStatus == admissionStatus).Select(x => x.AdmissionId).ToList();

                    var registrants = data.Registrations.Where(x => admissions.Contains(x.AdmissionId)).ToList();

                    return Json(new {Status = true, Message = $"Successful. {registrants.Count} Pending Approval Registration", Data = registrants},
                        JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return Json(new {Status = false, Message = ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetCurrentRegistration(string admissionId, string registrationGuid)
        {
            using (var data = new NDAPortalDatabaseEntities())
            {
                var registration = data.Registrations.FirstOrDefault(x => !x.IsDeleted &&
                                                                          x.AdmissionId == admissionId &&
                                                                          x.SessionId ==
                                                                          UserInformation.CurrentSession.Id &&
                                                                          x.RegistrationGuid == registrationGuid);
                return Json(new
                {
                    Status = true,
                    Data = registration,
                    Message = "Successful"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ProcessApplication(string admissionId, bool approvalStatus)
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var application =
                        data.Admissions.FirstOrDefault(
                            x => x.AdmissionId == admissionId && x.SessionId == UserInformation.CurrentSession.Id && !x.IsDeleted);

                    if (application == null)
                        return RedirectToAction("Index");

                    var credential = data.Credentials.FirstOrDefault(x => x.Id == application.CredentialId);

                    if (credential == null)
                        return RedirectToAction("Index");

                    var message = $"Dear {credential.Surname}, {credential.FirstName} {credential.OtherNames},<br/>";
                    if (approvalStatus)
                    {
                        application.AdmissionStatus = (int) AdmissionStates.SchoolApproved;
                        message += "Congratulations! Your application has been approved. You may proceed with Tuition Payment and Further Registration.";
                    }
                    else
                    { application.AdmissionStatus = (int)AdmissionStates.SchoolDenied;
                        message += "Your application has been Processed. However, your application has been denied. You may re-apply next Session.";
                    }

                    message += "<br/><br/>Regards";

                    new Thread(() =>
                    {
                        Messaging.SendMail(credential.Username, null, null, "Application Processing", message, null);
                    }).Start();

                    data.Entry(application).State = EntityState.Modified;
                    data.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return RedirectToAction("Index");
            }
        }
    }
}