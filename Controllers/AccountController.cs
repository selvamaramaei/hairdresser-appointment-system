﻿using Microsoft.AspNetCore.Mvc;
using WebProje.Models;
using WebProje.Context;
using System.Linq;

namespace WebProje.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // Giriş sayfası
        public IActionResult Login()
        {
            return View();
        }

        // Giriş işlemi
        [HttpPost]
        public IActionResult Login(string email, string sifre)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Sifre == sifre);

            if (user != null)
            {
                // Admin kontrolü
                if (email == "G221210583@sakarya.edu.tr" && sifre == "sau")
                {
                    HttpContext.Session.SetString("Username", $"{user.Ad} {user.Soyad}");
                    HttpContext.Session.SetString("Role", "Admin");
                    return RedirectToAction("AdminDashboard", "Admin");
                }

                // Kullanıcı kontrolü
                HttpContext.Session.SetString("Username", $"{user.Ad} {user.Soyad}");
                HttpContext.Session.SetString("Role", "User");
                return RedirectToAction("UserDashboard", "User");
            }

            ViewBag.ErrorMessage = "Geçersiz e-posta veya şifre!";
            return View();
        }


        // Kayıt sayfası
        public IActionResult Register()
        {
            return View();
        }

        // Kayıt işlemi
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(user);
        }

        // Çıkış işlemi
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Session'ı temizle
            return RedirectToAction("Login");
        }
    }
}
