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
    
    public partial class USER_CONTACT_INFO
    {
        public int ID { get; set; }
        public int USERREF { get; set; }
        public string PHONE { get; set; }
        public string MAIL { get; set; }
        public string COUNTRY { get; set; }
        public string CITY { get; set; }
        public string DISTRICT { get; set; }
        public string POSTAL_CODE { get; set; }
        public string OPEN_ADRESS { get; set; }
        public string PROFILE_PHOTO { get; set; }
    
        public virtual USERS USERS { get; set; }
    }
}
