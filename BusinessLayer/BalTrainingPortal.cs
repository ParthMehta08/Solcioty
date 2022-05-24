using DataAccessLayer;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer
{
	public class BalTrainingPortal : IDisposable
	{
        SolciotyNewEntities _dbcontext;
		BalTrainingPortalSubCategory _balTrainingPortalSubCategory;
		public BalTrainingPortal() {
			_dbcontext = new SolciotyNewEntities();
			_balTrainingPortalSubCategory = new BalTrainingPortalSubCategory();
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}


		#region  Methods

		public ModelTrainingPortal DalToModel(TrainingPortal DalModel)
		{
			var objModel = new ModelTrainingPortal();
			try
			{
				objModel.Id = DalModel.Id;
				objModel.Name = DalModel.Name;
				objModel.IsActiveString = DalModel.IsActive?"on":"";
				objModel.Description = DalModel.Description;
				objModel.CreatedBy = DalModel.CreatedBy;
				objModel.CreatedDate = DalModel.CreatedDate;
				objModel.UpdatedBy = DalModel.UpdatedBy;
				objModel.UpdatedDate = DalModel.UpdatedDate;
				//objModel.TrainingPortalCategories = DalModel.TrainingPortalCategories.ToList().ForEach(x=> );
				if (DalModel.TrainingPortalCategories != null && DalModel.TrainingPortalCategories.Count > 0) {
					objModel.TrainingPortalCategories = DalModel.TrainingPortalCategories
												.Select(x=>new ModelTrainingPortalCategory() {
													Id = x.Id,
													Name = x.Name,
													TraningPortalId = x.TrainingPortalId,
													DisplayOrder = x.DisplayOrder,
													IconString = x.IconString,
													IsActiveString = x.IsActive?"on":""													
													}).OrderBy(x=>x.DisplayOrder).ToList();

					foreach (var category in objModel.TrainingPortalCategories)
					{
						category.TrainingPortalSubCategories = _balTrainingPortalSubCategory.GetAllTrainingPortalSubCategories(category.Id, objModel.CreatedBy, false, false).OrderBy(x=>x.DisplayOrder).ToList();
					}

				}
				
			}
			catch (Exception)
			{
				objModel = null;
			}
			return objModel;

		}

		public ModelTrainingPortal GetTrainingPortalById(long Id)
		{
			try
			{
				var objdal = _dbcontext.TrainingPortals.FirstOrDefault(p => p.Id == Id);
				return DalToModel(objdal);
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public ModelTrainingPortal GetTrainingPortalByUserId(int userId)
		{
			try
			{
				var objdal = _dbcontext.TrainingPortalUserMappings.Include("TrainingPortal").Where(p => p.UserId == userId).Select(e=>e.TrainingPortal).FirstOrDefault();
				return DalToModel(objdal);
			}
			catch (Exception ex)
			{
				return null;
			}
		}


		public IList<ModelTrainingPortal> GetAllTrainingPortals(int userId, bool ForAdmin = false, bool withDeleted = false)
		{
			IList<ModelTrainingPortal> trainingPortals = new List<ModelTrainingPortal>();
			var Query = new List<TrainingPortal>();
			try
			{
				if (withDeleted)
					Query = _dbcontext.TrainingPortals.Where(e => e.CreatedBy == userId).ToList();
				else if (ForAdmin)
					Query = _dbcontext.TrainingPortals.Where(e => e.IsDeleted != true).ToList();
				else
					Query = _dbcontext.TrainingPortals.Where(p => (p.IsDeleted != true || p.IsDeleted == null) && p.CreatedBy == userId).ToList();

				foreach (var item in Query)
					trainingPortals.Add(DalToModel(item));
			}
			catch (Exception)
			{
				return null;
			}
			return trainingPortals;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="objModel"></param>
		/// <returns></returns>
		public CommonResultsMessage AddEditTrainingPortal(ModelTrainingPortal objModel)
		{
			try
			{
				//check Video name exists or not bcoz every video name must unique
				if (CheckPortalNameDuplication(objModel.Id, objModel.Name, Convert.ToInt32(objModel.CreatedBy)))
				{
					return CommonResultsMessage.duplicate;
				}
				if (objModel.Id > 0)
				{
					//edit
					var oldData = _dbcontext.TrainingPortals.FirstOrDefault(p => p.Id == objModel.Id);
					oldData.Name = objModel.Name;
					oldData.Description = objModel.Description;
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
					var objDal = new TrainingPortal();
					objDal.Name = objModel.Name;
					objDal.Description = objModel.Description;
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
					_dbcontext.TrainingPortals.Add(objDal);
					_dbcontext.SaveChanges();					

					return CommonResultsMessage.Add;
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		private bool CheckPortalNameDuplication(long Id, string PortalNameNew, int ClientOwnerId)
		{
			try
			{
				var duplicate = _dbcontext.TrainingPortals.Where(p => p.Id != Id && p.Name.ToLower().Trim() == PortalNameNew.ToLower().Trim() && p.CreatedBy == ClientOwnerId);
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
				var olddata = _dbcontext.TrainingPortals.FirstOrDefault(p => p.Id == Id);
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

		public bool SaveTrainingPortalUserMapping(ModelTrainingPortalUserMapping request) {
			var isMapped = false;
			try
			{
				using (var dbContext = new SolciotyNewEntities()) {
					var mappingDetail = dbContext.TrainingPortalUserMappings.FirstOrDefault(x=>x.UserId == request.UserId && x.TrainingPortalId == request.PortalId);
					if (mappingDetail == null)
					{
						var newMapping = new TrainingPortalUserMapping()
						{
							UserId = request.UserId,
							TrainingPortalId = request.PortalId,
							PermissionType = request.PermissionType,
							IsActive = true,
							CreatedBy = request.CreatedBy,
							CreatedDate = DateTime.Now,
							IsDeleted = false,
							UpdatedBy = request.UpdatedBy,
							UpdatedDate = DateTime.Now
						};
						dbContext.TrainingPortalUserMappings.Add(newMapping);
					}
					else {
						mappingDetail.PermissionType = request.PermissionType;
						mappingDetail.UpdatedBy = request.UpdatedBy;
						mappingDetail.UpdatedDate = DateTime.Now;
					}
					dbContext.SaveChanges();
					isMapped = true;
				}
			}
			catch (Exception ex)
			{
				isMapped = false;
			}
			return isMapped;
		}
		#endregion

	}
}
