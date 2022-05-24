using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataAccessLayer;

namespace BusinessLayer
{
    public class BALTemplate : IDisposable
    {
        SolciotyNewEntities _dbcontext;
        BalGym _balGym;
        public BALTemplate()
        {
            _dbcontext = new SolciotyNewEntities();
            _balGym = new BalGym();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #region Methods
        public ModelTemplate DalToModel(Template objDal)
        {
            ModelTemplate objModel = new ModelTemplate();
            try
            {
                objModel.ID = objDal.ID;
                objModel.IsDeleted = objDal.IsDeleted.HasValue ? objDal.IsDeleted.Value : false;
                objModel.IsReadyForLocations = objDal.IsReadyForLocations.HasValue ? objDal.IsReadyForLocations.Value : false;
                objModel.NumberOfAlterVideos = objDal.NumberOfAlterVideos;
                objModel.NumberOfBasicVideos = objDal.NumberOfBasicVideos;
                //   objModel.TemplateDescription = objDal.TemplateDescription;
                objModel.TemplateName = objDal.TemplateName;
                objModel.PrimaryText = objDal.PrimaryText;
                objModel.AlternateText = objDal.AlternateText;
                objModel.PrimaryColor = objDal.PrimaryColor;
                objModel.AlternateColor = objDal.AlternateColor;
                objModel.PrimaryBackgroundColor = objDal.PrimaryBackgroundColor;
                objModel.AlternateBackgroundColor = objDal.AlternateBackgroundColor;
                objModel.PrimaryGradientColor = objDal.PrimaryGradientColor;
                objModel.AlternateGradientColor = objDal.AlternateGradientColor;
                objModel.TimeText = objDal.TimeText;
                objModel.BlockLineOneText = objDal.BlockLineOneText;
                objModel.BlockLineTwoText = objDal.BlockLineTwoText;
                objModel.GoalText = objDal.GoalText;
                objModel.IsFooterText = Convert.ToBoolean(objDal.IsFooterText);
                objModel.FooterTextColor = objDal.FooterTextColor;
                objModel.FooterText = objDal.FooterText;
                var clientDetail = _balGym.GetClientDetailByUserId(Convert.ToInt32(objDal.CreatedBy));
                if (clientDetail != null)
                {
                    objModel.ClientName = clientDetail.Name;
                }
                objModel.CreatedBy = objDal.CreatedBy;
                objModel.CreatedDate = objDal.CreatedDate;
                objModel.UpdatedBy = objDal.UpdatedBy;
                objModel.UpdatedDate = objDal.UpdatedDate;

            }
            catch (Exception)
            {
                throw;
            }
            return objModel;
        }
        public ModelBoxerTemplate DalToModel(BoxerTemplate objDal)
        {
            ModelBoxerTemplate objModel = new ModelBoxerTemplate();
            try
            {
                objModel.ID = objDal.ID;
                objModel.IsDeleted = objDal.IsDeleted.HasValue ? objDal.IsDeleted.Value : false;
                objModel.IsReadyForLocations = objDal.IsReadyForLocations.HasValue ? objDal.IsReadyForLocations.Value : false;
                objModel.NumberOfAlterVideos = objDal.NumberOfAlterVideos;
                objModel.NumberOfBasicVideos = objDal.NumberOfBasicVideos;
                //   objModel.TemplateDescription = objDal.TemplateDescription;
                objModel.TemplateName = objDal.TemplateName;
                objModel.PrimaryText = objDal.PrimaryText;
                objModel.AlternateText = objDal.AlternateText;
                objModel.PrimaryColor = objDal.PrimaryColor;
                objModel.AlternateColor = objDal.AlternateColor;
                objModel.PrimaryBackgroundColor = objDal.PrimaryBackgroundColor;
                objModel.AlternateBackgroundColor = objDal.AlternateBackgroundColor;
                objModel.PrimaryGradientColor = objDal.PrimaryGradientColor;
                objModel.AlternateGradientColor = objDal.AlternateGradientColor;
                objModel.TimeText = objDal.TimeText;
                objModel.BlockLineOneText = objDal.BlockLineOneText;
                objModel.BlockLineTwoText = objDal.BlockLineTwoText;
                objModel.GoalText = objDal.GoalText;
                objModel.IsFooterText = Convert.ToBoolean(objDal.IsFooterText);
                objModel.FooterTextColor = objDal.FooterTextColor;
                objModel.FooterText = objDal.FooterText;
                var clientDetail = _balGym.GetClientDetailByUserId(Convert.ToInt32(objDal.CreatedBy));
                if (clientDetail != null)
                {
                    objModel.ClientName = clientDetail.Name;
                }
                objModel.CreatedBy = objDal.CreatedBy;
                objModel.CreatedDate = objDal.CreatedDate;
                objModel.UpdatedBy = objDal.UpdatedBy;
                objModel.UpdatedDate = objDal.UpdatedDate;

            }
            catch (Exception)
            {
                throw;
            }
            return objModel;
        }

        public CommonResultsMessage AddEdit(ModelTemplate objModel)
        {
            try
            {
                //check TEmplate name exists or not bcoz every TEmplate name must unique
                if (CheckTemplateNameDuplication(objModel.ID, objModel.TemplateName, Convert.ToInt32(objModel.CreatedBy)))
                {
                    return CommonResultsMessage.duplicate;
                }
                if (objModel.ID > 0)
                {//edit
                    var olddata = _dbcontext.Templates.FirstOrDefault(p => p.ID == objModel.ID);
                    olddata.UpdatedBy = objModel.UpdatedBy;//#PENDING
                    olddata.UpdatedDate = DateTime.UtcNow;
                    olddata.NumberOfAlterVideos = objModel.NumberOfAlterVideos;
                    olddata.NumberOfBasicVideos = objModel.NumberOfBasicVideos;
                    //   olddata.TemplateDescription = objModel.TemplateDescription;
                    olddata.TemplateName = objModel.TemplateName;
                    olddata.PrimaryText = objModel.PrimaryText;
                    olddata.AlternateText = objModel.AlternateText;
                    olddata.PrimaryColor = objModel.PrimaryColor;
                    olddata.AlternateColor = objModel.AlternateColor;
                    olddata.PrimaryGradientColor = objModel.PrimaryGradientColor;
                    olddata.AlternateGradientColor = objModel.AlternateGradientColor;
                    olddata.PrimaryBackgroundColor = objModel.PrimaryBackgroundColor;
                    olddata.AlternateBackgroundColor = objModel.AlternateBackgroundColor;
                    olddata.IsReadyForLocations = true;
                    olddata.TimeText = objModel.TimeText;
                    olddata.BlockLineOneText = objModel.BlockLineOneText;
                    olddata.BlockLineTwoText = objModel.BlockLineTwoText;
                    olddata.GoalText = objModel.GoalText;
                    olddata.IsFooterText = objModel.IsFooterText;
                    olddata.FooterTextColor = objModel.FooterTextColor;
                    olddata.FooterText = objModel.FooterText;

                    _dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
                    _dbcontext.SaveChanges();
#if false

#endif
                    return CommonResultsMessage.Update;
                }
                else
                {//add
                    Template objDal = new Template();
                    objDal.CreatedBy = objModel.CreatedBy;//#PENDING
                    objDal.UpdatedBy = objModel.UpdatedBy;//#PENDING
                    objDal.IsDeleted = false;
                    objDal.IsReadyForLocations = true;
                    objDal.NumberOfAlterVideos = objModel.NumberOfAlterVideos;
                    objDal.NumberOfBasicVideos = objModel.NumberOfBasicVideos;
                    //   objDal.TemplateDescription = objModel.TemplateDescription;
                    objDal.TemplateName = objModel.TemplateName;
                    objDal.PrimaryText = objModel.PrimaryText;
                    objDal.AlternateText = objModel.AlternateText;
                    objDal.PrimaryColor = objModel.PrimaryColor;
                    objDal.AlternateColor = objModel.AlternateColor;
                    objDal.PrimaryGradientColor = objModel.PrimaryGradientColor;
                    objDal.AlternateGradientColor = objModel.AlternateGradientColor;
                    objDal.PrimaryBackgroundColor = objModel.PrimaryBackgroundColor;
                    objDal.AlternateBackgroundColor = objModel.AlternateBackgroundColor;
                    objDal.TimeText = objModel.TimeText;
                    objDal.BlockLineOneText = objModel.BlockLineOneText;
                    objDal.BlockLineTwoText = objModel.BlockLineTwoText;
                    objDal.GoalText = objModel.GoalText;
                    objDal.IsFooterText = objModel.IsFooterText;
                    objDal.FooterTextColor = objModel.FooterTextColor;
                    objDal.FooterText = objModel.FooterText;
                    objDal.CreatedDate = DateTime.UtcNow;
                    objDal.UpdatedDate = DateTime.UtcNow;
                    _dbcontext.Templates.Add(objDal);
                    _dbcontext.SaveChanges();
                    return CommonResultsMessage.Add;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public CommonResultsMessage AddEditBoxer(ModelBoxerTemplate objModel)
        {
            try
            {
                //check TEmplate name exists or not bcoz every TEmplate name must unique
                if (CheckTemplateNameDuplicationBoxer(objModel.ID, objModel.TemplateName, Convert.ToInt32(objModel.CreatedBy)))
                {
                    return CommonResultsMessage.duplicate;
                }
                if (objModel.ID > 0)
                {//edit
                    var olddata = _dbcontext.BoxerTemplates.FirstOrDefault(p => p.ID == objModel.ID);
                    olddata.UpdatedBy = objModel.UpdatedBy;//#PENDING
                    olddata.UpdatedDate = DateTime.UtcNow;
                    olddata.NumberOfAlterVideos = objModel.NumberOfAlterVideos;
                    olddata.NumberOfBasicVideos = objModel.NumberOfBasicVideos;
                    //   olddata.TemplateDescription = objModel.TemplateDescription;
                    olddata.TemplateName = objModel.TemplateName;
                    olddata.PrimaryText = objModel.PrimaryText;
                    olddata.AlternateText = objModel.AlternateText;
                    olddata.PrimaryColor = objModel.PrimaryColor;
                    olddata.AlternateColor = objModel.AlternateColor;
                    olddata.PrimaryGradientColor = objModel.PrimaryGradientColor;
                    olddata.AlternateGradientColor = objModel.AlternateGradientColor;
                    olddata.PrimaryBackgroundColor = objModel.PrimaryBackgroundColor;
                    olddata.AlternateBackgroundColor = objModel.AlternateBackgroundColor;
                    olddata.IsReadyForLocations = true;
                    olddata.TimeText = objModel.TimeText;
                    olddata.BlockLineOneText = objModel.BlockLineOneText;
                    olddata.BlockLineTwoText = objModel.BlockLineTwoText;
                    olddata.GoalText = objModel.GoalText;
                    olddata.IsFooterText = objModel.IsFooterText;
                    olddata.FooterTextColor = objModel.FooterTextColor;
                    olddata.FooterText = objModel.FooterText;

                    _dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
                    _dbcontext.SaveChanges();
#if false

#endif
                    return CommonResultsMessage.Update;
                }
                else
                {//add
                    BoxerTemplate objDal = new BoxerTemplate();
                    objDal.CreatedBy = objModel.CreatedBy;//#PENDING
                    objDal.UpdatedBy = objModel.UpdatedBy;//#PENDING
                    objDal.IsDeleted = false;
                    objDal.IsReadyForLocations = true;
                    objDal.NumberOfAlterVideos = objModel.NumberOfAlterVideos;
                    objDal.NumberOfBasicVideos = objModel.NumberOfBasicVideos;
                    //   objDal.TemplateDescription = objModel.TemplateDescription;
                    objDal.TemplateName = objModel.TemplateName;
                    objDal.PrimaryText = objModel.PrimaryText;
                    objDal.AlternateText = objModel.AlternateText;
                    objDal.PrimaryColor = objModel.PrimaryColor;
                    objDal.AlternateColor = objModel.AlternateColor;
                    objDal.PrimaryGradientColor = objModel.PrimaryGradientColor;
                    objDal.AlternateGradientColor = objModel.AlternateGradientColor;
                    objDal.PrimaryBackgroundColor = objModel.PrimaryBackgroundColor;
                    objDal.AlternateBackgroundColor = objModel.AlternateBackgroundColor;
                    objDal.TimeText = objModel.TimeText;
                    objDal.BlockLineOneText = objModel.BlockLineOneText;
                    objDal.BlockLineTwoText = objModel.BlockLineTwoText;
                    objDal.GoalText = objModel.GoalText;
                    objDal.IsFooterText = objModel.IsFooterText;
                    objDal.FooterTextColor = objModel.FooterTextColor;
                    objDal.FooterText = objModel.FooterText;
                    objDal.CreatedDate = DateTime.UtcNow;
                    objDal.UpdatedDate = DateTime.UtcNow;
                    _dbcontext.BoxerTemplates.Add(objDal);
                    _dbcontext.SaveChanges();
                    return CommonResultsMessage.Add;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool CheckTemplateNameDuplication(Int32 ID, String TemplateNameNew, int ClientOwnerId)
        {
            try
            {
                var duplicate = _dbcontext.Templates.Where(p => p.ID != ID && p.TemplateName.ToLower().Trim() == TemplateNameNew.ToLower().Trim() && p.CreatedBy == ClientOwnerId);
                if (duplicate.Any())
                    return true;
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }
        public bool CheckTemplateNameDuplicationBoxer(Int32 ID, String TemplateNameNew, int ClientOwnerId)
        {
            try
            {
                var duplicate = _dbcontext.BoxerTemplates.Where(p => p.ID != ID && p.TemplateName.ToLower().Trim() == TemplateNameNew.ToLower().Trim() && p.CreatedBy == ClientOwnerId);
                if (duplicate.Any())
                    return true;
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }
        public IList<ModelTemplate> GetAllTemplates(int UserId, bool ForAdmin = false, bool withDeleted = false)
        {
            IList<ModelTemplate> Templates = new List<ModelTemplate>();
            var Query = new List<Template>();
            try
            {
                if (withDeleted)
                    Query = _dbcontext.Templates.Where(e => e.CreatedBy == UserId).ToList();
                else if (ForAdmin)
                    Query = _dbcontext.Templates.Where(e => e.IsDeleted != true).ToList();
                else
                    Query = _dbcontext.Templates.Where(p => (p.IsDeleted != true || p.IsDeleted == null) && p.CreatedBy == UserId).ToList();
                //if (ForAdmin)
                //    Query = Query.Where(p => p.IsReadyForLocations == true).ToList();
                foreach (var item in Query)
                    Templates.Add(DalToModel(item));
            }
            catch (Exception)
            {
                throw;
            }
            return Templates;
        }
        public IList<ModelBoxerTemplate> GetBoxerAllTemplates(int UserId, bool ForAdmin = false, bool withDeleted = false)
        {
            IList<ModelBoxerTemplate> BoxerTemplate = new List<ModelBoxerTemplate>();
            var Query = new List<BoxerTemplate>();
            try
            {
                if (withDeleted)
                    Query = _dbcontext.BoxerTemplates.Where(e => e.CreatedBy == UserId).ToList();
                else if (ForAdmin)
                    Query = _dbcontext.BoxerTemplates.Where(e => e.IsDeleted != true).ToList();
                else
                    Query = _dbcontext.BoxerTemplates.Where(p => (p.IsDeleted != true || p.IsDeleted == null) && p.CreatedBy == UserId).ToList();
                //if (ForAdmin)
                //    Query = Query.Where(p => p.IsReadyForLocations == true).ToList();
                foreach (var item in Query)
                    BoxerTemplate.Add(DalToModel(item));
            }
            catch (Exception)
            {
                throw;
            }
            return BoxerTemplate;
        }
        public CommonResultsMessage Delete(Int32 ID)
        {
            try
            {
                var olddata = _dbcontext.Templates.FirstOrDefault(p => p.ID == ID);
                olddata.IsReadyForLocations = false;
                olddata.IsDeleted = true;
                olddata.TemplateName = olddata.TemplateName + "_Deleted";
                foreach (var templateMapping in olddata.WorkoutTemplateMappings)
                {
                    templateMapping.IsDeleted = true;
                }
                foreach (var templateVideoMapping in olddata.TemplateVideoMappings)
                {
                    templateVideoMapping.IsDeleted = true;
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
                var olddata = _dbcontext.BoxerTemplates.FirstOrDefault(p => p.ID == ID);
                olddata.IsReadyForLocations = false;
                olddata.IsDeleted = true;
                olddata.TemplateName = olddata.TemplateName + "_Deleted";
                //foreach (var templateMapping in olddata.WorkoutTemplateMappings)
                //{
                //    templateMapping.IsDeleted = true;
                //}
                //foreach (var templateVideoMapping in olddata.TemplateVideoMappings)
                //{
                //    templateVideoMapping.IsDeleted = true;
                //}
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
                var allTemplates = _dbcontext.Templates.Where(x => Ids.Contains(x.ID)).ToList();
                foreach (var olddata in allTemplates)
                {
                    //var olddata = _dbcontext.Templates.FirstOrDefault(p => p.ID == ID);
                    olddata.IsReadyForLocations = false;
                    olddata.IsDeleted = true;
                    olddata.TemplateName = olddata.TemplateName + "_Deleted";
                    foreach (var templateMapping in olddata.WorkoutTemplateMappings)
                    {
                        templateMapping.IsDeleted = true;
                    }
                    foreach (var templateVideoMapping in olddata.TemplateVideoMappings)
                    {
                        templateVideoMapping.IsDeleted = true;
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
        public CommonResultsMessage BulkDeleteBoxer(List<int> Ids)
        {
            try
            {
                var allTemplates = _dbcontext.BoxerTemplates.Where(x => Ids.Contains(x.ID)).ToList();
                foreach (var olddata in allTemplates)
                {
                    //var olddata = _dbcontext.Templates.FirstOrDefault(p => p.ID == ID);
                    olddata.IsReadyForLocations = false;
                    olddata.IsDeleted = true;
                    olddata.TemplateName = olddata.TemplateName + "_Deleted";
                    //foreach (var templateMapping in olddata.WorkoutTemplateMappings)
                    //{
                    //    templateMapping.IsDeleted = true;
                    //}
                    //foreach (var templateVideoMapping in olddata.TemplateVideoMappings)
                    //{
                    //    templateVideoMapping.IsDeleted = true;
                    //}
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
        //public CommonResultsMessage TemplateIsReadyForLocations(Int32 ID, bool IsYes = false)
        //{
        //    try
        //    {
        //        var olddata = _dbcontext.Templates.FirstOrDefault(p => p.ID == ID);
        //        olddata.IsReadyForLocations = IsYes;
        //        _dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
        //        _dbcontext.SaveChanges();
        //        return CommonResultsMessage.Update;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        public CommonResultsMessage TemplateIsReadyForLocations(Int32 ID, bool IsYes = false)
        {
            try
            {
                var olddata = _dbcontext.Templates.FirstOrDefault(p => p.ID == ID);
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
        public CommonResultsMessage TemplateIsReadyForLocationsBoxer(Int32 ID, bool IsYes = false)
        {
            try
            {
                var olddata = _dbcontext.BoxerTemplates.FirstOrDefault(p => p.ID == ID);
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
        public ModelTemplate GetByID(Int32 ID)
        {
            try
            {
                return DalToModel(_dbcontext.Templates.FirstOrDefault(p => p.ID == ID));
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ModelBoxerTemplate GetByIDBoxer(Int32 ID)
        {
            try
            {
                return DalToModel(_dbcontext.BoxerTemplates.FirstOrDefault(p => p.ID == ID));
            }
            catch (Exception)
            {
                throw;
            }
        }
        public BoxerTemplateModel GetTemplateById(Int32 ID)
        {
            BoxerTemplateModel OBJ = new BoxerTemplateModel();
            var data = _dbcontext.BoxerTemplates.FirstOrDefault(p => p.ID == ID);

            var data2 = (from BWM in _dbcontext.BoxerWorkoutMasters
                         join BTM in _dbcontext.BoxerWorkoutTemplateMappings on BWM.ID equals BTM.BoxerWorkoutMasterID
                         join BT in _dbcontext.BoxerTemplates on BTM.BoxerTemplateID equals BT.ID
                         where BTM.BoxerTemplateID == ID 
                         select new BoxerWorkoutModelList
                         {
                             ID = BWM.ID,
                             WorkoutName = BWM.WorkoutName,
                             PDFName = BWM.PDFName,
                           

                         }).ToList();
            OBJ.ID = data.ID;
            OBJ.TemplateName = data.TemplateName;
            OBJ.TimeText = data.TimeText;
            OBJ.NumberOfBasicVideos = data.NumberOfBasicVideos;
            OBJ.NumberOfAlterVideos = data.NumberOfAlterVideos;
            OBJ.boxerWorkoutModelLists = data2;


            return OBJ;
        }

        #endregion
    }

    public class BALTemplateVideoMapping : IDisposable
    {
        SolciotyNewEntities _dbcontext;
        public BALTemplateVideoMapping()
        {
            _dbcontext = new SolciotyNewEntities();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #region Methods
        public CommonResultsMessage ADDEditVideointemplate(ModelTemplate objmodel)
        {
            try
            {
                // Basic Videos

                var BasicVideoMappingList = _dbcontext.TemplateVideoMappings.Where(p => p.TemplateID == objmodel.ID && p.IsDeleted == false && p.IsBasicVideo == true).ToList();
                foreach (var item in objmodel.BasicModelTemplateVideoMappingList)
                {
                    var find = BasicVideoMappingList.FirstOrDefault(p => p.VideoPosition == item.VideoPosition);
                    if (find != null)
                    {
                        if (item.VideoID > 0)
                        {
                            //edit
                            find.IsAlterVideo = false;
                            find.IsDeleted = false;
                            find.IsBasicVideo = true;
                            find.VideoID = item.VideoID;
                            find.Note = item.Note;
                            find.Reps = item.Reps;
                            _dbcontext.Entry(find).State = System.Data.Entity.EntityState.Modified;
                            _dbcontext.SaveChanges();
                        }
                        else
                        {
                            find.IsDeleted = true;
                            _dbcontext.Entry(find).State = System.Data.Entity.EntityState.Modified;
                            _dbcontext.SaveChanges();
                        }
                    }
                    else
                    {
                        //add
                        if (item.VideoID > 0)
                        {
                            var objdal = new TemplateVideoMapping();
                            //edit
                            objdal.TemplateID = objmodel.ID;
                            objdal.IsAlterVideo = false;
                            objdal.IsDeleted = false;
                            objdal.IsBasicVideo = true;
                            objdal.VideoID = item.VideoID;
                            objdal.Note = item.Note;
                            objdal.Reps = item.Reps;
                            objdal.VideoPosition = item.VideoPosition;
                            _dbcontext.TemplateVideoMappings.Add(objdal);
                            _dbcontext.SaveChanges();
                        }
                    }
                }
                //ALter Videos
                var AlterVideoMappingList = _dbcontext.TemplateVideoMappings.Where(p => p.TemplateID == objmodel.ID && p.IsDeleted == false && p.IsAlterVideo == true).ToList();
                foreach (var item in objmodel.AlterModelTemplateVideoMappingList)
                {
                    var find = AlterVideoMappingList.FirstOrDefault(p => p.VideoPosition == item.VideoPosition);
                    if (find != null)
                    {
                        if (item.VideoID > 0)
                        {
                            //edit
                            find.IsAlterVideo = true;
                            find.IsDeleted = false;
                            find.IsBasicVideo = false;
                            find.VideoID = item.VideoID;
                            find.Note = item.Note;
                            find.Reps = item.Reps;
                            _dbcontext.Entry(find).State = System.Data.Entity.EntityState.Modified;
                            _dbcontext.SaveChanges();
                        }
                        else
                        {
                            find.IsDeleted = true;
                            _dbcontext.Entry(find).State = System.Data.Entity.EntityState.Modified;
                            _dbcontext.SaveChanges();
                        }
                    }
                    else
                    {
                        //add
                        if (item.VideoID > 0)
                        {
                            var objdal = new TemplateVideoMapping();
                            //edit
                            objdal.TemplateID = objmodel.ID;
                            objdal.IsAlterVideo = true;
                            objdal.IsDeleted = false;
                            objdal.IsBasicVideo = false;
                            objdal.VideoID = item.VideoID;
                            objdal.VideoPosition = item.VideoPosition;
                            objdal.Note = item.Note;
                            objdal.Reps = item.Reps;
                            _dbcontext.TemplateVideoMappings.Add(objdal);
                            _dbcontext.SaveChanges();
                        }
                    }
                }

                return CommonResultsMessage.Update;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public CommonResultsMessage ADDEditVideointemplateBoxer(ModelBoxerTemplate objmodel)
        {
            try
            {
                // Basic Videos

                var BasicVideoMappingList = _dbcontext.BoxerTemplateVideoMappings.Where(p => p.BoxerTemplateID == objmodel.ID && p.IsDeleted == false && p.IsBasicVideo == true).ToList();
                foreach (var item in objmodel.BasicModelTemplateVideoMappingList)
                {
                    var find = BasicVideoMappingList.FirstOrDefault(p => p.VideoPosition == item.VideoPosition);
                    if (find != null)
                    {
                        if (item.VideoID > 0)
                        {
                            //edit
                            find.IsAlterVideo = false;
                            find.IsDeleted = false;
                            find.IsBasicVideo = true;
                            find.VideoID = item.VideoID;
                            find.Note = item.Note;
                            find.Reps = item.Reps;
                            _dbcontext.Entry(find).State = System.Data.Entity.EntityState.Modified;
                            _dbcontext.SaveChanges();
                        }
                        else
                        {
                            find.IsDeleted = true;
                            _dbcontext.Entry(find).State = System.Data.Entity.EntityState.Modified;
                            _dbcontext.SaveChanges();
                        }
                    }
                    else
                    {
                        //add
                        if (item.VideoID > 0)
                        {
                            var objdal = new BoxerTemplateVideoMapping();
                            //edit
                            objdal.BoxerTemplateID = objmodel.ID;
                            objdal.IsAlterVideo = false;
                            objdal.IsDeleted = false;
                            objdal.IsBasicVideo = true;
                            objdal.VideoID = item.VideoID;
                            objdal.Note = item.Note;
                            objdal.Reps = item.Reps;
                            objdal.VideoPosition = item.VideoPosition;
                            _dbcontext.BoxerTemplateVideoMappings.Add(objdal);
                            _dbcontext.SaveChanges();
                        }
                    }
                }
                //ALter Videos
                var AlterVideoMappingList = _dbcontext.BoxerTemplateVideoMappings.Where(p => p.BoxerTemplateID == objmodel.ID && p.IsDeleted == false && p.IsAlterVideo == true).ToList();
                foreach (var item in objmodel.AlterModelTemplateVideoMappingList)
                {
                    var find = AlterVideoMappingList.FirstOrDefault(p => p.VideoPosition == item.VideoPosition);
                    if (find != null)
                    {
                        if (item.VideoID > 0)
                        {
                            //edit
                            find.IsAlterVideo = true;
                            find.IsDeleted = false;
                            find.IsBasicVideo = false;
                            find.VideoID = item.VideoID;
                            find.Note = item.Note;
                            find.Reps = item.Reps;
                            _dbcontext.Entry(find).State = System.Data.Entity.EntityState.Modified;
                            _dbcontext.SaveChanges();
                        }
                        else
                        {
                            find.IsDeleted = true;
                            _dbcontext.Entry(find).State = System.Data.Entity.EntityState.Modified;
                            _dbcontext.SaveChanges();
                        }
                    }
                    else
                    {
                        //add
                        if (item.VideoID > 0)
                        {
                            var objdal = new BoxerTemplateVideoMapping();
                            //edit
                            objdal.BoxerTemplateID = objmodel.ID;
                            objdal.IsAlterVideo = true;
                            objdal.IsDeleted = false;
                            objdal.IsBasicVideo = false;
                            objdal.VideoID = item.VideoID;
                            objdal.VideoPosition = item.VideoPosition;
                            objdal.Note = item.Note;
                            objdal.Reps = item.Reps;
                            _dbcontext.BoxerTemplateVideoMappings.Add(objdal);
                            _dbcontext.SaveChanges();
                        }
                    }
                }

                return CommonResultsMessage.Update;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ModelTemplate GetTemplateVideos(ModelTemplate Template)
        {
            ModelTemplateVideoMapping model = new ModelTemplateVideoMapping();
            var MappingList = _dbcontext.TemplateVideoMappings.Where(p => p.TemplateID == Template.ID && p.IsDeleted == false).ToList();
            var DAtabaseBasicvideos = MappingList.Where(p => p.IsBasicVideo == true);
            //int newBasicvideos = Template.NumberOfBasicVideos - (DAtabaseBasicvideos.Any() ? DAtabaseBasicvideos.Count() : 0);
            var DAtabaseAltervideos = MappingList.Where(p => p.IsAlterVideo == true);
            // int newAltervideos = Template.NumberOfAlterVideos - (DAtabaseAltervideos.Any() ? DAtabaseAltervideos.Count() : 0);

            //basic videos
            for (int i = 0; i < Template.NumberOfBasicVideos; i++)
            {
                var find = DAtabaseBasicvideos.FirstOrDefault(p => p.VideoPosition == i);
                if (find != null)
                {
                    model = new ModelTemplateVideoMapping();
                    model.ID = find.ID;
                    model.IsAlterVideo = find.IsAlterVideo;
                    model.IsBasicVideo = find.IsBasicVideo;
                    model.TemplateID = find.TemplateID;
                    model.VideoID = find.VideoID;
                    model.Note = find.Note;
                    model.Reps = find.Reps;
                    model.VideoPosition = find.VideoPosition.HasValue ? find.VideoPosition.Value : 0;
                    model.FileName = find.Video.VideoAttachment;
                    Template.BasicModelTemplateVideoMappingList.Add(model);
                }
                else
                {
                    model = new ModelTemplateVideoMapping();
                    model.IsAlterVideo = false;
                    model.IsBasicVideo = true;
                    model.TemplateID = Template.ID;
                    model.VideoPosition = i;
                    model.VideoID = 0;
                    model.Note = string.Empty;
                    model.Reps = " ";
                    Template.BasicModelTemplateVideoMappingList.Add(model);
                }
            }
            //foreach (var item in DAtabaseBasicvideos)
            //{
            //    model = new ModelTemplateVideoMapping();
            //    model.ID = item.ID;
            //    model.IsAlterVideo = item.IsAlterVideo;
            //    model.IsBasicVideo = item.IsBasicVideo;
            //    model.TemplateID = item.TemplateID;
            //    model.VideoID = item.VideoID;
            //    model.VideoPosition = item.VideoPosition.HasValue ? item.VideoPosition.Value : 0;
            //    Template.BasicModelTemplateVideoMappingList.Add(model);
            //}
            //Alter videos
            for (int i = 0; i < Template.NumberOfAlterVideos; i++)
            {
                var find = DAtabaseAltervideos.FirstOrDefault(p => p.VideoPosition == i);
                if (find != null)
                {
                    model = new ModelTemplateVideoMapping();
                    model.ID = find.ID;
                    model.IsAlterVideo = find.IsAlterVideo;
                    model.IsBasicVideo = find.IsBasicVideo;
                    model.TemplateID = find.TemplateID;
                    model.VideoID = find.VideoID;
                    model.Note = find.Note;
                    model.Reps = find.Reps;
                    model.VideoPosition = find.VideoPosition.HasValue ? find.VideoPosition.Value : 0;
                    model.FileName = find.Video.VideoAttachment;
                    Template.AlterModelTemplateVideoMappingList.Add(model);
                }
                else
                {
                    model = new ModelTemplateVideoMapping();
                    model.IsAlterVideo = true;
                    model.IsBasicVideo = false;
                    model.TemplateID = Template.ID;
                    model.VideoPosition = i;
                    model.VideoID = 0;
                    model.Note = string.Empty;
                    model.Reps = " ";
                    Template.AlterModelTemplateVideoMappingList.Add(model);
                }
            }
            //foreach (var item in DAtabaseAltervideos)
            //{
            //    model = new ModelTemplateVideoMapping();
            //    model.ID = item.ID;
            //    model.IsAlterVideo = item.IsAlterVideo;
            //    model.IsBasicVideo = item.IsBasicVideo;
            //    model.TemplateID = item.TemplateID;
            //    model.VideoID = item.VideoID;
            //    model.VideoPosition = item.VideoPosition.HasValue ? item.VideoPosition.Value : 0;
            //    Template.AlterModelTemplateVideoMappingList.Add(model);
            //}

            //if (newBasicvideos > 0)
            //{
            //    for (int i = DAtabaseBasicvideos.Count(); i < Template.NumberOfBasicVideos; i++)
            //    {
            //        model = new ModelTemplateVideoMapping();
            //        model.IsAlterVideo = false;
            //        model.IsBasicVideo = true;
            //        model.TemplateID = Template.ID;
            //        model.VideoPosition = i;
            //        Template.BasicModelTemplateVideoMappingList.Add(model);
            //    }
            //}
            //if (newAltervideos > 0)
            //{
            //    for (int i = DAtabaseAltervideos.Count(); i < Template.NumberOfAlterVideos; i++)
            //    {
            //        model = new ModelTemplateVideoMapping();
            //        model.IsAlterVideo = true;
            //        model.IsBasicVideo = false;
            //        model.TemplateID = Template.ID;
            //        model.VideoPosition = i;
            //        Template.AlterModelTemplateVideoMappingList.Add(model);
            //    }
            //}
            return Template;
        }
        public ModelBoxerTemplate GetTemplateVideosBoxer(ModelBoxerTemplate Template)
        {
            ModelTemplateVideoMapping model = new ModelTemplateVideoMapping();
            var MappingList = _dbcontext.BoxerTemplateVideoMappings.Where(p => p.BoxerTemplateID == Template.ID && p.IsDeleted == false).ToList();
            var DAtabaseBasicvideos = MappingList.Where(p => p.IsBasicVideo == true);
            //int newBasicvideos = Template.NumberOfBasicVideos - (DAtabaseBasicvideos.Any() ? DAtabaseBasicvideos.Count() : 0);
            var DAtabaseAltervideos = MappingList.Where(p => p.IsAlterVideo == true);
            // int newAltervideos = Template.NumberOfAlterVideos - (DAtabaseAltervideos.Any() ? DAtabaseAltervideos.Count() : 0);

            //basic videos
            for (int i = 0; i < Template.NumberOfBasicVideos; i++)
            {
                var find = DAtabaseBasicvideos.FirstOrDefault(p => p.VideoPosition == i);
                if (find != null)
                {
                    model = new ModelTemplateVideoMapping();
                    model.ID = find.ID;
                    model.IsAlterVideo = find.IsAlterVideo;
                    model.IsBasicVideo = find.IsBasicVideo;
                    model.TemplateID = find.BoxerTemplateID;
                    model.VideoID = find.VideoID;
                    model.Note = find.Note;
                    model.Reps = find.Reps;
                    model.VideoPosition = find.VideoPosition.HasValue ? find.VideoPosition.Value : 0;
                    model.FileName = find.Video.VideoAttachment;
                    Template.BasicModelTemplateVideoMappingList.Add(model);
                }
                else
                {
                    model = new ModelTemplateVideoMapping();
                    model.IsAlterVideo = false;
                    model.IsBasicVideo = true;
                    model.TemplateID = Template.ID;
                    model.VideoPosition = i;
                    model.VideoID = 0;
                    model.Note = string.Empty;
                    model.Reps = " ";
                    Template.BasicModelTemplateVideoMappingList.Add(model);
                }
            }
            //foreach (var item in DAtabaseBasicvideos)
            //{
            //    model = new ModelTemplateVideoMapping();
            //    model.ID = item.ID;
            //    model.IsAlterVideo = item.IsAlterVideo;
            //    model.IsBasicVideo = item.IsBasicVideo;
            //    model.TemplateID = item.TemplateID;
            //    model.VideoID = item.VideoID;
            //    model.VideoPosition = item.VideoPosition.HasValue ? item.VideoPosition.Value : 0;
            //    Template.BasicModelTemplateVideoMappingList.Add(model);
            //}
            //Alter videos
            for (int i = 0; i < Template.NumberOfAlterVideos; i++)
            {
                var find = DAtabaseAltervideos.FirstOrDefault(p => p.VideoPosition == i);
                if (find != null)
                {
                    model = new ModelTemplateVideoMapping();
                    model.ID = find.ID;
                    model.IsAlterVideo = find.IsAlterVideo;
                    model.IsBasicVideo = find.IsBasicVideo;
                    model.TemplateID = find.BoxerTemplateID;
                    model.VideoID = find.VideoID;
                    model.Note = find.Note;
                    model.Reps = find.Reps;
                    model.VideoPosition = find.VideoPosition.HasValue ? find.VideoPosition.Value : 0;
                    model.FileName = find.Video.VideoAttachment;
                    Template.AlterModelTemplateVideoMappingList.Add(model);
                }
                else
                {
                    model = new ModelTemplateVideoMapping();
                    model.IsAlterVideo = true;
                    model.IsBasicVideo = false;
                    model.TemplateID = Template.ID;
                    model.VideoPosition = i;
                    model.VideoID = 0;
                    model.Note = string.Empty;
                    model.Reps = " ";
                    Template.AlterModelTemplateVideoMappingList.Add(model);
                }
            }
            //foreach (var item in DAtabaseAltervideos)
            //{
            //    model = new ModelTemplateVideoMapping();
            //    model.ID = item.ID;
            //    model.IsAlterVideo = item.IsAlterVideo;
            //    model.IsBasicVideo = item.IsBasicVideo;
            //    model.TemplateID = item.TemplateID;
            //    model.VideoID = item.VideoID;
            //    model.VideoPosition = item.VideoPosition.HasValue ? item.VideoPosition.Value : 0;
            //    Template.AlterModelTemplateVideoMappingList.Add(model);
            //}

            //if (newBasicvideos > 0)
            //{
            //    for (int i = DAtabaseBasicvideos.Count(); i < Template.NumberOfBasicVideos; i++)
            //    {
            //        model = new ModelTemplateVideoMapping();
            //        model.IsAlterVideo = false;
            //        model.IsBasicVideo = true;
            //        model.TemplateID = Template.ID;
            //        model.VideoPosition = i;
            //        Template.BasicModelTemplateVideoMappingList.Add(model);
            //    }
            //}
            //if (newAltervideos > 0)
            //{
            //    for (int i = DAtabaseAltervideos.Count(); i < Template.NumberOfAlterVideos; i++)
            //    {
            //        model = new ModelTemplateVideoMapping();
            //        model.IsAlterVideo = true;
            //        model.IsBasicVideo = false;
            //        model.TemplateID = Template.ID;
            //        model.VideoPosition = i;
            //        Template.AlterModelTemplateVideoMappingList.Add(model);
            //    }
            //}
            return Template;
        }
        #endregion
    }
}
