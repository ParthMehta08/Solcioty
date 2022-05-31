//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccessLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class BoxerTemplateVideoMapping
    {
        public int ID { get; set; }
        public int BoxerTemplateID { get; set; }
        public int VideoID { get; set; }
        public Nullable<int> VideoPosition { get; set; }
        public bool IsBasicVideo { get; set; }
        public bool IsAlterVideo { get; set; }
        public bool IsDeleted { get; set; }
        public string Note { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string Reps { get; set; }
        public string FileType { get; set; }
    
        public virtual BoxerTemplate BoxerTemplate { get; set; }
        public virtual Video Video { get; set; }
    }
}
