using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;


namespace WebProje.Models
{
    public class Randevu
    {
        [Key]
        public int Id { get; set; }


        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }


        [Required]
        public int PersonelId { get; set; }

        [ForeignKey("PersonelId")]
        public Personel Personel { get; set; }


        [Required]
        public int IslemId { get; set; }

        [ForeignKey("IslemId")]
        public Islem Islem { get; set; }


        [Required]
        public DateTime RandevuTarihi { get; set; }

        [Required]
        public bool OnayliMi { get; set; }

        // Süre ve Ücret, Islem sınıfından alınır
        public TimeSpan Sure => Islem?.Sure ?? TimeSpan.Zero;
        public decimal Ucret => Islem?.Ucret ?? 0;
    }
}
