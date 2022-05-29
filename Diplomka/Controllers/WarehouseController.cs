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
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Diplomka.Controllers
{
    public class WarehouseController : Controller
    {
        MyBaseContext db;

        public WarehouseController(MyBaseContext context)
        {
            db = context;
        }
        public ActionResult Warehouses()
        {
            IQueryable<Warehouse> warehouses = db.Warehouses;
            return View(warehouses.ToList());
        }

        //=======================================================

        public ActionResult CargoRemnants()
        {
            IQueryable<CargoRemnant> cargoRemnants = db.CargoRemnants.Include(c => c.Warehouse)
                                                                 .Include(c => c.Grain);
            return View(cargoRemnants.ToList());
        }

        //=======================================================

        public IActionResult CreateCargoRemnant()
        {
            ViewBag.Warehouse = new SelectList(db.Warehouses.ToList(), "WarehouseID", "Name");
            ViewBag.Grain = new SelectList(db.Grains.ToList(), "GrainID", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCargoRemnant(CargoRemnant cargoRemnant)
        {
            db.CargoRemnants.Add(cargoRemnant);
            await db.SaveChangesAsync();
            return RedirectToAction("CargoRemnants");
        }

        //=======================================================

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Warehouse warehouse)
        {
            db.Warehouses.Add(warehouse);
            await db.SaveChangesAsync();
            return RedirectToAction("AddWarehouseWays");
        }
        public async Task<IActionResult> AddWarehouseWays()
        {
            Random random = new Random();
            Warehouse warehouse = new Warehouse();
            warehouse = db.Warehouses.OrderByDescending(p => p.WarehouseID).First(p => !String.IsNullOrEmpty(p.WarehouseID.ToString()));
            foreach (Factory i in db.Factories)
            {
                DistanceReference distanceReference = new DistanceReference();
                distanceReference.ID_FirstPoint = warehouse.WarehouseID;
                distanceReference.NameFirstPoint = warehouse.Name;
                distanceReference.TypeFirstPoint = "Склад";
                distanceReference.ID_SecondPoint = i.FactoryID;
                distanceReference.NameSecondPoint = i.Name;
                distanceReference.TypeSecondPoint = "Завод";
                distanceReference.Distance = random.Next(15, 200);
                db.DistanceReferences.Add(distanceReference);
            }
            foreach (Depot i in db.Depots)
            {
                DistanceReference distanceReference = new DistanceReference();
                distanceReference.ID_FirstPoint = i.DepotID;
                distanceReference.NameFirstPoint = i.Name;
                distanceReference.TypeFirstPoint = "Автобаза";
                distanceReference.ID_SecondPoint = warehouse.WarehouseID;
                distanceReference.NameSecondPoint = warehouse.Name;
                distanceReference.TypeSecondPoint = "Склад";
                distanceReference.Distance = random.Next(15, 200);
                db.DistanceReferences.Add(distanceReference);
            }
            await db.SaveChangesAsync();
            return RedirectToAction("Warehouses");
        }

            //=======================================================
            public ActionResult BackToHome()
        {
            // Переход на главную страницу приложения
            return RedirectToAction("Warehouses");
        }

        //=======================================================
        public ActionResult BackToCargoRemnants()
        {
            // Переход на главную страницу приложения
            return RedirectToAction("CargoRemnants");
        }

        //=======================================================

        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                Warehouse warehouses = await db.Warehouses.FirstOrDefaultAsync(w => w.WarehouseID == id);
                if (warehouses != null)
                    return View(warehouses);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Warehouse warehouse)
        {
            db.Warehouses.Update(warehouse);
            await db.SaveChangesAsync();
            return RedirectToAction("Warehouses");
        }

        //=======================================================

        public async Task<IActionResult> EditCargoRemnant(int? id)
        {
            ViewBag.Warehouse = new SelectList(db.Warehouses.ToList(), "WarehouseID", "Name");
            ViewBag.Grain = new SelectList(db.Grains.ToList(), "GrainID", "Name");

            if (id != null)
            {
                CargoRemnant cargoRemnant = await db.CargoRemnants.FirstOrDefaultAsync(c => c.CargoRemnantID == id);
                if (cargoRemnant != null)
                    return View(cargoRemnant);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditCargoRemnant(CargoRemnant cargoRemnant)
        {
            db.CargoRemnants.Update(cargoRemnant);
            await db.SaveChangesAsync();
            return RedirectToAction("CargoRemnants");
        }

        //=======================================================

        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                Warehouse warehouse = await db.Warehouses.FirstOrDefaultAsync(w => w.WarehouseID == id);
                if (warehouse != null)
                    return View(warehouse);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                Warehouse warehouse = await db.Warehouses.FirstOrDefaultAsync(p => p.WarehouseID == id);
                if (warehouse != null)
                {
                    db.Warehouses.Remove(warehouse);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Warehouses");
                }
            }
            return NotFound();
        }

        //=======================================================

        [HttpGet]
        [ActionName("DeleteCargoRemnant")]
        public async Task<IActionResult> ConfirmDeleteCargoRemnant(int? id)
        {
            if (id != null)
            {
                CargoRemnant сargoRemnant = await db.CargoRemnants.FirstOrDefaultAsync(с => с.CargoRemnantID == id);
                if (сargoRemnant != null)
                    return View(сargoRemnant);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCargoRemnant(int? id)
        {
            if (id != null)
            {
                CargoRemnant сargoRemnant = await db.CargoRemnants.FirstOrDefaultAsync(c => c.CargoRemnantID == id);
                if (сargoRemnant != null)
                {
                    db.CargoRemnants.Remove(сargoRemnant);
                    await db.SaveChangesAsync();
                    return RedirectToAction("CargoRemnants");
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