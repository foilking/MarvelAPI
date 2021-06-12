using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ResourceURI { get; set; }
        public List<MarvelUrl> Urls { get; set; }
        public DateTime Modified { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public MarvelImage Thumbnail { get; set; }
        public ResourceList<CharacterSummary> Comics { get; set; }
        public ResourceList<StorySummary> Stories { get; set; }
        public ResourceList<SeriesSummary> Series { get; set; }
        public ResourceList<CharacterSummary> Characters { get; set; }
        public ResourceList<CreatorSummary> Creators { get; set; }
        public EventSummary Next { get; set; }
        public EventSummary Previous { get; set; }
    }

    public class EventSummary : Summary
    {
    }
}
