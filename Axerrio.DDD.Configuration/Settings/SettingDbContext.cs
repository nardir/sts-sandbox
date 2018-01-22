using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Axerrio.DDD.Configuration.Settings
{
    public class SettingDbContext : DbContext, ISettingDbContext
    {
        public DbSet<Setting> Settings { get; set; }

        public SettingDbContext() { }

        public SettingDbContext(DbContextOptions<SettingDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SettingEntityTypeConfiguration());
        }
    }
}
