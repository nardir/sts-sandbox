using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Configuration.Settings
{
    public class SettingEntityTypeConfiguration : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.ToTable("Setting");

            builder.Property<int>("SettingId")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.HasKey("SettingId");

            builder.Property(s => s.Key)
                .HasColumnType("nvarchar(255)")
                .IsRequired();

            builder.HasAlternateKey(s => s.Key);

            builder.Property(s => s.Value)
                .HasColumnType("nvarchar(max)");

            builder.Property(s => s.ValueType)
                .HasColumnType("nvarchar(max)");
        }
    }
}
