using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class ModelTemplate
    {
        public ModelTemplate()
        {
            ModelTemplateList = new List<ModelTemplate>();
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

        public IList<ModelTemplate> ModelTemplateList { get; set; }
        public IList<ModelTemplateVideoMapping> BasicModelTemplateVideoMappingList { get; set; }
        public IList<ModelTemplateVideoMapping> AlterModelTemplateVideoMappingList { get; set; }
        public IList<ModelVideo> VideosTable { get; set; }

        public string ClientName { get; set; }
    }

    public class ModelTemplateVideoMapping
    {
        public int ID { get; set; }
        public int TemplateID { get; set; }
        public int VideoID { get; set; }
        public int VideoPosition { get; set; }
        public bool IsBasicVideo { get; set; }
        public bool IsAlterVideo { get; set; }
        public string FileName { get; set; }
        public string Note { get; set; }
        public string Reps { get; set; }
    }


    public class TemplateDetail
    {
        public int WorkoutTemplateMapId { get; set; }
        public int TemplateId { get; set; }
        public string TemplateName { get; set; }
        public int NumberOfBasicVideos { get; set; }
        public int NumberOfAlternateVideos { get; set; }
        public List<TemplateVideoDetail> BasicVideos { get; set; }
        public List<TemplateVideoDetail> AlternateVideos { get; set; }
        public bool IsSelected { get; set; }
        public bool IsDeleted { get; set; }
        public int DisplayOrder { get; set; }
		public string PrimaryText { get; set; }
		public string PrimaryTextDisplay
		{
			get
			{
				if (!string.IsNullOrEmpty(PrimaryText))
					return string.Join("<br>", PrimaryText.ToCharArray());
				else
					return string.Empty;
			}
		}
		public string AlternateText { get; set; }
		public string AlternateTextDisplay
		{
			get
			{
				if (!string.IsNullOrEmpty(AlternateText))
					return string.Join("<br>", AlternateText.ToCharArray());
				else
					return string.Empty;
			}
		}

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
		public string Description { get; set; }


    }

    public class BoxerTemplateDetail
    {
        public int BoxerWorkoutTemplateMapId { get; set; }
        public int BoxerTemplateId { get; set; }
        public string BoxerTemplateName { get; set; }
        public int NumberOfBasicVideos { get; set; }
        public int NumberOfAlternateVideos { get; set; }
        public List<TemplateVideoDetail> BasicVideos { get; set; }
        public List<TemplateVideoDetail> AlternateVideos { get; set; }
        public bool IsSelected { get; set; }
        public bool IsDeleted { get; set; }
        public int DisplayOrder { get; set; }
        public string PrimaryText { get; set; }
        public string PrimaryTextDisplay
        {
            get
            {
                if (!string.IsNullOrEmpty(PrimaryText))
                    return string.Join("<br>", PrimaryText.ToCharArray());
                else
                    return string.Empty;
            }
        }
        public string AlternateText { get; set; }
        public string AlternateTextDisplay
        {
            get
            {
                if (!string.IsNullOrEmpty(AlternateText))
                    return string.Join("<br>", AlternateText.ToCharArray());
                else
                    return string.Empty;
            }
        }

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
        public string Description { get; set; }


    }

    public class TemplateVideoDetail
    {
        public int VideoId { get; set; }
        public int VideoPosition { get; set; }
        public bool IsBasicVideo { get; set; }
        public bool IsAlterVideo { get; set; }
        public string VideoFile { get; set; }
        public string VideoTitle { get; set; }


        public string SmallDescription { get; set; }

        public string Note { get; set; }
        public string Reps { get; set; }
        public string DisplayNote
        {
            get
            {
                if (!string.IsNullOrEmpty(Note) && Note.Length > 20)
                {
                    return Note.Substring(0, 20);
                }
                else
                {
                    return Note;
                }
                
            }
        }

    }
}