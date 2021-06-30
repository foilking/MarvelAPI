using System;
using System.Collections.Generic;

namespace MarvelAPI.Parameters
{
    public class GetComicsFor
    {
        public GetComicsFor()
        {
            Creators = new List<int>();
            Series = new List<int>();
            Events = new List<int>();
            Stories = new List<int>();
            SharedAppearances = new List<int>();
            Collaborators = new List<int>();
            Order = new List<OrderBy>();
        }

        public ComicFormat? Format { get; set; }
        public ComicFormatType? FormatType { get; set; }
        public bool? NoVariants { get; set; }
        public DateDescriptor? DateDescript { get; set; }
        public DateTime? DateRangeBegin { get; set; }
        public DateTime? DateRangeEnd { get; set; }
        public bool? HasDigitalIssue { get; set; }
        public DateTime? ModifiedSince { get; set; }
        public IEnumerable<int> Characters { get; set; }
        public IEnumerable<int> Creators { get; set; }
        public IEnumerable<int> Series { get; set; }
        public IEnumerable<int> Events { get; set; }
        public IEnumerable<int> Stories { get; set; }
        public IEnumerable<int> SharedAppearances { get; set; }
        public IEnumerable<int> Collaborators { get; set; }
        public IEnumerable<OrderBy> Order { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }
    }

    public class GetComicsForCharacter : GetComicsFor
    {
        public int CharacterId { get; set; }
    }

    public class GetComicsForCreator : GetComicsFor
    {
        public int CreatorId { get; set; }
    }

    public class GetComicsForEvent : GetComicsFor
    {
        public int EventId { get; set; }
    }

    public class GetComicsForSeries : GetComicsFor
    {
        public int SeriesId { get; set; }
    }

    public class GetComicsForStory : GetComicsFor
    {
        public int StoryId { get; set; }
    }
}
