using System.Collections.Generic;
using System.Web.Mvc;

namespace Models
{
    public  class ModelCity
    {
        public int ID { get; set; }
        public string CityName { get; set; }
        public string StateCode { get; set; }
        public SelectList States { get; set; }
        public IList<ModelCity> CityList { get; set; }
    }
    public  class ModelState
    {
        public string StateName { get; set; }
        public string Abbreviation { get; set; }
        public IList<ModelState> StateList { get; set; }
    }
}