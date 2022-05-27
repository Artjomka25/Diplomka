using Microsoft.EntityFrameworkCore;

namespace Diplomka.Models
{
    public class MyBaseContext : DbContext
    {
        public DbSet<Factory> Factories { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<CargoRemnant> CargoRemnants { get; set; }
        public DbSet<Grain> Grains { get; set; }
        public DbSet<Depot> Depots { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<DistanceReference> DistanceReferences { get; set; }

        public MyBaseContext(DbContextOptions<MyBaseContext> options)
               : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
