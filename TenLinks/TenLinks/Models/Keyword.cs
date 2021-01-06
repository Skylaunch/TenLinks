using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenLinks.Models
{
    public class Keyword
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public virtual ICollection<Link> Link { get; set; }

    }
}
