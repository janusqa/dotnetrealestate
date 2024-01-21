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
        public DbSet<VillaNumber> VillaNumbers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // we need this line when using identity or will get error
            // ERROR: InvalidOperationException: The entity type 'IdentityUserLogin<string>' requires a primary key to be defined. If you intended to use a keyless entity type, call 'HasNoKey' in 'OnModelCreating'. For more information on keyless entity types, see https://go.microsoft.com/fwlink/?linkid=2141943.
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Villa>().Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Villa>().Property(e => e.UpdatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<VillaNumber>().Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<VillaNumber>().Property(e => e.UpdatedDate).HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Villa>().HasData(
                new Villa
                {
                    Id = 1,
                    Name = "Royal Villa",
                    Details = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa3.jpg",
                    Occupancy = 4,
                    Rate = 200,
                    Sqft = 550,
                    Amenity = "",
                },
                new Villa
                {
                    Id = 2,
                    Name = "Premium Pool Villa",
                    Details = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa1.jpg",
                    Occupancy = 4,
                    Rate = 300,
                    Sqft = 550,
                    Amenity = ""
                },
                new Villa
                {
                    Id = 3,
                    Name = "Luxury Pool Villa",
                    Details = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa4.jpg",
                    Occupancy = 4,
                    Rate = 400,
                    Sqft = 750,
                    Amenity = ""
                },
                new Villa
                {
                    Id = 4,
                    Name = "Diamond Villa",
                    Details = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa5.jpg",
                    Occupancy = 4,
                    Rate = 550,
                    Sqft = 900,
                    Amenity = ""
                },
                new Villa
                {
                    Id = 5,
                    Name = "Diamond Pool Villa",
                    Details = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa2.jpg",
                    Occupancy = 4,
                    Rate = 600,
                    Sqft = 1100,
                    Amenity = ""
                }
            );

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableDetailedErrors(); // Enable detailed error messages

        }
    }
}