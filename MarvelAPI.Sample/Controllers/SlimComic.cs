using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarvelAPI.Sample.Controllers
{
    public class SlimComic
    {
        public int Id { get; set; }
        public int DigitalId { get; set; }
        public string Title { get; set; }
        public int IssueNumber { get; set; }
        public string VariantDescription { get; set; }
        public string Description { get; set; }
        public DateTime Modified { get; set; }
        public string ISBN { get; set; }
        public string UPC { get; set; }
        public string DiamondCode { get; set; }
        public string EAN { get; set; }
        public string ISSN { get; set; }
        public string Format { get; set; }
        public int PageCount { get; set; }
        public string ResourceURI { get; set; }
    }
}
