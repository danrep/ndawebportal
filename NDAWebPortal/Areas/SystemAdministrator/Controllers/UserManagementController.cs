using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NDAWebPortal.Controllers;
using NDAWebPortal.Core.ClassLib;
using NDAWebPortal.Core.EnumLib;

namespace NDAWebPortal.Areas.SystemAdministrator.Controllers
{
    public class UserManagementController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ManageUser(string username)
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var userCredo = data.Credentials.FirstOrDefault(x => x.Username == username && !x.IsDeleted);

                    if (userCredo == null)
                        return RedirectToAction("Index");

                    return View(userCredo);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public JsonResult AddProfile(Credential credential)
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var credo =
                        data.Credentials.FirstOrDefault(
                            x => x.Username == credential.Username);

                    if (credo != null)
                        return
                            Json(
                                new
                                {
                                    Status = false,
                                    Message = "This Credential already exists. Please configure another"
                                },
                                JsonRequestBehavior.DenyGet);

                    var passwordSalt = Encryption.GetUniqueKey(8);
                    credential.IsDeleted = false;
                    credential.LasLogIn = DateTime.Now;
                    credential.Password = Core.ClassLib.Encryption.SaltEncrypt(Encryption.GetUniqueKey(8), passwordSalt);
                    credential.UserState = (int)UserStates.Active;
                    credential.PasswordSalt = passwordSalt;
                    
                    data.Credentials.Add(credential);
                    data.SaveChanges();

                    return
                        Json(
                            new
                            {
                                Status = true,
                                Message = "Profile Update Successful"
                            },
                            JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        public JsonResult GetProfiles()
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var users = data.Credentials.Where(x => !x.IsDeleted).OrderBy(x => x.Username).ToList();
                    return Json(new { Status = true, Message = $"Successful. {users.Count} Users found", Data = users }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return Json(new {Status = false, Message = ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult MapRoles(int credentialId, string areaName, bool isDeleted)
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var role =
                        data.CredentialAreas.FirstOrDefault(
                            x => x.AreaName == areaName && x.CredentialId == credentialId);

                    if (role == null)
                    {
                        data.CredentialAreas.Add(new CredentialArea()
                        {
                            AreaName = areaName,
                            CredentialId = credentialId,
                            IsDeleted = isDeleted
                        });
                    }
                    else
                    {
                        role.IsDeleted = isDeleted;
                    }
                    data.SaveChanges();

                    return Json(new { Status = true, Message = "Successful" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return Json(new { Status = true, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult DeleteUser(int credentialId)
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var credential =
                        data.Credentials.FirstOrDefault(x => x.Id == credentialId && !x.IsDeleted);

                    if (credential == null)
                        return Json(new { Status = false, Message = "User NOT Found" }, JsonRequestBehavior.AllowGet);

                    credential.IsDeleted = true;
                    data.SaveChanges();

                    return Json(new { Status = true, Message = "Successful" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return Json(new { Status = true, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}