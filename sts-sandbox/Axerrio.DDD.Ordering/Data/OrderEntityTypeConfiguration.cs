using Axerrio.DDD.Ordering.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Ordering.Data
{
    class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> orderConfiguration)
        {
            orderConfiguration.ToTable("Orders");

            orderConfiguration.Ignore(o => o.DomainEvents);

            orderConfiguration.Property(o => o.Identity)
                .HasColumnName("OrderId")
                .IsRequired();

            orderConfiguration.HasKey(o => o.Identity);

            orderConfiguration.Property<int>("OrderStatusId")
                .IsRequired();

            orderConfiguration.HasOne(o => o.OrderStatus)
                .WithMany()
                .HasForeignKey("OrderStatusId")
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
