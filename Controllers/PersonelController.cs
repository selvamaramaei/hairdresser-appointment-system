using Microsoft.AspNetCore.Mvc;
using WebProje.Context;
using Microsoft.EntityFrameworkCore;
using WebProje.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebProje.Controllers
{
    public class PersonelController : Controller
    {
        private readonly AppDbContext _context;

        public PersonelController(AppDbContext context)
        {
            _context = context;
        }

        // Personel Listeleme
        public async Task<IActionResult> Listele()
        {
            var personeller = await _context.Personeller
                .Include(p => p.Uzmanliklar)
                .ThenInclude(u => u.Islemler)
                .Include(p => p.Mesailer)
                .ThenInclude(m => m.CalistigiGunler)
                .ToListAsync();

            return View(personeller);
        }

        // GET: Personel/Create
        public async Task<IActionResult> Create()
        {
            // Uzmanlık listesi ViewBag üzerinden gönderiliyor
            ViewBag.Uzmanliklar = await _context.Uzmanliklar
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.UzmanlikAdi
                }).ToListAsync();

            return View();
        }

        // POST: Personel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Personel personel, List<int> seciliUzmanliklar, TimeSpan MesaiBaslangic, TimeSpan MesaiBitis, List<DayOfWeek> CalismaGunleri)
        {
            if (ModelState.IsValid)
            {
                // Seçilen uzmanlıkları ilişkilendir
                if (seciliUzmanliklar != null && seciliUzmanliklar.Any())
                {
                    personel.Uzmanliklar = await _context.Uzmanliklar
                        .Where(u => seciliUzmanliklar.Contains(u.Id))
                        .ToListAsync();
                }

                // Mesai ve çalışma günlerini oluştur
                var mesai = new Mesai
                {
                    BaslangicZamani = MesaiBaslangic,
                    BitisZamani = MesaiBitis
                };

                if (CalismaGunleri != null && CalismaGunleri.Any())
                {
                    mesai.CalistigiGunler = CalismaGunleri.Select(gun => new MesaiGunu
                    {
                        Gun = gun
                    }).ToList();
                }

                // Personelin mesaisini ilişkilendir
                personel.Mesailer.Add(mesai);

                // Personeli kaydet
                _context.Personeller.Add(personel);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Personel başarıyla eklendi.";
                return RedirectToAction(nameof(Listele));
            }

            // Hata durumunda uzmanlıkları tekrar doldur
            ViewBag.Uzmanliklar = await _context.Uzmanliklar
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.UzmanlikAdi
                }).ToListAsync();

            return View(personel);
        }

    }
}
