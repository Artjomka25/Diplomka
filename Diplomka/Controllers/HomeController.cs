using Diplomka.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Diplomka.Controllers
{
    public class HomeController : Controller
    {
        MyBaseContext db;

        public HomeController(MyBaseContext context)
        {
            db = context;
        }

        public ActionResult Index()
        {           
            return View();
        }

        public ActionResult Users()
        {
            IQueryable<User> users = db.Users.Include(u => u.Role);
            return View(users.ToList());
        }

        [Authorize(Roles = "Администратор")]
        public IActionResult Create()
        {
            ViewBag.Role = new SelectList(db.Roles.ToList(), "RoleId", "Name");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Create(User user)
        {      
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Users");
        }

        //=======================================================
        [Authorize(Roles = "Администратор")]
        public ActionResult BackToHome()
        {
            // Переход на главную страницу приложения
            return RedirectToAction("Users");
        }

        //=======================================================

        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.Role = new SelectList(db.Roles.ToList(), "RoleId", "Name");

            if (id != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(c => c.Id == id);
                if (user != null)
                    return View(user);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Edit(User user)
        {
            db.Users.Update(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Users");
        }

        //=======================================================

        [HttpGet]
        [Authorize(Roles = "Администратор")]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(d => d.Id == id);
                if (user != null)
                    return View(user);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(d => d.Id == id);
                if (user != null)
                {
                    db.Users.Remove(user);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Users");
                }
            }
            return NotFound();
        }
        //=======================================================

        [HttpGet]
        [Authorize(Roles = "Администратор, Планировщик")]
        public ActionResult Applications(string factory, string grain)
        {
            IQueryable<Application> applications = db.Applications.Include(p => p.Warehouse)
                                                                 .Include(p => p.Order)
                                                                 .Include(p => p.Depot)
                                                                 .Include(p => p.Car)
                                                                 .Include(p => p.Driver);
            // Фильтрация по заказчику
            if (!String.IsNullOrEmpty(factory) && !factory.Equals("Все"))
            {
                applications = applications.Where(p => p.Order.Factory.Name == factory);
            }
            if (!String.IsNullOrEmpty(grain) && !grain.Equals("Все"))
            {
                applications = applications.Where(p => p.Order.Grain.Name == grain);
            }

            List<Factory> factories = db.Factories.ToList();
            // устанавливаем начальный элемент, который позволит выбрать всех
            factories.Insert(0, new Factory { Name = "Все", FactoryID = 0 });

            List<Grain> grains = db.Grains.ToList();
            // устанавливаем начальный элемент, который позволит выбрать всех
            grains.Insert(0, new Grain { Name = "Все", GrainID = 0 });

            CommonList commonList = new CommonList
            {
                Applications = applications.ToList(),
                Factories = new SelectList(factories, "FactoryID", "Name"),
                Grains = new SelectList(grains, "GrainID", "Name")

            };
            return View(commonList);            
        }
                
        //=======================================================

        public IActionResult CreatePlan()
        {
            List<Application> delete = db.Applications.ToList();
            foreach (Application a in delete)
            {
                db.Applications.Remove(a);
            }
            foreach (Order o in db.Orders.Where(s => s.Status == "Выполняется"))
            {
                o.Status = "Выполнен";
            }
            foreach (Car c in db.Cars.Where(s => s.Status == "Занят"))
            {
                c.Status = "Свободен";
            }

            List<Factory> factory = new List<Factory>();
            List<Application> PreApplication = new List<Application>();
            List<Application> CurrentApplication = new List<Application>();
            List<CargoRemnant> PreRemnants = new List<CargoRemnant>();
            List<Car> PreCars = new List<Car>();
            List<Car> CurrentCars = new List<Car>();
            List<CargoRemnant> CurrentRemnants = new List<CargoRemnant>();
            int Sum = 0;
            int[] count = new int[3]; //потом сделать =8
            int minSum = 0;
            int minWay = 0;
            int Way = 0;

            factory = db.Factories.ToList();
            for (count[0] = 1; count[0] <= db.Factories.Count(); count[0]++)
            {
                for (count[1] = 1; count[1] <= db.Factories.Count(); count[1]++)
                {
                    if (count[1] != count[0])
                    {
                        for (count[2] = 1; count[2] <= db.Factories.Count(); count[2]++)
                        {
                            if ((count[2] != count[1]) & (count[2] != count[0]))
                            {
                                CurrentApplication.Clear();
                                CurrentCars = db.Cars.ToList();
                                CurrentRemnants = db.CargoRemnants.ToList();
                                Way = 0;
                                Sum = 0;
                                foreach (int i in count)
                                {
                                    if (Sum > minSum)
                                    {
                                        continue;
                                    }
                                    foreach (Order order in db.Orders.Where(o => o.FactoryID == i))
                                    {
                                        if (Sum > minSum)
                                        {
                                            continue;
                                        }
                                        Application application = new Application();
                                        List<DistanceReference> distance = new List<DistanceReference>();
                                        List<DistanceReference> distancefromDepot = new List<DistanceReference>();

                                        foreach (CargoRemnant Remnants in PreRemnants.Where(p => p.GrainID == order.GrainID).ToList())
                                        {
                                            if (Remnants.Volume >= order.Volume)
                                            {
                                                distance.Add(db.DistanceReferences.Where(p => p.TypeFirstPoint.Equals("Склад"))
                                                .Where(p => p.TypeSecondPoint.Equals("Завод"))
                                                .Where(p => p.ID_SecondPoint == order.FactoryID)
                                                .FirstOrDefault(p => p.ID_FirstPoint == Remnants.WarehouseID));
                                            }

                                        }
                                        if (distance.Count != 0)
                                        {
                                            application.WarehouseID = distance.OrderBy(p => p.Distance).FirstOrDefault(p => !String.IsNullOrEmpty(p.ID_FirstPoint.ToString())).ID_FirstPoint;
                                            Way += distance.OrderBy(p => p.Distance).FirstOrDefault(p => !String.IsNullOrEmpty(p.ID_FirstPoint.ToString())).Distance;
                                            foreach (Depot depot in db.Depots)
                                            {
                                                if ((CurrentCars.Where(i => i.DepotID == depot.DepotID).Where(c => c.Status == "Свободен").Count()) * 5 >=
                                                order.Volume)
                                                {
                                                    distancefromDepot.Add(db.DistanceReferences.Where(p => p.TypeFirstPoint.Equals("Автобаза"))
                                                    .Where(p => p.TypeSecondPoint.Equals("Склад"))
                                                    .Where(p => p.ID_SecondPoint == application.WarehouseID)
                                                    .FirstOrDefault(p => p.ID_FirstPoint == depot.DepotID));
                                                }
                                            }
                                            if (distancefromDepot.Count != 0)
                                            {
                                                application.DepotID = distancefromDepot.OrderBy(p => p.Distance).FirstOrDefault(p => !String.IsNullOrEmpty(p.ID_FirstPoint.ToString())).ID_FirstPoint;
                                                int CarsToNeed;
                                                if (order.Volume % 5 == 0)
                                                {
                                                    CarsToNeed = order.Volume / 5;
                                                }
                                                else
                                                {
                                                    CarsToNeed = order.Volume / 5 + 1;
                                                }
                                                for (int j = 0; j < CarsToNeed - 1; j++)
                                                {
                                                    application.CarID = CurrentCars.Where(i => i.DepotID == application.DepotID).FirstOrDefault(s => s.Status == "Свободен").CarID;
                                                    application.DriverID = db.Drivers.FirstOrDefault(c => c.CarID == application.CarID).DriverID;
                                                    application.OrderID = order.OrderID;
                                                    application.FullDistance = distance.Where(p => p.TypeFirstPoint.Equals("Автобаза"))
                                                    .Where(p => p.TypeSecondPoint.Equals("Склад"))
                                                    .Where(p => p.ID_SecondPoint == application.WarehouseID)
                                                    .FirstOrDefault(p => p.ID_FirstPoint == application.DepotID).Distance
                                                    +
                                                    distance.Where(p => p.TypeFirstPoint.Equals("Склад"))
                                                    .Where(p => p.TypeSecondPoint.Equals("Завод"))
                                                    .Where(p => p.ID_SecondPoint == order.FactoryID)
                                                    .FirstOrDefault(p => p.ID_FirstPoint == application.WarehouseID).Distance
                                                    +
                                                    distance.Where(p => p.TypeFirstPoint.Equals("Завод"))
                                                    .Where(p => p.TypeSecondPoint.Equals("Автобаза"))
                                                    .Where(p => p.ID_SecondPoint == application.DepotID)
                                                    .FirstOrDefault(p => p.ID_FirstPoint == order.FactoryID).Distance;
                                                    application.FullPrice = (order.Price / CarsToNeed) + (application.FullDistance * 15);
                                                    CurrentApplication.Add(application);
                                                    CurrentCars.Where(i => i.DepotID == application.DepotID).FirstOrDefault(s => s.Status == "Свободен").Status = "Занят";
                                                    if ((order.Volume % 5 != 0) & (j * 5 - order.Volume < 5))
                                                    {
                                                        CurrentRemnants.Where(w => w.WarehouseID == application.WarehouseID).FirstOrDefault(g => g.GrainID == order.GrainID).Volume -= (j * 5 - order.Volume);
                                                    }
                                                    else
                                                    {
                                                        CurrentRemnants.Where(w => w.WarehouseID == application.WarehouseID).FirstOrDefault(g => g.GrainID == order.GrainID).Volume -= 5;
                                                    }
                                                    Way += application.FullDistance;
                                                    Sum += Way * 15;
                                                }

                                            }
                                            else
                                            {
                                                distancefromDepot.Clear();
                                                int CarsToNeed;
                                                distancefromDepot = (db.DistanceReferences.Where(p => p.TypeFirstPoint.Equals("Автобаза"))
                                                .Where(p => p.TypeSecondPoint.Equals("Склад"))
                                                .Where(p => p.ID_SecondPoint == application.WarehouseID)).ToList();
                                                if (order.Volume % 5 == 0)
                                                    CarsToNeed = order.Volume / 5;
                                                else
                                                    CarsToNeed = order.Volume / 5 + 1;
                                                for (int j = 0; j < CarsToNeed - 1; j++)
                                                {
                                                    application.CarID = null;


                                                    foreach (DistanceReference distancefd in distancefromDepot.OrderBy(p => p.Distance))
                                                    {
                                                        application.DepotID = distancefromDepot.OrderBy(p => p.Distance).FirstOrDefault(p => p.ID_FirstPoint == distancefd.ID_FirstPoint).ID_FirstPoint;
                                                        application.CarID = CurrentCars.Where(i => i.DepotID == application.DepotID).FirstOrDefault(s => s.Status == "Свободен").CarID;
                                                        if (application.CarID != null)
                                                            break;

                                                    }
                                                    if (application.CarID == null)
                                                    {
                                                        return RedirectToAction("Index");
                                                    }

                                                    application.DriverID = db.Drivers.FirstOrDefault(c => c.CarID == application.CarID).DriverID;
                                                    application.OrderID = order.OrderID;
                                                    application.FullDistance = distance.Where(p => p.TypeFirstPoint.Equals("Автобаза"))
                                                    .Where(p => p.TypeSecondPoint.Equals("Склад"))
                                                    .Where(p => p.ID_SecondPoint == application.WarehouseID)
                                                    .FirstOrDefault(p => p.ID_FirstPoint == application.DepotID).Distance
                                                    +
                                                    distance.Where(p => p.TypeFirstPoint.Equals("Склад"))
                                                    .Where(p => p.TypeSecondPoint.Equals("Завод"))
                                                    .Where(p => p.ID_SecondPoint == order.FactoryID)
                                                    .FirstOrDefault(p => p.ID_FirstPoint == application.WarehouseID).Distance
                                                    +
                                                    distance.Where(p => p.TypeFirstPoint.Equals("Завод"))
                                                    .Where(p => p.TypeSecondPoint.Equals("Автобаза"))
                                                    .Where(p => p.ID_SecondPoint == application.DepotID)
                                                    .FirstOrDefault(p => p.ID_FirstPoint == order.FactoryID).Distance;
                                                    application.FullPrice = (order.Price /
                                                    CarsToNeed) + (application.FullDistance * 15);
                                                    CurrentApplication.Add(application);
                                                    CurrentCars.Where(i => i.DepotID == application.DepotID).FirstOrDefault(s => s.Status == "Свободен").Status = "Занят";
                                                    if ((order.Volume % 5 != 0) & (j * 5 - order.Volume < 5))
                                                    {
                                                        CurrentRemnants.Where(w => w.WarehouseID == application.WarehouseID).FirstOrDefault(g => g.GrainID == order.GrainID).Volume -= (j * 5 - order.Volume);
                                                    }
                                                    else
                                                    {
                                                        CurrentRemnants.Where(w => w.WarehouseID == application.WarehouseID).FirstOrDefault(g => g.GrainID == order.GrainID).Volume -= 5;
                                                    }
                                                    CurrentRemnants.Where(w => w.WarehouseID == application.WarehouseID).FirstOrDefault(g => g.GrainID == order.GrainID).Volume -= 5;
                                                    Way += application.FullDistance;
                                                    Sum += Way * 15;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            distance.Clear();
                                            int OrderRemainMass = order.Volume;
                                            foreach (CargoRemnant Remnants in PreRemnants.Where(p => p.GrainID == order.GrainID))
                                            {
                                                if (Remnants.Volume > 0)
                                                {
                                                    distance.Add(db.DistanceReferences.Where(p => p.TypeFirstPoint.Equals("Склад")) //берем остатки
                                                    .Where(p => p.TypeSecondPoint.Equals("Завод"))
                                                    .FirstOrDefault(p => p.ID_SecondPoint == order.FactoryID));
                                                }

                                            }

                                            distance = distance.OrderBy(p => p.Distance).ToList(); //сортируем

                                            foreach (Depot depot in db.Depots)
                                            {
                                                if ((CurrentCars.Where(i => i.DepotID == depot.DepotID).Where(c => c.Status == "Свободен").Count()) * 5 >= order.Volume)
                                                {
                                                    distancefromDepot.Add(db.DistanceReferences.Where(p => p.TypeFirstPoint.Equals("Автобаза"))
                                                    .Where(p => p.TypeSecondPoint.Equals("Склад"))
                                                    .FirstOrDefault(p => p.ID_FirstPoint == depot.DepotID));
                                                }
                                            }
                                            if (distancefromDepot.Count != 0)
                                            {
                                                int Remains = 0;

                                                foreach (DistanceReference distancefw in distance.OrderBy(p => p.Distance))
                                                {
                                                    application.WarehouseID = distance.OrderBy(p => p.Distance).FirstOrDefault(p => p.ID_FirstPoint == distancefw.ID_FirstPoint).ID_FirstPoint;
                                                    Remains = CurrentRemnants.Where(g => g.GrainID == order.GrainID).FirstOrDefault(w => w.WarehouseID == application.WarehouseID).Volume;
                                                    if (Remains > 0)
                                                        break;
                                                }

                                                if (Remains == 0)
                                                {
                                                    return RedirectToAction("Index");
                                                }

                                                application.CarID = null;

                                                foreach (DistanceReference distancefd in distancefromDepot.OrderBy(p => p.Distance))
                                                {
                                                    application.DepotID = distancefromDepot.OrderBy(p => p.Distance).FirstOrDefault(p => p.ID_FirstPoint == distancefd.ID_FirstPoint).ID_FirstPoint;
                                                    application.CarID = CurrentCars.Where(i => i.DepotID == application.DepotID).FirstOrDefault(s => s.Status == "Свободен").CarID;
                                                    if (application.CarID != null)
                                                        break;
                                                }
                                                if (application.CarID == null)
                                                {
                                                    return RedirectToAction("Index");
                                                }

                                                int RemainInOrder = order.Volume;

                                                while (RemainInOrder > 0)
                                                {
                                                    application.CarID = CurrentCars.Where(i => i.DepotID == application.DepotID).FirstOrDefault(s => s.Status == "Свободен").CarID;
                                                    application.DriverID = db.Drivers.FirstOrDefault(c => c.CarID == application.CarID).DriverID;
                                                    application.OrderID = order.OrderID;
                                                    application.FullDistance = distance.Where(p => p.TypeFirstPoint.Equals("Автобаза"))
                                                    .Where(p => p.TypeSecondPoint.Equals("Склад"))
                                                    .Where(p => p.ID_SecondPoint == application.WarehouseID)
                                                    .FirstOrDefault(p => p.ID_FirstPoint == application.DepotID).Distance
                                                    +
                                                    distance.Where(p => p.TypeFirstPoint.Equals("Склад"))
                                                    .Where(p => p.TypeSecondPoint.Equals("Завод"))
                                                    .Where(p => p.ID_SecondPoint == order.FactoryID)
                                                    .FirstOrDefault(p => p.ID_FirstPoint == application.WarehouseID).Distance
                                                    +
                                                    distance.Where(p => p.TypeFirstPoint.Equals("Завод"))
                                                    .Where(p => p.TypeSecondPoint.Equals("Автобаза"))
                                                    .Where(p => p.ID_SecondPoint == application.DepotID)
                                                    .FirstOrDefault(p => p.ID_FirstPoint == order.FactoryID).Distance;
                                                    if (Remains >= 5)
                                                    {
                                                        application.FullPrice = ((order.Price / order.Volume) * 5) + (application.FullDistance * 15);
                                                    }
                                                    else
                                                    {
                                                        application.FullPrice = ((order.Price / order.Volume) * Remains) + (application.FullDistance * 15);
                                                    }
                                                    CurrentApplication.Add(application);
                                                    CurrentCars.Where(i => i.DepotID == application.DepotID).FirstOrDefault(s => s.Status == "Свободен").Status = "Занят";
                                                    if (Remains < 5)
                                                    {
                                                        CurrentRemnants.Where(w => w.WarehouseID ==
                                                        application.WarehouseID).FirstOrDefault(g => g.GrainID == order.GrainID).Volume -= Remains;
                                                    }
                                                    else
                                                    {
                                                        CurrentRemnants.Where(w => w.WarehouseID == application.WarehouseID).FirstOrDefault(g => g.GrainID == order.GrainID).Volume -= 5;
                                                    }
                                                    Way += application.FullDistance;
                                                    Sum += Way * 15;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if ((count[0] == 0) & (count[1] == 1) & (count[2] == 2))
                            {
                                minSum = Sum;
                                minWay = Way;

                            }
                            else
                            {
                                if (Sum < minSum)
                                {
                                    minSum = Sum;
                                    minWay = Way;
                                    PreApplication = CurrentApplication;
                                    PreCars = CurrentCars;
                                    PreRemnants = CurrentRemnants;
                                }
                            }

                            //for (count[3] = 1; count[3] < db.Factories.Count(); count[3]++)
                            //{
                            // if ((count[3] != count[2]) & (count[3] != count[1]) & (count[3] != count[0]))
                            // for (count[4] = 1; count[4] < db.Factories.Count(); count[4]++)
                            // {
                            // if ((count[4] != count[3]) & (count[4] != count[2]) & (count[4] != count[1]) & (count[4] != count[0]))

                            // for (count[5] = 1; count[5] < db.Factories.Count(); count[5]++)
                            // {
                            // if ((count[5] != count[4]) & (count[5] != count[3]) & (count[5] != count[2]) & (count[5] != count[1]) & (count[5] != count[0]))
                            // for (count[6] = 1; count[6] < db.Factories.Count(); count[6]++)
                            // {
                            // if ((count[6] != count[5]) & (count[6] != count[4]) & (count[6] != count[3]) & (count[6] != count[2]) & (count[6] != count[1]) & (count[6] != count[0]))

                            // for (count[7] = 1; count[7] < db.Factories.Count(); count[7]++)
                            // {
                            // if ((count[7] != count[6]) & (count[7] != count[5]) & (count[7] != count[4]) & (count[7] != count[3]) & (count[7] != count[2]) & (count[7] != count[1]) & (count[7] != count[0]))
                            // {


                            // }
                            // }

                            // }
                            // }
                            // }
                            //}
                        }
                    }
                        
                }
            }
            foreach (Application application1 in PreApplication)
            {
                db.Applications.Add(application1);
            }
            foreach (CargoRemnant crg in db.CargoRemnants)
            {
                if (PreRemnants.Where(p => p.CargoRemnantID == crg.CargoRemnantID).FirstOrDefault(g => g.GrainID == crg.GrainID) != null)
                    crg.Volume = PreRemnants.Where(p => p.CargoRemnantID == crg.CargoRemnantID).FirstOrDefault(g => g.GrainID == crg.GrainID).Volume;

            }
            foreach (Order o in db.Orders.Where(s => s.Status == "Активен"))
            {
                o.Status = "Выполняется";
            }
            foreach (Car car in db.Cars)
            {
                if (PreCars.FirstOrDefault(c => c.CarID == car.CarID) != null)
                    car.Status = PreCars.FirstOrDefault(c => c.CarID == car.CarID).Status;
            }
            db.SaveChanges();

            // Переход на страницу приложения с заявками
            return RedirectToAction("Applications");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}