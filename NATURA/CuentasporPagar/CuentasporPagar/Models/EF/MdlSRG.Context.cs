﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CuentasporPagar.Models.EF
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SRGEntities : DbContext
    {
        public SRGEntities()
            : base("name=SRGEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CxPAsistentes> CxPAsistentes { get; set; }
        public virtual DbSet<PanelAsistArea> PanelAsistArea { get; set; }
        public virtual DbSet<PanelAsistProceso> PanelAsistProceso { get; set; }
        public virtual DbSet<RCAU> RCAU { get; set; }
        public virtual DbSet<RFPARAM> RFPARAM { get; set; }
        public virtual DbSet<RFADJUNTO> RFADJUNTO { get; set; }
        public virtual DbSet<CxPConceptos> CxPConceptos { get; set; }
        public virtual DbSet<CxPRegimen> CxPRegimen { get; set; }
        public virtual DbSet<CxPImpuestos> CxPImpuestos { get; set; }
        public virtual DbSet<CxPTasas> CxPTasas { get; set; }
        public virtual DbSet<CxPProceso> CxPProceso { get; set; }
        public virtual DbSet<CxPProveedor> CxPProveedor { get; set; }
    }
}
