using Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Axerrio.DDD.Menu.Infrastructure.EntityTypeConfigurations
{
    public class MenuStatusEntityTypeConfiguration : IEntityTypeConfiguration<MenuStatus>
    {
        public void Configure(EntityTypeBuilder<MenuStatus> menuStatusConfiguration)
        {
            menuStatusConfiguration.ToTable("MenuStatus");

            menuStatusConfiguration.HasKey(s => s.Id);

            menuStatusConfiguration.Property(s => s.Id)                
                .ValueGeneratedNever()
                .IsRequired();

            menuStatusConfiguration.Property(s => s.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
