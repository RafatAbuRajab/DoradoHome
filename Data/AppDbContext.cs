using DoradoHome.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DoradoHome.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> option) : base(option)
        {
            
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<PropertyAmenity> PropertyAmenities { get; set; }
        public DbSet<SavedProperty> SavedProperties { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        public DbSet<Offer> Offers { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SavedProperty>()
                .HasKey(x => new { x.ClientId, x.PropertyId });

            modelBuilder.Entity<PropertyAmenity>()
                .HasKey(pa => new { pa.PropertyId, pa.AmenityId });
        }
    }
}
