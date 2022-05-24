using BusinessLayer.Helpers;
using DataAccessLayer;
using Models;
using Models.Enums;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace BusinessLayer
{
    public class BalGym : IDisposable
    {
        SolciotyNewEntities _dbcontext;
        BalCity _balCity;
        public BalGym()
        {
            _dbcontext = new SolciotyNewEntities();
            _balCity = new BalCity();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #region Methods
        public ModelGym DalToModel(Gym objDal)
        {
            ModelGym objModel = new ModelGym();
            try
            {
                objModel.Name = objDal.Name;
                objModel.Id = objDal.Id;
                objModel.IsActiveString = objDal.IsActive.HasValue ? (objDal.IsActive.Value == true ? "on" : "") : "";
                if (objDal.GymUserMappings != null && objDal.GymUserMappings.Count > 0)
                {
                    objModel.Owners = new List<ModelUser>();
                    var balUser = new BalUser();
                    foreach (var gymUser in objDal.GymUserMappings.Where(e => e.IsActive == true && e.IsDeleted == false))
                    {
                        var userDetail = balUser.DalToModel(gymUser.User);
                        if (userDetail != null && userDetail.IsActive == true && objModel.Owners.Count < 1)
                        {
                            objModel.Owners.Add(userDetail);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return objModel;
        }


        public CommonResultsMessage AddEdit(ModelGym objmodel)
        {
            try
            {
                var gymId = objmodel.Id;
                var olddata = _dbcontext.Gyms.FirstOrDefault(g => g.Id == objmodel.Id);
                dynamic returnResult;
                if (olddata != null)
                {//edit
                    olddata.Name = objmodel.Name;
                    olddata.IsActive = objmodel.IsActive;
                    olddata.UpdatedDate = DateTime.UtcNow;
                    _dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
                    _dbcontext.SaveChanges();
                    returnResult = CommonResultsMessage.Update;
                }
                else
                {//add
                    var objDal = new Gym();
                    objDal.Name = objmodel.Name;
                    objDal.IsActive = objmodel.IsActive;
                    objDal.CreatedDate = DateTime.UtcNow;
                    objDal.UpdatedDate = DateTime.UtcNow;
                    objDal.IsDeleted = false;
                    _dbcontext.Gyms.Add(objDal);
                    _dbcontext.SaveChanges();
                    gymId = objDal.Id;
                    returnResult = CommonResultsMessage.Add;
                }

                if (objmodel.Owners != null)
                {
                    var balUser = new BalUser();
                    foreach (var owner in objmodel.Owners)
                    {
                        owner.RoleId = (int)RoleType.GymAdmin;
                        var ownerDetail = balUser.SaveUser(owner);
                        owner.UserId = ownerDetail.UserId;

                        var gymAdminMapping = _dbcontext.GymUserMappings.FirstOrDefault(e => e.GymId == objmodel.Id && e.UserId == owner.UserId);
                        if (gymAdminMapping == null)
                        {
                            var newGymUserMapping = new GymUserMapping()
                            {
                                UserId = owner.UserId,
                                GymId = gymId,
                                CreatedDate = DateTime.UtcNow,
                                UpdatedDate = DateTime.UtcNow,
                                IsActive = true,
                                IsDeleted = false
                            };

                            _dbcontext.GymUserMappings.Add(newGymUserMapping);
                        }
                        else
                        {
                            gymAdminMapping.UpdatedDate = DateTime.UtcNow;
                            gymAdminMapping.IsActive = owner.IsActive;
                        }
                        _dbcontext.SaveChanges();
                        var userRoleMap = _dbcontext.UserRoleMappings.FirstOrDefault(ur => ur.RoleID == owner.RoleId && ur.UserID == owner.UserId);
                        if (userRoleMap == null)
                        {
                            var newUserRoleMap = new UserRoleMapping()
                            {
                                UserID = owner.UserId,
                                RoleID = owner.RoleId,
                                CreatedDate = DateTime.UtcNow,
                                UpdatedDate = DateTime.UtcNow,
                                IsActive = true

                            };
                            _dbcontext.UserRoleMappings.Add(newUserRoleMap);
                        }
                        else
                        {
                            userRoleMap.UpdatedDate = DateTime.UtcNow;
                            userRoleMap.UserID = owner.UserId;
                        }
                        _dbcontext.SaveChanges();
                    }
                }

                return returnResult;
            }
            catch (Exception)
            {
                return CommonResultsMessage.Fail;
            }
        }

        public Models.GymLocationMapping GetGymLocationMappingDetail(int mappingId)
        {
            var gymLocationMappingDetail = new Models.GymLocationMapping();
            var oldData = _dbcontext.GymLocationMappings.Include("Location").Include("Location.City").FirstOrDefault(e => e.Id == mappingId);
            if (oldData != null)
            {
                gymLocationMappingDetail.GymLocationMappingId = oldData.Id;
                gymLocationMappingDetail.ID = oldData.Location.ID;
                gymLocationMappingDetail.GymId = Convert.ToInt32(oldData.GymId);
                gymLocationMappingDetail.UpdatedDate = DateTime.UtcNow;
                gymLocationMappingDetail.AddressLine1 = oldData.Location.AddressLine1;
                gymLocationMappingDetail.AddressLine2 = oldData.Location.AddressLine2;
                gymLocationMappingDetail.ZipCode = oldData.Location.ZipCode;
                gymLocationMappingDetail.CityID = Convert.ToInt32(oldData.Location.CityId);
                gymLocationMappingDetail.CityName = oldData.Location != null && oldData.Location.City != null ? oldData.Location.City.CityName : string.Empty;
                gymLocationMappingDetail.IsActiveString = oldData.IsActive.HasValue ? (oldData.IsActive.Value == true ? "on" : "") : "";
                gymLocationMappingDetail.LocationName = oldData.Location.LocationName;
                gymLocationMappingDetail.Phone = oldData.Location.Phone;
            }
            return gymLocationMappingDetail;
        }

        public CommonResultsMessage AddEditBranch(Models.GymLocationMapping objmodel)
        {
            try
            {
                var branchDetail = _dbcontext.Locations.FirstOrDefault(e => e.ID == objmodel.ID);
                var cityId = _balCity.GetCityId(objmodel.CityName, objmodel.StateCode);
                if (branchDetail != null)
                {
                    branchDetail.UpdatedBy = objmodel.UpdatedBy;
                    branchDetail.UpdatedDate = DateTime.UtcNow;
                    branchDetail.AddressLine1 = objmodel.AddressLine1;
                    branchDetail.AddressLine2 = objmodel.AddressLine2;
                    branchDetail.ZipCode = objmodel.ZipCode;
                    branchDetail.CityId = cityId > 0 ? cityId : new Nullable<int>();
                    //branchDetail.City = objmodel.CityName;
                    branchDetail.IsActive = objmodel.IsActive;
                    branchDetail.LocationName = objmodel.LocationName;
                    branchDetail.Phone = objmodel.Phone;
                    _dbcontext.Entry(branchDetail).State = System.Data.Entity.EntityState.Modified;
                    _dbcontext.SaveChanges();
                }
                else
                {
                    Location objDal = new Location();
                    objDal.CreatedBy = objmodel.CreatedBy;
                    objDal.UpdatedBy = objmodel.UpdatedBy;
                    objDal.IsDeleted = false;
                    objDal.CreatedDate = DateTime.UtcNow;
                    objDal.UpdatedDate = DateTime.UtcNow;
                    objDal.AddressLine1 = objmodel.AddressLine1;
                    objDal.AddressLine2 = objmodel.AddressLine2;
                    objDal.ZipCode = objmodel.ZipCode;
                    objDal.CityId = cityId > 0 ? cityId : new Nullable<int>();
                    // objDal.City = objmodel.CityName;
                    objDal.IsActive = objmodel.IsActive;
                    objDal.LocationName = objmodel.LocationName;
                    objDal.Phone = objmodel.Phone;
                    _dbcontext.Locations.Add(objDal);
                    _dbcontext.SaveChanges();
                    objmodel.ID = objDal.ID;
                }
                var olddata = _dbcontext.GymLocationMappings.Include("Location").FirstOrDefault(g => g.Id == objmodel.GymLocationMappingId);
                dynamic returnResult;
                if (olddata != null)
                {//edit
                    olddata.LocationId = objmodel.ID;
                    olddata.IsActive = objmodel.IsActive;
                    olddata.UpdatedDate = DateTime.UtcNow;
                    _dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
                    _dbcontext.SaveChanges();
                    returnResult = CommonResultsMessage.Update;
                }
                else
                {//add
                    var objDal = new DataAccessLayer.GymLocationMapping();
                    objDal.LocationId = objmodel.ID;
                    objDal.GymId = objmodel.GymId;
                    objDal.IsActive = objmodel.IsActive;
                    objDal.CreatedDate = DateTime.UtcNow;
                    objDal.UpdatedDate = DateTime.UtcNow;
                    objDal.IsDeleted = false;
                    _dbcontext.GymLocationMappings.Add(objDal);
                    _dbcontext.SaveChanges();
                    returnResult = CommonResultsMessage.Add;
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ModelBranch GetBranchDetail(int BranchId)
        {
            try
            {
                var branchDetail = new ModelBranch();
                var result = _dbcontext.Locations.Include("UserLocationMappings").Include("GymLocationMappings").FirstOrDefault(e => e.ID == BranchId);

                if (result != null)
                {
                    branchDetail.BranchName = result.LocationName;
                    branchDetail.Id = result.ID;
                    branchDetail.GymName = result.GymLocationMappings.FirstOrDefault().Gym.Name;
                    branchDetail.GymId = result.GymLocationMappings.FirstOrDefault().Gym.Id;
                    if (result.UserLocationMappings != null && result.UserLocationMappings.Count > 0)
                    {
                        var balUser = new BalUser();
                        foreach (var userLocationMapping in result.UserLocationMappings.Where(e => e.IsDeleted != true).ToList())
                        {
                            branchDetail.UserList.Add(balUser.DalToModel(userLocationMapping.User));
                        }
                    }
                }

                return branchDetail;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public CommonResultsMessage SaveBranchUserDetail(ModelBranch branchDetail)
        {
            try
            {
                var balUser = new BalUser();
                var user = balUser.SaveUser(branchDetail.UserList.FirstOrDefault());
                if (user.UserId > 0)
                {
                    var branchUserMappingDetail = _dbcontext.UserLocationMappings.FirstOrDefault(e => e.UserID == user.UserId && e.LocationID == branchDetail.Id);
                    if (branchUserMappingDetail != null)
                    {
                        branchUserMappingDetail.UpdatedDate = DateTime.UtcNow;
                        //                        branchUserMappingDetail.IsActive = true;
                        if (branchUserMappingDetail.IsDeleted == true)
                        {
                            branchUserMappingDetail.IsDeleted = false;
                        }
                        _dbcontext.SaveChanges();
                        return CommonResultsMessage.Update;
                    }
                    else
                    {
                        var newBranchUserMapping = new UserLocationMapping()
                        {
                            UserID = user.UserId,
                            LocationID = branchDetail.Id,
                            CreatedDate = DateTime.UtcNow,
                            UpdatedDate = DateTime.UtcNow,
                            IsDeleted = false
                        };

                        _dbcontext.UserLocationMappings.Add(newBranchUserMapping);
                        _dbcontext.SaveChanges();
                        return CommonResultsMessage.Update;
                    }
                }
                return CommonResultsMessage.Fail;
            }
            catch (Exception ex)
            {
                return CommonResultsMessage.Fail;
            }
        }

        public ModelUser GetBranchUserDetail(int userId, int branchId)
        {
            try
            {
                var branchUserMapping = _dbcontext.UserLocationMappings.Include("User.UserRoleMappings").FirstOrDefault(e => e.UserID == userId && e.LocationID == branchId);
                if (branchUserMapping != null && branchUserMapping.User != null)
                {
                    var balUser = new BalUser();
                    var userDetail = balUser.DalToModel(branchUserMapping.User);
                    return userDetail;
                }
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }

        public bool DeleteBranchUser(int branchId, int userId)
        {
            try
            {
                var branchUserMapping = _dbcontext.UserLocationMappings.FirstOrDefault(e => e.UserID == userId && e.LocationID == branchId);
                if (branchUserMapping != null)
                {
                    if (branchUserMapping.User != null)
                    {
                        branchUserMapping.User.IsActive = false;
                        branchUserMapping.User.IsDeleted = true;
                        branchUserMapping.User.EmailID = branchUserMapping.User.EmailID + "_Deleted";
                    }
                    branchUserMapping.IsActive = false;
                    branchUserMapping.IsDeleted = true;
                    _dbcontext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool CheckGymDuplication(string gymName)
        {
            try
            {
                var isDuplicate = _dbcontext.Gyms.Any(g => g.Name == gymName);
                return isDuplicate;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Models.GymLocationMapping> GetAllLocationByGym(int gymId)
        {
            try
            {
                var gymLocations = new List<Models.GymLocationMapping>();
                var locationMappings = _dbcontext.GymLocationMappings.Include("Location").Where(e => e.GymId == gymId && e.IsDeleted != true).ToList();
                var balLocation = new BalLocation();
                foreach (var locationMapping in locationMappings)
                {
                    if (locationMapping.Location != null)
                    {
                        var gymLocationMapping = new Models.GymLocationMapping();
                        gymLocationMapping.GymLocationMappingId = locationMapping.Id;
                        gymLocationMapping.GymId = gymId;
                        var location = balLocation.DalToModel(locationMapping.Location);
                        if (location != null)
                        {
                            gymLocationMapping.ID = location.ID;
                            gymLocationMapping.LocationName = location.LocationName;
                            gymLocationMapping.AddressLine1 = location.AddressLine1;
                            gymLocationMapping.AddressLine2 = location.AddressLine2;
                            gymLocationMapping.Phone = location.Phone;
                            gymLocationMapping.CreatedDate = DateTime.UtcNow;
                            gymLocationMapping.UpdatedDate = DateTime.UtcNow;
                            gymLocationMapping.ZipCode = location.ZipCode;
                            gymLocationMapping.IsActiveString = location.IsActiveString;
                            gymLocationMapping.IsDeleted = location.IsDeleted;
                            gymLocationMapping.StateList = location.StateList;
                            gymLocationMapping.CityID = location.CityID;
                            gymLocationMapping.CityList = location.CityList;
                            gymLocationMapping.CityName = location.CityName;

                            gymLocations.Add(gymLocationMapping);
                        }
                    }
                }
                return gymLocations;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IList<ModelGym> GetAllGyms()
        {
            IList<ModelGym> gyms = new List<ModelGym>();
            var gymList = new List<Gym>();
            try
            {
                gymList = _dbcontext.Gyms.Include("GymUserMappings").Include("GymUserMappings.User").Where(g => g.IsDeleted == false).ToList();

                foreach (var item in gymList)
                    gyms.Add(DalToModel(item));
            }
            catch (Exception)
            {
                throw;
            }
            return gyms;
        }


        public ModelGym GetById(int gymId)
        {
            try
            {
                return DalToModel(_dbcontext.Gyms.FirstOrDefault(g => g.Id == gymId && g.IsDeleted != true));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool Delete(int gymId)
        {
            try
            {
                var gymDetail = _dbcontext.Gyms.FirstOrDefault(e => e.Id == gymId);
                if (gymDetail != null)
                {
                    //var gymUsers = _dbcontext.GymUserMappings.Where(e => e.GymId == gymId);
                    //foreach (var user in gymUsers)
                    //{
                    //    user.IsActive = false;
                    //    user.IsDeleted = true;
                    //    var gymUser = _dbcontext.Users.FirstOrDefault(e => e.ID == user.UserId);
                    //    gymUser.IsDeleted = true;
                    //    gymUser.IsActive = false;
                    //    _dbcontext.SaveChanges();
                    //}
                    gymDetail.IsDeleted = true;
                    gymDetail.IsActive = false;
                    _dbcontext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteBranch(int mappingId)
        {
            try
            {
                var gymBranchMappingDetail = _dbcontext.GymLocationMappings.Include("Location").FirstOrDefault(e => e.Id == mappingId);
                if (gymBranchMappingDetail != null)
                {
                    gymBranchMappingDetail.IsDeleted = true;
                    gymBranchMappingDetail.UpdatedDate = DateTime.UtcNow;
                    gymBranchMappingDetail.Location.IsActive = false;
                    gymBranchMappingDetail.Location.IsDeleted = true;
                    gymBranchMappingDetail.Location.UpdatedDate = DateTime.UtcNow;
                    gymBranchMappingDetail.IsActive = false;
                    _dbcontext.SaveChanges();

                    var branchUsers = _dbcontext.UserLocationMappings.Where(x => x.LocationID == gymBranchMappingDetail.LocationId).ToList();
                    foreach (var branchUser in branchUsers)
                    {
                        branchUser.IsDeleted = true;
                        branchUser.IsActive = false;

                        branchUser.User.IsDeleted = true;
                    }
                    _dbcontext.SaveChanges();

                    return true;
                }

                //var branchUsers = _dbcontext.UserLocationMappings.Where(x=>x.LocationID == )
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RemoveGymAdmin(int gymId, int adminId)
        {
            try
            {
                var gymAdminDetail = _dbcontext.GymUserMappings.FirstOrDefault(e => e.UserId == adminId && e.GymId == gymId);
                if (gymAdminDetail != null)
                {
                    var adminDetail = _dbcontext.Users.FirstOrDefault(e => e.ID == gymAdminDetail.UserId);
                    if (adminDetail != null)
                    {
                        adminDetail.EmailID = adminDetail.EmailID + "_Deleted";
                        adminDetail.IsActive = false;
                        adminDetail.IsDeleted = true;
                    }
                    gymAdminDetail.IsDeleted = true;
                    gymAdminDetail.UpdatedDate = DateTime.UtcNow;
                    _dbcontext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<ModelBranchAdmin> GetClientBranchAdmins(int ClientId)
        {
            var branchAdminList = new List<ModelBranchAdmin>();
            var clientBranches = _dbcontext.GymLocationMappings.Include("Location").Include("Location.UserLocationMappings").Where(e => e.GymId == ClientId).ToList();

            foreach (var clientBranch in clientBranches)
            {
                if (clientBranch.Location != null && clientBranch.Location.UserLocationMappings != null && clientBranch.Location.UserLocationMappings.Count > 0)
                {
                    var branchAdmins = clientBranch.Location.UserLocationMappings.Where(e => e.User.UserRoleMappings.FirstOrDefault().Role.UDC == RoleTypes.BranchAdmin && e.IsDeleted != true).Select(e => new ModelBranchAdmin { Id = e.User.ID, Name = e.User.FirstName + " " + e.User.LastName }).ToList();

                    if (branchAdmins.Count > 0)
                    {
                        foreach (var branchAdmin in branchAdmins)
                        {
                            if (!branchAdminList.Any(e => e.Id == branchAdmin.Id))
                            {
                                branchAdminList.Add(branchAdmin);
                            }
                        }
                    }
                }
            }
            return branchAdminList;

        }

        public bool SaveClientConfiguration(ModelGymConfiguration clientConfiguration)
        {
            try
            {
                var clientDetail = _dbcontext.Gyms.FirstOrDefault(e => e.Id == clientConfiguration.ClientId);
                if (clientDetail != null)
                {
                    clientDetail.HeaderPrimaryColor = clientConfiguration.HeaderPrimaryColor;
                    clientDetail.HeaderTextColor = clientConfiguration.HeaderTextColor;
                    clientDetail.HeaderSecondaryColor = clientConfiguration.HeaderSecondaryColor;
                    clientDetail.HeaderTextColor = clientConfiguration.HeaderTextColor;
                    clientDetail.PrimaryText = clientConfiguration.PrimaryText;
                    clientDetail.PrimaryBackground = clientConfiguration.PrimaryBackground;
                    clientDetail.PrimaryGradientBackground = clientConfiguration.PrimaryGradientBackground;
                    clientDetail.PrimaryFontColor = clientConfiguration.PrimaryFontColor;
                    clientDetail.AlternateText = clientConfiguration.AlternateText;
                    clientDetail.AlternateBackground = clientConfiguration.AlternateBackground;
                    clientDetail.AlternateGradientBackground = clientConfiguration.AlternateGradientBackground;
                    clientDetail.AlternateFontColor = clientConfiguration.AlternateFontColor;
                    clientDetail.ShowTime = clientConfiguration.ShowTime;
                    clientDetail.LogoutBackground = clientConfiguration.SignoutBackground;
                    clientDetail.UpdatedDate = DateTime.UtcNow;
                    clientDetail.Logo = !string.IsNullOrEmpty(clientConfiguration.LogoName) ? clientConfiguration.LogoName : clientDetail.Logo;
                    clientDetail.AlternateLogo = !string.IsNullOrEmpty(clientConfiguration.AlternateLogoName) ? clientConfiguration.AlternateLogoName : clientDetail.AlternateLogo;
                    clientDetail.BackgroundImage = !string.IsNullOrEmpty(clientConfiguration.BackgroundImageName) ? clientConfiguration.BackgroundImageName : clientDetail.BackgroundImage;
                    clientDetail.VideoTitleColor = clientConfiguration.VideoTitleColor;
                    clientDetail.VideoTitleBackgroundColor = clientConfiguration.VideoTitleBackgroundColor;
                    _dbcontext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public ModelGymConfiguration GetClientConfiguration(int ClientId)
        {
            var clientConfiguration = _dbcontext.Gyms.Where(e => e.Id == ClientId).Select(e => new ModelGymConfiguration()
            {
                ClientId = e.Id,
                HeaderPrimaryColor = e.HeaderPrimaryColor,
                HeaderTextColor = e.HeaderTextColor,
                HeaderSecondaryColor = e.HeaderSecondaryColor,
                BackgroundImageName = e.BackgroundImage,
                PrimaryText = e.PrimaryText,
                AlternateText = e.AlternateText,
                AlternateBackground = e.AlternateBackground,
                PrimaryBackground = e.PrimaryBackground,
                LogoName = e.Logo,
                AlternateLogoName = e.AlternateLogo,
                SignoutBackground = e.LogoutBackground,
                PrimaryGradientBackground = e.PrimaryGradientBackground,
                AlternateGradientBackground = e.AlternateGradientBackground,
                PrimaryFontColor = e.PrimaryFontColor,
                AlternateFontColor = e.AlternateFontColor,
                VideoTitleColor = e.VideoTitleColor,
                VideoTitleBackgroundColor = e.VideoTitleBackgroundColor,
                IsActiveString = e.ShowTime == true ? "on" : ""
            }).FirstOrDefault();
            return clientConfiguration;
        }

        public ModelGym GetClientDetailByUserId(int UserId)
        {
            var userDetail = _dbcontext.Users.Include("UserRoleMappings").Include("GymUserMappings").Include("GymUserMappings.Gym").FirstOrDefault(e => e.ID == UserId);
            var clientDetail = new ModelGym();
            if (userDetail != null)
            {
                if (userDetail.UserRoleMappings.FirstOrDefault().Role.UDC == RoleTypes.ClientAdmin)
                {
                    clientDetail = new ModelGym()
                    {
                        Id = userDetail.GymUserMappings.FirstOrDefault().Gym.Id,
                        Name = userDetail.GymUserMappings.FirstOrDefault().Gym.Name
                    };
                }
                else if (userDetail.UserRoleMappings.FirstOrDefault().Role.UDC == RoleTypes.BranchAdmin || userDetail.UserRoleMappings.FirstOrDefault().Role.UDC == RoleTypes.BranchInstructor || userDetail.UserRoleMappings.FirstOrDefault().Role.UDC == RoleTypes.Client)
                {
                    clientDetail = _dbcontext.Gyms.Include("GymLocationMappings").Where(e => e.GymLocationMappings.FirstOrDefault().LocationId == userDetail.UserLocationMappings.FirstOrDefault().LocationID).Select(e => new ModelGym()
                    {
                        Id = e.Id,
                        Name = e.Name
                    }).FirstOrDefault();
                }
            }
            return clientDetail;
        }


        public List<ModelClientUser> GetGymAllUsers(int gymId)
        {
            var allUsers = new List<ModelClientUser>();
            try
            {
                using (var dbContext = new SolciotyNewEntities())
                {

                    allUsers = (from g in dbContext.Gyms
                                join glm in dbContext.GymLocationMappings on g.Id equals glm.GymId
                                join ulm in dbContext.UserLocationMappings on glm.LocationId equals ulm.LocationID
                                join u in dbContext.Users on ulm.UserID equals u.ID
                                join urm in dbContext.UserRoleMappings on u.ID equals urm.UserID
                                join r in dbContext.Roles on urm.RoleID equals r.ID
                                join tpu in dbContext.TrainingPortalUserMappings on u.ID equals tpu.UserId into Users
                                from usr in Users.DefaultIfEmpty()
                                where g.Id == gymId && u.IsDeleted != true
                                select new ModelClientUser
                                {
                                    UserId = u.ID,
                                    FirstName = u.FirstName,
                                    LastName = u.LastName,
                                    RoleName = r.RoleName,
                                    LocationName = glm.Location.LocationName,
                                    LocationId = glm.LocationId,
                                    PermissionType = usr.PermissionType
                                }
                            ).ToList();

                }
            }
            catch (Exception ex)
            {
                allUsers = null;
            }
            return allUsers;
        }


        public List<ModelClientUserForWorkout> GetGymAllUsersForWorkout(int gymId, int workoutId)
        {
            var allUsers = new List<ModelClientUserForWorkout>();
            try
            {
                using (var dbContext = new SolciotyNewEntities())
                {

                    allUsers = (from g in dbContext.Gyms
                                join glm in dbContext.GymLocationMappings on g.Id equals glm.GymId
                                join ulm in dbContext.UserLocationMappings on glm.LocationId equals ulm.LocationID
                                join u in dbContext.Users on ulm.UserID equals u.ID
                                join urm in dbContext.UserRoleMappings on u.ID equals urm.UserID
                                join r in dbContext.Roles on urm.RoleID equals r.ID
                                join wum in dbContext.WorkoutUserMappings on u.ID equals wum.UserId
                                //from usr in Users.DefaultIfEmpty()
                                where g.Id == gymId && wum.WorkoutId == workoutId
                                select new ModelClientUserForWorkout
                                {
                                    UserId = u.ID,
                                    FirstName = u.FirstName,
                                    LastName = u.LastName,
                                    RoleName = r.RoleName,
                                    LocationName = wum.Location.LocationName,
                                    LocationId = wum.LocationId,
                                    IsChecked = wum.IsActive == null ? false : wum.IsActive
                                }
                            ).ToList();

                }
            }
            catch (Exception ex)
            {
                allUsers = null;
            }
            return allUsers;
        }

        public List<ModelClientUserForBoxerWorkout> GetGymAllUsersForBoxerWorkout(int gymId, int boxerWorkoutId)
        {
            var allUsers = new List<ModelClientUserForBoxerWorkout>();
            try
            {
                using (var dbContext = new SolciotyNewEntities())
                {

                    allUsers = (from g in dbContext.Gyms
                                join glm in dbContext.GymLocationMappings on g.Id equals glm.GymId
                                join ulm in dbContext.UserLocationMappings on glm.LocationId equals ulm.LocationID
                                join u in dbContext.Users on ulm.UserID equals u.ID
                                join urm in dbContext.UserRoleMappings on u.ID equals urm.UserID
                                join r in dbContext.Roles on urm.RoleID equals r.ID
                                join wum in dbContext.BoxerWorkoutUserMappings on u.ID equals wum.UserId
                                //from usr in Users.DefaultIfEmpty()
                                where g.Id == gymId && wum.BoxerWorkoutId == boxerWorkoutId
                                select new ModelClientUserForBoxerWorkout
                                {
                                    UserId = u.ID,
                                    FirstName = u.FirstName,
                                    LastName = u.LastName,
                                    RoleName = r.RoleName,
                                    LocationName = wum.Location.LocationName,
                                    LocationId = wum.LocationId,
                                    IsChecked = wum.IsActive == null ? false : wum.IsActive
                                }
                            ).ToList();

                }
            }
            catch (Exception ex)
            {
                allUsers = null;
            }
            return allUsers;
        }

        public bool CheckTrainingPortalAccessibleByUser(int userId)
        {
            var isAccessible = false;
            try
            {
                using (var dbContext = new SolciotyNewEntities())
                {
                    isAccessible = dbContext.TrainingPortalUserMappings.Any(x => x.UserId == userId && x.PermissionType != (int)PermissionType.None);
                }
            }
            catch (Exception ex)
            {
                isAccessible = false;
            }
            return isAccessible;
        }

        public ModelBaseCopyResponse CopySite(ModelCopyGym model)
        {
            ModelBaseCopyResponse response = new ModelBaseCopyResponse();
            try
            {
                var saltKey = EncryptionService.CreateSaltKey(6);
                var encryptedPassword = EncryptionService.CreatePasswordHash(model.Password, saltKey);
                _dbcontext.Database.CommandTimeout = 0;
                var copyDetails = _dbcontext.SP_proc_copy_site(model.Id, model.Name, model.FirstName, model.LastName, model.EmailId, model.UserName, model.strCopyAction, saltKey, encryptedPassword).ToList();
                if (copyDetails != null && copyDetails.Count() > 0)
                {
                    response.result = copyDetails.Select(x => new ModelCopySiteResponse()
                    {
                        SourceId = x.SourceId,
                        DestinationId = x.DestinationId,
                        GroupName = x.GroupName
                    }).ToList();

                    if (!string.IsNullOrEmpty(model.EmailId))
                    {
                        var emailBody = $"Hello !<br/><h2>{model.FirstName} {model.LastName} </h2><br/><p>You are successfully registered with Solcioty. Your account information are below:</p></br><p>Username: {model.UserName} <br/>Password: {model.Password}</p>";
                        EmailHelper.SendEmail(model.EmailId, EmailSubject.WelcomeUser, emailBody, true);
                    }

                    response.Code = "S";
                    response.Message = "Success";
                }
                else
                {
                    response.Code = "E";
                    response.Message = "Data not available for copy";
                }

            }
            catch (Exception ex)
            {
                response.Code = "E";
                response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            }
            return response;
        }

        public void CopyVideos(List<ModelCopySiteResponse> models, string videoPath)
        {
            try
            {
                foreach (var item in models)
                {
                    CopyFilesOneToAnotherDir(videoPath + item.SourceId, videoPath + item.DestinationId);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void CopyImages(List<ModelCopySiteResponse> models, string imagePath)
        {
            try
            {
                foreach (var item in models)
                {
                    CopyFilesOneToAnotherDir(imagePath + item.SourceId, imagePath + item.DestinationId);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void CopyPdf(List<ModelCopySiteResponse> models, string pdfPath)
        {
            try
            {
                foreach (var item in models)
                {
                    CopyFilesOneToAnotherDir(pdfPath + item.SourceId, pdfPath + item.DestinationId);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void CopyConfigurationFile(List<ModelCopySiteResponse> models, string filePath)
        {
            try
            {
                foreach (var item in models)
                {
                    CopyFilesOneToAnotherDir(filePath + item.SourceId, filePath + item.DestinationId);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void CopyFilesOneToAnotherDir(string source, string destination)
        {
            if(Directory.Exists(source))
            {
                string[] filePaths = Directory.GetFiles(source);
                if (filePaths.Length > 0)
                {
                    if (!Directory.Exists(destination))
                    {
                        Directory.CreateDirectory(destination);
                    }
                    foreach (var file in filePaths)
                    {
                        string fileName = Path.GetFileName(file);
                        string destFile = Path.Combine(destination, fileName);
                        File.Copy(file, destFile, true);
                    }
                }
            }
        }

        #endregion
    }
}
