using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Filmovi_projekt.Models
{
    public class Comments
    {
        [Key]
        public int id_comment { get; set; }

        [Column(TypeName = "int")]
        public int id_user { get; set; }
        [Column(TypeName = "int")]
        public int id_film { get; set; }

        [Column (TypeName = "varchar(MAX)")]
        public string comment { get; set; }

        [Column (TypeName = "DateTime NULL")]
        public DateTime change_date { get; set; }

        [Column (TypeName = "DateTime NULL")]
        public DateTime insert_date { get; set; }
    }
}
