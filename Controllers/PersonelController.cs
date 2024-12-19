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
            ViewBag.Uzmanliklar = await _context.Uzmanliklar
                .Select(u => new
                {
                    u.Id,
                    u.UzmanlikAdi
                }).ToListAsync();

            return View();
        }


        // POST: Personel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Personel personel, List<int> seciliUzmanliklar, string MesaiBaslangic, string MesaiBitis, List<DayOfWeek> CalismaGunleri)
        {
            if (ModelState.IsValid)
            {
                if (seciliUzmanliklar != null && seciliUzmanliklar.Any())
                {
                    var uzmanliklar = await _context.Uzmanliklar
                        .Where(u => seciliUzmanliklar.Contains(u.Id))
                        .ToListAsync();

                    foreach (var uzmanlik in uzmanliklar)
                    {
                        personel.Uzmanliklar.Add(uzmanlik);
                    }
                }

                // Mesai saatlerini doğru şekilde dönüştür
                if (TimeSpan.TryParse(MesaiBaslangic, out TimeSpan mesaiBaslangic) && TimeSpan.TryParse(MesaiBitis, out TimeSpan mesaiBitis))
                {
                    var mesai = new Mesai
                    {
                        BaslangicZamani = mesaiBaslangic,
                        BitisZamani = mesaiBitis
                    };

                    if (CalismaGunleri != null && CalismaGunleri.Any())
                    {
                        mesai.CalistigiGunler = CalismaGunleri.Select(gun => new MesaiGunu
                        {
                            Gun = gun
                        }).ToList();
                    }

                    personel.Mesailer.Add(mesai);
                }

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
        public async Task<IActionResult> Edit(int id)
        {
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
                    Text = u.UzmanlikAdi,
                    Selected = personel.Uzmanliklar.Any(uzmanlik => uzmanlik.Id == u.Id) // Mevcut uzmanlıkları işaretle
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
                    // Personelin mevcut verilerini getir
                    var mevcutPersonel = await _context.Personeller
                        .Include(p => p.Mesailer)
                            .ThenInclude(m => m.CalistigiGunler)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (mevcutPersonel == null)
                    {
                        return NotFound();
                    }

                    // Uzmanlıkları güncelle
                    var mevcutUzmanliklar = await _context.Uzmanliklar
                        .Where(u => seciliUzmanliklar.Contains(u.Id))
                        .ToListAsync();

                    // Mevcut uzmanlıkları temizleyip yeni uzmanlıkları ekleyelim
                    mevcutPersonel.Uzmanliklar.Clear(); // Eski uzmanlıkları temizle
                    foreach (var uzmanlik in mevcutUzmanliklar)
                    {
                        mevcutPersonel.Uzmanliklar.Add(uzmanlik); // Yeni uzmanlıkları ekle
                    }
                    // Mesai güncellemeleri
                    var mesai = mevcutPersonel.Mesailer.FirstOrDefault();
                    if (mesai != null)
                    {
                        mesai.BaslangicZamani = MesaiBaslangic;
                        mesai.BitisZamani = MesaiBitis;

                        // Mevcut çalışma günlerini temizle
                        _context.MesaiGunleri.RemoveRange(mesai.CalistigiGunler);

                        // Yeni çalışma günlerini ekle
                        mesai.CalistigiGunler = CalismaGunleri.Select(gun => new MesaiGunu
                        {
                            Gun = gun
                        }).ToList();
                    }

                    // Personeli güncelle
                    _context.Update(mevcutPersonel);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Personel başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Personeller.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

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
        // GET: Personel/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var personel = await _context.Personeller
                .Include(p => p.Uzmanliklar)
                .Include(p => p.Mesailer)
                    .ThenInclude(m => m.CalistigiGunler)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (personel == null)
            {
                return NotFound();
            }

            return View(personel);
        }

        // POST: Personel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var personel = await _context.Personeller
                .Include(p => p.Mesailer)
                    .ThenInclude(m => m.CalistigiGunler)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (personel == null)
            {
                return NotFound();
            }

            // Mesai ve çalışma günlerini kaldır
            if (personel.Mesailer != null)
            {
                foreach (var mesai in personel.Mesailer)
                {
                    _context.MesaiGunleri.RemoveRange(mesai.CalistigiGunler);
                }
                _context.Mesailer.RemoveRange(personel.Mesailer);
            }

            // Personeli sil
            _context.Personeller.Remove(personel);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Personel başarıyla silindi.";
            return RedirectToAction(nameof(Listele));
        }


    }
}
