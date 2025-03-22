using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Vidsnap.DataBase.Context
{
    /// <summary>
    /// For local use only
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connString = configuration.GetConnectionString("DefaultConnection")
                             ?? throw new InvalidOperationException("Connection string is not set in configuration.");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}