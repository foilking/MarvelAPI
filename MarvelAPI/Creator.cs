using System;
using System.Collections.Generic;

namespace MarvelAPI
{
    public class Creator
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string FullName { get; set; }
        public DateTime Modified { get; set; }
        public string ResourceURI { get; set; }
        public List<MarvelUrl> Urls { get; set; }
        public MarvelImage Thumbnail { get; set; }
        public ResourceList<SeriesSummary> Series { get; set; }
        public ResourceList<StorySummary> Stories { get; set; }
        public ResourceList<CharacterSummary> Comics { get; set; }
        public ResourceList<EventSummary> Events { get; set; }
    }

    public class CreatorSummary : Summary
    {
        public string Role { get; set; }
    }
}
