using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI.Parameters
{
    public class GetEvents
    {
        public string Name { get; set; }
        public string NameStartsWith { get; set; }
        public DateTime? ModifiedSince { get; set; }
        public IEnumerable<int> Creators { get; set; }
        public IEnumerable<int> Characters { get; set; }
        public IEnumerable<int> Series { get; set; }
        public IEnumerable<int> Comics { get; set; }
        public IEnumerable<int> Stories { get; set; }
        public IEnumerable<OrderBy> Order { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }
    }
}
