using Microsoft.EntityFrameworkCore;

namespace Diplomka.Models
{
    public class MyBaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string adminRoleName = "Администратор";
            string plannerRoleName = "Планировщик";
            string clientRoleName = "Заказчик";

            string adminUserName = "Администратор";
            string adminPassword = "admin";
            
            // добавляем роли
            Role adminRole = new Role { RoleId = 1, Name = adminRoleName };
            Role plannerRole = new Role { RoleId = 2, Name = plannerRoleName };
            Role clientRole = new Role { RoleId = 3, Name = clientRoleName };
            User adminUser = new User { Id = 1, UserName = adminUserName, Password = adminPassword, RoleId = adminRole.RoleId};

            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, plannerRole, clientRole});
            modelBuilder.Entity<User>().HasData(new User[] { adminUser });
            base.OnModelCreating(modelBuilder);
        }
    }
}
