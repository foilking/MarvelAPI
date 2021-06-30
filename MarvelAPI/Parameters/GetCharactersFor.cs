using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI.Parameters
{
    public class GetCharactersFor
    {
        public string Name { get; set; }
        public string NameStartsWith { get; set; }
        public DateTime? ModifiedSince { get; set; }
        public IEnumerable<int> Comics { get; set; }
        public IEnumerable<int> Events { get; set; }
        public IEnumerable<int> Series { get; set; }
        public IEnumerable<int> Stories { get; set; }
        public IEnumerable<OrderBy> Order { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }
    }

    public class GetCharactersForComic : GetCharactersFor
    {
        public int ComicId { get; set; }
    }

    public class GetCharactersForEvent : GetCharactersFor
    {
        public int EventId { get; set; }
    }

    public class GetCharactersForSeries : GetCharacters
    {
        public int SeriesId { get; set; }
    }

    public class GetCharactersForStory : GetCharactersFor
    {
        public int StoryId { get; set; }
    }
}
