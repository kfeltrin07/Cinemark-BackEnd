using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Filmovi_projekt.Models
{
    public class User {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_user { get; set; }

        [Column (TypeName = "nvarchar(255)")]
        public string username { get; set; }

        [Column (TypeName = "nvarchar(255)")] 
        public string password { get; set; }

        [Column (TypeName = "nvarchar(255)")]
        public string email { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string token { get; set; }

        [Column(TypeName = "bit")]
        public Boolean verified { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string activation_code { get; set; }

        [Column(TypeName = "int")]
        public int role { get; set; }
    }
}
