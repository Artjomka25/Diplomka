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
    public class DepotController : Controller
    {
        MyBaseContext db;

        public DepotController(MyBaseContext context)
        {
            db = context;
        }
        public ActionResult Depots()
        {
            IQueryable<Depot> depots = db.Depots;
            return View(depots.ToList());
        }

        //=======================================================

        public ActionResult Cars()
        {
            IQueryable<Car> cars = db.Cars.Include(с => с.Depot);
            return View(cars.ToList());
        }

        //=======================================================

        public ActionResult Drivers()
        {
            IQueryable<Driver> drivers = db.Drivers.Include(d => d.Car);
            return View(drivers.ToList());
        }

        //=======================================================

        public IActionResult CreateDepot()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepot(Depot depot)
        {
            db.Depots.Add(depot);
            await db.SaveChangesAsync();
            return RedirectToAction("Depots");
        }

        //=======================================================

        public IActionResult CreateCar()
        {
            ViewBag.Depot = new SelectList(db.Depots.ToList(), "DepotID", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCar(Car car)
        {
            db.Cars.Add(car);
            await db.SaveChangesAsync();
            return RedirectToAction("Cars");
        }

        //=======================================================

        public IActionResult CreateDriver()
        {
            ViewBag.Car = new SelectList(db.Cars.ToList(), "CarID", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDriver(Driver driver)
        {
            db.Drivers.Add(driver);
            await db.SaveChangesAsync();
            return RedirectToAction("Drivers");
        }

        //=======================================================

        public ActionResult BackToDepots()
        {
            // Переход на главную страницу приложения
            return RedirectToAction("Depots");
        }

        //=======================================================

        public ActionResult BackToCars()
        {
            // Переход на главную страницу приложения
            return RedirectToAction("Cars");
        }

        //=======================================================

        public ActionResult BackToDrivers()
        {
            // Переход на главную страницу приложения
            return RedirectToAction("Drivers");
        }

        //=======================================================

        public async Task<IActionResult> EditDepot(int? id)
        {
            if (id != null)
            {
                Depot depot = await db.Depots.FirstOrDefaultAsync(d => d.DepotID == id);
                if (depot != null)
                    return View(depot);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditDepot(Depot depot)
        {
            db.Depots.Update(depot);
            await db.SaveChangesAsync();
            return RedirectToAction("Depots");
        }

        //=======================================================

        public async Task<IActionResult> EditCar(int? id)
        {
            ViewBag.Depot = new SelectList(db.Depots.ToList(), "DepotID", "Name");

            if (id != null)
            {
                Car car = await db.Cars.FirstOrDefaultAsync(c => c.CarID == id);
                if (car != null)
                    return View(car);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditCar(Car car)
        {
            db.Cars.Update(car);
            await db.SaveChangesAsync();
            return RedirectToAction("Cars");
        }

        //=======================================================

        public async Task<IActionResult> EditDriver(int? id)
        {
            ViewBag.Car = new SelectList(db.Cars.ToList(), "CarID", "Name");

            if (id != null)
            {
                Driver driver = await db.Drivers.FirstOrDefaultAsync(d => d.DriverID == id);
                if (driver != null)
                    return View(driver);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditDriver(Driver driver)
        {
            db.Drivers.Update(driver);
            await db.SaveChangesAsync();
            return RedirectToAction("Drivers");
        }

        //=======================================================

        [HttpGet]
        [ActionName("DeleteDepot")]
        public async Task<IActionResult> ConfirmDeleteDepot(int? id)
        {
            if (id != null)
            {
                Depot depot = await db.Depots.FirstOrDefaultAsync(d => d.DepotID == id);
                if (depot != null)
                    return View(depot);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDepot(int? id)
        {
            if (id != null)
            {
                Depot depot = await db.Depots.FirstOrDefaultAsync(d => d.DepotID == id);
                if (depot != null)
                {
                    db.Depots.Remove(depot);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Depots");
                }
            }
            return NotFound();
        }

        //=======================================================

        [HttpGet]
        [ActionName("DeleteCar")]
        public async Task<IActionResult> ConfirmDeleteCar(int? id)
        {
            if (id != null)
            {
                Car car = await db.Cars.FirstOrDefaultAsync(c => c.CarID == id);
                if (car != null)
                    return View(car);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCar(int? id)
        {
            if (id != null)
            {
                Car car = await db.Cars.FirstOrDefaultAsync(c => c.CarID == id);
                if (car != null)
                {
                    db.Cars.Remove(car);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Cars");
                }
            }
            return NotFound();
        }

        //=======================================================

        [HttpGet]
        [ActionName("DeleteDriver")]
        public async Task<IActionResult> ConfirmDeleteDriver(int? id)
        {
            if (id != null)
            {
                Driver driver = await db.Drivers.FirstOrDefaultAsync(d => d.DriverID == id);
                if (driver != null)
                    return View(driver);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDriver(int? id)
        {
            if (id != null)
            {
                Driver driver = await db.Drivers.FirstOrDefaultAsync(d => d.DriverID == id);
                if (driver != null)
                {
                    db.Drivers.Remove(driver);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Drivers");
                }
            }
            return NotFound();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}