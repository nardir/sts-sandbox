using Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnsureThat;

namespace Axerrio.DDD.Menu.Infrastructure.EntityTypeConfigurations
{
    public class MenuStatusEntityTypeConfiguration : IEntityTypeConfiguration<MenuStatus>
    {
        private readonly string _schema;

        public MenuStatusEntityTypeConfiguration(string schema = "dbo")
        {
            _schema = EnsureArg.IsNotNullOrWhiteSpace(schema, nameof(schema));
        }
       
        public void Configure(EntityTypeBuilder<MenuStatus> menuStatusConfiguration)
        {
            menuStatusConfiguration.ToTable("MenuStatus", _schema);

            menuStatusConfiguration.HasKey(s => s.Id);

            menuStatusConfiguration.Property(s => s.Id) 
                .HasColumnName("MenuStatusId")
                .ValueGeneratedNever()
                .IsRequired();
            
            menuStatusConfiguration.Property(s => s.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
