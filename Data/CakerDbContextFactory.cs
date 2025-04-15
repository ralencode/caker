using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Caker.Data
{
    public class CakerDbContextFactory : IDesignTimeDbContextFactory<CakerDbContext>
    {
        public CakerDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<CakerDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new CakerDbContext(optionsBuilder.Options);
        }
    }
}