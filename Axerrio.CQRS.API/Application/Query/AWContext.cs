using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.Query
{
    public class AWContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<ProductPicture> ProductPictures { get; set; }

        //public DbQuery<ProductPicture2> ProductPictures2 { get; set; }
        public DbSet<ProductPicture2> ProductPictures2 { get; set; }

        public AWContext(DbContextOptions<AWContext> options)
            : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .ToTable("Product", "SalesLT");

            modelBuilder.Entity<Product>()
                .HasKey(p => p.ProductID);

            modelBuilder.Entity<Picture>()
               .ToTable("Picture", "SalesLT");

            modelBuilder.Entity<Picture>()
                .HasKey(p => p.PictureID);

            modelBuilder.Entity<EntityPicture>()
                .ToTable("EntityPicture", "SalesLT");

            modelBuilder.Entity<EntityPicture>()
                .HasKey(ep => new { ep.EntityID, ep.EntityInstanceID, ep.PictureID });

            modelBuilder.Entity<EntityPicture>()
                .HasDiscriminator(ep => ep.EntityID)
                .HasValue<EntityPicture>(0)
                .HasValue<ProductPicture>(2)
                .HasValue<CustomerPicture>(1);

            modelBuilder.Entity<ProductPicture>()
                .Ignore(pp => pp.ProductID);

            modelBuilder.Entity<ProductPicture2>()
                .ToTable("vProductPicture", "SalesLT");

            modelBuilder.Entity<ProductPicture2>()
                .HasKey(pp => new { pp.ProductID, pp.PictureID });

            modelBuilder.Entity<ProductPicture2>()
                .HasOne(pp => pp.Product)
                .WithMany(p => p.ProductPictures)
                .HasForeignKey(pp => pp.ProductID);

            modelBuilder.Entity<ProductPicture2>()
                .HasOne(pp => pp.Picture)
                .WithMany()
                .HasForeignKey(pp => pp.PictureID);

            //modelBuilder.Query<ProductPicture2>()
            //    .HasOne(pp => pp.Product)
            //    .WithMany(p => p.Pictures)
            //    .HasForeignKey(pp => pp.ProductID);

            //modelBuilder.Query<ProductPicture2>()
            //    .HasOne(pp => pp.Picture)
            //    .WithMany()
            //    .HasForeignKey(pp => pp.PictureID);
        }
    }
}
