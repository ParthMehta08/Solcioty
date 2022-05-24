using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataAccessLayer;
using System.Web;
using System.Web.Mvc;

namespace BusinessLayer
{
    public class BALVideo : IDisposable
    {
        SolciotyNewEntities _dbcontext;
        BALFittnessTag _BALFittnessTag;
        BalGym _balGym;

        public BALVideo()
        {
            _dbcontext = new SolciotyNewEntities();
            _BALFittnessTag = new BALFittnessTag();
            _balGym = new BalGym();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #region Methods

        public ModelVideo DalToModel(Video DalModel)
        {
            ModelVideo objModel = new ModelVideo();
            try
            {
                objModel.ContentType = DalModel.ContentType;
                objModel.CounterOfUseInTemplate = DalModel.CounterOfUseInWorkout.HasValue ? DalModel.CounterOfUseInWorkout.Value : 0;
                objModel.CreatedBy = DalModel.CreatedBy.HasValue ? DalModel.CreatedBy.Value : 0;
                objModel.UpdatedBy = DalModel.UpdatedBy.HasValue ? DalModel.UpdatedBy.Value : 0;
                objModel.FullDescription = DalModel.FullDescription;
                objModel.FullName = DalModel.VideoAttachment;
                objModel.ID = DalModel.ID;
                //objModel.IsActive = DalModel.IsActive.HasValue ? DalModel.IsActive.Value : false;
                objModel.IsActiveString = DalModel.IsActive.HasValue ? (DalModel.IsActive.Value == true ? "on" : "") : "";
                objModel.SmallDescription = DalModel.SmallDescription;
                objModel.UpdatedDate = DalModel.UpdatedDate;
                objModel.VideoName = DalModel.VideoTitle;
                objModel.RepsCount = DalModel.RepsCount;
                //get tags
                var tagsList = _dbcontext.VideoTagMappings.Where(p => p.VideoID == DalModel.ID && p.IsActive == true || p.IsActive == null).ToList();
                if (tagsList.Any())
                    tagsList.ToList().ForEach(p =>
                    {
                        objModel.VideoTagsMappings = objModel.VideoTagsMappings + p.FitnessTag.ID + ", ";
                        objModel.VideoTagsMappingsNames = objModel.VideoTagsMappingsNames + p.FitnessTag.TagName + ", ";
                    });
                if (!string.IsNullOrEmpty(objModel.VideoTagsMappingsNames))
                {
                    objModel.VideoTagsMappingsNames = objModel.VideoTagsMappingsNames.Substring(0, objModel.VideoTagsMappingsNames.LastIndexOf(","));
                }

                var clientDetail = _balGym.GetClientDetailByUserId(Convert.ToInt32(objModel.CreatedBy));
                if (clientDetail != null)
                {
                    objModel.ClientName = clientDetail.Name;
                }

                objModel.CategoryId = DalModel.CategoryId != null ? DalModel.CategoryId.Value.ToString() : "";
                objModel.Categories = _dbcontext.Categories.Where(x => x.IsActive == true && x.CreatedBy == objModel.CreatedBy && x.IsDeleted != true).Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
                if (DalModel.Category != null)
                    objModel.CategoryName = DalModel.Category.Name;

                objModel.DisplayOrder = DalModel.DisplayOrder != null ? DalModel.DisplayOrder.Value : 0;
            }
            catch (Exception)
            {
                throw;
            }
            return objModel;

        }

        //public Video ModelToDal(ModelVideo objModel)
        //{
        //    Video objDal = new Video();
        //    try
        //    {
        //        objDal.ContentType = objModel.ContentType;
        //        objDal.CounterOfUseInWorkout = objModel.CounterOfUseInTemplate.HasValue ? objModel.CounterOfUseInTemplate.Value : 0;
        //        objDal.CreatedBy = objModel.CreatedBy.HasValue ? objModel.CreatedBy.Value : 0;
        //        objDal.UpdatedBy = objModel.UpdatedBy.HasValue ? objModel.UpdatedBy.Value : 0;
        //        objDal.FullDescription = objModel.FullDescription;
        //        objDal.VideoAttachment = objModel.FullName;
        //        objDal.ID = objModel.ID;
        //        objDal.IsActive = objModel.IsActive;
        //        objDal.SmallDescription = objModel.SmallDescription;
        //        objDal.UpdatedDate = objModel.UpdatedDate;
        //        objDal.VideoTitle = objModel.VideoName;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    return objDal;

        //}
        public ModelVideo GetVideoByID(Int32 ID)
        {
            try
            {
                Video objdal = _dbcontext.Videos.FirstOrDefault(p => p.ID == ID);
                return DalToModel(objdal);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<ModelVideo> GetAllVideos(int userId, int? categoryId, bool ForAdmin = false, bool withDeleted = false)
        {
            IList<ModelVideo> videos = new List<ModelVideo>();
            var Query = new List<Video>();
            try
            {
                if (withDeleted)
                    Query = _dbcontext.Videos.Where(e => e.CreatedBy == userId).ToList();
                else if (ForAdmin)
                    Query = _dbcontext.Videos.Where(e => e.IsDeleted != true).ToList();
                else
                    Query = _dbcontext.Videos.Where(p => (p.IsDeleted != true || p.IsDeleted == null) && p.CreatedBy == userId).ToList();

                if (categoryId != null && categoryId > 0)
                    Query = Query.Where(x => x.CategoryId == categoryId).ToList();


                foreach (var item in Query)
                    videos.Add(DalToModel(item));
            }
            catch (Exception ex)
            {
                throw;
            }
            return videos.OrderBy(x => x.DisplayOrder).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objModel"></param>
        /// <returns></returns>
        public CommonResultsMessage AddEditVideo(ModelVideo objModel)
        {
            try
            {
                //check Video name exists or not bcoz every video name must unique
                if (CheckideoNameDuplication(objModel.ID, objModel.VideoName, Convert.ToInt32(objModel.CreatedBy)))
                {
                    return CommonResultsMessage.duplicate;
                }
                if (objModel.ID > 0)
                {//edit
                    var oldData = _dbcontext.Videos.FirstOrDefault(p => p.ID == objModel.ID);
                    oldData.FullDescription = objModel.FullDescription;
                    oldData.UpdatedBy = objModel.UpdatedBy == 0 ? oldData.UpdatedBy : objModel.UpdatedBy;
                    oldData.UpdatedDate = DateTime.UtcNow;
                    oldData.VideoTitle = objModel.VideoName;
                    oldData.IsActive = objModel.IsActive;
                    oldData.RepsCount = objModel.RepsCount;
                    oldData.SmallDescription = objModel.SmallDescription;
                    if (!string.IsNullOrEmpty(objModel.CategoryId))
                    {
                        oldData.CategoryId = Convert.ToInt32(objModel.CategoryId);
                    }
                    if (objModel.FullName != null)
                    {
                        oldData.ContentType = objModel.ContentType;
                        oldData.VideoAttachment = objModel.FullName;
                    }
                    _dbcontext.Entry(oldData).State = System.Data.Entity.EntityState.Modified;
                    _dbcontext.SaveChanges();
                    // Save Video 
                    if (!objModel.IsVideoStoreInDB && objModel.FullName != null)
                    {
                        objModel.VideoServerPath = objModel.VideoServerPath + oldData.ID;
                        if (!System.IO.Directory.Exists(objModel.VideoServerPath))
                        {

                            System.IO.Directory.CreateDirectory(objModel.VideoServerPath);
                        }
                        System.IO.File.WriteAllBytes(objModel.VideoServerPath + "\\" + objModel.FullName, objModel.Video1);
                    }
                    //save tags mapping
                    if (objModel.VideoTagsMappingsNames != null)
                    {

                        var tags = objModel.VideoTagsMappingsNames.Split(',');
                        if (tags.Length > 0)
                        {
                            foreach (var tag in tags)
                            {
                                var tagDetail = _BALFittnessTag.GetByName(tag.Trim());
                                if (tagDetail == null)
                                {
                                    var taginfo = new FitnessTag()
                                    {
                                        TagName = tag,
                                        IsActive = true,
                                        CreatedDate = DateTime.UtcNow,
                                        UpdatedDate = DateTime.UtcNow,
                                        IsDeleted = false
                                    };
                                    _dbcontext.FitnessTags.Add(taginfo);
                                    _dbcontext.SaveChanges();
                                    tagDetail.ID = taginfo.ID;
                                }
                                objModel.VideoTagsMappings += tagDetail.ID + ",";

                            }
                        }
                        if (!string.IsNullOrEmpty(objModel.VideoTagsMappings))
                        {
                            objModel.VideoTagsMappings = objModel.VideoTagsMappings.Substring(0, objModel.VideoTagsMappings.LastIndexOf(','));
                        }
                        _BALFittnessTag.SaveTagMappingWithVideo(oldData.ID, objModel.VideoTagsMappings);

                    }
                    else
                    {
                        _BALFittnessTag.SaveTagMappingWithVideo(oldData.ID, objModel.VideoTagsMappings);
                    }
                    return CommonResultsMessage.Update;
                }
                else
                {//add
                    var lastDisplayOrder = _dbcontext.Videos.Max(x => x.DisplayOrder).Value;
                    Video objDal = new Video();
                    objDal.FullDescription = objModel.FullDescription;
                    objDal.VideoTitle = objModel.VideoName;
                    objDal.IsActive = objModel.IsActive;
                    objDal.RepsCount = objModel.RepsCount;
                    objDal.SmallDescription = objModel.SmallDescription;
                    if (objModel.CreatedBy > 0)
                    {
                        objDal.CreatedBy = objModel.CreatedBy;
                    }
                    if (objModel.UpdatedBy > 0)
                    {
                        objDal.UpdatedBy = objModel.UpdatedBy;
                    }
                    objDal.UpdatedDate = DateTime.UtcNow;
                    if (objModel.FullName != null)
                    {
                        objDal.ContentType = objModel.ContentType;
                        objDal.VideoAttachment = objModel.FullName;
                    }
                    if (!string.IsNullOrEmpty(objModel.CategoryId))
                    {
                        objDal.CategoryId = Convert.ToInt32(objModel.CategoryId);
                    }
                    objDal.CounterOfUseInWorkout = 0;
                    objDal.DisplayOrder = lastDisplayOrder + 1;
                    _dbcontext.Videos.Add(objDal);
                    _dbcontext.SaveChanges();
                    // Save Video
                    if (!objModel.IsVideoStoreInDB && objModel.FullName != null)
                    {
                        objModel.VideoServerPath = objModel.VideoServerPath + objDal.ID;
                        if (!System.IO.Directory.Exists(objModel.VideoServerPath))
                        {
                            System.IO.Directory.CreateDirectory(objModel.VideoServerPath);
                        }
                        System.IO.File.WriteAllBytes(objModel.VideoServerPath + "\\" + objModel.FullName, objModel.Video1);
                    }
                    //save tags mapping
                    if (objModel.VideoTagsMappingsNames != null)
                    {
                        var tags = objModel.VideoTagsMappingsNames.Split(',');
                        if (tags.Length > 0)
                        {
                            foreach (var tag in tags)
                            {
                                var tagDetail = _BALFittnessTag.GetByName(tag);
                                if (tagDetail == null)
                                {
                                    var taginfo = new FitnessTag()
                                    {
                                        TagName = tag,
                                        IsActive = true,
                                        CreatedDate = DateTime.UtcNow,
                                        UpdatedDate = DateTime.UtcNow,
                                        IsDeleted = false
                                    };
                                    _dbcontext.FitnessTags.Add(taginfo);
                                    _dbcontext.SaveChanges();
                                    tagDetail = new ModelFitnessTag()
                                    {
                                        ID = taginfo.ID
                                    };
                                }
                                objModel.VideoTagsMappings += tagDetail.ID + ",";

                            }
                        }

                        _BALFittnessTag.SaveTagMappingWithVideo(objDal.ID, objModel.VideoTagsMappings);

                    }

                    return CommonResultsMessage.Add;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private bool CheckideoNameDuplication(Int32 ID, String VideoNameNew, int ClientOwnerId)
        {
            try
            {
                var duplicate = _dbcontext.Videos.Where(p => p.ID != ID && p.VideoAttachment.ToLower().Trim() == VideoNameNew.ToLower().Trim() && p.CreatedBy == ClientOwnerId);
                if (duplicate.Any())
                    return true;
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }

        public CommonResultsMessage Delete(Int32 ID)
        {
            try
            {
                var olddata = _dbcontext.Videos.FirstOrDefault(p => p.ID == ID);
                olddata.IsActive = false;
                olddata.IsDeleted = true;
                olddata.VideoTitle = olddata.VideoTitle + "_Deleted";
                _dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
                _dbcontext.SaveChanges();
                return CommonResultsMessage.Update;
            }
            catch (Exception)
            {
                return CommonResultsMessage.Fail;
            }
        }


        public CommonResultsMessage BulkDelete(List<int> Ids)
        {
            try
            {
                var allVideos = _dbcontext.Videos.Where(x => Ids.Contains(x.ID)).ToList();
                foreach (var olddata in allVideos)
                {
                    //var olddata = _dbcontext.Videos.FirstOrDefault(p => p.ID == id);
                    olddata.IsActive = false;
                    olddata.IsDeleted = true;
                    olddata.VideoTitle = olddata.VideoTitle + "_Deleted";
                    //_dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;

                }
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
                var videoCategoryDetail = _dbcontext.Categories.FirstOrDefault(x => x.Id == CategoryId);
                if (videoCategoryDetail == null)
                {
                    return CommonResultsMessage.Fail;
                }
                var allVideos = _dbcontext.Videos.Where(x => IDs.Contains(x.ID)).ToList();
                foreach (var olddata in allVideos)
                {
                    //var olddata = _dbcontext.WorkoutMasters.FirstOrDefault(p => p.ID == ID);
                    olddata.CategoryId = videoCategoryDetail.Id;
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


        public CommonResultsMessage ReorderVideos(List<VideoDisplayOrderData> DisplayOrderData)
        {
            try
            {
                var ids = DisplayOrderData.Select(x => x.Id).ToList();
                var allVideos = _dbcontext.Videos.Where(x => ids.Contains(x.ID)).ToList();
                foreach (var olddata in allVideos)
                {
                    var displayOrder = DisplayOrderData.Where(x => x.Id == olddata.ID).Select(x => x.DisplayOrder).FirstOrDefault();
                    //var olddata = _dbcontext.Videos.FirstOrDefault(p => p.ID == id);
                    olddata.DisplayOrder = displayOrder;
                    //_dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;

                }
                _dbcontext.SaveChanges();
                return CommonResultsMessage.Update;
            }
            catch (Exception)
            {
                return CommonResultsMessage.Fail;
            }
        }

        public CommonResultsMessage ReorderAllVideos()
        {
            try
            {
                //var ids = DisplayOrderData.Select(x => x.Id).ToList();
                var allVideos = _dbcontext.Videos.Where(x => x.IsDeleted != true).OrderByDescending(x => x.ID).ToList();
                var displayOrder = 1;
                foreach (var olddata in allVideos)
                {
                    //var displayOrder = DisplayOrderData.Where(x => x.Id == olddata.ID).Select(x => x.DisplayOrder).FirstOrDefault();
                    //var olddata = _dbcontext.Videos.FirstOrDefault(p => p.ID == id);
                    olddata.DisplayOrder = displayOrder;
                    //_dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
                    displayOrder++;
                }
                _dbcontext.SaveChanges();
                return CommonResultsMessage.Update;
            }
            catch (Exception)
            {
                return CommonResultsMessage.Fail;
            }
        }

        public List<CopyVideo> CopyVideos(int sourceUserId, int destinationUserId)
        {
            return _dbcontext.SP_Proc_Copy_Videos(sourceUserId, destinationUserId).Select(x => new CopyVideo
            {
                NewId = x.NewId,
                OldId = x.OldId,
                ContentType = x.ContentType,
                DestinationId = x.DestinationId,
                Message = x.Message,
                SourceId = x.SourceId,
                VideoAttachment = x.VideoAttachment,
                VideoTitle = x.VideoTitle
            }).ToList();
        }

        #endregion
    }




    public class BALFittnessTag : IDisposable

    {
        SolciotyNewEntities _dbcontext;
        BalGym _balGym;
        public BALFittnessTag()
        {
            _dbcontext = new SolciotyNewEntities();
            _balGym = new BalGym();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #region Methods
        public ModelFitnessTag DalToModel(FitnessTag objDal)
        {
            if (objDal == null) return null;
            ModelFitnessTag objModel = new ModelFitnessTag();
            try
            {
                objModel.ID = objDal.ID;
                //objModel.IsActive = objDal.IsActive.HasValue ? objDal.IsActive.Value : false;
                objModel.IsActiveString = objDal.IsActive.HasValue ? (objDal.IsActive.Value == true ? "on" : "") : "";
                objModel.TagName = objDal.TagName;

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
        //public FitnessTag ModelToDal(ModelFitnessTag objmodel)
        //{
        //    FitnessTag objDal = new FitnessTag();
        //    try
        //    {
        //        objDal.ID = objmodel.ID;
        //        objDal.IsActive = objmodel.IsActive;
        //        objDal.TagName = objmodel.TagName;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    return objDal;
        //}
        public CommonResultsMessage Delete(Int32 ID)
        {
            try
            {
                var olddata = _dbcontext.FitnessTags.FirstOrDefault(p => p.ID == ID);
                olddata.IsActive = false;
                olddata.IsDeleted = true;
                olddata.TagName = olddata.TagName + "_Deleted";
                _dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
                _dbcontext.SaveChanges();
                int result = _dbcontext.Database.ExecuteSqlCommand("update [dbo].[VideoTagMapping] set IsActive=0 where TagID=" + ID);
                return CommonResultsMessage.Update;
            }
            catch (Exception)
            {
                return CommonResultsMessage.Fail;
            }
        }
        public CommonResultsMessage AddEdit(ModelFitnessTag objModel)
        {
            try
            {
                //check tag name exists or not bcoz every tag name must unique
                if (CheckWorkoutNameDuplication(objModel.ID, objModel.TagName, Convert.ToInt32(objModel.CreatedBy)))
                {
                    return CommonResultsMessage.duplicate;
                }
                if (objModel.ID > 0)
                {//edit
                    var olddata = _dbcontext.FitnessTags.FirstOrDefault(p => p.ID == objModel.ID);
                    olddata.IsActive = objModel.IsActive;
                    olddata.TagName = objModel.TagName;
                    olddata.UpdatedBy = objModel.UpdatedBy;
                    olddata.UpdatedDate = DateTime.UtcNow;

                    var videoTagMappings = _dbcontext.VideoTagMappings.Where(e => e.TagID == objModel.ID).ToList();
                    if (videoTagMappings != null && videoTagMappings.Count > 0)
                    {
                        videoTagMappings.ForEach(e => e.IsActive = objModel.IsActive);
                    }
                    _dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
                    _dbcontext.SaveChanges();
                    return CommonResultsMessage.Update;
                }
                else
                {//add
                    FitnessTag objDal = new FitnessTag();
                    objDal.IsActive = objModel.IsActive;
                    objDal.TagName = objModel.TagName;
                    objDal.CreatedDate = DateTime.UtcNow;
                    objDal.UpdatedDate = DateTime.UtcNow;
                    objDal.CreatedBy = objModel.CreatedBy;
                    objDal.UpdatedBy = objModel.UpdatedBy;
                    _dbcontext.FitnessTags.Add(objDal);
                    _dbcontext.SaveChanges();
                    return CommonResultsMessage.Add;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool CheckWorkoutNameDuplication(Int32 ID, String TagNameNew, int ClientOwnerId)
        {
            try
            {
                var duplicate = _dbcontext.FitnessTags.Where(p => p.ID != ID && p.TagName.ToLower().Trim() == TagNameNew.ToLower().Trim() && p.CreatedBy == ClientOwnerId);
                if (duplicate.Any())
                    return true;
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }
        public ModelFitnessTag GetByID(Int32 ID)
        {
            try
            {
                return DalToModel(_dbcontext.FitnessTags.FirstOrDefault(p => p.ID == ID));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ModelFitnessTag GetByName(string name)
        {
            try
            {
                return DalToModel(_dbcontext.FitnessTags.FirstOrDefault(p => p.TagName == name));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<ModelFitnessTag> GetAllFitnessTag(int ClientOwnerId, bool ForAdmin = false, bool withDeleted = false)
        {
            try
            {
                List<ModelFitnessTag> resultList = new List<ModelFitnessTag>();
                var Query = new List<FitnessTag>();
                if (withDeleted)
                    Query = _dbcontext.FitnessTags.ToList();
                else if (ForAdmin)
                    Query = _dbcontext.FitnessTags.Where(p => p.IsDeleted != true).ToList();
                else
                    Query = _dbcontext.FitnessTags.Where(p => (p.IsDeleted != true || p.IsDeleted == null) && p.CreatedBy == ClientOwnerId).ToList();

                foreach (var item in Query)
                    resultList.Add(DalToModel(item));
                return resultList;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public Dictionary<Int32, String> GetAllFitnessTagDictionary(int UserId, bool ForAdmin = false, bool withDeleted = false)
        {
            try
            {
                Dictionary<Int32, String> resultList = new Dictionary<Int32, String>();
                var Query = new List<FitnessTag>();
                if (withDeleted)
                    Query = _dbcontext.FitnessTags.ToList();
                else if (ForAdmin)
                    Query = _dbcontext.FitnessTags.Where(p => p.IsActive == true).ToList();
                else
                    Query = _dbcontext.FitnessTags.Where(p => (p.IsDeleted != true || p.IsDeleted == null) && p.CreatedBy == UserId).ToList();

                foreach (var item in Query)
                    resultList.Add(key: item.ID, value: item.TagName);
                return resultList;
            }
            catch (Exception)
            {
                throw;
            }

        }


        public IList<ModelFitnessTag> GetAllFitnessTagByPrefix(string tagPrefix)
        {
            try
            {
                List<ModelFitnessTag> resultList = new List<ModelFitnessTag>();
                var result = _dbcontext.FitnessTags.Where(p => p.TagName.StartsWith(tagPrefix) && p.IsActive == true).ToList();
                foreach (var item in result)
                    resultList.Add(DalToModel(item));
                return resultList;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public void SaveTagMappingWithVideo(Int32 VideoID, string tagMappingIDS)
        {
            try
            {
                int result = _dbcontext.Database.ExecuteSqlCommand("update [dbo].[VideoTagMapping] set IsActive=0 where [VideoID]=" + VideoID);

                if (!String.IsNullOrEmpty(tagMappingIDS))
                {
                    var list = tagMappingIDS.Split(',').ToList();
                    int tagid = 0;
                    var getall = _dbcontext.VideoTagMappings;
                    VideoTagMapping obj = new VideoTagMapping();
                    foreach (var item in list)
                    {
                        if (string.IsNullOrEmpty(item)) continue;
                        tagid = Convert.ToInt32(item);

                        var getsingle = getall.Where(p => p.VideoID == VideoID && p.TagID == tagid);
                        if (getsingle.Any())
                        {
                            getsingle.FirstOrDefault().IsActive = true;
                            _dbcontext.Entry(getsingle.FirstOrDefault()).State = System.Data.Entity.EntityState.Modified;
                            _dbcontext.SaveChanges();
                        }
                        else
                        {
                            obj = new VideoTagMapping();
                            obj.VideoID = VideoID;
                            obj.TagID = tagid;
                            obj.IsActive = true;
                            _dbcontext.VideoTagMappings.Add(obj);
                            _dbcontext.SaveChanges();
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public void SaveTagMappingWithImage(Int32 ImageID, string tagMappingIDS)
        {
            try
            {
                int result = _dbcontext.Database.ExecuteSqlCommand("update [dbo].[ImageTagMapping] set IsActive=0 where [ImageId]=" + ImageID);

                if (!String.IsNullOrEmpty(tagMappingIDS))
                {
                    var list = tagMappingIDS.Split(',').ToList();
                    int tagid = 0;
                    var getall = _dbcontext.ImageTagMappings;
                    var obj = new ImageTagMapping();
                    foreach (var item in list)
                    {
                        if (string.IsNullOrEmpty(item)) continue;
                        tagid = Convert.ToInt32(item);

                        var getsingle = getall.Where(p => p.ImageId == ImageID && p.TagId == tagid);
                        if (getsingle.Any())
                        {
                            getsingle.FirstOrDefault().IsActive = true;
                            _dbcontext.Entry(getsingle.FirstOrDefault()).State = System.Data.Entity.EntityState.Modified;
                            _dbcontext.SaveChanges();
                        }
                        else
                        {
                            obj = new ImageTagMapping();
                            obj.ImageId = ImageID;
                            obj.TagId = tagid;
                            obj.IsActive = true;
                            obj.CreatedDate = DateTime.UtcNow;
                            obj.UpdatedDate = DateTime.UtcNow;
                            _dbcontext.ImageTagMappings.Add(obj);
                            _dbcontext.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion
    }
}
