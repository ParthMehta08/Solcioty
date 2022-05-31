using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class ModelBoxerTemplate
    {
        public ModelBoxerTemplate()
        {
            ModelTemplateList = new List<ModelBoxerTemplate>();
            BasicModelTemplateVideoMappingList = new List<ModelTemplateVideoMapping>();
            AlterModelTemplateVideoMappingList = new List<ModelTemplateVideoMapping>();
            VideosTable = new List<ModelVideo>();
        }
        public int ID { get; set; }
        public int TagID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
       // public string TemplateDescription { get; set; }
        public bool IsReadyForLocations { get; set; }
        public string TemplateName { get; set; }
        public int NumberOfBasicVideos { get; set; }
        public int NumberOfAlterVideos { get; set; }
        public bool IsDeleted { get; set; }
		public string PrimaryText { get; set; }
		public string AlternateText { get; set; }
		public string PrimaryColor { get; set; }
		public string AlternateColor { get; set; }
		public string PrimaryGradientColor { get; set; }

        public string TimeText { get; set; }
        public string BlockLineOneText { get; set; }
        public string BlockLineTwoText { get; set; }
        public string GoalText { get; set; }

        public bool IsFooterText { get; set; }
        public string FooterTextColor { get; set; }
        public string FooterText { get; set; }

        public string AlternateGradientColor { get; set; }
		public string PrimaryBackgroundColor { get; set; }
		public string AlternateBackgroundColor { get; set; }
		public int? DisplayOrder { get; set; }

        public IList<ModelBoxerTemplate> ModelTemplateList { get; set; }
        public IList<ModelTemplateVideoMapping> BasicModelTemplateVideoMappingList { get; set; }
        public IList<ModelTemplateVideoMapping> AlterModelTemplateVideoMappingList { get; set; }
        public IList<ModelVideo> VideosTable { get; set; }
        public IList<ModelImage> ImagesTable { get; set; }

        public string ClientName { get; set; }
    }
}