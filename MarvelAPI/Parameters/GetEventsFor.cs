using System;
using System.Collections.Generic;

namespace MarvelAPI.Parameters
{
    public class GetEventsFor
    {
        public GetEventsFor()
        {
            Creators = new List<int>();
            Series = new List<int>();
            Stories = new List<int>();
            Order = new List<OrderBy>();
        }
        public string Name { get; set; }
        public string NameStartsWith { get; set; }
        public DateTime? ModifiedSince { get; set; }
        public IEnumerable<int> Characters { get; set; }
        public IEnumerable<int> Comics { get; set; }
        public IEnumerable<int> Creators { get; set; }
        public IEnumerable<int> Series { get; set; }
        public IEnumerable<int> Stories { get; set; }
        public IEnumerable<OrderBy> Order { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }
    }

    public class GetEventsForCharacter : GetEventsFor
    {
        public int CharacterId { get; set; }
    }
    
    public class GetEventsForComic : GetEventsFor
    {
        public int ComicId { get; set; }
    }

    public class GetEventsForCreator : GetEventsFor
    {
        public int CreatorId { get; set; }
    }

    public class GetEventsForSeries : GetEventsFor
    {
        public int SeriesId { get; set; }
    }

    public class GetEventsForStory : GetEventsFor
    {
        public int StoryId { get; set; }
    }
}
