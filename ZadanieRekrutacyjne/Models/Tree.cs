using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZadanieRekrutacyjne.Models
{
    public class Tree
    {
        public int TreeId { get; set; }

        [Required]
        [Display(Name ="Nazwa")]
        public string Name { get; set; }

        [Required]
        [Display(Name="ID Rodzica")]
        public int ParentID { get; set; }

        

    }
}