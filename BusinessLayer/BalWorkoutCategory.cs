using DataAccessLayer;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer
{
	public class BalWorkoutCategory : IDisposable

	{
        SolciotyNewEntities _dbcontext;
		BalGym _balGym;
		public BalWorkoutCategory()
		{
			_dbcontext = new SolciotyNewEntities();
			_balGym = new BalGym();
		}
		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		#region Methods
		public ModelWorkoutCategory DalToModel(WorkoutCategory objDal)
		{
			if (objDal == null) return null;
			var objModel = new ModelWorkoutCategory();
			try
			{
				objModel.Id = objDal.Id;
				//objModel.IsActive = objDal.IsActive.HasValue ? objDal.IsActive.Value : false;
				objModel.IsActiveString = objDal.IsActive.HasValue ? (objDal.IsActive.Value == true ? "on" : "") : "";
				objModel.Name = objDal.Name;

				var clientDetail = _balGym.GetClientDetailByUserId(Convert.ToInt32(objDal.CreatedBy));
				if (clientDetail != null)
					objModel.ClientName = clientDetail.Name;
			}
			catch (Exception)
			{
				throw;
			}
			return objModel;
		}

		public ModelBoxerWorkoutCategory DalToModelBoxer(BoxerWorkoutCategory objDal)
		{
			if (objDal == null) return null;
			var objModel = new ModelBoxerWorkoutCategory();
			try
			{
				objModel.Id = objDal.Id;
				//objModel.IsActive = objDal.IsActive.HasValue ? objDal.IsActive.Value : false;
				objModel.IsActiveString = objDal.IsActive.HasValue ? (objDal.IsActive.Value == true ? "on" : "") : "";
				objModel.Name = objDal.Name;

				var clientDetail = _balGym.GetClientDetailByUserId(Convert.ToInt32(objDal.CreatedBy));
				if (clientDetail != null)
					objModel.ClientName = clientDetail.Name;
			}
			catch (Exception)
			{
				throw;
			}
			return objModel;
		}

		public CommonResultsMessage Delete(Int32 ID)
		{
			try
			{
				var olddata = _dbcontext.WorkoutCategories.FirstOrDefault(p => p.Id == ID);
				olddata.IsActive = false;
				olddata.IsDeleted = true;
				olddata.Name = olddata.Name + "_Deleted";
				_dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
				_dbcontext.SaveChanges();
				//_dbcontext.Database.ExecuteSqlCommand("update [dbo].[Video] set IsActive=0 where CategoryId=" + ID);
				_dbcontext.Database.ExecuteSqlCommand("update [dbo].[WorkoutMaster] set CategoryId==null where CategoryId=" + ID);
				return CommonResultsMessage.Update;
			}
			catch (Exception)
			{
				return CommonResultsMessage.Fail;
			}
		}
		public CommonResultsMessage AddEdit(ModelWorkoutCategory objModel)
		{
			try
			{
				if (CheckCategoryNameDuplication(objModel.Id, objModel.Name, Convert.ToInt32(objModel.CreatedBy)))
				{
					return CommonResultsMessage.duplicate;
				}
				if (objModel.Id > 0)
				{//edit
					var olddata = _dbcontext.WorkoutCategories.FirstOrDefault(p => p.Id == objModel.Id);
					olddata.IsActive = objModel.IsActive;
					olddata.Name = objModel.Name;
					olddata.UpdatedBy = objModel.UpdatedBy;
					olddata.UpdatedDate = DateTime.UtcNow;

					//var videoTagMappings = _dbcontext.VideoTagMappings.Where(e => e.TagID == objModel.ID).ToList();
					//if (videoTagMappings != null && videoTagMappings.Count > 0)
					//{
					//	videoTagMappings.ForEach(e => e.IsActive = objModel.IsActive);
					//}
					_dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
					_dbcontext.SaveChanges();
					return CommonResultsMessage.Update;
				}
				else
				{//add
					var objDal = new WorkoutCategory();
					objDal.IsActive = objModel.IsActive;
					objDal.Name = objModel.Name;
					objDal.CreatedDate = DateTime.UtcNow;
					objDal.UpdatedDate = DateTime.UtcNow;
					objDal.CreatedBy = objModel.CreatedBy;
					objDal.UpdatedBy = objModel.UpdatedBy;
					_dbcontext.WorkoutCategories.Add(objDal);
					_dbcontext.SaveChanges();
					return CommonResultsMessage.Add;
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public bool CheckCategoryNameDuplication(Int32 ID, String CategoryNameNew, int ClientOwnerId)
		{
			try
			{
				var duplicate = _dbcontext.WorkoutCategories.Where(p => p.Id != ID && p.Name.ToLower().Trim() == CategoryNameNew.ToLower().Trim() && p.CreatedBy == ClientOwnerId);
				if (duplicate.Any())
					return true;
			}
			catch (Exception)
			{
				throw;
			}
			return false;
		}
		public ModelWorkoutCategory GetByID(Int32 Id)
		{
			try
			{
				return DalToModel(_dbcontext.WorkoutCategories.FirstOrDefault(p => p.Id == Id));
			}
			catch (Exception)
			{
				throw;
			}
		}

		public ModelWorkoutCategory GetByName(string name)
		{
			try
			{
				return DalToModel(_dbcontext.WorkoutCategories.FirstOrDefault(p => p.Name == name));
			}
			catch (Exception)
			{
				throw;
			}
		}

		public IList<ModelWorkoutCategory> GetAllCategory(int ClientOwnerId, bool ForAdmin = false, bool withDeleted = false)
		{
			try
			{
				var resultList = new List<ModelWorkoutCategory>();
				var Query = new List<WorkoutCategory>();
				if (withDeleted)
					Query = _dbcontext.WorkoutCategories.ToList();
				else if (ForAdmin)
					Query = _dbcontext.WorkoutCategories.Where(p => p.IsDeleted != true).ToList();
				else
					Query = _dbcontext.WorkoutCategories.Where(p => (p.IsDeleted != true || p.IsDeleted == null) && p.CreatedBy == ClientOwnerId).ToList();

				foreach (var item in Query)
					resultList.Add(DalToModel(item));
				return resultList;
			}
			catch (Exception)
			{
				throw;
			}

		}

		public IList<ModelBoxerWorkoutCategory> GetAllBoxerCategory(int ClientOwnerId, bool ForAdmin = false, bool withDeleted = false)
		{
			try
			{
				var resultList = new List<ModelBoxerWorkoutCategory>();
				var Query = new List<BoxerWorkoutCategory>();
				if (withDeleted)
					Query = _dbcontext.BoxerWorkoutCategories.ToList();
				else if (ForAdmin)
					Query = _dbcontext.BoxerWorkoutCategories.Where(p => p.IsDeleted != true).ToList();
				else
					Query = _dbcontext.BoxerWorkoutCategories.Where(p => (p.IsDeleted != true || p.IsDeleted == null) && p.CreatedBy == ClientOwnerId).ToList();

				foreach (var item in Query)
					resultList.Add(DalToModelBoxer(item));
				return resultList;
			}
			catch (Exception)
			{
				throw;
			}

		}

		public Dictionary<Int32, String> GetAllCategoryDictionary(int UserId, bool ForAdmin = false, bool withDeleted = false)
		{
			try
			{
				Dictionary<Int32, String> resultList = new Dictionary<Int32, String>();
				var Query = new List<WorkoutCategory>();
				if (withDeleted)
					Query = _dbcontext.WorkoutCategories.ToList();
				else if (ForAdmin)
					Query = _dbcontext.WorkoutCategories.Where(p => p.IsActive == true).ToList();
				else
					Query = _dbcontext.WorkoutCategories.Where(p => (p.IsDeleted != true || p.IsDeleted == null) && p.CreatedBy == UserId).ToList();

				foreach (var item in Query)
					resultList.Add(key: item.Id, value: item.Name);
				return resultList;
			}
			catch (Exception)
			{
				throw;
			}

		}


		public IList<ModelWorkoutCategory> GetAllCategoryByPrefix(string categoryPrefix)
		{
			try
			{
				var resultList = new List<ModelWorkoutCategory>();
				var result = _dbcontext.WorkoutCategories.Where(p => p.Name.StartsWith(categoryPrefix) && p.IsActive == true).ToList();
				foreach (var item in result)
					resultList.Add(DalToModel(item));
				return resultList;
			}
			catch (Exception)
			{
				throw;
			}

		}
		#endregion
	}
}
