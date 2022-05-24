using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataAccessLayer;
namespace BusinessLayer
{
   public class BalLocation : IDisposable
    {
        SolciotyNewEntities _dbcontext;
        BalCity _balCity;
        public BalLocation()
        {
            _dbcontext = new SolciotyNewEntities();
            _balCity = new BalCity();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #region Methods
        public ModelLocation DalToModel(Location objDal)
        {
            ModelLocation objModel = new ModelLocation();
            try
            {
                objModel.ID = objDal.ID;
                objModel.IsDeleted = objDal.IsDeleted.HasValue ? objDal.IsDeleted.Value : false;
                //objModel.IsActive = objDal.IsActive.HasValue ? objDal.IsActive.Value : false;

                objModel.IsActiveString = objDal.IsActive.HasValue ? (objDal.IsActive.Value == true ? "on" : "") : "";
                objModel.LocationName = objDal.LocationName;
                objModel.AddressLine1 = objDal.AddressLine1;
                objModel.AddressLine2 = objDal.AddressLine2;
                objModel.ZipCode = objDal.ZipCode;
                objModel.CityID = objDal.CityId!=null && objDal.CityId.HasValue?objDal.CityId.Value:0;
               
                objModel.Phone = objDal.Phone;
                objModel.CreatedBy = objDal.CreatedBy;
                objModel.CreatedDate = objDal.CreatedDate;
                objModel.UpdatedBy = objDal.UpdatedBy;
                objModel.UpdatedDate = objDal.UpdatedDate;
                objModel.CityName = objDal.City!=null?objDal.City.CityName:string.Empty;
            }
            catch (Exception)
            {
                throw;
            }
            return objModel;
        }


        public CommonResultsMessage AddEdit(ModelLocation objmodel)
        {
            try
            {
                if (CheckLocationNameDuplication(objmodel.ID, objmodel.LocationName))
                {
                    return CommonResultsMessage.duplicate;
                }
                var cityId = _balCity.GetCityId(objmodel.CityName, objmodel.StateCode);
                if (objmodel.ID>0)
                {//edit
                    var olddata = _dbcontext.Locations.FirstOrDefault(p => p.ID == objmodel.ID);
                    olddata.UpdatedBy = objmodel.UpdatedBy;//#PENDING
                    olddata.UpdatedDate = DateTime.UtcNow;
                    olddata.AddressLine1 = objmodel.AddressLine1;
                    olddata.AddressLine2 = objmodel.AddressLine2;
                    olddata.ZipCode = objmodel.ZipCode;
                    olddata.CityId = cityId>0?cityId:new Nullable<int>();
                    //olddata.City = objmodel.CityName;
                    olddata.IsActive = objmodel.IsActive;
                    olddata.LocationName = objmodel.LocationName;
                    olddata.Phone = objmodel.Phone;
                    _dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
                    _dbcontext.SaveChanges();
                    return CommonResultsMessage.Update;
                }
                else
                {//add
                    Location objDal = new Location();
                    objDal.CreatedBy = objmodel.CreatedBy;
                    objDal.UpdatedBy = objmodel.UpdatedBy;
                    objDal.IsDeleted = false;
                    objDal.CreatedDate = DateTime.UtcNow;
                    objDal.UpdatedDate = DateTime.UtcNow;
                    objDal.AddressLine1 = objmodel.AddressLine1;
                    objDal.AddressLine2 = objmodel.AddressLine2;
                    objDal.ZipCode = objmodel.ZipCode;
                    objDal.CityId = cityId > 0 ? cityId : new Nullable<int>();
                    //objDal.City = objmodel.CityName;
                    objDal.IsActive = objmodel.IsActive;
                    objDal.LocationName = objmodel.LocationName;
                    objDal.Phone = objmodel.Phone;
                    _dbcontext.Locations.Add(objDal);
                    _dbcontext.SaveChanges();
                    return CommonResultsMessage.Add;
                }
            }
            catch (Exception)
            {
                return CommonResultsMessage.Fail;
            }
        }
        public bool CheckLocationNameDuplication(Int32 ID, String LocationNameNew)
        {
            try
            {
                var duplicate = _dbcontext.Locations.Where(p => p.ID != ID && p.LocationName.ToLower().Trim() == LocationNameNew.ToLower().Trim());
                if (duplicate.Any())
                    return true;
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }

        public IList<ModelLocation> GetAllLocations(bool ForAdmin = false, bool withDeleted = false)
        {
            IList<ModelLocation> Locations = new List<ModelLocation>();
            var Query = new List<Location>();
            try
            {
                if (withDeleted)
                    Query = _dbcontext.Locations.Include("City").ToList();
                else
                    Query = _dbcontext.Locations.Include("City").Where(p => p.IsDeleted != true || p.IsDeleted == null).ToList();
                //if (ForAdmin)
                //    Query = Query.Where(p => p.IsReadyForLocations == true).ToList();
                foreach (var item in Query)
                    Locations.Add(DalToModel(item));
            }
            catch (Exception)
            {
                throw;
            }
            return Locations;
        }

        public CommonResultsMessage Delete(Int32 ID)
        {
            try
            {
                var olddata = _dbcontext.Locations.FirstOrDefault(p => p.ID == ID);
                olddata.IsActive = false;
                olddata.IsDeleted = true;
                olddata.LocationName = olddata.LocationName + "_Deleted";
                _dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
                _dbcontext.SaveChanges();
                return CommonResultsMessage.Update;
            }
            catch (Exception)
            {
                return CommonResultsMessage.Fail;
            }
        }

        public ModelLocation GetByID(Int32 ID)
        {
            try
            {
                return DalToModel(_dbcontext.Locations.Include("City").FirstOrDefault(p => p.ID == ID));
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
