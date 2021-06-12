using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Modified { get; set; }
        public string ResourceURI { get; set; }
        public List<MarvelUrl> Urls { get; set; }
        public MarvelImage Thumbnail { get; set; }
        public ResourceList<ComicSummary> Comics { get; set; }
        public ResourceList<StorySummary> Stories { get; set; }
        public ResourceList<EventSummary> Events { get; set; }
        public ResourceList<SeriesSummary> Series { get; set; }
    }

    public class CharacterSummary : Summary
    {
        public string Role { get; set; }
    }
}
