using System.Collections.Generic;
namespace Models
{
	public class ModelCategory
	{
		public int Id { get; set; }
		public string Name { get; set; }
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
		public IList<ModelCategory> CategoryList { get; set; }

		public int CreatedBy { get; set; }
		public int UpdatedBy { get; set; }

		public string ClientName { get; set; }

	}
}
