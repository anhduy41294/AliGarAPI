﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AliGarAPI.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class QLAliGarEntities : DbContext
    {
        public QLAliGarEntities()
            : base("name=QLAliGarEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ActionType> ActionTypes { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<RecordAction> RecordActions { get; set; }
        public virtual DbSet<RecordSituation> RecordSituations { get; set; }
        public virtual DbSet<UserMode> UserModes { get; set; }
    }
}
