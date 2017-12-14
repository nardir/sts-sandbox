using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Configuration.Config.EFJson
{
    public class JsonEFConfigurationContext: DbContext
    {
        public JsonEFConfigurationContext()
        {
        }

        public JsonEFConfigurationContext(DbContextOptions<JsonEFConfigurationContext> options)
            : base(options)
        {
        }

        public JsonEFConfigurationContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<JsonEFConfigurationValue> Values { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new JsonEFConfigurationValueEntityTypeConfiguration());
        }
    }
}
