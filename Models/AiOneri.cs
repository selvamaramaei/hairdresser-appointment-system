using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProje.Models
{
    public class AiOneri
    {
        [Key] 
        public int Id { get; set; }

        [NotMapped]
        [Required]
        [Display(Name = "Fotoğraf")]
        [DataType(DataType.Upload)]

        public IFormFile? Photo { get; set; }


        [Required]
        [Display(Name = "Saç Tipi")]
        public string? HairStyle { get; set; }

        public List<int> HairStyles { get; set; } = new List<int>
        {
            101, 201, 301, 401, 402, 403, 502, 503, 603, 801, 901, 1001, 1101, 1201, 1301
        };

    }
}
