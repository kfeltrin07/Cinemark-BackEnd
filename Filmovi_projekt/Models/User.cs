using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Filmovi_projekt.Models
{
    public class User {
        [Key]
        public int id_user { get; set; }

        [Column (TypeName = "nvarchar(255)")]
        public string username { get; set; }

        [Column (TypeName = "nvarchar(255)")] 
        public string password { get; set; }

        [Column (TypeName = "nvarchar(255)")]
        public string email { get; set; }
    }
}
