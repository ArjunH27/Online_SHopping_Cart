//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Online_SHopping_Cart.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public partial class Role_Table
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Role_Table()
        {
            this.User_Table = new HashSet<User_Table>();
        }

        public int RoleId { get; set; }
        [Remote("IsRoleNameExist", "Admin",
              ErrorMessage = "Role name already exists")]
        [Required]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Use alphabets and spaces only please.")]
        public string RoleName { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Use alphabets and spaces only please.")]
        public string RoleDesc { get; set; }
        public string RoleCreatedBy { get; set; }
        public System.DateTime RoleCreatedDate { get; set; }
        public string RoleUpdatedBy { get; set; }
        public System.DateTime RoleUpdateDate { get; set; }
        public bool RoleIsDeleted { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User_Table> User_Table { get; set; }
    }
}
