using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NDAWebPortal.Controllers;
using NDAWebPortal.Core.ClassLib;

namespace NDAWebPortal.Areas.SystemAdministrator.Controllers
{
    public class ConfigurationController : BaseController
    {
        // GET: SystemAdministrator/Configuration
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult MakeCurrentSession(int sessionId)
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var newSession = data.Sessions.FirstOrDefault(x => !x.IsDeleted && x.Id == sessionId);
                    if (newSession == null)
                        return Json(
                            new {Status = false, Message = "This Session does not exist. Please select another"},
                            JsonRequestBehavior.AllowGet);

                    var oldSession = data.Sessions.FirstOrDefault(x => !x.IsDeleted && x.IsCurrentSession);
                    if (oldSession != null)
                    {
                        oldSession.IsCurrentSession = false;
                    }

                    newSession.IsCurrentSession = true;
                    data.SaveChanges();

                    return Json(
                        new { Status = true, Message = "Successful Modification" },
                        JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return Json(new {Status = false, Message = ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult AddSession(string sessionName)
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var session = data.Sessions.FirstOrDefault(x => x.SessionName == sessionName);
                    if (session != null)
                        return
                            Json(
                                new
                                {
                                    Status = false,
                                    Message = "This Session has been added before now. Please check and Verify"
                                },
                                JsonRequestBehavior.AllowGet);

                    data.Sessions.Add(new Session()
                    {
                        IsCurrentSession = false,
                        IsDeleted = false,
                        SessionName = sessionName.ToUpper()
                    });
                    data.SaveChanges();

                    return Json(new {Status = true, Message = "Successful Configuration"}, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return Json(new {Status = false, Message = ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetSessions()
        {
            using (var data = new NDAPortalDatabaseEntities())
            {
                return Json(new { Status = true, Data = data.Sessions.Where(x => !x.IsDeleted).ToList(), Message = "" },
                    JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult AddProgramme(string programmeName, string programmeCode)
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var programme =
                        data.Programmes.FirstOrDefault(
                            x => (x.ProgrammeCode == programmeCode || x.ProgrammeName == programmeName) && !x.IsDeleted);

                    if (programme != null)
                        return
                            Json(
                                new
                                {
                                    Status = false,
                                    Message = "This Programme has been added before now. Please check and Verify"
                                },
                                JsonRequestBehavior.AllowGet);

                    data.Programmes.Add(new Programme()
                    {
                        IsDeleted = false,
                        ProgrammeCode = programmeCode.ToUpper(),
                        ProgrammeName = programmeName,
                    });
                    data.SaveChanges();

                    return Json(new { Status = true, Message = "Successful Configuration" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RemoveProgramme(string programmeCode, int programeId)
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var programme =
                        data.Programmes.FirstOrDefault(
                            x => x.ProgrammeCode == programmeCode && x.Id == programeId && !x.IsDeleted);

                    if (programme == null)
                        return
                            Json(
                                new
                                {
                                    Status = false,
                                    Message = "This Programme does not Exist. Please check and Verify"
                                },
                                JsonRequestBehavior.AllowGet);

                    if (data.SubProgrammes.Any(x => x.ProgrammeId == programme.Id && !x.IsDeleted))
                        return
                            Json(
                                new
                                {
                                    Status = false,
                                    Message = $"Some Sub Programmes are still linked to {programme.ProgrammeName}. Please treat them first."
                                },
                                JsonRequestBehavior.AllowGet);

                    programme.IsDeleted = true;
                    data.Entry(programme).State = EntityState.Modified;
                    data.SaveChanges();

                    return Json(new { Status = true, Message = "Successful Configuration" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetProgrammes()
        {
            using (var data = new NDAPortalDatabaseEntities())
            {
                return Json(new { Status = true, Data = data.Programmes.Where(x => !x.IsDeleted).ToList(), Message = "" },
                    JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult AddSubProgramme(string subProgrammeName, string subProgrammeCode, int programmeId)
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var subProgramme =
                        data.SubProgrammes.FirstOrDefault(
                            x =>
                                (x.SubProgrammeCode == subProgrammeCode || x.SubProgrammeName == subProgrammeName) &&
                                x.ProgrammeId == programmeId && !x.IsDeleted);

                    if (subProgramme != null)
                        return
                            Json(
                                new
                                {
                                    Status = false,
                                    Message = "This Sub Programme has been added before now. Please check and Verify"
                                },
                                JsonRequestBehavior.AllowGet);

                    data.SubProgrammes.Add(new SubProgramme()
                    {
                        IsDeleted = false,
                        SubProgrammeCode = subProgrammeCode.ToUpper(),
                        SubProgrammeName = subProgrammeName,
                        ProgrammeId = programmeId
                    });
                    data.SaveChanges();

                    return Json(new { Status = true, Message = "Successful Configuration" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RemoveSubProgramme(string subProgrammeCode, int subProgrameId)
        {
            try
            {
                using (var data = new NDAPortalDatabaseEntities())
                {
                    var subProgramme =
                        data.SubProgrammes.FirstOrDefault(
                            x => x.SubProgrammeCode == subProgrammeCode && x.Id == subProgrameId);

                    if (subProgramme == null)
                        return
                            Json(
                                new
                                {
                                    Status = false,
                                    Message = "This Sub Programme does not Exist. Please check and verify"
                                },
                                JsonRequestBehavior.AllowGet);

                    subProgramme.IsDeleted = true;
                    data.Entry(subProgramme).State = EntityState.Modified;
                    data.SaveChanges();

                    return Json(new { Status = true, Message = "Successful Configuration" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetSubProgrammes(int programmeId = 0)
        {
            using (var data = new NDAPortalDatabaseEntities())
            {
                var programmes = data.Programmes.Where(x => !x.IsDeleted).ToList();
                if (programmeId == 0)
                    return
                        Json(
                            new
                            {
                                Status = true,
                                Data =
                                    data.SubProgrammes.Where(x => !x.IsDeleted)
                                        .ToList()
                                        .Select(
                                            x =>
                                                new
                                                {
                                                    x,
                                                    ProgrammeName =
                                                        programmes.FirstOrDefault(y => y.Id == x.ProgrammeId)?
                                                            .ProgrammeName ?? ""
                                                }),
                                Message = ""
                            },
                            JsonRequestBehavior.AllowGet);
                else
                    return
                        Json(
                            new
                            {
                                Status = true,
                                Data =
                                    data.SubProgrammes.Where(x => !x.IsDeleted && x.ProgrammeId == programmeId)
                                        .ToList()
                                        .Select(
                                            x =>
                                                new
                                                {
                                                    x,
                                                    ProgrammeName =
                                                        programmes.FirstOrDefault(y => y.Id == x.ProgrammeId)?
                                                            .ProgrammeName ?? ""
                                                }),
                                Message = ""
                            },
                            JsonRequestBehavior.AllowGet);
            }
        }
    }
}