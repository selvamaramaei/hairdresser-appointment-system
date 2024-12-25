using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProje.Context;
using WebProje.Models;

namespace WebProje.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KazancController : ControllerBase
    {
        private readonly AppDbContext _context;

        public KazancController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Hesapla")]
        public async Task<IActionResult> Hesapla([FromBody] TarihAraligiModel model)
        {
            if (model.BaslangicTarihi > model.BitisTarihi)
            {
                return BadRequest("Başlangıç tarihi, bitiş tarihinden büyük olamaz.");
            }

            var toplamKazanc = await _context.Randevular
                .Where(r => r.Durum == "Onaylandı" &&
                            r.RandevuTarihi >= model.BaslangicTarihi &&
                            r.RandevuTarihi <= model.BitisTarihi)
                .SumAsync(r => r.Ucret);

            var kazanc = new Kazanc
            {
                BaslangicTarihi = model.BaslangicTarihi,
                BitisTarihi = model.BitisTarihi,
                ToplamKazanc = toplamKazanc
            };

            return Ok(kazanc);
        }
    }

    public class TarihAraligiModel
    {
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
    }
}
