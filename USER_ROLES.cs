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
    
    public partial class USER_ROLES
    {
        public int RELID { get; set; }
        public int ROLEREF { get; set; }
        public int USERREF { get; set; }
    
        public virtual ROLES ROLES { get; set; }
        public virtual USERS USERS { get; set; }
    }
}
