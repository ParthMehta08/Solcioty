using BusinessLayer;
using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web;

namespace Solcioty.Controllers
{
	[SessionTimeoutFilterAttribute]
	public class TrainingController : Controller
    {
		private BalTrainingPortal _balTrainingPortal;
		private BalTrainingPortalCategory _balTrainingPortalCategory;
		private BalTrainingPortalSubCategory _balTrainingPortalSubCategory;
		private BalTrainingPortalSubCategoryVideo _balTrainingPortalSubCategoryVideo;
		private BalGym _balGym;
		public TrainingController()
		{
			_balTrainingPortal = new BalTrainingPortal();
			_balTrainingPortalCategory = new BalTrainingPortalCategory();
			_balTrainingPortalSubCategory = new BalTrainingPortalSubCategory();
			_balTrainingPortalSubCategoryVideo = new BalTrainingPortalSubCategoryVideo();
			_balGym = new BalGym();
		}


		// GET: Training Portal
		[HasPermissionFilter(PermissionCode = Permission.TrainingPortal)]
		[HttpGet]
		public ActionResult Index()
		{
			//get list of training portals
			var objModel = new List<ModelTrainingPortal>();
			var userData = Session["UserData"] as UserData;
			if (userData != null)
			{
				if (userData.RoleCode == RoleTypes.SuperAdmin)
				{
					objModel = _balTrainingPortal.GetAllTrainingPortals(userData.UserId, true).ToList();
				}
				else if (userData.RoleCode == RoleTypes.ClientAdmin)
				{
					objModel = _balTrainingPortal.GetAllTrainingPortals(userData.ClientOwnerId, false).ToList();
				}
			}
			return View(objModel);
		}


		/// <summary>
		/// Open Add Portal Detail page
		/// </summary>
		/// <returns></returns>
		[HasPermissionFilter(PermissionCode = Permission.TrainingPortal)]
		public ActionResult Add()
		{

			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.TrainingPortal);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return RedirectToAction("Unauthorized", "Account");
			}
			var objmodel = new ModelTrainingPortal();
			return View(objmodel);
		}


		/// <summary>
		/// Save Training Portal Detail
		/// </summary>
		/// <param name="objModel">Training Portal Detail Model</param>
		/// <returns></returns>
		[HttpPost]
		[HasPermissionFilter(PermissionCode = Permission.TrainingPortal)]
		public JsonResult Add(ModelTrainingPortal objModel)
		{
			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.TrainingPortal);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return Json(new { IsSuccess = false, IsAuthorized = false });
			}
			if (objModel.Id <= 0) {
				objModel.CreatedBy = userData.UserId;
				objModel.CreatedDate = DateTime.UtcNow;
			}
			objModel.UpdatedBy = userData.UserId;
			objModel.UpdatedDate = DateTime.UtcNow;
			var result = _balTrainingPortal.AddEditTrainingPortal(objModel);
			switch (result)
			{
				case CommonResultsMessage.Add:
					TempData["OperationType"] = "success";
					TempData["OperationMessage"] = ErrorMessage.TrainingPortalAdded;
					return Json(new { IsSuccess = true, Message = ErrorMessage.TrainingPortalAdded });
				case CommonResultsMessage.Update:
					TempData["OperationType"] = "success";
					TempData["OperationMessage"] = ErrorMessage.TrainingPortalUpdated;
					return Json(new { IsSuccess = true, ErrorMessage.TrainingPortalUpdated });
				case CommonResultsMessage.Fail:
					TempData["OperationType"] = "fail";
					TempData["OperationMessage"] = ErrorMessage.TryLater;
					break;
				default:
					break;
			}
			return Json(new { IsSuccess = false, Message = ErrorMessage.TryLater });
		}

		/// <summary>
		/// Open Portal Detail in Edit mode to update
		/// </summary>
		/// <param name="ID">Client Id</param>
		/// <returns></returns>
		[HttpGet]
		[HasPermissionFilter(PermissionCode = Permission.TrainingPortal)]
		public ActionResult Edit(Int32 Id)
		{
			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.TrainingPortal);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return RedirectToAction("Unauthorized", "Account");
			}
			var objModel = new ModelTrainingPortal();
			objModel = _balTrainingPortal.GetTrainingPortalById(Id);
			if (objModel == null)
				return RedirectToAction("index");
			return View("Add", objModel);
		}

		/// <summary>
		/// Make Soft Delete Portal Delete
		/// </summary>
		/// <param name="ID">Client Id</param>
		/// <returns></returns>
		[HttpGet]
		[HasPermissionFilter(PermissionCode = Permission.TrainingPortal)]
		public JsonResult Delete(Int32 Id)
		{
			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.TrainingPortal);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return Json(new { IsSuccess = false, IsAuthorized = false }, JsonRequestBehavior.AllowGet);
			}
			var result = new JsonResult();
			ModelFitnessTag objmodel = new ModelFitnessTag();
			var deleteresult = _balTrainingPortal.Delete(Id);
			if (deleteresult)
			{
				TempData["OperationType"] = "success";
				TempData["OperationMessage"] = ErrorMessage.TrainingPortalDeleted;
			}
			else
			{
				TempData["OperationType"] = "fail";
				TempData["OperationMessage"] = ErrorMessage.TryLater;
			}
			return Json(new { data = deleteresult }, JsonRequestBehavior.AllowGet);
		}

		#region Portal Categories

		/// <summary>
		/// Open Portal Detail in Edit mode to update
		/// </summary>
		/// <param name="ID">Client Id</param>
		/// <returns></returns>
		[HttpGet]
		[HasPermissionFilter(PermissionCode = Permission.TrainingPortal)]
		public ActionResult Categories(long Id)
		{
			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.TrainingPortal);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return RedirectToAction("Unauthorized", "Account");
			}
			ViewBag.PortalDetail = _balTrainingPortal.GetTrainingPortalById(Id);
			//ViewBag.PortalId = Id;
			//ViewBag.Test = "testdata";
			var objModel = new List<ModelTrainingPortalCategory>();
			objModel = _balTrainingPortalCategory.GetAllTrainingPortalCategories(Id,userData.UserId,false,false).ToList();
			if (objModel == null)
				return RedirectToAction("index");
			return View(objModel);
		}


		/// <summary>
		/// Open Add Portal Category Detail page
		/// </summary>
		/// <returns></returns>
		[HasPermissionFilter(PermissionCode = Permission.TrainingPortal)]
		public ActionResult AddPortalCategory(long Id,long PortalId)
		{

			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.TrainingPortal);
			if (PortalId<=0 || permissionType == Models.Enums.PermissionType.Read)
			{
				return RedirectToAction("Unauthorized", "Account");
			}
			ViewBag.PoratalName = _balTrainingPortal.GetTrainingPortalById(PortalId).Name;

			var objmodel = new ModelTrainingPortalCategory();
			objmodel.TraningPortalId = PortalId;
			if (Id>0)
				objmodel = _balTrainingPortalCategory.GetTrainingPortalCategoryById(Id);

			return View(objmodel);
		}


		/// <summary>
		/// Save Training Portal Category Detail
		/// </summary>
		/// <param name="objModel">Training Portal Detail Model</param>
		/// <returns></returns>
		[HttpPost]
		[HasPermissionFilter(PermissionCode = Permission.TrainingPortal)]
		public JsonResult AddPortalCategory(ModelTrainingPortalCategory objModel)
		{
			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.TrainingPortal);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return Json(new { IsSuccess = false, IsAuthorized = false });
			}
			if (objModel.Id <= 0)
			{
				objModel.CreatedBy = userData.UserId;
				objModel.CreatedDate = DateTime.UtcNow;
			}
			objModel.UpdatedBy = userData.UserId;
			objModel.UpdatedDate = DateTime.UtcNow;
			var result = _balTrainingPortalCategory.AddEditTrainingPortalCategory(objModel);
			switch (result)
			{
				case CommonResultsMessage.Add:
					TempData["OperationType"] = "success";
					TempData["OperationMessage"] = ErrorMessage.TrainingPortalCategoryAdded;
					return Json(new { IsSuccess = true, Message = ErrorMessage.TrainingPortalCategoryAdded });
				case CommonResultsMessage.Update:
					TempData["OperationType"] = "success";
					TempData["OperationMessage"] = ErrorMessage.TrainingPortalCategoryUpdated;
					return Json(new { IsSuccess = true, ErrorMessage.TrainingPortalCategoryUpdated });
				case CommonResultsMessage.Fail:
					TempData["OperationType"] = "fail";
					TempData["OperationMessage"] = ErrorMessage.TryLater;
					break;
				default:
					break;
			}
			return Json(new { IsSuccess = false, Message = ErrorMessage.TryLater });
		}

		/// <summary>
		/// Open Portal Category Detail in Edit mode to update
		/// </summary>
		/// <param name="ID">Client Id</param>
		/// <returns></returns>
		[HttpGet]
		[HasPermissionFilter(PermissionCode = Permission.TrainingPortal)]
		public ActionResult EditPortalCategory(long Id)
		{
			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.TrainingPortal);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return RedirectToAction("Unauthorized", "Account");
			}
			//ViewBag.PoratalName = _balTrainingPortal.GetTrainingPortalById(Id).Name;

			var objModel = new ModelTrainingPortalCategory();
			objModel = _balTrainingPortalCategory.GetTrainingPortalCategoryById(Id);
			if (objModel == null)
				return RedirectToAction("index");
			ViewBag.PoratalName = _balTrainingPortal.GetTrainingPortalById(objModel.TraningPortalId).Name;
			return View("AddPortalCategory", objModel);
		}

		/// <summary>
		/// Make Soft Delete Portal Category Delete
		/// </summary>
		/// <param name="ID">Client Id</param>
		/// <returns></returns>
		[HttpGet]
		[HasPermissionFilter(PermissionCode = Permission.TrainingPortal)]
		public JsonResult DeleteCategory(long Id)
		{
			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.TrainingPortal);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return Json(new { IsSuccess = false, IsAuthorized = false }, JsonRequestBehavior.AllowGet);
			}
			var result = new JsonResult();
			//ModelFitnessTag objmodel = new ModelFitnessTag();
			var deleteresult = _balTrainingPortalCategory.Delete(Id);
			if (deleteresult)
			{
				TempData["OperationType"] = "success";
				TempData["OperationMessage"] = ErrorMessage.TrainingPortalCategoryDeleted;
			}
			else
			{
				TempData["OperationType"] = "fail";
				TempData["OperationMessage"] = ErrorMessage.TryLater;
			}
			return Json(new { data = deleteresult }, JsonRequestBehavior.AllowGet);
		}

		#endregion Portal Categories


		#region Portal Sub Categories

		/// <summary>
		/// Open Portal Sub Category List
		/// </summary>
		/// <param name="ID">Category Id</param>
		/// <returns></returns>
		[HttpGet]
		[HasPermissionFilter(PermissionCode = Permission.TrainingPortal)]
		public ActionResult PortalSubCategories(long Id)
		{
			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.TrainingPortal);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return RedirectToAction("Unauthorized", "Account");
			}
			ViewBag.PortalCategoryDetail = _balTrainingPortalCategory.GetTrainingPortalCategoryById(Id);
			var objModel = new List<ModelTrainingPortalSubCategory>();
			objModel = _balTrainingPortalSubCategory.GetAllTrainingPortalSubCategories(Id, userData.UserId, false, false).ToList();
			if (objModel == null)
				return RedirectToAction("index");
			return View(objModel);
		}

		/// <summary>
		/// Open Add Portal Category Detail page
		/// </summary>
		/// <returns></returns>
		[HasPermissionFilter(PermissionCode = Permission.TrainingPortal)]
		public ActionResult AddPortalSubCategory(long Id, long CategoryId)
		{

			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.TrainingPortal);
			if (CategoryId <= 0 || permissionType == Models.Enums.PermissionType.Read)
			{
				return RedirectToAction("Unauthorized", "Account");
			}
			ViewBag.PortalCategoryDetail = _balTrainingPortalCategory.GetTrainingPortalCategoryById(CategoryId);

			var objmodel = new ModelTrainingPortalSubCategory();
			objmodel.TraningPortalCategoryId = CategoryId;
			objmodel.TraningPortalId = ViewBag.PortalCategoryDetail.TraningPortalId;
			if (Id > 0)
				objmodel = _balTrainingPortalSubCategory.GetTrainingPortalSubCategoryById(Id);

			return View(objmodel);
		}


		/// <summary>
		/// Save Training Portal Category Detail
		/// </summary>
		/// <param name="objModel">Training Portal Detail Model</param>
		/// <returns></returns>
		[HttpPost]
		[HasPermissionFilter(PermissionCode = Permission.TrainingPortal)]
		public JsonResult AddPortalSubCategory(ModelTrainingPortalSubCategory objModel)
		{
			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.TrainingPortal);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return Json(new { IsSuccess = false, IsAuthorized = false });
			}
			if (objModel.Id <= 0)
			{
				objModel.CreatedBy = userData.UserId;
				objModel.CreatedDate = DateTime.UtcNow;
			}
			objModel.UpdatedBy = userData.UserId;
			objModel.UpdatedDate = DateTime.UtcNow;
			var result = _balTrainingPortalSubCategory.AddEditTrainingPortalSubCategory(objModel);
			switch (result)
			{
				case CommonResultsMessage.Add:
					TempData["OperationType"] = "success";
					TempData["OperationMessage"] = ErrorMessage.TrainingPortalCategoryAdded;
					return Json(new { IsSuccess = true, Message = ErrorMessage.TrainingPortalCategoryAdded });
				case CommonResultsMessage.Update:
					TempData["OperationType"] = "success";
					TempData["OperationMessage"] = ErrorMessage.TrainingPortalCategoryUpdated;
					return Json(new { IsSuccess = true, ErrorMessage.TrainingPortalCategoryUpdated });
				case CommonResultsMessage.Fail:
					TempData["OperationType"] = "fail";
					TempData["OperationMessage"] = ErrorMessage.TryLater;
					break;
				default:
					break;
			}
			return Json(new { IsSuccess = false, Message = ErrorMessage.TryLater });
		}

		/// <summary>
		/// Open Portal Category Detail in Edit mode to update
		/// </summary>
		/// <param name="ID">Client Id</param>
		/// <returns></returns>
		[HttpGet]
		[HasPermissionFilter(PermissionCode = Permission.TrainingPortal)]
		public ActionResult EditPortalSubCategory(long Id)
		{
			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.TrainingPortal);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return RedirectToAction("Unauthorized", "Account");
			}
			//ViewBag.PoratalName = _balTrainingPortal.GetTrainingPortalById(Id).Name;

			var objModel = new ModelTrainingPortalSubCategory();
			objModel = _balTrainingPortalSubCategory.GetTrainingPortalSubCategoryById(Id);
			if (objModel == null)
				return RedirectToAction("index");
			ViewBag.PortalCategoryDetail = _balTrainingPortalCategory.GetTrainingPortalCategoryById(objModel.TraningPortalCategoryId);
			return View("AddPortalSubCategory", objModel);
		}

		/// <summary>
		/// Make Soft Delete Portal Category Delete
		/// </summary>
		/// <param name="ID">Client Id</param>
		/// <returns></returns>
		[HttpGet]
		[HasPermissionFilter(PermissionCode = Permission.TrainingPortal)]
		public JsonResult DeleteSubCategory(long Id)
		{
			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.TrainingPortal);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return Json(new { IsSuccess = false, IsAuthorized = false }, JsonRequestBehavior.AllowGet);
			}
			var result = new JsonResult();
			//ModelFitnessTag objmodel = new ModelFitnessTag();
			var deleteresult = _balTrainingPortalSubCategory.Delete(Id);
			if (deleteresult)
			{
				TempData["OperationType"] = "success";
				TempData["OperationMessage"] = ErrorMessage.TrainingPortalCategoryDeleted;
			}
			else
			{
				TempData["OperationType"] = "fail";
				TempData["OperationMessage"] = ErrorMessage.TryLater;
			}
			return Json(new { data = deleteresult }, JsonRequestBehavior.AllowGet);
		}


		/// <summary>
		/// Open Portal Sub Category Video List
		/// </summary>
		/// <param name="ID">SubCategory Id</param>
		/// <returns></returns>
		[HttpGet]
		[HasPermissionFilter(PermissionCode = Permission.TrainingPortal)]
		public ActionResult SubCategorieVideos(long Id)
		{
			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.TrainingPortal);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return RedirectToAction("Unauthorized", "Account");
			}
			ViewBag.SubCategoryDetail = _balTrainingPortalSubCategory.GetTrainingPortalSubCategoryById(Id);
			var objModel = new List<ModelTrainingPortalSubCategoryVideo>();
			objModel = _balTrainingPortalSubCategoryVideo.GetAllTrainingPortalSubCategorieVideos(Id, userData.UserId, false, false).ToList();
			if (objModel == null)
				return RedirectToAction("index");
			return View(objModel);
		}


		/// <summary>
		/// Open Add Portal Category Detail page
		/// </summary>
		/// <returns></returns>
		[HasPermissionFilter(PermissionCode = Permission.TrainingPortal)]
		public ActionResult AddPortalSubCategoryVideo(long Id, long SubCategoryId)
		{

			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.TrainingPortal);
			if (SubCategoryId <= 0 || permissionType == Models.Enums.PermissionType.Read)
			{
				return RedirectToAction("Unauthorized", "Account");
			}
			ViewBag.SubCategoryDetail = _balTrainingPortalSubCategory.GetTrainingPortalSubCategoryById(SubCategoryId);

			var objmodel = new ModelTrainingPortalSubCategoryVideo();
			objmodel.PortalSubCategoryId = SubCategoryId;
			if (Id > 0)
				objmodel = _balTrainingPortalSubCategoryVideo.GetTrainingPortalSubCategoryVideoById(Id);

			return View(objmodel);
		}


		/// <summary>
		/// Save Training Portal Category Detail
		/// </summary>
		/// <param name="objModel">Training Portal Detail Model</param>
		/// <returns></returns>
		[HttpPost]
		[HasPermissionFilter(PermissionCode = Permission.TrainingPortal)]
		public JsonResult AddPortalSubCategoryVideo(ModelTrainingPortalSubCategoryVideo objModel)
		{
			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.TrainingPortal);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return Json(new { IsSuccess = false, IsAuthorized = false });
			}
			var result = _balTrainingPortalSubCategoryVideo.AddEditTrainingPortalSubCategoryVideo(objModel);
			switch (result)
			{
				case CommonResultsMessage.Add:
					TempData["OperationType"] = "success";
					TempData["OperationMessage"] = ErrorMessage.TrainingPortalSubCategoryVideoAdded;
					return Json(new { IsSuccess = true, Message = ErrorMessage.TrainingPortalSubCategoryVideoAdded });
				case CommonResultsMessage.Update:
					TempData["OperationType"] = "success";
					TempData["OperationMessage"] = ErrorMessage.TrainingPortalSubCategoryVideoUpdated;
					return Json(new { IsSuccess = true, ErrorMessage.TrainingPortalSubCategoryVideoUpdated });
				case CommonResultsMessage.Fail:
					TempData["OperationType"] = "fail";
					TempData["OperationMessage"] = ErrorMessage.TryLater;
					break;
				default:
					break;
			}
			return Json(new { IsSuccess = false, Message = ErrorMessage.TryLater });
		}

		/// <summary>
		/// Open Portal Category Detail in Edit mode to update
		/// </summary>
		/// <param name="ID">Client Id</param>
		/// <returns></returns>
		[HttpGet]
		[HasPermissionFilter(PermissionCode = Permission.TrainingPortal)]
		public ActionResult EditPortalSubCategoryVideo(long Id)
		{
			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.TrainingPortal);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return RedirectToAction("Unauthorized", "Account");
			}
			//ViewBag.PoratalName = _balTrainingPortal.GetTrainingPortalById(Id).Name;

			var objModel = new ModelTrainingPortalSubCategoryVideo();
			objModel = _balTrainingPortalSubCategoryVideo.GetTrainingPortalSubCategoryVideoById(Id);
			if (objModel == null)
				return RedirectToAction("index");
			ViewBag.SubCategoryDetail = _balTrainingPortalSubCategory.GetTrainingPortalSubCategoryById(objModel.PortalSubCategoryId);
			return View("AddPortalSubCategoryVideo", objModel);
		}

		/// <summary>
		/// Make Soft Delete Portal Category Delete
		/// </summary>
		/// <param name="ID">Client Id</param>
		/// <returns></returns>
		[HttpGet]
		[HasPermissionFilter(PermissionCode = Permission.TrainingPortal)]
		public JsonResult DeleteSubCategoryVideo(long Id)
		{
			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.TrainingPortal);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return Json(new { IsSuccess = false, IsAuthorized = false }, JsonRequestBehavior.AllowGet);
			}
			var result = new JsonResult();
			//ModelFitnessTag objmodel = new ModelFitnessTag();
			var deleteresult = _balTrainingPortalSubCategoryVideo.Delete(Id);
			if (deleteresult)
			{
				TempData["OperationType"] = "success";
				TempData["OperationMessage"] = ErrorMessage.TrainingPortalCategoryDeleted;
			}
			else
			{
				TempData["OperationType"] = "fail";
				TempData["OperationMessage"] = ErrorMessage.TryLater;
			}
			return Json(new { data = deleteresult }, JsonRequestBehavior.AllowGet);
		}

		#endregion Portal Sub Categories


		#region Portal Users

		/// <summary>
		/// Retrieve Client's All Users
		/// </summary>
		/// <param name="RoleCode">Role Code</param>
		/// <returns></returns>
		[HasPermissionFilter(PermissionCode = Permission.TrainingPortal)]
		public ActionResult PortalUsers(long Id)
		{
			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.TrainingPortal);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return RedirectToAction("Unauthorized", "Account");
			}

			var allUsers = new List<ModelClientUser>();
			var clientId = userData.ClientId.Value;
			allUsers = _balGym.GetGymAllUsers(clientId);
			ViewBag.PortalDetail = _balTrainingPortal.GetTrainingPortalById(Id);
			//ViewBag.PortalId = PortalId;
			return View(allUsers);
		}

		/// <summary>
		/// Retrieve Portal User Mapping List
		/// </summary>
		/// <param name="RoleCode">Role Code</param>
		/// <returns>Role Permissions</returns>
		[HttpGet]
		[HasPermissionFilter(PermissionCode = Permission.TrainingPortal)]
		public JsonResult GetPortalUserMappings(long PortalId)
		{
			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.TrainingPortal);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return Json(new { IsSuccess = false, IsAuthorized = false }, JsonRequestBehavior.AllowGet);
			}
			var allUsers = new List<ModelClientUser>();
			var clientId = userData.ClientId.Value;
			allUsers = _balGym.GetGymAllUsers(clientId);
			return Json(new { Data = allUsers }, JsonRequestBehavior.AllowGet);
		}


		/// <summary>
		/// Save Training Portal User Permission Mapping
		/// </summary>
		/// <param name="request">Role Permission Detail Model</param>
		/// <returns></returns>
		[HttpPost]
		[HasPermissionFilter(PermissionCode = Permission.TrainingPortal)]
		public JsonResult SavePortalUser(ModelTrainingPortalUserMapping request)
		{
			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.TrainingPortal);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return Json(new { IsSuccess = false, IsAuthorized = false });
			}
			if (request != null)
			{
				request.CreatedBy = userData.UserId;
				request.UpdatedBy = userData.UserId;
				var result = _balTrainingPortal.SaveTrainingPortalUserMapping(request);
				return Json(new { IsSuccess = result });
			}
			return Json(new { IsSuccess = false });
		}


		/// <summary>
		/// Retrieve Portal Subcategory Video List
		/// </summary>
		/// <param name="Id">Sub Category Id</param>
		/// <returns>Video List</returns>
		[HttpGet]
		public JsonResult GetSubCategoryVideoList(long Id)
		{
			var userData = Session["UserData"] as UserData;
			var IsTrainingPortalAccessible = _balGym.CheckTrainingPortalAccessibleByUser(userData.UserId);
			if (!IsTrainingPortalAccessible)
			{
				return Json(new { IsSuccess = false, IsAuthorized = false }, JsonRequestBehavior.AllowGet);
			}
			var allVideos = new List<ModelTrainingPortalSubCategoryVideo>();
			allVideos = _balTrainingPortalSubCategoryVideo.GetAllTrainingPortalSubCategorieVideos(Id, userData.UserId, false, false).ToList();
			
			return Json(new { Data = allVideos }, JsonRequestBehavior.AllowGet);
		}

		#endregion Portal Users

	}
}