using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Configuration.Config.EFJson
{
    public class JsonEFConfigurationValueEntityTypeConfiguration : IEntityTypeConfiguration<JsonEFConfigurationValue>
    {
        public void Configure(EntityTypeBuilder<JsonEFConfigurationValue> builder)
        {
            builder.ToTable("ConfigurationValue", "config");

            builder.Property<int>("ConfigurationValueId")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.HasKey("ConfigurationValueId");

            builder.Property(v => v.Key)
                .HasColumnType("nvarchar(255)")
                .IsRequired();

            builder.HasAlternateKey(v => v.Key);

            builder.Property(v => v.Value)
                .HasColumnType("nvarchar(max)");
        }
    }
}
