using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate;
using EnsureThat;
using Axerrio.DDD.Menu.Domain.AggregatesModel.ArtistAggregate;

namespace Axerrio.DDD.Menu.Infrastructure.EntityTypeConfigurations
{
    public class ArtistEntityTypeConfiguration : IEntityTypeConfiguration<Artist>
    {
        private readonly string _schema;

        public ArtistEntityTypeConfiguration(string schema = "dbo")
        {
            _schema = EnsureArg.IsNotNullOrWhiteSpace(schema, nameof(schema));
        }

        public void Configure(EntityTypeBuilder<Artist> artistConfiguration)
        {
            artistConfiguration.ToTable("Artist", _schema);

            artistConfiguration.Ignore(o => o.DomainEvents);

            artistConfiguration.Property(o => o.Identity)
                .HasColumnName("ArtistId")
                .ForSqlServerUseSequenceHiLo("ArtistId", _schema) 
                .IsRequired();

            artistConfiguration.HasKey(o => o.Identity);

            artistConfiguration.Property<string>("FirstName")
                .HasMaxLength(255)
                .IsRequired();

            artistConfiguration.Property<string>("LastName")
                .HasMaxLength(255)
                .IsRequired();

            artistConfiguration.Property<string>("EmailAddress")
                .HasMaxLength(255)
                .IsRequired();

        }
    }
    
}
