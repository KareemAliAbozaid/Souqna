using Microsoft.EntityFrameworkCore;
using Souqna.Domin.Entities;
using System.Reflection;


namespace Souqna.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {        
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Photo> Photos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
            //modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
            //modelBuilder.Entity<Photo>().HasQueryFilter(p => !p.IsDeleted);
            // Configure your entity mappings here
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
