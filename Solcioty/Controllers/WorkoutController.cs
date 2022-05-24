using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Models;
using BusinessLayer;
using web;
using Services;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Web;
using System.Text.RegularExpressions;

namespace Solcioty.Controllers
{
    [SessionTimeoutFilterAttribute]
    public class WorkoutController : BaseController
    {
        private BALWorkout _balWorkout;
        private BALTemplate _balTemplate;
        private BALImage _balImage;
        private BalGym _balGym;
        private BalCategory _balCategory;
        private BalWorkoutCategory _balWorkoutCategory;
        public WorkoutController()
        {
            _balWorkout = new BALWorkout();
            _balTemplate = new BALTemplate();
            _balGym = new BalGym();
            _balImage = new BALImage();
            _balCategory = new BalCategory();
            _balWorkoutCategory = new BalWorkoutCategory();
        }
        // GET: Workout
        [HasPermissionFilter(PermissionCode = Permission.Workouts)]
        [HttpGet]

        #region Normal Workout


        public ActionResult Index(int? categoryId)
        {
            //get list of workout masters
            ModelWorkout objModel = new ModelWorkout();
            var userData = Session["UserData"] as UserData;
            if (userData != null)
            {
                ViewBag.UserId = userData.UserId;
                ViewBag.ClientId = userData.ClientId;
                if (categoryId != null)
                    ViewBag.Category = new BalWorkoutCategory().GetByID(categoryId.Value);

                if (userData.RoleCode == RoleTypes.SuperAdmin)
                {
                    objModel.ModelWorkoutList = _balWorkout.GetAllWorkouts(userData.UserId, categoryId, true);
                    objModel.Categories = _balWorkoutCategory.GetAllCategory(userData.UserId, true).Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }).ToList();
                }
                else if (userData.RoleCode == RoleTypes.ClientAdmin)
                {
                    objModel.ModelWorkoutList = _balWorkout.GetAllWorkouts(userData.ClientOwnerId, categoryId, false);
                    objModel.Categories = _balWorkoutCategory.GetAllCategory(userData.UserId, false).Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }).ToList();

                }
                else if (userData.RoleCode == RoleTypes.BranchAdmin)
                {
                    var result = _balWorkout.GetScheduledWorkouts(userData, Convert.ToInt32(userData.BranchId));

                    var workouts = result.Select(e => new { WorkoutId = e.WorkoutId, WorkoutName = e.WorkoutName }).Distinct().ToList();
                    objModel.ModelWorkoutList = workouts.Select(e => new ModelWorkout()
                    {
                        ID = e.WorkoutId,
                        WorkoutName = e.WorkoutName
                    }).ToList();
                    objModel.Categories = _balWorkoutCategory.GetAllCategory(userData.ClientOwnerId, false).Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }).ToList();
                }

            }
            return View(objModel);
        }


        [HasPermissionFilter(PermissionCode = Permission.Workouts)]
        [HttpGet]
        public ActionResult Add()
        {
            ModelWorkout objModel = new ModelWorkout();
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Workouts);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            BindDropdown(objModel);
            return View(objModel);
        }

        public void BindDropdown(ModelWorkout model)
        {
            var userData = Session["UserData"] as UserData;
            IList<ModelImage> imageList = new List<ModelImage>();
            if (userData != null)
            {
                if (userData.RoleCode == RoleTypes.SuperAdmin)
                {
                    imageList = _balImage.GetAllImages(userData.UserId, true);
                    model.Categories = _balWorkoutCategory.GetAllCategory(userData.UserId, true).Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }).ToList();
                }
                else
                {
                    imageList = _balImage.GetAllImages(userData.ClientOwnerId, false);
                    model.Categories = _balWorkoutCategory.GetAllCategory(userData.UserId, false).Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }).ToList();
                }
            }
            if (imageList != null && imageList.Count > 0)
            {
                model.ImageList = imageList.Select(e => new SelectListItem()
                {
                    Text = e.ImageName,
                    Value = e.Id.ToString()
                }).ToList();
            }


        }

        [HasPermissionFilter(PermissionCode = Permission.Workouts)]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(ModelWorkout objmodel)
        {
            if (ModelState.IsValid)
            {
                var userData = Session["UserData"] as UserData;
                var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Workouts);
                if (permissionType == Models.Enums.PermissionType.Read)
                {
                    return RedirectToAction("Unauthorized", "Account");
                }
                BindDropdown(objmodel);
                if (objmodel.WorkoutPDF != null && objmodel.WorkoutPDF.FileName != "" && objmodel.WorkoutPDF.ContentType == "application/pdf")
                {
                    HttpPostedFileBase file = objmodel.WorkoutPDF;
                    var buffer = new byte[file.ContentLength];
                    int bytesReaded = file.InputStream.Read(buffer, 0, objmodel.WorkoutPDF.ContentLength);
                    if (bytesReaded > 0)
                    {
                        objmodel.PDF = buffer;
                        objmodel.PDFServerPath = Server.MapPath(System.Web.Configuration.WebConfigurationManager.AppSettings["AllPDFs"].ToString().Trim());
                        //objModel.VideoSize = objModel.Videoclip.ContentLength;
                        //objModel.ContentType = objModel.Videoclip.ContentType;
                        var pdfName = Regex.Replace(objmodel.WorkoutPDF.FileName, @"[^.0-9a-zA-Z]+", " ");
                        //objModel.FullName = objModel.Videoclip.FileName.Replace("-", "").Replace("+", "").Substring(objModel.Videoclip.FileName.LastIndexOf("\\")+1);
                        objmodel.PDFName = pdfName;
                    }
                }
                var result = _balWorkout.AddEdit(objmodel, userData.ClientOwnerId);
                switch (result)
                {
                    case CommonResultsMessage.Add:
                        TempData["OperationType"] = "success";
                        TempData["OperationMessage"] = ErrorMessage.WorkoutAdded;
                        return RedirectToAction("Index");
                    case CommonResultsMessage.Update:
                        TempData["OperationType"] = "success";
                        TempData["OperationMessage"] = ErrorMessage.WorkoutUpdated;
                        return RedirectToAction("Index");
                    case CommonResultsMessage.Fail:
                        TempData["OperationType"] = "fail";
                        TempData["OperationMessage"] = ErrorMessage.WorkoutDeleted;
                        break;
                    case CommonResultsMessage.duplicate:
                        TempData["OperationType"] = "fail";
                        TempData["OperationMessage"] = ErrorMessage.DuplicateWorkout;
                        break;
                    default:
                        break;
                }
            }
            return View(objmodel);
        }


        [HasPermissionFilter(PermissionCode = Permission.Workouts)]
        [HttpGet]
        public ActionResult ViewTemplate(int ID, int workoutId, bool isRead, int? templateMapId)
        {
            var templateDetail = _balWorkout.GetTemplateDetail(workoutId, ID, isRead, templateMapId);
            return PartialView("_ViewTemplate", templateDetail);
        }


        //[HttpPost]
        //[ValidateInput(false)]
        //public ActionResult AddWorkoutTemplate(ModelWorkout objmodel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = _balWorkout.AddEdit(objmodel);
        //        switch (result)
        //        {
        //            case CommonResultsMessage.Add:
        //                ViewBag.OperationType = "success";
        //                ViewBag.OperationMessage = "Workout added.";
        //                return RedirectToAction("Index");
        //            case CommonResultsMessage.Update:
        //                ViewBag.OperationType = "success";
        //                ViewBag.OperationMessage = "Workout updated.";
        //                return RedirectToAction("Index");
        //            case CommonResultsMessage.Fail:
        //                ViewBag.OperationType = "fail";
        //                ViewBag.OperationMessage = "Please try again or later.";
        //                break;
        //            case CommonResultsMessage.duplicate:
        //                ViewBag.OperationType = "fail";
        //                ViewBag.OperationMessage = "Please try again with different workout name.";
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //    return View(objmodel);
        //}

        [HasPermissionFilter(PermissionCode = Permission.Workouts)]
        [HttpPost]
        public JsonResult MapTemplate(WorkoutTemplateMapRequest request)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Workouts);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false }, JsonRequestBehavior.AllowGet);
            }
            var mapId = _balWorkout.MapWorkoutTemplate(request);
            return Json(new { IsSuccess = mapId > 0 ? true : false, Id = mapId });
        }


        [HasPermissionFilter(PermissionCode = Permission.Workouts)]
        [HttpPost]
        public JsonResult SetDisplayOrder(List<WorkoutTemplateDisplayOrder> request)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Workouts);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false }, JsonRequestBehavior.AllowGet);
            }
            var isSuccess = _balWorkout.SetWorkoutTemplateDisplayOrder(request);
            return Json(new { IsSuccess = isSuccess });
        }

        [HasPermissionFilter(PermissionCode = Permission.Workouts)]
        [HttpPost]
        public JsonResult DeleteTemplate(WorkoutTemplateMapRequest request)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Workouts);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false }, JsonRequestBehavior.AllowGet);
            }
            var isDeleted = _balWorkout.DeleteWorkoutTemplate(request);
            return Json(new { IsSuccess = isDeleted });
        }


        [HasPermissionFilter(PermissionCode = Permission.Workouts)]
        [HttpGet]
        public ActionResult Edit(Int32 ID)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Workouts);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            ModelWorkout objmodel = new ModelWorkout();
            objmodel = _balWorkout.GetByID(ID);
            BindDropdown(objmodel);
            return View("Add", objmodel);
        }

        [HasPermissionFilter(PermissionCode = Permission.Workouts)]
        [HttpGet]
        public ActionResult Delete(Int32 ID)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Workouts);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            var result = new JsonResult();
            var deleteresult = _balWorkout.Delete(ID);
            switch (deleteresult)
            {
                case CommonResultsMessage.Update:
                    result.Data = true;
                    TempData["OperationType"] = "success";
                    TempData["OperationMessage"] = ErrorMessage.WorkoutDeleted;
                    break;
                case CommonResultsMessage.Fail:
                    TempData["OperationType"] = "fail";
                    TempData["OperationMessage"] = ErrorMessage.TryLater;
                    result.Data = false;
                    break;
            }
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }


        [HttpPost]
        [HasPermissionFilter(PermissionCode = Permission.Workouts)]
        public ActionResult BulkDelete(List<int> Ids)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Workouts);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            //  var result = new JsonResult();
            var deleteresult = _balWorkout.BulkDelete(Ids);
            switch (deleteresult)
            {
                case CommonResultsMessage.Update:
                    // result.Data = true;
                    //TempData["OperationType"] = "success";
                    //TempData["OperationMessage"] = ErrorMessage.WorkoutDeleted;

                    return Json(new { status = true, message = ErrorMessage.WorkoutDeleted });

                case CommonResultsMessage.Fail:
                    //result.Data = false;
                    //TempData["OperationType"] = "fail";
                    //TempData["OperationMessage"] = ErrorMessage.TryLater;

                    return Json(new { status = false, message = ErrorMessage.TryLater });

            }
            // result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return Json(new { statu = "fail", message = "something went wrong" });
        }


        [HasPermissionFilter(PermissionCode = Permission.Workouts)]
        [HttpGet]
        public ActionResult Ready(Int32 ID, string Ready)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Workouts);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            var result = new JsonResult();
            var deleteresult = _balWorkout.WorkoutIsReadyForLocations(ID, !(Convert.ToBoolean(Ready)));
            switch (deleteresult)
            {
                case CommonResultsMessage.Update:
                    result.Data = !(Convert.ToBoolean(Ready));
                    break;
                case CommonResultsMessage.Fail:
                    result.Data = Convert.ToBoolean(Ready);
                    break;
            }
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
            //_balWorkout.WorkoutIsReadyForLocations(ID, !(Convert.ToBoolean(Ready)));
            //return RedirectToAction("Index");
        }


        //workout template Mapping
        [HasPermissionFilter(PermissionCode = Permission.Workouts)]
        [HttpGet]
        public ActionResult Templates(Int32 ID)
        {
            //ModelWorkout objmodel = new ModelWorkout();
            //objmodel =_balWorkout.GetByID(ID);
            //objmodel.TemplateTable = _balTemplate.GetAllTemplates(true);
            //return View(objmodel);

            var workoutDetail = new WorkoutDetailModel();

            workoutDetail = _balWorkout.GetWorkoutTemplateDetail(ID);
            var userData = Session["UserData"] as UserData;
            if (userData != null)
            {
                if (userData.RoleCode == RoleTypes.SuperAdmin)
                {
                    workoutDetail.TemplateList = _balTemplate.GetAllTemplates(userData.UserId, ForAdmin: true).Where(e => e.IsReadyForLocations == true).ToList();
                }
                else
                {
                    workoutDetail.TemplateList = _balTemplate.GetAllTemplates(userData.ClientOwnerId, ForAdmin: false).Where(e => e.IsReadyForLocations == true).ToList();
                }
            }

            return View(workoutDetail);
        }

        [HasPermissionFilter(PermissionCode = Permission.Workouts_Schedule)]
        [HttpGet]
        public ActionResult WorkoutSchedule()
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Workouts_Schedule);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            return View();
        }

        [HasPermissionFilter(PermissionCode = Permission.Workouts_Schedule)]
        [HttpGet]
        public ActionResult WorkoutSelection()
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Workouts_Schedule);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            return View();
        }

        [HasPermissionFilter(PermissionCode = Permission.Workouts)]
        [HttpGet]
        public JsonResult GetAllWorkout()
        {
            IList<ModelWorkout> workoutList = new List<ModelWorkout>();
            var userData = Session["UserData"] as UserData;
            if (userData != null)
            {
                if (userData.RoleCode == RoleTypes.SuperAdmin)
                {
                    workoutList = _balWorkout.GetAllWorkouts(userData.UserId, null, true);
                }
                else
                {
                    workoutList = _balWorkout.GetAllWorkouts(userData.ClientOwnerId, null, false);
                }
                return Json(new { data = workoutList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Message = ErrorMessage.InvalidRequest }, JsonRequestBehavior.AllowGet);
            }
        }


        [HasPermissionFilter(PermissionCode = Permission.Workouts)]
        [HttpGet]
        public JsonResult GetWorkoutPDF(int workoutId)
        {
            var workoutDetail = new ModelWorkout();
            var userData = Session["UserData"] as UserData;
            if (userData != null)
            {

                workoutDetail = _balWorkout.GetByID(workoutId);
                return Json(new { Workout = workoutDetail }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Message = ErrorMessage.InvalidRequest }, JsonRequestBehavior.AllowGet);
            }
        }

        [HasPermissionFilter(PermissionCode = Permission.Workouts)]
        public ActionResult ViewWorkout(int WorkoutId)
        {
            var userData = Session["UserData"] as UserData;
            var homePageData = new ModelHomePage();
            homePageData.ClientConfiguration = _balGym.GetClientConfiguration(Convert.ToInt32(userData.ClientId));
            var workoutDetailModel = new WorkoutDetailModel();
            workoutDetailModel = _balWorkout.GetWorkoutTemplateDetail(WorkoutId);
            homePageData.WorkoutInfo = workoutDetailModel;
            return PartialView("_ViewWorkout", homePageData);
        }


        [HasPermissionFilter(PermissionCode = Permission.Workouts)]
        public ActionResult ViewWorkoutPDF(int WorkoutId)
        {
            var userData = Session["UserData"] as UserData;
            var homePageData = new ModelHomePage();
            homePageData.ClientConfiguration = _balGym.GetClientConfiguration(Convert.ToInt32(userData.ClientId));
            var workoutDetailModel = new WorkoutDetailModel();
            workoutDetailModel = _balWorkout.GetWorkoutTemplateDetail(WorkoutId);
            homePageData.WorkoutInfo = workoutDetailModel;
            return PartialView("_ViewWorkoutPDF", homePageData);
        }


        [HttpPost, ValidateInput(false)]
        [HasPermissionFilter(PermissionCode = Permission.Workouts)]
        public FileResult PrintWorkout(string WorkoutHtml)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader sr = new StringReader(WorkoutHtml);
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                return File(stream.ToArray(), "application/pdf", "WorkoutDetail.pdf");
            }
        }


        [HasPermissionFilter(PermissionCode = Permission.Workouts_Schedule)]
        [HttpGet]
        public JsonResult GetScheduledWorkout(int LocationId)
        {
            var userData = Session["UserData"] as UserData;
            IList<ModelWorkoutSchedule> workoutScheduleList = new List<ModelWorkoutSchedule>();
            if (userData != null)
            {
                workoutScheduleList = _balWorkout.GetScheduledWorkouts(userData, LocationId);
            }
            if (userData.RoleCode == RoleTypes.BranchAdmin)
            {
                workoutScheduleList = workoutScheduleList.Where(e => e.IsActive == true).ToList();
            }
            return Json(new { data = workoutScheduleList }, JsonRequestBehavior.AllowGet);
        }


        [HasPermissionFilter(PermissionCode = Permission.Workouts_Schedule)]
        [HttpPost]
        public JsonResult ScheduleWorkout(ModelWorkoutSchedule request)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Workouts_Schedule);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false }, JsonRequestBehavior.AllowGet);
            }
            if (request == null || userData == null)
            {
                return Json(new { IsSuccess = false, Message = ErrorMessage.InvalidRequest });
            }
            var result = _balWorkout.ScheduleWorkout(request);
            //if (result == 0)
            //{

            //    TempData["OperationType"] = "success";
            //    TempData["OperationMessage"] = ErrorMessage.WorkoutScheduled;
            //}
            return Json(new { data = result });
        }

        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Workouts_Schedule)]
        public ActionResult ScheduleWorkout(int WorkoutScheduleId, int LocationId)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Workouts_Schedule);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            var workoutSchedule = new ModelWorkoutSchedule();
            workoutSchedule = _balWorkout.GetScheduledWorkoutDetail(WorkoutScheduleId);
            if (workoutSchedule == null) workoutSchedule = new ModelWorkoutSchedule();
            if (userData.RoleCode == RoleTypes.SuperAdmin)
            {
                workoutSchedule.WorkoutList = _balWorkout.GetAllWorkouts(userData.UserId, null, true).Where(e => e.IsReadyForLocations).ToList();
            }
            else
            {
                workoutSchedule.WorkoutList = _balWorkout.GetAllWorkouts(userData.ClientOwnerId, null, false).Where(e => e.IsReadyForLocations).ToList();
            }

            if (workoutSchedule.WorkoutScheduleId == 0)
            {
                workoutSchedule.ScheduledDate = DateTime.UtcNow;
            }
            workoutSchedule.LocationId = LocationId;
            return PartialView("_ScheduleWorkout", workoutSchedule);
        }


        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Workouts_Schedule)]
        public ActionResult BulkScheduleWorkout()
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Workouts_Schedule);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            var workoutBulkSchedule = new ModelWorkoutBulkSchedule();
            //workoutBulkSchedule = _balWorkout.GetScheduledWorkoutDetail(WorkoutScheduleId);
            //if (workoutBulkSchedule == null) workoutBulkSchedule = new ModelWorkoutBulkSchedule();
            if (userData.RoleCode == RoleTypes.SuperAdmin)
            {
                workoutBulkSchedule.WorkoutList = _balWorkout.GetAllWorkouts(userData.UserId, null, true).Where(e => e.IsReadyForLocations).ToList();
            }
            else
            {
                workoutBulkSchedule.WorkoutList = _balWorkout.GetAllWorkouts(userData.ClientOwnerId, null, false).Where(e => e.IsReadyForLocations).ToList();
            }

            workoutBulkSchedule.ScheduledDate = DateTime.Now;
            var gymLocations = new List<GymLocationMapping>();
            gymLocations = _balGym.GetAllLocationByGym(Convert.ToInt32(userData.ClientId)).Where(e => e.IsActive == true).ToList();
            if (gymLocations != null && gymLocations.Count > 0)
            {
                workoutBulkSchedule.BulkScheduledWorkout = gymLocations.Select(e => new LocationWorkoutModel
                {
                    LocationId = e.ID,
                    IsActive = false,
                    IsSelected = false,
                    LocationName = e.LocationName,
                    WorkoutId = 0
                }).ToList();
            }
            return PartialView("_BulkScheduleWorkout", workoutBulkSchedule);
        }


        [HasPermissionFilter(PermissionCode = Permission.Workouts_Schedule)]
        [HttpPost]
        public JsonResult BulkScheduleWorkout(ModelWorkoutBulkSchedule request)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Workouts_Schedule);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false }, JsonRequestBehavior.AllowGet);
            }
            if (request == null || userData == null)
            {
                return Json(new { IsSuccess = false, Message = ErrorMessage.InvalidRequest });
            }
            if (request.BulkScheduledSelectedWorkout == null || request.BulkScheduledSelectedWorkout.Count == 0)
            {
                return Json(new { IsSuccess = false, Message = ErrorMessage.SelectLocation });
            }
            var result = _balWorkout.BulkScheduleWorkout(request);
            //if (result == 0)
            //{

            //    TempData["OperationType"] = "success";
            //    TempData["OperationMessage"] = ErrorMessage.WorkoutScheduled;
            //}
            return Json(new { data = result });
        }

        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Workouts_Schedule)]
        public JsonResult WorkoutSelect(int WorkoutScheduleId)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Workouts_Schedule);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false }, JsonRequestBehavior.AllowGet);
            }
            if (WorkoutScheduleId == 0)
            {
                return Json(new { IsSuccess = false, Message = ErrorMessage.InvalidRequest }, JsonRequestBehavior.AllowGet);
            }
            var result = _balWorkout.WorkoutSelect(WorkoutScheduleId, userData);
            return Json(new { IsSuccess = result, Message = ErrorMessage.WorkoutSelected }, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// Retrieve Client's All Users for workout
        /// </summary>
        /// <param name="RoleCode">Role Code</param>
        /// <returns></returns>
        [HasPermissionFilter(PermissionCode = Permission.Workouts)]
        public ActionResult WorkoutUsers(int Id)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Workouts);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }

            var allUsers = new List<ModelClientUser>();
            var clientId = userData.ClientId.Value;
            allUsers = _balGym.GetGymAllUsers(clientId);
            ViewBag.WorkoutDetail = _balWorkout.GetByID(Id);
            //ViewBag.PortalId = PortalId;
            return View(allUsers);
        }

        /// <summary>
        /// Retrieve Portal User Mapping List
        /// </summary>
        /// <param name="RoleCode">Role Code</param>
        /// <returns>Role Permissions</returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Workouts)]
        public JsonResult GetWorkoutUserMappings(int WorkoutId)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Workouts);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false }, JsonRequestBehavior.AllowGet);
            }
            var allUsers = new List<ModelClientUserForWorkout>();
            var clientId = userData.ClientId.Value;
            allUsers = _balGym.GetGymAllUsersForWorkout(clientId, WorkoutId);
            return Json(new { Data = allUsers }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Save Training Portal User Permission Mapping
        /// </summary>
        /// <param name="request">Role Permission Detail Model</param>
        /// <returns></returns>
        [HttpPost]
        [HasPermissionFilter(PermissionCode = Permission.Workouts)]
        public JsonResult SaveWorkoutUser(ModelWorkoutUserMapping request)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Workouts);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false });
            }
            if (request != null)
            {
                request.CreatedBy = userData.UserId;
                request.UpdatedBy = userData.UserId;
                var result = _balWorkout.SaveWorkoutUserMapping(request);
                return Json(new { IsSuccess = result });
            }
            return Json(new { IsSuccess = false });
        }


        [HttpPost]
        [HasPermissionFilter(PermissionCode = Permission.Workouts)]
        public ActionResult BulkMove(int CategoryId, List<int> Ids)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Workouts);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            var moveResult = _balWorkout.BulkMove(CategoryId, Ids);
            switch (moveResult)
            {
                case CommonResultsMessage.Update:
                    return Json(new { status = true, message = ErrorMessage.WorkoutMoved });
                case CommonResultsMessage.Fail:
                    return Json(new { status = false, message = ErrorMessage.TryLater });

            }
            return Json(new { statu = "fail", message = "something went wrong" });
        }


        #endregion

        #region Boxer Workout

        //[HasPermissionFilter(PermissionCode = Permission.Workouts)]
        //[HttpGet]
        //public ActionResult IndexBoxer(int? categoryId)
        //{
        //    //get list of workout masters
        //    ModelBoxerWorkout objModel = new ModelBoxerWorkout();
        //    var userData = Session["UserData"] as UserData;
        //    if (userData != null)
        //    {
        //        ViewBag.UserId = userData.UserId;
        //        ViewBag.ClientId = userData.ClientId;
        //        if (categoryId != null)
        //            ViewBag.Category = new BalWorkoutCategory().GetByID(categoryId.Value);

        //        if (userData.RoleCode == RoleTypes.SuperAdmin)
        //        {
        //            objModel.ModelWorkoutList = _balWorkout.GetAllWorkoutsBoxer(userData.UserId, categoryId, true);
        //            objModel.Categories = _balWorkoutCategory.GetAllCategory(userData.UserId, true).Select(x => new SelectListItem
        //            {
        //                Text = x.Name,
        //                Value = x.Id.ToString()
        //            }).ToList();
        //        }
        //        else if (userData.RoleCode == RoleTypes.ClientAdmin)
        //        {
        //            objModel.ModelWorkoutList = _balWorkout.GetAllWorkoutsBoxer(userData.ClientOwnerId, categoryId, false);
        //            objModel.Categories = _balWorkoutCategory.GetAllCategory(userData.UserId, false).Select(x => new SelectListItem
        //            {
        //                Text = x.Name,
        //                Value = x.Id.ToString()
        //            }).ToList();

        //        }
        //        else if (userData.RoleCode == RoleTypes.BranchAdmin)
        //        {
        //            var result = _balWorkout.GetScheduledWorkouts(userData, Convert.ToInt32(userData.BranchId));

        //            var workouts = result.Select(e => new { WorkoutId = e.WorkoutId, WorkoutName = e.WorkoutName }).Distinct().ToList();
        //            objModel.ModelWorkoutList = workouts.Select(e => new ModelBoxerWorkout()
        //            {
        //                ID = e.WorkoutId,
        //                WorkoutName = e.WorkoutName
        //            }).ToList();
        //            objModel.Categories = _balWorkoutCategory.GetAllCategory(userData.ClientOwnerId, false).Select(x => new SelectListItem
        //            {
        //                Text = x.Name,
        //                Value = x.Id.ToString()
        //            }).ToList();
        //        }

        //    }
        //    return View(objModel);
        //}


        //[HasPermissionFilter(PermissionCode = Permission.Workouts)]
        //[HttpGet]
        //public ActionResult Add()
        //{
        //    ModelWorkout objModel = new ModelWorkout();
        //    var userData = Session["UserData"] as UserData;
        //    var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Workouts);
        //    if (permissionType == Models.Enums.PermissionType.Read)
        //    {
        //        return RedirectToAction("Unauthorized", "Account");
        //    }
        //    BindDropdown(objModel);
        //    return View(objModel);
        //}


        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts)]
        [HttpGet]
        public ActionResult IndexBoxer(int? categoryId)
        {
            //get list of workout masters
            ModelBoxerWorkout objModel = new ModelBoxerWorkout();
            var userData = Session["UserData"] as UserData;
            if (userData != null)
            {
                ViewBag.UserId = userData.UserId;
                ViewBag.ClientId = userData.ClientId;
                if (categoryId != null)
                    ViewBag.Category = new BalWorkoutCategory().GetByID(categoryId.Value);

                if (userData.RoleCode == RoleTypes.SuperAdmin)
                {
                    objModel.ModelWorkoutList = _balWorkout.GetAllWorkoutsBoxer(userData.UserId, categoryId, true);
                    objModel.Categories = _balWorkoutCategory.GetAllCategory(userData.UserId, true).Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }).ToList();
                }
                else if (userData.RoleCode == RoleTypes.ClientAdmin)
                {
                    objModel.ModelWorkoutList = _balWorkout.GetAllWorkoutsBoxer(userData.ClientOwnerId, categoryId, false);
                    objModel.Categories = _balWorkoutCategory.GetAllCategory(userData.UserId, false).Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }).ToList();

                }
                else if (userData.RoleCode == RoleTypes.BranchAdmin)
                {
                    var result = _balWorkout.GetScheduledWorkouts(userData, Convert.ToInt32(userData.BranchId));

                    var workouts = result.Select(e => new { WorkoutId = e.WorkoutId, WorkoutName = e.WorkoutName }).Distinct().ToList();
                    objModel.ModelWorkoutList = workouts.Select(e => new ModelBoxerWorkout()
                    {
                        ID = e.WorkoutId,
                        WorkoutName = e.WorkoutName
                    }).ToList();
                    objModel.Categories = _balWorkoutCategory.GetAllCategory(userData.ClientOwnerId, false).Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }).ToList();
                }

            }
            return View(objModel);
        }

        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts)]
        [HttpGet]
        public ActionResult AddBoxer()
        {
            ModelBoxerWorkout objModel = new ModelBoxerWorkout();
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.BoxerWorkouts);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            BindBoxerDropdown(objModel);
            return View(objModel);
        }

        public void BindBoxerDropdown(ModelBoxerWorkout model)
        {
            var userData = Session["UserData"] as UserData;
            IList<ModelImage> imageList = new List<ModelImage>();
            if (userData != null)
            {
                if (userData.RoleCode == RoleTypes.SuperAdmin)
                {
                    imageList = _balImage.GetAllImages(userData.UserId, true);
                    model.Categories = _balWorkoutCategory.GetAllCategory(userData.UserId, true).Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }).ToList();
                }
                else
                {
                    imageList = _balImage.GetAllImages(userData.ClientOwnerId, false);
                    model.Categories = _balWorkoutCategory.GetAllBoxerCategory(userData.UserId, false).Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }).ToList();
                }
            }
            if (imageList != null && imageList.Count > 0)
            {
                model.ImageList = imageList.Select(e => new SelectListItem()
                {
                    Text = e.ImageName,
                    Value = e.Id.ToString()
                }).ToList();
            }


        }

        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts)]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddBoxer(ModelBoxerWorkout objmodel)
        {
            if (ModelState.IsValid)
            {
                var userData = Session["UserData"] as UserData;
                var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.BoxerWorkouts);
                if (permissionType == Models.Enums.PermissionType.Read)
                {
                    return RedirectToAction("Unauthorized", "Account");
                }
                BindBoxerDropdown(objmodel);
                if (objmodel.WorkoutPDF != null && objmodel.WorkoutPDF.FileName != "" /*&& objmodel.WorkoutPDF.ContentType == "application/pdf"*/)
                {
                    HttpPostedFileBase file = objmodel.WorkoutPDF;
                    var buffer = new byte[file.ContentLength];
                    int bytesReaded = file.InputStream.Read(buffer, 0, objmodel.WorkoutPDF.ContentLength);
                    if (bytesReaded > 0)
                    {
                        objmodel.PDF = buffer;
                        if (objmodel.WorkoutPDF.FileName.Contains(".pdf"))
                        {
                            objmodel.PDFServerPath = Server.MapPath(System.Web.Configuration.WebConfigurationManager.AppSettings["AllPDFs"].ToString().Trim());
                        }
                        else if (objmodel.WorkoutPDF.FileName.Contains(".jpeg") || objmodel.WorkoutPDF.FileName.Contains(".png") || objmodel.WorkoutPDF.FileName.Contains(".jpg") || objmodel.WorkoutPDF.FileName.Contains(".gif"))
                        {
                            objmodel.PDFServerPath = Server.MapPath(System.Web.Configuration.WebConfigurationManager.AppSettings["AllImages"].ToString().Trim());
                        }
                        else if (objmodel.WorkoutPDF.FileName.Contains(".mp4"))
                        {
                            objmodel.PDFServerPath = Server.MapPath(System.Web.Configuration.WebConfigurationManager.AppSettings["AllVideos"].ToString().Trim());
                        }
                        else if (objmodel.WorkoutPDF.FileName.Contains(".txt"))
                        {
                            objmodel.PDFServerPath = Server.MapPath(System.Web.Configuration.WebConfigurationManager.AppSettings["AllTextes"].ToString().Trim());
                        }
                        //objModel.VideoSize = objModel.Videoclip.ContentLength;
                        //objModel.ContentType = objModel.Videoclip.ContentType;
                        var pdfName = Regex.Replace(objmodel.WorkoutPDF.FileName, @"[^.0-9a-zA-Z]+", " ");
                        //objModel.FullName = objModel.Videoclip.FileName.Replace("-", "").Replace("+", "").Substring(objModel.Videoclip.FileName.LastIndexOf("\\")+1);
                        objmodel.PDFName = pdfName;
                    }
                }
                var result = _balWorkout.AddEditBoxer(objmodel, userData.ClientOwnerId);
                switch (result)
                {
                    case CommonResultsMessage.Add:
                        TempData["OperationType"] = "success";
                        TempData["OperationMessage"] = ErrorMessage.BoxerWorkoutAdded;
                        return RedirectToAction("IndexBoxer");
                    case CommonResultsMessage.Update:
                        TempData["OperationType"] = "success";
                        TempData["OperationMessage"] = ErrorMessage.BoxerWorkoutUpdated;
                        return RedirectToAction("IndexBoxer");
                    case CommonResultsMessage.Fail:
                        TempData["OperationType"] = "fail";
                        TempData["OperationMessage"] = ErrorMessage.BoxerWorkoutDeleted;
                        break;
                    case CommonResultsMessage.duplicate:
                        TempData["OperationType"] = "fail";
                        TempData["OperationMessage"] = ErrorMessage.BoxerDuplicateWorkout;
                        break;
                    default:
                        break;
                }
            }
            return View(objmodel);
        }

        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts)]
        [HttpGet]
        public ActionResult ViewBoxerTemplate(int ID, int boxerWorkoutId, bool isRead, int? boxerWorkoutTemplateMapId)
        {
            var templateDetail = _balWorkout.GetBoxerTemplateDetail(boxerWorkoutId, ID, isRead, boxerWorkoutTemplateMapId);
            return PartialView("_ViewBoxerTemplate", templateDetail);
        }

        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts)]
        [HttpPost]
        public JsonResult MapBoxerTemplate(BoxerWorkoutTemplateMapRequest request)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.BoxerWorkouts);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false }, JsonRequestBehavior.AllowGet);
            }
            var mapId = _balWorkout.MapBoxerWorkoutTemplate(request);
            return Json(new { IsSuccess = mapId > 0 ? true : false, Id = mapId });
        }


        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts)]
        [HttpPost]
        public JsonResult SetBoxerDisplayOrder(List<BoxerWorkoutTemplateDisplayOrder> request)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.BoxerWorkouts);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false }, JsonRequestBehavior.AllowGet);
            }
            var isSuccess = _balWorkout.SetBoxerWorkoutTemplateDisplayOrder(request);
            return Json(new { IsSuccess = isSuccess });
        }

        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts)]
        [HttpPost]
        public JsonResult DeleteBoxerTemplate(BoxerWorkoutTemplateMapRequest request)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.BoxerWorkouts);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false }, JsonRequestBehavior.AllowGet);
            }
            var isDeleted = _balWorkout.DeleteBoxerWorkoutTemplate(request);
            return Json(new { IsSuccess = isDeleted });
        }


        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts)]
        [HttpGet]
        public ActionResult EditBoxer(Int32 ID)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.BoxerWorkouts);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            ModelBoxerWorkout objmodel = new ModelBoxerWorkout();
            objmodel = _balWorkout.GetByIDBoxer(ID);
            BindBoxerDropdown(objmodel);
            return View("AddBoxer", objmodel);
        }

        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts)]
        [HttpGet]
        public ActionResult DeleteBoxer(Int32 ID)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.BoxerWorkouts);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            var result = new JsonResult();
            var deleteresult = _balWorkout.DeleteBoxer(ID);
            switch (deleteresult)
            {
                case CommonResultsMessage.Update:
                    result.Data = true;
                    TempData["OperationType"] = "success";
                    TempData["OperationMessage"] = ErrorMessage.BoxerWorkoutDeleted;
                    break;
                case CommonResultsMessage.Fail:
                    TempData["OperationType"] = "fail";
                    TempData["OperationMessage"] = ErrorMessage.TryLater;
                    result.Data = false;
                    break;
            }
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }


        [HttpPost]
        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts)]
        public ActionResult BulkDeleteBoxer(List<int> Ids)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.BoxerWorkouts);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            //  var result = new JsonResult();
            var deleteresult = _balWorkout.BulkDeleteBoxer(Ids);
            switch (deleteresult)
            {
                case CommonResultsMessage.Update:
                    // result.Data = true;
                    //TempData["OperationType"] = "success";
                    //TempData["OperationMessage"] = ErrorMessage.WorkoutDeleted;

                    return Json(new { status = true, message = ErrorMessage.BoxerWorkoutDeleted });

                case CommonResultsMessage.Fail:
                    //result.Data = false;
                    //TempData["OperationType"] = "fail";
                    //TempData["OperationMessage"] = ErrorMessage.TryLater;

                    return Json(new { status = false, message = ErrorMessage.TryLater });

            }
            // result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return Json(new { statu = "fail", message = "something went wrong" });
        }


        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts)]
        [HttpGet]
        public ActionResult ReadyBoxer(Int32 ID, string Ready)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.BoxerWorkouts);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            var result = new JsonResult();
            var deleteresult = _balWorkout.WorkoutIsReadyForLocationsBoxer(ID, !(Convert.ToBoolean(Ready)));
            switch (deleteresult)
            {
                case CommonResultsMessage.Update:
                    result.Data = !(Convert.ToBoolean(Ready));
                    break;
                case CommonResultsMessage.Fail:
                    result.Data = Convert.ToBoolean(Ready);
                    break;
            }
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
            //_balWorkout.WorkoutIsReadyForLocations(ID, !(Convert.ToBoolean(Ready)));
            //return RedirectToAction("Index");
        }


        //workout template Mapping
        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts)]
        [HttpGet]
        public ActionResult BoxerTemplates(Int32 ID)
        {
            //ModelWorkout objmodel = new ModelWorkout();
            //objmodel =_balWorkout.GetByID(ID);
            //objmodel.TemplateTable = _balTemplate.GetAllTemplates(true);
            //return View(objmodel);

            var workoutDetail = new BoxerWorkoutDetailModel();

            workoutDetail = _balWorkout.GetWorkoutTemplateDetailBoxer(ID);
            var userData = Session["UserData"] as UserData;
            if (userData != null)
            {
                if (userData.RoleCode == RoleTypes.SuperAdmin)
                {
                    workoutDetail.BoxerTemplateList = _balTemplate.GetBoxerAllTemplates(userData.UserId, ForAdmin: true).Where(e => e.IsReadyForLocations == true).ToList();
                }
                else
                {
                    workoutDetail.BoxerTemplateList = _balTemplate.GetBoxerAllTemplates(userData.ClientOwnerId, ForAdmin: false).Where(e => e.IsReadyForLocations == true).ToList();
                }
            }

            return View(workoutDetail);
        }

        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts_Schedule)]
        [HttpGet]
        public ActionResult BoxerWorkoutSchedule()
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.BoxerWorkouts_Schedule);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            return View();
        }

        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts_Schedule)]
        [HttpGet]
        public ActionResult BoxerWorkoutSelection()
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.BoxerWorkouts_Schedule);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            return View();
        }

        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts)]
        [HttpGet]
        public JsonResult GetAllBoxerWorkout()
        {
            IList<ModelBoxerWorkout> workoutList = new List<ModelBoxerWorkout>();
            var userData = Session["UserData"] as UserData;
            if (userData != null)
            {
                if (userData.RoleCode == RoleTypes.SuperAdmin)
                {
                    workoutList = _balWorkout.GetAllWorkoutsBoxer(userData.UserId, null, true);
                }
                else
                {
                    workoutList = _balWorkout.GetAllWorkoutsBoxer(userData.ClientOwnerId, null, false);
                }
                return Json(new { data = workoutList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Message = ErrorMessage.InvalidRequest }, JsonRequestBehavior.AllowGet);
            }
        }


        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts)]
        [HttpGet]
        public JsonResult GetBoxerWorkoutPDF(int boxerWorkoutId)
        {
            var workoutDetail = new ModelBoxerWorkout();
            var userData = Session["UserData"] as UserData;
            if (userData != null)
            {
                workoutDetail = _balWorkout.GetByIDBoxer(boxerWorkoutId);
                return Json(new { BoxerWorkout = workoutDetail }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Message = ErrorMessage.InvalidRequest }, JsonRequestBehavior.AllowGet);
            }
        }

        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts)]
        public ActionResult ViewBoxerWorkout(int boxerWorkoutId)
        {
            var userData = Session["UserData"] as UserData;
            var homePageData = new ModelBoxerHomePage();
            homePageData.ClientConfiguration = _balGym.GetClientConfiguration(Convert.ToInt32(userData.ClientId));
            var boxerWorkoutDetailModel = new BoxerWorkoutDetailModel();
            boxerWorkoutDetailModel = _balWorkout.GetWorkoutTemplateDetailBoxer(boxerWorkoutId);
            homePageData.BoxerWorkoutInfo = boxerWorkoutDetailModel;
            return PartialView("_ViewBoxerWorkout", homePageData);
        }


        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts)]
        public ActionResult ViewBoxerWorkoutPDF(int BoxerWorkoutId)
        {
            var userData = Session["UserData"] as UserData;
            var homePageData = new ModelBoxerHomePage();
            homePageData.ClientConfiguration = _balGym.GetClientConfiguration(Convert.ToInt32(userData.ClientId));
            var workoutDetailModel = new BoxerWorkoutDetailModel();
            workoutDetailModel = _balWorkout.GetWorkoutTemplateDetailBoxer(BoxerWorkoutId);
            homePageData.BoxerWorkoutInfo = workoutDetailModel;
            return PartialView("_ViewBoxerWorkoutPDF", homePageData);
        }


        [HttpPost, ValidateInput(false)]
        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts)]
        public FileResult PrintBoxerWorkout(string BoxerWorkoutHtml)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader sr = new StringReader(BoxerWorkoutHtml);
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                return File(stream.ToArray(), "application/pdf", "BoxerWorkoutDetail.pdf");
            }
        }


        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts_Schedule)]
        [HttpGet]
        public JsonResult GetScheduledBoxerWorkout(int LocationId)
        {
            var userData = Session["UserData"] as UserData;
            IList<ModelBoxerWorkoutSchedule> boxerWorkoutScheduleList = new List<ModelBoxerWorkoutSchedule>();
            if (userData != null)
            {
                boxerWorkoutScheduleList = _balWorkout.GetBoxerScheduledWorkouts(userData, LocationId);
            }
            if (userData.RoleCode == RoleTypes.BranchAdmin)
            {
                boxerWorkoutScheduleList = boxerWorkoutScheduleList.Where(e => e.IsActive == true).ToList();
            }
            return Json(new { data = boxerWorkoutScheduleList }, JsonRequestBehavior.AllowGet);
        }


        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts_Schedule)]
        [HttpPost]
        public JsonResult BoxerWorkoutSchedule(ModelBoxerWorkoutSchedule request)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.BoxerWorkouts_Schedule);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false }, JsonRequestBehavior.AllowGet);
            }
            if (request == null || userData == null)
            {
                return Json(new { IsSuccess = false, Message = ErrorMessage.InvalidRequest });
            }
            var result = _balWorkout.ScheduleBoxerWorkout(request);
            //if (result == 0)
            //{

            //    TempData["OperationType"] = "success";
            //    TempData["OperationMessage"] = ErrorMessage.WorkoutScheduled;
            //}
            return Json(new { data = result });
        }

        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts_Schedule)]
        public ActionResult ScheduleBoxerWorkout(int WorkoutScheduleId, int LocationId)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.BoxerWorkouts_Schedule);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            var boxerWorkoutSchedule = new ModelBoxerWorkoutSchedule();
            boxerWorkoutSchedule = _balWorkout.GetScheduledBoxerWorkoutDetail(WorkoutScheduleId);
            if (boxerWorkoutSchedule == null) boxerWorkoutSchedule = new ModelBoxerWorkoutSchedule();
            if (userData.RoleCode == RoleTypes.SuperAdmin)
            {
                boxerWorkoutSchedule.BoxerWorkoutList = _balWorkout.GetAllWorkoutsBoxer(userData.UserId, null, true).Where(e => e.IsReadyForLocations).ToList();
            }
            else
            {
                boxerWorkoutSchedule.BoxerWorkoutList = _balWorkout.GetAllWorkoutsBoxer(userData.ClientOwnerId, null, false).Where(e => e.IsReadyForLocations).ToList();
            }

            if (boxerWorkoutSchedule.BoxerWorkoutScheduleId == 0)
            {
                boxerWorkoutSchedule.ScheduledDate = DateTime.UtcNow;
            }
            boxerWorkoutSchedule.LocationId = LocationId;
            return PartialView("_ScheduleBoxerWorkout", boxerWorkoutSchedule);
        }


        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts_Schedule)]
        public ActionResult BulkScheduleBoxerWorkout()
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.BoxerWorkouts_Schedule);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            var boxerWorkoutBulkSchedule = new ModelBoxerWorkoutBulkSchedule();
            //workoutBulkSchedule = _balWorkout.GetScheduledWorkoutDetail(WorkoutScheduleId);
            //if (workoutBulkSchedule == null) workoutBulkSchedule = new ModelWorkoutBulkSchedule();
            if (userData.RoleCode == RoleTypes.SuperAdmin)
            {
                boxerWorkoutBulkSchedule.BoxerWorkoutList = _balWorkout.GetAllWorkoutsBoxer(userData.UserId, null, true).Where(e => e.IsReadyForLocations).ToList();
            }
            else
            {
                boxerWorkoutBulkSchedule.BoxerWorkoutList = _balWorkout.GetAllWorkoutsBoxer(userData.ClientOwnerId, null, false).Where(e => e.IsReadyForLocations).ToList();
            }

            boxerWorkoutBulkSchedule.ScheduledDate = DateTime.Now;
            var gymLocations = new List<GymLocationMapping>();
            gymLocations = _balGym.GetAllLocationByGym(Convert.ToInt32(userData.ClientId)).Where(e => e.IsActive == true).ToList();
            if (gymLocations != null && gymLocations.Count > 0)
            {
                boxerWorkoutBulkSchedule.BulkScheduledBoxerWorkout = gymLocations.Select(e => new LocationBoxerWorkoutModel
                {
                    LocationId = e.ID,
                    IsActive = false,
                    IsSelected = false,
                    LocationName = e.LocationName,
                    BoxerWorkoutId = 0
                }).ToList();
            }
            return PartialView("_BulkScheduleBoxerWorkout", boxerWorkoutBulkSchedule);
        }


        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts_Schedule)]
        [HttpPost]
        public JsonResult BulkScheduleBoxerWorkout(ModelBoxerWorkoutBulkSchedule request)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.BoxerWorkouts_Schedule);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false }, JsonRequestBehavior.AllowGet);
            }
            if (request == null || userData == null)
            {
                return Json(new { IsSuccess = false, Message = ErrorMessage.InvalidRequest });
            }
            if (request.BulkScheduledSelectedBoxerWorkout == null || request.BulkScheduledSelectedBoxerWorkout.Count == 0)
            {
                return Json(new { IsSuccess = false, Message = ErrorMessage.SelectLocation });
            }
            var result = _balWorkout.BulkScheduleBoxerWorkout(request);
            //if (result == 0)
            //{

            //    TempData["OperationType"] = "success";
            //    TempData["OperationMessage"] = ErrorMessage.WorkoutScheduled;
            //}
            return Json(new { data = result });
        }

        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts_Schedule)]
        public JsonResult BoxerWorkoutSelect(int BoxerWorkoutScheduleId)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.BoxerWorkouts_Schedule);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false }, JsonRequestBehavior.AllowGet);
            }
            if (BoxerWorkoutScheduleId == 0)
            {
                return Json(new { IsSuccess = false, Message = ErrorMessage.InvalidRequest }, JsonRequestBehavior.AllowGet);
            }
            var result = _balWorkout.BoxerWorkoutSelect(BoxerWorkoutScheduleId, userData);
            return Json(new { IsSuccess = result, Message = ErrorMessage.WorkoutSelected }, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// Retrieve Client's All Users for boxer workout
        /// </summary>
        /// <param name="RoleCode">Role Code</param>
        /// <returns></returns>
        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts)]
        public ActionResult BoxerWorkoutUsers(int Id)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.BoxerWorkouts);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }

            var allUsers = new List<ModelClientUser>();
            var clientId = userData.ClientId.Value;
            allUsers = _balGym.GetGymAllUsers(clientId);
            ViewBag.BoxerWorkoutDetail = _balWorkout.GetByIDBoxer(Id);
            //ViewBag.PortalId = PortalId;
            return View(allUsers);
        }

        /// <summary>
        /// Retrieve Portal User Mapping List
        /// </summary>
        /// <param name="RoleCode">Role Code</param>
        /// <returns>Role Permissions</returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts)]
        public JsonResult GetBoxerWorkoutUserMappings(int WorkoutId)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.BoxerWorkouts);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false }, JsonRequestBehavior.AllowGet);
            }
            var allUsers = new List<ModelClientUserForBoxerWorkout>();
            var clientId = userData.ClientId.Value;
            allUsers = _balGym.GetGymAllUsersForBoxerWorkout(clientId, WorkoutId);
            return Json(new { Data = allUsers }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Save Training Portal User Permission Mapping
        /// </summary>
        /// <param name="request">Role Permission Detail Model</param>
        /// <returns></returns>
        [HttpPost]
        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts)]
        public JsonResult SaveBoxerWorkoutUser(ModelBoxerWorkoutUserMapping request)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.BoxerWorkouts);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false });
            }
            if (request != null)
            {
                request.CreatedBy = userData.UserId;
                request.UpdatedBy = userData.UserId;
                var result = _balWorkout.SaveBoxerWorkoutUserMapping(request);
                return Json(new { IsSuccess = result });
            }
            return Json(new { IsSuccess = false });
        }


        [HttpPost]
        [HasPermissionFilter(PermissionCode = Permission.BoxerWorkouts)]
        public ActionResult BulkMoveBoxer(int CategoryId, List<int> Ids)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.BoxerWorkouts);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            var moveResult = _balWorkout.BulkMoveBoxer(CategoryId, Ids);
            switch (moveResult)
            {
                case CommonResultsMessage.Update:
                    return Json(new { status = true, message = ErrorMessage.BoxerWorkoutMoved });
                case CommonResultsMessage.Fail:
                    return Json(new { status = false, message = ErrorMessage.TryLater });

            }
            return Json(new { statu = "fail", message = "something went wrong" });
        }

        #endregion
    }
}