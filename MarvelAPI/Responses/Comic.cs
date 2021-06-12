using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI
{
    public class Comic
    {
        public int Id { get; set; }
        public int DigitalId { get; set; }
        public string Title { get; set; }
        public int IssueNumber { get; set; }
        public string VariantDescription { get; set; }
        public string Description { get; set; }
        public DateTime Modified { get; set; }
        public string ISBN { get; set; }
        public string UPC { get; set; }
        public string DiamondCode { get; set; }
        public string EAN { get; set; }
        public string ISSN { get; set; }
        public string Format { get; set; }
        public int PageCount { get; set; }
        public List<TextObject> TextObjects { get; set; }
        public string ResourceURI { get; set; }
        public List<MarvelUrl> Urls { get; set; }
        public SeriesSummary Series { get; set; }
        public List<ComicSummary> Variants { get; set; }
        public List<ComicSummary> Collections { get; set; }
        public List<ComicSummary> CollectedIssues { get; set; }
        public List<ComicDate> Dates { get; set; }
        public List<ComicPrice> Prices { get; set; }
        public MarvelImage Thumbnail { get; set; }
        public List<MarvelImage> Images { get; set; }
        public ResourceList<CreatorSummary> Creators { get; set; }
        public ResourceList<CharacterSummary> Characters { get; set; }
        public ResourceList<StorySummary> Stories { get; set; }
        public ResourceList<EventSummary> Events { get; set; }
    }

    public class ComicSummary : Summary
    {
    }

    public class ComicDate
    {
        public string Type { get; set; }
        public DateTime Date { get; set; }
    }

    public class ComicPrice
    {
        public string Type { get; set; }
        public float Price { get; set; }
    }
}
