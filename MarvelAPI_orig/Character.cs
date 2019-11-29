using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI
{
    public class CharacterDataWrapper
    {
        public int Code { get; set; }
        public string Status { get; set; }
        public CharacterDataContainer Data { get; set; }
        public string Etag { get; set; }
    }

    public class CharacterDataContainer
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
        public int Total { get; set; }
        public int Count { get; set; }
        public List<Character> Results { get; set; }
    }

    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Modified { get; set; }
        public string ResourceURI { get; set; }
        public List<MarvelUrl> Urls { get; set; }
        public MarvelImage Thumbnail { get; set; }
        public ComicList Comics { get; set; }
        public StoryList Stories { get; set; }
        public EventList Events { get; set; }
        public SeriesList Series { get; set; }
    }

    public class CharacterList
    {
        public int Available { get; set; }
        public int Returned { get; set; }
        public string CollectionURI { get; set; }
        public List<CharacterSummary> Items { get; set; }
    }

    public class CharacterSummary
    {
        public string ResourceURI { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }
}
