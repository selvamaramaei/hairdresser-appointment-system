using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;
using WebProje.Context;
using WebProje.Models;

namespace WebProje.Controllers
{
    public class RandevuController : Controller
    {
        private readonly AppDbContext _context;

        public RandevuController(AppDbContext context)
        {
            _context = context;
        }

        // Adım 1: İşlem Seçimi
        public async Task<IActionResult> IslemSec()
        {
            // Tüm uzmanlıkları ve bunlara bağlı işlemleri getiriyoruz
            var uzmanliklar = await _context.Uzmanliklar
                .Include(u => u.Islemler)
                .ToListAsync();

            return View(uzmanliklar);
        }


        // Adım 2: Personel Seçimi
        public async Task<IActionResult> PersonelSec(int islemId)
        {
            var islem = await _context.Islemler
                .Include(i => i.Uzmanlik)
                    .ThenInclude(u => u.Personeller)
                .FirstOrDefaultAsync(i => i.Id == islemId);

            if (islem == null || islem.Uzmanlik == null)
            {
                return RedirectToAction("IslemSec");

            }

            var personeller = islem.Uzmanlik.Personeller.ToList();

            if (!personeller.Any())
            {
                ViewBag.HataMesaji = "Seçtiğiniz hizmete ait çalışan bulunmamaktadır.";
                ViewBag.Islem = islem; // İşlem bilgisi tekrar gösterilebilir
                return View(new List<Personel>()); // Boş bir liste döndür
            }

            ViewBag.Islem = islem;
            return View(personeller);
        }


        // Adım 3: Tarih ve Saat Seçimi
        public async Task<IActionResult> TarihSaatSec(int islemId, int personelId)
        {
            var personel = await _context.Personeller
                .Include(p => p.Mesailer)
                    .ThenInclude(m => m.CalistigiGunler)
                .FirstOrDefaultAsync(p => p.Id == personelId);

            if (personel == null)
                return RedirectToAction("PersonelSec", new { islemId });

            ViewBag.IslemId = islemId;
            ViewBag.PersonelId = personelId;
            ViewBag.MesaiGunleri = personel.Mesailer
                .SelectMany(m => m.CalistigiGunler.Select(g => g.Gun))
                .Distinct()
                .ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TarihSaatSec(int islemId, int personelId, DateTime randevuTarihi, TimeSpan randevuSaati)
        {
            var islem = await _context.Islemler.FindAsync(islemId);
            var personel = await _context.Personeller
                .Include(p => p.Mesailer)
                    .ThenInclude(m => m.CalistigiGunler)
                .FirstOrDefaultAsync(p => p.Id == personelId);

            if (islem == null || personel == null)
                return RedirectToAction("IslemSec");

            TimeSpan islemSuresi = islem.Sure;

            // Mesai kontrolü
            var uygunMesai = personel.Mesailer.Any(m =>
                m.CalistigiGunler.Any(g => g.Gun == randevuTarihi.DayOfWeek) &&
                m.BaslangicZamani <= randevuSaati &&
                m.BitisZamani >= randevuSaati.Add(islemSuresi));

            if (!uygunMesai)
            {
                TempData["HataMesaji"] = "Seçilen tarihte ve saatte personelin mesaisi yok.";
                return RedirectToAction("TarihSaatSec", new { islemId, personelId });
            }

            // Randevu çakışma kontrolü
            var randevuCakisma = await _context.Randevular
                .Where(r => r.PersonelId == personelId && r.RandevuTarihi.Date == randevuTarihi.Date)
                .AnyAsync(r => r.RandevuSaati <= randevuSaati && r.RandevuSaati.Add(r.Sure) > randevuSaati);

            if (randevuCakisma)
            {
                TempData["HataMesaji"] = "Seçilen tarihte ve saatte personelin başka bir randevusu var.";
                return RedirectToAction("TarihSaatSec", new { islemId, personelId });
            }

            return RedirectToAction("RandevuOnayla");
        }

        /*
        public async Task<IActionResult> RandevuOnayla(int islemId, int personelId, DateTime randevuTarihi, TimeSpan randevuSaati)
        {
            var islem = await _context.Islemler.FindAsync(islemId);
            var personel = await _context.Personeller.FindAsync(personelId);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name); // Oturumdaki kullanıcıyı al

            if (islem == null || personel == null || user == null)
                return RedirectToAction("IslemSec");

            ViewBag.Islem = islem;
            ViewBag.Personel = personel;
            ViewBag.User = user;
            ViewBag.RandevuTarihi = randevuTarihi;
            ViewBag.RandevuSaati = randevuSaati;

            return View();
        }*/
        /*
        [HttpPost]
        public async Task<IActionResult> RandevuOnayla(int islemId, int personelId, DateTime randevuTarihi, TimeSpan randevuSaati)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var randevu = new Randevu
            {
                UserId = user.Id,
                IslemId = islemId,
                PersonelId = personelId,
                RandevuTarihi = randevuTarihi,
                RandevuSaati = randevuSaati,
                OnayliMi = false // Başlangıçta "beklemede" durumu
            };

            _context.Randevular.Add(randevu);
            await _context.SaveChangesAsync();

            TempData["BasariMesaji"] = "Randevu talebiniz alınmıştır. Onay bekleniyor.";
            return RedirectToAction("Randevularim", "User");
        }*/

    }
}
