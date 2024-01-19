using RealEstate.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace RealEstate.DataAccess.Data
{
    // To configure Identity we changed DBContext to IdentityDbContext<ApplicationUser>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Villa> Villas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // we need this line when using identity or will get error
            // ERROR: InvalidOperationException: The entity type 'IdentityUserLogin<string>' requires a primary key to be defined. If you intended to use a keyless entity type, call 'HasNoKey' in 'OnModelCreating'. For more information on keyless entity types, see https://go.microsoft.com/fwlink/?linkid=2141943.
            base.OnModelCreating(modelBuilder);

            // modelBuilder.Entity<Category>().HasData(
            //     new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
            //     new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
            //     new Category { Id = 3, Name = "History", DisplayOrder = 3 }
            // );

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableDetailedErrors(); // Enable detailed error messages

        }
    }
}