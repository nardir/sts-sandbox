using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate;
using MenuAggr = Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate;
using EnsureThat;

namespace Axerrio.DDD.Menu.Infrastructure.EntityTypeConfigurations
{
    public class MenuEntityTypeConfiguration : IEntityTypeConfiguration<MenuAggr.Menu>
    {
        private readonly string _schema;

        public MenuEntityTypeConfiguration(string schema = "dbo")
        {
            _schema = EnsureArg.IsNotNullOrWhiteSpace(schema, nameof(schema));
        }

        public void Configure(EntityTypeBuilder<MenuAggr.Menu> menuConfiguration)
        {
            menuConfiguration.ToTable("Menus", _schema);

            menuConfiguration.Ignore(m => m.DomainEvents);

            menuConfiguration.Property(m  => m.Identity)
                .HasColumnName("MenuId")
                .ForSqlServerUseSequenceHiLo("MenuId", _schema) 
                .IsRequired();

            menuConfiguration.HasKey(m => m.Identity);                       

            menuConfiguration.Property<int>("MenuStatusId")
                .IsRequired();

            menuConfiguration.Property<string>("Description")
                .HasMaxLength(255)
                .IsRequired();

            menuConfiguration.OwnsOne(m => m.RequestInfo)
                .ToTable("RequestInfo");     //

            menuConfiguration.HasOne(m => m.ArtistPickedUp)
                .WithMany()
                .HasForeignKey("ArtistId")
                .OnDelete(DeleteBehavior.Restrict);

            menuConfiguration.HasOne(m => m.MenuStatus)
                .WithMany()
                .HasForeignKey("MenuStatusId")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
    
}
