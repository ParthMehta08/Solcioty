using DataAccessLayer;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
	public class BALImage : IDisposable
	{
        SolciotyNewEntities _dbcontext;
		public BALImage()
		{
			_dbcontext = new SolciotyNewEntities();
		}
		public List<ModelImage> GetAllImages(int userId, bool isAdmin)
		{
			var images = new List<ModelImage>();
			if (isAdmin)
			{
				images = _dbcontext.ImageGalleries.Where(e => e.IsDeleted != true).Select(e => new ModelImage
				{
					Id = e.Id,
					CreatedBy = e.CreatedBy,
					ImageName=e.ImageFile,
					CreatedDate = e.CreatedDate,
					SmallDescription = e.Description,
					ImageTitle = e.ImageTitle,
					IsActiveString = e.IsActive ? "on" : "",
					UpdatedBy = e.UpdatedBy,
					UpdatedDate = e.UpdatedDate
				}).ToList();
			}
			else {
				images = _dbcontext.ImageGalleries.Where(e => e.IsDeleted != true  && e.CreatedBy==userId).Select(e => new ModelImage
				{
					Id = e.Id,
					CreatedBy = e.CreatedBy,
					ImageName = e.ImageFile,
					CreatedDate = e.CreatedDate,
					SmallDescription = e.Description,
					ImageTitle = e.ImageTitle,
					IsActiveString = e.IsActive ? "on" : "",
					UpdatedBy = e.UpdatedBy,
					UpdatedDate = e.UpdatedDate
				}).ToList();
			}
			return images;
		}


		public ModelImage GetImageByID(int imageId)
		{
			var imageDetail = new ModelImage();
			imageDetail= _dbcontext.ImageGalleries.Where(e => e.IsDeleted != true && e.Id==imageId).Select(e => new ModelImage
			{
				Id = e.Id,
				CreatedBy = e.CreatedBy,
				ImageName = e.ImageFile,
				CreatedDate = e.CreatedDate,
				SmallDescription = e.Description,
				ImageTitle = e.ImageTitle,
				IsActiveString = e.IsActive ? "on" : "",
				UpdatedBy = e.UpdatedBy,
				UpdatedDate = e.UpdatedDate
			}).FirstOrDefault();
			return imageDetail;
		}


		public CommonResultsMessage Delete(int imageId) {
			
			var imageDetail = _dbcontext.ImageGalleries.Where(e => e.IsActive == true && e.IsDeleted != true && e.Id == imageId).FirstOrDefault();
			if (imageDetail != null) {
				imageDetail.IsDeleted = true;
				_dbcontext.SaveChanges();
				return CommonResultsMessage.Update;
			}
			return CommonResultsMessage.Fail;
		}

		public CommonResultsMessage AddEditImage(ModelImage model) {
			dynamic returnResult;
			var imageDetail = _dbcontext.ImageGalleries.FirstOrDefault(e => e.Id == model.Id);
			if (imageDetail != null)
			{
				imageDetail.ImageTitle = model.ImageTitle;
				imageDetail.Description = model.SmallDescription;
				if(!string.IsNullOrEmpty(model.FullName))
					imageDetail.ImageFile = model.FullName;
				imageDetail.UpdatedBy = model.UpdatedBy;
				imageDetail.UpdatedDate = DateTime.UtcNow;
				imageDetail.IsActive = model.IsActive;
				_dbcontext.Entry(imageDetail).State = System.Data.Entity.EntityState.Modified;
				_dbcontext.SaveChanges();
				// Save Image 
				if (!string.IsNullOrEmpty(model.FullName))
				{
					model.ImageServerPath = model.ImageServerPath + model.Id;
					if (!System.IO.Directory.Exists(model.ImageServerPath))
					{

						System.IO.Directory.CreateDirectory(model.ImageServerPath);
					}
					System.IO.File.WriteAllBytes(model.ImageServerPath + "\\" + model.FullName, model.Image1);
				}
				returnResult = CommonResultsMessage.Update;
			}
			else {

				var newImageDetail = new ImageGallery()
				{
					ImageTitle = model.ImageTitle,
					Description = model.SmallDescription,
					IsActive = model.IsActive,
					CreatedBy = model.CreatedBy,
					CreatedDate = DateTime.UtcNow,
					IsDeleted = false,
					ImageFile = model.FullName
				};
				_dbcontext.ImageGalleries.Add(newImageDetail);
				_dbcontext.SaveChanges();
				model.Id = newImageDetail.Id;
				// Save Image 
				if (!string.IsNullOrEmpty(model.FullName))
				{
					model.ImageServerPath = model.ImageServerPath + model.Id;
					if (!System.IO.Directory.Exists(model.ImageServerPath))
					{

						System.IO.Directory.CreateDirectory(model.ImageServerPath);
					}
					System.IO.File.WriteAllBytes(model.ImageServerPath + "\\" + model.FullName, model.Image1);
				}
				if (model.ImageTagsMappingsNames!=null && model.ImageTagsMappingsNames.Length > 0) {
					var tags = model.ImageTagsMappingsNames.Split(',');
					foreach (var tag in tags)
					{
						var tagDetail = _dbcontext.FitnessTags.FirstOrDefault(e => e.TagName == tag);
						if (tagDetail == null) {
							var newTag = new FitnessTag()
							{
								TagName = tag,
								UpdatedBy = model.UpdatedBy,
								UpdatedDate = DateTime.UtcNow,
								IsActive = true,
								CreatedBy = model.CreatedBy,
								CreatedDate = DateTime.UtcNow,
								IsDeleted = false
							};
							_dbcontext.FitnessTags.Add(newTag);
							_dbcontext.SaveChanges();
							tagDetail = newTag;
						}

						var imageTagMapping = new ImageTagMapping()
						{
							ImageId = newImageDetail.Id,
							TagId = tagDetail.ID,
							IsActive = true,
							CreatedDate = DateTime.UtcNow,
							UpdatedDate = DateTime.UtcNow
						};
						_dbcontext.ImageTagMappings.Add(imageTagMapping);
						_dbcontext.SaveChanges();						
					}
				}

				returnResult = CommonResultsMessage.Add;
			}

			return returnResult;
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}
}
