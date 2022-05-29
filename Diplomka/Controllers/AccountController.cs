using Diplomka.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Diplomka.Controllers
{
    public class AccountController : Controller
    {
        MyBaseContext db;

        public AccountController(MyBaseContext context)
        {
            db = context;
        }

        //=======================================================

        [HttpGet]
        public IActionResult Login()
        {
            if (User.IsInRole(RoleEnum.Администратор.ToString()))
            {
                return RedirectToAction("Applications", "Application");
            }
            if (User.IsInRole(RoleEnum.Планировщик.ToString()))
            {
                return RedirectToAction("Applications", "Application");
            }
            if (User.IsInRole(RoleEnum.Заказчик.ToString()))
            {
                return RedirectToAction("Orders", "Order");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.UserName == model.UserName && u.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(user); // аутентификация
                    if (User.IsInRole(RoleEnum.Администратор.ToString()))
                    {
                        return RedirectToAction("Applications", "Application");
                    }
                    if (User.IsInRole(RoleEnum.Планировщик.ToString()))
                    {
                        return RedirectToAction("Applications", "Application");
                    }
                    if (User.IsInRole(RoleEnum.Заказчик.ToString()))
                    {
                        return RedirectToAction("Orders", "Order");
                    }
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        } 
        private async Task Authenticate(User user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name.ToString())
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        //=====================================================
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
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