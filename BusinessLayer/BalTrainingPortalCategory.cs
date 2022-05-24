using DataAccessLayer;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
	public class BalTrainingPortalCategory : IDisposable
	{
        SolciotyNewEntities _dbcontext;
		public BalTrainingPortalCategory()
		{
			_dbcontext = new SolciotyNewEntities();
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}


		#region  Methods

		public ModelTrainingPortalCategory DalToModel(TrainingPortalCategory DalModel)
		{
			var objModel = new ModelTrainingPortalCategory();
			try
			{
				objModel.Id = DalModel.Id;
				objModel.Name = DalModel.Name;
				objModel.TraningPortalId = DalModel.TrainingPortalId;
				objModel.DisplayOrder = DalModel.DisplayOrder;
				objModel.IconString = DalModel.IconString;
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

		public ModelTrainingPortalCategory GetTrainingPortalCategoryById(long Id)
		{
			try
			{
				var objdal = _dbcontext.TrainingPortalCategories.FirstOrDefault(p => p.Id == Id);
				return DalToModel(objdal);
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public IList<ModelTrainingPortalCategory> GetAllTrainingPortalCategories(long portalId,int userId, bool ForAdmin = false, bool withDeleted = false)
		{
			var trainingPortalCategories = new List<ModelTrainingPortalCategory>();
			var Query = new List<TrainingPortalCategory>();
			try
			{
				if (withDeleted)
					Query = _dbcontext.TrainingPortalCategories.Where(e => e.CreatedBy == userId && e.TrainingPortalId==portalId).ToList();
				else if (ForAdmin)
					Query = _dbcontext.TrainingPortalCategories.Where(e => e.IsDeleted != true && e.TrainingPortalId == portalId).ToList();
				else
					Query = _dbcontext.TrainingPortalCategories.Where(p => p.IsDeleted != true && p.TrainingPortalId == portalId && p.CreatedBy == userId).ToList();

				foreach (var item in Query)
					trainingPortalCategories.Add(DalToModel(item));
			}
			catch (Exception)
			{
				return null;
			}
			return trainingPortalCategories;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="objModel"></param>
		/// <returns></returns>
		public CommonResultsMessage AddEditTrainingPortalCategory(ModelTrainingPortalCategory objModel)
		{
			try
			{
				//check Video name exists or not bcoz every video name must unique
				if (CheckPortalCategoryNameDuplication(objModel.Id, objModel.Name, Convert.ToInt32(objModel.CreatedBy)))
				{
					return CommonResultsMessage.duplicate;
				}
				if (objModel.Id > 0)
				{
					//edit
					var oldData = _dbcontext.TrainingPortalCategories.FirstOrDefault(p => p.Id == objModel.Id);
					oldData.Name = objModel.Name;
					oldData.DisplayOrder = objModel.DisplayOrder;
					oldData.IconString = objModel.IconString;
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
					var objDal = new TrainingPortalCategory();
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
					objDal.IconString = objModel.IconString;
					objDal.TrainingPortalId = objModel.TraningPortalId;
					objDal.IsDeleted = false;
					_dbcontext.TrainingPortalCategories.Add(objDal);
					_dbcontext.SaveChanges();

					return CommonResultsMessage.Add;
				}
			}
			catch (Exception ex)
			{
				return CommonResultsMessage.Fail;
			}
		}
		private bool CheckPortalCategoryNameDuplication(long Id, string PortalNameNew, int ClientOwnerId)
		{
			try
			{
				var duplicate = _dbcontext.TrainingPortalCategories.Where(p => p.Id != Id && p.Name.ToLower().Trim() == PortalNameNew.ToLower().Trim() && p.CreatedBy == ClientOwnerId);
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
				var olddata = _dbcontext.TrainingPortalCategories.FirstOrDefault(p => p.Id == Id);
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
