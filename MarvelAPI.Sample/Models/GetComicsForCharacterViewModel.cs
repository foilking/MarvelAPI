using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarvelAPI.Sample.Models
{
    public class GetComicsForCharacterViewModel
    {
        public int CharacterId { get; set; }

        public ComicFormat? Format { get; set; }
        public ComicFormatType? FormatType { get; set; }
        public bool? NoVariants { get; set; }
        public DateDescriptor? Descriptor { get; set; }
        public bool? HasDigitalIssue { get; set; }
        public IEnumerable<OrderBy> Order { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }

        public IEnumerable<Comic> ResultComics { get; set; }
    }
}