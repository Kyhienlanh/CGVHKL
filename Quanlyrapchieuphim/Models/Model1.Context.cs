﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Quanlyrapchieuphim.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class QLRapChieuPhimEntities : DbContext
    {
        public QLRapChieuPhimEntities()
            : base("name=QLRapChieuPhimEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ChoNgoi> ChoNgois { get; set; }
        public virtual DbSet<ChoNgoi__SuatChieu> ChoNgoi__SuatChieu { get; set; }
        public virtual DbSet<DichVu> DichVus { get; set; }
        public virtual DbSet<DV_VE> DV_VE { get; set; }
        public virtual DbSet<HoaDon> HoaDons { get; set; }
        public virtual DbSet<NguoiDung> NguoiDungs { get; set; }
        public virtual DbSet<Phim> Phims { get; set; }
        public virtual DbSet<PhongChieu> PhongChieux { get; set; }
        public virtual DbSet<SuatChieu> SuatChieux { get; set; }
        public virtual DbSet<Ve1> Ve1 { get; set; }
    }
}
