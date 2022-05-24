using System;
using System.Collections.Generic;
using System.Web;

namespace Models
{
	public class ModelImage
	{
		public int Id { get; set; }
		public string ImageTitle { get; set; }
		public string Description { get; set; }
		public DateTime? CreatedDate { get; set; }
		public int? CreatedBy { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int? UpdatedBy { get; set; }
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
		public bool IsDeleted { get; set; }
		public HttpPostedFileBase ImageFile { get; set; }
		public byte[] Image1 { get; set; }
		public string ImageServerPath { get; set; }
		public int ImageSize { get; set; }
		public string ContentType { get; set; }
		public string FullName { get; set; }
		public Dictionary<int,string> tagsDictionary{get;set;}
		public string ImageName { get; set; }
		public List<ModelImage> ImageList { get; set; }
		public string ImageTagsMappingsNames { get; set; }
		public string SmallDescription { get; set; }
		public string ClientName { get; set; }
	}
}
