using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Filmovi_projekt.Models
{
    public class Rating
    {
        [Key]
        public int id_rating { get; set; }

       
        [Column(TypeName = "int")]
        public int id_film { get; set; }


        [Column(TypeName = "float")]
        public float rating { get; set; }


        [Column(TypeName = "DateTime")]
        public DateTime change_date { get; set; }


        [Column(TypeName = "DateTime")]
        public DateTime insert_date { get; set; }

        [Column(TypeName = "int")]
        public int id_user { get; set; }

        
    }
}
