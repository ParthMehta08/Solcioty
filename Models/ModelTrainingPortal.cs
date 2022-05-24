using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public class ModelTrainingPortal:BaseClass
	{
		public ModelTrainingPortal() {
			TrainingPortalCategories = new List<ModelTrainingPortalCategory>();
		}
		public long Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public List<ModelTrainingPortalCategory> TrainingPortalCategories { get; set; }
		

	}

	public class ModelTrainingPortalCategory : BaseClass
	{
		public ModelTrainingPortalCategory() {
			TrainingPortalSubCategories = new List<ModelTrainingPortalSubCategory>();
		}

		public long Id { get; set; }
		public string Name { get; set; }
		public long TraningPortalId { get; set; }
		public string IconString { get; set; }
		public int DisplayOrder { get; set; }
		public List<ModelTrainingPortalSubCategory> TrainingPortalSubCategories { get; set; }
	}


	public class ModelTrainingPortalSubCategory:BaseClass
	{
		public ModelTrainingPortalSubCategory()
		{
		}
		public long Id { get; set; }
		public string Name { get; set; }
		public long TraningPortalId { get; set; }
		public long TraningPortalCategoryId { get; set; }
		public string IconString { get; set; }
		public int DisplayOrder { get; set; }
	}

	public class BaseClass {
		public int CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public int UpdatedBy { get; set; }
		public DateTime UpdatedDate { get; set; }
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
	}

	public class ModelTrainingPortalSubCategoryVideo {

		public long Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string VideoPath { get; set; }
		public long PortalSubCategoryId { get; set; }
		public bool IsDeleted { get; set; }
		public string IsActiveString { get; set; }
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
	}


	public class ModelTrainingPortalUserMapping {
		public int UserId { get; set; }
		public long PortalId { get; set; }
		public int PermissionType { get; set; }
		public int CreatedBy { get; set; }
		public int UpdatedBy { get; set; }

	}

	public class ModelWorkoutUserMapping
	{
		public int UserId { get; set; }
		public int WorkoutId { get; set; }
		public int LocationId { get; set; }
		public bool IsChecked { get; set; }
		public int CreatedBy { get; set; }
		public int UpdatedBy { get; set; }

	}

	public class ModelBoxerWorkoutUserMapping
	{
		public int UserId { get; set; }
		public int BoxerWorkoutId { get; set; }
		public int LocationId { get; set; }
		public bool IsChecked { get; set; }
		public int CreatedBy { get; set; }
		public int UpdatedBy { get; set; }

	}
}
