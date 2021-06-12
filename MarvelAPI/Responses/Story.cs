using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI
{
    public class Story
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ResourceURI { get; set; }
        public string Type { get; set; }
        public DateTime Modified { get; set; }
        public MarvelImage Thumbnail { get; set; }
        public ResourceList<ComicSummary> Comics { get; set; }
        public ResourceList<SeriesSummary> Series { get; set; }
        public ResourceList<EventSummary> Events { get; set; }
        public ResourceList<CharacterSummary> Characters { get; set; }
        public ResourceList<CreatorSummary> Creators { get; set; }
        public ComicSummary OriginalIssue { get; set; }
    }
    
    public class StorySummary : Summary
    {
        public string Type { get; set; }
    }
}
