﻿using Axerrio.BuildingBlocks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate;
using Axerrio.DDD.Menu.Infrastructure.EntityTypeConfigurations;
using MediatR;
using EnsureThat;
using Microsoft.EntityFrameworkCore.Design;
using MenuAggr = Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate;

namespace Axerrio.DDD.Menu.Infrastructure
{
    public class MenuContext : DbContextUnitOfWork //: DbContext, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "dbo";

        public DbSet<MenuStatus> MenuStatus { get; set; }
        public DbSet<MenuAggr.Menu> Menu { get; set; }      
        public MenuContext(DbContextOptions<MenuContext> options) : base (options) { }

        public MenuContext(DbContextOptions<MenuContext> options, IMediator mediator): base(options, mediator)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasSequence<int>("MenuId", DEFAULT_SCHEMA)
                        .IncrementsBy(5);

            modelBuilder.ApplyConfiguration(new MenuStatusEntityTypeConfiguration(DEFAULT_SCHEMA));
            modelBuilder.ApplyConfiguration(new MenuEntityTypeConfiguration(DEFAULT_SCHEMA));            
        }        
    }

    
}
