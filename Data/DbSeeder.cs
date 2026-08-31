using DoradoHome.Models;
using Microsoft.EntityFrameworkCore;

namespace DoradoHome.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            await db.Database.MigrateAsync();

            await SeedAmenitiesAsync(db);
        }

        private static async Task SeedAmenitiesAsync(AppDbContext db)
        {
            string[] defaultAmenities =
            {
            "Balcony",
            "Parking",
            "Swimming Pool",
            "Garden",
            "Air Conditioning",
            "Elevator",
            "Gym",
            "Pet Friendly"
            };

            var existingAmenities = await db.Amenities
                .Select(a => a.Name)
                .ToListAsync();

            var newAmenities = defaultAmenities
                .Where(name => !existingAmenities.Contains(name))
                .Select(name => new Amenity
                {
                    Name = name
                })
                .ToList();

            if (newAmenities.Count > 0)
            {
                await db.Amenities.AddRangeAsync(newAmenities);
                await db.SaveChangesAsync();
            }
        }
    }
}
