using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Axerrio.Identity.API.Data
{
    public class PersistedGrantDbContextFactory : IDesignTimeDbContextFactory<PersistedGrantDbContext>
    {
        public PersistedGrantDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<PersistedGrantDbContext>()
                .UseSqlServer(@"Data Source=(localdb)\ProjectsV13;Initial Catalog=Axerrio_Accounts;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False",
                                sqlOptions =>
                                {
                                    sqlOptions.MigrationsAssembly(typeof(PersistedGrantDbContextFactory).GetTypeInfo().Assembly.GetName().Name);
                                }).Options;

            return new PersistedGrantDbContext(options, new IdentityServer4.EntityFramework.Options.OperationalStoreOptions());
        }
    }
}