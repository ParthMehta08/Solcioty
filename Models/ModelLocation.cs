using System;
using System.Collections.Generic;
namespace Models
{
    public class ModelLocation
    {
        public ModelLocation()
        {
            ModelLocationList =new List<ModelLocation>();
            StateList = new List<ModelState>();
            CityList = new List<ModelCity>();
        }
        public int ID { get; set; }
        public string LocationName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public int CityID { get; set; }

        public string CityName { get; set; }
        public bool IsDeleted { get; set; }
        public IList<ModelLocation> ModelLocationList { get; set; }
        public IList<ModelState> StateList { get; set; }
        public IList<ModelCity> CityList { get; set; }

        public string StateCode { get; set; }

        public bool IsActive
        {
            get
            {
                bool t = false;
                if (!string.IsNullOrEmpty(this.IsActiveString))
                {
                    t = this.IsActiveString.ToUpper().Trim() == "ON" ? true : false;
                }
                return t;
            }
            set
            {
                if (!string.IsNullOrEmpty(this.IsActiveString))
                {
                    value = (this.IsActiveString.ToUpper().Trim() == "ON" ? true : false);
                }
                value = false;
            }
        }
        public string IsActiveString { get; set; }

    }


    public class GymLocationMapping: ModelLocation
    {
        public long GymLocationMappingId { get; set; }
        public int GymId { get; set; }
    }


    public class ModelBranch
    {
        public ModelBranch()
        {
            UserList = new List<ModelUser>();
        }
        public int Id { get; set; }
        public int GymId { get; set; }
        public string BranchName { get; set; }
        public string GymName { get; set; }
        public IList<ModelUser> UserList { get; set; }
    }



}
