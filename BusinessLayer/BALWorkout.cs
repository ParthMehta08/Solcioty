using System;
using System.Collections.Generic;
using System.Linq;
using Models;
using DataAccessLayer;
using Services;
using System.Data.Entity;
using System.Web.Mvc;

namespace BusinessLayer
{
    public class BALWorkout : IDisposable
    {
        SolciotyNewEntities _dbcontext;
        BalGym _balGym;
        public BALWorkout()
        {
            _dbcontext = new SolciotyNewEntities();
            _balGym = new BalGym();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #region Methods
        /// <summary>
        /// Map Workout Master Model to Model Workout
        /// </summary>
        /// <param name="objDal"></param>
        /// <returns></returns>
        public ModelWorkout DalToModel(WorkoutMaster objDal)
        {
            ModelWorkout objModel = new ModelWorkout();
            try
            {
                objModel.CreatedBy = objDal.CreatedBy;
                objModel.CreatedDate = objDal.CreatedDate;
                objModel.ID = objDal.ID;
                objModel.IsDeleted = objDal.IsDeleted.HasValue ? objDal.IsDeleted.Value : false;
                objModel.IsReadyForLocations = objDal.IsReadyForLocations.HasValue ? objDal.IsReadyForLocations.Value : false;
                objModel.UpdatedBy = objDal.UpdatedBy;
                objModel.UpdatedDate = objDal.UpdatedDate;
                objModel.WorkoutDescription = objDal.WorkoutDescription;
                objModel.WorkoutName = objDal.WorkoutName;
                objModel.PDFName = objDal.PDFName;
                objModel.FrontCoverImageId = objDal.FrontCoverImageId;
                objModel.FrontCoverImageName = objDal.ImageGallery1 != null ? objDal.ImageGallery1.ImageFile : string.Empty;
                objModel.BackCoverImageId = objDal.BackCoverImageId;
                objModel.BackCoverImageName = objDal.ImageGallery != null ? objDal.ImageGallery.ImageFile : string.Empty;
                var clientDetail = _balGym.GetClientDetailByUserId(Convert.ToInt32(objModel.CreatedBy));
                objModel.CategoryId = objDal.CategoryId != null ? objDal.CategoryId.Value.ToString() : "";
                objModel.Categories = _dbcontext.WorkoutCategories.Where(x => x.IsActive == true && x.CreatedBy == objModel.CreatedBy && x.IsDeleted != true).Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
                if (objDal.WorkoutCategory != null)
                    objModel.CategoryName = objDal.WorkoutCategory.Name;

                if (clientDetail != null)
                {
                    objModel.ClientName = clientDetail.Name;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return objModel;
        }
        public ModelBoxerWorkout DalToModel(BoxerWorkoutMaster objDal)
        {
            ModelBoxerWorkout objModel = new ModelBoxerWorkout();
            try
            {
                objModel.CreatedBy = objDal.CreatedBy;
                objModel.CreatedDate = objDal.CreatedDate;
                objModel.ID = objDal.ID;
                objModel.IsDeleted = objDal.IsDeleted.HasValue ? objDal.IsDeleted.Value : false;
                objModel.IsReadyForLocations = objDal.IsReadyForLocations.HasValue ? objDal.IsReadyForLocations.Value : false;
                objModel.UpdatedBy = objDal.UpdatedBy;
                objModel.UpdatedDate = objDal.UpdatedDate;
                objModel.WorkoutDescription = objDal.WorkoutDescription;
                objModel.WorkoutName = objDal.WorkoutName;
                objModel.PDFName = objDal.PDFName;
                objModel.FrontCoverImageId = objDal.FrontCoverImageId;
                objModel.FrontCoverImageName = objDal.ImageGallery1 != null ? objDal.ImageGallery1.ImageFile : string.Empty;
                objModel.BackCoverImageId = objDal.BackCoverImageId;
                objModel.BackCoverImageName = objDal.ImageGallery != null ? objDal.ImageGallery.ImageFile : string.Empty;
                var clientDetail = _balGym.GetClientDetailByUserId(Convert.ToInt32(objModel.CreatedBy));
                objModel.CategoryId = objDal.CategoryId != null ? objDal.CategoryId.Value.ToString() : "";
                objModel.Categories = _dbcontext.WorkoutCategories.Where(x => x.IsActive == true && x.CreatedBy == objModel.CreatedBy && x.IsDeleted != true).Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
                //if (objDal.WorkoutCategory != null)
                //    objModel.CategoryName = objDal.WorkoutCategory.Name;

                if (clientDetail != null)
                {
                    objModel.ClientName = clientDetail.Name;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return objModel;
        }
        /// <summary>
        /// Save Workout Detail
        /// </summary>
        /// <param name="objModel">Workout Detail Model</param>
        /// <param name="UserId">User Id</param>
        /// <returns></returns>
        public CommonResultsMessage AddEdit(ModelWorkout objModel, int UserId)
        {
            try
            {
                //check TEmplate name exists or not bcoz every TEmplate name must unique
                if (CheckWorkoutNameDuplication(objModel.ID, objModel.WorkoutName, UserId))
                {
                    return CommonResultsMessage.duplicate;
                }
                if (objModel.ID > 0)
                {//edit
                    var olddata = _dbcontext.WorkoutMasters.FirstOrDefault(p => p.ID == objModel.ID);
                    olddata.UpdatedBy = UserId;
                    olddata.UpdatedDate = DateTime.UtcNow;
                    olddata.WorkoutName = objModel.WorkoutName;
                    olddata.WorkoutDescription = objModel.WorkoutDescription;
                    olddata.FrontCoverImageId = objModel.FrontCoverImageId;
                    olddata.BackCoverImageId = objModel.BackCoverImageId;
                    olddata.IsReadyForLocations = true;
                    if (objModel.PDFName != null)
                    {
                        olddata.PDFName = objModel.PDFName;
                    }
                    if (!string.IsNullOrEmpty(objModel.CategoryId))
                    {
                        olddata.CategoryId = Convert.ToInt32(objModel.CategoryId);
                    }
                    _dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
                    _dbcontext.SaveChanges();

                    // Save Video 
                    if (objModel.PDFName != null)
                    {
                        objModel.PDFServerPath = objModel.PDFServerPath + olddata.ID;
                        if (!System.IO.Directory.Exists(objModel.PDFServerPath))
                        {

                            System.IO.Directory.CreateDirectory(objModel.PDFServerPath);
                        }
                        System.IO.File.WriteAllBytes(objModel.PDFServerPath + "\\" + objModel.PDFName, objModel.PDF);
                    }

                    return CommonResultsMessage.Update;
                }
                else
                {//add
                    WorkoutMaster objDal = new WorkoutMaster();
                    objDal.CreatedBy = UserId;
                    objDal.UpdatedBy = UserId;
                    objDal.IsDeleted = false;
                    objDal.IsReadyForLocations = true;
                    objDal.CreatedDate = DateTime.UtcNow;
                    objDal.UpdatedDate = DateTime.UtcNow;
                    objDal.WorkoutDescription = objModel.WorkoutDescription;
                    objDal.FrontCoverImageId = objModel.FrontCoverImageId;
                    objDal.BackCoverImageId = objModel.BackCoverImageId;
                    objDal.WorkoutName = objModel.WorkoutName;
                    if (objModel.PDFName != null)
                    {
                        objDal.PDFName = objModel.PDFName;
                    }
                    if (!string.IsNullOrEmpty(objModel.CategoryId))
                    {
                        objDal.CategoryId = Convert.ToInt32(objModel.CategoryId);
                    }
                    _dbcontext.WorkoutMasters.Add(objDal);
                    _dbcontext.SaveChanges();

                    if (objModel.PDFName != null)
                    {
                        objModel.PDFServerPath = objModel.PDFServerPath + objDal.ID;
                        if (!System.IO.Directory.Exists(objModel.PDFServerPath))
                        {
                            System.IO.Directory.CreateDirectory(objModel.PDFServerPath);
                        }
                        System.IO.File.WriteAllBytes(objModel.PDFServerPath + "\\" + objModel.PDFName, objModel.PDF);
                    }

                    return CommonResultsMessage.Add;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public CommonResultsMessage AddEditBoxer(ModelBoxerWorkout objModel, int UserId)
        {
            try
            {
                //check TEmplate name exists or not bcoz every TEmplate name must unique
                if (CheckWorkoutNameDuplicationBoxer(objModel.ID, objModel.WorkoutName, UserId))
                {
                    return CommonResultsMessage.duplicate;
                }
                if (objModel.ID > 0)
                {//edit
                    var olddata = _dbcontext.BoxerWorkoutMasters.FirstOrDefault(p => p.ID == objModel.ID);
                    olddata.UpdatedBy = UserId;
                    olddata.UpdatedDate = DateTime.UtcNow;
                    olddata.WorkoutName = objModel.WorkoutName;
                    olddata.WorkoutDescription = objModel.WorkoutDescription;
                    olddata.FrontCoverImageId = objModel.FrontCoverImageId;
                    olddata.BackCoverImageId = objModel.BackCoverImageId;
                    olddata.IsReadyForLocations = true;
                    if (objModel.PDFName != null)
                    {
                        olddata.PDFName = objModel.PDFName;
                    }
                    if (!string.IsNullOrEmpty(objModel.CategoryId))
                    {
                        olddata.CategoryId = Convert.ToInt32(objModel.CategoryId);
                    }
                    _dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
                    _dbcontext.SaveChanges();

                    // Save Video 
                    if (objModel.PDFName != null)
                    {
                        objModel.PDFServerPath = objModel.PDFServerPath + olddata.ID;
                        if (!System.IO.Directory.Exists(objModel.PDFServerPath))
                        {

                            System.IO.Directory.CreateDirectory(objModel.PDFServerPath);
                        }
                        System.IO.File.WriteAllBytes(objModel.PDFServerPath + "\\" + objModel.PDFName, objModel.PDF);
                    }

                    return CommonResultsMessage.Update;
                }
                else
                {//add
                    BoxerWorkoutMaster objDal = new BoxerWorkoutMaster();
                    objDal.CreatedBy = UserId;
                    objDal.UpdatedBy = UserId;
                    objDal.IsDeleted = false;
                    objDal.IsReadyForLocations = true;
                    objDal.CreatedDate = DateTime.UtcNow;
                    objDal.UpdatedDate = DateTime.UtcNow;
                    objDal.WorkoutDescription = objModel.WorkoutDescription;
                    objDal.FrontCoverImageId = objModel.FrontCoverImageId;
                    objDal.BackCoverImageId = objModel.BackCoverImageId;
                    objDal.WorkoutName = objModel.WorkoutName;
                    if (objModel.PDFName != null)
                    {
                        objDal.PDFName = objModel.PDFName;
                    }
                    if (!string.IsNullOrEmpty(objModel.CategoryId))
                    {
                        objDal.CategoryId = Convert.ToInt32(objModel.CategoryId);
                    }
                    _dbcontext.BoxerWorkoutMasters.Add(objDal);
                    _dbcontext.SaveChanges();

                    if (objModel.PDFName != null)
                    {
                        objModel.PDFServerPath = objModel.PDFServerPath + objDal.ID;
                        if (!System.IO.Directory.Exists(objModel.PDFServerPath))
                        {
                            System.IO.Directory.CreateDirectory(objModel.PDFServerPath);
                        }
                        System.IO.File.WriteAllBytes(objModel.PDFServerPath + "\\" + objModel.PDFName, objModel.PDF);
                    }

                    return CommonResultsMessage.Add;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check whether Workout Exit or not
        /// </summary>
        /// <param name="ID">Workout Id</param>
        /// <param name="WorkoutNameNew">Workout Name</param>
        /// <returns></returns>
        public bool CheckWorkoutNameDuplication(Int32 ID, String WorkoutNameNew, int ClientOwnerId)
        {
            try
            {
                var duplicate = _dbcontext.WorkoutMasters.Where(p => p.ID != ID && p.WorkoutName.ToLower().Trim() == WorkoutNameNew.ToLower().Trim() && p.CreatedBy == ClientOwnerId);
                if (duplicate.Any())
                    return true;
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }

        public bool CheckWorkoutNameDuplicationBoxer(Int32 ID, String WorkoutNameNew, int ClientOwnerId)
        {
            try
            {
                var duplicate = _dbcontext.BoxerWorkoutMasters.Where(p => p.ID != ID && p.WorkoutName.ToLower().Trim() == WorkoutNameNew.ToLower().Trim() && p.CreatedBy == ClientOwnerId);
                if (duplicate.Any())
                    return true;
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }

        /// <summary>
        /// Retrieve All workout if user is Super Admin. 
        /// Else workout related to logged in user will be retrieved.
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="ForAdmin"></param>
        /// <param name="withDeleted"></param>
        /// <returns></returns>
        public IList<ModelWorkout> GetAllWorkouts(int UserId, int? categoryId, bool ForAdmin = false, bool withDeleted = false)
        {
            IList<ModelWorkout> Workouts = new List<ModelWorkout>();
            var Query = new List<WorkoutMaster>();
            try
            {
                if (withDeleted)
                    Query = _dbcontext.WorkoutMasters.Where(e => e.CreatedBy == UserId).ToList();
                else if (ForAdmin)
                    Query = _dbcontext.WorkoutMasters.Where(e => e.IsDeleted == false).ToList();
                else
                    Query = _dbcontext.WorkoutMasters.Where(p => (p.IsDeleted != true || p.IsDeleted == null) && p.CreatedBy == UserId).ToList();
                //if (!ForAdmin)
                //    Query = Query.Where(p => p.IsActive == true).ToList();
                if (categoryId != null && categoryId.Value > 0)
                    Query = Query.Where(x => x.CategoryId == categoryId.Value).ToList();

                foreach (var item in Query)
                    Workouts.Add(DalToModel(item));
            }
            catch (Exception ex)
            {
                throw;
            }
            return Workouts;
        }

        
        public IList<ModelBoxerWorkout> GetAllWorkoutsBoxer(int UserId, int? categoryId, bool ForAdmin = false, bool withDeleted = false)
        {
            IList<ModelBoxerWorkout> Workouts = new List<ModelBoxerWorkout>();
            var Query = new List<BoxerWorkoutMaster>();
            try
            {
                if (withDeleted)
                    Query = _dbcontext.BoxerWorkoutMasters.Where(e => e.CreatedBy == UserId).ToList();
                else if (ForAdmin)
                    Query = _dbcontext.BoxerWorkoutMasters.Where(e => e.IsDeleted == false).ToList();
                else
                    Query = _dbcontext.BoxerWorkoutMasters.Where(p => (p.IsDeleted != true || p.IsDeleted == null) && p.CreatedBy == UserId).ToList();
                //if (!ForAdmin)
                //    Query = Query.Where(p => p.IsActive == true).ToList();
                if (categoryId != null && categoryId.Value > 0)
                    Query = Query.Where(x => x.CategoryId == categoryId.Value).ToList();

               foreach (var item in Query)
                    Workouts.Add(DalToModel(item));
            }
            catch (Exception ex)
            {
                throw;
            }
            return Workouts;
        }

        public bool WorkoutSelect(int WorkoutScheduleId, UserData userData)
        {
            try
            {
                var allSelectedWorkouts = _dbcontext.LocationSelectedWorkouts.Where(e => e.LocationID == userData.BranchId).ToList();
                var oldSelectedWorkout = _dbcontext.LocationSelectedWorkouts.Include("WorkoutSchedule").FirstOrDefault(e => e.WorkoutScheduleID == WorkoutScheduleId && e.LocationID == userData.BranchId);
                var isActive = oldSelectedWorkout != null ? oldSelectedWorkout.IsActive : false;
                var scheduledWorkout = _dbcontext.WorkoutSchedules.FirstOrDefault(e => e.ID == WorkoutScheduleId);

                var allWorkouts = _dbcontext.WorkoutSchedules.ToList();
                foreach (var scheduleWorkout in allWorkouts)
                {
                    if (scheduleWorkout.ScheduledDate.Date == scheduledWorkout.ScheduledDate.Date)
                    {
                        var selectedWorkoutDetail = _dbcontext.LocationSelectedWorkouts.FirstOrDefault(e => e.WorkoutScheduleID == scheduleWorkout.ID && e.LocationID == userData.BranchId);
                        if (selectedWorkoutDetail != null)
                        {
                            //selectedWorkoutDetail.IsActive = false;
                            //selectedWorkoutDetail.IsDeleted = true;
                        }
                    }


                }
                //_dbcontext.SaveChanges();
                var selectedWorkout = _dbcontext.LocationSelectedWorkouts.Include("WorkoutSchedule").FirstOrDefault(e => e.WorkoutScheduleID == WorkoutScheduleId && e.LocationID == userData.BranchId);
                var isNewSelection = false;
                if (selectedWorkout != null)
                {
                    if (selectedWorkout.WorkoutSchedule.ScheduledDate.Date < DateTime.UtcNow.Date)
                    {
                        selectedWorkout.IsActive = false;
                        selectedWorkout.IsDeleted = true;
                        selectedWorkout.UpdatedDate = DateTime.UtcNow;
                        selectedWorkout.UpdatedBy = userData.UserId;
                        isNewSelection = true;
                    }
                    if (selectedWorkout.WorkoutSchedule.ScheduledDate.Date >= DateTime.UtcNow.Date)
                    {
                        selectedWorkout.IsActive = !isActive;
                        selectedWorkout.IsDeleted = isActive;
                        selectedWorkout.UpdatedDate = DateTime.UtcNow;
                        selectedWorkout.UpdatedBy = userData.UserId;
                    }
                }
                if (selectedWorkout == null || isNewSelection == true)
                {
                    var newSelectedWorkout = new LocationSelectedWorkout()
                    {
                        LocationID = Convert.ToInt32(userData.BranchId),
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow,
                        UpdatedBy = userData.UserId,
                        IsActive = true,
                        IsDeleted = false,
                        WorkoutScheduleID = WorkoutScheduleId
                    };
                    _dbcontext.LocationSelectedWorkouts.Add(newSelectedWorkout);
                }
                _dbcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;


        }

        public bool BoxerWorkoutSelect(int BoxerWorkoutScheduleId, UserData userData)
        {
            try
            {
                var allSelectedBoxerWorkouts = _dbcontext.LocationSelectedBoxerWorkouts.Where(e => e.LocationID == userData.BranchId).ToList();
                var oldSelectedBoxerWorkout = _dbcontext.LocationSelectedBoxerWorkouts.Include("BoxerWorkoutSchedule").FirstOrDefault(e => e.BoxerWorkoutScheduleID == BoxerWorkoutScheduleId && e.LocationID == userData.BranchId);
                var isActive = oldSelectedBoxerWorkout != null ? oldSelectedBoxerWorkout.IsActive : false;
                var scheduledBoxerWorkout = _dbcontext.BoxerWorkoutSchedules.FirstOrDefault(e => e.ID == BoxerWorkoutScheduleId);

                var allBoxerWorkouts = _dbcontext.BoxerWorkoutSchedules.ToList();
                foreach (var scheduleBoxerWorkout in allBoxerWorkouts)
                {
                    if (scheduleBoxerWorkout.ScheduledDate.Date == scheduledBoxerWorkout.ScheduledDate.Date)
                    {
                        var selectedBoxerWorkoutDetail = _dbcontext.LocationSelectedBoxerWorkouts.FirstOrDefault(e => e.BoxerWorkoutScheduleID == scheduleBoxerWorkout.ID && e.LocationID == userData.BranchId);
                        if (selectedBoxerWorkoutDetail != null)
                        {
                            //selectedWorkoutDetail.IsActive = false;
                            //selectedWorkoutDetail.IsDeleted = true;
                        }
                    }


                }
                //_dbcontext.SaveChanges();
                var selectedBoxerWorkout = _dbcontext.LocationSelectedBoxerWorkouts.Include("BoxerWorkoutSchedule").FirstOrDefault(e => e.BoxerWorkoutScheduleID == BoxerWorkoutScheduleId && e.LocationID == userData.BranchId);
                var isNewSelection = false;
                if (selectedBoxerWorkout != null)
                {
                    if (selectedBoxerWorkout.BoxerWorkoutSchedule.ScheduledDate.Date < DateTime.UtcNow.Date)
                    {
                        selectedBoxerWorkout.IsActive = false;
                        selectedBoxerWorkout.IsDeleted = true;
                        selectedBoxerWorkout.UpdatedDate = DateTime.UtcNow;
                        selectedBoxerWorkout.UpdatedBy = userData.UserId;
                        isNewSelection = true;
                    }
                    if (selectedBoxerWorkout.BoxerWorkoutSchedule.ScheduledDate.Date >= DateTime.UtcNow.Date)
                    {
                        selectedBoxerWorkout.IsActive = !isActive;
                        selectedBoxerWorkout.IsDeleted = isActive;
                        selectedBoxerWorkout.UpdatedDate = DateTime.UtcNow;
                        selectedBoxerWorkout.UpdatedBy = userData.UserId;
                    }
                }
                if (selectedBoxerWorkout == null || isNewSelection == true)
                {
                    var newSelectedBoxerWorkout = new LocationSelectedBoxerWorkout()
                    {
                        LocationID = Convert.ToInt32(userData.BranchId),
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow,
                        UpdatedBy = userData.UserId,
                        IsActive = true,
                        IsDeleted = false,
                        BoxerWorkoutScheduleID = BoxerWorkoutScheduleId
                    };
                    _dbcontext.LocationSelectedBoxerWorkouts.Add(newSelectedBoxerWorkout);
                }
                _dbcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;


        }


        public IList<ModelWorkoutSchedule> GetScheduledWorkouts(UserData userData, int locationId)
        {
            IList<ModelWorkout> workoutList = new List<ModelWorkout>();
            if (userData.RoleCode == RoleTypes.SuperAdmin)
            {
                workoutList = GetAllWorkouts(userData.UserId, null, true);
            }
            else
            {
                workoutList = GetAllWorkouts(userData.ClientOwnerId, null, false);

            }
            var scheduledWorkouts = new List<ModelWorkoutSchedule>();
            if (workoutList != null && workoutList.Count > 0)
            {
                var workoutIds = workoutList.Select(e => e.ID).ToList();
                scheduledWorkouts = _dbcontext.WorkoutSchedules.Include("WorkoutMasters").Where(e => workoutIds.Contains(e.WorkoutMasterID) && e.IsDeleted != true && e.LocationId == locationId)
                    .Select(e => new ModelWorkoutSchedule()
                    {
                        ScheduledDate = e.ScheduledDate,
                        WorkoutId = e.WorkoutMasterID,
                        WorkoutScheduleId = e.ID,
                        WorkoutName = e.WorkoutMaster.WorkoutName,
                        IsActiveString = e.IsActive.HasValue ? (e.IsActive.Value == true ? "on" : "") : ""
                    }).ToList();

            }
            foreach (var scheduleWorkout in scheduledWorkouts)
            {
                scheduleWorkout.IsSelected = _dbcontext.LocationSelectedWorkouts.Any(e => e.LocationID == userData.BranchId && e.WorkoutScheduleID == scheduleWorkout.WorkoutScheduleId && e.IsDeleted == false && e.IsActive == true);
            }
            return scheduledWorkouts;

        }

        public IList<ModelBoxerWorkoutSchedule> GetBoxerScheduledWorkouts(UserData userData, int locationId)
        {
            IList<ModelBoxerWorkout> workoutList = new List<ModelBoxerWorkout>();
            if (userData.RoleCode == RoleTypes.SuperAdmin)
            {
                workoutList = GetAllWorkoutsBoxer(userData.UserId, null, true);
            }
            else
            {
                workoutList = GetAllWorkoutsBoxer(userData.ClientOwnerId, null, false);

            }
            var scheduledBoxerWorkouts = new List<ModelBoxerWorkoutSchedule>();
            if (workoutList != null && workoutList.Count > 0)
            {
                var workoutIds = workoutList.Select(e => e.ID).ToList();
                scheduledBoxerWorkouts = _dbcontext.BoxerWorkoutSchedules.Include("BoxerWorkoutMasters").Where(e => workoutIds.Contains(e.BoxerWorkoutMasterID) && e.IsDeleted != true && e.LocationId == locationId)
                    .Select(e => new ModelBoxerWorkoutSchedule()
                    {
                        ScheduledDate = e.ScheduledDate,
                        BoxerWorkoutId = e.BoxerWorkoutMasterID,
                        BoxerWorkoutScheduleId = e.ID,
                        BoxerWorkoutName = e.BoxerWorkoutMaster.WorkoutName,
                        IsActiveString = e.IsActive.HasValue ? (e.IsActive.Value == true ? "on" : "") : ""
                    }).ToList();

            }
            foreach (var scheduleWorkout in scheduledBoxerWorkouts)
            {
                scheduleWorkout.IsSelected = _dbcontext.LocationSelectedWorkouts.Any(e => e.LocationID == userData.BranchId && e.WorkoutScheduleID == scheduleWorkout.BoxerWorkoutScheduleId && e.IsDeleted == false && e.IsActive == true);
            }
            return scheduledBoxerWorkouts;

        }

        public List<ModelWorkout> GetTodayWorkoutForLocation(UserData userdata)
        {
            var workoutList = new List<ModelWorkout>();

            var scheduledWorkouts = _dbcontext.LocationSelectedWorkouts.Where(e => e.LocationID == userdata.BranchId && e.IsActive == true && e.IsDeleted == false && e.WorkoutSchedule.IsDeleted != true);
            foreach (var scheduledWorkout in scheduledWorkouts)
            {
                if (scheduledWorkout.WorkoutSchedule.ScheduledDate.Date >= DateTime.Now.Date && scheduledWorkout.WorkoutSchedule.ScheduledDate.Date <= DateTime.Now.Date.AddDays(7) && scheduledWorkout.WorkoutSchedule.IsActive == true)
                {
                    var workoutDetail = new ModelWorkout();
                    workoutDetail.WorkoutName = scheduledWorkout.WorkoutSchedule.WorkoutMaster.WorkoutName;
                    workoutDetail.ID = scheduledWorkout.WorkoutSchedule.WorkoutMaster.ID;
                    workoutDetail.CreatedDate = scheduledWorkout.WorkoutSchedule.ScheduledDate;
                    if (scheduledWorkout.WorkoutSchedule.ScheduledDate.Date != DateTime.Now.Date)
                        workoutDetail.IsFutureWorkout = true;
                    workoutList.Add(workoutDetail);
                }
            }
            var userMappedWorkout = _dbcontext.WorkoutUserMappings.Where(x => x.LocationId == userdata.BranchId && x.UserId == userdata.UserId && x.IsActive == true && x.WorkoutMaster.IsDeleted != true).Select(x => new ModelWorkout
            {
                WorkoutName = x.WorkoutMaster.WorkoutName,
                ID = x.WorkoutMaster.ID
            }).ToList();
            if (userMappedWorkout != null && userMappedWorkout.Count > 0)
                workoutList.AddRange(userMappedWorkout);
            return workoutList.OrderBy(e => e.CreatedDate).ToList();
        }

        public List<ModelBoxerWorkout> GetTodayBoxerWorkoutForLocation(UserData userdata)
        {
            var boxerWorkoutList = new List<ModelBoxerWorkout>();
            var scheduledBoxerWorkouts = _dbcontext.LocationSelectedBoxerWorkouts.Where(e => e.LocationID == userdata.BranchId && e.IsActive == true && e.IsDeleted == false && e.BoxerWorkoutSchedule.IsDeleted != true);
            foreach (var scheduledBoxerWorkout in scheduledBoxerWorkouts)
            {
                if (scheduledBoxerWorkout.BoxerWorkoutSchedule.ScheduledDate.Date >= DateTime.Now.Date && scheduledBoxerWorkout.BoxerWorkoutSchedule.ScheduledDate.Date <= DateTime.Now.Date.AddDays(7) && scheduledBoxerWorkout.BoxerWorkoutSchedule.IsActive == true)
                {
                    var workoutBoxerDetail = new ModelBoxerWorkout();
                    workoutBoxerDetail.WorkoutName = scheduledBoxerWorkout.BoxerWorkoutSchedule.BoxerWorkoutMaster.WorkoutName;
                    workoutBoxerDetail.ID = scheduledBoxerWorkout.BoxerWorkoutSchedule.BoxerWorkoutMaster.ID;
                    workoutBoxerDetail.CreatedDate = scheduledBoxerWorkout.BoxerWorkoutSchedule.ScheduledDate;
                    if (scheduledBoxerWorkout.BoxerWorkoutSchedule.ScheduledDate.Date != DateTime.Now.Date)
                        workoutBoxerDetail.IsFutureWorkout = true;
                    boxerWorkoutList.Add(workoutBoxerDetail);
                }
            }
            var userMappedBoxerWorkout = _dbcontext.BoxerWorkoutUserMappings.Where(x => x.LocationId == userdata.BranchId && x.UserId == userdata.UserId && x.IsActive == true && x.BoxerWorkoutMaster.IsDeleted != true).Select(x => new ModelBoxerWorkout
            {
                WorkoutName = x.BoxerWorkoutMaster.WorkoutName,
                ID = x.BoxerWorkoutMaster.ID
            }).ToList();
            if (userMappedBoxerWorkout != null && userMappedBoxerWorkout.Count > 0)
                boxerWorkoutList.AddRange(userMappedBoxerWorkout);
            return boxerWorkoutList.OrderBy(e => e.CreatedDate).ToList();
        }

        public List<ModelWorkout> GetFutureWorkoutForLocation(UserData userdata)
        {
            var workoutList = new List<ModelWorkout>();

            var scheduledWorkouts = _dbcontext.LocationSelectedWorkouts.Where(e => e.LocationID == userdata.BranchId && e.IsActive == true && e.IsDeleted == false);
            foreach (var scheduledWorkout in scheduledWorkouts)
            {
                if (scheduledWorkout.WorkoutSchedule.ScheduledDate.Date >= DateTime.Now.Date && scheduledWorkout.WorkoutSchedule.ScheduledDate.Date <= DateTime.Now.Date.AddDays(7) && scheduledWorkout.WorkoutSchedule.IsActive == true)
                {
                    var workoutDetail = new ModelWorkout();
                    workoutDetail.WorkoutName = scheduledWorkout.WorkoutSchedule.WorkoutMaster.WorkoutName;
                    workoutDetail.ID = scheduledWorkout.WorkoutSchedule.WorkoutMaster.ID;
                    workoutDetail.CreatedDate = scheduledWorkout.WorkoutSchedule.ScheduledDate;
                    workoutList.Add(workoutDetail);
                }
            }

            var userMappedWorkout = _dbcontext.WorkoutUserMappings.Where(x => x.LocationId == userdata.BranchId && x.UserId == userdata.UserId && x.IsActive == true).Select(x => new ModelWorkout
            {
                WorkoutName = x.WorkoutMaster.WorkoutName,
                ID = x.WorkoutMaster.ID
            }).ToList();
            if (userMappedWorkout != null && userMappedWorkout.Count > 0)
                workoutList.AddRange(userMappedWorkout);
            return workoutList.OrderBy(e => e.CreatedDate).Distinct().ToList();
        }

        public List<ModelBoxerWorkout> GetFutureBoxerWorkoutForLocation(UserData userdata)
        {
            var boxerWorkoutList = new List<ModelBoxerWorkout>();

            var scheduledBoxerWorkouts = _dbcontext.LocationSelectedBoxerWorkouts.Where(e => e.LocationID == userdata.BranchId && e.IsActive == true && e.IsDeleted == false);
            foreach (var scheduledBoxerWorkout in scheduledBoxerWorkouts)
            {
                if (scheduledBoxerWorkout.BoxerWorkoutSchedule.ScheduledDate.Date >= DateTime.Now.Date && scheduledBoxerWorkout.BoxerWorkoutSchedule.ScheduledDate.Date <= DateTime.Now.Date.AddDays(7) && scheduledBoxerWorkout.BoxerWorkoutSchedule.IsActive == true)
                {
                    var boxerWorkoutDetail = new ModelBoxerWorkout();
                    boxerWorkoutDetail.WorkoutName = scheduledBoxerWorkout.BoxerWorkoutSchedule.BoxerWorkoutMaster.WorkoutName;
                    boxerWorkoutDetail.ID = scheduledBoxerWorkout.BoxerWorkoutSchedule.BoxerWorkoutMaster.ID;
                    boxerWorkoutDetail.CreatedDate = scheduledBoxerWorkout.BoxerWorkoutSchedule.ScheduledDate;
                    boxerWorkoutList.Add(boxerWorkoutDetail);
                }
            }

            var userMappedBoxerWorkout = _dbcontext.BoxerWorkoutUserMappings.Where(x => x.LocationId == userdata.BranchId && x.UserId == userdata.UserId && x.IsActive == true).Select(x => new ModelBoxerWorkout
            {
                WorkoutName = x.BoxerWorkoutMaster.WorkoutName,
                ID = x.BoxerWorkoutMaster.ID
            }).ToList();
            if (userMappedBoxerWorkout != null && userMappedBoxerWorkout.Count > 0)
                boxerWorkoutList.AddRange(userMappedBoxerWorkout);
            return boxerWorkoutList.OrderBy(e => e.CreatedDate).Distinct().ToList();
        }


        public int ScheduleWorkout(ModelWorkoutSchedule request)
        {
            try
            {
                var scheduledWorkout = new WorkoutSchedule();
                if (request.WorkoutScheduleId > 0)
                {
                    scheduledWorkout = _dbcontext.WorkoutSchedules.FirstOrDefault(e => e.ID == request.WorkoutScheduleId);
                }
                else
                {
                    scheduledWorkout = _dbcontext.WorkoutSchedules.FirstOrDefault(e => e.LocationId == request.LocationId && e.WorkoutMasterID == request.WorkoutId && e.IsDeleted != true &&
               DbFunctions.DiffDays(e.ScheduledDate, request.ScheduledDate) == 0);
                }

                var isDuplicate = false;
                if (scheduledWorkout != null && scheduledWorkout.ScheduledDate.Date == request.ScheduledDate.Date && request.WorkoutScheduleId == 0)
                {
                    isDuplicate = true;
                    return 1;
                }
                if (scheduledWorkout == null && isDuplicate != true)
                {
                    var newWorkoutSchedule = new WorkoutSchedule()
                    {
                        WorkoutMasterID = request.WorkoutId,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow,
                        ScheduledDate = request.ScheduledDate,
                        IsActive = request.IsActive,
                        LocationId = request.LocationId,
                        IsDeleted = false
                    };

                    _dbcontext.WorkoutSchedules.Add(newWorkoutSchedule);


                }
                else
                {
                    var workoutId = request.WorkoutId > 0 ? request.WorkoutId : scheduledWorkout.WorkoutMasterID;
                    var duplicateScheduledWorkout = _dbcontext.WorkoutSchedules.Where(e => e.WorkoutMasterID == workoutId && e.LocationId == request.LocationId && e.IsDeleted != true).ToList();
                    var isFound = false;
                    var count = 0;
                    if (duplicateScheduledWorkout != null)
                    {
                        foreach (var item in duplicateScheduledWorkout)
                        {
                            if (item.ScheduledDate.Date == request.ScheduledDate.Date)
                            {
                                // if (count > 1)
                                // {
                                //scheduledWorkout.IsDeleted = true;
                                //scheduledWorkout.IsActive = false;
                                // item.IsDeleted = true;
                                // item.IsActive = false;
                                // _dbcontext.SaveChanges();
                                //}
                                isFound = true;
                                if (item.ID == request.WorkoutScheduleId)
                                {
                                    count++;
                                    item.IsActive = request.IsActive;
                                    item.IsDeleted = request.IsDeleted;
                                }
                                if (item.ID != request.WorkoutScheduleId && item.LocationId == request.LocationId && item.WorkoutMasterID == request.WorkoutId)
                                {
                                    //if (item.IsActive == request.IsActive && item.IsDeleted == request.IsDeleted)
                                    // {
                                    isDuplicate = true;
                                    //}

                                }
                            }
                        }

                    }
                    if (isDuplicate)
                    {
                        //scheduledWorkout.IsDeleted = request.IsDeleted;
                        //scheduledWorkout.IsActive = request.IsActive;
                        //scheduledWorkout.ScheduledDate = request.ScheduledDate == DateTime.MinValue ? scheduledWorkout.ScheduledDate : request.ScheduledDate;
                        //_dbcontext.SaveChanges();
                        return 1;

                    }
                    if (isFound || request.WorkoutScheduleId != 0)

                    {
                        scheduledWorkout.IsDeleted = request.IsDeleted;
                        scheduledWorkout.IsActive = request.IsActive;
                        scheduledWorkout.ScheduledDate = request.ScheduledDate == DateTime.MinValue ? scheduledWorkout.ScheduledDate : request.ScheduledDate;
                        _dbcontext.SaveChanges();
                        //scheduledWorkout.WorkoutMasterID = request.WorkoutId > 0 ? request.WorkoutId : scheduledWorkout.WorkoutMasterID;
                        if (count <= 1)
                        {
                            return 0;
                        }
                        return 1;
                    }
                    //else if(request.WorkoutScheduleId == 0 && isFound)
                    //{
                    //    return 1;
                    //}

                }
                _dbcontext.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                return 2;
            }

        }

        public int ScheduleBoxerWorkout(ModelBoxerWorkoutSchedule request)
        {
            try
            {
                var scheduledBoxerWorkout = new BoxerWorkoutSchedule();
                if (request.BoxerWorkoutScheduleId > 0)
                {
                    scheduledBoxerWorkout = _dbcontext.BoxerWorkoutSchedules.FirstOrDefault(e => e.ID == request.BoxerWorkoutScheduleId);
                }
                else
                {
                    scheduledBoxerWorkout = _dbcontext.BoxerWorkoutSchedules.FirstOrDefault(e => e.LocationId == request.LocationId && e.BoxerWorkoutMasterID == request.BoxerWorkoutId && e.IsDeleted != true &&
               DbFunctions.DiffDays(e.ScheduledDate, request.ScheduledDate) == 0);
                }

                var isDuplicate = false;
                if (scheduledBoxerWorkout != null && scheduledBoxerWorkout.ScheduledDate.Date == request.ScheduledDate.Date && request.BoxerWorkoutScheduleId == 0)
                {
                    isDuplicate = true;
                    return 1;
                }
                if (scheduledBoxerWorkout == null && isDuplicate != true)
                {
                    var newWorkoutSchedule = new BoxerWorkoutSchedule()
                    {
                        BoxerWorkoutMasterID = request.BoxerWorkoutId,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow,
                        ScheduledDate = request.ScheduledDate,
                        IsActive = request.IsActive,
                        LocationId = request.LocationId,
                        IsDeleted = false
                    };

                    _dbcontext.BoxerWorkoutSchedules.Add(newWorkoutSchedule);


                }
                else
                {
                    var workoutId = request.BoxerWorkoutId > 0 ? request.BoxerWorkoutId : scheduledBoxerWorkout.BoxerWorkoutMasterID;
                    var duplicateScheduledWorkout = _dbcontext.WorkoutSchedules.Where(e => e.WorkoutMasterID == workoutId && e.LocationId == request.LocationId && e.IsDeleted != true).ToList();
                    var isFound = false;
                    var count = 0;
                    if (duplicateScheduledWorkout != null)
                    {
                        foreach (var item in duplicateScheduledWorkout)
                        {
                            if (item.ScheduledDate.Date == request.ScheduledDate.Date)
                            {
                                // if (count > 1)
                                // {
                                //scheduledWorkout.IsDeleted = true;
                                //scheduledWorkout.IsActive = false;
                                // item.IsDeleted = true;
                                // item.IsActive = false;
                                // _dbcontext.SaveChanges();
                                //}
                                isFound = true;
                                if (item.ID == request.BoxerWorkoutScheduleId)
                                {
                                    count++;
                                    item.IsActive = request.IsActive;
                                    item.IsDeleted = request.IsDeleted;
                                }
                                if (item.ID != request.BoxerWorkoutScheduleId && item.LocationId == request.LocationId && item.WorkoutMasterID == request.BoxerWorkoutId)
                                {
                                    //if (item.IsActive == request.IsActive && item.IsDeleted == request.IsDeleted)
                                    // {
                                    isDuplicate = true;
                                    //}

                                }
                            }
                        }

                    }
                    if (isDuplicate)
                    {
                        //scheduledWorkout.IsDeleted = request.IsDeleted;
                        //scheduledWorkout.IsActive = request.IsActive;
                        //scheduledWorkout.ScheduledDate = request.ScheduledDate == DateTime.MinValue ? scheduledWorkout.ScheduledDate : request.ScheduledDate;
                        //_dbcontext.SaveChanges();
                        return 1;

                    }
                    if (isFound || request.BoxerWorkoutScheduleId != 0)

                    {
                        scheduledBoxerWorkout.IsDeleted = request.IsDeleted;
                        scheduledBoxerWorkout.IsActive = request.IsActive;
                        scheduledBoxerWorkout.ScheduledDate = request.ScheduledDate == DateTime.MinValue ? scheduledBoxerWorkout.ScheduledDate : request.ScheduledDate;
                        _dbcontext.SaveChanges();
                        //scheduledWorkout.WorkoutMasterID = request.WorkoutId > 0 ? request.WorkoutId : scheduledWorkout.WorkoutMasterID;
                        if (count <= 1)
                        {
                            return 0;
                        }
                        return 1;
                    }
                    //else if(request.WorkoutScheduleId == 0 && isFound)
                    //{
                    //    return 1;
                    //}

                }
                _dbcontext.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                return 2;
            }

        }


        public int BulkScheduleWorkout(ModelWorkoutBulkSchedule request)
        {
            try
            {
                foreach (var workoutSchedule in request.BulkScheduledSelectedWorkout)
                {
                    var scheduledWorkout = _dbcontext.WorkoutSchedules.FirstOrDefault(e => e.LocationId == workoutSchedule.LocationId
                                             && e.WorkoutMasterID == request.WorkoutId && e.IsDeleted != true &&
                                             DbFunctions.DiffDays(e.ScheduledDate, request.ScheduledDate) == 0);
                    if (scheduledWorkout == null)
                    {
                        var newWorkoutSchedule = new WorkoutSchedule()
                        {
                            WorkoutMasterID = request.WorkoutId,
                            CreatedDate = DateTime.UtcNow,
                            UpdatedDate = DateTime.UtcNow,
                            ScheduledDate = request.ScheduledDate,
                            IsActive = workoutSchedule.IsActive,
                            LocationId = workoutSchedule.LocationId,
                            IsDeleted = false
                        };
                        _dbcontext.WorkoutSchedules.Add(newWorkoutSchedule);
                        _dbcontext.SaveChanges();

                        if (request.BulkScheduledActiveWorkout != null && request.BulkScheduledActiveWorkout.Count > 0)
                        {
                            var activeWorkoutLocation = request.BulkScheduledActiveWorkout.FirstOrDefault(e => e.WorkoutId == workoutSchedule.WorkoutId && e.LocationId == workoutSchedule.LocationId); ;
                            if (activeWorkoutLocation != null)
                            {
                                newWorkoutSchedule.IsActive = activeWorkoutLocation.IsActive;
                                var newWorkoutLocationSelection = new LocationSelectedWorkout()
                                {
                                    IsActive = activeWorkoutLocation.IsActive,
                                    LocationID = workoutSchedule.LocationId,
                                    IsDeleted = false,
                                    WorkoutScheduleID = newWorkoutSchedule.ID,
                                    CreatedDate = DateTime.UtcNow,
                                    UpdatedDate = DateTime.UtcNow
                                };
                                _dbcontext.LocationSelectedWorkouts.Add(newWorkoutLocationSelection);
                                _dbcontext.SaveChanges();
                            }
                        }



                    }
                }


                foreach (var workoutSchedule in request.BulkScheduledSelectedWorkout)
                {
                    var scheduledWorkout = _dbcontext.WorkoutSchedules.FirstOrDefault(e => e.LocationId == workoutSchedule.LocationId
                                            && e.WorkoutMasterID == request.WorkoutId && e.IsDeleted != true &&
                                            DbFunctions.DiffDays(e.ScheduledDate, request.ScheduledDate) == 0);
                }

                return 0;
            }
            catch (Exception ex)
            {
                return 2;
            }

        }

        public int BulkScheduleBoxerWorkout(ModelBoxerWorkoutBulkSchedule request)
        {
            try
            {
                foreach (var boxerWorkoutSchedule in request.BulkScheduledSelectedBoxerWorkout)
                {
                    var scheduledBoxerWorkout = _dbcontext.BoxerWorkoutSchedules.FirstOrDefault(e => e.LocationId == boxerWorkoutSchedule.LocationId
                                             && e.BoxerWorkoutMasterID == request.BoxerWorkoutId && e.IsDeleted != true &&
                                             DbFunctions.DiffDays(e.ScheduledDate, request.ScheduledDate) == 0);
                    if (scheduledBoxerWorkout == null)
                    {
                        var newBoxerWorkoutSchedule = new BoxerWorkoutSchedule()
                        {
                            BoxerWorkoutMasterID = request.BoxerWorkoutId,
                            CreatedDate = DateTime.UtcNow,
                            UpdatedDate = DateTime.UtcNow,
                            ScheduledDate = request.ScheduledDate,
                            IsActive = boxerWorkoutSchedule.IsActive,
                            LocationId = boxerWorkoutSchedule.LocationId,
                            IsDeleted = false
                        };
                        _dbcontext.BoxerWorkoutSchedules.Add(newBoxerWorkoutSchedule);
                        _dbcontext.SaveChanges();

                        if (request.BulkScheduledActiveBoxerWorkout != null && request.BulkScheduledActiveBoxerWorkout.Count > 0)
                        {
                            var activeWorkoutLocation = request.BulkScheduledActiveBoxerWorkout.FirstOrDefault(e => e.BoxerWorkoutId == boxerWorkoutSchedule.BoxerWorkoutId && e.LocationId == boxerWorkoutSchedule.LocationId); ;
                            if (activeWorkoutLocation != null)
                            {
                                newBoxerWorkoutSchedule.IsActive = activeWorkoutLocation.IsActive;
                                var newWorkoutLocationSelection = new LocationSelectedBoxerWorkout()
                                {
                                    IsActive = activeWorkoutLocation.IsActive,
                                    LocationID = boxerWorkoutSchedule.LocationId,
                                    IsDeleted = false,
                                    BoxerWorkoutScheduleID = newBoxerWorkoutSchedule.ID,
                                    CreatedDate = DateTime.UtcNow,
                                    UpdatedDate = DateTime.UtcNow
                                };
                                _dbcontext.LocationSelectedBoxerWorkouts.Add(newWorkoutLocationSelection);
                                _dbcontext.SaveChanges();
                            }
                        }



                    }
                }


                foreach (var workoutSchedule in request.BulkScheduledSelectedBoxerWorkout)
                {
                    var scheduledWorkout = _dbcontext.WorkoutSchedules.FirstOrDefault(e => e.LocationId == workoutSchedule.LocationId
                                            && e.WorkoutMasterID == request.BoxerWorkoutId && e.IsDeleted != true &&
                                            DbFunctions.DiffDays(e.ScheduledDate, request.ScheduledDate) == 0);
                }

                return 0;
            }
            catch (Exception ex)
            {
                return 2;
            }

        }


        public ModelWorkoutSchedule GetScheduledWorkoutDetail(int WorkoutScheduleId)
        {
            try
            {
                var scheduledWorkout = _dbcontext.WorkoutSchedules.Where(e => e.ID == WorkoutScheduleId).Select(e => new ModelWorkoutSchedule()
                {
                    ScheduledDate = e.ScheduledDate,
                    WorkoutId = e.WorkoutMasterID,
                    WorkoutScheduleId = e.ID,
                    WorkoutName = e.WorkoutMaster.WorkoutName,
                    IsActiveString = e.IsActive.HasValue ? (e.IsActive.Value == true ? "on" : "") : ""
                }).FirstOrDefault();

                return scheduledWorkout;

            }
            catch (Exception)
            {
                return null;
            }
        }


        public ModelBoxerWorkoutSchedule GetScheduledBoxerWorkoutDetail(int BoxerWorkoutScheduleId)
        {
            try
            {
                var scheduledBoxerWorkout = _dbcontext.BoxerWorkoutSchedules.Where(e => e.ID == BoxerWorkoutScheduleId).Select(e => new ModelBoxerWorkoutSchedule()
                {
                    ScheduledDate = e.ScheduledDate,
                    BoxerWorkoutId = e.BoxerWorkoutMasterID,
                    BoxerWorkoutScheduleId = e.ID,
                    BoxerWorkoutName = e.BoxerWorkoutMaster.WorkoutName,
                    IsActiveString = e.IsActive.HasValue ? (e.IsActive.Value == true ? "on" : "") : ""
                }).FirstOrDefault();

                return scheduledBoxerWorkout;

            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Make soft delete workout detail
        /// </summary>
        /// <param name="ID">Workout Id</param>
        /// <returns></returns>
        public CommonResultsMessage Delete(Int32 ID)
        {
            try
            {
                var olddata = _dbcontext.WorkoutMasters.FirstOrDefault(p => p.ID == ID);
                olddata.IsReadyForLocations = false;
                olddata.IsDeleted = true;
                olddata.WorkoutName = olddata.WorkoutName + "_Deleted";
                foreach (var workoutSchedule in olddata.WorkoutSchedules)
                {
                    workoutSchedule.IsDeleted = true;
                }
                foreach (var workoutUserMapping in olddata.WorkoutUserMappings)
                {
                    workoutUserMapping.IsActive = false;
                }
                _dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
                _dbcontext.SaveChanges();
                return CommonResultsMessage.Update;
            }
            catch (Exception)
            {
                return CommonResultsMessage.Fail;
            }
        }
        public CommonResultsMessage DeleteBoxer(Int32 ID)
        {
            try
            {
                var olddata = _dbcontext.BoxerWorkoutMasters.FirstOrDefault(p => p.ID == ID);
                olddata.IsReadyForLocations = false;
                olddata.IsDeleted = true;
                olddata.WorkoutName = olddata.WorkoutName + "_Deleted";
                foreach (var workoutSchedule in olddata.BoxerWorkoutSchedules)
                {
                    workoutSchedule.IsDeleted = true;
                }
                foreach (var workoutUserMapping in olddata.BoxerWorkoutUserMappings)
                {
                    workoutUserMapping.IsActive = false;
                }
                _dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
                _dbcontext.SaveChanges();
                return CommonResultsMessage.Update;
            }
            catch (Exception)
            {
                return CommonResultsMessage.Fail;
            }
        }

        public CommonResultsMessage BulkDelete(List<int> IDs)
        {
            try
            {
                var allWorkouts = _dbcontext.WorkoutMasters.Where(x => IDs.Contains(x.ID)).ToList();
                foreach (var olddata in allWorkouts)
                {
                    //var olddata = _dbcontext.WorkoutMasters.FirstOrDefault(p => p.ID == ID);
                    olddata.IsReadyForLocations = false;
                    olddata.IsDeleted = true;
                    olddata.WorkoutName = olddata.WorkoutName + "_Deleted";

                    foreach (var workoutSchedule in olddata.WorkoutSchedules)
                    {
                        workoutSchedule.IsDeleted = true;
                    }
                    foreach (var workoutUserMapping in olddata.WorkoutUserMappings)
                    {
                        workoutUserMapping.IsActive = false;
                    }
                }
                //_dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
                _dbcontext.SaveChanges();
                return CommonResultsMessage.Update;
            }
            catch (Exception)
            {
                return CommonResultsMessage.Fail;
            }
        }
        public CommonResultsMessage BulkDeleteBoxer(List<int> IDs)
        {
            try
            {
                var allWorkouts = _dbcontext.BoxerWorkoutMasters.Where(x => IDs.Contains(x.ID)).ToList();
                foreach (var olddata in allWorkouts)
                {
                    //var olddata = _dbcontext.WorkoutMasters.FirstOrDefault(p => p.ID == ID);
                    olddata.IsReadyForLocations = false;
                    olddata.IsDeleted = true;
                    olddata.WorkoutName = olddata.WorkoutName + "_Deleted";

                    foreach (var workoutSchedule in olddata.BoxerWorkoutSchedules)
                    {
                        workoutSchedule.IsDeleted = true;
                    }
                    foreach (var workoutUserMapping in olddata.BoxerWorkoutUserMappings)
                    {
                        workoutUserMapping.IsActive = false;
                    }
                }
                //_dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
                _dbcontext.SaveChanges();
                return CommonResultsMessage.Update;
            }
            catch (Exception)
            {
                return CommonResultsMessage.Fail;
            }
        }

        public CommonResultsMessage BulkMove(int CategoryId, List<int> IDs)
        {
            try
            {
                var workoutCategoryDetail = _dbcontext.WorkoutCategories.FirstOrDefault(x => x.Id == CategoryId);
                if (workoutCategoryDetail == null)
                {
                    return CommonResultsMessage.Fail;
                }
                var allWorkouts = _dbcontext.WorkoutMasters.Where(x => IDs.Contains(x.ID)).ToList();
                foreach (var olddata in allWorkouts)
                {
                    olddata.CategoryId = workoutCategoryDetail.Id;
                }
                _dbcontext.SaveChanges();
                return CommonResultsMessage.Update;
            }
            catch (Exception)
            {
                return CommonResultsMessage.Fail;
            }
        }
        public CommonResultsMessage BulkMoveBoxer(int CategoryId, List<int> IDs)
        {
            try
            {
                var workoutCategoryDetail = _dbcontext.WorkoutCategories.FirstOrDefault(x => x.Id == CategoryId);
                if (workoutCategoryDetail == null)
                {
                    return CommonResultsMessage.Fail;
                }
                var allWorkouts = _dbcontext.BoxerWorkoutMasters.Where(x => IDs.Contains(x.ID)).ToList();
                foreach (var olddata in allWorkouts)
                {
                    olddata.CategoryId = workoutCategoryDetail.Id;
                }
                _dbcontext.SaveChanges();
                return CommonResultsMessage.Update;
            }
            catch (Exception)
            {
                return CommonResultsMessage.Fail;
            }
        }

        /// <summary>
        /// Save workout is ready for branch or not.
        /// </summary>
        /// <param name="ID">Workout Id</param>
        /// <param name="IsYes">True or False</param>
        /// <returns></returns>
        public CommonResultsMessage WorkoutIsReadyForLocations(Int32 ID, bool IsYes = false)
        {
            try
            {
                var olddata = _dbcontext.WorkoutMasters.FirstOrDefault(p => p.ID == ID);
                olddata.IsReadyForLocations = IsYes;
                _dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
                _dbcontext.SaveChanges();
                return CommonResultsMessage.Update;
            }
            catch (Exception)
            {
                return CommonResultsMessage.Fail;
            }
        }
        public CommonResultsMessage WorkoutIsReadyForLocationsBoxer(Int32 ID, bool IsYes = false)
        {
            try
            {
                var olddata = _dbcontext.BoxerWorkoutMasters.FirstOrDefault(p => p.ID == ID);
                olddata.IsReadyForLocations = IsYes;
                _dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
                _dbcontext.SaveChanges();
                return CommonResultsMessage.Update;
            }
            catch (Exception)
            {
                return CommonResultsMessage.Fail;
            }
        }
        /// <summary>
        /// Retrieve workout detail by Id.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>

        public ModelWorkout GetByID(Int32 ID)
        {
            try
            {
                return DalToModel(_dbcontext.WorkoutMasters.FirstOrDefault(p => p.ID == ID));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ModelBoxerWorkout GetByIDBoxer(Int32 ID)
        {
            try
            {
                return DalToModel(_dbcontext.BoxerWorkoutMasters.FirstOrDefault(p => p.ID == ID));
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Workout Template

        /// <summary>
        /// Retrieve Workout Template List
        /// </summary>
        /// <param name="workoutId">Workout Id</param>
        /// <returns></returns>
        public WorkoutDetailModel GetWorkoutTemplateDetail(int workoutId)
        {
            try
            {
                var workoutDetail = new WorkoutDetailModel();
                var workoutInfo = _dbcontext.WorkoutMasters.Include("WorkoutTemplateMappings").FirstOrDefault(e => e.ID == workoutId);
                var templateResult = _dbcontext.Templates.Include("TemplateVideoMappings").Include("TemplateVideoMappings.Video").Where(e => e.IsDeleted == false).ToList();
                var allTemplates = templateResult.Select(e => new TemplateDetail
                {
                    TemplateId = e.ID,
                    TemplateName = e.TemplateName,
                    //Description = e.TemplateDescription,
                    NumberOfBasicVideos = e.NumberOfBasicVideos,
                    NumberOfAlternateVideos = e.NumberOfAlterVideos,
                    PrimaryText = e.PrimaryText,
                    AlternateText = e.AlternateText,
                    PrimaryColor = e.PrimaryColor,
                    AlternateColor = e.AlternateColor,
                    PrimaryBackgroundColor = e.PrimaryBackgroundColor,
                    AlternateBackgroundColor = e.AlternateBackgroundColor,
                    PrimaryGradientColor = e.PrimaryGradientColor,
                    AlternateGradientColor = e.AlternateGradientColor,
                    IsSelected = false,
                    IsFooterText = Convert.ToBoolean(e.IsFooterText),
                    FooterText = e.FooterText,
                    FooterTextColor = e.FooterTextColor,
                    GoalText = e.GoalText == null ? " " : e.GoalText,
                    TimeText = e.TimeText == null ? " " : e.TimeText,
                    BlockLineOneText = e.BlockLineOneText == null ? " " : e.BlockLineOneText,
                    BlockLineTwoText = e.BlockLineTwoText == null ? " " : e.BlockLineTwoText,
                    BasicVideos = e.TemplateVideoMappings.Where(tv => tv.TemplateID == e.ID && tv.IsBasicVideo && !tv.IsDeleted).Select(tv => new TemplateVideoDetail
                    {
                        IsAlterVideo = tv.IsAlterVideo,
                        IsBasicVideo = tv.IsBasicVideo,
                        VideoFile = tv.Video.VideoAttachment,
                        VideoId = tv.Video.ID,
                        VideoPosition = Convert.ToInt32(tv.VideoPosition),
                        VideoTitle = tv.Video.VideoTitle,
                        SmallDescription = tv.Video.SmallDescription,
                        Note = tv.Note == null || tv.Note == "0"? " " : tv.Note,
                        Reps = tv.Reps == null || tv.Reps == "0" ? " " : tv.Reps
                    }).OrderBy(tv => tv.VideoPosition).ToList(),
                    AlternateVideos = e.TemplateVideoMappings.Where(tv => tv.TemplateID == e.ID && tv.IsAlterVideo && !tv.IsDeleted).Select(tv => new TemplateVideoDetail
                    {
                        IsAlterVideo = tv.IsAlterVideo,
                        IsBasicVideo = tv.IsBasicVideo,
                        VideoFile = tv.Video.VideoAttachment,
                        VideoId = tv.Video.ID,
                        VideoPosition = Convert.ToInt32(tv.VideoPosition),
                        VideoTitle = tv.Video.VideoTitle,
                        SmallDescription = tv.Video.SmallDescription,
                        Note = tv.Note == null || tv.Note == "0"? " " : tv.Note,
                        Reps = tv.Reps == null || tv.Reps == "0" ? " " : tv.Reps
                    }).OrderBy(tv => tv.VideoPosition).ToList(),
                }).ToList();


                if (workoutInfo != null)
                {
                    workoutDetail.WorkoutId = workoutInfo.ID;
                    workoutDetail.WorkoutName = workoutInfo.WorkoutName;
                    workoutDetail.FrontCoverImageId = workoutInfo.FrontCoverImageId;
                    workoutDetail.FrontCoverImageName = workoutInfo.ImageGallery1 != null ? workoutInfo.ImageGallery1.ImageFile : string.Empty;

                    workoutDetail.BackCoverImageId = workoutInfo.BackCoverImageId;
                    workoutDetail.BackCoverImageName = workoutInfo.ImageGallery != null ? workoutInfo.ImageGallery.ImageFile : string.Empty;
                    if (workoutInfo.WorkoutTemplateMappings != null && workoutInfo.WorkoutTemplateMappings.Count > 0)
                    {
                        // allTemplates.ForEach(x => x.IsSelected = && x.WorkoutTemplateMapId == workoutInfo.WorkoutTemplateMappings.FirstOrDefault(y => y.TemplateID == x.TemplateId).ID);
                        //foreach (var template in allTemplates)
                        //{
                        //	//template.IsSelected = workoutInfo.WorkoutTemplateMappings.Any(y => y.TemplateID == template.TemplateId && y.IsActive==true);
                        //	var workoutTemplateMap = workoutInfo.WorkoutTemplateMappings.FirstOrDefault(y => y.TemplateID == template.TemplateId);
                        //	if (workoutTemplateMap != null)
                        //	{
                        //		template.WorkoutTemplateMapId = workoutTemplateMap.ID;
                        //		template.IsDeleted = Convert.ToBoolean(workoutTemplateMap.IsDeleted);
                        //		template.IsSelected = Convert.ToBoolean(workoutTemplateMap.IsActive);
                        //		template.DisplayOrder = workoutTemplateMap.DisplayOrder;
                        //	}

                        //}
                        var workoutMappedTemplates = workoutInfo.WorkoutTemplateMappings.Where(y => y.IsDeleted != true && y.WorkoutMaster.IsDeleted != true && y.Template.IsDeleted != true).OrderBy(x => x.DisplayOrder).ToList();
                        workoutDetail.Templates = new List<TemplateDetail>();
                        foreach (var template in workoutMappedTemplates)
                        {
                            var templateDetail = templateResult.Where(x => x.ID == template.TemplateID).Select(e => new TemplateDetail
                            {
                                TemplateId = e.ID,
                                TemplateName = e.TemplateName,
                                //    Description = e.TemplateDescription,
                                NumberOfBasicVideos = e.NumberOfBasicVideos,
                                NumberOfAlternateVideos = e.NumberOfAlterVideos,
                                PrimaryText = e.PrimaryText,
                                AlternateText = e.AlternateText,
                                PrimaryColor = e.PrimaryColor,
                                AlternateColor = e.AlternateColor,
                                PrimaryBackgroundColor = e.PrimaryBackgroundColor,
                                AlternateBackgroundColor = e.AlternateBackgroundColor,
                                PrimaryGradientColor = e.PrimaryGradientColor,
                                AlternateGradientColor = e.AlternateGradientColor,
                                IsSelected = false,
                                IsFooterText = Convert.ToBoolean(e.IsFooterText),
                                FooterText = e.FooterText,
                                GoalText = e.GoalText == null ? " " : e.GoalText,
                                TimeText = e.TimeText == null ? " " : e.TimeText,
                                BlockLineOneText = e.BlockLineOneText == null ? " " : e.BlockLineOneText,
                                BlockLineTwoText = e.BlockLineTwoText == null ? " " : e.BlockLineTwoText,
                                FooterTextColor = e.FooterTextColor,
                                BasicVideos = e.TemplateVideoMappings.Where(tv => tv.TemplateID == e.ID && tv.IsBasicVideo && !tv.IsDeleted).Select(tv => new TemplateVideoDetail
                                {
                                    IsAlterVideo = tv.IsAlterVideo,
                                    IsBasicVideo = tv.IsBasicVideo,
                                    VideoFile = tv.Video.VideoAttachment,
                                    VideoId = tv.Video.ID,
                                    VideoPosition = Convert.ToInt32(tv.VideoPosition),
                                    VideoTitle = tv.Video.VideoTitle,
                                    SmallDescription = tv.Video.SmallDescription,
                                    Note = tv.Note == null || tv.Note == "0" ? " " : tv.Note,
                                    Reps = tv.Reps == null || tv.Reps == "0" ? " " : tv.Reps
                                }).OrderBy(tv => tv.VideoPosition).ToList(),
                                AlternateVideos = e.TemplateVideoMappings.Where(tv => tv.TemplateID == e.ID && tv.IsAlterVideo && !tv.IsDeleted).Select(tv => new TemplateVideoDetail
                                {
                                    IsAlterVideo = tv.IsAlterVideo,
                                    IsBasicVideo = tv.IsBasicVideo,
                                    VideoFile = tv.Video.VideoAttachment,
                                    VideoId = tv.Video.ID,
                                    VideoPosition = Convert.ToInt32(tv.VideoPosition),
                                    VideoTitle = tv.Video.VideoTitle,
                                    SmallDescription = tv.Video.SmallDescription,
                                    Note = tv.Note == null || tv.Note == "0" ? " " : tv.Note,
                                    Reps = tv.Reps == null || tv.Reps == "0" ? " " : tv.Reps
                                }).OrderBy(tv => tv.VideoPosition).ToList(),
                            }).FirstOrDefault();
                            // = new TemplateDetail();
                            //templateDetail = allTemplates.FirstOrDefault(x => x.TemplateId == template.TemplateID);
                            //templateDetail.TemplateId = template.TemplateID;

                            templateDetail.WorkoutTemplateMapId = template.ID;
                            templateDetail.IsDeleted = Convert.ToBoolean(template.IsDeleted);
                            templateDetail.IsSelected = Convert.ToBoolean(template.IsActive);
                            templateDetail.DisplayOrder = template.DisplayOrder;

                            workoutDetail.Templates.Add(templateDetail);


                        }
                        //var workoutMappedTemplates = _dbcontext.WorkoutTemplateMappings.Where(x=>x.WorkoutMasterID == workoutInfo.ID)
                    }
                }

                //workoutDetail.Templates = allTemplates.Where(e => e.IsDeleted == false && e.WorkoutTemplateMapId > 0).OrderBy(e => e.DisplayOrder).ToList();
                workoutDetail.Templates = workoutDetail.Templates != null ? workoutDetail.Templates.OrderBy(e => e.DisplayOrder).ToList() : null;
                return workoutDetail;
            }
            catch (Exception)
            {
                throw;
            }
        }


        // backup by parth
        //public BoxerWorkoutDetailModel GetWorkoutTemplateDetailBoxer(int boxerWorkoutId)
        //{
        //    try
        //    {
        //        var boxerWorkoutDetail = new BoxerWorkoutDetailModel();
        //        var boxerWorkoutInfo = _dbcontext.BoxerWorkoutMasters.Include("BoxerWorkoutTemplateMappings").FirstOrDefault(e => e.ID == boxerWorkoutId);
        //        var templateResult = _dbcontext.BoxerTemplates.Include("BoxerTemplateVideoMappings").Include("BoxerTemplateVideoMappings.Video").Where(e => e.IsDeleted == false).ToList();
        //        var allTemplates = templateResult.Select(e => new BoxerTemplateDetail
        //        {
        //            BoxerTemplateId = e.ID,
        //            BoxerTemplateName = e.TemplateName,
        //            //Description = e.TemplateDescription,
        //            NumberOfBasicVideos = e.NumberOfBasicVideos,
        //            NumberOfAlternateVideos = e.NumberOfAlterVideos,
        //            PrimaryText = e.PrimaryText,
        //            AlternateText = e.AlternateText,
        //            PrimaryColor = e.PrimaryColor,
        //            AlternateColor = e.AlternateColor,
        //            PrimaryBackgroundColor = e.PrimaryBackgroundColor,
        //            AlternateBackgroundColor = e.AlternateBackgroundColor,
        //            PrimaryGradientColor = e.PrimaryGradientColor,
        //            AlternateGradientColor = e.AlternateGradientColor,
        //            IsSelected = false,
        //            IsFooterText = Convert.ToBoolean(e.IsFooterText),
        //            FooterText = e.FooterText,
        //            FooterTextColor = e.FooterTextColor,
        //            GoalText = e.GoalText == null ? " " : e.GoalText,
        //            TimeText = e.TimeText == null ? " " : e.TimeText,
        //            BlockLineOneText = e.BlockLineOneText == null ? " " : e.BlockLineOneText,
        //            BlockLineTwoText = e.BlockLineTwoText == null ? " " : e.BlockLineTwoText,
        //            BasicVideos = e.BoxerTemplateVideoMappings.Where(tv => tv.BoxerTemplateID == e.ID && tv.IsBasicVideo && !tv.IsDeleted).Select(tv => new TemplateVideoDetail
        //            {
        //                IsAlterVideo = tv.IsAlterVideo,
        //                IsBasicVideo = tv.IsBasicVideo,
        //                VideoFile = tv.Video.VideoAttachment,
        //                VideoId = tv.Video.ID,
        //                VideoPosition = Convert.ToInt32(tv.VideoPosition),
        //                VideoTitle = tv.Video.VideoTitle,
        //                SmallDescription = tv.Video.SmallDescription,
        //                Note = tv.Note == null || tv.Note == "0" ? " " : tv.Note,
        //                Reps = tv.Reps == null || tv.Reps == "0" ? " " : tv.Reps
        //            }).OrderBy(tv => tv.VideoPosition).ToList(),
        //            AlternateVideos = e.BoxerTemplateVideoMappings.Where(tv => tv.BoxerTemplateID == e.ID && tv.IsAlterVideo && !tv.IsDeleted).Select(tv => new TemplateVideoDetail
        //            {
        //                IsAlterVideo = tv.IsAlterVideo,
        //                IsBasicVideo = tv.IsBasicVideo,
        //                VideoFile = tv.Video.VideoAttachment,
        //                VideoId = tv.Video.ID,
        //                VideoPosition = Convert.ToInt32(tv.VideoPosition),
        //                VideoTitle = tv.Video.VideoTitle,
        //                SmallDescription = tv.Video.SmallDescription,
        //                Note = tv.Note == null || tv.Note == "0" ? " " : tv.Note,
        //                Reps = tv.Reps == null || tv.Reps == "0" ? " " : tv.Reps
        //            }).OrderBy(tv => tv.VideoPosition).ToList(),
        //        }).ToList();


        //        if (boxerWorkoutInfo != null)
        //        {
        //            boxerWorkoutDetail.BoxerWorkoutId = boxerWorkoutInfo.ID;
        //            boxerWorkoutDetail.BoxerWorkoutName = boxerWorkoutInfo.WorkoutName;
        //            boxerWorkoutDetail.FrontCoverImageId = boxerWorkoutInfo.FrontCoverImageId;
        //            boxerWorkoutDetail.FrontCoverImageName = boxerWorkoutInfo.ImageGallery1 != null ? boxerWorkoutInfo.ImageGallery1.ImageFile : string.Empty;

        //            boxerWorkoutDetail.BackCoverImageId = boxerWorkoutInfo.BackCoverImageId;
        //            boxerWorkoutDetail.BackCoverImageName = boxerWorkoutInfo.ImageGallery != null ? boxerWorkoutInfo.ImageGallery.ImageFile : string.Empty;
        //            if (boxerWorkoutInfo.BoxerWorkoutTemplateMappings != null && boxerWorkoutInfo.BoxerWorkoutTemplateMappings.Count > 0)
        //            {
        //                // allTemplates.ForEach(x => x.IsSelected = && x.WorkoutTemplateMapId == workoutInfo.WorkoutTemplateMappings.FirstOrDefault(y => y.TemplateID == x.TemplateId).ID);
        //                //foreach (var template in allTemplates)
        //                //{
        //                //	//template.IsSelected = workoutInfo.WorkoutTemplateMappings.Any(y => y.TemplateID == template.TemplateId && y.IsActive==true);
        //                //	var workoutTemplateMap = workoutInfo.WorkoutTemplateMappings.FirstOrDefault(y => y.TemplateID == template.TemplateId);
        //                //	if (workoutTemplateMap != null)
        //                //	{
        //                //		template.WorkoutTemplateMapId = workoutTemplateMap.ID;
        //                //		template.IsDeleted = Convert.ToBoolean(workoutTemplateMap.IsDeleted);
        //                //		template.IsSelected = Convert.ToBoolean(workoutTemplateMap.IsActive);
        //                //		template.DisplayOrder = workoutTemplateMap.DisplayOrder;
        //                //	}

        //                //}
        //                var boxerWorkoutMappedTemplates = boxerWorkoutInfo.BoxerWorkoutTemplateMappings.Where(y => y.IsDeleted != true && y.BoxerWorkoutMaster.IsDeleted != true && y.BoxerTemplate.IsDeleted != true).OrderBy(x => x.DisplayOrder).ToList();
        //                boxerWorkoutDetail.BoxerTemplates = new List<BoxerTemplateDetail>();
        //                foreach (var template in boxerWorkoutMappedTemplates)
        //                {
        //                    var templateDetail = templateResult.Where(x => x.ID == template.BoxerTemplateID).Select(e => new BoxerTemplateDetail
        //                    {
        //                        BoxerTemplateId = e.ID,
        //                        BoxerTemplateName = e.TemplateName,
        //                        //    Description = e.TemplateDescription,
        //                        NumberOfBasicVideos = e.NumberOfBasicVideos,
        //                        NumberOfAlternateVideos = e.NumberOfAlterVideos,
        //                        PrimaryText = e.PrimaryText,
        //                        AlternateText = e.AlternateText,
        //                        PrimaryColor = e.PrimaryColor,
        //                        AlternateColor = e.AlternateColor,
        //                        PrimaryBackgroundColor = e.PrimaryBackgroundColor,
        //                        AlternateBackgroundColor = e.AlternateBackgroundColor,
        //                        PrimaryGradientColor = e.PrimaryGradientColor,
        //                        AlternateGradientColor = e.AlternateGradientColor,
        //                        IsSelected = false,
        //                        IsFooterText = Convert.ToBoolean(e.IsFooterText),
        //                        FooterText = e.FooterText,
        //                        GoalText = e.GoalText == null ? " " : e.GoalText,
        //                        TimeText = e.TimeText == null ? " " : e.TimeText,
        //                        BlockLineOneText = e.BlockLineOneText == null ? " " : e.BlockLineOneText,
        //                        BlockLineTwoText = e.BlockLineTwoText == null ? " " : e.BlockLineTwoText,
        //                        FooterTextColor = e.FooterTextColor,
        //                        BasicVideos = e.BoxerTemplateVideoMappings.Where(tv => tv.BoxerTemplateID == e.ID && tv.IsBasicVideo && !tv.IsDeleted).Select(tv => new TemplateVideoDetail
        //                        {
        //                            IsAlterVideo = tv.IsAlterVideo,
        //                            IsBasicVideo = tv.IsBasicVideo,
        //                            VideoFile = tv.Video.VideoAttachment,
        //                            VideoId = tv.Video.ID,
        //                            VideoPosition = Convert.ToInt32(tv.VideoPosition),
        //                            VideoTitle = tv.Video.VideoTitle,
        //                            SmallDescription = tv.Video.SmallDescription,
        //                            Note = tv.Note == null || tv.Note == "0" ? " " : tv.Note,
        //                            Reps = tv.Reps == null || tv.Reps == "0" ? " " : tv.Reps
        //                        }).OrderBy(tv => tv.VideoPosition).ToList(),
        //                        AlternateVideos = e.BoxerTemplateVideoMappings.Where(tv => tv.BoxerTemplateID == e.ID && tv.IsAlterVideo && !tv.IsDeleted).Select(tv => new TemplateVideoDetail
        //                        {
        //                            IsAlterVideo = tv.IsAlterVideo,
        //                            IsBasicVideo = tv.IsBasicVideo,
        //                            VideoFile = tv.Video.VideoAttachment,
        //                            VideoId = tv.Video.ID,
        //                            VideoPosition = Convert.ToInt32(tv.VideoPosition),
        //                            VideoTitle = tv.Video.VideoTitle,
        //                            SmallDescription = tv.Video.SmallDescription,
        //                            Note = tv.Note == null || tv.Note == "0" ? " " : tv.Note,
        //                            Reps = tv.Reps == null || tv.Reps == "0" ? " " : tv.Reps
        //                        }).OrderBy(tv => tv.VideoPosition).ToList(),
        //                    }).FirstOrDefault();
        //                    // = new TemplateDetail();
        //                    //templateDetail = allTemplates.FirstOrDefault(x => x.TemplateId == template.TemplateID);
        //                    //templateDetail.TemplateId = template.TemplateID;

        //                    templateDetail.BoxerWorkoutTemplateMapId = template.ID;
        //                    templateDetail.IsDeleted = Convert.ToBoolean(template.IsDeleted);
        //                    templateDetail.IsSelected = Convert.ToBoolean(template.IsActive);
        //                    templateDetail.DisplayOrder = template.DisplayOrder;

        //                    boxerWorkoutDetail.BoxerTemplates.Add(templateDetail);


        //                }
        //                //var workoutMappedTemplates = _dbcontext.WorkoutTemplateMappings.Where(x=>x.WorkoutMasterID == workoutInfo.ID)
        //            }
        //        }

        //        //workoutDetail.Templates = allTemplates.Where(e => e.IsDeleted == false && e.WorkoutTemplateMapId > 0).OrderBy(e => e.DisplayOrder).ToList();
        //        boxerWorkoutDetail.BoxerTemplates = boxerWorkoutDetail.BoxerTemplates != null ? boxerWorkoutDetail.BoxerTemplates.OrderBy(e => e.DisplayOrder).ToList() : null;
        //        return boxerWorkoutDetail;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}






        public BoxerWorkoutDetailModel GetWorkoutTemplateDetailBoxer(int boxerWorkoutId)
        {
            try
            {
                var boxerWorkoutDetail = new BoxerWorkoutDetailModel();
                var boxerWorkoutInfo = _dbcontext.BoxerWorkoutMasters.Include("BoxerWorkoutTemplateMappings").FirstOrDefault(e => e.ID == boxerWorkoutId);
                var templateResult = _dbcontext.BoxerTemplates.Include("BoxerTemplateVideoMappings").Include("BoxerTemplateVideoMappings.Video").Where(e => e.IsDeleted == false).ToList();
                var allTemplates = templateResult.Select(e => new BoxerTemplateDetail
                {
                    BoxerTemplateId = e.ID,
                    BoxerTemplateName = e.TemplateName,
                    //Description = e.TemplateDescription,
                    NumberOfBasicVideos = e.NumberOfBasicVideos,
                    NumberOfAlternateVideos = e.NumberOfAlterVideos,
                    PrimaryText = e.PrimaryText,
                    AlternateText = e.AlternateText,
                    PrimaryColor = e.PrimaryColor,
                    AlternateColor = e.AlternateColor,
                    PrimaryBackgroundColor = e.PrimaryBackgroundColor,
                    AlternateBackgroundColor = e.AlternateBackgroundColor,
                    PrimaryGradientColor = e.PrimaryGradientColor,
                    AlternateGradientColor = e.AlternateGradientColor,
                    IsSelected = false,
                    IsFooterText = Convert.ToBoolean(e.IsFooterText),
                    FooterText = e.FooterText,
                    FooterTextColor = e.FooterTextColor,
                    GoalText = e.GoalText == null ? " " : e.GoalText,
                    TimeText = e.TimeText == null ? " " : e.TimeText,
                    BlockLineOneText = e.BlockLineOneText == null ? " " : e.BlockLineOneText,
                    BlockLineTwoText = e.BlockLineTwoText == null ? " " : e.BlockLineTwoText,
                    BasicVideos = e.BoxerTemplateVideoMappings.Where(tv => tv.BoxerTemplateID == e.ID && tv.IsBasicVideo && !tv.IsDeleted).Select(tv => new TemplateVideoDetail
                    {
                        IsAlterVideo = tv.IsAlterVideo,
                        IsBasicVideo = tv.IsBasicVideo,
                        FileType = tv.FileType,
                        VideoFile = tv.FileType == "V" ? tv.Video.VideoAttachment : _dbcontext.ImageGalleries.Where(x => x.Id == tv.VideoID).FirstOrDefault().ImageFile,
                        VideoId = tv.FileType == "V" ?  tv.Video.ID : tv.VideoID,
                        VideoPosition = Convert.ToInt32(tv.VideoPosition),
                        VideoTitle = tv.Video.VideoTitle,
                        SmallDescription = tv.Video.SmallDescription,
                        Note = tv.Note == null || tv.Note == "0" ? " " : tv.Note,
                        Reps = tv.Reps == null || tv.Reps == "0" ? " " : tv.Reps
                    }).OrderBy(tv => tv.VideoPosition).ToList(),
                    AlternateVideos = e.BoxerTemplateVideoMappings.Where(tv => tv.BoxerTemplateID == e.ID && tv.IsAlterVideo && !tv.IsDeleted).Select(tv => new TemplateVideoDetail
                    {
                        IsAlterVideo = tv.IsAlterVideo,
                        IsBasicVideo = tv.IsBasicVideo,
                        FileType = tv.FileType,
                        VideoFile = tv.FileType == "V" ? tv.Video.VideoAttachment : _dbcontext.ImageGalleries.Where(x => x.Id == tv.VideoID).FirstOrDefault().ImageFile,
                        VideoId = tv.FileType == "V" ? tv.Video.ID : tv.VideoID,
                        VideoPosition = Convert.ToInt32(tv.VideoPosition),
                        VideoTitle = tv.Video.VideoTitle,
                        SmallDescription = tv.Video.SmallDescription,
                        Note = tv.Note == null || tv.Note == "0" ? " " : tv.Note,
                        Reps = tv.Reps == null || tv.Reps == "0" ? " " : tv.Reps
                    }).OrderBy(tv => tv.VideoPosition).ToList(),
                }).ToList();


                if (boxerWorkoutInfo != null)
                {
                    boxerWorkoutDetail.BoxerWorkoutId = boxerWorkoutInfo.ID;
                    boxerWorkoutDetail.BoxerWorkoutName = boxerWorkoutInfo.WorkoutName;
                    boxerWorkoutDetail.FrontCoverImageId = boxerWorkoutInfo.FrontCoverImageId;
                    boxerWorkoutDetail.FrontCoverImageName = boxerWorkoutInfo.ImageGallery1 != null ? boxerWorkoutInfo.ImageGallery1.ImageFile : string.Empty;

                    boxerWorkoutDetail.BackCoverImageId = boxerWorkoutInfo.BackCoverImageId;
                    boxerWorkoutDetail.BackCoverImageName = boxerWorkoutInfo.ImageGallery != null ? boxerWorkoutInfo.ImageGallery.ImageFile : string.Empty;
                    if (boxerWorkoutInfo.BoxerWorkoutTemplateMappings != null && boxerWorkoutInfo.BoxerWorkoutTemplateMappings.Count > 0)
                    {
                        // allTemplates.ForEach(x => x.IsSelected = && x.WorkoutTemplateMapId == workoutInfo.WorkoutTemplateMappings.FirstOrDefault(y => y.TemplateID == x.TemplateId).ID);
                        //foreach (var template in allTemplates)
                        //{
                        //	//template.IsSelected = workoutInfo.WorkoutTemplateMappings.Any(y => y.TemplateID == template.TemplateId && y.IsActive==true);
                        //	var workoutTemplateMap = workoutInfo.WorkoutTemplateMappings.FirstOrDefault(y => y.TemplateID == template.TemplateId);
                        //	if (workoutTemplateMap != null)
                        //	{
                        //		template.WorkoutTemplateMapId = workoutTemplateMap.ID;
                        //		template.IsDeleted = Convert.ToBoolean(workoutTemplateMap.IsDeleted);
                        //		template.IsSelected = Convert.ToBoolean(workoutTemplateMap.IsActive);
                        //		template.DisplayOrder = workoutTemplateMap.DisplayOrder;
                        //	}

                        //}
                        var boxerWorkoutMappedTemplates = boxerWorkoutInfo.BoxerWorkoutTemplateMappings.Where(y => y.IsDeleted != true && y.BoxerWorkoutMaster.IsDeleted != true && y.BoxerTemplate.IsDeleted != true).OrderBy(x => x.DisplayOrder).ToList();
                        boxerWorkoutDetail.BoxerTemplates = new List<BoxerTemplateDetail>();
                        foreach (var template in boxerWorkoutMappedTemplates)
                        {
                            var templateDetail = templateResult.Where(x => x.ID == template.BoxerTemplateID).Select(e => new BoxerTemplateDetail
                            {
                                BoxerTemplateId = e.ID,
                                BoxerTemplateName = e.TemplateName,
                                //    Description = e.TemplateDescription,
                                NumberOfBasicVideos = e.NumberOfBasicVideos,
                                NumberOfAlternateVideos = e.NumberOfAlterVideos,
                                PrimaryText = e.PrimaryText,
                                AlternateText = e.AlternateText,
                                PrimaryColor = e.PrimaryColor,
                                AlternateColor = e.AlternateColor,
                                PrimaryBackgroundColor = e.PrimaryBackgroundColor,
                                AlternateBackgroundColor = e.AlternateBackgroundColor,
                                PrimaryGradientColor = e.PrimaryGradientColor,
                                AlternateGradientColor = e.AlternateGradientColor,
                                IsSelected = false,
                                IsFooterText = Convert.ToBoolean(e.IsFooterText),
                                FooterText = e.FooterText,
                                GoalText = e.GoalText == null ? " " : e.GoalText,
                                TimeText = e.TimeText == null ? " " : e.TimeText,
                                BlockLineOneText = e.BlockLineOneText == null ? " " : e.BlockLineOneText,
                                BlockLineTwoText = e.BlockLineTwoText == null ? " " : e.BlockLineTwoText,
                                FooterTextColor = e.FooterTextColor,
                                BasicVideos = e.BoxerTemplateVideoMappings.Where(tv => tv.BoxerTemplateID == e.ID && tv.IsBasicVideo && !tv.IsDeleted).Select(tv => new TemplateVideoDetail
                                {
                                    IsAlterVideo = tv.IsAlterVideo,
                                    IsBasicVideo = tv.IsBasicVideo,
                                    FileType = tv.FileType,
                                    VideoFile = tv.FileType == "V" ? tv.Video.VideoAttachment : _dbcontext.ImageGalleries.Where(x => x.Id == tv.VideoID).FirstOrDefault().ImageFile,
                                    VideoId = tv.FileType == "V" ? tv.Video.ID : tv.VideoID,
                                    VideoPosition = Convert.ToInt32(tv.VideoPosition),
                                    VideoTitle = tv.Video.VideoTitle,
                                    SmallDescription = tv.Video.SmallDescription,
                                    Note = tv.Note == null || tv.Note == "0" ? " " : tv.Note,
                                    Reps = tv.Reps == null || tv.Reps == "0" ? " " : tv.Reps
                                }).OrderBy(tv => tv.VideoPosition).ToList(),
                                AlternateVideos = e.BoxerTemplateVideoMappings.Where(tv => tv.BoxerTemplateID == e.ID && tv.IsAlterVideo && !tv.IsDeleted).Select(tv => new TemplateVideoDetail
                                {
                                    IsAlterVideo = tv.IsAlterVideo,
                                    IsBasicVideo = tv.IsBasicVideo,
                                    FileType = tv.FileType,
                                    VideoFile = tv.FileType == "V" ? tv.Video.VideoAttachment : _dbcontext.ImageGalleries.Where(x => x.Id == tv.VideoID).FirstOrDefault().ImageFile,
                                    VideoId = tv.FileType == "V" ? tv.Video.ID : tv.VideoID,
                                    VideoPosition = Convert.ToInt32(tv.VideoPosition),
                                    VideoTitle = tv.Video.VideoTitle,
                                    SmallDescription = tv.Video.SmallDescription,
                                    Note = tv.Note == null || tv.Note == "0" ? " " : tv.Note,
                                    Reps = tv.Reps == null || tv.Reps == "0" ? " " : tv.Reps
                                }).OrderBy(tv => tv.VideoPosition).ToList(),
                            }).FirstOrDefault();
                            // = new TemplateDetail();
                            //templateDetail = allTemplates.FirstOrDefault(x => x.TemplateId == template.TemplateID);
                            //templateDetail.TemplateId = template.TemplateID;

                            templateDetail.BoxerWorkoutTemplateMapId = template.ID;
                            templateDetail.IsDeleted = Convert.ToBoolean(template.IsDeleted);
                            templateDetail.IsSelected = Convert.ToBoolean(template.IsActive);
                            templateDetail.DisplayOrder = template.DisplayOrder;

                            boxerWorkoutDetail.BoxerTemplates.Add(templateDetail);


                        }
                        //var workoutMappedTemplates = _dbcontext.WorkoutTemplateMappings.Where(x=>x.WorkoutMasterID == workoutInfo.ID)
                    }
                }

                //workoutDetail.Templates = allTemplates.Where(e => e.IsDeleted == false && e.WorkoutTemplateMapId > 0).OrderBy(e => e.DisplayOrder).ToList();
                boxerWorkoutDetail.BoxerTemplates = boxerWorkoutDetail.BoxerTemplates != null ? boxerWorkoutDetail.BoxerTemplates.OrderBy(e => e.DisplayOrder).ToList() : null;
                return boxerWorkoutDetail;
            }
            catch (Exception)
            {
                throw;
            }
        }



        /// <summary>
        /// Get Template detail for Workout
        /// </summary>
        /// <param name="workoutId">Workout Id</param>
        /// <param name="templateId">Template Id</param>
        /// <returns></returns>
        public TemplateDetail GetTemplateDetail(int workoutId, int templateId, bool isRead, int? templateMapId)
        {
            try
            {
                if (templateId == 0)
                    return null;
                var workoutDetail = new WorkoutDetailModel();
                var workoutInfo = _dbcontext.WorkoutMasters.Include("WorkoutTemplateMappings").FirstOrDefault(e => e.ID == workoutId);
                var templateResult = _dbcontext.Templates.Include("TemplateVideoMappings").Include("TemplateVideoMappings.Video").ToList();
                var templateDetail = templateResult.Where(e => e.ID == templateId).Select(e => new TemplateDetail
                {
                    TemplateId = e.ID,
                    TemplateName = e.TemplateName,
                    NumberOfBasicVideos = e.NumberOfBasicVideos,
                    NumberOfAlternateVideos = e.NumberOfAlterVideos,
                    IsSelected = false,
                    PrimaryText = e.PrimaryText,
                    AlternateText = e.AlternateText,
                    PrimaryColor = e.PrimaryColor,
                    AlternateColor = e.AlternateColor,
                    PrimaryGradientColor = e.PrimaryGradientColor,
                    AlternateGradientColor = e.AlternateGradientColor,
                    FooterText = e.FooterText,
                    GoalText = e.GoalText == null ? " " : e.GoalText,
                    TimeText = e.TimeText == null ? " " : e.TimeText,
                    BlockLineOneText = e.BlockLineOneText == null ? " " : e.BlockLineOneText,
                    BlockLineTwoText = e.BlockLineTwoText == null ? " " : e.BlockLineTwoText,
                    FooterTextColor = e.FooterTextColor,
                    BasicVideos = e.TemplateVideoMappings.Where(tv => tv.TemplateID == e.ID && tv.IsBasicVideo && !tv.IsDeleted).Select(tv => new TemplateVideoDetail
                    {
                        IsAlterVideo = tv.IsAlterVideo,
                        IsBasicVideo = tv.IsBasicVideo,
                        VideoFile = tv.Video.VideoAttachment,
                        VideoId = tv.Video.ID,
                        VideoPosition = Convert.ToInt32(tv.VideoPosition),
                        VideoTitle = tv.Video.VideoTitle,
                        SmallDescription = tv.Video.SmallDescription,
                        Note = tv.Note == null || tv.Note == "0"? " " : tv.Note,
                        Reps = tv.Reps == null || tv.Reps == "0" ? " " : tv.Reps
                    }).OrderBy(tv => tv.VideoPosition).ToList(),
                    AlternateVideos = e.TemplateVideoMappings.Where(tv => tv.TemplateID == e.ID && tv.IsAlterVideo && !tv.IsDeleted).Select(tv => new TemplateVideoDetail
                    {
                        IsAlterVideo = tv.IsAlterVideo,
                        IsBasicVideo = tv.IsBasicVideo,
                        VideoFile = tv.Video.VideoAttachment,
                        VideoId = tv.Video.ID,
                        VideoPosition = Convert.ToInt32(tv.VideoPosition),
                        VideoTitle = tv.Video.VideoTitle,
                        SmallDescription = tv.Video.SmallDescription,
                        Note = tv.Note == null || tv.Note == "0"? " " : tv.Note,
                        Reps = tv.Reps == null || tv.Reps == "0" ? " " : tv.Reps
                    }).OrderBy(tv => tv.VideoPosition).ToList(),
                }).FirstOrDefault();
                if (workoutInfo != null)
                {
                    workoutDetail.FrontCoverImageId = workoutInfo.FrontCoverImageId;
                    workoutDetail.FrontCoverImageName = workoutInfo.ImageGallery1 != null ? workoutInfo.ImageGallery1.ImageFile : string.Empty;

                    workoutDetail.BackCoverImageId = workoutInfo.BackCoverImageId;
                    workoutDetail.BackCoverImageName = workoutInfo.ImageGallery != null ? workoutInfo.ImageGallery.ImageFile : string.Empty;

                    if (workoutInfo.WorkoutTemplateMappings != null && workoutInfo.WorkoutTemplateMappings.Count > 0 && isRead)
                    {
                        //allTemplates.ForEach(x => x.IsSelected = && x.WorkoutTemplateMapId == workoutInfo.WorkoutTemplateMappings.FirstOrDefault(y => y.TemplateID == x.TemplateId).ID);
                        //template.IsSelected = workoutInfo.WorkoutTemplateMappings.Any(y => y.TemplateID == template.TemplateId && y.IsActive == true);
                        var workoutTemplateMapList = workoutInfo.WorkoutTemplateMappings.Where(y => y.TemplateID == templateDetail.TemplateId && y.IsDeleted != true).ToList();
                        if (templateMapId != null && templateMapId > 0)
                            workoutTemplateMapList = workoutInfo.WorkoutTemplateMappings.Where(y => y.TemplateID == templateDetail.TemplateId && y.IsDeleted != true && y.ID == templateMapId).ToList();
                        foreach (var workoutTemplateMap in workoutTemplateMapList)
                        {
                            if (workoutTemplateMap != null)
                            {
                                templateDetail.WorkoutTemplateMapId = workoutTemplateMap.ID;
                                templateDetail.IsDeleted = false;
                                templateDetail.IsSelected = Convert.ToBoolean(workoutTemplateMap.IsActive);
                                var workouttemplatemap = _dbcontext.WorkoutTemplateMappings.FirstOrDefault(e => e.ID == workoutTemplateMap.ID);
                                workouttemplatemap.IsDeleted = false;
                                _dbcontext.SaveChanges();
                            }
                            else
                            {
                                var newWorkoutTemplateMapDetail = new WorkoutTemplateMapping()
                                {
                                    TemplateID = templateId,
                                    CreatedDate = DateTime.UtcNow,
                                    UpdatedDate = DateTime.UtcNow,
                                    IsActive = true,
                                    IsDeleted = false,
                                    WorkoutMasterID = workoutId
                                };
                                templateDetail.IsSelected = true;
                                _dbcontext.WorkoutTemplateMappings.Add(newWorkoutTemplateMapDetail);
                                _dbcontext.SaveChanges();

                                templateDetail.WorkoutTemplateMapId = newWorkoutTemplateMapDetail.ID;

                            }
                        }

                    }
                    else
                    {
                        var newWorkoutTemplateMapDetail = new WorkoutTemplateMapping()
                        {
                            TemplateID = templateId,
                            DisplayOrder = 1,
                            CreatedDate = DateTime.UtcNow,
                            UpdatedDate = DateTime.UtcNow,
                            IsActive = true,
                            WorkoutMasterID = workoutId
                        };
                        templateDetail.IsSelected = true;
                        _dbcontext.WorkoutTemplateMappings.Add(newWorkoutTemplateMapDetail);
                        _dbcontext.SaveChanges();

                        templateDetail.WorkoutTemplateMapId = newWorkoutTemplateMapDetail.ID;
                    }
                }

                return templateDetail;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Get Template detail for Workout
        /// </summary>
        /// <param name="workoutId">Workout Id</param>
        /// <param name="templateId">Template Id</param>
        /// <returns></returns>
        public BoxerTemplateDetail GetBoxerTemplateDetail(int workoutId, int templateId, bool isRead, int? templateMapId)
        {
            try
            {
                if (templateId == 0)
                    return null;
                var workoutDetail = new BoxerWorkoutDetailModel();
                var workoutInfo = _dbcontext.BoxerWorkoutMasters.Include("BoxerWorkoutTemplateMappings").FirstOrDefault(e => e.ID == workoutId);
                var templateResult = _dbcontext.BoxerTemplates.Include("BoxerTemplateVideoMappings").Include("BoxerTemplateVideoMappings.Video").ToList();
                var templateDetail = templateResult.Where(e => e.ID == templateId).Select(e => new BoxerTemplateDetail
                {
                    BoxerTemplateId = e.ID,
                    BoxerTemplateName = e.TemplateName,
                    NumberOfBasicVideos = e.NumberOfBasicVideos,
                    NumberOfAlternateVideos = e.NumberOfAlterVideos,
                    IsSelected = false,
                    PrimaryText = e.PrimaryText,
                    AlternateText = e.AlternateText,
                    PrimaryColor = e.PrimaryColor,
                    AlternateColor = e.AlternateColor,
                    PrimaryGradientColor = e.PrimaryGradientColor,
                    AlternateGradientColor = e.AlternateGradientColor,
                    FooterText = e.FooterText,
                    GoalText = e.GoalText == null ? " " : e.GoalText,
                    TimeText = e.TimeText == null ? " " : e.TimeText,
                    BlockLineOneText = e.BlockLineOneText == null ? " " : e.BlockLineOneText,
                    BlockLineTwoText = e.BlockLineTwoText == null ? " " : e.BlockLineTwoText,
                    FooterTextColor = e.FooterTextColor,
                    BasicVideos = e.BoxerTemplateVideoMappings.Where(tv => tv.BoxerTemplateID == e.ID && tv.IsBasicVideo && !tv.IsDeleted).Select(tv => new TemplateVideoDetail
                    {
                        IsAlterVideo = tv.IsAlterVideo,
                        IsBasicVideo = tv.IsBasicVideo,
                        VideoFile = tv.Video.VideoAttachment,
                        VideoId = tv.Video.ID,
                        VideoPosition = Convert.ToInt32(tv.VideoPosition),
                        VideoTitle = tv.Video.VideoTitle,
                        SmallDescription = tv.Video.SmallDescription,
                        Note = tv.Note == null || tv.Note == "0" ? " " : tv.Note,
                        Reps = tv.Reps == null || tv.Reps == "0" ? " " : tv.Reps
                    }).OrderBy(tv => tv.VideoPosition).ToList(),
                    AlternateVideos = e.BoxerTemplateVideoMappings.Where(tv => tv.BoxerTemplateID == e.ID && tv.IsAlterVideo && !tv.IsDeleted).Select(tv => new TemplateVideoDetail
                    {
                        IsAlterVideo = tv.IsAlterVideo,
                        IsBasicVideo = tv.IsBasicVideo,
                        VideoFile = tv.Video.VideoAttachment,
                        VideoId = tv.Video.ID,
                        VideoPosition = Convert.ToInt32(tv.VideoPosition),
                        VideoTitle = tv.Video.VideoTitle,
                        SmallDescription = tv.Video.SmallDescription,
                        Note = tv.Note == null || tv.Note == "0" ? " " : tv.Note,
                        Reps = tv.Reps == null || tv.Reps == "0" ? " " : tv.Reps
                    }).OrderBy(tv => tv.VideoPosition).ToList(),
                }).FirstOrDefault();
                if (workoutInfo != null)
                {
                    workoutDetail.FrontCoverImageId = workoutInfo.FrontCoverImageId;
                    workoutDetail.FrontCoverImageName = workoutInfo.ImageGallery1 != null ? workoutInfo.ImageGallery1.ImageFile : string.Empty;

                    workoutDetail.BackCoverImageId = workoutInfo.BackCoverImageId;
                    workoutDetail.BackCoverImageName = workoutInfo.ImageGallery != null ? workoutInfo.ImageGallery.ImageFile : string.Empty;

                    if (workoutInfo.BoxerWorkoutTemplateMappings != null && workoutInfo.BoxerWorkoutTemplateMappings.Count > 0 && isRead)
                    {
                        //allTemplates.ForEach(x => x.IsSelected = && x.WorkoutTemplateMapId == workoutInfo.WorkoutTemplateMappings.FirstOrDefault(y => y.TemplateID == x.TemplateId).ID);
                        //template.IsSelected = workoutInfo.WorkoutTemplateMappings.Any(y => y.TemplateID == template.TemplateId && y.IsActive == true);
                        var workoutTemplateMapList = workoutInfo.BoxerWorkoutTemplateMappings.Where(y => y.BoxerTemplateID == templateDetail.BoxerTemplateId && y.IsDeleted != true).ToList();
                        if (templateMapId != null && templateMapId > 0)
                            workoutTemplateMapList = workoutInfo.BoxerWorkoutTemplateMappings.Where(y => y.BoxerTemplateID == templateDetail.BoxerTemplateId && y.IsDeleted != true && y.ID == templateMapId).ToList();
                        foreach (var workoutTemplateMap in workoutTemplateMapList)
                        {
                            if (workoutTemplateMap != null)
                            {
                                templateDetail.BoxerWorkoutTemplateMapId = workoutTemplateMap.ID;
                                templateDetail.IsDeleted = false;
                                templateDetail.IsSelected = Convert.ToBoolean(workoutTemplateMap.IsActive);
                                var workouttemplatemap = _dbcontext.BoxerWorkoutTemplateMappings.FirstOrDefault(e => e.ID == workoutTemplateMap.ID);
                                workouttemplatemap.IsDeleted = false;
                                _dbcontext.SaveChanges();
                            }
                            else
                            {
                                var newWorkoutTemplateMapDetail = new BoxerWorkoutTemplateMapping()
                                {
                                    BoxerTemplateID = templateId,
                                    CreatedDate = DateTime.UtcNow,
                                    UpdatedDate = DateTime.UtcNow,
                                    IsActive = true,
                                    IsDeleted = false,
                                    BoxerWorkoutMasterID = workoutId
                                };
                                templateDetail.IsSelected = true;
                                _dbcontext.BoxerWorkoutTemplateMappings.Add(newWorkoutTemplateMapDetail);
                                _dbcontext.SaveChanges();

                                templateDetail.BoxerWorkoutTemplateMapId = newWorkoutTemplateMapDetail.ID;

                            }
                        }
                    }
                    else
                    {
                        var newWorkoutTemplateMapDetail = new BoxerWorkoutTemplateMapping()
                        {
                            BoxerTemplateID = templateId,
                            DisplayOrder = 1,
                            CreatedDate = DateTime.UtcNow,
                            UpdatedDate = DateTime.UtcNow,
                            IsActive = true,
                            BoxerWorkoutMasterID = workoutId
                        };
                        templateDetail.IsSelected = true;
                        _dbcontext.BoxerWorkoutTemplateMappings.Add(newWorkoutTemplateMapDetail);
                        _dbcontext.SaveChanges();

                        templateDetail.BoxerWorkoutTemplateMapId = newWorkoutTemplateMapDetail.ID;
                    }
                }

                return templateDetail;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Save Template for Workout
        /// </summary>
        /// <param name="request">Workout Template Model</param>
        /// <returns></returns>
        public int MapWorkoutTemplate(WorkoutTemplateMapRequest request)
        {
            var workoutTemplateMapDetail = _dbcontext.WorkoutTemplateMappings.FirstOrDefault(e => e.WorkoutMasterID == request.WorkoutId && e.TemplateID == request.TemplateId && e.ID == request.WorkoutTemplateMapId);

            if (workoutTemplateMapDetail != null)
            {
                workoutTemplateMapDetail.TemplateID = request.TemplateId;
                workoutTemplateMapDetail.WorkoutMasterID = request.WorkoutId;
                workoutTemplateMapDetail.UpdatedDate = DateTime.UtcNow;
                workoutTemplateMapDetail.DisplayOrder = request.DisplayOrder;
                workoutTemplateMapDetail.IsActive = request.IsActive;
                _dbcontext.SaveChanges();
                return workoutTemplateMapDetail.ID;
            }
            else
            {
                var newWorkoutTemplateMap = new WorkoutTemplateMapping()
                {
                    WorkoutMasterID = request.WorkoutId,
                    TemplateID = request.TemplateId,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    DisplayOrder = request.DisplayOrder
                };
                _dbcontext.WorkoutTemplateMappings.Add(newWorkoutTemplateMap);
                _dbcontext.SaveChanges();
                return newWorkoutTemplateMap.ID;
            }
        }

        /// <summary>
        /// Save Template for Workout
        /// </summary>
        /// <param name="request">Workout Template Model</param>
        /// <returns></returns>
        public int MapBoxerWorkoutTemplate(BoxerWorkoutTemplateMapRequest request)
        {
            var workoutTemplateMapDetail = _dbcontext.BoxerWorkoutTemplateMappings.FirstOrDefault(e => e.BoxerWorkoutMasterID == request.BoxerWorkoutId && e.BoxerTemplateID == request.BoxerTemplateId && e.ID == request.BoxerWorkoutTemplateMapId);

            if (workoutTemplateMapDetail != null)
            {
                workoutTemplateMapDetail.BoxerTemplateID = request.BoxerTemplateId;
                workoutTemplateMapDetail.BoxerWorkoutMasterID = request.BoxerWorkoutId;
                workoutTemplateMapDetail.UpdatedDate = DateTime.UtcNow;
                workoutTemplateMapDetail.DisplayOrder = request.DisplayOrder;
                workoutTemplateMapDetail.IsActive = request.IsActive;
                _dbcontext.SaveChanges();
                return workoutTemplateMapDetail.ID;
            }
            else
            {
                var newWorkoutTemplateMap = new BoxerWorkoutTemplateMapping()
                {
                    BoxerWorkoutMasterID = request.BoxerWorkoutId,
                    BoxerTemplateID = request.BoxerTemplateId,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    DisplayOrder = request.DisplayOrder
                };
                _dbcontext.BoxerWorkoutTemplateMappings.Add(newWorkoutTemplateMap);
                _dbcontext.SaveChanges();
                return newWorkoutTemplateMap.ID;
            }
        }


        /// <summary>
        /// Make soft delete for Workout Template
        /// </summary>
        /// <param name="request">Workout Template Request Model</param>
        /// <returns>True or False</returns>
        public bool DeleteWorkoutTemplate(WorkoutTemplateMapRequest request)
        {
            var workoutTemplateMapDetail = _dbcontext.WorkoutTemplateMappings.FirstOrDefault(e => e.ID == request.WorkoutTemplateMapId);

            if (workoutTemplateMapDetail != null)
            {
                workoutTemplateMapDetail.IsDeleted = true;
                _dbcontext.SaveChanges();
                return true;
            }
            return false;
        }



        /// <summary>
        /// Make soft delete for Boxer Workout Template
        /// </summary>
        /// <param name="request">Workout Template Request Model</param>
        /// <returns>True or False</returns>
        public bool DeleteBoxerWorkoutTemplate(BoxerWorkoutTemplateMapRequest request)
        {
            var boxerWorkoutTemplateMapDetail = _dbcontext.BoxerWorkoutTemplateMappings.FirstOrDefault(e => e.ID == request.BoxerWorkoutTemplateMapId);

            if (boxerWorkoutTemplateMapDetail != null)
            {
                boxerWorkoutTemplateMapDetail.IsDeleted = true;
                _dbcontext.SaveChanges();
                return true;
            }
            return false;
        }


        /// <summary>
        /// Set Workout Template Display order
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool SetWorkoutTemplateDisplayOrder(List<WorkoutTemplateDisplayOrder> request)
        {
            foreach (var templatemap in request)
            {
                var workoutTemplateMapDetail = _dbcontext.WorkoutTemplateMappings.FirstOrDefault(e => e.ID == templatemap.WorkoutTemplateMapId);
                if (workoutTemplateMapDetail != null)
                {
                    workoutTemplateMapDetail.UpdatedDate = DateTime.UtcNow;
                    workoutTemplateMapDetail.DisplayOrder = templatemap.DisplayOrder;
                    _dbcontext.SaveChanges();
                }
            }
            return true;
        }

        /// <summary>
        /// Set Boxer Workout Template Display order
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool SetBoxerWorkoutTemplateDisplayOrder(List<BoxerWorkoutTemplateDisplayOrder> request)
        {
            foreach (var templatemap in request)
            {
                var workoutTemplateMapDetail = _dbcontext.BoxerWorkoutTemplateMappings.FirstOrDefault(e => e.ID == templatemap.BoxerWorkoutTemplateMapId);
                if (workoutTemplateMapDetail != null)
                {
                    workoutTemplateMapDetail.UpdatedDate = DateTime.UtcNow;
                    workoutTemplateMapDetail.DisplayOrder = templatemap.DisplayOrder;
                    _dbcontext.SaveChanges();
                }
            }
            return true;
        }
        #endregion Workout Template

        public bool SaveWorkoutUserMapping(ModelWorkoutUserMapping request)
        {
            var isSaved = false;
            try
            {
                using (var dbContext = new SolciotyNewEntities())
                {
                    var workoutUserMapping = dbContext.WorkoutUserMappings.FirstOrDefault(e => e.UserId == request.UserId && e.WorkoutId == request.WorkoutId && e.LocationId == request.LocationId);
                    if (workoutUserMapping == null)
                    {
                        var newWorkoutUserMapping = new WorkoutUserMapping()
                        {
                            UserId = request.UserId,
                            LocationId = request.LocationId,
                            WorkoutId = request.WorkoutId,
                            IsActive = request.IsChecked,
                            CreatedBy = request.CreatedBy,
                            CreatedDate = DateTime.UtcNow,
                            UpdatedBy = request.UpdatedBy,
                            UpdatedDate = DateTime.UtcNow
                        };
                        dbContext.WorkoutUserMappings.Add(newWorkoutUserMapping);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        workoutUserMapping.IsActive = request.IsChecked;
                        workoutUserMapping.UpdatedBy = request.UpdatedBy;
                        workoutUserMapping.UpdatedDate = DateTime.UtcNow;

                        dbContext.SaveChanges();
                    }
                    isSaved = true;
                }
            }
            catch (Exception ex)
            {
                isSaved = false;
            }
            return isSaved;
        }

        public bool SaveBoxerWorkoutUserMapping(ModelBoxerWorkoutUserMapping request)
        {
            var isSaved = false;
            try
            {
                using (var dbContext = new SolciotyNewEntities())
                {
                    var boxerWorkoutUserMapping = dbContext.BoxerWorkoutUserMappings.FirstOrDefault(e => e.UserId == request.UserId && e.BoxerWorkoutId == request.BoxerWorkoutId && e.LocationId == request.LocationId);
                    if (boxerWorkoutUserMapping == null)
                    {
                        var newBoxerWorkoutUserMapping = new BoxerWorkoutUserMapping()
                        {
                            UserId = request.UserId,
                            LocationId = request.LocationId,
                            BoxerWorkoutId = request.BoxerWorkoutId,
                            IsActive = request.IsChecked,
                            CreatedBy = request.CreatedBy,
                            CreatedDate = DateTime.UtcNow,
                            UpdatedBy = request.UpdatedBy,
                            UpdatedDate = DateTime.UtcNow
                        };
                        dbContext.BoxerWorkoutUserMappings.Add(newBoxerWorkoutUserMapping);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        boxerWorkoutUserMapping.IsActive = request.IsChecked;
                        boxerWorkoutUserMapping.UpdatedBy = request.UpdatedBy;
                        boxerWorkoutUserMapping.UpdatedDate = DateTime.UtcNow;

                        dbContext.SaveChanges();
                    }
                    isSaved = true;
                }
            }
            catch (Exception ex)
            {
                isSaved = false;
            }
            return isSaved;
        }
        #endregion
    }

    public class BALWorkoutTemplateMapping : IDisposable
    {
        SolciotyNewEntities _dbcontext;
        public BALWorkoutTemplateMapping()
        {
            _dbcontext = new SolciotyNewEntities();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #region Methods

        /// <summary>
        /// Make soft delete of Template from Workout
        /// </summary>
        /// <param name="ID">Workout Template Mappint Id</param>
        /// <returns></returns>
        public CommonResultsMessage Delete(Int32 ID)
        {
            try
            {
                var mapng = _dbcontext.WorkoutTemplateMappings.FirstOrDefault(p => p.ID == ID);
                mapng.IsActive = false;
                mapng.IsDeleted = true;
                _dbcontext.Entry(mapng).State = System.Data.Entity.EntityState.Modified;
                _dbcontext.SaveChanges();
                return CommonResultsMessage.Update;
            }
            catch (Exception)
            {
                return CommonResultsMessage.Fail;
            }
        }

        #endregion
    }
}
