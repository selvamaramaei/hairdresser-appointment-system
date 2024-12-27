﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProje.Context;

namespace WebProje.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult UserDashboard()
        {
            // Session'dan rolü al
            var role = HttpContext.Session.GetString("Role");

            // Eğer kullanıcı adminse, erişim reddedilir
            if (string.IsNullOrEmpty(role) || role == "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        public async Task<IActionResult> Randevularim()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            var randevular = await _context.Randevular
                .Include(r => r.Personel)
                .Include(r => r.Islem)
                .Where(r => r.UserId == userId)
                .ToListAsync();

            return View(randevular);
        }



    }
}