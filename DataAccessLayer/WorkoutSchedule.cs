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
    
    public partial class WorkoutSchedule
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WorkoutSchedule()
        {
            this.LocationSelectedWorkouts = new HashSet<LocationSelectedWorkout>();
        }
    
        public int ID { get; set; }
        public int WorkoutMasterID { get; set; }
        public System.DateTime ScheduledDate { get; set; }
        public int LocationId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    
        public virtual Location Location { get; set; }
        public virtual Location Location1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LocationSelectedWorkout> LocationSelectedWorkouts { get; set; }
        public virtual WorkoutMaster WorkoutMaster { get; set; }
    }
}
