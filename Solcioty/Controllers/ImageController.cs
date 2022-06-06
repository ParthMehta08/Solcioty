using BusinessLayer;
using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using web;

namespace Solcioty.Controllers
{
    [SessionTimeoutFilterAttribute]
    public class ImageController : BaseController
    {
        private BALImage _balImage;
        private BALFittnessTag _balFittnessTag;
        public ImageController()
        {
            _balImage = new BALImage();
            _balFittnessTag = new BALFittnessTag();
        }
        // GET: Video
        /// <summary>
        /// Get all images based on logged in user.
        /// If user is super admin then all images will be retrieved else images created by user will be retrieved
        /// </summary>
        /// <returns>Image List</returns>
        [HasPermissionFilter(PermissionCode = Permission.Images)]
        public ActionResult Index()
        {
            var objModel = new ModelImage();
            var userData = Session["UserData"] as UserData;
            if (userData != null)
            {
                objModel.ImageList = new List<ModelImage>();
                if (userData.RoleCode == RoleTypes.SuperAdmin)
                {
                    objModel.ImageList = _balImage.GetAllImages(userData.UserId, true);
                }
                else
                {
                    objModel.ImageList = _balImage.GetAllImages(userData.ClientOwnerId, false);
                }
            }
            return View(objModel);
        }

        /// <summary>
        /// Add or Edit Page for Image detail
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Images)]
        public ActionResult Add()
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Images);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            var objModel = new ModelImage();
            objModel = InitialiseImageObject(objModel);
            return View(objModel);
        }

        /// <summary>
        /// Get All tags and Build Image model
        /// </summary>
        /// <param name="objModel"></param>
        /// <returns></returns>
        private ModelImage InitialiseImageObject(ModelImage objModel)
        {
            //get all tages 

            var userData = Session["UserData"] as UserData;
            if (userData.RoleCode == RoleTypes.SuperAdmin)
            {
                objModel.tagsDictionary = _balFittnessTag.GetAllFitnessTagDictionary(userData.UserId, ForAdmin: true);
            }
            else
            {
                objModel.tagsDictionary = _balFittnessTag.GetAllFitnessTagDictionary(userData.ClientOwnerId, ForAdmin: false);
            }
            return objModel;
        }

        /// <summary>
        /// Save image detail
        /// </summary>
        /// <param name="objModel"></param>
        /// <returns></returns>
        [HttpPost]
        [HasPermissionFilter(PermissionCode = Permission.Images)]
        [ValidateInput(false)]
        public ActionResult Add(ModelImage objModel)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Images);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
			if (objModel.Id == 0 && objModel.ImageFile == null)
			{
				TempData["OperationType"] = "fail";
				TempData["OperationMessage"] = "Please select image";
			}
			else {
				if (ModelState.IsValid)
				{
					objModel.CreatedBy = userData.ClientOwnerId;
					objModel.UpdatedBy = userData.ClientOwnerId;
					//Convert video to stream to save in database
					if (objModel.ImageFile != null && objModel.ImageFile.FileName != "" && (objModel.ImageFile.ContentType == "image/jpg" || objModel.ImageFile.ContentType == "image/jpeg" || objModel.ImageFile.ContentType == "image/png" || objModel.ImageFile.ContentType == "image/svg" || objModel.ImageFile.ContentType == "image/gif"))
					{
						HttpPostedFileBase file = objModel.ImageFile;
						var buffer = new byte[file.ContentLength];
						int bytesReaded = file.InputStream.Read(buffer, 0, objModel.ImageFile.ContentLength);
						if (bytesReaded > 0)
						{
							objModel.Image1 = buffer;
							objModel.ImageServerPath = Server.MapPath(System.Web.Configuration.WebConfigurationManager.AppSettings["AllImages"].ToString().Trim());
							objModel.ImageSize = objModel.ImageFile.ContentLength;
							objModel.ContentType = objModel.ImageFile.ContentType;
							var videoName = Regex.Replace(objModel.ImageFile.FileName, @"[^.0-9a-zA-Z]+", " ");
							//objModel.FullName = objModel.Videoclip.FileName.Replace("-", "").Replace("+", "").Substring(objModel.Videoclip.FileName.LastIndexOf("\\")+1);
							objModel.FullName = videoName;
						}
					}
					var result = _balImage.AddEditImage(objModel);
					switch (result)
					{
						case CommonResultsMessage.Add:
							TempData["OperationType"] = "success";
							TempData["OperationMessage"] = ErrorMessage.ImageAdded;
							return RedirectToAction("Index");
						case CommonResultsMessage.Update:
							TempData["OperationType"] = "success";
							TempData["OperationMessage"] = ErrorMessage.ImageUpdated;
							return RedirectToAction("Index");
						case CommonResultsMessage.Fail:
							TempData["OperationType"] = "fail";
							TempData["OperationMessage"] = ErrorMessage.TryLater;
							break;
						case CommonResultsMessage.duplicate:
							TempData["OperationType"] = "fail";
							TempData["OperationMessage"] = ErrorMessage.DuplicateImage;
							break;
						default:
							break;
					}
				}
			}
            
            objModel = InitialiseImageObject(objModel);
            return View(objModel);
        }

        /// <summary>
        /// Open image detail page in Edit mode
        /// </summary>
        /// <param name="ID">Video Id</param>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Images)]
        public ActionResult Edit(Int32 ID)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Images);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            var objmodel = _balImage.GetImageByID(ID);
            objmodel = InitialiseImageObject(objmodel);
            return View("Add", objmodel);
        }


        /// <summary>
        /// Make soft delete of Image
        /// </summary>
        /// <param name="ID">Image Id</param>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Images)]
        public ActionResult Delete(Int32 ID)
        {
            var userData = Session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.Images);
            if (permissionType == Models.Enums.PermissionType.Read)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            //  var result = new JsonResult();
            var deleteresult = _balImage.Delete(ID);
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

        /// <summary>
        /// Retrieve Video file detail
        /// </summary>
        /// <param name="ID">Video Id</param>
        /// <returns></returns>
        [HttpGet]
        [HasPermissionFilter(PermissionCode = Permission.Images)]
        public JsonResult ShowImage(Int32 ID)
        {
            try
            {
                var objModel = _balImage.GetImageByID(ID);
                var userData = Session["UserData"] as UserData;
                if (objModel.IsActive || userData.RoleCode == RoleTypes.ClientAdmin)
                {
                    var imageUrl = System.Web.Configuration.WebConfigurationManager.AppSettings["AllImages"].ToString().Trim().Remove(0,1) +objModel.Id + "//" + objModel.ImageName;
                    objModel.ImageServerPath = Server.MapPath(System.Web.Configuration.WebConfigurationManager.AppSettings["AllImages"].ToString().Trim()) + objModel.Id + "\\" + objModel.ImageName;
                    objModel.Image1 = System.IO.File.ReadAllBytes(objModel.ImageServerPath);
                    Response.Cache.SetExpires(DateTime.Now.AddHours(1));
                    Response.Cache.SetCacheability(HttpCacheability.Public);
                    return Json(imageUrl, JsonRequestBehavior.AllowGet);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

}