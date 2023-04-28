using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Filmovi_projekt.Models
{
    public class Film_Genre
    {
        [Key]
        public int id_genre { get; set; }

        [Column (TypeName = "int")]
        public int id_film { get; set; }
    }
}
