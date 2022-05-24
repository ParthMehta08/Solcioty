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
    public class ModelBoxerWorkout
    {
        public ModelBoxerWorkout()
        {
            ModelWorkoutList = new List<ModelBoxerWorkout>();
            TemplateTable = new List<ModelBoxerTemplate>();
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
        public IList<ModelBoxerWorkout> ModelWorkoutList { get; set; }
        public IList<ModelBoxerTemplate> TemplateTable { get; set; }

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
}