using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NDAWebPortal.Core;
using NDAWebPortal.Core.ClassLib;
using NDAWebPortal.Core.EnumLib;

namespace NDAWebPortal.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult LogIn(string username, string password)
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var userInformation =
                        data.Credentials.FirstOrDefault(x => x.Username == username && x.IsDeleted == false);

                    if (userInformation == null)
                        return
                            Json(
                                new
                                {
                                    Status = false,
                                    Message = "This Username does not exist on this Platform. Please Register"
                                },
                                JsonRequestBehavior.AllowGet);

                    if (!Core.ClassLib.Encryption.IsSaltEncryptValid(password, userInformation.Password,
                        userInformation.PasswordSalt))
                        return Json(new {Status = false, Message = "Your Password is Incorrect"},
                            JsonRequestBehavior.AllowGet);

                    UserInformation.ActivateSession(userInformation);
                    return Json(new {Status = true, Message = "Successful Authentication"}, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return Json(new {Status = false, Message = ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Registration(string username, string password, string phoneNumber)
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var userInformation =
                        data.Credentials.FirstOrDefault(x => x.Username == username && x.IsDeleted == false);

                    if (userInformation != null)
                        return
                            Json(
                                new
                                {
                                    Status = false,
                                    Message = "This Username already exists on this Platform. Please Log In"
                                },
                                JsonRequestBehavior.AllowGet);

                    var passwordSalt = Encryption.GetUniqueKey(10);
                    userInformation = data.Credentials.Add(new Credential()
                    {
                        Username = username,
                        IsDeleted = false, 
                        PasswordSalt = passwordSalt, 
                        UserState = (int)UserStates.Active,
                        Password = Encryption.SaltEncrypt(password, passwordSalt), 
                        FirstName = "", 
                        LasLogIn = DateTime.Now, 
                        OtherNames = "", 
                        Surname = "", 
                        PhoneNumber = phoneNumber
                    });
                    data.SaveChanges();

                    data.CredentialAreas.Add(new CredentialArea()
                    {
                        AreaName = UserRoles.Students.ToString(), 
                        IsDeleted = false, 
                        CredentialId = userInformation.Id
                    });
                    data.SaveChanges();

                    return Json(new { Status = true, Message = "Successful Registration" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RecoverPassword(string username)
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var userInformation =
                        data.Credentials.FirstOrDefault(x => x.Username == username && x.IsDeleted == false);

                    if (userInformation == null)
                        return
                            Json(
                                new
                                {
                                    Status = false,
                                    Message = "This Username does not exist on this Platform. Please Register"
                                },
                                JsonRequestBehavior.AllowGet);

                    var password = Encryption.SaltDecrypt(userInformation.Password, userInformation.PasswordSalt);
                    var displayName = userInformation.Surname == "" ? userInformation.Username : userInformation.Surname + ", " + userInformation.OtherNames;
                    var message = $"Dear {displayName}, <br/>";
                    message += $"You have requested that your Password be sent to You. Your Password is <strong>{password}</strong>.<br/><br/>";
                    message += $"Regards.";

                    Messaging.SendMail(userInformation.Username, "", "", "Recovered Password", message, "");

                    return Json(new { Status = true, Message = "Successful Retrieval" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LogOut()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetNoStore();

            UserInformation.DeactivateSession();

            return RedirectToAction("Index", "Home", new {area = ""});
        }
    }
}