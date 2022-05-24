using BusinessLayer;
using Models;
using Models.Enums;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web;

namespace Solcioty.Controllers
{

    public class HomeController : Controller
    {
        private BALVideo _balVideo;
        private BALTemplate _balTemplate;

        private BALWorkout _balWorkout;
        private BalGym _balGym;
        private BalTrainingPortal _balTrainingPortal;
        private BalUser _balUser;
        public HomeController()
        {
            _balVideo = new BALVideo();
            _balTemplate = new BALTemplate();

            _balWorkout = new BALWorkout();
            _balGym = new BalGym();
            _balTrainingPortal = new BalTrainingPortal();
            _balUser = new BalUser();
        }

        [SessionTimeoutFilterAttribute]
        // GET: Home
        public ActionResult Index()
        {
            var userData = Session["UserData"] as UserData;
            userData.WorkOutType = userData.WorkOutType == null ? 2 : userData.WorkOutType;
            if (userData.RoleCode != RoleTypes.BranchInstructor && userData.RoleCode != RoleTypes.Client)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            if(userData.WorkOutType == 1)
            {
                var workouts = _balWorkout.GetTodayWorkoutForLocation(userData);
                var homePageData = new ModelHomePage();
                homePageData.ClientConfiguration = _balGym.GetClientConfiguration(Convert.ToInt32(userData.ClientId));
                ViewBag.SignoutColor = homePageData.ClientConfiguration.SignoutBackground == null ? "#FFFFF" : homePageData.ClientConfiguration.SignoutBackground;
                ViewBag.SignoutFontColor = homePageData.ClientConfiguration.PrimaryFontColor;
                ViewBag.VideoTitleColor = homePageData.ClientConfiguration.VideoTitleColor;
                if (!string.IsNullOrEmpty(homePageData.ClientConfiguration.LogoName))
                {
                    ViewBag.ClientLogo = $"{homePageData.ClientConfiguration.ClientId}/{homePageData.ClientConfiguration.LogoName}";
                }
                if (!string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateLogoName))
                {
                    ViewBag.ClientAlternateLogo = $"{homePageData.ClientConfiguration.ClientId}/{homePageData.ClientConfiguration.AlternateLogoName}";
                }
                if (workouts.Count == 1)
                {
                    //    var workoutDetail = new ModelWorkout();
                    //    workoutDetail = workouts.FirstOrDefault();
                    //    var workoutDetailModel = new WorkoutDetailModel();
                    //    workoutDetailModel = _balWorkout.GetWorkoutTemplateDetail(workoutDetail.ID);
                    //    workoutDetailModel.Templates = workoutDetailModel.Templates.Where(e => e.IsSelected == true).ToList();
                    //    homePageData.WorkoutInfo = workoutDetailModel;
                }
                // ViewBag.ShowTime = homePageData.ClientConfiguration.ShowTime;
                homePageData.WorkoutList = workouts;
                homePageData.FutureWorkoutList = _balWorkout.GetFutureWorkoutForLocation(userData);
                homePageData.IsTrainingPortalAccessible = _balGym.CheckTrainingPortalAccessibleByUser(userData.UserId);

                return View(homePageData);
            }
            else if(userData.WorkOutType == 2)
            {
                var workouts = _balWorkout.GetTodayBoxerWorkoutForLocation(userData);
                var homePageData = new ModelBoxerHomePage();
                homePageData.ClientConfiguration = _balGym.GetClientConfiguration(Convert.ToInt32(userData.ClientId));
                ViewBag.SignoutColor = homePageData.ClientConfiguration.SignoutBackground == null ? "#FFFFF" : homePageData.ClientConfiguration.SignoutBackground;
                ViewBag.SignoutFontColor = homePageData.ClientConfiguration.PrimaryFontColor;
                ViewBag.VideoTitleColor = homePageData.ClientConfiguration.VideoTitleColor;
                if (!string.IsNullOrEmpty(homePageData.ClientConfiguration.LogoName))
                {
                    ViewBag.ClientLogo = $"{homePageData.ClientConfiguration.ClientId}/{homePageData.ClientConfiguration.LogoName}";
                }
                if (!string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateLogoName))
                {
                    ViewBag.ClientAlternateLogo = $"{homePageData.ClientConfiguration.ClientId}/{homePageData.ClientConfiguration.AlternateLogoName}";
                }
                if (workouts.Count == 1)
                {
                    //    var workoutDetail = new ModelWorkout();
                    //    workoutDetail = workouts.FirstOrDefault();
                    //    var workoutDetailModel = new WorkoutDetailModel();
                    //    workoutDetailModel = _balWorkout.GetWorkoutTemplateDetail(workoutDetail.ID);
                    //    workoutDetailModel.Templates = workoutDetailModel.Templates.Where(e => e.IsSelected == true).ToList();
                    //    homePageData.WorkoutInfo = workoutDetailModel;
                }
                // ViewBag.ShowTime = homePageData.ClientConfiguration.ShowTime;
                homePageData.BoxerWorkoutList = workouts;
                homePageData.BoxerTempalteList = _balTemplate.GetBoxerAllTemplates(userData.UserId, ForAdmin: true);
                homePageData.IsTrainingPortalAccessible = _balGym.CheckTrainingPortalAccessibleByUser(userData.UserId);

                return View("IndexBoxer",homePageData);
            }
            return View();
        }

        [SessionTimeoutFilterAttribute]
        [HttpPost]
        public ActionResult Index(int ID)
        {
            var userData = Session["UserData"] as UserData;
            if(userData.WorkOutType == 1)
            {
                ViewBag.WorkoutId = ID;
                if (userData.RoleCode != RoleTypes.BranchInstructor && userData.RoleCode != RoleTypes.Client)
                {
                    return RedirectToAction("Unauthorized", "Account");
                }
                var workouts = _balWorkout.GetTodayWorkoutForLocation(userData);
                var workoutDetail = workouts.FirstOrDefault(e => e.ID == ID);
                var homePageData = new ModelHomePage();
                homePageData.ClientConfiguration = _balGym.GetClientConfiguration(Convert.ToInt32(userData.ClientId));
                ViewBag.SignoutColor = homePageData.ClientConfiguration.SignoutBackground;
                ViewBag.SignoutFontColor = homePageData.ClientConfiguration.PrimaryFontColor;
                //ViewBag.PrimaryText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.PrimaryText) ? string.Join("<br>", homePageData.ClientConfiguration.PrimaryText.ToCharArray()) : "P<br>R<br>I<br>M<br>A<br>R<br>Y";
                //ViewBag.AlternateText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateText) ? string.Join("<br>", homePageData.ClientConfiguration.AlternateText.ToCharArray()) : "A<br>L<br>T<br>E<br>R<br>N<br>A<br>T<br>E";
                ViewBag.PrimaryText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.PrimaryText) ? "" : homePageData.ClientConfiguration.PrimaryText;
                ViewBag.AlternateText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateText) ? "" : homePageData.ClientConfiguration.AlternateText;

                ViewBag.VideoTitleColor = homePageData.ClientConfiguration.VideoTitleColor;
                if (!string.IsNullOrEmpty(homePageData.ClientConfiguration.LogoName))
                {
                    ViewBag.ClientLogo = $"{homePageData.ClientConfiguration.ClientId}/{homePageData.ClientConfiguration.LogoName}";
                }
                if (!string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateLogoName))
                {
                    ViewBag.ClientAlternateLogo = $"{homePageData.ClientConfiguration.ClientId}/{homePageData.ClientConfiguration.AlternateLogoName}";
                }
                if (workouts.Count > 0 && workoutDetail != null)
                {
                    var workoutDetailModel = new WorkoutDetailModel();
                    workoutDetailModel = _balWorkout.GetWorkoutTemplateDetail(workoutDetail.ID);
                    workoutDetailModel.Templates = workoutDetailModel.Templates.Where(e => e.IsSelected == true).ToList();
                    workoutDetailModel.IsFutureWorkout = workoutDetail.IsFutureWorkout;
                    ViewBag.IsFutureWorkout = workoutDetailModel.IsFutureWorkout;

                    homePageData.WorkoutInfo = workoutDetailModel;
                }
                ViewBag.ShowTime = homePageData.ClientConfiguration.ShowTime;
                homePageData.IsTrainingPortalAccessible = _balGym.CheckTrainingPortalAccessibleByUser(userData.UserId);
                // return View(homePageData);
                return View("Index", homePageData);
            }
            else if(userData.WorkOutType == 2)
            {
                ViewBag.BoxerWorkoutId = ID;
                if (userData.RoleCode != RoleTypes.BranchInstructor && userData.RoleCode != RoleTypes.Client)
                {
                    return RedirectToAction("Unauthorized", "Account");
                }
                var workouts = _balWorkout.GetTodayBoxerWorkoutForLocation(userData);
                var workoutDetail = workouts.FirstOrDefault(e => e.ID == ID);
                var homePageData = new ModelBoxerHomePage();
                homePageData.ClientConfiguration = _balGym.GetClientConfiguration(Convert.ToInt32(userData.ClientId));
                ViewBag.SignoutColor = homePageData.ClientConfiguration.SignoutBackground;
                ViewBag.SignoutFontColor = homePageData.ClientConfiguration.PrimaryFontColor;
                //ViewBag.PrimaryText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.PrimaryText) ? string.Join("<br>", homePageData.ClientConfiguration.PrimaryText.ToCharArray()) : "P<br>R<br>I<br>M<br>A<br>R<br>Y";
                //ViewBag.AlternateText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateText) ? string.Join("<br>", homePageData.ClientConfiguration.AlternateText.ToCharArray()) : "A<br>L<br>T<br>E<br>R<br>N<br>A<br>T<br>E";
                ViewBag.PrimaryText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.PrimaryText) ? "" : homePageData.ClientConfiguration.PrimaryText;
                ViewBag.AlternateText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateText) ? "" : homePageData.ClientConfiguration.AlternateText;

                ViewBag.VideoTitleColor = homePageData.ClientConfiguration.VideoTitleColor;
                if (!string.IsNullOrEmpty(homePageData.ClientConfiguration.LogoName))
                {
                    ViewBag.ClientLogo = $"{homePageData.ClientConfiguration.ClientId}/{homePageData.ClientConfiguration.LogoName}";
                }
                if (!string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateLogoName))
                {
                    ViewBag.ClientAlternateLogo = $"{homePageData.ClientConfiguration.ClientId}/{homePageData.ClientConfiguration.AlternateLogoName}";
                }
                if (workouts.Count > 0 && workoutDetail != null)
                {
                    var workoutDetailModel = new BoxerWorkoutDetailModel();
                    workoutDetailModel = _balWorkout.GetWorkoutTemplateDetailBoxer(workoutDetail.ID);
                    workoutDetailModel.BoxerTemplates = workoutDetailModel.BoxerTemplates.Where(e => e.IsSelected == true).ToList();
                    workoutDetailModel.IsFutureWorkout = workoutDetail.IsFutureWorkout;
                    ViewBag.IsFutureWorkout = workoutDetailModel.IsFutureWorkout;

                    homePageData.BoxerWorkoutInfo = workoutDetailModel;
                }
                ViewBag.ShowTime = homePageData.ClientConfiguration.ShowTime;
                homePageData.IsTrainingPortalAccessible = _balGym.CheckTrainingPortalAccessibleByUser(userData.UserId);
                // return View(homePageData);
                return View("IndexBoxer", homePageData);
            }
            return View();
        }

        [SessionTimeoutFilterAttribute]
        public ActionResult Preview(int workoutId)
        {
            var userData = Session["UserData"] as UserData;

            if (userData.RoleCode == RoleTypes.Client)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            //var workouts = _balWorkout.GetTodayWorkoutForLocation(userData);
            var homePageData = new ModelHomePage();
            homePageData.ClientConfiguration = _balGym.GetClientConfiguration(Convert.ToInt32(userData.ClientId));
            ViewBag.SignoutColor = homePageData.ClientConfiguration.SignoutBackground;
            ViewBag.SignoutFontColor = homePageData.ClientConfiguration.PrimaryFontColor;
            //   ViewBag.PrimaryText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.PrimaryText) ? string.Join("<br>", homePageData.ClientConfiguration.PrimaryText.ToCharArray()) : "P<br>R<br>I<br>M<br>A<br>R<br>Y";

            ViewBag.PrimaryText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.PrimaryText) ? "" : homePageData.ClientConfiguration.PrimaryText;
            ViewBag.AlternateText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateText) ? "" : homePageData.ClientConfiguration.AlternateText;

            //ViewBag.AlternateText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateText) ? string.Join("<br>", homePageData.ClientConfiguration.AlternateText.ToCharArray()) : "A<br>L<br>T<br>E<br>R<br>N<br>A<br>T<br>E";
            ViewBag.VideoTitleColor = homePageData.ClientConfiguration.VideoTitleColor;
            if (!string.IsNullOrEmpty(homePageData.ClientConfiguration.LogoName))
            {
                ViewBag.ClientLogo = $"{homePageData.ClientConfiguration.ClientId}/{homePageData.ClientConfiguration.LogoName}";
            }
            if (!string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateLogoName))
            {
                ViewBag.ClientAlternateLogo = $"{homePageData.ClientConfiguration.ClientId}/{homePageData.ClientConfiguration.AlternateLogoName}";
            }
            //if (workouts.Count == 1)
            //{
            //    var workoutDetail = new ModelWorkout();
            //    workoutDetail = workouts.FirstOrDefault();
            var workoutDetailModel = new WorkoutDetailModel();
            workoutDetailModel = _balWorkout.GetWorkoutTemplateDetail(workoutId);
            if (workoutDetailModel.Templates != null)
            {
                workoutDetailModel.Templates = workoutDetailModel.Templates.Where(e => e.IsSelected == true).ToList();
            }

            homePageData.WorkoutInfo = workoutDetailModel;
            //}
            ViewBag.ShowTime = homePageData.ClientConfiguration.ShowTime;
            homePageData.IsTrainingPortalAccessible = _balGym.CheckTrainingPortalAccessibleByUser(userData.UserId);
            // homePageData.WorkoutList = workouts;
            // homePageData.FutureWorkoutList = _balWorkout.GetFutureWorkoutForLocation(userData);
            return View(homePageData);
        }

        [SessionTimeoutFilterAttribute]
        public ActionResult PreviewBoxer(int boxerWorkoutId)
        {
            var userData = Session["UserData"] as UserData;

            if (userData.RoleCode == RoleTypes.Client)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            //var workouts = _balWorkout.GetTodayWorkoutForLocation(userData);
            var homePageData = new ModelBoxerHomePage();
            homePageData.ClientConfiguration = _balGym.GetClientConfiguration(Convert.ToInt32(userData.ClientId));
            ViewBag.SignoutColor = homePageData.ClientConfiguration.SignoutBackground;
            ViewBag.SignoutFontColor = homePageData.ClientConfiguration.PrimaryFontColor;
            //   ViewBag.PrimaryText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.PrimaryText) ? string.Join("<br>", homePageData.ClientConfiguration.PrimaryText.ToCharArray()) : "P<br>R<br>I<br>M<br>A<br>R<br>Y";

            ViewBag.PrimaryText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.PrimaryText) ? "" : homePageData.ClientConfiguration.PrimaryText;
            ViewBag.AlternateText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateText) ? "" : homePageData.ClientConfiguration.AlternateText;

            //ViewBag.AlternateText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateText) ? string.Join("<br>", homePageData.ClientConfiguration.AlternateText.ToCharArray()) : "A<br>L<br>T<br>E<br>R<br>N<br>A<br>T<br>E";
            ViewBag.VideoTitleColor = homePageData.ClientConfiguration.VideoTitleColor;
            if (!string.IsNullOrEmpty(homePageData.ClientConfiguration.LogoName))
            {
                ViewBag.ClientLogo = $"{homePageData.ClientConfiguration.ClientId}/{homePageData.ClientConfiguration.LogoName}";
            }
            if (!string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateLogoName))
            {
                ViewBag.ClientAlternateLogo = $"{homePageData.ClientConfiguration.ClientId}/{homePageData.ClientConfiguration.AlternateLogoName}";
            }
            //if (workouts.Count == 1)
            //{
            //    var workoutDetail = new ModelWorkout();
            //    workoutDetail = workouts.FirstOrDefault();
            var workoutDetailModel = new BoxerWorkoutDetailModel();
            workoutDetailModel = _balWorkout.GetWorkoutTemplateDetailBoxer(boxerWorkoutId);
            if (workoutDetailModel.BoxerTemplates != null)
            {
                workoutDetailModel.BoxerTemplates = workoutDetailModel.BoxerTemplates.Where(e => e.IsSelected == true).ToList();
            }

            homePageData.BoxerWorkoutInfo = workoutDetailModel;
            //}
            ViewBag.ShowTime = homePageData.ClientConfiguration.ShowTime;
            homePageData.IsTrainingPortalAccessible = _balGym.CheckTrainingPortalAccessibleByUser(userData.UserId);
            // homePageData.WorkoutList = workouts;
            // homePageData.FutureWorkoutList = _balWorkout.GetFutureWorkoutForLocation(userData);
            return View(homePageData);
        }

        public ActionResult Share(int workoutId, int userid, int clientid)
        {
            var UserId = userid;
            var ClientId = clientid;
            //var workouts = _balWorkout.GetTodayWorkoutForLocation(userData);
            var homePageData = new ModelHomePage();
            homePageData.ClientConfiguration = _balGym.GetClientConfiguration(Convert.ToInt32(ClientId));
            ViewBag.SignoutColor = homePageData.ClientConfiguration.SignoutBackground;
            ViewBag.SignoutFontColor = homePageData.ClientConfiguration.PrimaryFontColor;
            //   ViewBag.PrimaryText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.PrimaryText) ? string.Join("<br>", homePageData.ClientConfiguration.PrimaryText.ToCharArray()) : "P<br>R<br>I<br>M<br>A<br>R<br>Y";

            ViewBag.PrimaryText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.PrimaryText) ? "" : homePageData.ClientConfiguration.PrimaryText;
            ViewBag.AlternateText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateText) ? "" : homePageData.ClientConfiguration.AlternateText;

            //ViewBag.AlternateText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateText) ? string.Join("<br>", homePageData.ClientConfiguration.AlternateText.ToCharArray()) : "A<br>L<br>T<br>E<br>R<br>N<br>A<br>T<br>E";
            ViewBag.VideoTitleColor = homePageData.ClientConfiguration.VideoTitleColor;
            if (!string.IsNullOrEmpty(homePageData.ClientConfiguration.LogoName))
            {
                ViewBag.ClientLogo = $"{homePageData.ClientConfiguration.ClientId}/{homePageData.ClientConfiguration.LogoName}";
            }
            if (!string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateLogoName))
            {
                ViewBag.ClientAlternateLogo = $"{homePageData.ClientConfiguration.ClientId}/{homePageData.ClientConfiguration.AlternateLogoName}";
            }
            //if (workouts.Count == 1)
            //{
            //    var workoutDetail = new ModelWorkout();
            //    workoutDetail = workouts.FirstOrDefault();
            var workoutDetailModel = new WorkoutDetailModel();
            workoutDetailModel = _balWorkout.GetWorkoutTemplateDetail(workoutId);
            if (workoutDetailModel.Templates != null)
            {
                workoutDetailModel.Templates = workoutDetailModel.Templates.Where(e => e.IsSelected == true).ToList();
            }

            homePageData.WorkoutInfo = workoutDetailModel;
            //}
            ViewBag.ShowTime = homePageData.ClientConfiguration.ShowTime;
            homePageData.IsTrainingPortalAccessible = _balGym.CheckTrainingPortalAccessibleByUser(UserId);
            // homePageData.WorkoutList = workouts;
            // homePageData.FutureWorkoutList = _balWorkout.GetFutureWorkoutForLocation(userData);
            return View(homePageData);
        }

        public ActionResult ShareBoxer(int boxerWorkoutId, int userid, int clientid)
        {
            var UserId = userid;
            var ClientId = clientid;
            //var workouts = _balWorkout.GetTodayWorkoutForLocation(userData);
            var homePageData = new ModelBoxerHomePage();
            homePageData.ClientConfiguration = _balGym.GetClientConfiguration(Convert.ToInt32(ClientId));
            ViewBag.SignoutColor = homePageData.ClientConfiguration.SignoutBackground;
            ViewBag.SignoutFontColor = homePageData.ClientConfiguration.PrimaryFontColor;
            //   ViewBag.PrimaryText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.PrimaryText) ? string.Join("<br>", homePageData.ClientConfiguration.PrimaryText.ToCharArray()) : "P<br>R<br>I<br>M<br>A<br>R<br>Y";

            ViewBag.PrimaryText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.PrimaryText) ? "" : homePageData.ClientConfiguration.PrimaryText;
            ViewBag.AlternateText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateText) ? "" : homePageData.ClientConfiguration.AlternateText;

            //ViewBag.AlternateText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateText) ? string.Join("<br>", homePageData.ClientConfiguration.AlternateText.ToCharArray()) : "A<br>L<br>T<br>E<br>R<br>N<br>A<br>T<br>E";
            ViewBag.VideoTitleColor = homePageData.ClientConfiguration.VideoTitleColor;
            if (!string.IsNullOrEmpty(homePageData.ClientConfiguration.LogoName))
            {
                ViewBag.ClientLogo = $"{homePageData.ClientConfiguration.ClientId}/{homePageData.ClientConfiguration.LogoName}";
            }
            if (!string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateLogoName))
            {
                ViewBag.ClientAlternateLogo = $"{homePageData.ClientConfiguration.ClientId}/{homePageData.ClientConfiguration.AlternateLogoName}";
            }
            //if (workouts.Count == 1)
            //{
            //    var workoutDetail = new ModelWorkout();
            //    workoutDetail = workouts.FirstOrDefault();
            var workoutDetailModel = new BoxerWorkoutDetailModel();
            workoutDetailModel = _balWorkout.GetWorkoutTemplateDetailBoxer(boxerWorkoutId);
            if (workoutDetailModel.BoxerTemplates != null)
            {
                workoutDetailModel.BoxerTemplates = workoutDetailModel.BoxerTemplates.Where(e => e.IsSelected == true).ToList();
            }

            homePageData.BoxerWorkoutInfo = workoutDetailModel;
            //}
            ViewBag.ShowTime = homePageData.ClientConfiguration.ShowTime;
            homePageData.IsTrainingPortalAccessible = _balGym.CheckTrainingPortalAccessibleByUser(UserId);
            // homePageData.WorkoutList = workouts;
            // homePageData.FutureWorkoutList = _balWorkout.GetFutureWorkoutForLocation(userData);
            return View(homePageData);
        }

        [SessionTimeoutFilterAttribute]
        public JsonResult TodaysWorkouts()
        {
            var userData = Session["UserData"] as UserData;

            if (userData.RoleCode != RoleTypes.BranchInstructor && userData.RoleCode != RoleTypes.Client)
            {
                return Json(new { IsSuccess = false, IsAuthorized = false }, JsonRequestBehavior.AllowGet);
            }
            var todaysWorkouts = new List<ModelWorkout>();
            todaysWorkouts = _balWorkout.GetTodayWorkoutForLocation(userData);

            return Json(new { Wokrouts = todaysWorkouts }, JsonRequestBehavior.AllowGet);
        }

        [SessionTimeoutFilterAttribute]
        public ActionResult VideoLibrary()
        {
            ModelVideo objModel = new ModelVideo();
            var userData = Session["UserData"] as UserData;

            if (userData.RoleCode == RoleTypes.Client)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            //var workouts = _balWorkout.GetTodayWorkoutForLocation(userData);
            var homePageData = new ModelHomePage();
            homePageData.ClientConfiguration = _balGym.GetClientConfiguration(Convert.ToInt32(userData.ClientId));
            ViewBag.SignoutColor = homePageData.ClientConfiguration.SignoutBackground;
            ViewBag.SignoutFontColor = homePageData.ClientConfiguration.PrimaryFontColor;
            ViewBag.VideoTitleColor = homePageData.ClientConfiguration.VideoTitleColor;
            if (!string.IsNullOrEmpty(homePageData.ClientConfiguration.LogoName))
            {
                ViewBag.ClientLogo = $"{homePageData.ClientConfiguration.ClientId}/{homePageData.ClientConfiguration.LogoName}";
            }
            if (!string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateLogoName))
            {
                ViewBag.ClientAlternateLogo = $"{homePageData.ClientConfiguration.ClientId}/{homePageData.ClientConfiguration.AlternateLogoName}";
            }
            if (userData != null)
            {
                objModel.VideoList = new List<ModelVideo>();
                objModel.VideoList = _balVideo.GetAllVideos(userData.ClientOwnerId, null, false);
            }
            return View(objModel);
        }

        [SessionTimeoutFilterAttribute]
        public ActionResult TrainingPortal()
        {
            var userData = Session["UserData"] as UserData;
            //var permissionType = AccessbilityService.GetPermission(userData.RoleCode, Permission.TrainingPortal);
            var IsTrainingPortalAccessible = _balGym.CheckTrainingPortalAccessibleByUser(userData.UserId);
            if (!IsTrainingPortalAccessible)
            {
                return RedirectToAction("Unauthorized", "Account");
            }
            var homePageData = new ModelHomePage();
            homePageData.ClientConfiguration = _balGym.GetClientConfiguration(Convert.ToInt32(userData.ClientId));
            ViewBag.VideoTitleColor = homePageData.ClientConfiguration.VideoTitleColor;
            ViewBag.VideoTitleBackgroundColor = homePageData.ClientConfiguration.VideoTitleBackgroundColor;
            if (!string.IsNullOrEmpty(homePageData.ClientConfiguration.LogoName))
            {
                ViewBag.ClientLogo = $"{homePageData.ClientConfiguration.ClientId}/{homePageData.ClientConfiguration.LogoName}";
            }
            if (!string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateLogoName))
            {
                ViewBag.ClientAlternateLogo = $"{homePageData.ClientConfiguration.ClientId}/{homePageData.ClientConfiguration.AlternateLogoName}";
            }
            var userPortal = _balTrainingPortal.GetTrainingPortalByUserId(userData.UserId);
            return View(userPortal);
        }

        public ActionResult CopyVideos(int sourceId, int destinationId)
        {
            CopyVideosModel copyVideosModel = new CopyVideosModel();
           
            if(sourceId > 0 && destinationId > 0)
            {
                var lstInsertedVideos = _balVideo.CopyVideos(sourceId, destinationId);
                if(lstInsertedVideos != null && lstInsertedVideos.Count > 0)
                {
                    copyVideosModel.modelVideos = lstInsertedVideos;
                }
                else
                {
                    copyVideosModel.Message = "Videos are not available.";
                }
                
            }
            else
            {
                copyVideosModel.Message = "Please pass source and destination userid";
            }

            
            return View(copyVideosModel);
        }

    }
}