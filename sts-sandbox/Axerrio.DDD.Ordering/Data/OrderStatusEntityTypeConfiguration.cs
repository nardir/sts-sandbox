using Axerrio.DDD.Ordering.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Ordering.Data
{
    class OrderStatusEntityTypeConfiguration
        : IEntityTypeConfiguration<OrderStatus>
    {
        public void Configure(EntityTypeBuilder<OrderStatus> orderStatusConfiguration)
        {
            orderStatusConfiguration.ToTable("OrderStatus");

            orderStatusConfiguration.HasKey(s => s.Id);

            orderStatusConfiguration.Property(s => s.Id)
                //.HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            orderStatusConfiguration.Property(s => s.Name)
                .HasMaxLength(200)
                .IsRequired();

            //AlternateKey
        }
    }
}
