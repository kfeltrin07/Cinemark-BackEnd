using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Filmovi_projekt.Models
{
    public class Genre
    {
        [Key] 
        public int id_genre { get; set; }

        [Column (TypeName = "varchar(255)")] 
        public string name { get; set; }

        [Column (TypeName = "varchar(255)")]
        public string description { get; set; }
    }
}
