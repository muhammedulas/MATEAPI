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
    
    public partial class COMPANY_INFO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public COMPANY_INFO()
        {
            this.COMPANY_ANNOUNCEMENTS = new HashSet<COMPANY_ANNOUNCEMENTS>();
            this.DEPARTMENTS = new HashSet<DEPARTMENTS>();
        }
    
        public int COMPID { get; set; }
        public string COUNTRY { get; set; }
        public string CITY { get; set; }
        public string DISTRICT { get; set; }
        public string OPEN_ADRESS { get; set; }
        public string POSTAL_CODE { get; set; }
        public string MAIL { get; set; }
        public string PHONE { get; set; }
        public string TITLE { get; set; }
        public string COMMERCIAL_TITLE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COMPANY_ANNOUNCEMENTS> COMPANY_ANNOUNCEMENTS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEPARTMENTS> DEPARTMENTS { get; set; }
    }
}
