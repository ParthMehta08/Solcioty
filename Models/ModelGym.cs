using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace Models
{
    public class ModelGym
    {
        public ModelGym()
        {
            GymList = new List<ModelGym>();
            Owners = new List<ModelUser>();
            GymLocations = new List<GymLocationMapping>();
        }
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDates { get; set; }
        public bool IsDeleted { get; set; }
        public IList<ModelUser> Owners { get; set; }
        public IList<ModelGym> GymList{get;set;}

        public IList<GymLocationMapping> GymLocations { get; set; }

        public bool IsActive
        {
            get
            {
                bool t = false;
                if (!string.IsNullOrEmpty(this.IsActiveString))
                {
                    t = this.IsActiveString.ToUpper().Trim() == "ON" ? true : false;
                }
                return t;
            }
            set
            {
                if (!string.IsNullOrEmpty(this.IsActiveString))
                {
                    value = (this.IsActiveString.ToUpper().Trim() == "ON" ? true : false);
                }
                value = false;
            }
        }
        public string IsActiveString { get; set; }
    }

    public class ModelGymConfiguration
    {
        public int ClientId { get; set; }

        public string HeaderPrimaryColor { get; set; }

        public string HeaderSecondaryColor { get; set; }

        public string PrimaryText { get; set; }
        public string PrimaryBackground { get; set; }

        public string PrimaryGradientBackground { get; set; }
        public string PrimaryFontColor { get; set; }
        public string AlternateText { get; set; }
        public string AlternateBackground { get; set;}
        public string AlternateGradientBackground { get; set; }
        public string AlternateFontColor { get; set; }

        public string HeaderTextColor { get; set; }
         
        public string TimeText { get; set; }
        public string BlockLineOneText { get; set; }
        public string BlockLineTwoText { get; set; }
        public string GoalText { get; set; }

        public bool IsFooterText { get; set; }
        public string FooterTextColor { get; set; }
        public string FooterText { get; set; }


        public bool? ShowTime
        {
            get
            {
                bool t = false;
                if (!string.IsNullOrEmpty(this.IsActiveString))
                {
                    t = this.IsActiveString.ToUpper().Trim() == "ON" ? true : false;
                }
                return t;
            }
            set
            {
                if (!string.IsNullOrEmpty(this.IsActiveString))
                {
                    value = (this.IsActiveString.ToUpper().Trim() == "ON" ? true : false);
                }
                value = false;
            }
        }
        public HttpPostedFileBase Logo { get; set; }
        public string LogoName { get; set; }
		public string LogoServerPath { get; set; }
		public HttpPostedFileBase AlternateLogo { get; set; }
		public string AlternateLogoName { get; set; }
		public string AlternateLogoServerPath { get; set; }
		public string SignoutBackground { get; set; }
		public string VideoTitleColor { get; set; }
		public string VideoTitleBackgroundColor { get; set; }
		public string IsActiveString { get; set; }
        public int SliderCount { get; set; }
        public HttpPostedFileBase BackgroundImage { get; set; }
        public string BackgroundImageName { get; set; }
        public string BackgroundImageServerPath { get; set; }
    }

    public class ModelCopyGym
    {
        public ModelCopyGym()
        {
            CopyActions = new List<SelectListItem>();
            CopyActions.Add(new SelectListItem { Text="Entire Site",Value="1"});
            CopyActions.Add(new SelectListItem { Text="Branding",Value="2"});
            CopyActions.Add(new SelectListItem { Text="Tags",Value="3"});
            CopyActions.Add(new SelectListItem { Text="Videos",Value="4"});
            CopyActions.Add(new SelectListItem { Text="Workouts & Blocks",Value="5"});
        }
        public int Id { get; set; }
        [Required(ErrorMessage = "Client name is required",AllowEmptyStrings =false)]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        public string Name { get; set; }
        public int UserId { get; set; }

        [Required(ErrorMessage="First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [Remote("CheckEmailExistForCopy", "User", ErrorMessage = "Email Already Exist")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [Remote("CheckUsernameExistForCopy", "User", ErrorMessage = "Username Already Exist")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Action is required")]
        public string[] CopyAction { get; set; }

        public IList<SelectListItem> CopyActions { get; set; }

        public string strCopyAction { get; set; }
    }

    
}
