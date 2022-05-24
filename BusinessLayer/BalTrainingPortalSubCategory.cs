using DataAccessLayer;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer
{
	public class BalTrainingPortalSubCategory : IDisposable
	{
        SolciotyNewEntities _dbcontext;
		public BalTrainingPortalSubCategory()
		{
			_dbcontext = new SolciotyNewEntities();
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}


		#region  Methods

		public ModelTrainingPortalSubCategory DalToModel(TrainingPortalSubCategory DalModel)
		{
			var objModel = new ModelTrainingPortalSubCategory();
			try
			{
				objModel.Id = DalModel.Id;
				objModel.Name = DalModel.Name;
				objModel.TraningPortalId = DalModel.TrainingPortalId;
				objModel.TraningPortalCategoryId = DalModel.TrainingPortalCategoryId;
				objModel.DisplayOrder = DalModel.DisplayOrder;
				objModel.IsActiveString = DalModel.IsActive ? "on" : "";
				objModel.CreatedBy = DalModel.CreatedBy;
				objModel.CreatedDate = DalModel.CreatedDate;
				objModel.UpdatedBy = DalModel.UpdatedBy;
				objModel.UpdatedDate = DalModel.UpdatedDate;
			}
			catch (Exception)
			{
				objModel = null;
			}
			return objModel;

		}

		public ModelTrainingPortalSubCategory GetTrainingPortalSubCategoryById(long Id)
		{
			try
			{
				var objdal = _dbcontext.TrainingPortalSubCategories.FirstOrDefault(p => p.Id == Id);
				return DalToModel(objdal);
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public IList<ModelTrainingPortalSubCategory> GetAllTrainingPortalSubCategories(long categoryId, int userId, bool ForAdmin = false, bool withDeleted = false)
		{
			var trainingPortalSubCategories = new List<ModelTrainingPortalSubCategory>();
			var Query = new List<TrainingPortalSubCategory>();
			try
			{
				if (withDeleted)
					Query = _dbcontext.TrainingPortalSubCategories.Where(e => e.CreatedBy == userId && e.TrainingPortalCategoryId == categoryId).ToList();
				else if (ForAdmin)
					Query = _dbcontext.TrainingPortalSubCategories.Where(e => e.IsDeleted != true && e.TrainingPortalCategoryId == categoryId).ToList();
				else
					Query = _dbcontext.TrainingPortalSubCategories.Where(p => p.IsDeleted != true && p.TrainingPortalCategoryId == categoryId && p.CreatedBy == userId).ToList();

				foreach (var item in Query)
					trainingPortalSubCategories.Add(DalToModel(item));
			}
			catch (Exception)
			{
				return null;
			}
			return trainingPortalSubCategories;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="objModel"></param>
		/// <returns></returns>
		public CommonResultsMessage AddEditTrainingPortalSubCategory(ModelTrainingPortalSubCategory objModel)
		{
			try
			{
				//check Video name exists or not bcoz every video name must unique
				if (CheckPortalSubCategoryNameDuplication(objModel.Id, objModel.Name, Convert.ToInt32(objModel.CreatedBy)))
				{
					return CommonResultsMessage.duplicate;
				}
				if (objModel.Id > 0)
				{
					//edit
					var oldData = _dbcontext.TrainingPortalSubCategories.FirstOrDefault(p => p.Id == objModel.Id);
					oldData.DisplayOrder = objModel.DisplayOrder;
					oldData.IsActive = objModel.IsActive;
					oldData.UpdatedBy = objModel.UpdatedBy == 0 ? oldData.UpdatedBy : objModel.UpdatedBy;
					oldData.UpdatedDate = DateTime.UtcNow;
					_dbcontext.Entry(oldData).State = System.Data.Entity.EntityState.Modified;
					_dbcontext.SaveChanges();
					return CommonResultsMessage.Update;
				}
				else
				{
					//add
					var objDal = new TrainingPortalSubCategory();
					objDal.Name = objModel.Name;
					objDal.IsDeleted = objModel.IsDeleted;
					if (objModel.CreatedBy > 0)
					{
						objDal.CreatedBy = objModel.CreatedBy;
					}
					if (objModel.UpdatedBy > 0)
					{
						objDal.UpdatedBy = objModel.UpdatedBy;
					}
					objDal.CreatedDate = DateTime.UtcNow;
					objDal.UpdatedDate = DateTime.UtcNow;
					objDal.IsActive = objModel.IsActive;
					objDal.DisplayOrder = objModel.DisplayOrder;
					objDal.TrainingPortalId = objModel.TraningPortalId;
					objDal.TrainingPortalCategoryId = objModel.TraningPortalCategoryId;
					objDal.IsDeleted = false;
					_dbcontext.TrainingPortalSubCategories.Add(objDal);
					_dbcontext.SaveChanges();

					return CommonResultsMessage.Add;
				}
			}
			catch (Exception ex)
			{
				return CommonResultsMessage.Fail;
			}
		}
		private bool CheckPortalSubCategoryNameDuplication(long Id, string PortalSubCategoryNameNew, int ClientOwnerId)
		{
			try
			{
				var duplicate = _dbcontext.TrainingPortalSubCategories.Where(p => p.Id != Id && p.Name.ToLower().Trim() == PortalSubCategoryNameNew.ToLower().Trim() && p.CreatedBy == ClientOwnerId);
				if (duplicate.Any())
					return true;
			}
			catch (Exception ex)
			{
				return false;
			}
			return false;
		}

		public bool Delete(long Id)
		{
			var isDeleted = false;
			try
			{
				var olddata = _dbcontext.TrainingPortalSubCategories.FirstOrDefault(p => p.Id == Id);
				olddata.IsDeleted = true;
				_dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
				_dbcontext.SaveChanges();
				isDeleted = true;
			}
			catch (Exception)
			{
				isDeleted = false;
			}
			return isDeleted;
		}
		#endregion

	}
}