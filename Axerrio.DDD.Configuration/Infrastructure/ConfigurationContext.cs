using Axerrio.DDD.Configuration.Config.EFJson;
using Axerrio.DDD.Configuration.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Configuration.Infrastructure
{
    public class ConfigurationContext: DbContext
    {
        public ConfigurationContext()
        {
        }

        public ConfigurationContext(DbContextOptions<ConfigurationContext> options)
            : base(options)
        {
        }

        public ConfigurationContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<ConfigurationSection> Sections { get; set; }

        public DbSet<ConfigurationSetting> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new JsonEFConfigurationValueEntityTypeConfiguration());
        }
    }
}
