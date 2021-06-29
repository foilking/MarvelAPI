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

            Characters = new CharacterRequests(publicApiKey, privateApiKey, client, useGZip);
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
        public IEnumerable<Comic> GetComics(
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
            IEnumerable<int> Stories = null,
            IEnumerable<int> SharedAppearances = null,
            IEnumerable<int> Collaborators = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetComics(new GetComics
            {
                Format = Format,
                FormatType = FormatType,
                NoVariants = NoVariants,
                DateDescript = DateDescript,
                DateRangeBegin = DateRangeBegin,
                DateRangeEnd = DateRangeEnd,
                HasDigitalIssue = HasDigitalIssue,
                ModifiedSince = ModifiedSince,
                Creators = Creators,
                Characters = Characters,
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
        public IEnumerable<Comic> GetComics(GetComics model)
        {
            return Comics.GetComics(model);
        }

        public Comic GetComic(int ComicId)
        {
            return Comics.GetComic(ComicId);
        }

        [Obsolete("Use method with GetComics object")]
        public IEnumerable<Character> GetCharactersForComic(int ComicId,
            string Name = null, string NameStartsWith = null, DateTime? ModifiedSince = null,
            IEnumerable<int> Series = null, IEnumerable<int> Events = null, IEnumerable<int> Stories = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetCharactersForComic(new GetCharactersForComic
            {
                ComicId = ComicId,
                Name = Name,
                NameStartsWith = NameStartsWith,
                ModifiedSince = ModifiedSince,
                Series = Series,
                Events = Events,
                Stories = Stories,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
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

        [Obsolete("Use method with GetEventsForComic object")]
        public IEnumerable<Event> GetEventsForComic(int ComicId,
            string Name = null, string NameStartsWith = null, DateTime? ModifiedSince = null,
            IEnumerable<int> Creators = null, IEnumerable<int> Characters = null,
            IEnumerable<int> Series = null, IEnumerable<int> Stories = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetEventsForComic(new GetEventsForComic
            {
                ComicId = ComicId,
                Name = Name,
                NameStartsWith = NameStartsWith,
                ModifiedSince = ModifiedSince,
                Creators = Creators,
                Characters = Characters,
                Series = Series,
                Stories = Stories,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
        }

        public IEnumerable<Event> GetEventsForComic(GetEventsForComic model)
        {
            return Comics.GetEventsForComic(model);
        }

        [Obsolete("Use method with GetStoriesForComic object")]
        public IEnumerable<Story> GetStoriesForComic(int ComicId,
            DateTime? ModifiedSince = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null,
            IEnumerable<int> Creators = null, IEnumerable<int> Characters = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetStoriesForComic(new GetStoriesForComic
            {
                ComicId = ComicId,
                ModifiedSince = ModifiedSince,
                Series = Series,
                Events = Events,
                Creators = Creators,
                Characters = Characters,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
        }

        public IEnumerable<Story> GetStoriesForComic(GetStoriesForComic model)
        {
            return Comics.GetStoriesForComic(model);
        }
        #endregion

        #region Creators

        [Obsolete("Use method with GetCreators object")]
        public IEnumerable<Creator> GetCreators(
            string FirstName = null, string MiddleName = null, string LastName = null, string Suffix = null,
            string NameStartsWith = null, string FirstNameStartsWith = null, string MiddleNameStartsWith = null, string LastNameStartsWith = null,
            DateTime? ModifiedSince = null,
            IEnumerable<int> Comics = null, IEnumerable<int> Series = null,
            IEnumerable<int> Events = null, IEnumerable<int> Stories = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetCreators(new GetCreators
            {
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
                Events = Events,
                Stories = Stories,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
        }

        public IEnumerable<Creator> GetCreators(GetCreators model)
        {
            return Creators.GetCreators(model);
        }
        public Creator GetCreator(int CreatorId)
        {
            return Creators.GetCreator(CreatorId);
        }

        [Obsolete("Use method with GetComicsForCreator object")]
        public IEnumerable<Comic> GetComicsForCreator(int CreatorId,
            ComicFormat? Format = null, ComicFormatType? FormatType = null, bool? NoVariants = null,
            DateDescriptor? DateDescript = null, DateTime? DateRangeBegin = null, DateTime? DateRangeEnd = null,
            bool? HasDigitalIssue = null, DateTime? ModifiedSince = null,
            IEnumerable<int> Characters = null, IEnumerable<int> Series = null,
            IEnumerable<int> Events = null, IEnumerable<int> Stories = null,
            IEnumerable<int> SharedAppearances = null, IEnumerable<int> Collaborators = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetComicsForCreator(new GetComicsForCreator
            {
                CreatorId = CreatorId,
                Format = Format,
                FormatType = FormatType,
                NoVariants = NoVariants,
                DateDescript = DateDescript,
                DateRangeBegin = DateRangeBegin,
                DateRangeEnd = DateRangeEnd,
                HasDigitalIssue = HasDigitalIssue,
                ModifiedSince = ModifiedSince,
                Characters = Characters,
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
        public IEnumerable<Comic> GetComicsForCreator(GetComicsForCreator model)
        {
            return Creators.GetComicsForCreator(model);
        }

        [Obsolete("Use method with GetEventsForCreator object")]
        public IEnumerable<Event> GetEventsForCreator(int CreatorId,
            string Name = null, string NameStartsWith = null, DateTime? ModifiedSince = null,
            IEnumerable<int> Characters = null, IEnumerable<int> Series = null,
            IEnumerable<int> Comics = null, IEnumerable<int> Stories = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetEventsForCreator(new GetEventsForCreator
            {
                CreatorId = CreatorId,
                Name = Name,
                NameStartsWith = NameStartsWith,
                ModifiedSince = ModifiedSince,
                Characters = Characters,
                Series = Series,
                Comics = Comics,
                Stories = Stories,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
        }
        public IEnumerable<Event> GetEventsForCreator(GetEventsForCreator model)
        {
            return Creators.GetEventsForCreator(model);
        }

        [Obsolete("Use method with GetSeriesForCreator object")]
        public IEnumerable<Series> GetSeriesForCreator(int CreatorId,
            string Title = null, string TitleStartsWith = null, DateTime? ModifiedSince = null,
            IEnumerable<int> Comics = null, IEnumerable<int> Stories = null,
            IEnumerable<int> Events = null, IEnumerable<int> Characters = null,
            SeriesType? SeriesType = null, IEnumerable<ComicFormat> Contains = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetSeriesForCreator(new GetSeriesForCreator
            {
                CreatorId = CreatorId,
                Title = Title,
                TitleStartsWith = TitleStartsWith,
                ModifiedSince = ModifiedSince,
                Comics = Comics,
                Stories = Stories,
                Events = Events,
                Characters = Characters,
                SeriesType = SeriesType,
                Contains = Contains,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
        }
        public IEnumerable<Series> GetSeriesForCreator(GetSeriesForCreator model)
        {
            return Creators.GetSeriesForCreator(model);
        }

        [Obsolete("Use method with GetStoriesForCreator object")]
        public IEnumerable<Story> GetStoriesForCreator(int CreatorId,
            DateTime? ModifiedSince = null,
            IEnumerable<int> Comics = null, IEnumerable<int> Series = null,
            IEnumerable<int> Events = null, IEnumerable<int> Characters = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetStoriesForCreator(new GetStoriesForCreator
            {
                CreatorId = CreatorId,
                ModifiedSince = ModifiedSince,
                Comics = Comics,
                Series = Series,
                Events = Events,
                Characters = Characters,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
        }
        public IEnumerable<Story> GetStoriesForCreator(GetStoriesForCreator model)
        {
            return Creators.GetStoriesForCreator(model);
        }
        #endregion

        #region Events

        [Obsolete("Use method with GetEvents object")]
        public IEnumerable<Event> GetEvents(
            string Name = null, string NameStartsWith = null, DateTime? ModifiedSince = null,
            IEnumerable<int> Creators = null, IEnumerable<int> Characters = null,
            IEnumerable<int> Series = null, IEnumerable<int> Comics = null, IEnumerable<int> Stories = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetEvents(new GetEvents
            {
                Name = Name,
                NameStartsWith = NameStartsWith,
                ModifiedSince = ModifiedSince,
                Creators = Creators,
                Characters = Characters,
                Series = Series,
                Comics = Comics,
                Stories = Stories,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
        }
        public IEnumerable<Event> GetEvents(GetEvents model)
        {
            return Events.GetEvents(model);
        }

        public Event GetEvent(int EventId)
        {
            return Events.GetEvent(EventId);
        }

        [Obsolete("Use method with GetCharactersForEvent object")]
        public IEnumerable<Character> GetCharactersForEvent(int EventId,
            string Name = null, string NameStartsWith = null, DateTime? ModifiedSince = null,
            IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Stories = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetCharactersForEvent(new GetCharactersForEvent
            {
                EventId = EventId,
                Name = Name,
                NameStartsWith = NameStartsWith,
                ModifiedSince = ModifiedSince,
                Comics = Comics,
                Series = Series,
                Stories = Stories,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
        }
        public IEnumerable<Character> GetCharactersForEvent(GetCharactersForEvent model)
        {
            return Events.GetCharactersForEvent(model);
        }

        [Obsolete("Use method with GetComicsForEvent object")]
        public IEnumerable<Comic> GetComicsForEvent(int EventId,
            ComicFormat? Format = null, ComicFormatType? FormatType = null, bool? NoVariants = null,
            DateDescriptor? DateDescript = null, DateTime? DateRangeBegin = null, DateTime? DateRangeEnd = null,
            bool? HasDigitalIssue = null, DateTime? ModifiedSince = null,
            IEnumerable<int> Creators = null, IEnumerable<int> Characters = null,
            IEnumerable<int> Series = null, IEnumerable<int> Events = null,
            IEnumerable<int> Stories = null, IEnumerable<int> SharedAppearances = null,
            IEnumerable<int> Collaborators = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetComicsForEvent(new GetComicsForEvent
            {
                EventId = EventId,
                Format = Format,
                FormatType = FormatType,
                NoVariants = NoVariants,
                DateDescript = DateDescript,
                DateRangeBegin = DateRangeBegin,
                DateRangeEnd = DateRangeEnd,
                HasDigitalIssue = HasDigitalIssue,
                ModifiedSince = ModifiedSince,
                Creators = Creators,
                Characters = Characters,
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
        public IEnumerable<Comic> GetComicsForEvent(GetComicsForEvent model)
        {
            return Events.GetComicsForEvent(model);
        }

        [Obsolete("Use method with GetCreatorsForEvent object")]
        public IEnumerable<Creator> GetCreatorsForEvent(int EventId,
            string FirstName = null, string MiddleName = null, string LastName = null, string Suffix = null,
            string NameStartsWith = null,
            string FirstNameStartsWith = null, string MiddleNameStartsWith = null, string LastNameStartsWith = null,
            DateTime? ModifiedSince = null, IEnumerable<int> Comics = null,
            IEnumerable<int> Series = null, IEnumerable<int> Stories = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetCreatorsForEvent(new GetCreatorsForEvent
            {
                EventId = EventId,
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
        public IEnumerable<Creator> GetCreatorsForEvent(GetCreatorsForEvent model)
        {
            return Events.GetCreatorsForEvent(model);
        }

        [Obsolete("Use method with GetSeriesForEvent object")]
        public IEnumerable<Series> GetSeriesForEvent(int EventId,
            string Title = null, string TitleStartsWith = null, DateTime? ModifiedSince = null,
            IEnumerable<int> Comics = null, IEnumerable<int> Stories = null,
            IEnumerable<int> Creators = null, IEnumerable<int> Characters = null,
            SeriesType? SeriesType = null, IEnumerable<ComicFormat> Contains = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetSeriesForEvent(new GetSeriesForEvent
            {
                EventId = EventId,
                Title = Title,
                TitleStartsWith = TitleStartsWith,
                ModifiedSince = ModifiedSince,
                Comics = Comics,
                Stories = Stories,
                Creators = Creators,
                Characters = Characters,
                SeriesType = SeriesType,
                Contains = Contains,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
        }
        public IEnumerable<Series> GetSeriesForEvent(GetSeriesForEvent model)
        {
            return Events.GetSeriesForEvent(model);
        }

        [Obsolete("Use method with GetStoriesForEvent object")]
        public IEnumerable<Story> GetStoriesForEvent(int EventId, DateTime? ModifiedSince = null,
            IEnumerable<int> Comics = null, IEnumerable<int> Series = null,
            IEnumerable<int> Creators = null, IEnumerable<int> Characters = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetStoriesForEvent(new GetStoriesForEvent
            {
                EventId = EventId,
                ModifiedSince = ModifiedSince,
                Comics = Comics,
                Series = Series,
                Creators = Creators,
                Characters = Characters,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
        }
        public IEnumerable<Story> GetStoriesForEvent(GetStoriesForEvent model)
        {
            return Events.GetStoriesForEvent(model);
        }
        #endregion

        #region Series
        [Obsolete("Use method with GetSeries object")]
        public IEnumerable<Series> GetSeries(
            string Title = null, string TitleStartsWith = null, DateTime? ModifiedSince = null,
            IEnumerable<int> Comics = null, IEnumerable<int> Stories = null, IEnumerable<int> Events = null,
            IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, SeriesType? Type = null,
            ComicFormat? Contains = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetSeries(new GetSeries
            {
                Title = Title,
                TitleStartsWith = TitleStartsWith,
                ModifiedSince = ModifiedSince,
                Comics = Comics,
                Stories = Stories,
                Events = Events,
                Creators = Creators,
                Characters = Characters,
                Type = Type,
                Contains = Contains,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
        }
        public IEnumerable<Series> GetSeries(GetSeries model)
        {
            return Series.GetSeries(model);
        }

        public Series GetSeries(int SeriesId)
        {
            return Series.GetSeries(SeriesId);
        }

        [Obsolete("Use method with GetCharactersForSeries object")]
        public IEnumerable<Character> GetCharactersForSeries(int SeriesId,
            string Name = null, string NameStartsWith = null, DateTime? ModifiedSince = null,
            IEnumerable<int> Comics = null, IEnumerable<int> Events = null, IEnumerable<int> Stories = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetCharactersForSeries(new GetCharactersForSeries
            {
                SeriesId = SeriesId,
                Name = Name,
                NameStartsWith = NameStartsWith,
                ModifiedSince = ModifiedSince,
                Comics = Comics,
                Events = Events,
                Stories = Stories,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
        }
        public IEnumerable<Character> GetCharactersForSeries(GetCharactersForSeries model)
        {
            return Series.GetCharactersForSeries(model);
        }

        [Obsolete("Use method with GetComicsForSeries object")]
        public IEnumerable<Comic> GetComicsForSeries(int SeriesId,
            ComicFormat? Format = null, ComicFormatType? FormatType = null, bool? NoVariants = null,
            DateDescriptor? DateDescript = null, DateTime? DateRangeBegin = null, DateTime? DateRangeEnd = null,
            bool? HasDigitalIssue = null, DateTime? ModifiedSince = null,
            IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<int> Events = null,
            IEnumerable<int> Stories = null, IEnumerable<int> SharedAppearances = null, IEnumerable<int> Collaborators = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetComicsForSeries(new GetComicsForSeries
            {
                SeriesId = SeriesId,
                Format = Format,
                FormatType = FormatType,
                NoVariants = NoVariants,
                DateDescript = DateDescript,
                DateRangeBegin = DateRangeBegin,
                DateRangeEnd = DateRangeEnd,
                HasDigitalIssue = HasDigitalIssue,
                ModifiedSince = ModifiedSince,
                Creators = Creators,
                Characters = Characters,
                Events = Events,
                Stories = Stories,
                SharedAppearances = SharedAppearances,
                Collaborators = Collaborators,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
        }
        public IEnumerable<Comic> GetComicsForSeries(GetComicsForSeries model)
        {
            return Series.GetComicsForSeries(model);
        }

        [Obsolete("Use method with GetCreatorsForSeries object")]
        public IEnumerable<Creator> GetCreatorsForSeries(int SeriesId,
            string FirstName = null, string MiddleName = null, string LastName = null, string Suffix = null,
            string NameStartsWith = null, string FirstNameStartsWith = null, string MiddleNameStartsWith = null,
            string LastNameStartsWith = null, DateTime? ModifiedSince = null,
            IEnumerable<int> Comics = null, IEnumerable<int> Events = null, IEnumerable<int> Stories = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetCreatorsForSeries(new GetCreatorsForSeries
            {
                SeriesId = SeriesId,
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
                Events = Events,
                Stories = Stories,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
        }
        public IEnumerable<Creator> GetCreatorsForSeries(GetCreatorsForSeries model)
        {
            return Series.GetCreatorsForSeries(model);
        }

        [Obsolete("Use method with GetEventsForSeries object")]
        public IEnumerable<Event> GetEventsForSeries(int SeriesId,
            string Name = null, string NameStartsWith = null, DateTime? ModifiedSince = null,
            IEnumerable<int> Creators = null, IEnumerable<int> Characters = null,
            IEnumerable<int> Comics = null, IEnumerable<int> Stories = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetEventsForSeries(new GetEventsForSeries
            {
                SeriesId = SeriesId,
                Name = Name,
                NameStartsWith = NameStartsWith,
                ModifiedSince = ModifiedSince,
                Creators = Creators,
                Characters = Characters,
                Comics = Comics,
                Stories = Stories,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
        }
        public IEnumerable<Event> GetEventsForSeries(GetEventsForSeries model)
        {
            return Series.GetEventsForSeries(model);
        }

        [Obsolete("Use method with GetStoriesForSeries object")]
        public IEnumerable<Story> GetStoriesForSeries(int SeriesId, DateTime? ModifiedSince = null,
            IEnumerable<int> Comics = null, IEnumerable<int> Events = null,
            IEnumerable<int> Creators = null, IEnumerable<int> Characters = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetStoriesForSeries(new GetStoriesForSeries
            {
                SeriesId = SeriesId,
                ModifiedSince = ModifiedSince,
                Comics = Comics,
                Events = Events,
                Creators = Creators,
                Characters = Characters,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
        }
        public IEnumerable<Story> GetStoriesForSeries(GetStoriesForSeries model)
        {
            return Series.GetStoriesForSeries(model);
        }
        #endregion

        #region Story
        [Obsolete("Use method with GetStories object")]
        public IEnumerable<Story> GetStories(DateTime? ModifiedSince = null,
            IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null,
            IEnumerable<int> Creators = null, IEnumerable<int> Characters = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetStories(new GetStories
            {
                ModifiedSince = ModifiedSince,
                Comics = Comics,
                Series = Series,
                Events = Events,
                Creators = Creators,
                Characters = Characters,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
        }
        public IEnumerable<Story> GetStories(GetStories model)
        {
            return Stories.GetStories(model);
        }

        public Story GetStory(int StoryId)
        {
            return Stories.GetStory(StoryId);
        }

        [Obsolete("Use method with GetCharactersForStory object")]
        public IEnumerable<Character> GetCharactersForStory(int StoryId, string Name = null, string NameStartsWith = null,
            DateTime? ModifiedSince = null,
            IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetCharactersForStory(new GetCharactersForStory
            {
                StoryId = StoryId,
                Name = Name,
                NameStartsWith = NameStartsWith,
                ModifiedSince = ModifiedSince,
                Comics = Comics,
                Series = Series,
                Events = Events,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
        }
        public IEnumerable<Character> GetCharactersForStory(GetCharactersForStory model)
        {
            return Stories.GetCharactersForStory(model);
        }

        [Obsolete("Use method with GetComicsForStory object")]
        public IEnumerable<Comic> GetComicsForStory(int StoryId, ComicFormat? Format = null, ComicFormatType? FormatType = null,
            bool? NoVariants = null, DateDescriptor? DateDescript = null, DateTime? DateRangeBegin = null, DateTime? DateRangeEnd = null,
            bool? HasDigitalIssue = null, DateTime? ModifiedSince = null,
            IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<int> Series = null,
            IEnumerable<int> Events = null,
            IEnumerable<int> SharedAppearances = null, IEnumerable<int> Collaborators = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetComicsForStory(new GetComicsForStory
            {
                StoryId = StoryId,
                Format = Format,
                FormatType = FormatType,
                NoVariants = NoVariants,
                DateDescript = DateDescript,
                DateRangeBegin = DateRangeBegin,
                DateRangeEnd = DateRangeEnd,
                HasDigitalIssue = HasDigitalIssue,
                ModifiedSince = ModifiedSince,
                Creators = Creators,
                Characters = Characters,
                Series = Series,
                Events = Events,
                SharedAppearances = SharedAppearances,
                Collaborators = Collaborators,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
        }
        public IEnumerable<Comic> GetComicsForStory(GetComicsForStory model)
        {
            return Stories.GetComicsForStory(model);
        }

        [Obsolete("Use method with GetCreatorsForStory object")]
        public IEnumerable<Creator> GetCreatorsForStory(int StoryId,
            string FirstName = null, string MiddleName = null, string LastName = null, string Suffix = null,
            string NameStartsWith = null, string FirstNameStartsWith = null, string MiddleNameStartsWith = null, string LastNameStartsWith = null,
            DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetCreatorsForStory(new GetCreatorsForStory
            {
                StoryId = StoryId,
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
                Events = Events,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
        }
        public IEnumerable<Creator> GetCreatorsForStory(GetCreatorsForStory model)
        {
            return Stories.GetCreatorsForStory(model);
        }

        [Obsolete("Use method with GetEventsForStory object")]
        public IEnumerable<Event> GetEventsForStories(int StoryId, string Name = null, string NameStartsWith = null,
            DateTime? ModifiedSince = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null,
            IEnumerable<int> Series = null, IEnumerable<int> Comics = null,
            IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)
        {
            return GetEventsForStory(new GetEventsForStory
            {
                StoryId = StoryId,
                Name = Name,
                NameStartsWith = NameStartsWith,
                ModifiedSince = ModifiedSince,
                Creators = Creators,
                Characters = Characters,
                Series = Series,
                Comics = Comics,
                Order = Order,
                Limit = Limit,
                Offset = Offset
            });
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