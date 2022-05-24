using BusinessLayer.Helpers;
using DataAccessLayer;
using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace BusinessLayer
{
    public class BalUser : IDisposable
    {
        private BalState _balState;
        private BalCity _balCity;
        SolciotyNewEntities _dbcontext;
        public BalUser()
        {
            _dbcontext = new SolciotyNewEntities();
            _balState = new BalState();
            _balCity = new BalCity();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #region Methods
        public ModelUser DalToModel(User objDal)
        {
            ModelUser objModel = new ModelUser();
            try
            {
                //objModel.Name = objDal.Name;
                objModel.UserId = objDal.ID;
                objModel.FirstName = objDal.FirstName;
                objModel.LastName = objDal.LastName;
                objModel.Username = objDal.Username;
                objModel.Password = objDal.UserPassword;
                //objModel.Phone = objDal.Phone;
                objModel.EmailId = objDal.EmailID;
                objModel.WorkOutType = Convert.ToInt32(objDal.WorkOutType);
                //objModel.AddressLine1 = objDal.AddressLine1;
                //objModel.AddressLine2 = objDal.AddressLine2;
                //objModel.ZipCode = objDal.ZipCode;
                //objModel.CityId = Convert.ToInt32(objDal.CityId);

                //objModel.StateList = _balState.GetAllStates();
                if (objDal.UserRoleMappings != null && objDal.UserRoleMappings.Count > 0)
                {
                    objModel.RoleId = objDal.UserRoleMappings.FirstOrDefault().Role.ID;
                    objModel.RoleName = objDal.UserRoleMappings.FirstOrDefault().Role.DisplayName;
                }
                //if (objDal.CityId > 0)
                //{
                //    //objModel.CityList = _balCity.GetAllCitiesByStateCode(objDal.City.StateCode);
                //    objModel.StateCode = objDal.City.StateCode;
                //    objModel.CityName = objDal.City.CityName;
                //}
                //else
                //{
                //    // objModel.CityList = _balCity.GetAllCitiesByStateCode(objModel.StateList[0].Abbreviation);
                //    objModel.StateCode = objModel.StateList[0].Abbreviation;
                //}
                objModel.IsActiveString = objDal.IsActive.HasValue ? (objDal.IsActive.Value == true ? "on" : "") : "";
            }
            catch (Exception)
            {
                return null;
            }
            return objModel;
        }

        public ModelUser SaveUser(ModelUser userDetail)
        {
            //var userInfo = new ModelUser();
            try
            {
                var ownerDetail = _dbcontext.Users.FirstOrDefault(u => (u.ID == userDetail.UserId || u.Username == userDetail.Username) && u.IsDeleted != true);
                if (ownerDetail != null)
                {
                    if (!string.IsNullOrEmpty(userDetail.Username))
                    {
                        ownerDetail.FirstName = userDetail.FirstName;
                        ownerDetail.LastName = userDetail.LastName;
                        ownerDetail.IsActive = userDetail.IsActive;
                        ownerDetail.IsDeleted = userDetail.IsDeleted;
                        //ownerDetail.Phone = userDetail.Phone;
                        ownerDetail.EmailID = userDetail.EmailId;
                        ownerDetail.WorkOutType = userDetail.WorkOutType;
                        if (userDetail.Password != ownerDetail.UserPassword)
                        {
                            var encryptedPassword = EncryptionService.CreatePasswordHash(userDetail.Password, ownerDetail.SaltKey);
                            ownerDetail.UserPassword = encryptedPassword;

                        }
                        //ownerDetail.AddressLine1 = userDetail.AddressLine1;
                        //ownerDetail.AddressLine2 = userDetail.AddressLine2;
                        //ownerDetail.ZipCode = userDetail.ZipCode;
                        //ownerDetail.CityId = userDetail.CityId == 0 ? new Nullable<int>():userDetail.CityId;
                        //var cityId = _balCity.GetCityId(userDetail.CityName, userDetail.StateCode);
                        //ownerDetail.CityId = cityId > 0 ? cityId : new Nullable<int>();
                        // ownerDetail.City = userDetail.CityName;
                        ownerDetail.UpdatedDate = DateTime.UtcNow;
                        _dbcontext.SaveChanges();
                    }
                }
                else
                {
                    var saltKey = EncryptionService.CreateSaltKey(6);
                    //var randomPassword = EncryptionService.RandomString(8);
                    var encryptedPassword = EncryptionService.CreatePasswordHash(userDetail.Password, saltKey);
                    //var cityId = _balCity.GetCityId(userDetail.CityName, userDetail.StateCode);
                    var newUser = new User()
                    {
                        Username = userDetail.Username,
                        FirstName = userDetail.FirstName,
                        LastName = userDetail.LastName,
                        //AddressLine1 = userDetail.AddressLine1,
                        //AddressLine2 = userDetail.AddressLine2,
                        //ZipCode = userDetail.ZipCode,
                        EmailID = userDetail.EmailId,
                        WorkOutType = userDetail.WorkOutType,

                        //CityId = cityId > 0 ? cityId : new Nullable<int>(),
                        //City = userDetail.CityName,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow,
                        IsActive = userDetail.IsActive,
                        //Phone = userDetail.Phone,
                        UserPassword = encryptedPassword,
                        SaltKey = saltKey,
                        IsDeleted = false
                    };
                    _dbcontext.Users.Add(newUser);
                    _dbcontext.SaveChanges();
                    userDetail.UserId = newUser.ID;
                    if (!string.IsNullOrEmpty(userDetail.EmailId))
                    {
                        var emailBody = $"Hello !<br/><h2>{newUser.FirstName} {newUser.LastName} </h2><br/><p>You are successfully registered with Solcioty. Your account information are below:</p></br><p>Username: {newUser.Username} <br/>Password: {userDetail.Password}</p>";
                        EmailHelper.SendEmail(userDetail.EmailId, EmailSubject.WelcomeUser, emailBody, true);
                    }
                }
                var userRoleMapping = _dbcontext.UserRoleMappings.FirstOrDefault(e => e.UserID == userDetail.UserId);
                if (userRoleMapping == null)
                {
                    var newUserRoleMapping = new UserRoleMapping()
                    {
                        UserID = userDetail.UserId,
                        RoleID = userDetail.RoleId,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow
                    };
                    _dbcontext.UserRoleMappings.Add(newUserRoleMapping);
                }
                else
                {
                    userRoleMapping.RoleID = userDetail.RoleId;
                    userRoleMapping.UpdatedDate = DateTime.UtcNow;
                }
                _dbcontext.SaveChanges();
                return userDetail;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public CommonResultsMessage AddEdit(ModelUser objmodel)
        {
            try
            {
                //var gymId = objmodel.Id;
                //var olddata = _dbcontext.Gyms.FirstOrDefault(g => g.Id == objmodel.Id);
                //dynamic returnResult;
                //if (olddata != null)
                //{//edit
                //    olddata.Name = objmodel.Name;

                //    _dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
                //    _dbcontext.SaveChanges();
                //    returnResult = CommonResultsMessage.Update;
                //}
                //else
                //{//add
                //    var objDal = new Gym();
                //    objDal.Name = objmodel.Name;
                //    objDal.IsActive = objmodel.IsActive;
                //    objDal.CreatedDate = DateTime.UtcNow;
                //    objDal.UpdatedDate = DateTime.UtcNow;
                //    objDal.IsDeleted = false;
                //    _dbcontext.Gyms.Add(objDal);
                //    _dbcontext.SaveChanges();
                //    gymId = objDal.Id;
                //    returnResult = CommonResultsMessage.Add;
                //}

                //if (objmodel.Owners != null)
                //{
                //    foreach (var owner in objmodel.Owners)
                //    {
                //        var ownerDetail = _dbcontext.Users.FirstOrDefault(u => u.ID == owner.Id);
                //        if (ownerDetail != null)
                //        {
                //            ownerDetail.FirstName = owner.FirstName;
                //            ownerDetail.LastName = owner.LastName;
                //            ownerDetail.IsActive = owner.IsActive;
                //            ownerDetail.Phone = owner.Phone;
                //            ownerDetail.EmailID = owner.EmailId;
                //            ownerDetail.Address = owner.Address;
                //            ownerDetail.CityID = owner.CityId;
                //            _dbcontext.SaveChanges();
                //            var userRoleMap = ownerDetail.UserRoleMappings.FirstOrDefault(ur => ur.RoleID == owner.RoleId);
                //            if (userRoleMap == null)
                //            {
                //                var newUserRoleMap = new UserRoleMapping()
                //                {
                //                    UserID = owner.Id,
                //                    RoleID = owner.RoleId,
                //                    CreatedDate = DateTime.UtcNow,
                //                    UpdatedDate = DateTime.UtcNow,
                //                    IsActive = true

                //                };
                //            }
                //            else
                //            {
                //                userRoleMap.UpdatedDate = DateTime.UtcNow;
                //                userRoleMap.UserID = owner.Id;
                //            }
                //            _dbcontext.SaveChanges();

                //        }
                //        else
                //        {
                //            var newUser = new User()
                //            {
                //                FirstName = owner.FirstName,
                //                LastName = owner.LastName,
                //                Address = owner.Address,
                //                EmailID = owner.EmailId,
                //                CityID = owner.CityId,
                //                CreatedDate = DateTime.UtcNow,
                //                UpdatedDate = DateTime.UtcNow,
                //                IsActive = owner.IsActive,
                //                Phone = owner.Phone,
                //                UserPassword = owner.Password,
                //                SaltKey = owner.SaltKey,
                //                IsDeleted = false
                //            };
                //            _dbcontext.Users.Add(newUser);
                //            _dbcontext.SaveChanges();

                //            var newGymUserMapping = new GymUserMapping()
                //            {
                //                UserId = newUser.ID,
                //                GymId = gymId,
                //                CreatedDate = DateTime.UtcNow,
                //                UpdatedDate = DateTime.UtcNow,
                //                IsActive = true,
                //                IsDeleted = false
                //            };

                //            _dbcontext.GymUserMappings.Add(newGymUserMapping);
                //            _dbcontext.SaveChanges();
                //        }
                //    }
                //}

                return CommonResultsMessage.Fail;
            }
            catch (Exception)
            {
                return CommonResultsMessage.Fail;
            }
        }
        public bool CheckUserDuplication(string emailId)
        {
            try
            {
                var isDuplicate = _dbcontext.Users.Any(g => g.EmailID == emailId);
                return isDuplicate;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool CheckUsernameDuplication(string username)
        {
            try
            {
                var isDuplicate = _dbcontext.Users.Any(g => g.Username == username && g.IsDeleted != true);
                return isDuplicate;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<ModelUser> GetAllUsers()
        {
            IList<ModelUser> allUsers = new List<ModelUser>();
            var userList = new List<User>();
            try
            {
                userList = _dbcontext.Users.ToList();

                foreach (var item in userList)
                    allUsers.Add(DalToModel(item));
            }
            catch (Exception)
            {
                throw;
            }
            return allUsers;
        }


        public IList<ModelUser> GetGymOwners(int gymId)
        {
            IList<ModelUser> allUsers = new List<ModelUser>();
            try
            {
                var gymUserMappings = _dbcontext.GymUserMappings.Include("User").Where(e => e.GymId == gymId && e.IsDeleted == false).ToList();

                foreach (var item in gymUserMappings)
                    allUsers.Add(DalToModel(item.User));
            }
            catch (Exception)
            {
                throw;
            }
            return allUsers;
        }


        public ModelUser GetById(int userId)
        {
            try
            {
                return DalToModel(_dbcontext.Users.Include("City").FirstOrDefault(g => g.ID == userId));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ModelRole> GetAllRoles()
        {
            var roles = new List<ModelRole>();
            try
            {
                roles = _dbcontext.Roles.Select(e => new ModelRole()
                {
                    Id = e.ID,
                    DisplayName = e.DisplayName,
                    IsActive = e.IsActive,
                    RoleName = e.RoleName,
                    UDC = e.UDC
                }).ToList();
            }
            catch (Exception ex)
            {

            }
            return roles;
        }

        public ModelRole GetRoleByCode(string RoleCode)
        {
            var role = new ModelRole();
            try
            {
                role = _dbcontext.Roles.Where(e => e.UDC == RoleCode).Select(e => new ModelRole()
                {
                    Id = e.ID,
                    DisplayName = e.DisplayName,
                    IsActive = e.IsActive,
                    RoleName = e.RoleName,
                    UDC = e.UDC
                }).FirstOrDefault();
            }
            catch (Exception)
            {

                return role;
            }
            return role;
        }

        public bool UpdateRoleStatus(int RoleId, bool Status)
        {
            var role = _dbcontext.Roles.FirstOrDefault(e => e.ID == RoleId);
            if (role != null)
            {
                var allUsers = _dbcontext.UserRoleMappings.Include("User").Where(e => e.RoleID == RoleId).ToList();
                foreach (var userdetail in allUsers)
                {
                    userdetail.User.IsActive = Status;
                }
                role.IsActive = Status;
                _dbcontext.SaveChanges();
                return true;
            }
            return false;
        }

        public List<ModelPermission> GetAllPermissions(bool ForAdmin = false)
        {
            var permissions = new List<ModelPermission>();
            try
            {
                permissions = _dbcontext.Permissions.Select(e => new ModelPermission()
                {
                    Id = e.Id,
                    IsActive = e.IsActive,
                    PermissionCode = e.PermissionCode,
                    PermissionName = e.PermissionName,
                    DisplayOrder = e.DisplayOrder
                }).OrderBy(e => e.DisplayOrder).ToList();
                if (!ForAdmin)
                {
                    permissions = permissions.Where(e => e.IsActive == true).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return permissions;
        }

        public bool UpdatePermissionStatus(int PermissionId, bool Status)
        {
            var permission = _dbcontext.Permissions.FirstOrDefault(e => e.Id == PermissionId);
            if (permission != null)
            {
                permission.IsActive = Status;
                _dbcontext.SaveChanges();
                return true;
            }
            return false;
        }


        public ModelRole GetRolePermissionMapping(string RoleCode)
        {
            var modelRole = new ModelRole();
            modelRole = GetRoleByCode(RoleCode);
            IList<ModelPermission> permissions = new List<ModelPermission>();
            if (RoleCode == RoleTypes.SuperAdmin)
            {
                permissions = GetAllPermissions(true);
            }
            else
            {
                permissions = GetAllPermissions();
            }
            var rolePermissionMappings = _dbcontext.RolePermissionMappings.Where(e => e.RoleCode == RoleCode).Select(e => new ModelRolePermissionMapping()
            {
                MappingId = e.Id,
                PermissionCode = e.PermissionCode,
                //PermissionName = !string.IsNullOrEmpty(e.PermissionCode)?permissions.FirstOrDefault(p=>p.PermissionCode == e.PermissionCode).PermissionName:string.Empty,
                PermissionType = e.PermissionType,
                UDC = e.RoleCode
            }).ToList();
            modelRole.PermissionMappings = rolePermissionMappings;
            modelRole.Permissions = permissions;
            return modelRole;
        }

        public ModelRole GetRolePermissions(string RoleCode)
        {
            var modelRole = new ModelRole();
            modelRole = GetRoleByCode(RoleCode);
            IList<ModelPermission> permissions = new List<ModelPermission>();
            if (RoleCode == RoleTypes.SuperAdmin)
            {
                permissions = GetAllPermissions(true);
            }
            else
            {
                permissions = GetAllPermissions();
            }
            modelRole.Permissions = permissions;
            return modelRole;

        }

        public bool SaveRolePermission(ModelRolePermissionRequest request)
        {
            try
            {
                var rolePermissionMapping = _dbcontext.RolePermissionMappings.FirstOrDefault(e => e.RoleCode == request.RoleCode && e.PermissionCode == request.PermissionCode);
                if (rolePermissionMapping != null)
                {
                    rolePermissionMapping.PermissionType = request.PermissionType;
                    rolePermissionMapping.UpdatedDate = DateTime.UtcNow;
                }
                else
                {
                    var newRolePermissionMapping = new RolePermissionMapping()
                    {
                        PermissionCode = request.PermissionCode,
                        RoleCode = request.RoleCode,
                        PermissionType = request.PermissionType,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow,
                        IsActive = true,
                        IsDeleted = false
                    };
                    _dbcontext.RolePermissionMappings.Add(newRolePermissionMapping);
                }
                _dbcontext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RunDbScript()
        {
            try
            {
                _dbcontext.Database.ExecuteSqlCommand("ALTER TABLE [User] ALTER COLUMN [EmailID] varchar(50) NULL");
                _dbcontext.Database.ExecuteSqlCommand("ALTER TABLE [User] ADD  [Username] varchar(50)  null");
                _dbcontext.Database.ExecuteSqlCommand("update [User] set Username = EmailID");
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public UserData AuthenticateUser(ModelLogin loginRequest)
        {

            var userDetail = _dbcontext.Users.Include("GymUserMappings")
                .Include("GymUserMappings.Gym").Include("UserLocationMappings")
                .Include("UserLocationMappings.Location")
                .FirstOrDefault(e => e.Username == loginRequest.Username && e.IsDeleted != true);

            var userData = new UserData();
            if (userDetail != null)
            {
                var password = EncryptionService.CreatePasswordHash(loginRequest.Password, userDetail.SaltKey);
                if (password == userDetail.UserPassword)
                {
                    if (!Convert.ToBoolean(userDetail.IsActive) ||
                        Convert.ToBoolean(userDetail.IsDeleted))
                    {
                        userData.IsSuccess = false;
                        userData.Message = ErrorMessage.AccountDeactivated;
                        return userData;
                    }

                    userData.UserId = userDetail.ID;
                    userData.FirstName = userDetail.FirstName;
                    userData.LastName = userDetail.LastName;
                    userData.WorkOutType = userDetail.WorkOutType;
                    userData.RoleCode = userDetail.UserRoleMappings.FirstOrDefault().Role.UDC;
                    if (userDetail.GymUserMappings.FirstOrDefault() != null)
                        userData.ClientId = userDetail.GymUserMappings.FirstOrDefault().GymId;
                    if (userDetail.UserLocationMappings.FirstOrDefault() != null)
                        userData.ClientId = userDetail.UserLocationMappings.FirstOrDefault().Location.GymLocationMappings.FirstOrDefault().GymId;

                    //        userData.BranchId = userDetail.UserLocationMappings.FirstOrDefault() != null ? userDetail.UserLocationMappings.FirstOrDefault().LocationID : 0;


                    userData.BranchId = userDetail.UserLocationMappings.SingleOrDefault() != null ? userDetail.UserLocationMappings.FirstOrDefault().LocationID : 0;

                    var branchDetail = _dbcontext.Locations.FirstOrDefault(e => e.ID == userData.BranchId);
                    if (branchDetail != null && (branchDetail.IsActive == false || branchDetail.IsDeleted == true))
                    {
                        userData.IsSuccess = false;
                        userData.Message = ErrorMessage.AccountDeactivated;
                        return userData;
                    }
                    if (userData.ClientId > 0)
                    {
                        var clientOwner = _dbcontext.GymUserMappings.Include("User.UserRoleMappings").Where(e => e.GymId == userData.ClientId && e.User.UserRoleMappings.FirstOrDefault().Role.UDC == RoleTypes.ClientAdmin).FirstOrDefault();
                        if (clientOwner != null)
                        {
                            userData.ClientOwnerId = Convert.ToInt32(clientOwner.UserId);
                        }
                        var clientDetail = _dbcontext.Gyms.FirstOrDefault(e => e.Id == userData.ClientId);
                        if (clientDetail != null && (clientDetail.IsActive == false || clientDetail.IsDeleted == true))
                        {
                            userData.IsSuccess = false;
                            userData.Message = ErrorMessage.AccountDeactivated;
                            return userData;
                        }
                    }

                    userData.IsSuccess = true;

                    userData.AdminLocations = _dbcontext.UserLocationMappings.Where(e => e.UserID == userData.UserId && e.IsDeleted != true && e.IsActive == true).Select(e => new ModelLocation()
                    {
                        LocationName = e.Location.LocationName,
                        ID = e.LocationID
                    }).ToList();

                    return userData;
                }
                else
                {
                    userData.IsSuccess = false;
                    userData.Message = ErrorMessage.UsernamePasswordWrong;
                    return userData;
                }
            }
            else
            {
                userData.IsSuccess = false;
                userData.Message = ErrorMessage.UsernamePasswordWrong;
                return userData;
            }
        }

        public ModelUserPasswordResponse ChangePassword(ModelUserPassword model)
        {
            var response = new ModelUserPasswordResponse();

            var userDetail = _dbcontext.Users.FirstOrDefault(e => e.ID == model.UserId);

            if (userDetail != null)
            {
                var oldPassword = EncryptionService.CreatePasswordHash(model.OldPassword, userDetail.SaltKey);
                if (oldPassword != userDetail.UserPassword)
                {
                    response.IsSuccess = false;
                    response.Message = ErrorMessage.OldPasswordWrong;
                }
                else
                {
                    var password = EncryptionService.CreatePasswordHash(model.NewPassword, userDetail.SaltKey);
                    userDetail.UserPassword = password;
                    _dbcontext.SaveChanges();
                    response.IsSuccess = true;
                    response.Message = ErrorMessage.PasswordChanged;
                }
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ErrorMessage.GeneralError;
                return response;
            }

            return response;
        }

        public bool CheckUserExist(string emailId)
        {
            var isExist = false;
            try
            {
                isExist = _dbcontext.Users.Any(e => e.EmailID == emailId && e.IsDeleted != true);
            }
            catch (Exception)
            {
                isExist = false;
            }
            return isExist;
        }

        public bool CheckResetPasswordTokenExpired(string Token)
        {
            var isExpired = false;
            try
            {
                isExpired = _dbcontext.Users.Any(e => e.PasswordResetToken == Token && DbFunctions.DiffDays(e.TokenExpiryDate, DateTime.UtcNow) > 1);
            }
            catch (Exception)
            {
                isExpired = false;
            }
            return isExpired;
        }

        public bool SendResetPasswordLink(ForgorPasswordRequestModel model)
        {
            var isSent = false;
            try
            {
                var token = EncryptionService.RandomString(20);
                var user = _dbcontext.Users.FirstOrDefault(e => e.EmailID == model.Email && e.IsDeleted != true);
                if (user != null)
                {
                    user.PasswordResetToken = token;
                    user.TokenExpiryDate = DateTime.UtcNow.AddDays(1);
                    _dbcontext.SaveChanges();
                    var resetLink = $"http://www.fitnessvisualtrainer.com/account/resetpassword?token={token}";
                    var emailBody = $"Hello !<br/><h2>{user.FirstName} {user.LastName} </h2><br/><p>Please click on below reset password link.</p></br><p>Reset Link: <a href='{resetLink}'>Reset Password</a></p>";
                    EmailHelper.SendEmail(user.EmailID, EmailSubject.ResetPassword, emailBody, true);
                    isSent = true;
                }
            }
            catch (Exception)
            {
                isSent = false;
            }
            return isSent;
        }

        public bool ResetPassword(ForgotPasswordModel model)
        {
            var isReset = false;
            try
            {
                var userDetail = _dbcontext.Users.FirstOrDefault(e => e.PasswordResetToken == model.Token);
                if (userDetail != null)
                {
                    var encryptedPassword = EncryptionService.CreatePasswordHash(model.NewPassword, userDetail.SaltKey);
                    userDetail.UserPassword = encryptedPassword;
                    userDetail.PasswordResetToken = string.Empty;
                    userDetail.TokenExpiryDate = null;
                    _dbcontext.SaveChanges();
                    isReset = true;
                }
            }
            catch (Exception)
            {
                isReset = false;
            }
            return isReset;
        }

        #endregion
    }
}
