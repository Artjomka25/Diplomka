using Microsoft.EntityFrameworkCore;
using System;

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
            //Random random = new Random();
            ////Добавление машин
            //string status = "Активен", carName;
            //string[] carsName = new string[10]{ "МАЗ-6501А8","ISUZU GIGA 6х4 Евро-5","Тонар-857971","КрАЗ-6230С4-330","КамАЗ-45144",
            //    "КамАЗ-689011","Тонар-9523","КАМАЗ-45143","КАМАЗ-45144","КАМАЗ-65115"};
            //string alphabet = "АВЕКМНОРСТУХ";
            //int num; string plateNumber;
            //for (int i = 1; i < 71; i++)
            //{                
            //    num = random.Next(0, 12);
            //    plateNumber = alphabet[num].ToString();
            //    num = random.Next(0, 10);
            //    plateNumber += num;
            //    num = random.Next(0, 10);
            //    plateNumber += num;
            //    num = random.Next(1, 10);
            //    plateNumber += num;
            //    num = random.Next(0, 12);
            //    plateNumber = alphabet[num].ToString();
            //    num = random.Next(0, 12);
            //    plateNumber = alphabet[num].ToString() + "716";
            //    num = random.Next(0, 10);
            //    carName = carsName[num];
            //    Car car = new Car { CarID = i, Name = carName, PlateNumber = plateNumber, Status = status };
            //    modelBuilder.Entity<Car>().HasData(car);
            //}
            ////=================================================
            ////Добавление данных о водителях
            //string[] surname = new string[10]{ "Иванов","Смирнов","Кузнецов","Попов","Васильев",
            //    "Петров","Соколов","Михайлов","Новиков","Федоров"};

            //string[] name = new string[10]{ "Петр","Михаил","Владимир","Константин","Евгений",
            //    "Александр","Алексей","Вадим","Георгий","Дмитрий"};

            //string[] patronymic = new string[10]{ "Иванович","Сергеевич","Петрович","Юрьевич","Викторович",
            //    "Геннадьевич","Владимирович","Александрович","Алексеевич","Дмитриевич"};
            //int num1, num2, num3;
            //string number = "";
            //string FIO = "";
            //for (int i = 1; i < 71; i++)
            //{
            //    number = "+7";
            //    for (int j = 1; j < 10; j++)
            //    {
            //        number += random.Next(1, 10);
            //    }
            //    num1 = random.Next(0, 10);
            //    num2 = random.Next(0, 10);
            //    num3 = random.Next(0, 10);
            //    FIO = surname[num1] + " " + name[num2] + " " + patronymic[num3];
            //    Driver driver = new Driver { DriverID = i, FIO = FIO, PhoneNumber = number, CarID = i};
            //    modelBuilder.Entity<Driver>().HasData(driver);
            //}
            //==============================================
            //Добавление ролей и юзера
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
