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
    public class CategoryController : Controller
    {
		private BalCategory _balCategory;
		public CategoryController()
		{
			_balCategory = new BalCategory();
		}

		/// <summary>
		/// Get All Categories
		/// </summary>
		/// <returns>Category List</returns>
		// GET: WorkoutType
		[HttpGet]
		[HasPermissionFilter(PermissionCode = Permission.Category)]
		public ActionResult Index()
		{
			//#Pending 
			//Filter list as per role
			var userData = Session["UserData"] as UserData;

			if (userData.RoleCode == RoleTypes.BranchAdmin)
			{
				return RedirectToAction("Unauthorized", "Account");
			}
			var objmodel = new ModelCategory();
			if (userData.RoleCode == RoleTypes.SuperAdmin)
			{
				objmodel.CategoryList = _balCategory.GetAllCategory(userData.UserId, true);
			}
			else
			{
				objmodel.CategoryList = _balCategory.GetAllCategory(userData.ClientOwnerId, false);
			}
			return View(objmodel);
		}

		/// <summary>
		/// Retrieve All categories to bind Autocomplete textbox
		/// </summary>
		/// <returns>Category List</returns>
		[HttpGet]
		[HasPermissionFilter(PermissionCode = Permission.Category)]
		public JsonResult GetAllCategories()
		{
			var userData = Session["UserData"] as UserData;
			var categoryList = new List<string>();
			if (userData.RoleCode == RoleTypes.SuperAdmin)
			{
				categoryList = _balCategory.GetAllCategory(userData.UserId, true).Where(e => e.IsActive).Select(e => e.Name).ToList(); ;
			}
			else
			{
				categoryList = _balCategory.GetAllCategory(userData.ClientOwnerId).Where(e => e.IsActive).Select(e => e.Name).ToList();
			}
			return Json(new { data = categoryList }, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Save Category detail
		/// </summary>
		/// <param name="objmodel"></param>
		/// <returns></returns>
		[HttpPost]
		[HasPermissionFilter(PermissionCode = Permission.Category)]
		public ActionResult Index(ModelCategory objmodel)
		{
			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Category);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return RedirectToAction("Unauthorized", "Account");
			}
			if (ModelState.IsValid)
			{
				objmodel.CreatedBy = userData.ClientOwnerId;
				objmodel.UpdatedBy = userData.ClientOwnerId;
				var result = _balCategory.AddEdit(objmodel);
				switch (result)
				{
					case CommonResultsMessage.Add:
						ViewBag.OperationType = "success";
						ViewBag.OperationMessage = ErrorMessage.CategoryAdded;
						ModelState.Clear();
						break;
					case CommonResultsMessage.Update:
						ViewBag.OperationType = "success";
						ViewBag.OperationMessage = ErrorMessage.CategoryUpdated;
						ModelState.Clear();
						break;
					case CommonResultsMessage.Fail:
						ViewBag.OperationType = "fail";
						ViewBag.OperationMessage = ErrorMessage.TryLater;
						break;
					case CommonResultsMessage.duplicate:
						ViewBag.OperationType = "fail";
						ViewBag.OperationMessage = ErrorMessage.DuplicateCategory;
						break;
					default:
						break;
				}
			}
			if (userData.RoleCode == RoleTypes.SuperAdmin)
			{
				objmodel.CategoryList = _balCategory.GetAllCategory(userData.UserId, true);
			}
			else
			{
				objmodel.CategoryList = _balCategory.GetAllCategory(userData.ClientOwnerId, false);
			}
			objmodel.Name = string.Empty;
			objmodel.Id = 0;
			objmodel.IsActiveString = string.Empty;
			return View(objmodel);
		}

		/// <summary>
		/// Open Category detail in Edit mode
		/// </summary>
		/// <param name="ID">Category Id</param>
		/// <returns></returns>
		[HttpGet]
		[HasPermissionFilter(PermissionCode = Permission.Category)]
		public ActionResult Edit(Int32 ID)
		{
			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Category);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return RedirectToAction("Unauthorized", "Account");
			}
			var objmodel = new ModelCategory();
			objmodel = _balCategory.GetByID(ID);
			if (userData.RoleCode == RoleTypes.SuperAdmin)
			{
				objmodel.CategoryList = _balCategory.GetAllCategory(userData.UserId, true);
			}
			else
			{
				objmodel.CategoryList = _balCategory.GetAllCategory(userData.ClientOwnerId, false);
			}
			return View("Index", objmodel);
		}


		/// <summary>
		/// Make soft delete of Category
		/// </summary>
		/// <param name="ID">Category Id</param>
		/// <returns></returns>
		[HttpGet]
		[HasPermissionFilter(PermissionCode = Permission.Category)]
		public ActionResult Delete(Int32 ID)
		{
			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Category);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return RedirectToAction("Unauthorized", "Account");
			}
			var objmodel = new ModelCategory();
			var result = _balCategory.Delete(ID);
			switch (result)
			{
				case CommonResultsMessage.Update:
					ViewBag.OperationType = "success";
					ViewBag.OperationMessage = ErrorMessage.CategoryDeleted;
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
			if (userData.RoleCode == RoleTypes.SuperAdmin)
			{
				objmodel.CategoryList = _balCategory.GetAllCategory(userData.UserId, true);
			}
			else
			{
				objmodel.CategoryList = _balCategory.GetAllCategory(userData.ClientOwnerId, false);
			}

			return RedirectToAction("Index");
		}
	}
}