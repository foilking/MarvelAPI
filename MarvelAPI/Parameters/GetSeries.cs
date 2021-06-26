using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI.Parameters
{
    public class GetSeries
    {
        public GetSeries()
        {
            Comics = new List<int>();
            Characters = new List<int>();
            Creators = new List<int>();
            Events = new List<int>();
            Stories = new List<int>();
            Order = new List<OrderBy>();
        }

        public string Title  { get; set; }
        public string TitleStartsWith  { get; set; }
        public DateTime? ModifiedSince  { get; set; }
        public IEnumerable<int> Characters  { get; set; }
        public IEnumerable<int> Comics  { get; set; }
        public IEnumerable<int> Creators  { get; set; }
        public IEnumerable<int> Events  { get; set; }
        public IEnumerable<int> Stories  { get; set; }
        public SeriesType? Type  { get; set; }
        public ComicFormat? Contains  { get; set; }
        public IEnumerable<OrderBy> Order  { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }
    }
}
