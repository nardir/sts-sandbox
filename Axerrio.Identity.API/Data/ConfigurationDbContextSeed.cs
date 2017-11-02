using AutoMapper.Configuration;
using Axerrio.Identity.API.Configuration;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.Identity.API.Data
{
    public static class ConfigurationDbContextSeed
    {
        public static async Task SeedAsync(ConfigurationDbContext context)
        {
            foreach (var client in Config.GetClients())
            {
                if (!context.Clients.Any(c => c.ClientId == client.ClientId))
                {
                    await context.Clients.AddAsync(client.ToEntity());
                }
            }
            //await context.SaveChangesAsync();

            foreach (var resource in Config.GetIdentityResources())
            {
                if (!context.IdentityResources.Any(r => r.Name == resource.Name))
                {
                    await context.IdentityResources.AddAsync(resource.ToEntity());
                }
            }
            //await context.SaveChangesAsync();
 
            foreach (var api in Config.GetApiResources())
            {
                if (!context.ApiResources.Any(r => r.Name == api.Name))
                {
                    await context.ApiResources.AddAsync(api.ToEntity());
                }
            }

            await context.SaveChangesAsync();
    }
    }
}
