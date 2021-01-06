using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenLinks.Models
{
    public class Link
    {
        public int Id { get; set; }
        public string Adress { get; set; }

        public string Description { get; set; }

        public int KeywordId { get; set; }
        public virtual Keyword Keyword { get; set; }
    }
}