using BusinessLayer;
using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using web;

namespace Solcioty.Controllers
{
    [SessionTimeoutFilterAttribute]
    public class GymController : Controller
    {
        private BalGym _BalGym;
        private BalUser _BalUser;
        private BalState _BalState;
        private BalCity _BalCity;
        private BalLocation _BalLocation;
        public GymController()
        {
            _BalGym = new BalGym();
            _BalUser = new BalUser();
            _BalState = new BalState();
            _BalCity = new BalCity();
            _BalLocation = new BalLocation();

        }
        // GET: Gym
        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// Retrieve All Client (Gym)
        /// </summary>
        /// <returns></returns>
        [HasPermissionFilter(PermissionCode = Permission.Client)]
        [OutputCache(Duration = 40, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "none")]
        public ActionResult GymList()
        {
            var gym = new ModelGym();
            gym.GymList = _BalGym.GetAllGyms();
            return View(gym);
        }


        /// <summary>
        /// Get All Branch
        /// </summary>
        /// <param name="ID">Client Id</param>
        /// <returns>Branch List</returns>
        [HasPermissionFilter(PermissionCode = Permission.Branch)]
        public ActionResult Branch(int ID)
        {
            var gym = new ModelGym();
            gym = _BalGym.GetById(ID);
            var gymLocations = new List<GymLocationMapping>();
            gymLocations = _BalGym.GetAllLocationByGym(ID);
            gym.GymLocations = gymLocations;
            return View(gym);
        }


        [HasPermissionFilter(PermissionCode = Permission.Branch)]
        public JsonResult GetAllBranchByClient()
        {
            var userData = Session["UserData"] as UserData;
            var gymLocations = new List<GymLocationMapping>();
            gymLocations = _BalGym.GetAllLocationByGym(Convert.ToInt32(userData.ClientId)).Where(e => e.IsActive == true).ToList();
            return Json(new { Locations = gymLocations }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Save Branch Detail for Client
        /// </summary>
        /// <param name="GymId">Client Id</param>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Branch)]
        public ActionResult AddBranch(int GymId)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Branch);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            var gymLocationMapping = new GymLocationMapping();
            gymLocationMapping.GymId = GymId;
            gymLocationMapping.StateList = _BalState.GetAllStates();
            gymLocationMapping.CityList = _BalCity.GetAllCitiesByStateCode(gymLocationMapping.StateList[0].Abbreviation);
            return View(gymLocationMapping);
        }

        /// <summary>
        /// Open Branch Detail in Edit mode
        /// </summary>
        /// <param name="MappingId">Branch Location mapping Id</param>
        /// <returns></returns>
        [HasPermissionFilter(PermissionCode = Permission.Branch)]
        public ActionResult EditBranch(int MappingId)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Branch);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            var gymLocationMapping = new GymLocationMapping();
            gymLocationMapping = _BalGym.GetGymLocationMappingDetail(MappingId);
            gymLocationMapping.StateList = _BalState.GetAllStates();
            if (gymLocationMapping.CityID > 0)
            {
                var city = _BalCity.GetById(gymLocationMapping.CityID);
                gymLocationMapping.CityList = _BalCity.GetAllCitiesByStateCode(city.StateCode);
                gymLocationMapping.StateCode = city.StateCode;
            }
            else
            {
                gymLocationMapping.CityList = _BalCity.GetAllCitiesByStateCode(gymLocationMapping.StateList[0].Abbreviation);

            }
            return View("AddBranch", gymLocationMapping);
        }

        /// <summary>
        /// Save Branch Detail
        /// </summary>
        /// <param name="objModel">Branch Detail Model</param>
        /// <returns></returns>
        [HttpPost]
        [HasPermissionFilter(PermissionCode = Permission.Branch)]
        [ValidateInput(false)]
        public ActionResult SaveBranch(GymLocationMapping objModel)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Branch);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            objModel.CreatedBy = userData.ClientOwnerId;
            objModel.UpdatedBy = userData.ClientOwnerId;
            var result = _BalGym.AddEditBranch(objModel);
            switch (result)
            {
                case CommonResultsMessage.Add:
                    TempData["OperationType"] = "success";
                    TempData["OperationMessage"] = ErrorMessage.LocationAdded;
                    return RedirectToAction("Branch", new { ID = objModel.GymId });
                case CommonResultsMessage.Update:
                    TempData["OperationType"] = "success";
                    TempData["OperationMessage"] = ErrorMessage.LocationUpdated;
                    return RedirectToAction("Branch", new { ID = objModel.GymId });
                case CommonResultsMessage.Fail:
                    TempData["OperationType"] = "success";
                    TempData["OperationMessage"] = ErrorMessage.TryLater;
                    break;
                default:
                    break;
            }
            return View(objModel);
        }


        /// <summary>
        /// Open Branch Manage page
        /// </summary>
        /// <param name="Id">Branch Id</param>
        /// <returns></returns>
        [HasPermissionFilter(PermissionCode = Permission.Branch)]
        public ActionResult ManageBranch(int Id)
        {

            var userData = Session["UserData"] as UserData;
            if (userData.RoleCode == RoleTypes.BranchAdmin && userData.BranchId != Id)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            var branchDetail = new ModelBranch();
            branchDetail = _BalGym.GetBranchDetail(Id);
            return View(branchDetail);
        }


        /// <summary>
        /// Save Branch User Detail
        /// </summary>
        /// <param name="branchDetail">User Detail Model</param>
        /// <returns></returns>
        [HttpPost]
        [HasPermissionFilter(PermissionCode = Permission.Branch)]
        public JsonResult SaveBranchUserDetail(ModelBranch branchDetail)
        {

            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.User);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false });
            }
            if (branchDetail != null && branchDetail.UserList.Count > 0)
            {
                var result = _BalGym.SaveBranchUserDetail(branchDetail);
                switch (result)
                {
                    case CommonResultsMessage.Update:
                        TempData["OperationType"] = "success";
                        TempData["OperationMessage"] = ErrorMessage.UserSaved;
                        break;
                    case CommonResultsMessage.Fail:
                        TempData["OperationType"] = "fail";
                        TempData["OperationMessage"] = ErrorMessage.GeneralError;
                        break;
                }
                if (result == CommonResultsMessage.Add || result == CommonResultsMessage.Update)
                {
                    return Json(new { IsSuccess = true });
                }
                return Json(new { IsSuccess = false });
            }
            else
            {
                return Json(new { IsSuccess = false, Message = ErrorMessage.GeneralError });
            }
        }


        /// <summary>
        /// Retrieved Branch User Detail
        /// </summary>
        /// <param name="branchId">Branch Id</param>
        /// <param name="userId">User Id</param>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Branch)]
        public JsonResult GetBranchUserDetail(int branchId, int userId)
        {
            if (branchId > 0 && userId > 0)
            {
                var result = _BalGym.GetBranchUserDetail(userId, branchId);
                return Json(new { IsSuccess = true, Data = result }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { IsSuccess = false }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Make Soft delete for Branch User
        /// </summary>
        /// <param name="BranchId">Branch Id</param>
        /// <param name="UserId">User Id</param>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Branch)]
        public JsonResult DeleteBranchUser(int BranchId, int UserId)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Branch);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false }, JsonRequestBehavior.AllowGet);
            }
            if (BranchId > 0 && UserId > 0)
            {
                var result = _BalGym.DeleteBranchUser(BranchId, UserId);
                return Json(new { IsSuccess = result }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { IsSuccess = false }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Open Add Client(Gym) Detail page
        /// </summary>
        /// <returns></returns>
        [HasPermissionFilter(PermissionCode = Permission.Client)]
        public ActionResult Add()
        {

            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Client);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            var objmodel = new ModelGym();
            objmodel.Owners = new List<ModelUser>();
            var user = new ModelUser();
            user.StateList = _BalState.GetAllStates();
            user.CityList = _BalCity.GetAllCitiesByStateCode(user.StateList[0].Abbreviation);
            BindWorkoutType();
            objmodel.Owners.Add(user);
            return View(InitialiseGym(objmodel));
        }
        public void BindWorkoutType()
        {
            List<SelectListItem> WorkOutType = new List<SelectListItem>();
            WorkOutType.Add(new SelectListItem { Value = "1", Text = "Normal Workout" });
            WorkOutType.Add(new SelectListItem { Value = "2", Text = "Boxer Workout" });
            ViewBag.WorkoutBag = WorkOutType;
        }

        /// <summary>
        /// Save Client(Gym) Detail
        /// </summary>
        /// <param name="objModel">Client Detail Model</param>
        /// <returns></returns>
        [HttpPost]
        [HasPermissionFilter(PermissionCode = Permission.Client)]
        public JsonResult Add(ModelGym objModel)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Client);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false });
            }
            if(objModel.Owners[0].WorkOutType == 0)
            {
                return Json(new { IsSuccess = false, Message = ErrorMessage.WorkoutType });
            }
            var result = _BalGym.AddEdit(objModel);
            switch (result)
            {
                case CommonResultsMessage.Add:
                    TempData["OperationType"] = "success";
                    TempData["OperationMessage"] = ErrorMessage.ClientAdded;
                    return Json(new { IsSuccess = true, Message = ErrorMessage.ClientAdded });
                case CommonResultsMessage.Update:
                    TempData["OperationType"] = "success";
                    TempData["OperationMessage"] = ErrorMessage.ClientUpdated;
                    return Json(new { IsSuccess = true, ErrorMessage.ClientUpdated });
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
        /// Remove Client Admin from Client Admin List
        /// </summary>
        /// <param name="GymId">Client Id</param>
        /// <param name="AdminId">Admin Id</param>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Client)]
        public JsonResult RemoveAdmin(int GymId, int AdminId)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Client);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false });
            }
            var result = _BalGym.RemoveGymAdmin(GymId, AdminId);
            return Json(new { IsSuccess = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Open Client Detail in Edit mode to update
        /// </summary>
        /// <param name="ID">Client Id</param>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Client)]
        public ActionResult Edit(Int32 ID)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Client);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            ModelGym objModel = new ModelGym();
            objModel = _BalGym.GetById(ID);
            List<SelectListItem> WorkOutType = new List<SelectListItem>();
            WorkOutType.Add(new SelectListItem { Value = "1", Text = "Normal Workout" });
            WorkOutType.Add(new SelectListItem { Value = "2", Text = "Boxer Workout" });

            if(objModel.Owners[0].WorkOutType != 0)
            {
            WorkOutType.Where(x => x.Value == objModel.Owners[0].WorkOutType.ToString()).FirstOrDefault().Selected = true;
            }

            ViewBag.WorkoutBag = WorkOutType;
            if (objModel == null)
                return RedirectToAction("gymlist");
            return View("Add", InitialiseGym(objModel));
        }

        /// <summary>
        /// Make Soft Delete Client Delete
        /// </summary>
        /// <param name="ID">Client Id</param>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Client)]
        public JsonResult Delete(Int32 ID)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Client);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false }, JsonRequestBehavior.AllowGet);
            }
            var result = new JsonResult();
            ModelFitnessTag objmodel = new ModelFitnessTag();
            var deleteresult = _BalGym.Delete(ID);
            if (deleteresult)
            {
                TempData["OperationType"] = "success";
                TempData["OperationMessage"] = ErrorMessage.ClientDeleted;
            }
            else
            {
                TempData["OperationType"] = "fail";
                TempData["OperationMessage"] = ErrorMessage.TryLater;
            }
            return Json(new { data = deleteresult }, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// Get All Admin list for all branches of client
        /// </summary>
        /// <param name="ClientId"></param>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Branch)]
        public JsonResult GetClientBranchAdmin(int ClientId)
        {
            var clientBranchAdminList = new List<ModelBranchAdmin>();
            clientBranchAdminList = _BalGym.GetClientBranchAdmins(ClientId);
            return Json(new { Data = clientBranchAdminList }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Make soft delete of Branch Detail
        /// </summary>
        /// <param name="MappingId">Client Location Mapping Id</param>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Branch)]
        public JsonResult DeleteBranch(int MappingId)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Branch);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false }, JsonRequestBehavior.AllowGet);
            }
            var result = _BalGym.DeleteBranch(MappingId);
            if (result)
            {
                TempData["OperationType"] = "success";
                TempData["OperationMessage"] = ErrorMessage.LocationDeleted;
            }
            else
            {
                TempData["OperationType"] = "fail";
                TempData["OperationMessage"] = ErrorMessage.TryLater;
            }
            return Json(new { IsSuccess = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Build Client Detail Model
        /// </summary>
        /// <param name="objModel"></param>
        /// <returns></returns>
        private ModelGym InitialiseGym(ModelGym objModel)
        {
            if (objModel != null && objModel.Id > 0)
            {
                objModel.Owners = _BalUser.GetGymOwners(objModel.Id);
                if (objModel.Owners.Count == 0)
                {
                    var user = new ModelUser();
                    user.StateList = _BalState.GetAllStates();
                    user.CityList = _BalCity.GetAllCitiesByStateCode(user.StateList[0].Abbreviation);
                    objModel.Owners.Add(user);
                }
            }
            return objModel;
        }

        [HttpPost]
        public JsonResult SaveClientConfiguration(ModelGymConfiguration objModel)
        {
            var result = false;
            if (objModel == null)
            {
                return Json(new { IsSuccess = result, Message = ErrorMessage.InvalidRequest });
            }
            result = _BalGym.SaveClientConfiguration(objModel);
            return Json(new { IsSuccess = result, Message = ErrorMessage.ConfigurationSaved });
        }

        public ActionResult Configuration()
        {
            var clientConfiguration = new ModelGymConfiguration();
            var userData = Session["UserData"] as UserData;
            clientConfiguration.ClientId = Convert.ToInt32(userData.ClientId);

            clientConfiguration = _BalGym.GetClientConfiguration(clientConfiguration.ClientId);
            return View(clientConfiguration);
        }

        public ActionResult SaveConfiguration(ModelGymConfiguration configuration)
        {
            configuration.LogoName = string.Empty;
            configuration.AlternateLogoName = string.Empty;
            configuration.BackgroundImageName = string.Empty;
            if (configuration.Logo != null && (configuration.Logo.ContentType == "image/jpg" || configuration.Logo.ContentType == "image/jpeg" || configuration.Logo.ContentType == "image/png"))
            {
                var file = configuration.Logo;
                var buffer = new byte[file.ContentLength];
                if (buffer != null)
                {
                    configuration.LogoName = configuration.Logo.FileName.Substring(configuration.Logo.FileName.LastIndexOf("\\") + 1);


                    if (configuration.LogoName != null)
                    {
                        configuration.LogoServerPath = Server.MapPath(System.Web.Configuration.WebConfigurationManager.AppSettings["ClientLogos"].ToString().Trim());
                        configuration.LogoServerPath = configuration.LogoServerPath + configuration.ClientId;
                        if (!System.IO.Directory.Exists(configuration.LogoServerPath))
                        {
                            System.IO.Directory.CreateDirectory(configuration.LogoServerPath);
                        }
                        //System.IO.File.WriteAllBytes(configuration.LogoServerPath + "\\" + configuration.LogoName, buffer);
                        configuration.Logo.SaveAs(configuration.LogoServerPath + "\\" + configuration.LogoName);
                    }
                }
            }
            if (configuration.AlternateLogo != null && (configuration.AlternateLogo.ContentType == "image/jpg" || configuration.AlternateLogo.ContentType == "image/jpeg" || configuration.AlternateLogo.ContentType == "image/png"))
            {
                var file = configuration.AlternateLogo;
                var buffer = new byte[file.ContentLength];
                if (buffer != null)
                {
                    configuration.AlternateLogoName = configuration.AlternateLogo.FileName.Substring(configuration.AlternateLogo.FileName.LastIndexOf("\\") + 1);


                    if (configuration.AlternateLogoName != null)
                    {
                        configuration.AlternateLogoServerPath = Server.MapPath(System.Web.Configuration.WebConfigurationManager.AppSettings["ClientLogos"].ToString().Trim());
                        configuration.AlternateLogoServerPath = configuration.AlternateLogoServerPath + configuration.ClientId;
                        if (!System.IO.Directory.Exists(configuration.AlternateLogoServerPath))
                        {
                            System.IO.Directory.CreateDirectory(configuration.AlternateLogoServerPath);
                        }
                        //System.IO.File.WriteAllBytes(configuration.LogoServerPath + "\\" + configuration.LogoName, buffer);
                        configuration.AlternateLogo.SaveAs(configuration.AlternateLogoServerPath + "\\" + configuration.AlternateLogoName);
                    }
                }
            }
            if (configuration.BackgroundImage != null && (configuration.BackgroundImage.ContentType == "image/jpg" || configuration.BackgroundImage.ContentType == "image/jpeg" || configuration.BackgroundImage.ContentType == "image/png"))
            {
                var file = configuration.BackgroundImage;
                var buffer = new byte[file.ContentLength];
                if (buffer != null)
                {
                    configuration.BackgroundImageName = configuration.BackgroundImage.FileName.Substring(configuration.BackgroundImage.FileName.LastIndexOf("\\") + 1);

                    if (configuration.BackgroundImageName != null)
                    {
                        configuration.BackgroundImageServerPath = Server.MapPath(System.Web.Configuration.WebConfigurationManager.AppSettings["ClientLogos"].ToString().Trim());
                        configuration.BackgroundImageServerPath = configuration.BackgroundImageServerPath + configuration.ClientId;

                        if (!System.IO.Directory.Exists(configuration.BackgroundImageServerPath))
                        {
                            System.IO.Directory.CreateDirectory(configuration.BackgroundImageServerPath);
                        }
                        configuration.BackgroundImage.SaveAs(configuration.BackgroundImageServerPath + "\\" + configuration.BackgroundImageName);
                    }
                }
            }


            var result = _BalGym.SaveClientConfiguration(configuration);
            if (result)
            {
                ViewBag.OperationType = "success";
                ViewBag.OperationMessage = ErrorMessage.ConfigurationSaved;
            }
            else
            {
                ViewBag.OperationType = "fail";
                ViewBag.OperationMessage = ErrorMessage.TryLater;
            }
            return RedirectToAction("Configuration");
        }

        [HasPermissionFilter(PermissionCode = Permission.Client)]
        public ActionResult Copy(int id = 0)
        {
            if (id > 0)
            {
                var userData = Session["UserData"] as UserData;
                var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Client);
                if (permissionType == Models.Enums.PermissionType.Read)
                {
                    return RedirectToAction("Unauthorized", "Account");
                }

                var gymDetails = _BalGym.GetById(id);
                if (gymDetails != null)
                {
                    var objmodel = new ModelCopyGym();
                    return View(objmodel);
                }
                ViewBag.GymNotAvaiable = "Gym not found";
            }
            return RedirectToAction("GymList", "Gym");

        }

        [HttpPost]
        public ActionResult Copy(ModelCopyGym model)
        {
            try
            {
                model.strCopyAction = string.Join(",", model.CopyAction);
                var response =  _BalGym.CopySite(model);
                if(response.Code == "S")
                {
                    string videoPath = Server.MapPath(System.Web.Configuration.WebConfigurationManager.AppSettings["AllVideos"].ToString().Trim());
                    string imagePath = Server.MapPath(System.Web.Configuration.WebConfigurationManager.AppSettings["AllImages"].ToString().Trim());
                    string pdfPath = Server.MapPath(System.Web.Configuration.WebConfigurationManager.AppSettings["AllPDFs"].ToString().Trim());
                    string configurationFilePath = Server.MapPath(System.Web.Configuration.WebConfigurationManager.AppSettings["ClientLogos"].ToString().Trim());

                    _BalGym.CopyVideos(response.result.Where(x => x.GroupName.ToUpper() == "VIDEO").ToList(), videoPath);
                    _BalGym.CopyImages(response.result.Where(x => x.GroupName.ToUpper() == "IMAGE").ToList(), imagePath);
                    _BalGym.CopyPdf(response.result.Where(x => x.GroupName.ToUpper() == "PDF").ToList(), pdfPath);
                    _BalGym.CopyConfigurationFile(response.result.Where(x => x.GroupName.ToUpper() == "GYM").ToList(), configurationFilePath);

                    return Json(new { Response = true, Message = "Data copied success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Response = false, Message = "Something went wrong. Please try after sometimes!!!" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Response = false,
                    Message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}