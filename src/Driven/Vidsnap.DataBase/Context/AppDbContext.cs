using Microsoft.EntityFrameworkCore;
using Vidsnap.Domain.Entities;

namespace Vidsnap.DataBase.Context
{
    public class AppDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Video> Videos { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder) =>
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
