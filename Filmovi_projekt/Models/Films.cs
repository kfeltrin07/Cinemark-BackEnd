﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Filmovi_projekt.Models
{
    public class Films
    {
        [Key]
        public int id_film { get; set; }

        [Column (TypeName ="varchar(255)")]
        public string title { get; set; }

        [Column (TypeName = "varchar(255)")]
        public string director { get; set; }

        [Column (TypeName = "varchar(255)")]
        public string main_actor { get; set; }

        [Column (TypeName = "DateTime")]
        public DateTime release_date { get; set; } 

        [Column (TypeName = "varchar(500)")]
        public string summary { get; set; }
    }
}
