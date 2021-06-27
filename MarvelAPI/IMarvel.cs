using MarvelAPI.Parameters;
using System;
using System.Collections.Generic;

namespace MarvelAPI
{
    public interface IMarvel
    {
        #region Character methods
        [Obsolete("Use method with GetCharacter object")]
        IEnumerable<Character> GetCharacters(string Name, string NameStartsWith, DateTime? ModifiedSince, IEnumerable<int> Comics, IEnumerable<int> Series, IEnumerable<int> Events, IEnumerable<int> Stories, IEnumerable<OrderBy> Order, int? Limit, int? Offset);
        IEnumerable<Character> GetCharacters(GetCharacters model);

        Character GetCharacter(int CharacterId);

        [Obsolete("Use method with GetComicsForCharacter object")]
        IEnumerable<Comic> GetComicsForCharacter(int CharacterId, ComicFormat? Format, ComicFormatType? FormatType, bool? NoVariants, DateDescriptor? DateDescript, DateTime? DateRangeBegin, DateTime? DateRangeEnd, bool? HasDigitalIssue, DateTime? ModifiedSince,
            IEnumerable<int> Creators, IEnumerable<int> Series, IEnumerable<int> Events, IEnumerable<int> Stories, IEnumerable<int> SharedAppearances, IEnumerable<int> Collaborators, IEnumerable<OrderBy> Order,
            int? Limit, int? Offset);
        IEnumerable<Comic> GetComicsForCharacter(GetComicsForCharacter model);

        [Obsolete("Use method with GetEventsForCharacter object")]
        IEnumerable<Event> GetEventsForCharacter(int CharacterId, string Name, string NameStartsWith, DateTime? ModifiedSince, IEnumerable<int> Creators, IEnumerable<int> Series, IEnumerable<int> Comics, IEnumerable<int> Stories, IEnumerable<OrderBy> Order, int? Limit, int? Offset);
        IEnumerable<Event> GetEventsForCharacter(GetEventsForCharacter model);

        [Obsolete("Use method with GetSeriesForCharacter object")]
        IEnumerable<Series> GetSeriesForCharacter(int CharacterId, string Title, string TitleStartsWith, DateTime? ModifiedSince, IEnumerable<int> Comics, IEnumerable<int> Stories, IEnumerable<int> Events, IEnumerable<int> Creators, SeriesType? SeriesType, IEnumerable<ComicFormat> Contains, IEnumerable<OrderBy> Order, int? Limit, int? Offset);
        IEnumerable<Series> GetSeriesForCharacter(GetSeriesForCharacter model);

        [Obsolete("Use method with GetStoriesForCharacter object")]
        IEnumerable<Story> GetStoriessForCharacter(int CharacterId, DateTime? ModifiedSince, IEnumerable<int> Comics, IEnumerable<int> Series, IEnumerable<int> Events, IEnumerable<int> Creators, IEnumerable<OrderBy> Order, int? Limit, int? Offset);
        IEnumerable<Story> GetStoriesForCharacter(GetStoriesForCharacter model);
        #endregion
        #region Comic methods
        [Obsolete("Use method with GetComics object")]
        IEnumerable<Comic> GetComics(
            ComicFormat? Format, ComicFormatType? FormatType,
            bool? NoVariants, DateDescriptor? DateDescript,
            DateTime? DateRangeBegin, DateTime? DateRangeEnd,
            bool? HasDigitalIssue, DateTime? ModifiedSince,
            IEnumerable<int> Creators, IEnumerable<int> Characters,
            IEnumerable<int> Series, IEnumerable<int> Events,
            IEnumerable<int> Stories, IEnumerable<int> SharedAppearances,
            IEnumerable<int> Collaborators, IEnumerable<OrderBy> Order,
            int? Limit, int? Offset);
        IEnumerable<Comic> GetComics(GetComics model);

        Comic GetComic(int ComicId);

        [Obsolete("Use method with GetCharactersForComic object")]
        IEnumerable<Character> GetCharactersForComic(
            int ComicId, string Name, string NameStartsWith,
            DateTime? ModifiedSince, IEnumerable<int> Series,
            IEnumerable<int> Events, IEnumerable<int> Stories,
            IEnumerable<OrderBy> Order, int? Limit, int? Offset);
        IEnumerable<Character> GetCharactersForComic(GetCharactersForComic model);

        [Obsolete("Use method with GetCreatorsForComic object")]
        IEnumerable<Creator> GetCreatorsForComic(
            int ComicId, string FirstName, string MiddleName,
            string LastName, string Suffix, string NameStartsWith,
            string FirstNameStartsWith, string MiddleNameStartsWith,
            string LastNameStartsWith, DateTime? ModifiedSince,
            IEnumerable<int> Comics, IEnumerable<int> Series,
            IEnumerable<int> Stories, IEnumerable<OrderBy> Order,
            int? Limit, int? Offset);
        IEnumerable<Creator> GetCreatorsForComic(GetCreatorsForComic model);

        [Obsolete("Use method with GetEventsForComic object")]
        IEnumerable<Event> GetEventsForComic(
            int ComicId, string Name, string NameStartsWith,
            DateTime? ModifiedSince, IEnumerable<int> Creators,
            IEnumerable<int> Characters, IEnumerable<int> Series,
            IEnumerable<int> Stories, IEnumerable<OrderBy> Order,
            int? Limit, int? Offset);
        IEnumerable<Event> GetEventsForComic(GetEventsForComic model);

        [Obsolete("Use method with GetStoriesForComic object")]
        IEnumerable<Story> GetStoriesForComic(
            int ComicId, DateTime? ModifiedSince, IEnumerable<int> Series,
            IEnumerable<int> Events, IEnumerable<int> Creators,
            IEnumerable<int> Characters, IEnumerable<OrderBy> Order,
            int? Limit, int? Offset);
        IEnumerable<Story> GetStoriesForComic(GetStoriesForComic model);
        #endregion
        #region Creator methods
        [Obsolete("Use method with GetCreators object")]
        IEnumerable<Creator> GetCreators(
            string FirstName, string MiddleName, string LastName,
            string Suffix, string NameStartsWith, string FirstNameStartsWith,
            string MiddleNameStartsWith, string LastNameStartsWith,
            DateTime? ModifiedSince, IEnumerable<int> Comics,
            IEnumerable<int> Series, IEnumerable<int> Events,
            IEnumerable<int> Stories, IEnumerable<OrderBy> Order,
            int? Limit, int? Offset);
        IEnumerable<Creator> GetCreators(GetCreators model);
        Creator GetCreator(int CreatorId);
        [Obsolete("Use method with GetComicsForCreator object")]
        IEnumerable<Comic> GetComicsForCreator(
            int CreatorId, ComicFormat? Format, ComicFormatType? FormatType,
            bool? NoVariants, DateDescriptor? DateDescript,
            DateTime? DateRangeBegin, DateTime? DateRangeEnd,
            bool? HasDigitalIssue, DateTime? ModifiedSince,
            IEnumerable<int> Characters, IEnumerable<int> Series,
            IEnumerable<int> Events, IEnumerable<int> Stories,
            IEnumerable<int> SharedAppearances, IEnumerable<int> Collaborators,
            IEnumerable<OrderBy> Order, int? Limit, int? Offset);
        IEnumerable<Comic> GetComicsForCreator(GetComicsForCreator model);

        [Obsolete("Use method with GetEventsForCreator object")]
        IEnumerable<Event> GetEventsForCreator(
            int CreatorId,
            string Name,
            string NameStartsWith,
            DateTime? ModifiedSince,
            IEnumerable<int> Characters,
            IEnumerable<int> Series,
            IEnumerable<int> Comics,
            IEnumerable<int> Stories,
            IEnumerable<OrderBy> Order,
            int? Limit,
            int? Offset);
        IEnumerable<Event> GetEventsForCreator(GetEventsForCreator model);

        [Obsolete("Use method with GetSeriesForCreator object")]
        IEnumerable<Series> GetSeriesForCreator(
            int CreatorId,
            string Title = null,
            string TitleStartsWith = null,
            DateTime? ModifiedSince = null,
            IEnumerable<int> Comics = null,
            IEnumerable<int> Stories = null,
            IEnumerable<int> Events = null,
            IEnumerable<int> Characters = null,
            SeriesType? SeriesType = null,
            IEnumerable<ComicFormat> Contains = null,
            IEnumerable<OrderBy> Order = null,
            int? Limit = null,
            int? Offset = null);
        IEnumerable<Series> GetSeriesForCreator(GetSeriesForCreator model);

        [Obsolete("Use method with GetStoriesForCreator object")]
        IEnumerable<Story> GetStoriesForCreator(
            int CreatorId,
            DateTime? ModifiedSince = null,
            IEnumerable<int> Comics = null,
            IEnumerable<int> Series = null,
            IEnumerable<int> Events = null,
            IEnumerable<int> Characters = null,
            IEnumerable<OrderBy> Order = null,
            int? Limit = null,
            int? Offset = null);
        IEnumerable<Story> GetStoriesForCreator(GetStoriesForCreator model);
        #endregion
        #region Event methods
        [Obsolete("Use method with GetEvents object")]
        IEnumerable<Event> GetEvents(
            string Name = null, string NameStartsWith = null,
            DateTime? ModifiedSince = null,
            IEnumerable<int> Creators = null,
            IEnumerable<int> Characters = null,
            IEnumerable<int> Series = null,
            IEnumerable<int> Comics = null,
            IEnumerable<int> Stories = null,
            IEnumerable<OrderBy> Order = null,
            int? Limit = null,
            int? Offset = null);
        IEnumerable<Event> GetEvents(GetEvents model);
        Event GetEvent(int EventId);
        [Obsolete("Use method with GetCharactersForEvent object")]
        IEnumerable<Character> GetCharactersForEvent(
            int EventId,
            string Name = null, string NameStartsWith = null,
            DateTime? ModifiedSince = null,
            IEnumerable<int> Comics = null, IEnumerable<int> Series = null,
            IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null,
            int? Limit = null, int? Offset = null);
        IEnumerable<Character> GetCharactersForEvent(GetCharactersForEvent model);
        [Obsolete("Use method with GetComicsForEvent object")]
        IEnumerable<Comic> GetComicsForEvent(
            int EventId,
            ComicFormat? Format = null, ComicFormatType? FormatType = null,
            bool? NoVariants = null,
            DateDescriptor? DateDescript = null,
            DateTime? DateRangeBegin = null, DateTime? DateRangeEnd = null,
            bool? HasDigitalIssue = null, DateTime? ModifiedSince = null,
            IEnumerable<int> Creators = null, IEnumerable<int> Characters = null,
            IEnumerable<int> Series = null, IEnumerable<int> Events = null, // Weird to see this here
            IEnumerable<int> Stories = null, IEnumerable<int> SharedAppearances = null,
            IEnumerable<int> Collaborators = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null);
        IEnumerable<Comic> GetComicsForEvent(GetComicsForEvent model);
        [Obsolete("Use method with GetCreatorsForEvent object")]
        IEnumerable<Creator> GetCreatorsForEvent(
            int EventId,
            string FirstName = null, string MiddleName = null, string LastName = null, string Suffix = null,
            string NameStartsWith = null,
            string FirstNameStartsWith = null, string MiddleNameStartsWith = null, string LastNameStartsWith = null,
            DateTime? ModifiedSince = null,
            IEnumerable<int> Comics = null,
            IEnumerable<int> Series = null,
            IEnumerable<int> Stories = null,
            IEnumerable<OrderBy> Order = null,
            int? Limit = null,
            int? Offset = null);
        IEnumerable<Creator> GetCreatorsForEvent(GetCreatorsForEvent model);
        [Obsolete("Use method with GetSeriesForEvent object")]
        IEnumerable<Series> GetSeriesForEvent(int EventId,
                                            string Title = null,
                                            string TitleStartsWith = null,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Stories = null,
                                            IEnumerable<int> Creators = null,
                                            IEnumerable<int> Characters = null,
                                            SeriesType? SeriesType = null,
                                            IEnumerable<ComicFormat> Contains = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null);
        IEnumerable<Series> GetSeriesForEvent(GetSeriesForEvent model);
        [Obsolete("Use method with GetStoriesForEvent object")]
        IEnumerable<Story> GetStoriesForEvent(int EventId,
                                           DateTime? ModifiedSince = null,
                                           IEnumerable<int> Comics = null,
                                           IEnumerable<int> Series = null,
                                           IEnumerable<int> Creators = null,
                                           IEnumerable<int> Characters = null,
                                           IEnumerable<OrderBy> Order = null,
                                           int? Limit = null,
                                           int? Offset = null);
        IEnumerable<Story> GetStoriesForEvent(GetStoriesForEvent model);
        #endregion
        #region Series methods
        IEnumerable<Series> GetSeries(
            string Title = null,
            string TitleStartsWith = null,
            DateTime? ModifiedSince = null,
            IEnumerable<int> Comics = null,
            IEnumerable<int> Stories = null,
            IEnumerable<int> Events = null,
            IEnumerable<int> Creators = null,
            IEnumerable<int> Characters = null,
            SeriesType? Type = null,
            ComicFormat? Contains = null,
            IEnumerable<OrderBy> Order = null,
            int? Limit = null,
            int? Offset = null);
        Series GetSeries(int SeriesId);
        IEnumerable<Character> GetCharactersForSeries(
            int SeriesId,
            string Name = null, string NameStartsWith = null, DateTime? ModifiedSince = null,
            IEnumerable<int> Comics = null, IEnumerable<int> Events = null, IEnumerable<int> Stories = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null);
        IEnumerable<Character> GetCharactersForSeries(GetCharactersForSeries model);

        IEnumerable<Comic> GetComicsForSeries(
            int SeriesId,
            ComicFormat? Format = null, ComicFormatType? FormatType = null,
            bool? NoVariants = null, DateDescriptor? DateDescript = null,
            DateTime? DateRangeBegin = null, DateTime? DateRangeEnd = null,
            bool? HasDigitalIssue = null,
            DateTime? ModifiedSince = null,
            IEnumerable<int> Creators = null, IEnumerable<int> Characters = null,
            IEnumerable<int> Events = null, IEnumerable<int> Stories = null,
            IEnumerable<int> SharedAppearances = null, IEnumerable<int> Collaborators = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null);
        IEnumerable<Comic> GetComicsForSeries(GetComicsForSeries model);
        IEnumerable<Creator> GetCreatorsForSeries(
            int SeriesId,
            string FirstName = null, string MiddleName = null, string LastName = null, string Suffix = null,
            string NameStartsWith = null,
            string FirstNameStartsWith = null, string MiddleNameStartsWith = null, string LastNameStartsWith = null,
            DateTime? ModifiedSince = null,
            IEnumerable<int> Comics = null, IEnumerable<int> Events = null, IEnumerable<int> Stories = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null);
        IEnumerable<Creator> GetCreatorsForSeries(GetCreatorsForSeries model);
        IEnumerable<Event> GetEventsForSeries(
            int SeriesId,
            string Name = null,
            string NameStartsWith = null,
            DateTime? ModifiedSince = null,
            IEnumerable<int> Creators = null,
            IEnumerable<int> Characters = null,
            IEnumerable<int> Comics = null,
            IEnumerable<int> Stories = null,
            IEnumerable<OrderBy> Order = null,
            int? Limit = null,
            int? Offset = null);
        IEnumerable<Event> GetEventsForSeries(GetEventsForSeries model);
        IEnumerable<Story> GetStoriesForSeries(
            int SeriesId,
            DateTime? ModifiedSince = null,
            IEnumerable<int> Comics = null,
            IEnumerable<int> Events = null,
            IEnumerable<int> Creators = null,
            IEnumerable<int> Characters = null,
            IEnumerable<OrderBy> Order = null,
            int? Limit = null,
            int? Offset = null);
        IEnumerable<Story> GetStoriesForSeries(GetStoriesForSeries model);
        #endregion
        #region Story methods
        IEnumerable<Story> GetStories(
            DateTime? ModifiedSince = null,
            IEnumerable<int> Comics = null,
            IEnumerable<int> Series = null,
            IEnumerable<int> Events = null,
            IEnumerable<int> Creators = null,
            IEnumerable<int> Characters = null,
            IEnumerable<OrderBy> Order = null,
            int? Limit = null,
            int? Offset = null);
        IEnumerable<Story> GetStories(GetStories model);
        Story GetStory(int StoryId);
        IEnumerable<Character> GetCharactersForStory(
            int StoryId,
            string Name = null,
            string NameStartsWith = null,
            DateTime? ModifiedSince = null,
            IEnumerable<int> Comics = null,
            IEnumerable<int> Series = null,
            IEnumerable<int> Events = null,
            IEnumerable<OrderBy> Order = null,
            int? Limit = null,
            int? Offset = null);
        IEnumerable<Character> GetCharactersForStory(GetCharactersForStory model);
        IEnumerable<Comic> GetComicsForStory(
            int StoryId,
            ComicFormat? Format = null,
            ComicFormatType? FormatType = null,
            bool? NoVariants = null,
            DateDescriptor? DateDescript = null,
            DateTime? DateRangeBegin = null,
            DateTime? DateRangeEnd = null,
            bool? HasDigitalIssue = null,
            DateTime? ModifiedSince = null,
            IEnumerable<int> Creators = null,
            IEnumerable<int> Characters = null,
            IEnumerable<int> Series = null,
            IEnumerable<int> Events = null,
            IEnumerable<int> SharedAppearances = null,
            IEnumerable<int> Collaborators = null,
            IEnumerable<OrderBy> Order = null,
            int? Limit = null,
            int? Offset = null);
        IEnumerable<Comic> GetComicsForStory(GetComicsForStory model);
        IEnumerable<Creator> GetCreatorsForStory(
            int StoryId,
            string FirstName = null,
            string MiddleName = null,
            string LastName = null,
            string Suffix = null,
            string NameStartsWith = null,
            string FirstNameStartsWith = null,
            string MiddleNameStartsWith = null,
            string LastNameStartsWith = null,
            DateTime? ModifiedSince = null,
            IEnumerable<int> Comics = null,
            IEnumerable<int> Series = null,
            IEnumerable<int> Events = null,
            IEnumerable<OrderBy> Order = null,
            int? Limit = null,
            int? Offset = null);
        IEnumerable<Creator> GetCreatorsForStory(GetCreatorsForStory model);
        IEnumerable<Event> GetEventsForStories(
            int StoryId,
            string Name = null,
            string NameStartsWith = null,
            DateTime? ModifiedSince = null,
            IEnumerable<int> Creators = null,
            IEnumerable<int> Characters = null,
            IEnumerable<int> Series = null,
            IEnumerable<int> Comics = null,
            IEnumerable<OrderBy> Order = null,
            int? Limit = null,
            int? Offset = null);
        IEnumerable<Event> GetEventsForStory(GetEventsForStory model);
        IEnumerable<Series> GetSeriesForStory(GetSeriesForStory model);
        #endregion
    }
}
