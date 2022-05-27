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
                    "Выполнен"
                })
            };
            return View(commonList);
        }

        //===========================================
        [HttpPost]
        public async Task<IActionResult> Index(
                                    SortState sortOrder = SortState.DeliveryDateAsc)
        {
            IQueryable<Application> applications = db.Applications.Include(p => p.Warehouse)
                                                                 .Include(p => p.Order)
                                                                 .Include(p => p.Depot)
                                                                 .Include(p => p.Car)
                                                                 .Include(p => p.Driver);
            ViewData["DeliveryDateSort"] = sortOrder == SortState.DeliveryDateAsc ?
                                          SortState.DeliveryDateDesc : SortState.DeliveryDateAsc;
            
            switch (sortOrder)
            {
                case SortState.DeliveryDateAsc:
                    applications = applications.OrderByDescending(s => s.Order.DeliveryDate);
                    break;                
                default:
                    applications = applications.OrderBy(s => s.Order.DeliveryDate);
                    break;
            }
            return View(await applications.AsNoTracking().ToListAsync());
        }

        //=======================================================

        public ActionResult CreatePlan()
        {            

           




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