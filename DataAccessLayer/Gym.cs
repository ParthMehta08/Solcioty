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
    
    public partial class Gym
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Gym()
        {
            this.GymLocationMappings = new HashSet<GymLocationMapping>();
            this.GymUserMappings = new HashSet<GymUserMapping>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public string PrimaryText { get; set; }
        public string PrimaryBackground { get; set; }
        public string PrimaryGradientBackground { get; set; }
        public string PrimaryFontColor { get; set; }
        public string AlternateText { get; set; }
        public string AlternateBackground { get; set; }
        public string AlternateGradientBackground { get; set; }
        public string AlternateFontColor { get; set; }
        public string LogoutBackground { get; set; }
        public Nullable<bool> ShowTime { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string AlternateLogo { get; set; }
        public string VideoTitleColor { get; set; }
        public string VideoTitleBackgroundColor { get; set; }
        public string BackgroundImage { get; set; }
        public string HeaderPrimaryColor { get; set; }
        public string HeaderSecondaryColor { get; set; }
        public string HeaderTextColor { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GymLocationMapping> GymLocationMappings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GymUserMapping> GymUserMappings { get; set; }
    }
}
