using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZadanieRekrutacyjne.Models
{
    public class Tree
    {
        public int TreeId { get; set; }

        public string Name { get; set; }

        public int ParentID { get; set; }

        public virtual ICollection<Tree> Kids { get; set; }

    }
}