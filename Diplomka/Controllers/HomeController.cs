using Diplomka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Diplomka.Controllers
{
    public class HomeController : Controller
    {
        MyBaseContext db;

        public HomeController(MyBaseContext context)
        {
            db = context;
        }

        [HttpGet]
        public ActionResult Index(string status)
        {
            IQueryable<Application> applications = db.Applications.Include(p => p.Warehouse)
                                                     .Include(p => p.Order)
                                                     .Include(p => p.Depot)
                                                     .Include(p => p.Car)
                                                     .Include(p => p.Driver);
            // Фильтрация по статусу заказа
            if (!String.IsNullOrEmpty(status) && !status.Equals("Все"))
            {
                applications = applications.Where(p => p.Order.Status == status);
            }

            CommonList commonList = new CommonList
            {
                Applications = applications.ToList(),
                Orders = new SelectList(new List<string>()
                {
               "Все",
               "Активен",
               "Выполнен"})
            };
            return View(commonList);
        }

        //===========================================
        [HttpPost]
        public async Task<IActionResult> Index(SortState sortOrder = SortState.DeliveryDateAsc)
        {
            IQueryable<Application> applications = db.Applications.Include(p => p.Warehouse)
                                                                             .Include(p => p.Order)
                                                                             .Include(p => p.Depot)
                                                                             .Include(p => p.Car)
                                                                             .Include(p => p.Driver);

            ViewData["DeliveryDateSort"] = sortOrder == SortState.DeliveryDateAsc ?
                                          SortState.DeliveryDateDesc : SortState.DeliveryDateAsc;
            applications = sortOrder switch
            {
                SortState.DeliveryDateAsc => applications.OrderBy(s => s.FullPrice),
                SortState.DeliveryDateDesc => applications.OrderByDescending(s => s.FullPrice),
                _ => applications.OrderBy(s => s.ApplicationID),
            };            
            return View(await applications.AsNoTracking().ToListAsync());
        }

        //=======================================================

        public ActionResult CreatePlan()
        {
            List<Factory> factory = new List<Factory>();
            factory = db.Factories.ToList();
            List<Application> PreApplication = new List<Application>();
            List<Application> CurrentApplication = new List<Application>();
            List<CargoRemnant> PreRemnants = new List<CargoRemnant>();
            int Sum = 0;
            int[] count = new int[3]; // потом сделать =8
            int minSum;
            int minWay;
            int Way = 0;

            for (count[0] = 0; count[0] < db.Factories.Count() - 1; count[0]++)
            {
                for ( count[1] = 0; count[1] < db.Factories.Count() - 1; count[1]++)
                {
                    if (count[1] != count[0])
                    {
                        for (count[2] = 0; count[2] < db.Factories.Count() - 1; count[2]++)
                        {
                            if ((count[2] != count[1]) & (count[2] != count[0]))
                            {
                                CurrentApplication.Clear();
                                Way = 0;
                                Sum = 0;
                                PreRemnants = db.CargoRemnants.ToList();
                                foreach (int i in count)
                                {
                                    foreach (Order order in db.Orders.Where(o => o.FactoryID == i).Where(o => o.Status=="Активен"))
                                    {
                                        PreRemnants = db.CargoRemnants.ToList();
                                        Application application = new Application();
                                        List<DistanceReference> distance = new List<DistanceReference>();
                                        foreach (CargoRemnant Remnants in PreRemnants.Where(p => p.GrainID == order.GrainID))
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
                                            distance.Clear();
                                            foreach (Depot depot in db.Depots)
                                            {
                                                distance.Add(db.DistanceReferences.Where(p => p.TypeFirstPoint.Equals("Автобаза"))
                                                                                           .Where(p => p.TypeSecondPoint.Equals("Склад"))
                                                                                           .Where(p => p.ID_SecondPoint == application.WarehouseID)
                                                                                           .FirstOrDefault(p => p.ID_FirstPoint == depot.DepotID));
                                            }
                                            application.DepotID = distance.OrderBy(p => p.Distance).FirstOrDefault(p => !String.IsNullOrEmpty(p.ID_FirstPoint.ToString())).ID_FirstPoint;

                                        }
                                        else
                                        {
                                            distance.Clear();
                                            int OrderRemainMass = order.Volume;
                                            foreach (CargoRemnant Remnants in PreRemnants.Where(p => p.GrainID == order.GrainID))
                                            {
                                                if (Remnants.Volume > 0)
                                                {
                                                    distance.Add(db.DistanceReferences.Where(p => p.TypeFirstPoint.Equals("Склад"))
                                                                                           .Where(p => p.TypeSecondPoint.Equals("Завод"))
                                                                                           .Where(p => p.ID_SecondPoint == order.FactoryID)
                                                                                           .FirstOrDefault(p => p.ID_FirstPoint == Remnants.WarehouseID));
                                                    while (OrderRemainMass > 0)
                                                    {
                                                        distance = distance.OrderBy(p => p.Distance).ToList();

                                                        foreach (Depot depot in db.Depots)
                                                        {

                                                        }

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

                                }
                            }
                            //for (count[3] = 0; count[3] < db.Factories.Count() - 1; count[3]++)
                            //{
                            //    if ((count[3] != count[2]) & (count[3] != count[1]) & (count[3] != count[0]))
                            
                            //        for (count[4] = 0; count[4] < db.Factories.Count() - 1; count[4]++)
                            //        {
                            //            if ((count[4] != count[3]) & (count[4] != count[2]) & (count[4] != count[1]) & (count[4] != count[0]))

                            //                for (count[5] = 0; count[5] < db.Factories.Count() - 1; count[5]++)
                            //                {
                            //                    if ((count[5] != count[4]) & (count[5] != count[3]) & (count[5] != count[2]) & (count[5] != count[1]) & (count[5] != count[0]))
                            //                        for (count[6] = 0; count[6] < db.Factories.Count() - 1; count[6]++)
                            //                        {
                            //                            if ((count[6] != count[5]) & (count[6] != count[4]) & (count[6] != count[3]) & (count[6] != count[2]) & (count[6] != count[1]) & (count[6] != count[0]))

                            //                                for (count[7] = 0; count[7] < db.Factories.Count() - 1; count[7]++)
                            //                                {
                            //                                    if ((count[7] != count[6]) & (count[7] != count[5]) & (count[7] != count[4]) & (count[7] != count[3]) & (count[7] != count[2]) & (count[7] != count[1]) & (count[7] != count[0]))
                            //                                    {


                            //                                    }
                            //                                }

                            //                        }
                            //                }
                            //        }
                            //}
                        }
                    }


                }
            }





            // Переход на главную страницу приложения
            return RedirectToAction("Index");
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