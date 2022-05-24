using BusinessLayer;
using Models;
using Models.Enums;
using Services;
using System;
using System.Web.Mvc;

namespace Solcioty.Controllers
{
    public class AccountController : BaseController
    {
        // GET: Account
        private BalUser _balUser;
        private BalLocation _balLocation;
        private BalGym _balGym;
        public AccountController()
        {
            _balUser = new BalUser();
            _balLocation = new BalLocation();
            _balGym = new BalGym();
        }
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Open Login Page
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {           
            var result = Session["UserData"] as UserData;
            if (result != null)
            {
                if (result.RoleCode == RoleTypes.SuperAdmin)
                    return RedirectToAction("GymList", "Gym");
                else if (result.RoleCode == RoleTypes.ClientAdmin)
                    return RedirectToAction("branch", "gym", new { ID = result.ClientId });
                else if (result.RoleCode == RoleTypes.BranchAdmin)
                    return RedirectToAction("managebranch", "gym", new { ID = result.BranchId });
                else
                    return RedirectToAction("Index", "Home");
            }

            return View();
        }

        /// <summary>
        /// Authenticate User detail and Authorise User
        /// </summary>
        /// <param name="loginRequest">Login Detail</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(ModelLogin loginRequest)
        {

            if (loginRequest != null && ModelState.IsValid)
            {
                var result = _balUser.AuthenticateUser(loginRequest);
                if (result != null && result.IsSuccess)
                {
                    Session["UserData"] = result;

                    if (result.RoleCode == RoleTypes.SuperAdmin)
                        return RedirectToAction("GymList", "Gym");
                    else if (result.RoleCode == RoleTypes.ClientAdmin)
                        return RedirectToAction("branch", "gym", new { ID = result.ClientId });
                    else if (result.RoleCode == RoleTypes.BranchAdmin)
                        return RedirectToAction("managebranch", "gym", new { ID = result.BranchId });
                    else
                        return RedirectToAction("Index", "Home");
                }
                else
                {

                    ViewBag.OperationType = "fail";
                    ViewBag.OperationMessage = result.Message;
                }

            }
            return View();
        }

        //public ActionResult AlterDb()
        //{
        //    var result =_balUser.RunDbScript();
        //    return Json(result,JsonRequestBehavior.AllowGet);
        //}
        public JsonResult SetAdminCurrentLocation(int LocationId)
        {

            var result = Session["UserData"] as UserData;
            if (result != null && result.RoleCode == RoleTypes.BranchAdmin)
            {
                var location = _balLocation.GetByID(LocationId);
                if (location != null)
                {
                    result.BranchId = LocationId;
                    Session["UserData"] = result;
                }
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Clear Logged in user's Session data and Redirect user to Login page
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        public ActionResult ChangePassword()
        {
            var userData = Session["UserData"] as UserData;
            if (userData.RoleCode == RoleTypes.BranchInstructor)
            {
                var homePageData = new ModelHomePage();
                homePageData.ClientConfiguration = _balGym.GetClientConfiguration(Convert.ToInt32(userData.ClientId));
                ViewBag.SignoutColor = homePageData.ClientConfiguration.SignoutBackground;
                ViewBag.SignoutFontColor = homePageData.ClientConfiguration.PrimaryFontColor;
                ViewBag.PrimaryBackgroundColor = homePageData.ClientConfiguration.PrimaryBackground;
                ViewBag.PrimaryText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.PrimaryText) ? string.Join("<br>", homePageData.ClientConfiguration.PrimaryText.ToCharArray()) : "P<br>R<br>I<br>M<br>A<br>R<br>Y";
                ViewBag.AlternateText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateText) ? string.Join("<br>", homePageData.ClientConfiguration.AlternateText.ToCharArray()) : "A<br>L<br>T<br>E<br>R<br>N<br>A<br>T<br>E";
                ViewBag.VideoTitleColor = homePageData.ClientConfiguration.VideoTitleColor;
                if (!string.IsNullOrEmpty(homePageData.ClientConfiguration.LogoName))
                {
                    ViewBag.ClientLogo = $"{homePageData.ClientConfiguration.ClientId}/{homePageData.ClientConfiguration.LogoName}";
                }
                if (!string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateLogoName))
                {
                    ViewBag.ClientAlternateLogo = $"{homePageData.ClientConfiguration.ClientId}/{homePageData.ClientConfiguration.AlternateLogoName}";
                }
                ViewBag.ShowTime = homePageData.ClientConfiguration.ShowTime;
            }
            //var workouts = _balWorkout.GetTodayWorkoutForLocation(userData);

            var model = new ModelUserPassword();
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangePassword(ModelUserPassword model)
        {
            var result = new ModelUserPasswordResponse();
            try
            {
                var userData = Session["userdata"] as UserData;
                if (userData.RoleCode == RoleTypes.BranchInstructor)
                {
                    var homePageData = new ModelHomePage();
                    homePageData.ClientConfiguration = _balGym.GetClientConfiguration(Convert.ToInt32(userData.ClientId));
                    ViewBag.SignoutColor = homePageData.ClientConfiguration.SignoutBackground;
                    ViewBag.SignoutFontColor = homePageData.ClientConfiguration.PrimaryFontColor;
                    ViewBag.PrimaryText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.PrimaryText) ? string.Join("<br>", homePageData.ClientConfiguration.PrimaryText.ToCharArray()) : "P<br>R<br>I<br>M<br>A<br>R<br>Y";
                    ViewBag.AlternateText = !string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateText) ? string.Join("<br>", homePageData.ClientConfiguration.AlternateText.ToCharArray()) : "A<br>L<br>T<br>E<br>R<br>N<br>A<br>T<br>E";
                    ViewBag.VideoTitleColor = homePageData.ClientConfiguration.VideoTitleColor;

                    if (!string.IsNullOrEmpty(homePageData.ClientConfiguration.LogoName))
                    {
                        ViewBag.ClientLogo = $"{homePageData.ClientConfiguration.ClientId}/{homePageData.ClientConfiguration.LogoName}";
                    }
                    if (!string.IsNullOrEmpty(homePageData.ClientConfiguration.AlternateLogoName))
                    {
                        ViewBag.ClientAlternateLogo = $"{homePageData.ClientConfiguration.ClientId}/{homePageData.ClientConfiguration.AlternateLogoName}";
                    }
                    ViewBag.ShowTime = homePageData.ClientConfiguration.ShowTime;
                }
                model.UserId = userData.UserId;
                result = _balUser.ChangePassword(model);
                ViewBag.OperationType = result.IsSuccess ? "success" : "fail";
                ViewBag.OperationMessage = result.Message;
            }
            catch (Exception ex)
            {
                ViewBag.OperationType = "fail";
                ViewBag.OperationMessage = result.Message;
                return View(model);
            }
            return View(model);
        }

        public ActionResult ForgotPassword()
        {
            var model = new ForgorPasswordRequestModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgorPasswordRequestModel model)
        {
            if (model != null && !string.IsNullOrEmpty(model.Email))
            {
                var result = _balUser.CheckUserExist(model.Email);
                if (result)
                {
                    var resetLinkSent = _balUser.SendResetPasswordLink(model);
                    if (resetLinkSent)
                    {
                        ViewBag.OperationType = "success";
                        ViewBag.OperationMessage = "Reset password link sent to your registered email. Please check your inbox.";
                    }
                    else
                    {
                        ViewBag.OperationType = "fail";
                        ViewBag.OperationMessage = "Something went wrong.";
                    }
                }
                else
                {
                    ViewBag.OperationType = "fail";
                    ViewBag.OperationMessage = "Email id does not exist.";
                }
            }
            else
            {
                ViewBag.OperationType = "fail";
                ViewBag.OperationMessage = "Please enter email id.";
            }
            return View();
        }

        [HttpGet]
        public ActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("ForgotPassword");
            }
            var isTokenExpired = _balUser.CheckResetPasswordTokenExpired(token);
            if (isTokenExpired)
            {
                ViewBag.OperationType = "fail";
                ViewBag.OperationMessage = "Your reset password link expired.";
            }
            var model = new ForgotPasswordModel()
            {
                Token = token
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult ResetPassword(ForgotPasswordModel model)
        {
            if (model == null)
            {
                ViewBag.OperationType = "fail";
                ViewBag.OperationMessage = "Invalid request.";
            }
            else
            {
                var isReset = _balUser.ResetPassword(model);
                if (!isReset)
                {
                    ViewBag.OperationType = "fail";
                    ViewBag.OperationMessage = "Invalid request.";
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }

            return View(model);
        }

        /// <summary>
        /// Redirect user to Unauthorized page if user doesn't have permission.
        /// </summary>
        /// <returns></returns>
        public ActionResult Unauthorized()
        {
            return View();
        }

    }
}