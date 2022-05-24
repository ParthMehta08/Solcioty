using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using BusinessLayer;
using web;
using Services;
using System.Web.UI;
using System.Text.RegularExpressions;

namespace Solcioty.Controllers
{
    [SessionTimeoutFilterAttribute]
    public class VideoController : BaseController
    {
        private BALVideo _balVideo;
        private BALFittnessTag _balFittnessTag;
		private BalCategory _balCategory;
		public VideoController()
        {
            _balVideo = new BALVideo();
            _balFittnessTag = new BALFittnessTag();
			_balCategory = new BalCategory();
        }
        // GET: Video
        /// <summary>
        /// Get all videos based on logged in user.
        /// If user is super admin then all videos will be retrieved else videos created by user will be retrieved
        /// </summary>
        /// <returns>Video List</returns>
        [HasPermissionFilter(PermissionCode = Permission.Videos)]
        public ActionResult Index(int? categoryId)
        {
            //show list of all Videos
            //#PENDING
            //as per role, will filtre video list
            ModelVideo objModel = new ModelVideo();
            var userData = Session["UserData"] as UserData;
            if (userData != null)
            {
				if(categoryId!=null)
					ViewBag.Category = new BalCategory().GetByID(categoryId.Value);
                objModel.VideoList = new List<ModelVideo>();
                if(userData.RoleCode == RoleTypes.SuperAdmin)
                {
                    objModel.VideoList = _balVideo.GetAllVideos(userData.UserId,categoryId, true);
					objModel.Categories = _balCategory.GetAllCategory(userData.UserId, true).Select(x => new SelectListItem
					{
						Text = x.Name,
						Value = x.Id.ToString()
					}).ToList();
				}
                else
                {
                    objModel.VideoList = _balVideo.GetAllVideos(userData.ClientOwnerId,categoryId, false);
					objModel.Categories = _balCategory.GetAllCategory(userData.ClientOwnerId, false).Select(x => new SelectListItem
					{
						Text = x.Name,
						Value = x.Id.ToString()
					}).ToList();
				}
            }
            return View(objModel);
        }

        /// <summary>
        /// Add or Edit Page for Video detail
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Videos)]
        public ActionResult Add()
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Videos);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            ModelVideo objModel = new ModelVideo();
            objModel = initialVideoObject(objModel);
            return View(objModel);
        }

        /// <summary>
        /// Get All tags and Build Video model
        /// </summary>
        /// <param name="objModel"></param>
        /// <returns></returns>
        private ModelVideo initialVideoObject(ModelVideo objModel)
        {
            //get all tages 

            var userData = Session["UserData"] as UserData;
            if(userData.RoleCode == RoleTypes.SuperAdmin)
            {
                objModel.tagsDictionary = _balFittnessTag.GetAllFitnessTagDictionary(userData.UserId,ForAdmin: true);
				objModel.Categories = _balCategory.GetAllCategory(userData.UserId, true).Select(x => new SelectListItem
				{
					Text = x.Name,
					Value = x.Id.ToString()
				}).ToList();
			}
            else
            {
                objModel.tagsDictionary = _balFittnessTag.GetAllFitnessTagDictionary(userData.ClientOwnerId,ForAdmin: false);
				objModel.Categories = _balCategory.GetAllCategory(userData.UserId, false).Select(x => new SelectListItem
				{
					Text = x.Name,
					Value = x.Id.ToString()
				}).ToList();
			}
            return objModel;
        }

        /// <summary>
        /// Save video detail
        /// </summary>
        /// <param name="objModel"></param>
        /// <returns></returns>
        [HttpPost]
        [HasPermissionFilter(PermissionCode = Permission.Videos)]
        [ValidateInput(false)]
        public ActionResult Add(ModelVideo objModel)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Videos);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            if (ModelState.IsValid)
            {
                objModel.CreatedBy = userData.ClientOwnerId;
                objModel.UpdatedBy = userData.ClientOwnerId;
                //Convert video to stream to save in database
                if (objModel.Videoclip != null && objModel.Videoclip.FileName != "" && objModel.Videoclip.ContentType == "video/mp4")
                {
                    objModel.IsVideoStoreInDB = false;
                    HttpPostedFileBase file = objModel.Videoclip;
                    var buffer = new byte[file.ContentLength];
                    int bytesReaded = file.InputStream.Read(buffer, 0, objModel.Videoclip.ContentLength);
                    if (bytesReaded > 0)
                    {
                        objModel.Video1 = buffer;
                        objModel.VideoServerPath = Server.MapPath(System.Web.Configuration.WebConfigurationManager.AppSettings["AllVideos"].ToString().Trim());
                        objModel.VideoSize = objModel.Videoclip.ContentLength;
                        objModel.ContentType = objModel.Videoclip.ContentType;
                        var videoName = Regex.Replace(objModel.Videoclip.FileName, @"[^.0-9a-zA-Z]+", " ");
                        //objModel.FullName = objModel.Videoclip.FileName.Replace("-", "").Replace("+", "").Substring(objModel.Videoclip.FileName.LastIndexOf("\\")+1);
                        objModel.FullName = videoName;
                    }
                }
                var result = _balVideo.AddEditVideo(objModel);
                switch (result)
                {
                    case CommonResultsMessage.Add:
                        TempData["OperationType"] = "success";
                        TempData["OperationMessage"] = ErrorMessage.VideoAdded;
                        return RedirectToAction("Index");
                    case CommonResultsMessage.Update:
                        TempData["OperationType"] = "success";
                        TempData["OperationMessage"] = ErrorMessage.VideoUpdated;
                        return RedirectToAction("Index");
                    case CommonResultsMessage.Fail:
                        TempData["OperationType"] = "fail";
                        TempData["OperationMessage"] = ErrorMessage.TryLater;
                        break;
                    case CommonResultsMessage.duplicate:
                        TempData["OperationType"] = "fail";
                        TempData["OperationMessage"] = ErrorMessage.DuplicateVideo;
                        break;
                    default:
                        break;
                }
            }
            objModel = initialVideoObject(objModel);
            return View(objModel);
        }

        /// <summary>
        /// Open video detail page in Edit mode
        /// </summary>
        /// <param name="ID">Video Id</param>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Videos)]
        public ActionResult Edit(Int32 ID)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Videos);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            var objmodel = _balVideo.GetVideoByID(ID);
            objmodel = initialVideoObject(objmodel);
            return View("Add", objmodel);
        }

       
        /// <summary>
        /// Make soft delete of Video
        /// </summary>
        /// <param name="ID">Video Id</param>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Videos)]
        public ActionResult Delete(Int32 ID)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Videos);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
          //  var result = new JsonResult();
            var deleteresult = _balVideo.Delete(ID);
            switch (deleteresult)
            {
                case CommonResultsMessage.Update:
                   // result.Data = true;
                    TempData["OperationType"] = "success";
                    TempData["OperationMessage"] = ErrorMessage.VideoDeleted;
                    break;
                case CommonResultsMessage.Fail:
                    //result.Data = false;
                    TempData["OperationType"] = "fail";
                    TempData["OperationMessage"] = ErrorMessage.TryLater;
                    break;
            }
           // result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return RedirectToAction("Index");
        }


		[HttpPost]
		[HasPermissionFilter(PermissionCode = Permission.Videos)]
		public ActionResult BulkDelete(List<int> Ids)
		{
			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Videos);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return RedirectToAction("Unauthorized", "Account");
			}
			//  var result = new JsonResult();
			var deleteresult = _balVideo.BulkDelete(Ids);
			switch (deleteresult)
			{
				case CommonResultsMessage.Update:
					// result.Data = true;
					TempData["OperationType"] = "success";
					TempData["OperationMessage"] = ErrorMessage.VideoDeleted;

					return Json(new { status = true, message = ErrorMessage.VideoDeleted });
					
				case CommonResultsMessage.Fail:
					//result.Data = false;
					//TempData["OperationType"] = "fail";
					//TempData["OperationMessage"] = ErrorMessage.TryLater;

					return Json(new { status = false, message = ErrorMessage.TryLater });
				
			}
			// result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
			return Json(new { statu="fail", message="something went wrong" });
		}

		[HttpPost]
		[HasPermissionFilter(PermissionCode = Permission.Videos)]
		public ActionResult BulkMove(int CategoryId, List<int> Ids)
		{
			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Videos);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return RedirectToAction("Unauthorized", "Account");
			}
			//  var result = new JsonResult();
			var moveresult = _balVideo.BulkMove(CategoryId,Ids);
			switch (moveresult)
			{
				case CommonResultsMessage.Update:
					// result.Data = true;
					TempData["OperationType"] = "success";
					TempData["OperationMessage"] = ErrorMessage.VideoMoved;

					return Json(new { status = true, message = ErrorMessage.VideoMoved });

				case CommonResultsMessage.Fail:
					//result.Data = false;
					//TempData["OperationType"] = "fail";
					//TempData["OperationMessage"] = ErrorMessage.TryLater;

					return Json(new { status = false, message = ErrorMessage.TryLater });

			}
			// result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
			return Json(new { statu = "fail", message = "something went wrong" });
		}


		[HttpGet]
		public ActionResult ReorderAllVideos() {

			var reorderResult = _balVideo.ReorderAllVideos();
			switch (reorderResult)
			{
				case CommonResultsMessage.Update:
					// result.Data = true;
					TempData["OperationType"] = "success";
					TempData["OperationMessage"] = ErrorMessage.VideoUpdated;

					return Json(new { status = true, message = ErrorMessage.VideoDeleted }, JsonRequestBehavior.AllowGet);

				case CommonResultsMessage.Fail:
					//result.Data = false;
					//TempData["OperationType"] = "fail";
					//TempData["OperationMessage"] = ErrorMessage.TryLater;

					return Json(new { status = false, message = ErrorMessage.TryLater }, JsonRequestBehavior.AllowGet);

			}
			// result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
			return Json(new { statu = "fail", message = "something went wrong" },JsonRequestBehavior.AllowGet);

		}

		[HttpPost]
		[HasPermissionFilter(PermissionCode = Permission.Videos)]
		public ActionResult ReorderVideos(List<VideoDisplayOrderData> DisplayOrderData)
		{
			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Videos);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return RedirectToAction("Unauthorized", "Account");
			}
			//  var result = new JsonResult();
			var reorderResult = _balVideo.ReorderVideos(DisplayOrderData);
			switch (reorderResult)
			{
				case CommonResultsMessage.Update:
					// result.Data = true;
					TempData["OperationType"] = "success";
					TempData["OperationMessage"] = ErrorMessage.VideoUpdated;

					return Json(new { status = true, message = ErrorMessage.VideoDeleted });

				case CommonResultsMessage.Fail:
					//result.Data = false;
					//TempData["OperationType"] = "fail";
					//TempData["OperationMessage"] = ErrorMessage.TryLater;

					return Json(new { status = false, message = ErrorMessage.TryLater });

			}
			// result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
			return Json(new { statu = "fail", message = "something went wrong" });
		}

		/// <summary>
		/// Retrieve Video file detail
		/// </summary>
		/// <param name="ID">Video Id</param>
		/// <returns></returns>
		[OutputCache(Duration = 300, VaryByParam = "ID", Location = OutputCacheLocation.ServerAndClient)]
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Videos)]
        public FileContentResult ShowVideo(Int32 ID)
        {
            try
            {
                ModelVideo objModel = _balVideo.GetVideoByID(ID);
                var userData = Session["UserData"] as UserData;
                if (objModel.IsActive || userData.RoleCode == RoleTypes.ClientAdmin)
                {
                    objModel.VideoServerPath = Server.MapPath(System.Web.Configuration.WebConfigurationManager.AppSettings["AllVideos"].ToString().Trim()) + objModel.ID + "\\" + objModel.FullName;
                    objModel.Video1 = System.IO.File.ReadAllBytes(objModel.VideoServerPath);
                    Response.Cache.SetExpires(DateTime.Now.AddHours(1));
                    Response.Cache.SetCacheability(HttpCacheability.Public);
                    return File(objModel.Video1, objModel.ContentType, objModel.FullName);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }


    [SessionTimeoutFilterAttribute]
    public class TagController : BaseController
    {
        private BALFittnessTag _BALFittnessTag;
        public TagController()
        {
            _BALFittnessTag = new BALFittnessTag();
        }

        /// <summary>
        /// Get All Tags
        /// </summary>
        /// <returns>Tag List</returns>
        // GET: WorkoutType
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Tags)]
        public ActionResult Index()
        {
            //#Pending 
            //Filter list as per role
            var userData = Session["UserData"] as UserData;

            if (userData.RoleCode == RoleTypes.BranchAdmin)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            ModelFitnessTag objmodel = new ModelFitnessTag();
            if(userData.RoleCode == RoleTypes.SuperAdmin)
            {
                objmodel.FitnessTagList = _BALFittnessTag.GetAllFitnessTag(userData.UserId, true);
            }
            else
            {
                objmodel.FitnessTagList = _BALFittnessTag.GetAllFitnessTag(userData.ClientOwnerId, false);
            }
            return View(objmodel);
        }

        /// <summary>
        /// Retrieve All tags to bind Autocomplete textbox
        /// </summary>
        /// <returns>TagName List</returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Tags)]
        public JsonResult GetAllTags()
        {
            var userData = Session["UserData"] as UserData;
            var tagList = new List<string>();
            if(userData.RoleCode == RoleTypes.SuperAdmin)
            {
                tagList = _BALFittnessTag.GetAllFitnessTag(userData.UserId, true).Where(e => e.IsActive).Select(e => e.TagName).ToList(); ;
            }
            else
            {
                tagList = _BALFittnessTag.GetAllFitnessTag(userData.ClientOwnerId).Where(e=>e.IsActive).Select(e => e.TagName).ToList();
            }
            return Json(new {data = tagList }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save Tag detail
        /// </summary>
        /// <param name="objmodel"></param>
        /// <returns></returns>
        [HttpPost]
        [HasPermissionFilter(PermissionCode = Permission.Tags)]
        public ActionResult Index(ModelFitnessTag objmodel)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Tags);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            if (ModelState.IsValid)
            {
                objmodel.CreatedBy = userData.ClientOwnerId;
                objmodel.UpdatedBy = userData.ClientOwnerId;
                var result = _BALFittnessTag.AddEdit(objmodel);
                switch (result)
                {
                    case CommonResultsMessage.Add:
                        ViewBag.OperationType = "success";
                        ViewBag.OperationMessage = ErrorMessage.TagAdded;
                        ModelState.Clear();
                        break;
                    case CommonResultsMessage.Update:
                        ViewBag.OperationType = "success";
                        ViewBag.OperationMessage = ErrorMessage.TagUpdated;
                        ModelState.Clear();
                        break;
                    case CommonResultsMessage.Fail:
                        ViewBag.OperationType = "fail";
                        ViewBag.OperationMessage = ErrorMessage.TryLater;
                        break;
                    case CommonResultsMessage.duplicate:
                        ViewBag.OperationType = "fail";
                        ViewBag.OperationMessage = ErrorMessage.DuplicateTag;
                        break;
                    default:
                        break;
                }
            }
            if(userData.RoleCode  == RoleTypes.SuperAdmin)
            {
                objmodel.FitnessTagList = _BALFittnessTag.GetAllFitnessTag(userData.UserId, true);
            }
            else
            {
                objmodel.FitnessTagList = _BALFittnessTag.GetAllFitnessTag(userData.ClientOwnerId, false);
            }
            objmodel.TagName = string.Empty;
            objmodel.ID = 0;
            objmodel.IsActiveString = string.Empty;
            return View(objmodel);
        }

        /// <summary>
        /// Open Tag detail in Edit mode
        /// </summary>
        /// <param name="ID">Tag Id</param>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Tags)]
        public ActionResult Edit(Int32 ID)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Tags);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            ModelFitnessTag objmodel = new ModelFitnessTag();
            objmodel = _BALFittnessTag.GetByID(ID);
            if(userData.RoleCode == RoleTypes.SuperAdmin)
            {
                objmodel.FitnessTagList = _BALFittnessTag.GetAllFitnessTag(userData.UserId, true);
            }
            else
            {
                objmodel.FitnessTagList = _BALFittnessTag.GetAllFitnessTag(userData.ClientOwnerId, false);
            }
            return View("Index", objmodel);
        }


        /// <summary>
        /// Make soft delete of Tag
        /// </summary>
        /// <param name="ID">Tag Id</param>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Tags)]
        public ActionResult Delete(Int32 ID)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Tags);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            ModelFitnessTag objmodel = new ModelFitnessTag();
            var result = _BALFittnessTag.Delete(ID);
            switch (result)
            {
                case CommonResultsMessage.Update:
                    ViewBag.OperationType = "success";
                    ViewBag.OperationMessage = ErrorMessage.TagDeleted;
                    ModelState.Clear();
                    break;
                case CommonResultsMessage.Fail:
                    ViewBag.OperationType = "fail";
                    ViewBag.OperationMessage = ErrorMessage.TryLater;
                    ModelState.Clear();
                    break;
                default:
                    break;
            }
            if(userData.RoleCode == RoleTypes.SuperAdmin)
            {
                objmodel.FitnessTagList = _BALFittnessTag.GetAllFitnessTag(userData.UserId, true);
            }
            else
            {
                objmodel.FitnessTagList = _BALFittnessTag.GetAllFitnessTag(userData.ClientOwnerId, false);
            }
            
            return View("Index", objmodel);
        }


    }
}