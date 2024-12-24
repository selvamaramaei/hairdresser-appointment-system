using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProje.Context;

namespace WebProje.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult AdminDashboard()
        {
            // Session'dan rolü al
            var role = HttpContext.Session.GetString("Role");

            // Eğer kullanıcı admin değilse, erişim reddedilir
            if (string.IsNullOrEmpty(role) || role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
        public async Task<IActionResult> Randevular()
        {
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(role) || role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var randevular = await _context.Randevular
                .Include(r => r.User)
                .Include(r => r.Personel)
                .Include(r => r.Islem)
                .ToListAsync();

            return View(randevular);
        }

        // Randevu Onaylama
        [HttpPost]
        public async Task<IActionResult> Onayla(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu == null)
            {
                return NotFound();
            }

            randevu.OnayliMi = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("Randevular");
        }

        // Randevu Reddetme
        [HttpPost]
        public async Task<IActionResult> Reddet(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu == null)
            {
                return NotFound();
            }

            // _context.Randevular.Remove(randevu); 
            randevu.OnayliMi = false;
            await _context.SaveChangesAsync();

            return RedirectToAction("Randevular");
        }

    }
}
