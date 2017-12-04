using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using Axerrio.IWebHostsExtensions;
using Axerrio.BuildingBlocks;

namespace Axerrio.DDD.Messaging
{
    public class Program
    {
        //TODO:
        //- Multiple projects for this test case.
        //- Axerrio.DDD.Lib structure/folders 
        //- What to test?

        //Ideas:
        //- Create Default Axerrio API/Microservices Project template(s) based on Building Blocks.
        //- Create Class templates based on Building Blcoks (DomainEvent etc.)


        public static void Main(string[] args)
        {
            BuildWebHost(args)
                .MigrateDbContext<ClientRequestContext>()
                .Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
