using DataAccessLayer;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class BalState : IDisposable
    {
        SolciotyNewEntities _dbcontext;
        public BalState()
        {
            _dbcontext = new SolciotyNewEntities();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #region Methods
        public ModelState DalToModel(State objDal)
        {
            ModelState objModel = new ModelState();
            try
            {
                objModel.StateName = objDal.StateName;
                objModel.Abbreviation = objDal.Abbreviation;
            }
            catch (Exception)
            {
                throw;
            }
            return objModel;
        }


        public CommonResultsMessage AddEdit(ModelState objmodel)
        {
            try
            {
                if (CheckStateDuplication(objmodel.Abbreviation,objmodel.StateName))
                {
                    return CommonResultsMessage.duplicate;
                }

                var olddata = _dbcontext.States.FirstOrDefault(s => s.Abbreviation == objmodel.Abbreviation);
                if (olddata!=null)
                {//edit
                    olddata.StateName = objmodel.StateName;
                    _dbcontext.Entry(olddata).State = System.Data.Entity.EntityState.Modified;
                    _dbcontext.SaveChanges();
                    return CommonResultsMessage.Update;
                }
                else
                {//add
                    State objDal = new State();
                    objDal.StateName = objmodel.StateName;
                    _dbcontext.States.Add(objDal);
                    _dbcontext.SaveChanges();
                    return CommonResultsMessage.Add;
                }
            }
            catch (Exception)
            {
                return CommonResultsMessage.Fail;
            }
        }
        public bool CheckStateDuplication(string stateCode,string stateName)
        {
            try
            {
                var isDuplicate = _dbcontext.States.Any(s => s.Abbreviation == stateCode && s.StateName == stateName);
                return isDuplicate;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<ModelState> GetAllStates()
        {
            IList<ModelState> states = new List<ModelState>();
            var stateList = new List<State>();
            try
            {
                stateList = _dbcontext.States.ToList();
               
                foreach (var item in stateList)
                    states.Add(DalToModel(item));
            }
            catch (Exception ex)
            {
                throw;
            }
            return states;
        }
        
        public ModelState GetByStateCode(string stateCode)
        {
            try
            {
                return DalToModel(_dbcontext.States.FirstOrDefault(s=>s.Abbreviation == stateCode));
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
