using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate;
using MenuAggr = Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate;

namespace Axerrio.DDD.Menu.Infrastructure.EntityTypeConfigurations
{
    public class MenuEntityTypeConfiguration : IEntityTypeConfiguration<MenuAggr.Menu>
    {
        public void Configure(EntityTypeBuilder<MenuAggr.Menu> menuConfiguration)
        {
            menuConfiguration.ToTable("Menus");

            menuConfiguration.Ignore(o => o.DomainEvents);

            menuConfiguration.Property(o => o.Identity)
                .HasColumnName("MenuId")
                .IsRequired();

            menuConfiguration.HasKey(o => o.Identity);

            menuConfiguration.Property<int>("MenuStatusId")
                .IsRequired();

            menuConfiguration.HasOne(o => o.MenuStatus)
                .WithMany()
                .HasForeignKey("MenuStatusId")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
    
}
