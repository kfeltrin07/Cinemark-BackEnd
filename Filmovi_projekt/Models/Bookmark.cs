using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Filmovi_projekt.Models
{
    public class Bookmark 
    {
        [Key]
        public int id_Bookmark { get; set; }

        [Column(TypeName = "int")]
        public int id_user { get; set; }

        [Column (TypeName = "int")]
        public int id_film { get; set; }
    }
}
