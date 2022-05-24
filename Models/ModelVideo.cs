using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
namespace Models{

    public class ModelFitnessTag
    {
        public int ID { get; set; }
        public string TagName { get; set; }
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
        public IList<ModelFitnessTag> FitnessTagList { get; set; }

        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }

        public string ClientName { get; set; }

    }
    public class ModelWorkoutType
    {
        public int ID { get; set; }
        [Required]
        public string WorkoutName { get; set; }
        // public Nullable<bool> IsActive { get; set; }
        public bool IsActive
        {
            get {
                bool t = false;
                if(!string.IsNullOrEmpty(this.IsActiveString))
                {
                    t = this.IsActiveString.ToUpper().Trim() == "ON" ? true : false;
                }
                return t;
            }
            set {
                if (!string.IsNullOrEmpty(this.IsActiveString))
                {
                    value= ( this.IsActiveString.ToUpper().Trim() == "ON" ? true : false);
                }
                value= false;
            }
        }
        public string IsActiveString { get; set; }
        public IList<ModelWorkoutType> WorkoutList { get; set; }
    }
    public class ModelVideoTagMapping
    {
        public int ID { get; set; }
        public int VideoID { get; set; }
        public int TagID { get; set; }
        public IList<ModelVideoTagMapping> VideoTagMappingList { get; set; }
    }
    public class ModelVideoWorkoutMapping
    {
        public int ID { get; set; }
        public int WorkoutID { get; set; }
        public int VideoID { get; set; }
        public IList<ModelVideoWorkoutMapping> VideoWorkoutMappingList { get; set; }
    }

    public class ModelVideo
    {
        public ModelVideo()
        {
            tagsDictionary = new Dictionary<int, string>();
            WorkoutTypesDictionary = new Dictionary<int, string>();
			Categories = new List<SelectListItem>();
        }
        public int ID { get; set; }
        public string VideoName { get; set; }
        public string FullName { get; set; }
        public string ContentType { get; set; }
        public string SmallDescription { get; set; }
        [AllowHtml]
        public string FullDescription { get; set; }
        public Nullable<int> CounterOfUseInTemplate { get; set; }
        public string Thumbnail { get; set; }
        public byte[] Video1 { get; set; }

        public int? RepsCount { get; set; }
        public Nullable<long> VideoSize { get; set; }
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
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }

        public IList<ModelVideo> VideoList { get; set; }
        public HttpPostedFileBase Videoclip { get; set; }

        public bool IsVideoStoreInDB { get; set; }
        public string VideoServerPath { get; set; }

        public String VideoTagsMappings { get; set; }
        public String VideoTagsMappingsNames { get; set; }

        public String VideoWorkoutTypesMappings { get; set; }
        public String VideoWorkoutTypesMappingsNames { get; set; }
        public Dictionary<Int32, String> tagsDictionary { get; set; }
        public Dictionary<Int32, String> WorkoutTypesDictionary { get; set; }

        public string ClientName { get; set; }

		public string CategoryId { get; set; }
		public string CategoryName { get; set; }
		public List<SelectListItem> Categories { get; set; }
		public int DisplayOrder { get; set; }


	}

	public class VideoDisplayOrderData {
		public int Id { get; set; }
		public int DisplayOrder { get; set; }
	}

}
