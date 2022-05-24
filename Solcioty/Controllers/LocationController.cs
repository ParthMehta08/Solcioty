using System;
using System.Web.Mvc;
using Models;
using BusinessLayer;
using web;
using Services;

namespace Solcioty.Controllers
{

    [SessionTimeoutFilterAttribute]
    public class LocationController : BaseController
    {
        private BalLocation _BalLocation;
        private BalState _BalState;
        private BalCity _BalCity;

        public LocationController()
        {
            _BalLocation = new BalLocation();
            _BalState = new BalState();
            _BalCity = new BalCity();
        }

        /// <summary>
        /// Get All Branches
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Branch)]
        public ActionResult Index()
        {
            ModelLocation objmodel = new ModelLocation();
            objmodel.ModelLocationList = _BalLocation.GetAllLocations(true);
            return View(objmodel);
        }
       

        /// <summary>
        /// Open Add Branch Detail page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Branch)]
        public ActionResult Add()
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Branch);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            ModelLocation objmodel=new ModelLocation();
           return View(InitialiseLoaction(objmodel));
        }

        /// <summary>
        /// Save Branch Detail
        /// </summary>
        /// <param name="objModel">Branch Detail Model</param>
        /// <returns></returns>
        [HttpPost]
        [HasPermissionFilter(PermissionCode = Permission.Branch)]
        public ActionResult Add(ModelLocation objModel)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Branch);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            if (ModelState.IsValid)
            {
                objModel.CreatedBy = userData.ClientOwnerId;
                objModel.UpdatedBy = userData.ClientOwnerId;
                var result =_BalLocation.AddEdit(objModel);
                switch (result)
                {
                    case CommonResultsMessage.Add:
                        return RedirectToAction("Index");
                    case CommonResultsMessage.Update:
                        return RedirectToAction("Index");
                    case CommonResultsMessage.Fail:
                        ViewBag.OperationType = "fail";
                        ViewBag.OperationMessage = ErrorMessage.TryLater;
                        break;
                    case CommonResultsMessage.duplicate:
                        ViewBag.OperationType = "fail";
                        ViewBag.OperationMessage = ErrorMessage.DuplicateLocation;
                        break;
                    default:
                        break;
                }
            }
            return View(InitialiseLoaction(objModel));
        }


        /// <summary>
        /// Build Branch Model
        /// </summary>
        /// <param name="objModel"></param>
        /// <returns></returns>
        private ModelLocation InitialiseLoaction(ModelLocation objModel)
        {
            //#Pending Set states List
            objModel.StateList = _BalState.GetAllStates();
            if (objModel.CityID > 0)
            {
                var city = _BalCity.GetById(objModel.CityID);
                //.CityList = _BalCity.GetAllCitiesByStateCode(city.StateCode);
                objModel.StateCode = city.StateCode;
                objModel.CityName = city.CityName;
            }
            else
            {
                //objModel.CityList = _BalCity.GetAllCitiesByStateCode(objModel.StateList[0].Abbreviation);
                objModel.StateCode = objModel.StateList[0].Abbreviation;

            }
            return objModel;
        }


        /// <summary>
        /// Open Branch Detail in Edit mode
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Branch)]
        public  ActionResult Edit(Int32 ID)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Branch);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            ModelLocation objModel = new ModelLocation();
            objModel = _BalLocation.GetByID(ID);

            return View("Add", InitialiseLoaction(objModel));
        }


        /// <summary>
        /// Make soft delete of Branch detail
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Branch)]
        public ActionResult Delete(Int32 ID)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Branch);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            var result = new JsonResult();
            ModelFitnessTag objmodel = new ModelFitnessTag();
            var deleteresult = _BalLocation.Delete(ID);
            switch (deleteresult)
            {
                case CommonResultsMessage.Update:
                    result.Data = true;
                    break;
                case CommonResultsMessage.Fail:
                    result.Data = false;
                    break;
            }
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }


        /// <summary>
        /// Retrieve All cities by State
        /// </summary>
        /// <param name="stateCode">State Code</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetAllCityByStateCode(string stateCode)
        {
            var cities = _BalCity.GetAllCitiesByStateCode(stateCode);

            return Json(new { Data = cities }, JsonRequestBehavior.AllowGet);
        }
    }
}