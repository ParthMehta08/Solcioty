using DataAccessLayer;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer
{
    public class BalCity : IDisposable
    {
        SolciotyNewEntities _dbcontext;
        public BalCity()
        {
            _dbcontext = new SolciotyNewEntities();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #region Methods
        public ModelCity DalToModel(City objDal)
        {
            ModelCity objModel = new ModelCity();
            try
            {
                objModel.CityName = objDal.CityName;
                objModel.ID = objDal.ID;
                objModel.StateCode = objDal.StateCode;
            }
            catch (Exception)
            {
                throw;
            }
            return objModel;
        }


        public CommonResultsMessage AddEdit(ModelCity objmodel)
        {
            try
            {
                if (CheckCityDuplication(objmodel.StateCode, objmodel.CityName))
                {
                    return CommonResultsMessage.duplicate;
                }

                if (objmodel.ID > 0)
                {
                    //var olddata = _dbcontext.Cities.FirstOrDefault(c => c.ID == objmodel.ID);
                    //olddata.CityName = objmodel.CityName;
                    //olddata.StateCode = objmodel.StateCode;
                    //_dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
                    //_dbcontext.SaveChanges();
                    var result = SaveCity(objmodel);
                    return CommonResultsMessage.Update;
                }
                else
                {
                    //City objDal = new City();
                    //objDal.CityName = objmodel.CityName;
                    //objDal.StateCode = objmodel.StateCode;
                    //_dbcontext.Cities.Add(objDal);
                    //_dbcontext.SaveChanges();

                    var result = SaveCity(objmodel);
                    return CommonResultsMessage.Add;
                }
            }
            catch (Exception)
            {
                return CommonResultsMessage.Fail;
            }
        }
        public bool CheckCityDuplication(string stateCode, string cityName)
        {
            try
            {
                var isDuplicate = _dbcontext.Cities.Any(c => c.CityName == cityName && c.StateCode == stateCode);
                return isDuplicate;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<ModelCity> GetAllCitiesByStateCode(string stateCode)
        {
            IList<ModelCity> cities = new List<ModelCity>();
            var cityList = new List<City>();
            try
            {
                cityList = _dbcontext.Cities.Where(e => e.StateCode == stateCode).ToList();

                foreach (var item in cityList)
                    cities.Add(DalToModel(item));
            }
            catch (Exception)
            {
                throw;
            }
            return cities;
        }


        public IList<ModelCity> GetAllCities()
        {
            IList<ModelCity> cities = new List<ModelCity>();
            var cityList = new List<City>();
            try
            {
                cityList = _dbcontext.Cities.ToList();

                foreach (var item in cityList)
                    cities.Add(DalToModel(item));
            }
            catch (Exception)
            {
                throw;
            }
            return cities;
        }

        public ModelCity GetById(int cityId)
        {
            try
            {
                return DalToModel(_dbcontext.Cities.FirstOrDefault(c => c.ID == cityId));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ModelCity SaveCity(ModelCity objmodel)
        {
            var olddata = _dbcontext.Cities.FirstOrDefault(c => c.ID == objmodel.ID || (c.CityName == objmodel.CityName && c.StateCode == objmodel.StateCode));
            if (olddata!=null && olddata.ID > 0)
            {//edit
                olddata.CityName = objmodel.CityName;
                olddata.StateCode = objmodel.StateCode;
                _dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
                _dbcontext.SaveChanges();
                objmodel.ID = olddata.ID;
                return objmodel;
            }
            else
            {//add
                City objDal = new City();
                objDal.CityName = objmodel.CityName;
                objDal.StateCode = objmodel.StateCode;
                _dbcontext.Cities.Add(objDal);
                _dbcontext.SaveChanges();
                objmodel.ID = objDal.ID;
                return objmodel;
            }
        }

        public int GetCityId(string CityName,string StateCode)
        {
            var city = new ModelCity()
            {
                CityName = CityName,
                StateCode = StateCode
            };
            var result = SaveCity(city);
            return result.ID;
        }
        #endregion
    }
}
