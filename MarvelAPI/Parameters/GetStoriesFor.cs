using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI.Parameters
{
    public class GetStoriesFor
    {
        public GetStoriesFor()
        {
            Comics = new List<int>();
            Series = new List<int>();
            Events = new List<int>();
            Creators = new List<int>();
            Order = new List<OrderBy>();
        }

        public DateTime? ModifiedSince { get; set; }
        public IEnumerable<int> Comics { get; set; }
        public IEnumerable<int> Series { get; set; }
        public IEnumerable<int> Events { get; set; }
        public IEnumerable<int> Creators { get; set; }
        public IEnumerable<int> Characters { get; set; }
        public IEnumerable<OrderBy> Order { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }
    }

    public class GetStoriesForCharacter : GetStoriesFor
    {
        public int CharacterId { get; set; }
    }

    public class GetStoriesForComic : GetStoriesFor
    {
        public int ComicId { get; set; }
    }
}
