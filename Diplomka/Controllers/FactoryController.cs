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