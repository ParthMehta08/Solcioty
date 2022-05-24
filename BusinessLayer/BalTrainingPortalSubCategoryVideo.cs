using DataAccessLayer;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
namespace BusinessLayer
{
	public class BalTrainingPortalSubCategoryVideo : IDisposable
	{
        SolciotyNewEntities _dbcontext;
		public BalTrainingPortalSubCategoryVideo()
		{
			_dbcontext = new SolciotyNewEntities();
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}


		#region  Methods

		public ModelTrainingPortalSubCategoryVideo DalToModel(TrainingPortalSubCategoryVideo DalModel)
		{
			var objModel = new ModelTrainingPortalSubCategoryVideo();
			try
			{
				objModel.Id = DalModel.Id;
				objModel.Name = DalModel.Name;
				objModel.PortalSubCategoryId = DalModel.TrainingPortalSubCategoryId;
				objModel.IsActiveString = DalModel.IsActive ? "on" : "";
				objModel.Description = DalModel.Description;
				objModel.VideoPath = DalModel.VideoPath;
			}
			catch (Exception)
			{
				objModel = null;
			}
			return objModel;

		}

		public ModelTrainingPortalSubCategoryVideo GetTrainingPortalSubCategoryVideoById(long Id)
		{
			try
			{
				var objdal = _dbcontext.TrainingPortalSubCategoryVideos.FirstOrDefault(p => p.Id == Id);
				return DalToModel(objdal);
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public IList<ModelTrainingPortalSubCategoryVideo> GetAllTrainingPortalSubCategorieVideos(long subCategoryId, int userId, bool ForAdmin = false, bool withDeleted = false)
		{
			var trainingPortalSubCategorieVideos = new List<ModelTrainingPortalSubCategoryVideo>();
			var Query = new List<TrainingPortalSubCategoryVideo>();
			try
			{
				if (withDeleted)
					Query = _dbcontext.TrainingPortalSubCategoryVideos.Where(e => e.TrainingPortalSubCategoryId == subCategoryId).ToList();
				else if (ForAdmin)
					Query = _dbcontext.TrainingPortalSubCategoryVideos.Where(e => e.IsDeleted != true && e.TrainingPortalSubCategoryId == subCategoryId).ToList();
				else
					Query = _dbcontext.TrainingPortalSubCategoryVideos.Where(p => p.IsDeleted != true && p.TrainingPortalSubCategoryId == subCategoryId).ToList();

				foreach (var item in Query)
					trainingPortalSubCategorieVideos.Add(DalToModel(item));
			}
			catch (Exception)
			{
				return null;
			}
			return trainingPortalSubCategorieVideos;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="objModel"></param>
		/// <returns></returns>
		public CommonResultsMessage AddEditTrainingPortalSubCategoryVideo(ModelTrainingPortalSubCategoryVideo objModel)
		{
			try
			{
				//check Video name exists or not bcoz every video name must unique
				if (CheckPortalSubCategoryVideoNameDuplication(objModel.Id, objModel.Name))
				{
					return CommonResultsMessage.duplicate;
				}
				if (objModel.Id > 0)
				{
					//edit
					var oldData = _dbcontext.TrainingPortalSubCategoryVideos.FirstOrDefault(p => p.Id == objModel.Id);
					oldData.VideoPath = objModel.VideoPath;
					oldData.Description = objModel.Description;
					oldData.Name = objModel.Name;
					oldData.IsActive = objModel.IsActive;
					_dbcontext.Entry(oldData).State = System.Data.Entity.EntityState.Modified;
					_dbcontext.SaveChanges();
					return CommonResultsMessage.Update;
				}
				else
				{
					//add
					var objDal = new TrainingPortalSubCategoryVideo();
					objDal.Name = objModel.Name;
					objDal.Description = objModel.Description;
					objDal.VideoPath = objModel.VideoPath;
					objDal.IsDeleted = objModel.IsDeleted;
					objDal.TrainingPortalSubCategoryId = objModel.PortalSubCategoryId;
					objDal.IsActive = objModel.IsActive;
					objDal.IsDeleted = false;
					_dbcontext.TrainingPortalSubCategoryVideos.Add(objDal);
					_dbcontext.SaveChanges();

					return CommonResultsMessage.Add;
				}
			}
			catch (Exception ex)
			{
				return CommonResultsMessage.Fail;
			}
		}
		private bool CheckPortalSubCategoryVideoNameDuplication(long Id, string PortalSubCategoryNameNew)
		{
			try
			{
				var duplicate = _dbcontext.TrainingPortalSubCategoryVideos.Where(p => p.Id != Id && p.Name.ToLower().Trim() == PortalSubCategoryNameNew.ToLower().Trim());
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
				var olddata = _dbcontext.TrainingPortalSubCategoryVideos.FirstOrDefault(p => p.Id == Id);
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