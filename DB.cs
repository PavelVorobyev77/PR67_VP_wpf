using PR67_VP.model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR67_VP
{
    internal class DB : DbContext
    {
        public DbSet<Workers> Workers { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Clients> Clients { get; set; }
        public DbSet<AdditionalServices> AdditionalServices { get; set; }
        public DbSet<ConstructionMaterials> ConstructionMaterials { get; set; }
        public DbSet<ConstructionPhases> ConstructionPhases { get; set; }
        public DbSet<DesignerServices> DesignerService { get; set; }
        public DbSet<FinancialData> FinancialData { get; set; }
        public DbSet<FinishingMaterials> FinishingMaterials { get; set; }
        public DbSet<MaterialBatches> MaterialBatches { get; set; }
        public DbSet<Objects> Objects { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<SecurityServices> SecurityServices { get; set; }
    }
}

