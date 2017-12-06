using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Axerrio.BuildingBlocks
{
    public class ClientRequestContextFactory : IDesignTimeDbContextFactory<ClientRequestContext>
    {
        public ClientRequestContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ClientRequestContext>()
                .UseSqlServer("Server=(localdb)\\ProjectsV12;Initial Catalog=DataStore;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

            return new ClientRequestContext(optionsBuilder.Options);
        }
    }
}