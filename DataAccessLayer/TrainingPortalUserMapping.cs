
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
    
public partial class TrainingPortalUserMapping
{

    public int Id { get; set; }

    public int UserId { get; set; }

    public long TrainingPortalId { get; set; }

    public int PermissionType { get; set; }

    public System.DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public System.DateTime UpdatedDate { get; set; }

    public int UpdatedBy { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }



    public virtual TrainingPortal TrainingPortal { get; set; }

    public virtual User User { get; set; }

}

}
