using BusinessLayer;
using Models;
using Services;
using System.Collections.Generic;
using System.Web.Mvc;
using web;

namespace Solcioty.Controllers
{
    [SessionTimeoutFilterAttribute]
    public class UserController : BaseController
    {
        private BalState _balState;
        private BalCity _balCity;
        private BalUser _balUser;
        public UserController()
        {
            _balState = new BalState();
            _balCity = new BalCity();
            _balUser = new BalUser();
        }
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Return Partial View for Add User detail
        /// </summary>
        /// <returns></returns>
        [HasPermissionFilter(PermissionCode = Permission.User)]
        public ActionResult Add()
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.User);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            var userModel = new ModelUser();
            //userModel.StateList = _balState.GetAllStates();
            //userModel.CityList = _balCity.GetAllCitiesByStateCode(userModel.StateList[0].Abbreviation);
            return PartialView("_AddUser", userModel);
        }

        /// <summary>
        /// Retrieve Role List
        /// </summary>
        /// <returns></returns>
        [HasPermissionFilter(PermissionCode = Permission.Roles)]
        public ActionResult UserRoles()
        {
            var userRoles = new List<ModelRole>();
            userRoles = _balUser.GetAllRoles();

            return View(userRoles);

        }

        /// <summary>
        /// Save Role status to Active or InActive
        /// </summary>
        /// <param name="roleId">Role Id</param>
        /// <param name="status">True or False</param>
        /// <returns></returns>
        [HasPermissionFilter(PermissionCode = Permission.Roles)]
        public JsonResult UpdateRoleStatus(int roleId, bool status)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Roles);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false },JsonRequestBehavior.AllowGet);
            }
            var result = _balUser.UpdateRoleStatus(roleId, status);
            if (result)
            {
                TempData["OperationType"] = "success";
                TempData["OperationMessage"] = ErrorMessage.RoleStatusChanged;
            }
            else
            {
                TempData["OperationType"] = "fail";
                TempData["OperationMessage"] = ErrorMessage.TryLater;
            }
                
            return Json(new { IsSuccess = result }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Retrieve Permission List
        /// </summary>
        /// <returns></returns>
        [HasPermissionFilter(PermissionCode = Permission.Permissions)]
        public ActionResult Permissions()
        {
            var permissions = new List<ModelPermission>();
            var userData = Session["UserData"] as UserData;
            if (userData.RoleCode == RoleTypes.SuperAdmin)
            {
                permissions = _balUser.GetAllPermissions(true);
            }
            else
            {
                permissions = _balUser.GetAllPermissions();
            }
            return View(permissions);
        }

        /// <summary>
        /// Save Permission status to Active or InActive
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HasPermissionFilter(PermissionCode = Permission.Permissions)]
        public JsonResult UpdatePermissionStatus(int permissionId, bool status)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Permissions);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false },JsonRequestBehavior.AllowGet);
            }
            var result = _balUser.UpdatePermissionStatus(permissionId, status);
            return Json(new { IsSuccess = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Retrieve All Permission for Role
        /// </summary>
        /// <param name="RoleCode">Role Code</param>
        /// <returns></returns>
        [HasPermissionFilter(PermissionCode = Permission.Permissions)]
        public ActionResult RolePermissions(string RoleCode)
        {
            if(RoleCode == RoleTypes.Client || RoleCode == RoleTypes.BranchInstructor)
            {
               return RedirectToAction("Unauthorized", "Account");
            }
            var roleDetail = new ModelRole();
            roleDetail = _balUser.GetRolePermissions(RoleCode);
            return View(roleDetail);
        }


        /// <summary>
        /// Retrieve Role Permission Mapping List
        /// </summary>
        /// <param name="RoleCode">Role Code</param>
        /// <returns>Role Permissions</returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Roles)]
        public JsonResult GetRolePermissionMappings(string RoleCode)
        {
            var roleDetail = new ModelRole();
            roleDetail = _balUser.GetRolePermissionMapping(RoleCode);
            return Json(new { Data = roleDetail },JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save Role Permission Mapping
        /// </summary>
        /// <param name="request">Role Permission Detail Model</param>
        /// <returns></returns>
        [HttpPost]
        [HasPermissionFilter(PermissionCode = Permission.Roles)]
        public JsonResult SaveRolePermission(ModelRolePermissionRequest request)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Roles);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false });
            }
            if (request != null)
            {
                var result = _balUser.SaveRolePermission(request);
                return Json(new { IsSuccess = result });
            }
            return Json(new { IsSuccess = false });
        }

        public JsonResult CheckUserExist(string EmailId)
        {
            var result = _balUser.CheckUserDuplication(EmailId);
            return Json(new { IsExist = result },JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckUsernameExist(string Username)
        {
            var result = _balUser.CheckUsernameDuplication(Username);
            return Json(new { IsExist = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckEmailExistForCopy(string EmailId)
        {
            var result = _balUser.CheckUserDuplication(EmailId);
            return Json(!result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckUsernameExistForCopy(string Username)
        {
            var result = _balUser.CheckUsernameDuplication(Username);
            return Json(!result, JsonRequestBehavior.AllowGet);
        }
    }
}