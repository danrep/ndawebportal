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

namespace NDAWebPortal.Areas.Bank.Controllers
{
    public class PaymentConfirmationsController : BaseController
    {
        public ActionResult AdmissionPaymentConfirmation(string admissionId, bool? confirmation)
        {
            ViewBag.AID = "";

            if (string.IsNullOrEmpty(admissionId))
            {
                return View();
            }

            using (var data = new NDAPortalDatabaseEntities())
            {
                var admission =
                    data.Admissions.FirstOrDefault(
                        x => x.AdmissionId == admissionId && x.SessionId == UserInformation.CurrentSession.Id);

                if (admission != null)
                {
                    if (confirmation == null)
                        return View(admission);

                    var pin = Encryption.GetUniqueKey(15);
                    admission.AdmissionStatus = (int) AdmissionStates.Bank;
                    admission.AccessPin = pin;
                    admission.DatePinApplied = DateTime.Now;

                    data.PinMaps.Add(new PinMap()
                    {
                        CanExpire = false, 
                        CredentialAssigned = UserInformation.UserInformationCredential.Id, 
                        CredentialId = admission.CredentialId, 
                        ExpiryDate = DateTime.Now, 
                        HasUseCounts = false, 
                        InitiationDate = DateTime.Now, 
                        IsDeleted = false, 
                        PinData = pin, 
                        PinUse = (int)PinTypes.AdmissionPin, 
                        UseCount = 0, 
                        UseCountMax = 0
                    });

                    data.Entry(admission).State = EntityState.Modified;
                    data.SaveChanges();
                    SendPin(admissionId);

                    return View(admission);
                }

                ViewBag.AID = admissionId;
                return View();
            }
        }

        public void SendPin(string admissionId)
        {
            var sessionId = UserInformation.CurrentSession.Id;

            new Thread(() =>
            {
                try
                {
                    using (var data = new NDAPortalDatabaseEntities())
                    {
                        var admission =
                            data.Admissions.FirstOrDefault(x => x.AdmissionId == admissionId && x.SessionId == sessionId);

                        var credential = data.Credentials.FirstOrDefault(x => x.Id == admission.CredentialId);

                        var message = $"Dear {credential.Surname}, {credential.FirstName} {credential.OtherNames}, <br/>";
                        message += $"Your Payment has been confirmed by the bank and your PIN has been generated. Find your PIN below.<br/>";
                        message += $"PIN: <strong>{admission.AccessPin}</strong>. <br/> <br/> Regards.";

                        Core.ClassLib.Messaging.SendMail(credential.Username, "", "", "NDA Admission PIN", message, "");
                    }
                }
                catch (Exception ex)
                {
                        ActivityLogger.Log(ex);
                }
            }).Start();
        }

        public JsonResult ResendPin(string admissionId)
        {
            SendPin(admissionId);
            return Json(new {Status = true}, JsonRequestBehavior.AllowGet);
        }
    }
}