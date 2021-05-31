using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI
{
    public class Series
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ResourceURI { get; set; }
        public List<MarvelUrl> Urls { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public string Rating { get; set; }
        public DateTime Modified { get; set; }
        public MarvelImage Thumbnail { get; set; }
        public ResourceList<ComicSummary> Comics { get; set; }
        public ResourceList<StorySummary> Stories { get; set; }
        public ResourceList<EventSummary> Events { get; set; }
        public ResourceList<CharacterSummary> Characters { get; set; }
        public ResourceList<CreatorSummary> Creators { get; set; }
        public SeriesSummary Next { get; set; }
        public SeriesSummary Previous { get; set; }
    }

    public class SeriesSummary : Summary { }
}
