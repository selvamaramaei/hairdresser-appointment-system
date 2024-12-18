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

        // GET: Personel/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Personeller == null)
            {
                return NotFound();
            }

            var personel = await _context.Personeller
                .Include(p => p.Uzmanliklar)
                .Include(p => p.Mesailer)
                .ThenInclude(m => m.CalistigiGunler)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (personel == null)
            {
                return NotFound();
            }

            ViewBag.Uzmanliklar = await _context.Uzmanliklar
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.UzmanlikAdi
                }).ToListAsync();

            return View(personel);
        }

        // POST: Personel/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Personel personel, List<int> seciliUzmanliklar, TimeSpan MesaiBaslangic, TimeSpan MesaiBitis, List<DayOfWeek> CalismaGunleri)
        {
            if (id != personel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Mevcut personeli yükle
                    var mevcutPersonel = await _context.Personeller
                        .Include(p => p.Uzmanliklar)
                        .Include(p => p.Mesailer)
                        .ThenInclude(m => m.CalistigiGunler)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (mevcutPersonel == null)
                    {
                        return NotFound();
                    }

                    // Personel bilgilerini güncelle
                    mevcutPersonel.Ad = personel.Ad;
                    mevcutPersonel.Soyad = personel.Soyad;
                    mevcutPersonel.Cinsiyet = personel.Cinsiyet;

                    // Uzmanlıkları güncelle
                    mevcutPersonel.Uzmanliklar.Clear();
                    if (seciliUzmanliklar != null && seciliUzmanliklar.Any())
                    {
                        mevcutPersonel.Uzmanliklar = await _context.Uzmanliklar
                            .Where(u => seciliUzmanliklar.Contains(u.Id))
                            .ToListAsync();
                    }

                    // Mesai bilgilerini güncelle
                    if (mevcutPersonel.Mesailer.Any())
                    {
                        var mesai = mevcutPersonel.Mesailer.First();
                        mesai.BaslangicZamani = MesaiBaslangic;
                        mesai.BitisZamani = MesaiBitis;
                        mesai.CalistigiGunler.Clear();

                        if (CalismaGunleri != null && CalismaGunleri.Any())
                        {
                            mesai.CalistigiGunler = CalismaGunleri.Select(gun => new MesaiGunu { Gun = gun }).ToList();
                        }
                    }

                    _context.Update(mevcutPersonel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonelExists(personel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "Personel başarıyla güncellendi.";
                return RedirectToAction(nameof(Listele));
            }

            ViewBag.Uzmanliklar = await _context.Uzmanliklar
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.UzmanlikAdi
                }).ToListAsync();

            return View(personel);
        }

        private bool PersonelExists(int id)
        {
            return _context.Personeller.Any(e => e.Id == id);
        }

    }
}
