
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
    
public partial class BoxerWorkoutTemplateMapping
{

    public int ID { get; set; }

    public int BoxerWorkoutMasterID { get; set; }

    public int BoxerTemplateID { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public Nullable<System.DateTime> CreatedDate { get; set; }

    public Nullable<System.DateTime> UpdatedDate { get; set; }

    public Nullable<bool> IsDeleted { get; set; }

    public int DisplayOrder { get; set; }



    public virtual BoxerTemplate BoxerTemplate { get; set; }

    public virtual BoxerWorkoutMaster BoxerWorkoutMaster { get; set; }

}

}
