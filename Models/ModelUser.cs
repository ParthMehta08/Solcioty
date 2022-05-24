using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class ModelUser
    {
        public ModelUser()
        {
            StateList = new List<ModelState>();
            CityList = new List<ModelCity>();
        }
        public int UserId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string EmailId { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }

        [Required]
        public string ZipCode { get; set; }

        public string Password { get; set; }
        public bool IsDeleted { get; set; }
        public int CityId { get; set; }

        public string CityName { get; set; }

        public string StateCode { get; set; }
        public string Username { get; set; }
        public string SaltKey { get; set; }
        public int RoleId { get; set; }

        public string RoleName { get; set; }
        public int WorkOutType { get; set; } //pm

        public IList<ModelState> StateList { get; set; }
        public IList<ModelCity> CityList { get; set; }


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

    public class ModelRole
    {
        public ModelRole()
        {
            Permissions = new List<ModelPermission>();
            PermissionMappings = new List<ModelRolePermissionMapping>();
        }
        public int Id { get; set; }
        public string UDC { get; set; }
        public string DisplayName { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public IList<ModelPermission> Permissions { get; set; }
        public IList<ModelRolePermissionMapping> PermissionMappings { get; set; }
    }

    public class ModelPermission
    {
        public int Id { get; set; }
        public string PermissionName { get; set; }
        public string PermissionCode { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class ModelRolePermissionMapping
    {
        public int MappingId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string UDC { get; set; }
        public string PermissionName { get; set; }

        public int PermissionType { get; set; }
        public int PermissionId { get; set; }

        public string PermissionCode { get; set; }
        public bool IsActive { get; set; }

    }

    public class ModelUserPassword
    {
        public int UserId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

    }

    public class ModelUserPasswordResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class ModelRolePermissionRequest
    {
        public string RoleCode { get; set; }
        public string PermissionCode { get; set; }
        public int PermissionType { get; set; }
    }


    public class ForgotPasswordModel
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }

    public class ForgorPasswordRequestModel
    {
        public string Email { get; set; }
    }

    public class ModelBranchAdmin
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ModelLogin
    {

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }

    public class LoginResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

    }

    public class UserData : LoginResult
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Nullable<int> WorkOutType { get; set; }
        public string RoleCode { get; set; }
        public int? BranchId { get; set; }
        public int? ClientId { get; set; } //Gym Id

        public int ClientOwnerId { get; set; }

        public List<ModelLocation> AdminLocations { get; set; }
    }


    public class ModelClientUser
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RoleName { get; set; }
        public string LocationName { get; set; }
        public int? LocationId { get; set; }
        public int? PermissionType { get; set; }
    }


    public class ModelClientUserForWorkout
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RoleName { get; set; }
        public string LocationName { get; set; }
        public int? LocationId { get; set; }
        public bool IsChecked { get; set; }
    }

    public class ModelClientUserForBoxerWorkout
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RoleName { get; set; }
        public string LocationName { get; set; }
        public int? LocationId { get; set; }
        public bool IsChecked { get; set; }
    }
}
