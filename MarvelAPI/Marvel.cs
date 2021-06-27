using MarvelAPI.Parameters;
using MarvelAPI.Requests;
using RestSharp;
using System;
using System.Collections.Generic;

namespace MarvelAPI
{
    public class Marvel : IMarvel
    {
        internal CharacterRequests Characters { get; }
        internal ComicRequests Comics { get; }
        internal CreatorRequests Creators { get; }
        internal EventRequest Events { get; }
        internal SeriesRequest Series { get; }
        internal StoryRequest Stories { get; }

        private const string BASE_URL = "http://gateway.marvel.com/v1/public";

        public Marvel(string publicApiKey, string privateApiKey, bool? useGZip = null)
        {
            var client = new RestClient(BASE_URL);

            Characters =  new CharacterRequests(publicApiKey, privateApiKey, client, useGZip);
            Comics = new ComicRequests(publicApiKey, privateApiKey, client, useGZip);
            Creators = new CreatorRequests(publicApiKey, privateApiKey, client, useGZip);
            Events = new EventRequest(publicApiKey, privateApiKey, client, useGZip);
            Series = new SeriesRequest(publicApiKey, privateApiKey, client, useGZip);
            Stories = new StoryRequest(publicApiKey, privateApiKey, client, useGZip);
        }

        #region Characters
        [Obsolete("Use method with GetCharacter object")]
        public IEnumerable<Character> GetCharacters(
            string Name = null,
            string NameStartsWith = null,
            DateTime? ModifiedSince = null,
            IEnumerable<int> Comics = null,
            IEnumerable<int> Series = null,
            IEnumerable<int> Events = null,
            IEnumerable<int> Stories = null,
            IEnumerable<OrderBy> Order = null,
            int? Limit = null,
            int? Offset = null)
        {
            return GetCharacters(new GetCharacters
            {
                Name = Name,
                NameStartsWith = NameStartsWith,
                ModifiedSince = ModifiedSince,
                Comics = Comics,
                Series = Series,
                Events = Events,
                Stories = Stories,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
        }

        public IEnumerable<Character> GetCharacters(GetCharacters model)
        {
            return Characters.GetCharacters(model);
        }

        [Obsolete("Use method with GetComicsForCharacter object")]
        public IEnumerable<Comic> GetComicsForCharacter(
            int CharacterId,
            ComicFormat? Format = null,
            ComicFormatType? FormatType = null,
            bool? NoVariants = null,
            DateDescriptor? DateDescript = null,
            DateTime? DateRangeBegin = null,
            DateTime? DateRangeEnd = null,
            bool? HasDigitalIssue = null,
            DateTime? ModifiedSince = null,
            IEnumerable<int> Creators = null,
            IEnumerable<int> Series = null,
            IEnumerable<int> Events = null,
            IEnumerable<int> Stories = null,
            IEnumerable<int> SharedAppearances = null,
            IEnumerable<int> Collaborators = null,
            IEnumerable<OrderBy> Order = null,
            int? Limit = null,
            int? Offset = null)
        {
            return GetComicsForCharacter(new GetComicsForCharacter
            {
                CharacterId = CharacterId,
                Format = Format,
                FormatType = FormatType,
                NoVariants = NoVariants,
                DateDescript = DateDescript,
                DateRangeBegin = DateRangeBegin,
                DateRangeEnd = DateRangeEnd,
                HasDigitalIssue = HasDigitalIssue,
                ModifiedSince = ModifiedSince,
                Creators = Creators,
                Series = Series,
                Events = Events,
                Stories = Stories,
                SharedAppearances = SharedAppearances,
                Collaborators = Collaborators,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
        }

        public IEnumerable<Comic> GetComicsForCharacter(GetComicsForCharacter model)
        {
            return Characters.GetComicsForCharacter(model);
        }

        [Obsolete("Use method with GetEventsForCharacter object")]
        public IEnumerable<Event> GetEventsForCharacter(int CharacterId,
            string Name = null,
            string NameStartsWith = null,
            DateTime? ModifiedSince = null,
            IEnumerable<int> Creators = null,
            IEnumerable<int> Series = null,
            IEnumerable<int> Comics = null,
            IEnumerable<int> Stories = null,
            IEnumerable<OrderBy> Order = null,
            int? Limit = null,
            int? Offset = null)
        {
            return GetEventsForCharacter(new GetEventsForCharacter 
            { 
                CharacterId = CharacterId,
                Name = Name,
                NameStartsWith = NameStartsWith,
                ModifiedSince = ModifiedSince,
                Creators = Creators,
                Series = Series,
                Comics = Comics,
                Stories = Stories,
                Order = Order, 
                Limit = Limit,
                Offset = Offset
            });
        }
        public IEnumerable<Event> GetEventsForCharacter(GetEventsForCharacter model)
        {
            return Characters.GetEventsForCharacter(model);
        }

        public Character GetCharacter(int CharacterId)
        {
            return Characters.GetCharacter(CharacterId);
        }

        [Obsolete("Use method with GetSeriesForCharacter object")]
        public IEnumerable<Series> GetSeriesForCharacter(int CharacterId, 
            string Title = null, string TitleStartsWith = null, 
            DateTime? ModifiedSince = null, 
            IEnumerable<int> Comics = null, IEnumerable<int> Stories = null, 
            IEnumerable<int> Events = null, IEnumerable<int> Creators = null, 
            SeriesType? SeriesType = null, IEnumerable<ComicFormat> Contains = null, 
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetSeriesForCharacter(new GetSeriesForCharacter
            {
                CharacterId = CharacterId,
                Title = Title,
                TitleStartsWith = TitleStartsWith,
                ModifiedSince = ModifiedSince,
                Comics = Comics,
                Stories = Stories,
                Events = Events,
                Creators = Creators,
                SeriesType = SeriesType,
                Contains = Contains,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
        }

        public IEnumerable<Series> GetSeriesForCharacter(GetSeriesForCharacter model)
        {
            return Characters.GetSeriesForCharacter(model);
        }

        [Obsolete("Use method with GetStoriesForCharacter object")]
        public IEnumerable<Story> GetStoriessForCharacter(
            int CharacterId, 
            DateTime? ModifiedSince = null, 
            IEnumerable<int> Comics = null, IEnumerable<int> Series = null, 
            IEnumerable<int> Events = null, IEnumerable<int> Creators = null, 
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetStoriesForCharacter(new GetStoriesForCharacter
            {
                CharacterId = CharacterId,
                ModifiedSince = ModifiedSince,
                Comics = Comics,
                Series = Series,
                Events = Events,
                Creators = Creators,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
        }
        public IEnumerable<Story> GetStoriesForCharacter(GetStoriesForCharacter model)
        {
            return Characters.GetStoriesForCharacter(model);
        }
        #endregion

        #region Comics
        [Obsolete("Use method with GetComics object")]
        public IEnumerable<Comic> GetComics(ComicFormat? Format, ComicFormatType? FormatType, bool? NoVariants, DateDescriptor? DateDescript, DateTime? DateRangeBegin, DateTime? DateRangeEnd, bool? HasDigitalIssue, DateTime? ModifiedSince, IEnumerable<int> Creators, IEnumerable<int> Characters, IEnumerable<int> Series, IEnumerable<int> Events, IEnumerable<int> Stories, IEnumerable<int> SharedAppearances, IEnumerable<int> Collaborators, IEnumerable<OrderBy> Order, int? Limit, int? Offset)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Comic> GetComics(GetComics model)
        {
            return Comics.GetComics(model);
        }
        
        public Comic GetComic(int ComicId)
        {
            return Comics.GetComic(ComicId);
        }

        [Obsolete("Use method with GetComics object")]
        public IEnumerable<Character> GetCharactersForComic(int ComicId, string Name, string NameStartsWith, DateTime? ModifiedSince, IEnumerable<int> Series, IEnumerable<int> Events, IEnumerable<int> Stories, IEnumerable<OrderBy> Order, int? Limit, int? Offset)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Character> GetCharactersForComic(GetCharactersForComic model)
        {
            return Comics.GetCharactersForComic(model);
        }

        [Obsolete("Use method with GetCreatorsForComic object")]
        public IEnumerable<Creator> GetCreatorsForComic(int ComicId, 
            string FirstName = null, string MiddleName = null, string LastName = null, 
            string Suffix = null, string NameStartsWith = null, 
            string FirstNameStartsWith = null, string MiddleNameStartsWith = null, string LastNameStartsWith = null, 
            DateTime? ModifiedSince = null, 
            IEnumerable<int> Comics = null, 
            IEnumerable<int> Series = null, 
            IEnumerable<int> Stories = null, 
            IEnumerable<OrderBy> Order = null, 
            int? Limit = null, int? Offset = null)
        {
            return GetCreatorsForComic(new GetCreatorsForComic
            {
                ComicId = ComicId,
                FirstName = FirstName,
                MiddleName = MiddleName,
                LastName = LastName,
                Suffix = Suffix,
                NameStartsWith = NameStartsWith,
                FirstNameStartsWith = FirstNameStartsWith,
                MiddleNameStartsWith = MiddleNameStartsWith,
                LastNameStartsWith = LastNameStartsWith,
                ModifiedSince = ModifiedSince,
                Comics = Comics,
                Series = Series,
                Stories = Stories,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
        }
        public IEnumerable<Creator> GetCreatorsForComic(GetCreatorsForComic model)
        {
            return Comics.GetCreatorsForComic(model);
        }

        public IEnumerable<Event> GetEventsForComic(int ComicId, string Name, string NameStartsWith, DateTime? ModifiedSince, IEnumerable<int> Creators, IEnumerable<int> Characters, IEnumerable<int> Series, IEnumerable<int> Stories, IEnumerable<OrderBy> Order, int? Limit, int? Offset)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Event> GetEventsForComic(GetEventsForComic model)
        {
            return Comics.GetEventsForComic(model);
        }

        public IEnumerable<Story> GetStoriesForComic(int ComicId, DateTime? ModifiedSince, IEnumerable<int> Series, IEnumerable<int> Events, IEnumerable<int> Creators, IEnumerable<int> Characters, IEnumerable<OrderBy> Order, int? Limit, int? Offset)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Story> GetStoriesForComic(GetStoriesForComic model)
        {
            return Comics.GetStoriesForComic(model);
        }
        #endregion

        #region Creators
        public IEnumerable<Creator> GetCreators(string FirstName, string MiddleName, string LastName, string Suffix, string NameStartsWith, string FirstNameStartsWith, string MiddleNameStartsWith, string LastNameStartsWith, DateTime? ModifiedSince, IEnumerable<int> Comics, IEnumerable<int> Series, IEnumerable<int> Events, IEnumerable<int> Stories, IEnumerable<OrderBy> Order, int? Limit, int? Offset)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Creator> GetCreators(GetCreators model)
        {
            return Creators.GetCreators(model);
        }
        public Creator GetCreator(int CreatorId)
        {
            return Creators.GetCreator(CreatorId);
        }

        public IEnumerable<Comic> GetComicsForCreator(int CreatorId, ComicFormat? Format, ComicFormatType? FormatType, bool? NoVariants, DateDescriptor? DateDescript, DateTime? DateRangeBegin, DateTime? DateRangeEnd, bool? HasDigitalIssue, DateTime? ModifiedSince, IEnumerable<int> Characters, IEnumerable<int> Series, IEnumerable<int> Events, IEnumerable<int> Stories, IEnumerable<int> SharedAppearances, IEnumerable<int> Collaborators, IEnumerable<OrderBy> Order, int? Limit, int? Offset)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Comic> GetComicsForCreator(GetComicsForCreator model)
        {
            return Creators.GetComicsForCreator(model);
        }

        public IEnumerable<Event> GetEventsForCreator(int CreatorId, string Name, string NameStartsWith, DateTime? ModifiedSince, IEnumerable<int> Characters, IEnumerable<int> Series, IEnumerable<int> Comics, IEnumerable<int> Stories, IEnumerable<OrderBy> Order, int? Limit, int? Offset)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Event> GetEventsForCreator(GetEventsForCreator model)
        {
            return Creators.GetEventsForCreator(model);
        }

        public IEnumerable<Series> GetSeriesForCreator(int CreatorId, string Title = null, string TitleStartsWith = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Stories = null, IEnumerable<int> Events = null, IEnumerable<int> Characters = null, SeriesType? SeriesType = null, IEnumerable<ComicFormat> Contains = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Series> GetSeriesForCreator(GetSeriesForCreator model)
        {
            return Creators.GetSeriesForCreator(model);
        }

        public IEnumerable<Story> GetStoriesForCreator(int CreatorId, 
            DateTime? ModifiedSince = null, 
            IEnumerable<int> Comics = null, IEnumerable<int> Series = null,
            IEnumerable<int> Events = null, IEnumerable<int> Characters = null, 
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Story> GetStoriesForCreator(GetStoriesForCreator model)
        {
            return Creators.GetStoriesForCreator(model);
        }
        #endregion

        #region Events

        public IEnumerable<Event> GetEvents(string Name = null, string NameStartsWith = null, DateTime? ModifiedSince = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<int> Series = null, IEnumerable<int> Comics = null, IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Event> GetEvents(GetEvents model)
        {
            return Events.GetEvents(model);
        }

        public Event GetEvent(int EventId)
        {
            return Events.GetEvent(EventId);
        }

        public IEnumerable<Character> GetCharactersForEvent(int EventId, string Name = null, string NameStartsWith = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Character> GetCharactersForEvent(GetCharactersForEvent model)
        {
            return Events.GetCharactersForEvent(model);
        }

        public IEnumerable<Comic> GetComicsForEvent(int EventId, ComicFormat? Format = null, ComicFormatType? FormatType = null, bool? NoVariants = null, DateDescriptor? DateDescript = null, DateTime? DateRangeBegin = null, DateTime? DateRangeEnd = null, bool? HasDigitalIssue = null, DateTime? ModifiedSince = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null, IEnumerable<int> Stories = null, IEnumerable<int> SharedAppearances = null, IEnumerable<int> Collaborators = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Comic> GetComicsForEvent(GetComicsForEvent model)
        {
            return Events.GetComicsForEvent(model);
        }

        public IEnumerable<Creator> GetCreatorsForEvent(int EventId, string FirstName = null, string MiddleName = null, string LastName = null, string Suffix = null, string NameStartsWith = null, string FirstNameStartsWith = null, string MiddleNameStartsWith = null, string LastNameStartsWith = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Creator> GetCreatorsForEvent(GetCreatorsForEvent model)
        {
            return Events.GetCreatorsForEvent(model);
        }

        public IEnumerable<Series> GetSeriesForEvent(int EventId, string Title = null, string TitleStartsWith = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Stories = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, SeriesType? SeriesType = null, IEnumerable<ComicFormat> Contains = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Series> GetSeriesForEvent(GetSeriesForEvent model)
        {
            return Events.GetSeriesForEvent(model);
        }

        public IEnumerable<Story> GetStoriesForEvent(int EventId, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Story> GetStoriesForEvent(GetStoriesForEvent model)
        {
            return Events.GetStoriesForEvent(model);
        }
        #endregion

        #region Series
        public IEnumerable<Series> GetSeries(string Title = null, string TitleStartsWith = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Stories = null, IEnumerable<int> Events = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, SeriesType? Type = null, ComicFormat? Contains = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Series> GetSeries(GetSeries model)
        {
            return Series.GetSeries(model);
        }

        public Series GetSeries(int SeriesId)
        {
            return Series.GetSeries(SeriesId);
        }

        public IEnumerable<Character> GetCharactersForSeries(int SeriesId, string Name = null, string NameStartsWith = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Events = null, IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Character> GetCharactersForSeries(GetCharactersForSeries model)
        {
            return Series.GetCharactersForSeries(model);
        }

        public IEnumerable<Comic> GetComicsForSeries(int SeriesId, ComicFormat? Format = null, ComicFormatType? FormatType = null, bool? NoVariants = null, DateDescriptor? DateDescript = null, DateTime? DateRangeBegin = null, DateTime? DateRangeEnd = null, bool? HasDigitalIssue = null, DateTime? ModifiedSince = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<int> Events = null, IEnumerable<int> Stories = null, IEnumerable<int> SharedAppearances = null, IEnumerable<int> Collaborators = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Comic> GetComicsForSeries(GetComicsForSeries model)
        {
            return Series.GetComicsForSeries(model);
        }

        public IEnumerable<Creator> GetCreatorsForSeries(int SeriesId, string FirstName = null, string MiddleName = null, string LastName = null, string Suffix = null, string NameStartsWith = null, string FirstNameStartsWith = null, string MiddleNameStartsWith = null, string LastNameStartsWith = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Events = null, IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Creator> GetCreatorsForSeries(GetCreatorsForSeries model)
        {
            return Series.GetCreatorsForSeries(model);
        }

        public IEnumerable<Event> GetEventsForSeries(int SeriesId, string Name = null, string NameStartsWith = null, DateTime? ModifiedSince = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<int> Comics = null, IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Event> GetEventsForSeries(GetEventsForSeries model)
        {
            return Series.GetEventsForSeries(model);
        }

        public IEnumerable<Story> GetStoriesForSeries(int SeriesId, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Events = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Story> GetStoriesForSeries(GetStoriesForSeries model)
        {
            return Series.GetStoriesForSeries(model);
        }
        #endregion

        #region Story
        public IEnumerable<Story> GetStories(DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Story> GetStories(GetStories model)
        {
            return Stories.GetStories(model);
        }

        public Story GetStory(int StoryId)
        {
            return Stories.GetStory(StoryId);
        }

        public IEnumerable<Character> GetCharactersForStory(int StoryId, string Name = null, string NameStartsWith = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Character> GetCharactersForStory(GetCharactersForStory model)
        {
            return Stories.GetCharactersForStory(model);
        }

        public IEnumerable<Comic> GetComicsForStory(int StoryId, ComicFormat? Format = null, ComicFormatType? FormatType = null, bool? NoVariants = null, DateDescriptor? DateDescript = null, DateTime? DateRangeBegin = null, DateTime? DateRangeEnd = null, bool? HasDigitalIssue = null, DateTime? ModifiedSince = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null, IEnumerable<int> SharedAppearances = null, IEnumerable<int> Collaborators = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Comic> GetComicsForStory(GetComicsForStory model)
        {
            return Stories.GetComicsForStory(model);
        }

        public IEnumerable<Creator> GetCreatorsForStory(int StoryId, string FirstName = null, string MiddleName = null, string LastName = null, string Suffix = null, string NameStartsWith = null, string FirstNameStartsWith = null, string MiddleNameStartsWith = null, string LastNameStartsWith = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Creator> GetCreatorsForStory(GetCreatorsForStory model)
        {
            return Stories.GetCreatorsForStory(model);
        }

        public IEnumerable<Event> GetEventsForStories(int StoryId, string Name = null, string NameStartsWith = null, DateTime? ModifiedSince = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<int> Series = null, IEnumerable<int> Comics = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Event> GetEventsForStory(GetEventsForStory model)
        {
            return Stories.GetEventsForStory(model);
        }
        public IEnumerable<Series> GetSeriesForStory(GetSeriesForStory model)
        {
            return Stories.GetSeriesForStory(model);
        }
        #endregion
    }
}