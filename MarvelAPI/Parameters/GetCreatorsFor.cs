using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI.Parameters
{
    public class GetCreatorsFor
    {
        public GetCreatorsFor()
        {
            Comics = new List<int>();
            Series = new List<int>();
            Stories = new List<int>();
            Order = new List<OrderBy>();
        }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string NameStartsWith { get; set; }
        public string FirstNameStartsWith { get; set; }
        public string MiddleNameStartsWith { get; set; }
        public string LastNameStartsWith { get; set; }
        public DateTime? ModifiedSince { get; set; }
        public IEnumerable<int> Comics { get; set; }
        public IEnumerable<int> Events { get; set; }
        public IEnumerable<int> Series { get; set; }
        public IEnumerable<int> Stories { get; set; }
        public IEnumerable<OrderBy> Order { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }
    }

    public class GetCreatorsForComic : GetCreatorsFor
    {
        public int ComicId { get; set; }
    }

    public class GetCreatorsForEvent : GetCreatorsFor
    {
        public int EventId { get; set; }
    }

    public class GetCreatorsForSeries : GetCreatorsFor
    {
        public int SeriesId { get; set; }
    }
}
