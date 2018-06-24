using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NDAWebPortal.Controllers;
using NDAWebPortal.Core;
using NDAWebPortal.Core.ClassLib;
using NDAWebPortal.Core.EnumLib;

namespace NDAWebPortal.Areas.Students.Controllers
{
    public class AdmissionController : BaseController
    {
        // GET: Students/Admission
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GenerateAdmissionId()
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var credo =
                        data.Credentials.FirstOrDefault(
                            x => x.Username == UserInformation.UserInformationCredential.Username);

                    if (credo == null)
                        return RedirectToAction("Index");

                    if (data.Admissions.FirstOrDefault(
                        x =>
                            x.CredentialId == UserInformation.UserInformationCredential.Id &&
                            x.SessionId == UserInformation.CurrentSession.Id) == null)
                    {
                        data.Admissions.Add(new Admission()
                        {
                            AccessPin = "",
                            AdmissionId = $"NDA/APP/PG/{UserInformation.UserInformationCredential.Id.ToString("00000")}",
                            AdmissionStatus = (int) AdmissionStates.PreBank,
                            CredentialId = UserInformation.UserInformationCredential.Id,
                            DateAdmissionRequested = DateTime.Now,
                            DatePinApplied = DateTime.Now,
                            IsDeleted = false,
                            SessionId = UserInformation.CurrentSession.Id
                        });
                        data.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ConfirmAdmissionPin(string admissionId, string pin)
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var admission =
                        data.Admissions.FirstOrDefault(
                            x =>
                                x.AdmissionId == admissionId && !x.IsDeleted &&
                                x.SessionId == UserInformation.CurrentSession.Id);

                    if (admission == null)
                        return
                            Json(
                                new
                                {
                                    Status = false,
                                    Message =
                                        "Your Admission ID is not alid. Please check or refresh the page."
                                }, JsonRequestBehavior.DenyGet);

                    if (admission.AdmissionStatus != (int)AdmissionStates.Bank)
                        return
                            Json(
                                new
                                {
                                    Status = false,
                                    Message =
                                        "You have not processed the Admission Properly. Please confirm your current Status"
                                }, JsonRequestBehavior.DenyGet);

                    if (admission.AccessPin != pin)
                        return
                            Json(
                                new
                                {
                                    Status = false,
                                    Message =
                                        "This PIN is not Valid. Please check your Mail or the with the Bank that you made Payment"
                                }, JsonRequestBehavior.DenyGet);

                    admission.AdmissionStatus = (int) AdmissionStates.PostBank;

                    data.Entry(admission).State = EntityState.Modified;
                    data.SaveChanges();

                    return
                        Json(
                            new
                            {
                                Status = true,
                                Message =
                                    "Successful Authentication"
                            }, JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return Json(new {Status = false, Message = ex.Message}, JsonRequestBehavior.DenyGet);
            }
        }
    }
}