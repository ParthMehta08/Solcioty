using System;
using System.Linq;
using System.Web.Mvc;
using Models;
using BusinessLayer;
using web;
using Services;
using System.Collections.Generic;

namespace Solcioty.Controllers
{
    [SessionTimeoutFilterAttribute]
    public class TemplateController : BaseController
    {
        private BALTemplate _balTemplate;
        private BALTemplateVideoMapping _balTemplateVideoMapping;
        private BALVideo _balVideo;
        private BALImage _balImage;
        public TemplateController()
        {
            _balTemplate = new BALTemplate();
            _balTemplateVideoMapping = new BALTemplateVideoMapping();
            _balVideo = new BALVideo();
            _balImage = new BALImage();
        }

        #region Template Block
       
        /// <summary>
        /// Get Template list based on Logged in User. If user is Super Admin then all templates will be retrieved.
        /// Else templates will be retrieved for logged in user
        /// </summary>
        /// <returns>Template List</returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Templates)]
        public ActionResult Index()
        {
            ModelTemplate objModel = new ModelTemplate();
            var userData = Session["UserData"] as UserData;
            if (userData != null)
            {
                if (userData.RoleCode == RoleTypes.BranchAdmin)
                {
                    return RedirectToAction("Unauthorized", "Account");
                }
                //get list of templates
                if (userData.RoleCode == RoleTypes.SuperAdmin)
                {
                    objModel.ModelTemplateList = _balTemplate.GetAllTemplates(userData.UserId, ForAdmin: true);
                }
                else
                {
                    objModel.ModelTemplateList = _balTemplate.GetAllTemplates(userData.ClientOwnerId, ForAdmin: false);
                }

            }
            return View(objModel);
        }


        [HttpGet]
        public JsonResult GetAllTemplate()
        {
            ModelTemplate objModel = new ModelTemplate();
            var userData = Session["UserData"] as UserData;
            if (userData != null)
            {
                //get list of templates
                if (userData.RoleCode == RoleTypes.SuperAdmin)
                {
                    objModel.ModelTemplateList = _balTemplate.GetAllTemplates(userData.UserId, ForAdmin: true);
                }
                else
                {
                    objModel.ModelTemplateList = _balTemplate.GetAllTemplates(userData.ClientOwnerId, ForAdmin: false);
                }

            }

            return Json(new { data = objModel }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Open Add Template Detail page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Templates)]
        public ActionResult Add()
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Templates);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            ModelTemplate objModel = new ModelTemplate();
            return View(objModel);
        }

        /// <summary>
        /// Save Template Detail
        /// </summary>
        /// <param name="objmodel">Template Detail Model</param>
        /// <returns></returns>
        [HttpPost]
        [HasPermissionFilter(PermissionCode = Permission.Templates)]
        [ValidateInput(false)]
        public ActionResult Add(ModelTemplate objmodel)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Templates);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            if (ModelState.IsValid)
            {
                objmodel.CreatedBy = userData.ClientOwnerId;
                objmodel.UpdatedBy = userData.ClientOwnerId;
                var result = _balTemplate.AddEdit(objmodel);
                switch (result)
                {
                    case CommonResultsMessage.Add:
                        TempData["OperationType"] = "success";
                        TempData["OperationMessage"] = ErrorMessage.TemplateAdded;
                        return RedirectToAction("Index");
                    case CommonResultsMessage.Update:
                        TempData["OperationType"] = "success";
                        TempData["OperationMessage"] =ErrorMessage.TemplateUpdated;
                        return RedirectToAction("Index");
                    case CommonResultsMessage.Fail:
                        TempData["OperationType"] = "fail";
                        TempData["OperationMessage"] = ErrorMessage.TryLater;
                        break;
                    case CommonResultsMessage.duplicate:
                        TempData["OperationType"] = "fail";
                        TempData["OperationMessage"] = ErrorMessage.DuplicateTemplate;
                        break;
                    default:
                        break;
                }
            }
            return View(objmodel);
        }

        /// <summary>
        /// Open Template Detail in Edit mode
        /// </summary>
        /// <param name="ID">Template Id</param>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Templates)]
        public ActionResult Edit(Int32 ID)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Templates);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            ModelTemplate objmodel = new ModelTemplate();
            objmodel = _balTemplate.GetByID(ID);
            return View("Add", objmodel);
        }

        /// <summary>
        /// Make soft delete of Template detail
        /// </summary>
        /// <param name="ID">Template Id</param>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Templates)]
        public ActionResult Delete(Int32 ID)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Templates);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            var result = new JsonResult();
            var deleteresult = _balTemplate.Delete(ID);
            switch (deleteresult)
            {
                case CommonResultsMessage.Update:
                    result.Data = true;
                    TempData["OperationType"] = "success";
                    TempData["OperationMessage"] = ErrorMessage.TemplateDeleted;
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
		[HasPermissionFilter(PermissionCode = Permission.Templates)]
		public ActionResult BulkDelete(List<int> Ids)
		{
			var userData = Session["UserData"] as UserData;
			var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Templates);
			if (permissionType == Models.Enums.PermissionType.Read)
			{
				return RedirectToAction("Unauthorized", "Account");
			}
			//  var result = new JsonResult();
			var deleteresult = _balTemplate.BulkDelete(Ids);
			switch (deleteresult)
			{
				case CommonResultsMessage.Update:
					// result.Data = true;
					TempData["OperationType"] = "success";
					TempData["OperationMessage"] = ErrorMessage.TemplateDeleted;

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
		/// Make change Template is ready for Branch or not.
		/// </summary>
		/// <param name="ID">Template Id</param>
		/// <param name="Ready">True or False</param>
		/// <returns></returns>
		[HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Templates)]
        public ActionResult Ready(Int32 ID, string Ready)
        {
            var result = new JsonResult();
            var deleteresult = _balTemplate.TemplateIsReadyForLocations(ID, !(Convert.ToBoolean(Ready)));
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
            //_balTemplate.TemplateIsReadyForLocations(ID, !(Convert.ToBoolean(Ready)));
            //return RedirectToAction("Index");
        }

        /// <summary>
        /// Retrieve all Videos for Template
        /// </summary>
        /// <param name="ID">Template Id</param>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Templates)]
        public ActionResult Videos(Int32 ID)
        {
            var userData = Session["UserData"] as UserData;
            ModelTemplate objModel = new ModelTemplate();
            if (userData != null)
            {
                objModel = _balTemplate.GetByID(ID);
                objModel = _balTemplateVideoMapping.GetTemplateVideos(objModel);
                if(userData.RoleCode == RoleTypes.SuperAdmin)
                {
                    objModel.VideosTable = _balVideo.GetAllVideos(userData.UserId,null, true).Where(p => p.IsActive == true).ToList();

                }
                else
                {
                    objModel.VideosTable = _balVideo.GetAllVideos(userData.ClientOwnerId,null, false).Where(p => p.IsActive == true).ToList();
                }
                objModel.BasicModelTemplateVideoMappingList = objModel.BasicModelTemplateVideoMappingList.OrderBy(p => p.VideoPosition).ToList();
                objModel.AlterModelTemplateVideoMappingList = objModel.AlterModelTemplateVideoMappingList.OrderBy(p => p.VideoPosition).ToList();
            }
           
            return View(objModel);
        }

        /// <summary>
        /// Save All Videos for Templates
        /// </summary>
        /// <param name="objmodel">Template Detail Model</param>
        /// <returns></returns>
        [HttpPost]
        [HasPermissionFilter(PermissionCode = Permission.Templates)]
        public ActionResult Videos(ModelTemplate objmodel)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Videos);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            //save template video mapping 
            _balTemplateVideoMapping.ADDEditVideointemplate(objmodel);
            return RedirectToAction("Videos", new { ID = objmodel.ID });
        }

        /// <summary>
        /// Return Partial view for Showing video
        /// </summary>
        /// <param name="ID">Video Id</param>
        /// <param name="OnlyVideo">True or False</param>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Videos)]
        public ActionResult ViewVideo(Int32 ID, bool OnlyVideo = false)
        {
            ModelVideo objModel = _balVideo.GetVideoByID(ID);
            if (OnlyVideo)
            {
                var temp = new ModelVideo();
                temp.ID = objModel.ID;
				temp.FullName = objModel.FullName;
                objModel = temp;
                ViewBag.SmallSize = true;
            }
            else
            {
                ViewBag.SmallSize = false;
            }
            return PartialView("_ViewVideo", objModel);
        }

        #endregion

        #region Boxer Template Block

        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.BoxerTemplates)]
        public ActionResult BoxerBlock()
        {
            ModelBoxerTemplate objModel = new ModelBoxerTemplate();
            var userData = Session["UserData"] as UserData;
            if (userData != null)
            {
                if (userData.RoleCode == RoleTypes.BranchAdmin)
                {
                    return RedirectToAction("Unauthorized", "Account");
                }
                //get list of templates
                if (userData.RoleCode == RoleTypes.SuperAdmin)
                {
                    objModel.ModelTemplateList = _balTemplate.GetBoxerAllTemplates(userData.UserId, ForAdmin: true);
                }
                else
                {
                    objModel.ModelTemplateList = _balTemplate.GetBoxerAllTemplates(userData.ClientOwnerId, ForAdmin: false);
                }

            }
            return View(objModel);
        }

        [HttpGet]
        public JsonResult GetBoxerAllTemplate()
        {
            ModelBoxerTemplate objModel = new ModelBoxerTemplate();
            var userData = Session["UserData"] as UserData;
            if (userData != null)
            {
                //get list of templates
                if (userData.RoleCode == RoleTypes.SuperAdmin)
                {
                    objModel.ModelTemplateList = _balTemplate.GetBoxerAllTemplates(userData.UserId, ForAdmin: true);
                }
                else
                {
                    objModel.ModelTemplateList = _balTemplate.GetBoxerAllTemplates(userData.ClientOwnerId, ForAdmin: false);
                }

            }

            return Json(new { data = objModel }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.BoxerTemplates)]
        public ActionResult AddBoxer()
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.BoxerTemplates);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            ModelBoxerTemplate objModel = new ModelBoxerTemplate();
            return View(objModel);
        }
        [HttpPost]
        [HasPermissionFilter(PermissionCode = Permission.BoxerTemplates)]
        [ValidateInput(false)]
        public ActionResult AddBoxer(ModelBoxerTemplate objmodel)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Templates);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            if (ModelState.IsValid)
            {
                objmodel.CreatedBy = userData.ClientOwnerId;
                objmodel.UpdatedBy = userData.ClientOwnerId;
                var result = _balTemplate.AddEditBoxer(objmodel);
                switch (result)
                {
                    case CommonResultsMessage.Add:
                        TempData["OperationType"] = "success";
                        TempData["OperationMessage"] = ErrorMessage.TemplateAdded;
                        return RedirectToAction("BoxerBlock");
                    case CommonResultsMessage.Update:
                        TempData["OperationType"] = "success";
                        TempData["OperationMessage"] = ErrorMessage.TemplateUpdated;
                        return RedirectToAction("BoxerBlock");
                    case CommonResultsMessage.Fail:
                        TempData["OperationType"] = "fail";
                        TempData["OperationMessage"] = ErrorMessage.TryLater;
                        break;
                    case CommonResultsMessage.duplicate:
                        TempData["OperationType"] = "fail";
                        TempData["OperationMessage"] = ErrorMessage.DuplicateTemplate;
                        break;
                    default:
                        break;
                }
            }
           // return View(objmodel);
            return RedirectToAction("BoxerBlock");
        }
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.BoxerTemplates)]
        public ActionResult EditBoxer(Int32 ID)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.BoxerTemplates);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            ModelBoxerTemplate objmodel = new ModelBoxerTemplate();
            objmodel = _balTemplate.GetByIDBoxer(ID);
            return View("AddBoxer", objmodel);
        }

        //added by parth mehta for Tempalte render
        public ActionResult BoxerTemplate(Int32 ID)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.BoxerTemplates);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            List<BoxerTemplateModel> boxerTemplateModels = _balTemplate.GetTemplateById(ID);
            return View("Template", boxerTemplateModels);
        }
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.BoxerTemplates)]
        public ActionResult DeleteBoxer(Int32 ID)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Templates);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            var result = new JsonResult();
            var deleteresult = _balTemplate.DeleteBoxer(ID);
            switch (deleteresult)
            {
                case CommonResultsMessage.Update:
                    result.Data = true;
                    TempData["OperationType"] = "success";
                    TempData["OperationMessage"] = ErrorMessage.TemplateDeleted;
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
        [HasPermissionFilter(PermissionCode = Permission.BoxerTemplates)]
        public ActionResult BulkDeleteBoxer(List<int> Ids)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Templates);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            //  var result = new JsonResult();
            var deleteresult = _balTemplate.BulkDeleteBoxer(Ids);
            switch (deleteresult)
            {
                case CommonResultsMessage.Update:
                    // result.Data = true;
                    TempData["OperationType"] = "success";
                    TempData["OperationMessage"] = ErrorMessage.TemplateDeleted;

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
        /// Make change Template is ready for Branch or not.
        /// </summary>
        /// <param name="ID">Template Id</param>
        /// <param name="Ready">True or False</param>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.BoxerTemplates)]
        public ActionResult ReadyBoxer(Int32 ID, string Ready)
        {
            var result = new JsonResult();
            var deleteresult = _balTemplate.TemplateIsReadyForLocationsBoxer(ID, !(Convert.ToBoolean(Ready)));
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
            //_balTemplate.TemplateIsReadyForLocations(ID, !(Convert.ToBoolean(Ready)));
            //return RedirectToAction("Index");
        }

        /// <summary>
        /// Retrieve all Videos for Template
        /// </summary>
        /// <param name="ID">Template Id</param>
        /// <returns></returns>
       
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.BoxerTemplates)]
        public ActionResult VideosBoxer(Int32 ID)
        {
            var userData = Session["UserData"] as UserData;
            ModelBoxerTemplate objModel = new ModelBoxerTemplate();
            if (userData != null)
            {
                objModel = _balTemplate.GetByIDBoxer(ID);
                objModel = _balTemplateVideoMapping.GetTemplateVideosBoxer(objModel);
                if (userData.RoleCode == RoleTypes.SuperAdmin)
                {
                    if (Session["VideosTable"] as IList<ModelVideo> == null)
                    {
                        Session["VideosTable"] = _balVideo.GetAllVideos(userData.UserId, null, true).Where(p => p.IsActive == true).ToList();
                        objModel.ImagesTable = _balImage.GetAllImages(userData.UserId, true).Where(p => p.IsActive == true).ToList();
                        objModel.VideosTable = Session["VideosTable"] as IList<ModelVideo>;
                    }
                    else
                    {
                        objModel.VideosTable = Session["VideosTable"] as IList<ModelVideo>;
                        objModel.ImagesTable = _balImage.GetAllImages(userData.UserId, true).Where(p => p.IsActive == true).ToList();

                    }
                }
                else
                {
                    if (Session["VideosTable"] as IList<ModelVideo> == null)
                    {
                        Session["VideosTable"] = _balVideo.GetAllVideos(userData.ClientOwnerId, null, false).Where(p => p.IsActive == true).ToList();
                        objModel.ImagesTable = _balImage.GetAllImages(userData.ClientOwnerId, false).Where(p => p.IsActive == true).ToList();
                        objModel.VideosTable = Session["VideosTable"] as IList<ModelVideo>;
                    }
                    else
                    {
                        objModel.VideosTable = Session["VideosTable"] as IList<ModelVideo>;
                        objModel.ImagesTable = _balImage.GetAllImages(userData.ClientOwnerId, false).Where(p => p.IsActive == true).ToList();

                    }
                }
                objModel.BasicModelTemplateVideoMappingList = objModel.BasicModelTemplateVideoMappingList.OrderBy(p => p.VideoPosition).ToList();
                objModel.AlterModelTemplateVideoMappingList = objModel.AlterModelTemplateVideoMappingList.OrderBy(p => p.VideoPosition).ToList();
            }



            return View(objModel);
        }


        /// <summary>
        /// Save All Videos for Templates
        /// </summary>
        /// <param name="objmodel">Template Detail Model</param>
        /// <returns></returns>
        [HttpPost]
        [HasPermissionFilter(PermissionCode = Permission.BoxerTemplates)]
        public ActionResult VideosBoxer(ModelBoxerTemplate objmodel)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Videos);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            //save template video mapping 
            _balTemplateVideoMapping.ADDEditVideointemplateBoxer(objmodel);
            return RedirectToAction("VideosBoxer", new { ID = objmodel.ID });
        }

        /// <summary>
        /// Return Partial view for Showing video
        /// </summary>
        /// <param name="ID">Video Id</param>
        /// <param name="OnlyVideo">True or False</param>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Videos)]
        public ActionResult ViewVideoBoxer(Int32 ID, bool OnlyVideo = false,string FileType="")
        {
            if (FileType == "V")
            {
            ModelVideo objModel = _balVideo.GetVideoByID(ID);
            if (OnlyVideo)
            {
                var temp = new ModelVideo();
                temp.ID = objModel.ID;
                temp.FullName = objModel.FullName;
                objModel = temp;
                ViewBag.SmallSize = true;
            }
            else
            {
                ViewBag.SmallSize = false;
            }
            return PartialView("_ViewVideo", objModel);
            }
            else
            {
                ModelImage objModel = _balImage.GetImageByID(ID);
                if (OnlyVideo)
                {
                    var temp = new ModelImage();
                    temp.Id = objModel.Id;
                    temp.ImageTitle = objModel.ImageTitle;
                    temp.ImageFile = objModel.ImageFile;
                    temp.ImageName = objModel.ImageName;
                    objModel = temp;
                    ViewBag.SmallSize = true;
                }
                else
                {
                    ViewBag.SmallSize = false;
                }
                return PartialView("_ViewImage", objModel);
            }
        }


        //added by parth 

        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Videos)]
        public ActionResult ViewIamgeBoxer(Int32 ID, bool OnlyVideo = false)
        {
            ModelImage objModel = _balImage.GetImageByID(ID);
            if (OnlyVideo)
            {
                var temp = new ModelImage();
                temp.Id = objModel.Id;
                temp.ImageTitle= objModel.ImageTitle;
                objModel = temp;
                ViewBag.SmallSize = true;
            }
            else
            {
                ViewBag.SmallSize = false;
            }
            return PartialView("_ViewImage", objModel);
        }

        #endregion
    }
}