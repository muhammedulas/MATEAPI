﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class MATEEntities : DbContext
    {
        public MATEEntities()
            : base("name=MATEEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<COMPANY_ANNOUNCEMENTS> COMPANY_ANNOUNCEMENTS { get; set; }
        public virtual DbSet<COMPANY_INFO> COMPANY_INFO { get; set; }
        public virtual DbSet<DEPARTMENT_ANNOUNCEMENTS> DEPARTMENT_ANNOUNCEMENTS { get; set; }
        public virtual DbSet<DEPARTMENTS> DEPARTMENTS { get; set; }
        public virtual DbSet<PERSONAL_TASKS> PERSONAL_TASKS { get; set; }
        public virtual DbSet<ROLES> ROLES { get; set; }
        public virtual DbSet<TASKS> TASKS { get; set; }
        public virtual DbSet<TEAM_ANNOUNCEMENTS> TEAM_ANNOUNCEMENTS { get; set; }
        public virtual DbSet<TEAM_MEMBERS> TEAM_MEMBERS { get; set; }
        public virtual DbSet<TEAMS> TEAMS { get; set; }
        public virtual DbSet<USER_CONTACT_INFO> USER_CONTACT_INFO { get; set; }
        public virtual DbSet<USER_ROLES> USER_ROLES { get; set; }
        public virtual DbSet<USERS> USERS { get; set; }
        public virtual DbSet<VW_DEPARTMENTS> VW_DEPARTMENTS { get; set; }
        public virtual DbSet<VW_PERSONAL_TASKS> VW_PERSONAL_TASKS { get; set; }
        public virtual DbSet<VW_TEAMS> VW_TEAMS { get; set; }
        public virtual DbSet<VW_USERS_DETAILED> VW_USERS_DETAILED { get; set; }
    }
}
