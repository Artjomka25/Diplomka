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
    public class FactoryController : Controller
    {
        MyBaseContext db;

        public FactoryController(MyBaseContext context)
        {
            db = context;
        }
        public ActionResult Factories()
        {
            IQueryable<Factory> factories = db.Factories;
            return View(factories.ToList());
        }

        //=======================================================

        public IActionResult Create()
        {            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Factory factory)
        {           
            db.Factories.Add(factory);
            await db.SaveChangesAsync();
            return RedirectToAction("AddFactoriesWays");
        }

        public async Task<IActionResult> AddFactoriesWays()
        {
            Factory factories = new Factory();
            factories = db.Factories.OrderByDescending(p => p.FactoryID).First(p => !String.IsNullOrEmpty(p.FactoryID.ToString()));
            Random random = new Random();
            foreach (Warehouse i in db.Warehouses)
            {
                DistanceReference distanceReference = new DistanceReference();
                distanceReference.ID_FirstPoint = i.WarehouseID;
                distanceReference.NameFirstPoint = i.Name;
                distanceReference.TypeFirstPoint = "Склад";
                distanceReference.ID_SecondPoint = factories.FactoryID;
                distanceReference.NameSecondPoint = factories.Name;
                distanceReference.TypeSecondPoint = "Завод";
                distanceReference.Distance = random.Next(15, 200);
                db.DistanceReferences.Add(distanceReference);
            }
            foreach (Depot i in db.Depots)
            {
                DistanceReference distanceReference = new DistanceReference();
                distanceReference.ID_FirstPoint = factories.FactoryID;
                distanceReference.NameFirstPoint = factories.Name;
                distanceReference.TypeFirstPoint = "Завод";
                distanceReference.ID_SecondPoint = i.DepotID;
                distanceReference.NameSecondPoint = i.Name;
                distanceReference.TypeSecondPoint = "Автобаза";
                distanceReference.Distance = random.Next(15, 200);
                db.DistanceReferences.Add(distanceReference);
            }
            await db.SaveChangesAsync();
            return RedirectToAction("Factories");
        }


        //=======================================================
        public ActionResult BackToHome()
        {
            // Переход на главную страницу приложения
            return RedirectToAction("Factories");
        }

        //=======================================================

        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                Factory factories = await db.Factories.FirstOrDefaultAsync(f => f.FactoryID == id);
                if (factories != null)
                    return View(factories);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Factory factory)
        {
            db.Factories.Update(factory);
            await db.SaveChangesAsync();
            return RedirectToAction("Factories");
        }

        //=======================================================

        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                Factory factory = await db.Factories.FirstOrDefaultAsync(p => p.FactoryID == id);
                if (factory != null)
                    return View(factory);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                Factory factory = await db.Factories.FirstOrDefaultAsync(p => p.FactoryID == id);
                if (factory != null)
                {
                    db.Factories.Remove(factory);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Factories");
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