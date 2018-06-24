using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NDAWebPortal.Controllers;
using NDAWebPortal.Core;
using NDAWebPortal.Core.ClassLib;
using NDAWebPortal.Core.EnumLib;

namespace NDAWebPortal.Areas.Students.Controllers
{
    public class RegistrationController : BaseController
    {
        // GET: Students/Registration
        public ActionResult Index()
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {

                    if (
                        data.Registrations.FirstOrDefault(
                            x => x.AdmissionId == UserInformation.Admission.AdmissionId && !x.IsDeleted) == null)
                    {
                        data.Registrations.Add(new Registration()
                        {
                            AdmissionId = UserInformation.Admission.AdmissionId, 
                            CredentialId = UserInformation.Admission.CredentialId, 
                            IsDeleted = false, 
                            RegistrationGuid = Guid.NewGuid().ToString(), 
                            SessionId = UserInformation.CurrentSession.Id
                        });
                        data.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
            }
            return View();
        }

        public bool CheckFileExtension(int fileType, string extension)
        {
            string[] imageValidExtensions = { ".jpg", ".png", ".jpeg" };
            string[] docValidExtensions = { ".pdf" };

            try
            {
                if (fileType == (int) FileUploadType.PassportPort || fileType == (int) FileUploadType.Signature)
                {
                    return imageValidExtensions.Contains(extension);
                }
                else
                {
                    return docValidExtensions.Contains(extension);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return false;
            }
        }

        [HttpPost]
        public JsonResult UploadFiles(int fileType)
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var reg =
                        data.Registrations.FirstOrDefault(
                            x =>
                                x.AdmissionId == UserInformation.Admission.AdmissionId &&
                                x.CredentialId == UserInformation.UserInformationCredential.Id &&
                                !x.IsDeleted &&
                                x.SessionId == UserInformation.CurrentSession.Id);

                    if (reg == null)
                        return
                            Json(
                                new
                                {
                                    Status = false,
                                    Message =
                                        "It would seem that a Registration Session has not been Created for you. Please Log In and Try Again."
                                },
                                JsonRequestBehavior.AllowGet);

                    var file = Request.Files[0];

                    if (file == null)
                        return
                            Json(
                                new { Status = false, Message = "There is no file selected. Please select a file and try again." },
                                JsonRequestBehavior.AllowGet);

                    if (file.ContentLength <= 0)
                        return
                            Json(
                                new { Status = false, Message = "There is no file selected. Please select a file and try again." },
                                JsonRequestBehavior.AllowGet);

                    var fileExtension = Path.GetExtension(file.FileName);
                    var fileName = "";

                    if (fileType == (int)FileUploadType.PassportPort)
                    {
                        fileName = $"PassportPicture_{UserInformation.UserInformationCredential.Username}" + fileExtension;

                        if (!CheckFileExtension(fileType, fileExtension))
                        {
                            return Json(new { Status = false, Message = $"{file.FileName} is not of the acceptable formats. Please Upload PNG, JPG or JPEG" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else if (fileType == (int)FileUploadType.Signature)
                    {
                        fileName = $"SignaturePicture_{UserInformation.UserInformationCredential.Username}" + fileExtension;

                        if (!CheckFileExtension(fileType, fileExtension))
                        {
                            return Json(new { Status = false, Message = $"{file.FileName} is not of the acceptable formats. Please Upload PNG, JPG or JPEG" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else 
                    {
                        fileName = $"{((FileUploadType)fileType)}_{UserInformation.UserInformationCredential.Username}" + fileExtension;

                        if (!CheckFileExtension(fileType, fileExtension))
                        {
                            return Json(new { Status = false, Message = $"{file.FileName} is not of the acceptable formats. Please Upload PDF" }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    var directory = Server.MapPath(
                        $"~/DataStorage/{UserInformation.UserInformationCredential.Username}/{((FileUploadType)fileType).ToString()}");
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    var path = Path.Combine(directory, fileName);
                    file.SaveAs(path);

                    var particular =
                        data.UploadedParticulars.FirstOrDefault(
                            x =>
                                x.DocumentTypeId == fileType &&
                                x.ActiveSessionId == UserInformation.CurrentSession.Id &&
                                x.CredentialId == UserInformation.UserInformationCredential.Id &&
                                !x.IsDeleted);
                    
                    if (particular != null)
                    {
                        data.UploadedParticulars.Remove(particular);
                        data.SaveChanges();
                    }

                    data.UploadedParticulars.Add(new UploadedParticular()
                    {
                        ActiveSessionId = UserInformation.CurrentSession.Id, 
                        AdmissionId = data.Admissions.FirstOrDefault(x => x.CredentialId == UserInformation.UserInformationCredential.Id && x.SessionId == UserInformation.CurrentSession.Id)?.AdmissionId ?? "", 
                        CredentialId = UserInformation.UserInformationCredential.Id, 
                        DocumentTypeId = fileType, 
                        FileNameData = fileName, 
                        IsDeleted = false, 
                        RegistationGuid = reg.RegistrationGuid
                    });
                    data.SaveChanges();

                    return Json(new {Status = true, Message = "File Succesfully Uploaded", Data = fileName},
                        JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return
                    Json(
                        new { Status = false, Message = ex.Message },
                        JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetUploadedDocuments()
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var uploadedDoc =
                        data.UploadedParticulars.Where(
                            x => !x.IsDeleted &&
                                 x.CredentialId == UserInformation.UserInformationCredential.Id &&
                                 x.AdmissionId == UserInformation.Admission.AdmissionId &&
                                 x.ActiveSessionId == UserInformation.CurrentSession.Id &&
                                 x.DocumentTypeId != (int)FileUploadType.PassportPort &&
                                 x.DocumentTypeId != (int)FileUploadType.Signature)
                            .ToList()
                            .Select(x => new {x, FileType = ((FileUploadType) x.DocumentTypeId).DisplayName()});

                    return Json(new {Status = true, Message = "Succesful", Data = uploadedDoc},
                        JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return
                    Json(
                        new { Status = false, Message = ex.Message },
                        JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetUploadedPersonalData()
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var uploadedDoc =
                        data.UploadedParticulars.Where(
                            x => !x.IsDeleted &&
                                 x.CredentialId == UserInformation.UserInformationCredential.Id &&
                                 x.AdmissionId == UserInformation.Admission.AdmissionId &&
                                 x.ActiveSessionId == UserInformation.CurrentSession.Id &&
                                 (x.DocumentTypeId == (int)FileUploadType.PassportPort ||
                                 x.DocumentTypeId == (int)FileUploadType.Signature))
                            .ToList()
                            .Select(x => new { x, FileType = ((FileUploadType)x.DocumentTypeId).DisplayName() });

                    return Json(new { Status = true, Message = "Succesful", Data = uploadedDoc },
                        JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return
                    Json(
                        new { Status = false, Message = ex.Message },
                        JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult LoadRegistrationInformation()
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var regInformation =
                        data.Registrations.FirstOrDefault(
                            x => !x.IsDeleted &&
                                 x.CredentialId == UserInformation.UserInformationCredential.Id &&
                                 x.AdmissionId == UserInformation.Admission.AdmissionId &&
                                 x.SessionId == UserInformation.CurrentSession.Id);

                    if (regInformation == null)
                        return Json(new { Status = false, Message = "No Information Available to Display. Please Contact Support" },
                            JsonRequestBehavior.AllowGet);

                    var dob = regInformation.DateOfBirth?.ToString("dd-MM-yyyy") ?? DateTime.Now.ToString("dd-MM-yyyy");

                    return Json(new { Status = true, Message = "Succesful", Data = regInformation, DateOfBirth = dob},
                        JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return
                    Json(
                        new { Status = false, Message = ex.Message },
                        JsonRequestBehavior.AllowGet);
            }
        }

        private Registration GetCurrentRegistration()
        {
            using (var data = new NDAPortalDatabaseEntities())
            {
                return data.Registrations.FirstOrDefault(x => !x.IsDeleted &&
                    x.AdmissionId == UserInformation.Admission.AdmissionId &&
                    x.SessionId == UserInformation.CurrentSession.Id &&
                    x.CredentialId == UserInformation.UserInformationCredential.Id);
            }
        }

        public JsonResult SavePersonalInformation(Registration reg, string dateOfBirth)
        {
            try
            {
                var dob = DateTime.ParseExact(dateOfBirth, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                if (DateTime.Now.Subtract(dob).TotalDays < 6575)
                    return
                        Json(
                            new
                            {
                                Status = false,
                                Message =
                                    "You are too young to Apply. You need to be at least 18 years or older"
                            }, JsonRequestBehavior.DenyGet);

                using (var data = new NDAPortalDatabaseEntities())
                {
                    var registration = GetCurrentRegistration();

                    if (registration == null)
                        return
                            Json(
                                new
                                {
                                    Status = false,
                                    Message =
                                        "It would seem your Registration Session is not Properly Initiated. Please Log Out and Re Log In. If this persists, please contact Support"
                                }, JsonRequestBehavior.DenyGet);

                    registration.Surname = reg.Surname;
                    registration.FirstName = reg.FirstName;
                    registration.OtherName = reg.OtherName;
                    registration.DateOfBirth = dob;
                    registration.Sex = reg.Sex;
                    registration.MaritalStatus = reg.MaritalStatus;
                    registration.Religion = reg.Religion;
                    registration.PhysicalFeatures = reg.PhysicalFeatures;

                    data.Entry(registration).State = EntityState.Modified;
                    data.SaveChanges();

                    return Json(new { Status = true, Message = "Successful" }, JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return Json(new {Status  = false, Message = ex.Message}, JsonRequestBehavior.DenyGet);
            }
        }

        public JsonResult SaveEthnicityInformation(Registration reg)
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var registration = GetCurrentRegistration();

                    if (registration == null)
                        return
                            Json(
                                new
                                {
                                    Status = false,
                                    Message =
                                        "It would seem your Registration Session is not Properly Initiated. Please Log Out and Re Log In. If this persists, please contact Support"
                                }, JsonRequestBehavior.DenyGet);

                    registration.Nationality = reg.Nationality;
                    registration.HomeAddress = reg.HomeAddress;
                    registration.StateOfOrigin = reg.StateOfOrigin;
                    registration.LgaOfOrigin = reg.LgaOfOrigin;
                    registration.HomeTown = reg.HomeTown;
                    registration.LandMarks = reg.LandMarks;

                    data.Entry(registration).State = EntityState.Modified;
                    data.SaveChanges();

                    return Json(new { Status = true, Message = "Successful" }, JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.DenyGet);
            }
        }

        public JsonResult SaveOtherInfoInformation(Registration reg)
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var registration = GetCurrentRegistration();

                    if (registration == null)
                        return
                            Json(
                                new
                                {
                                    Status = false,
                                    Message =
                                        "It would seem your Registration Session is not Properly Initiated. Please Log Out and Re Log In. If this persists, please contact Support"
                                }, JsonRequestBehavior.DenyGet);

                    registration.BloodType = reg.BloodType;
                    registration.Genotype = reg.Genotype;
                    registration.Hobbies = reg.Hobbies;
                    registration.ForceFullName = reg.ForceFullName;
                    registration.ForceRelationship = reg.ForceRelationship;
                    registration.ForceUnit = reg.ForceUnit;

                    data.Entry(registration).State = EntityState.Modified;
                    data.SaveChanges();

                    return Json(new { Status = true, Message = "Successful" }, JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.DenyGet);
            }
        }

        public JsonResult SaveRefereeInformation(Registration reg)
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var registration = GetCurrentRegistration();

                    if (registration == null)
                        return
                            Json(
                                new
                                {
                                    Status = false,
                                    Message =
                                        "It would seem your Registration Session is not Properly Initiated. Please Log Out and Re Log In. If this persists, please contact Support"
                                }, JsonRequestBehavior.DenyGet);

                    registration.SponsorEmail = reg.SponsorEmail;
                    registration.SponsorFullName = reg.SponsorFullName;
                    registration.SponsorPhoneNumber = reg.SponsorPhoneNumber;
                    registration.SponsorRelationship = reg.SponsorRelationship;
                    registration.SponsorYearlyIncome = reg.SponsorYearlyIncome;

                    registration.NokEmail = reg.NokEmail;
                    registration.NokFullName = reg.NokFullName;
                    registration.NokPhoneNumber = reg.NokPhoneNumber;
                    registration.NokRelationship = reg.NokRelationship;

                    registration.PgEmail = reg.PgEmail;
                    registration.PgFullName = reg.PgFullName;
                    registration.PgPhoneNumber = reg.PgPhoneNumber;
                    registration.PgRelationship = reg.PgRelationship;

                    data.Entry(registration).State = EntityState.Modified;
                    data.SaveChanges();

                    return Json(new { Status = true, Message = "Successful" }, JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.DenyGet);
            }
        }

        public JsonResult SaveProgrammeInformation(int programmeId, int subProgrammeId, int choice)
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var registration = GetCurrentRegistration();

                    if (registration == null)
                        return
                            Json(
                                new
                                {
                                    Status = false,
                                    Message =
                                        "It would seem your Registration Session is not Properly Initiated. Please Log Out and Re Log In. If this persists, please contact Support"
                                }, JsonRequestBehavior.DenyGet);

                    if (choice == 1)
                    {
                        registration.FirstChoiceProgramme = programmeId;
                        registration.FirstChoiceSubProgramme = subProgrammeId;
                    }
                    else if (choice == 2)
                    {
                        registration.SecondChoiceProgramme = programmeId;
                        registration.SecondChoiceSubProgramme = subProgrammeId;
                    }
                    else
                        return
                            Json(
                                new
                                {
                                    Status = false,
                                    Message =
                                        "Please refresh this Page. Something is wrong."
                                }, JsonRequestBehavior.DenyGet);

                    data.Entry(registration).State = EntityState.Modified;
                    data.SaveChanges();

                    return Json(new { Status = true, Message = "Successful" }, JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.DenyGet);
            }
        }

        public ActionResult ConfirmData()
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var admission = data.Admissions.FirstOrDefault(x => x.Id == UserInformation.Admission.Id);

                    if (admission == null)
                        return RedirectToAction("Index");

                    admission.AdmissionStatus = (int) AdmissionStates.SchoolApprovalPending;
                    data.Entry(admission).State = EntityState.Modified;
                    data.SaveChanges();

                    UserInformation.ActivateSession(UserInformation.UserInformationCredential);

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