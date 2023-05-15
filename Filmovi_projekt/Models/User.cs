using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Filmovi_projekt.Models
{
    public class User {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_user { get; set; }

        [Column (TypeName = "varchar(255)")]
        public string username { get; set; }

        [Column (TypeName = "varchar(255)")] 
        public string password { get; set; }

        [Column (TypeName = "varchar(255)")]
        public string email { get; set; }

        [Column(TypeName = "bit")]
        public Boolean verified { get; set; }

        public string activation_code { get; set; }

        public string role { get; set; }

        public string token { get; set; }

        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }

    }
}
