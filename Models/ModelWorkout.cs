using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace Models
{
  public  class ModelWorkout
    {
        public ModelWorkout()
        {
            ModelWorkoutList = new List<ModelWorkout>();
            TemplateTable = new List<ModelTemplate>();
            ImageList = new List<SelectListItem>();
        }
        public int ID { get; set; }
        public int TagID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string WorkoutDescription { get; set; }
        public bool IsReadyForLocations { get; set; }
        public string WorkoutName { get; set; }
        public bool IsDeleted { get; set; }
        public IList<ModelWorkout> ModelWorkoutList { get; set; }
        public IList<ModelTemplate> TemplateTable { get; set; }

        public HttpPostedFileBase WorkoutPDF { get; set; }

        public byte[] PDF { get; set; }

        public string PDFName { get; set; }

        public string PDFServerPath { get; set; }

        public string ClientName { get; set; }
        public IList<SelectListItem> ImageList { get; set; }

        public int? FrontCoverImageId { get; set; }
        public int? BackCoverImageId { get; set; }
        public string FrontCoverImageName { get; set; }
        public string BackCoverImageName { get; set; }
		public bool IsFutureWorkout { get; set; }

		public string CategoryId { get; set; }
		public string CategoryName { get; set; }
		public List<SelectListItem> Categories { get; set; }
	}
    public  class ModelWorkoutTemplateMapping
    {
        public int ID { get; set; }
        public int WorkoutMasterID { get; set; }
        public int TemplateID { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public int DisplayOrder { get; set; }
    }


    public class ModelWorkoutTemplateDetail
    {
        public int Id { get; set; }
        public int WorkoutId { get; set; }

        public List<TemplateDetail> WorkoutTemplates { get; set; }

    }


    public class ModelBoxerWorkoutTemplateDetail
    {
        public int Id { get; set; }
        public int BoxerWorkoutId { get; set; }

        public List<BoxerTemplateDetail> BoxerWorkoutTemplates { get; set; }

    }

    public class WorkoutDetailModel
	{
		public int WorkoutId { get; set; }
		public string WorkoutName { get; set; }
		public ModelWorkoutTemplateDetail WorkoutTemplateDetail { get; set; }
		public List<TemplateDetail> Templates { get; set; }
		public List<ModelTemplate> TemplateList { get; set; }

		public string WorkoutPDF { get; set; }

		public int? FrontCoverImageId { get; set; }
		public int? BackCoverImageId { get; set; }
		public string FrontCoverImageName { get; set; }
		public string BackCoverImageName { get; set; }
		public bool IsFutureWorkout { get; set; }
    }

    public class BoxerWorkoutDetailModel
    {
        public int BoxerWorkoutId { get; set; }
        public string BoxerWorkoutName { get; set; }
        public ModelBoxerWorkoutTemplateDetail BoxerWorkoutTemplateDetail { get; set; }
        public List<BoxerTemplateDetail> BoxerTemplates { get; set; }
        public List<ModelBoxerTemplate> BoxerTemplateList { get; set; }

        public string WorkoutPDF { get; set; }

        public int? FrontCoverImageId { get; set; }
        public int? BackCoverImageId { get; set; }
        public string FrontCoverImageName { get; set; }
        public string BackCoverImageName { get; set; }
        public bool IsFutureWorkout { get; set; }
    }

    public class WorkoutTemplateMapRequest
    {
        public int WorkoutId { get; set; }
        public int TemplateId { get; set; }
        public int WorkoutTemplateMapId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class BoxerWorkoutTemplateMapRequest
    { 
        public int BoxerWorkoutId { get; set; }
        public int BoxerTemplateId { get; set; }
        public int BoxerWorkoutTemplateMapId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class WorkoutTemplateDisplayOrder
    {
        public int WorkoutTemplateMapId { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class BoxerWorkoutTemplateDisplayOrder
    {
        public int BoxerWorkoutTemplateMapId { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class ModelWorkoutSchedule
    {
        public ModelWorkoutSchedule()
        {
            WorkoutList = new List<ModelWorkout>();
        }
        public int WorkoutScheduleId { get; set; }
        public int WorkoutId { get; set; }
        public string WorkoutName { get; set; }
        public DateTime ScheduledDate { get; set; }

        public string DisplayDate { get {
                return this.ScheduledDate.ToString("MM/dd/yyyy");
            } }
        public bool IsDeleted { get; set; }
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

        public IList<ModelWorkout> WorkoutList { get; set; }

        public bool IsSelected { get; set; }

        public int LocationId { get; set; }
    }

    public class ModelBoxerWorkoutSchedule
    {
        public ModelBoxerWorkoutSchedule()
        {
            BoxerWorkoutList = new List<ModelBoxerWorkout>();
        }
        public int BoxerWorkoutScheduleId { get; set; }
        public int BoxerWorkoutId { get; set; }
        public string BoxerWorkoutName { get; set; }
        public DateTime ScheduledDate { get; set; }

        public string DisplayDate
        {
            get
            {
                return this.ScheduledDate.ToString("MM/dd/yyyy");
            }
        }
        public bool IsDeleted { get; set; }
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

        public IList<ModelBoxerWorkout> BoxerWorkoutList { get; set; }

        public bool IsSelected { get; set; }

        public int LocationId { get; set; }
    }


    public class ModelWorkoutBulkSchedule
	{
		public ModelWorkoutBulkSchedule()
		{
			WorkoutList = new List<ModelWorkout>();
			BulkScheduledWorkout = new List<LocationWorkoutModel>();

		}
		public int WorkoutId { get; set; }
		public string WorkoutName { get; set; }
		public DateTime ScheduledDate { get; set; }

		public string DisplayDate
		{
			get
			{
				return this.ScheduledDate.ToString("MM/dd/yyyy");
			}
		}

		public IList<ModelWorkout> WorkoutList { get; set; }

		public IList<LocationWorkoutModel> BulkScheduledWorkout { get; set; }
		public IList<LocationWorkoutModel> BulkScheduledSelectedWorkout { get; set; }
		public IList<LocationWorkoutModel> BulkScheduledActiveWorkout { get; set; }

	}

    public class ModelBoxerWorkoutBulkSchedule
    {
        public ModelBoxerWorkoutBulkSchedule()
        {
            BoxerWorkoutList = new List<ModelBoxerWorkout>();
            BulkScheduledBoxerWorkout = new List<LocationBoxerWorkoutModel>();

        }
        public int BoxerWorkoutId { get; set; }
        public string BoxerWorkoutName { get; set; }
        public DateTime ScheduledDate { get; set; }

        public string DisplayDate
        {
            get
            {
                return this.ScheduledDate.ToString("MM/dd/yyyy");
            }
        }

        public IList<ModelBoxerWorkout> BoxerWorkoutList { get; set; }

        public IList<LocationBoxerWorkoutModel> BulkScheduledBoxerWorkout { get; set; }
        public IList<LocationBoxerWorkoutModel> BulkScheduledSelectedBoxerWorkout { get; set; }
        public IList<LocationBoxerWorkoutModel> BulkScheduledActiveBoxerWorkout { get; set; }

    }

    public class LocationWorkoutModel {
		public int WorkoutId { get; set; }
		public bool IsSelected { get; set; }
		public bool IsActive { get; set; }
		public int LocationId { get; set; }
		public string LocationName { get; set; }
	}

    public class LocationBoxerWorkoutModel
    {
        public int BoxerWorkoutId { get; set; }
        public bool IsSelected { get; set; }
        public bool IsActive { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
    }
}
