//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MATE
{
    using System;
    using System.Collections.Generic;
    
    public partial class DEPARTMENTS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DEPARTMENTS()
        {
            this.DEPARTMENT_ANNOUNCEMENTS = new HashSet<DEPARTMENT_ANNOUNCEMENTS>();
            this.TEAMS = new HashSet<TEAMS>();
        }
    
        public int DEPID { get; set; }
        public int COMPREF { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public string DEPARTMENT_DEF { get; set; }
    
        public virtual COMPANY_INFO COMPANY_INFO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEPARTMENT_ANNOUNCEMENTS> DEPARTMENT_ANNOUNCEMENTS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEAMS> TEAMS { get; set; }
    }
}
