﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnsureThat;

namespace Axerrio.DDD.Ordering.BuildingBlocks
{
    class ClientRequestEntityTypeConfiguration : IEntityTypeConfiguration<ClientRequest>
    {
        private readonly string _schema;

        public ClientRequestEntityTypeConfiguration(string schema = "dbo")
        {
            _schema = EnsureArg.IsNotNullOrWhiteSpace(schema, nameof(schema));
        }

        public void Configure(EntityTypeBuilder<ClientRequest> builder)
        {
            builder.ToTable("ClientRequest", _schema);

            builder.Property(cr => cr.Id)
                .HasColumnName("ClientrequestId");

            builder.HasKey(cr => cr.Id);

            builder.Property(cr => cr.Name).IsRequired();
            builder.Property(cr => cr.Time).IsRequired();
        }
    }
}
