using DataAccessLayer;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer
{
	public class BalCategory : IDisposable

	{
        SolciotyNewEntities _dbcontext;
		BalGym _balGym;
		public BalCategory()
		{
			_dbcontext = new SolciotyNewEntities();
			_balGym = new BalGym();
		}
		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		#region Methods
		public ModelCategory DalToModel(Category objDal)
		{
			if (objDal == null) return null;
			var objModel = new ModelCategory();
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
				var olddata = _dbcontext.Categories.FirstOrDefault(p => p.Id == ID);
				olddata.IsActive = false;
				olddata.IsDeleted = true;
				olddata.Name = olddata.Name + "_Deleted";
				_dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
				_dbcontext.SaveChanges();
				_dbcontext.Database.ExecuteSqlCommand("update [dbo].[Video] set CategoryId=null where CategoryId=" + ID);
				//_dbcontext.Database.ExecuteSqlCommand("update [dbo].[WorkoutMaster] set IsActive=0 where CategoryId=" + ID);
				return CommonResultsMessage.Update;
			}
			catch (Exception)
			{
				return CommonResultsMessage.Fail;
			}
		}
		public CommonResultsMessage AddEdit(ModelCategory objModel)
		{
			try
			{
				if (CheckCategoryNameDuplication(objModel.Id, objModel.Name, Convert.ToInt32(objModel.CreatedBy)))
				{
					return CommonResultsMessage.duplicate;
				}
				if (objModel.Id > 0)
				{//edit
					var olddata = _dbcontext.Categories.FirstOrDefault(p => p.Id == objModel.Id);
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
					Category objDal = new Category();
					objDal.IsActive = objModel.IsActive;
					objDal.Name = objModel.Name;
					objDal.CreatedDate = DateTime.UtcNow;
					objDal.UpdatedDate = DateTime.UtcNow;
					objDal.CreatedBy = objModel.CreatedBy;
					objDal.UpdatedBy = objModel.UpdatedBy;
					_dbcontext.Categories.Add(objDal);
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
				var duplicate = _dbcontext.Categories.Where(p => p.Id != ID && p.Name.ToLower().Trim() == CategoryNameNew.ToLower().Trim() && p.CreatedBy == ClientOwnerId);
				if (duplicate.Any())
					return true;
			}
			catch (Exception)
			{
				throw;
			}
			return false;
		}
		public ModelCategory GetByID(Int32 Id)
		{
			try
			{
				return DalToModel(_dbcontext.Categories.FirstOrDefault(p => p.Id == Id));
			}
			catch (Exception)
			{
				throw;
			}
		}

		public ModelCategory GetByName(string name)
		{
			try
			{
				return DalToModel(_dbcontext.Categories.FirstOrDefault(p => p.Name == name));
			}
			catch (Exception)
			{
				throw;
			}
		}

		public IList<ModelCategory> GetAllCategory(int ClientOwnerId, bool ForAdmin = false, bool withDeleted = false)
		{
			try
			{
				List<ModelCategory> resultList = new List<ModelCategory>();
				var Query = new List<Category>();
				if (withDeleted)
					Query = _dbcontext.Categories.ToList();
				else if (ForAdmin)
					Query = _dbcontext.Categories.Where(p => p.IsDeleted != true).ToList();
				else
					Query = _dbcontext.Categories.Where(p => (p.IsDeleted != true || p.IsDeleted == null) && p.CreatedBy == ClientOwnerId).ToList();

				foreach (var item in Query)
					resultList.Add(DalToModel(item));
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
				var Query = new List<Category>();
				if (withDeleted)
					Query = _dbcontext.Categories.ToList();
				else if (ForAdmin)
					Query = _dbcontext.Categories.Where(p => p.IsActive == true).ToList();
				else
					Query = _dbcontext.Categories.Where(p => (p.IsDeleted != true || p.IsDeleted == null) && p.CreatedBy == UserId).ToList();

				foreach (var item in Query)
					resultList.Add(key: item.Id, value: item.Name);
				return resultList;
			}
			catch (Exception)
			{
				throw;
			}

		}


		public IList<ModelCategory> GetAllCategoryByPrefix(string categoryPrefix)
		{
			try
			{
				List<ModelCategory> resultList = new List<ModelCategory>();
				var result = _dbcontext.Categories.Where(p => p.Name.StartsWith(categoryPrefix) && p.IsActive == true).ToList();
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
