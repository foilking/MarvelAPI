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

        private const string BASE_URL = "http://gateway.marvel.com/v1/public";

        public Marvel(string publicApiKey, string privateApiKey, bool? useGZip = null)
        {
            var client = new RestClient(BASE_URL);

            Characters =  new CharacterRequests(publicApiKey, privateApiKey, client, useGZip);
            Comics = new ComicRequests(publicApiKey, privateApiKey, client, useGZip);
            Creators = new CreatorRequests(publicApiKey, privateApiKey, client, useGZip);
            Events = new EventRequest(publicApiKey, privateApiKey, client, useGZip);
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
            return Characters.GetCharacters(new GetCharacters
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
            return Characters.GetComicsForCharacter(new GetComicsForCharacter
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

            throw new NotImplementedException();
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

        public IEnumerable<Story> GetStoriessForCharacter(int CharacterId, DateTime? ModifiedSince, IEnumerable<int> Comics, IEnumerable<int> Series, IEnumerable<int> Events, IEnumerable<int> Creators, IEnumerable<OrderBy> Order, int? Limit, int? Offset)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Comics
        public IEnumerable<Comic> GetComics(ComicFormat? Format, ComicFormatType? FormatType, bool? NoVariants, DateDescriptor? DateDescript, DateTime? DateRangeBegin, DateTime? DateRangeEnd, bool? HasDigitalIssue, DateTime? ModifiedSince, IEnumerable<int> Creators, IEnumerable<int> Characters, IEnumerable<int> Series, IEnumerable<int> Events, IEnumerable<int> Stories, IEnumerable<int> SharedAppearances, IEnumerable<int> Collaborators, IEnumerable<OrderBy> Order, int? Limit, int? Offset)
        {
            throw new NotImplementedException();
        }

        public Comic GetComic(int ComicId)
        {
            return Comics.GetComic(ComicId);
        }

        public IEnumerable<Character> GetCharactersForComic(int ComicId, string Name, string NameStartsWith, DateTime? ModifiedSince, IEnumerable<int> Series, IEnumerable<int> Events, IEnumerable<int> Stories, IEnumerable<OrderBy> Order, int? Limit, int? Offset)
        {
            throw new NotImplementedException();
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

        public IEnumerable<Event> GetEventsForComic(int ComicId, string Name, string NameStartsWith, DateTime? ModifiedSince, IEnumerable<int> Creators, IEnumerable<int> Characters, IEnumerable<int> Series, IEnumerable<int> Stories, IEnumerable<OrderBy> Order, int? Limit, int? Offset)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Story> GetStoriesForComic(int ComicId, DateTime? ModifiedSince, IEnumerable<int> Series, IEnumerable<int> Events, IEnumerable<int> Creators, IEnumerable<int> Characters, IEnumerable<OrderBy> Order, int? Limit, int? Offset)
        {
            throw new NotImplementedException();
        }
        #endregion

        public IEnumerable<Creator> GetCreators(string FirstName, string MiddleName, string LastName, string Suffix, string NameStartsWith, string FirstNameStartsWith, string MiddleNameStartsWith, string LastNameStartsWith, DateTime? ModifiedSince, IEnumerable<int> Comics, IEnumerable<int> Series, IEnumerable<int> Events, IEnumerable<int> Stories, IEnumerable<OrderBy> Order, int? Limit, int? Offset)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public IEnumerable<Event> GetEventsForCreator(int CreatorId, string Name, string NameStartsWith, DateTime? ModifiedSince, IEnumerable<int> Characters, IEnumerable<int> Series, IEnumerable<int> Comics, IEnumerable<int> Stories, IEnumerable<OrderBy> Order, int? Limit, int? Offset)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Series> GetSeriesForCreator(int CreatorId, string Title = null, string TitleStartsWith = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Stories = null, IEnumerable<int> Events = null, IEnumerable<int> Characters = null, SeriesType? SeriesType = null, IEnumerable<ComicFormat> Contains = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Story> GetStoriesForCreator(int CreatorId, 
            DateTime? ModifiedSince = null, 
            IEnumerable<int> Comics = null, IEnumerable<int> Series = null,
            IEnumerable<int> Events = null, IEnumerable<int> Characters = null, 
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Event> GetEventsForCharacter(GetEventsForCharacter model)
        {
            return Characters.GetEventsForCharacter(model);
        }

        public IEnumerable<Series> GetSeriesForCharacter(GetSeriesForCharacter model)
        {
            return Characters.GetSeriesForCharacter(model);
        }

        public IEnumerable<Story> GetStoriesForCharacter(GetStoriesForCharacter model)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Comic> GetComics(GetComics model)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Character> GetCharactersForComic(GetCharactersForComic model)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Creator> GetCreatorsForComic(GetCreatorsForComic model)
        {
            return Comics.GetCreatorsForComic(model);
        }

        public IEnumerable<Event> GetEventsForComic(GetEventsForComic model)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Story> GetStoriesForComic(GetStoriesForComic model)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Creator> GetCreators(GetCreators model)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Event> GetEvents(string Name = null, string NameStartsWith = null, DateTime? ModifiedSince = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<int> Series = null, IEnumerable<int> Comics = null, IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }

        public Event GetEvent(int EventId)
        {
            return Events.GetEvent(EventId);
        }

        public IEnumerable<Character> GetCharactersForEvent(int EventId, string Name = null, string NameStartsWith = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Comic> GetComicsForEvent(int EventId, ComicFormat? Format = null, ComicFormatType? FormatType = null, bool? NoVariants = null, DateDescriptor? DateDescript = null, DateTime? DateRangeBegin = null, DateTime? DateRangeEnd = null, bool? HasDigitalIssue = null, DateTime? ModifiedSince = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null, IEnumerable<int> Stories = null, IEnumerable<int> SharedAppearances = null, IEnumerable<int> Collaborators = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Creator> GetCreatorsForEvent(int EventId, string FirstName = null, string MiddleName = null, string LastName = null, string Suffix = null, string NameStartsWith = null, string FirstNameStartsWith = null, string MiddleNameStartsWith = null, string LastNameStartsWith = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Series> GetSeriesForEvent(int EventId, string Title = null, string TitleStartsWith = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Stories = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, SeriesType? SeriesType = null, IEnumerable<ComicFormat> Contains = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Story> GetStoriesForEvent(int EventId, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Series> GetSeries(string Title = null, string TitleStartsWith = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Stories = null, IEnumerable<int> Events = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, SeriesType? Type = null, ComicFormat? Contains = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }

        public Series GetSeries(int SeriesId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Character> GetCharactersForSeries(int SeriesId, string Name = null, string NameStartsWith = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Events = null, IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Comic> GetComicsForSeries(int SeriesId, ComicFormat? Format = null, ComicFormatType? FormatType = null, bool? NoVariants = null, DateDescriptor? DateDescript = null, DateTime? DateRangeBegin = null, DateTime? DateRangeEnd = null, bool? HasDigitalIssue = null, DateTime? ModifiedSince = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<int> Events = null, IEnumerable<int> Stories = null, IEnumerable<int> SharedAppearances = null, IEnumerable<int> Collaborators = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Creator> GetCreatorsForSeries(int SeriesId, string FirstName = null, string MiddleName = null, string LastName = null, string Suffix = null, string NameStartsWith = null, string FirstNameStartsWith = null, string MiddleNameStartsWith = null, string LastNameStartsWith = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Events = null, IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Event> GetEventsForSeries(int SeriesId, string Name = null, string NameStartsWith = null, DateTime? ModifiedSince = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<int> Comics = null, IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Story> GetStoriesForSeries(int SeriesId, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Events = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Story> GetStories(DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }

        public Story GetStory(int StoryId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Character> GetCharactersForStory(int StoryId, string Name = null, string NameStartsWith = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Comic> GetComicsForStory(int StoryId, ComicFormat? Format = null, ComicFormatType? FormatType = null, bool? NoVariants = null, DateDescriptor? DateDescript = null, DateTime? DateRangeBegin = null, DateTime? DateRangeEnd = null, bool? HasDigitalIssue = null, DateTime? ModifiedSince = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null, IEnumerable<int> SharedAppearances = null, IEnumerable<int> Collaborators = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Creator> GetCreatorsForStory(int StoryId, string FirstName = null, string MiddleName = null, string LastName = null, string Suffix = null, string NameStartsWith = null, string FirstNameStartsWith = null, string MiddleNameStartsWith = null, string LastNameStartsWith = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Event> GetEventsForStories(int StoryId, string Name = null, string NameStartsWith = null, DateTime? ModifiedSince = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<int> Series = null, IEnumerable<int> Comics = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            throw new NotImplementedException();
        }
    }
}