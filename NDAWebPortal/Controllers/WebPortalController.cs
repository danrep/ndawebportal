using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NDAWebPortal.Core;
using NDAWebPortal.Core.ClassLib;
using NDAWebPortal.Core.EnumLib;

namespace NDAWebPortal.Controllers
{
    public class WebPortalController : BaseController
    {
        // GET: HomeBase
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ManageProfile()
        {
            return View();
        }

        [HttpPost]
        public JsonResult UpdateProfile(Credential credential)
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var credo =
                        data.Credentials.FirstOrDefault(
                            x => x.Username == UserInformation.UserInformationCredential.Username);

                    if (credo == null)
                        return
                            Json(
                                new
                                {
                                    Status = false,
                                    Message = "You are not properly authenticated. Please refresh the page"
                                },
                                JsonRequestBehavior.DenyGet);

                    credo.FirstName = credential.FirstName;
                    credo.Surname = credential.Surname;
                    credo.OtherNames = credential.OtherNames;
                    credo.PhoneNumber = credential.PhoneNumber;

                    data.Entry(credo).State = EntityState.Modified;
                    data.SaveChanges();

                    UserInformation.ActivateSession(credo);
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
                return Json(new {Status = false, Message = ex.Message}, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        public JsonResult GetProfile()
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var credo =
                        data.Credentials.FirstOrDefault(
                            x => x.Username == UserInformation.UserInformationCredential.Username);

                    if (credo == null)
                        return
                            Json(
                                new
                                {
                                    Status = false,
                                    Message = "You are not properly authenticated. Please refresh the page"
                                },
                                JsonRequestBehavior.AllowGet);
                    return
                        Json(
                            new
                            {
                                Status = true,
                                Message = "Profile Retrieval Successful",
                                Data = credo
                            },
                            JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetStates()
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var states =
                        data.States.OrderBy(x => x.StateName).ToList();

                    return
                        Json(
                            new
                            {
                                Status = true,
                                Message = "Successful",
                                Data = states
                            },
                            JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetStateLga(string stateName)
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var state = data.States.FirstOrDefault(x => x.StateName == stateName);

                    var lga =
                        data.Localities.Where(x => x.StateId == state.StateId).OrderBy(x => x.LocalityName).ToList();

                    return
                        Json(
                            new
                            {
                                Status = true,
                                Message = "Successful",
                                Data = lga
                            },
                            JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}