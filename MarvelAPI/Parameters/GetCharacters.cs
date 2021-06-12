using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI.Parameters
{
    public class GetCharacters
    {
        public GetCharacters()
        {
            Comics = new List<int>();
            Series = new List<int>();
            Events = new List<int>();
            Stories = new List<int>();
            Order = new List<OrderBy>();
        }

        public string Name { get; set; }
        public string NameStartsWith { get; set; }
        public DateTime? ModifiedSince { get; set; }
        public IEnumerable<int> Comics { get; set; }
        public IEnumerable<int> Series { get; set; }
        public IEnumerable<int> Events { get; set; }
        public IEnumerable<int> Stories { get; set; }
        public IEnumerable<OrderBy> Order { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }
    }
}
