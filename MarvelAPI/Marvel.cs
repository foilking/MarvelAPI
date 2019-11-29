using MarvelAPI.Exceptions;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;

namespace MarvelAPI
{
    /* TODO SECTION
     * 
     * TODO: TESTS
     * TODO: Handle Etags
     * 
    */

    public class Marvel
    {
        private const string BASE_URL = "http://gateway.marvel.com/v1/public";
        private string _publicApiKey { get; set; }
        private string _privateApiKey { get; set; }
        private bool _useGZip { get; set; }
        private readonly RestClient _client;

        public Marvel(string publicApiKey, string privateApiKey, bool? useGZip = null)
        {
            _publicApiKey = publicApiKey;
            _privateApiKey = privateApiKey;
            _useGZip = useGZip.HasValue ? useGZip.Value : false;

            _client = new RestClient(BASE_URL);
        }

        private string CreateHash(string input)
        {
            var hash = string.Empty;
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                string hashText = data.ToString();

                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                hash = sBuilder.ToString();
            }

            return hash;
        }

        private RestRequest CreateRequest(string requestUrl)
        {
            var request = new RestRequest(requestUrl, Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();

            string hashVal = CreateHash($"{timestamp}{_privateApiKey}{_publicApiKey}");

            request.AddParameter("apikey", _publicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", hashVal);

            StringBuilder urlBuilder = new StringBuilder();
            urlBuilder.Append(BASE_URL);
            urlBuilder.Append(requestUrl);
            urlBuilder.Append($"ts={timestamp}&");
            urlBuilder.Append($"apikey={_publicApiKey}&");
            urlBuilder.Append($"hash={hashVal}");

            string url = urlBuilder.ToString();


            if (_useGZip)
            {
                request.AddHeader("Accept-Encoded", "gzip");
            }
            else
            {
                request.AddHeader("Accept", "*/*");
            }

            return request;
        }

        private void HandleResponseErrors<T>(IRestResponse<T> response)
        {
            var code = 0;
            var status = string.Empty;
            var responseStatus = response.ResponseStatus;
            if (responseStatus == ResponseStatus.Error)
            {
                var content = JsonConvert.DeserializeObject<MarvelError>(response.Content);
                switch (content.Code)
                {
                    case "InvalidCredentials":
                        code = 401;
                        status = content.Message;
                        break;
                    case "RequestThrottled":
                        code = 429;
                        status = content.Message;
                        break;
                    default:
                        code = 404;
                        status = content.Message;
                        break;
                }
            }
            else
            {
                var data = response.Data;

                if (data is CharacterDataWrapper wrapper)
                {
                    code = wrapper.Code;
                    status = wrapper.Status;
                }
                else if (data is ComicDataWrapper dataWrapper)
                {
                    code = dataWrapper.Code;
                    status = dataWrapper.Status;
                }
                else if (data is CreatorDataWrapper creatorDataWrapper)
                {
                    code = creatorDataWrapper.Code;
                    status = creatorDataWrapper.Status;
                }
                else if (data is EventDataWrapper eventDataWrapper)
                {
                    code = eventDataWrapper.Code;
                    status = eventDataWrapper.Status;
                }
                else if (data is SeriesDataWrapper seriesDataWrapper)
                {
                    code = seriesDataWrapper.Code;
                    status = seriesDataWrapper.Status;
                }
                else if (data is StoryDataWrapper storyDataWrapper)
                {
                    code = storyDataWrapper.Code;
                    status = storyDataWrapper.Status;
                }
            }

            switch (code)
            {
                case 409:
                    throw new ArgumentException(status);
                case 404:
                    throw new NotFoundException(status, response.ErrorException);
                case 401:
                    throw new InvalidCredentialException(status);
                case 429:
                    throw new LimitExceededException(status);
            }
        }

        #region Characters

        /// <summary>
        /// Fetches lists of comic characters with optional filters.
        /// </summary>
        /// <param name="name">Return only characters matching the specified full character name (e.g. Spider-Man).</param>
        /// <param name="nameStartsWith">Return characters with names that begin with the specified string (e.g. Sp).</param>
        /// <param name="modifiedSince">Return only characters which have been modified since the specified date.</param>
        /// <param name="comics">Return only characters which appear in the specified comics.</param>
        /// <param name="series">Return only characters which appear the specified series.</param>
        /// <param name="events">Return only characters which appear in the specified events.</param>
        /// <param name="stories">Return only characters which appear the specified stories.</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources, between 1 - 100.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// List of comic characters
        /// </returns>
        public IEnumerable<Character> GetCharacters(string name = null,
            string nameStartsWith = null,
            DateTime? modifiedSince = null,
            IEnumerable<int> comics = null,
            IEnumerable<int> series = null,
            IEnumerable<int> events = null,
            IEnumerable<int> stories = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest("/characters");
            if (!string.IsNullOrWhiteSpace(name))
            {
                request.AddParameter("name", name);
            }

            if (!string.IsNullOrWhiteSpace(nameStartsWith))
            {
                request.AddParameter("nameStartsWith", nameStartsWith);
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(comics, "comics");
            request.AddParameterList(series, "series");
            request.AddParameterList(events, "events");
            request.AddParameterList(stories, "stories");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Name,
                OrderBy.NameDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value >= 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<CharacterDataWrapper> response = _client.Execute<CharacterDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetch a single character by Id.
        /// </summary>
        /// <param name="characterId">A single character id.</param>
        /// <returns>Character details</returns>
        public Character GetCharacter(int characterId)
        {
            var request = CreateRequest($"/characters/{characterId}");

            IRestResponse<CharacterDataWrapper> response = _client.Execute<CharacterDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results.FirstOrDefault(character => character.Id == characterId);
        }

        /// <summary>
        /// Fetches lists of comics containing specific character, with optional filters.
        /// </summary>
        /// <param name="characterId">The character id.</param>
        /// <param name="format">Filter by the issue format.</param>
        /// <param name="formatType">Filter by the issue format type.</param>
        /// <param name="noVariants">Exclude variant comics from the result set.</param>
        /// <param name="dateDescript">Return comics within a predefined date range</param>
        /// <param name="dateRangeBegin">Return comics within a predefined date range, this date being the beginning.</param>
        /// <param name="dateRangeEnd">Return comics within a predefined date range, this date being the ending.</param>
        /// <param name="hasDigitalIssue">Include only results which are available digitally.</param>
        /// <param name="modifiedSince">Return only comics which have been modified since the specified date.</param>
        /// <param name="creators">Return only comics which feature work by the specified creators.</param>
        /// <param name="series">Return only comics which are part of the specified series.</param>
        /// <param name="events">Return only comics which take place in the specified events.</param>
        /// <param name="stories">Return only comics which contain the specified stories.</param>
        /// <param name="sharedAppearances">Return only comics in which the specified characters appear together (for example in which BOTH Spider-Man and Wolverine appear).</param>
        /// <param name="collaborators">Return only comics in which the specified creators worked together (for example in which BOTH Stan Lee and Jack Kirby did work).</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comics
        /// </returns>
        public IEnumerable<Comic> GetComicsForCharacter(int characterId,
            ComicFormat? format = null,
            ComicFormatType? formatType = null,
            bool? noVariants = null,
            DateDescriptor? dateDescript = null,
            DateTime? dateRangeBegin = null,
            DateTime? dateRangeEnd = null,
            bool? hasDigitalIssue = null,
            DateTime? modifiedSince = null,
            IEnumerable<int> creators = null,
            IEnumerable<int> series = null,
            IEnumerable<int> events = null,
            IEnumerable<int> stories = null,
            IEnumerable<int> sharedAppearances = null,
            IEnumerable<int> collaborators = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            if (creators == null) throw new ArgumentNullException(nameof(creators));
            var request = CreateRequest(string.Format("/characters/{0}/comics", characterId));
            if (format.HasValue)
            {
                request.AddParameter("format", format.Value.ToParameter());
            }

            if (formatType.HasValue)
            {
                request.AddParameter("formatType", formatType.Value.ToParameter());
            }

            if (noVariants.HasValue)
            {
                request.AddParameter("noVariants", noVariants.Value.ToString().ToLower());
            }

            if (dateDescript.HasValue)
            {
                request.AddParameter("dateDescriptor", dateDescript.Value.ToParameter());
            }

            if (dateRangeBegin.HasValue && dateRangeEnd.HasValue)
            {
                if (dateRangeBegin.Value <= dateRangeEnd.Value)
                {
                    request.AddParameter("dateRange",
                        $"{dateRangeBegin.Value:YYYY-MM-DD},{dateRangeEnd.Value:YYYY-MM-DD}");
                }
                else
                {
                    throw new ArgumentException("DateRangeBegin must be greater than DateRangeEnd");
                }
            }
            else if (dateRangeBegin.HasValue || dateRangeEnd.HasValue)
            {
                throw new ArgumentException("Date Range requires both a start and end date");
            }

            if (hasDigitalIssue.HasValue)
            {
                request.AddParameter("hasDigitalIssue", hasDigitalIssue.Value.ToString().ToLower());
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(creators, "creators");
            request.AddParameterList(series, "series");
            request.AddParameterList(events, "events");
            request.AddParameterList(stories, "stories");
            request.AddParameterList(sharedAppearances, "sharedAppearances");
            request.AddParameterList(collaborators, "collaborators");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.FocDate,
                OrderBy.FocDateDesc,
                OrderBy.OnSaleDate,
                OrderBy.OnSaleDateDesc,
                OrderBy.Title,
                OrderBy.TitleDesc,
                OrderBy.IssueNumber,
                OrderBy.IssueNumberDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<ComicDataWrapper> response = _client.Execute<ComicDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of events in which a specific character appears, with optional filters.
        /// </summary>
        /// <param name="characterId">The character id</param>
        /// <param name="name">Filter the event list by name.</param>
        /// <param name="nameStartsWith">Return characters with names that begin with the specified string (e.g. Sp).</param>
        /// <param name="modifiedSince">Return only events which have been modified since the specified date.</param>
        /// <param name="creators">Return only events which feature work by the specified creators.</param>
        /// <param name="series">Return only events which are part of the specified series.</param>
        /// <param name="comics">Return only events which take place in the specified comics.</param>
        /// <param name="stories">Return only events which contain the specified stories.</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of events
        /// </returns>
        public IEnumerable<Event> GetEventsForCharacter(int characterId,
            string name = null,
            string nameStartsWith = null,
            DateTime? modifiedSince = null,
            IEnumerable<int> creators = null,
            IEnumerable<int> series = null,
            IEnumerable<int> comics = null,
            IEnumerable<int> stories = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest(string.Format("/characters/{0}/events/", characterId));

            if (!string.IsNullOrWhiteSpace(name))
            {
                request.AddParameter("name", name);
            }

            if (!string.IsNullOrWhiteSpace(nameStartsWith))
            {
                request.AddParameter("nameStartsWith", nameStartsWith);
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(creators, "creators");
            request.AddParameterList(series, "series");
            request.AddParameterList(comics, "comics");
            request.AddParameterList(stories, "stories");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Name,
                OrderBy.NameDesc,
                OrderBy.StartDate,
                OrderBy.StartDateDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<EventDataWrapper> response = _client.Execute<EventDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic series in which a specific character appears, with optional filters.
        /// </summary>
        /// <param name="characterId">The character ID</param>
        /// <param name="title">Filter by series title.</param>
        /// <param name="titleStartsWith">Return titles that begin with the specified string (e.g. Sp).</param>
        /// <param name="modifiedSince">Return only series which have been modified since the specified date.</param>
        /// <param name="comics">Return only series which contain the specified comics.</param>
        /// <param name="stories">Return only series which contain the specified stories.</param>
        /// <param name="events">Return only series which have comics that take place during the specified events.</param>
        /// <param name="creators">Return only series which feature work by the specified creators.</param>
        /// <param name="seriesType">Filter the series by publication frequency type.</param>
        /// <param name="contains">Return only series containing one or more comics with the specified format.</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic series
        /// </returns>
        public IEnumerable<Series> GetSeriesForCharacter(int characterId,
            string title = null,
            string titleStartsWith = null,
            DateTime? modifiedSince = null,
            IEnumerable<int> comics = null,
            IEnumerable<int> stories = null,
            IEnumerable<int> events = null,
            IEnumerable<int> creators = null,
            SeriesType? seriesType = null,
            IEnumerable<ComicFormat> contains = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest($"/characters/{characterId}/series/");

            if (!string.IsNullOrWhiteSpace(title))
            {
                request.AddParameter("title", title);
            }

            if (!string.IsNullOrWhiteSpace(titleStartsWith))
            {
                request.AddParameter("titleStartsWith", titleStartsWith);
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(comics, "comics");
            request.AddParameterList(stories, "stories");
            request.AddParameterList(events, "events");
            request.AddParameterList(creators, "creators");

            if (seriesType.HasValue)
            {
                request.AddParameter("seriesType", seriesType.Value.ToParameter());
            }

            if (contains != null && contains.Any())
            {
                var containsParameters = contains.Select(contain => contain.ToParameter());
                request.AddParameter("contains", string.Join(",", containsParameters));
            }

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Title,
                OrderBy.TitleDesc,
                OrderBy.StartYear,
                OrderBy.StartYearDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<SeriesDataWrapper> response = _client.Execute<SeriesDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic stories featuring a specific character with optional filters.
        /// </summary>
        /// <param name="characterId"></param>
        /// <param name="modifiedSince"></param>
        /// <param name="comics"></param>
        /// <param name="series"></param>
        /// <param name="events"></param>
        /// <param name="creators"></param>
        /// <param name="order"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns>List of stories the character appears in.</returns>
        public IEnumerable<Story> GetStoriesForCharacter(int characterId, 
            DateTime? modifiedSince = null,
            IEnumerable<int> comics = null, 
            IEnumerable<int> series = null, 
            IEnumerable<int> events = null,
            IEnumerable<int> creators = null, 
            IEnumerable<OrderBy> order = null, 
            int? limit = null, 
            int? offset = null)
        {
            var request = CreateRequest($"/characters/{characterId}/stories/");

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(creators, "creators");
            request.AddParameterList(series, "series");
            request.AddParameterList(comics, "comics");
            request.AddParameterList(events, "events");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Id,
                OrderBy.IdDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<StoryDataWrapper> response = _client.Execute<StoryDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        #endregion

        #region Comics

        /// <summary>
        /// Fetches lists of comics with optional filters.
        /// </summary>
        /// <param name="format">Filter by the issue format.</param>
        /// <param name="formatType">Filter by the issue format type.</param>
        /// <param name="noVariants">Exclude variants (alternate covers, secondary printings, director's cuts, etc.) from the result set.</param>
        /// <param name="dateDescript">Return comics within a predefined date range.</param>
        /// <param name="dateRangeBegin">Return comics within a predefined date range, this date being the beginning.</param>
        /// <param name="dateRangeEnd">Return comics within a predefined date range, this date being the ending.</param>
        /// <param name="hasDigitalIssue">Include only results which are available digitally.</param>
        /// <param name="modifiedSince">Return only comics which have been modified since the specified date.</param>
        /// <param name="creators">Return only comics which feature work by the specified creators.</param>
        /// <param name="characters">Return only comics which feature the specified characters.</param>
        /// <param name="series">Return only comics which are part of the specified series.</param>
        /// <param name="events">Return only comics which take place in the specified events.</param>
        /// <param name="stories">Return only comics which contain the specified stories.</param>
        /// <param name="sharedAppearances">Return only comics in which the specified characters appear together (for example in which BOTH Spider-Man and Wolverine appear).</param>
        /// <param name="collaborators">Return only comics in which the specified creators worked together (for example in which BOTH Stan Lee and Jack Kirby did work).</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comics
        /// </returns>
        public IEnumerable<Comic> GetComics(ComicFormat? format = null,
            ComicFormatType? formatType = null,
            bool? noVariants = null,
            DateDescriptor? dateDescript = null,
            DateTime? dateRangeBegin = null,
            DateTime? dateRangeEnd = null,
            bool? hasDigitalIssue = null,
            DateTime? modifiedSince = null,
            IEnumerable<int> creators = null,
            IEnumerable<int> characters = null,
            IEnumerable<int> series = null,
            IEnumerable<int> events = null,
            IEnumerable<int> stories = null,
            IEnumerable<int> sharedAppearances = null,
            IEnumerable<int> collaborators = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest("/comics");

            if (format.HasValue)
            {
                request.AddParameter("format", format.Value.ToParameter());
            }

            if (formatType.HasValue)
            {
                request.AddParameter("formatType", formatType.Value.ToParameter());
            }

            if (noVariants.HasValue)
            {
                request.AddParameter("noVariants", noVariants.Value.ToString().ToLower());
            }

            if (dateDescript.HasValue)
            {
                request.AddParameter("dateDescriptor", dateDescript.Value.ToParameter());
            }

            if (dateRangeBegin.HasValue && dateRangeEnd.HasValue)
            {
                if (dateRangeBegin.Value <= dateRangeEnd.Value)
                {
                    request.AddParameter("dateRange",
                        $"{dateRangeBegin.Value.ToString("YYYY-MM-DD")},{dateRangeEnd.Value.ToString("YYYY-MM-DD")}");
                }
                else
                {
                    throw new ArgumentException("DateRangeBegin must be greater than DateRangeEnd");
                }
            }
            else if (dateRangeBegin.HasValue || dateRangeEnd.HasValue)
            {
                throw new ArgumentException("Date Range requires both a start and end date");
            }

            if (hasDigitalIssue.HasValue)
            {
                request.AddParameter("hasDigitalIssue", hasDigitalIssue.Value.ToString().ToLower());
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(creators, "creators");
            request.AddParameterList(characters, "characters");
            request.AddParameterList(series, "series");
            request.AddParameterList(events, "events");
            request.AddParameterList(stories, "stories");
            request.AddParameterList(sharedAppearances, "sharedAppearances");
            request.AddParameterList(collaborators, "collaborators");

            var availableOrder = new List<OrderBy>
            {
                OrderBy.FocDate,
                OrderBy.FocDateDesc,
                OrderBy.OnSaleDate,
                OrderBy.OnSaleDateDesc,
                OrderBy.Title,
                OrderBy.TitleDesc,
                OrderBy.IssueNumber,
                OrderBy.IssueNumberDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrder);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<ComicDataWrapper> response = _client.Execute<ComicDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches a single comic resource
        /// </summary>
        /// <param name="ComicId">A single comic.</param>
        /// <returns>
        /// Single comic resource
        /// </returns>
        public Comic GetComic(int comicId)
        {
            var request = CreateRequest($"/comics/{comicId}");

            IRestResponse<ComicDataWrapper> response = _client.Execute<ComicDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results.FirstOrDefault(comic => comic.Id == comicId);
        }

        /// <summary>
        /// Fetches lists of characters which appear in a specific comic with optional filters.
        /// </summary>
        /// <param name="comicId">The comic id.</param>
        /// <param name="name">Return only characters matching the specified full character name (e.g. Spider-Man).</param>
        /// <param name="nameStartsWith">Return characters with names that begin with the specified string (e.g. Sp).</param>
        /// <param name="modifiedSince">Return only characters which have been modified since the specified date.</param>
        /// <param name="series">Return only characters which appear the specified series.</param>
        /// <param name="events">Return only characters which appear comics that took place in the specified events.</param>
        /// <param name="stories">Return only characters which appear the specified stories .</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of characters who appear in a specific comic
        /// </returns>
        public IEnumerable<Character> GetCharactersForComic(int comicId,
            string name = null,
            string nameStartsWith = null,
            DateTime? modifiedSince = null,
            IEnumerable<int> series = null,
            IEnumerable<int> events = null,
            IEnumerable<int> stories = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest($"/comics/{comicId}/characters");

            if (!string.IsNullOrWhiteSpace(name))
            {
                request.AddParameter("name", name);
            }

            if (!string.IsNullOrWhiteSpace(nameStartsWith))
            {
                request.AddParameter("nameStartsWith", nameStartsWith);
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(series, "series");
            request.AddParameterList(events, "events");
            request.AddParameterList(stories, "stories");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Name,
                OrderBy.NameDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<CharacterDataWrapper> response = _client.Execute<CharacterDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic creators whose work appears in a specific comic, with optional filters.
        /// </summary>
        /// <param name="comicId">The comic id.</param>
        /// <param name="firstName">Filter by creator first name (e.g. brian).</param>
        /// <param name="middleName">Filter by creator middle name (e.g. Michael).</param>
        /// <param name="lastName">Filter by creator last name (e.g. Bendis).</param>
        /// <param name="suffix">Filter by suffix or honorific (e.g. Jr., Sr.).</param>
        /// <param name="nameStartsWith">Filter by creator names that match critera (e.g. B, St L).</param>
        /// <param name="firstNameStartsWith">Filter by creator first names that match critera (e.g. B, St L).</param>
        /// <param name="middleNameStartsWith">Filter by creator middle names that match critera (e.g. Mi).</param>
        /// <param name="lastNameStartsWith">Filter by creator last names that match critera (e.g. Ben).</param>
        /// <param name="modifiedSince">Return only creators which have been modified since the specified date.</param>
        /// <param name="comics">Return only creators who worked on in the specified comics.</param>
        /// <param name="series">Return only creators who worked on the specified series.</param>
        /// <param name="stories">Return only creators who worked on the specified stories.</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic creators whose work appears in a specific comic
        /// </returns>
        public IEnumerable<Creator> GetCreatorsForComic(int comicId,
            string firstName = null,
            string middleName = null,
            string lastName = null,
            string suffix = null,
            string nameStartsWith = null,
            string firstNameStartsWith = null,
            string middleNameStartsWith = null,
            string lastNameStartsWith = null,
            DateTime? modifiedSince = null,
            IEnumerable<int> comics = null,
            IEnumerable<int> series = null,
            IEnumerable<int> stories = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest($"/comics/{comicId}/characters");

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                request.AddParameter("firstName", firstName);
            }

            if (!string.IsNullOrWhiteSpace(middleName))
            {
                request.AddParameter("middleName", middleName);
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                request.AddParameter("lastName", lastName);
            }

            if (!string.IsNullOrWhiteSpace(suffix))
            {
                request.AddParameter("suffix", suffix);
            }

            if (!string.IsNullOrWhiteSpace(nameStartsWith))
            {
                request.AddParameter("nameStartsWith", nameStartsWith);
            }

            if (!string.IsNullOrWhiteSpace(firstNameStartsWith))
            {
                request.AddParameter("firstNameStartsWith", firstNameStartsWith);
            }

            if (!string.IsNullOrWhiteSpace(middleNameStartsWith))
            {
                request.AddParameter("middleNameStartsWith", middleNameStartsWith);
            }

            if (!string.IsNullOrWhiteSpace(lastNameStartsWith))
            {
                request.AddParameter("lastNameStartsWith", lastNameStartsWith);
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(series, "series");
            request.AddParameterList(comics, "comics");
            request.AddParameterList(stories, "stories");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.FirstName,
                OrderBy.FirstNameDesc,
                OrderBy.MiddleName,
                OrderBy.MiddleNameDesc,
                OrderBy.LastName,
                OrderBy.LastNameDesc,
                OrderBy.Suffix,
                OrderBy.SuffixDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.GetValueOrDefault() > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.GetValueOrDefault() > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<CreatorDataWrapper> response = _client.Execute<CreatorDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of events in which a specific comic appears, with optional filters.
        /// </summary>
        /// <param name="comicId">The comic ID.</param>
        /// <param name="name">Filter the event list by name.</param>
        /// <param name="nameStartsWith">Return characters with names that begin with the specified string (e.g. Sp).</param>
        /// <param name="modifiedSince">Return only events which have been modified since the specified date.</param>
        /// <param name="creators">Return only events which feature work by the specified creators.</param>
        /// <param name="characters">Return only events which feature the specified characters.</param>
        /// <param name="series">Return only events which are part of the specified series.</param>
        /// <param name="stories">Return only events which contain the specified stories.</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of events in which a specific comic appears
        /// </returns>
        public IEnumerable<Event> GetEventsForComic(int comicId,
            string name = null,
            string nameStartsWith = null,
            DateTime? modifiedSince = null,
            IEnumerable<int> creators = null,
            IEnumerable<int> characters = null,
            IEnumerable<int> series = null,
            IEnumerable<int> stories = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest($"/comics/{comicId}/events/");

            if (!string.IsNullOrWhiteSpace(name))
            {
                request.AddParameter("name", name);
            }

            if (!string.IsNullOrWhiteSpace(nameStartsWith))
            {
                request.AddParameter("nameStartsWith", nameStartsWith);
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(creators, "creators");
            request.AddParameterList(characters, "characters");
            request.AddParameterList(series, "series");
            request.AddParameterList(stories, "stories");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Name,
                OrderBy.NameDesc,
                OrderBy.StartDate,
                OrderBy.StartDateDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<EventDataWrapper> response = _client.Execute<EventDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic stories in a specific comic issue, with optional filters.
        /// </summary>
        /// <param name="comicId"></param>
        /// <param name="modifiedSince"></param>
        /// <param name="series"></param>
        /// <param name="events"></param>
        /// <param name="creators"></param>
        /// <param name="characters"></param>
        /// <param name="order"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns>
        /// Lists of comic stories in a specific comic issue
        /// </returns>
        public IEnumerable<Story> GetStoriesForComic(int comicId, 
            DateTime? modifiedSince = null,
            IEnumerable<int> series = null, 
            IEnumerable<int> events = null, 
            IEnumerable<int> creators = null,
            IEnumerable<int> characters = null, 
            IEnumerable<OrderBy> order = null, 
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest($"/comics/{comicId}/events/");

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(series, "series");
            request.AddParameterList(events, "events");
            request.AddParameterList(creators, "creators");
            request.AddParameterList(characters, "characters");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Id,
                OrderBy.IdDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<StoryDataWrapper> response = _client.Execute<StoryDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        #endregion

        #region Creators

        /// <summary>
        /// Fetches lists of comic creators with optional filters.
        /// </summary>
        /// <param name="firstName">Filter by creator first name (e.g. Brian).</param>
        /// <param name="middleName">Filter by creator middle name (e.g. Michael).</param>
        /// <param name="lastName">Filter by creator last name (e.g. Bendis).</param>
        /// <param name="suffix">Filter by suffix or honorific (e.g. Jr., Sr.).</param>
        /// <param name="nameStartsWith"></param>
        /// <param name="firstNameStartsWith"></param>
        /// <param name="middleNameStartsWith"></param>
        /// <param name="lastNameStartsWith"></param>
        /// <param name="modifiedSince"></param>
        /// <param name="comics"></param>
        /// <param name="series"></param>
        /// <param name="events"></param>
        /// <param name="stories"></param>
        /// <param name="order"></param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic creators
        /// </returns>
        public IEnumerable<Creator> GetCreators(string firstName = null,
            string middleName = null,
            string lastName = null,
            string suffix = null,
            string nameStartsWith = null,
            string firstNameStartsWith = null, 
            string middleNameStartsWith = null,
            string lastNameStartsWith = null, 
            DateTime? modifiedSince = null, 
            IEnumerable<int> comics = null,
            IEnumerable<int> series = null, 
            IEnumerable<int> events = null, 
            IEnumerable<int> stories = null,
            IEnumerable<OrderBy> order = null,
            int? Limit = null,
            int? Offset = null)
        {
            var request = CreateRequest("/creators");

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                request.AddParameter("firstName", firstName);
            }

            if (!string.IsNullOrWhiteSpace(middleName))
            {
                request.AddParameter("middleName", middleName);
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                request.AddParameter("lastName", lastName);
            }

            if (!string.IsNullOrWhiteSpace(suffix))
            {
                request.AddParameter("suffix", suffix);
            }

            if (!string.IsNullOrWhiteSpace(nameStartsWith))
            {
                request.AddParameter("nameStartsWith", nameStartsWith);
            }

            if (!string.IsNullOrWhiteSpace(firstNameStartsWith))
            {
                request.AddParameter("firstNameStartsWith", firstNameStartsWith);
            }

            if (!string.IsNullOrWhiteSpace(middleNameStartsWith))
            {
                request.AddParameter("middleNameStartsWith", middleNameStartsWith);
            }

            if (!string.IsNullOrWhiteSpace(lastNameStartsWith))
            {
                request.AddParameter("lastNameStartsWith", lastNameStartsWith);
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(comics, "comics");
            request.AddParameterList(series, "series");
            request.AddParameterList(events, "events");
            request.AddParameterList(stories, "stories");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.FirstName,
                OrderBy.FirstNameDesc,
                OrderBy.MiddleName,
                OrderBy.MiddleNameDesc,
                OrderBy.LastName,
                OrderBy.LastNameDesc,
                OrderBy.Suffix,
                OrderBy.SuffixDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }

            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            IRestResponse<CreatorDataWrapper> response = _client.Execute<CreatorDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// This method fetches a single creator resource. It is the canonical URI for any creator resource provided by the API.
        /// </summary>
        /// <param name="CreatorId">A single creator id.</param>
        /// <returns>
        /// A single creator resource.
        /// </returns>
        public Creator GetCreator(int CreatorId)
        {
            var request = CreateRequest(string.Format("/creators/{0}", CreatorId));

            IRestResponse<CreatorDataWrapper> response = _client.Execute<CreatorDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results.FirstOrDefault(creator => creator.Id == CreatorId);
        }

        /// <summary>
        /// Fetches lists of comics in which the work of a specific creator appears, with optional filters.
        /// </summary>
        /// <param name="creatorId"></param>
        /// <param name="format"></param>
        /// <param name="formatType"></param>
        /// <param name="noVariants"></param>
        /// <param name="dateDescript"></param>
        /// <param name="dateRangeBegin"></param>
        /// <param name="dateRangeEnd"></param>
        /// <param name="hasDigitalIssue"></param>
        /// <param name="modifiedSince"></param>
        /// <param name="characters"></param>
        /// <param name="series"></param>
        /// <param name="events"></param>
        /// <param name="stories"></param>
        /// <param name="sharedAppearances"></param>
        /// <param name="collaborators"></param>
        /// <param name="order"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns>
        /// Lists of comics in which the work of a specific creator appears.
        /// </returns>
        public IEnumerable<Comic> GetComicsForCreator(int creatorId, 
            ComicFormat? format = null,
            ComicFormatType? formatType = null, 
            bool? noVariants = null, 
            DateDescriptor? dateDescript = null,
            DateTime? dateRangeBegin = null, 
            DateTime? dateRangeEnd = null, 
            bool? hasDigitalIssue = null,
            DateTime? modifiedSince = null, 
            IEnumerable<int> characters = null, 
            IEnumerable<int> series = null,
            IEnumerable<int> events = null, 
            IEnumerable<int> stories = null, 
            IEnumerable<int> sharedAppearances = null,
            IEnumerable<int> collaborators = null, 
            IEnumerable<OrderBy> order = null, 
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest(string.Format("/creators/{0}/comics", creatorId));

            if (format.HasValue)
            {
                request.AddParameter("format", format.Value.ToParameter());
            }

            if (formatType.HasValue)
            {
                request.AddParameter("formatType", formatType.Value.ToParameter());
            }

            if (noVariants.HasValue)
            {
                request.AddParameter("noVariants", noVariants.Value.ToString().ToLower());
            }

            if (dateDescript.HasValue)
            {
                request.AddParameter("dateDescriptor", dateDescript.Value.ToParameter());
            }

            if (dateRangeBegin.HasValue && dateRangeEnd.HasValue)
            {
                if (dateRangeBegin.Value <= dateRangeEnd.Value)
                {
                    request.AddParameter("dateRange",
                        string.Format("{0},{1}", dateRangeBegin.Value.ToString("YYYY-MM-DD"),
                            dateRangeEnd.Value.ToString("YYYY-MM-DD")));
                }
                else
                {
                    throw new ArgumentException("DateRangeBegin must be greater than DateRangeEnd");
                }
            }
            else if (dateRangeBegin.HasValue || dateRangeEnd.HasValue)
            {
                throw new ArgumentException("Date Range requires both a start and end date");
            }

            if (hasDigitalIssue.HasValue)
            {
                request.AddParameter("hasDigitalIssue", hasDigitalIssue.Value.ToString().ToLower());
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(characters, "characters");
            request.AddParameterList(series, "series");
            request.AddParameterList(events, "events");
            request.AddParameterList(stories, "stories");
            request.AddParameterList(sharedAppearances, "sharedAppearances");
            request.AddParameterList(collaborators, "collaborators");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.FocDate,
                OrderBy.FocDateDesc,
                OrderBy.OnSaleDate,
                OrderBy.OnSaleDateDesc,
                OrderBy.Title,
                OrderBy.TitleDesc,
                OrderBy.IssueNumber,
                OrderBy.IssueNumberDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<ComicDataWrapper> response = _client.Execute<ComicDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of events featuring the work of a specific creator with optional filters.
        /// </summary>
        /// <param name="creatorId">The creator ID.</param>
        /// <param name="name">Filter the event list by name.</param>
        /// <param name="nameStartsWith">Return characters with names that begin with the specified string (e.g. Sp).</param>
        /// <param name="modifiedSince">Return only events which have been modified since the specified date.</param>
        /// <param name="characters">Return only events which feature the specified characters.</param>
        /// <param name="series">Return only events which are part of the specified series.</param>
        /// <param name="comics">Return only events which take place in the specified comics.</param>
        /// <param name="stories">Return only events which contain the specified stories.</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of events featuring the work of a specific creator
        /// </returns>
        public IEnumerable<Event> GetEventsForCreator(int creatorId,
            string name = null,
            string nameStartsWith = null,
            DateTime? modifiedSince = null,
            IEnumerable<int> characters = null,
            IEnumerable<int> series = null,
            IEnumerable<int> comics = null,
            IEnumerable<int> stories = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest($"/creators/{creatorId}/events/");

            if (!string.IsNullOrWhiteSpace(name))
            {
                request.AddParameter("name", name);
            }

            if (!string.IsNullOrWhiteSpace(nameStartsWith))
            {
                request.AddParameter("nameStartsWith", nameStartsWith);
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            if (characters != null && characters.Any())
            {
                request.AddParameter("characters", string.Join<int>(",", characters));
            }

            if (series != null && series.Any())
            {
                request.AddParameter("series", string.Join<int>(",", series));
            }

            if (comics != null && comics.Any())
            {
                request.AddParameter("comics", string.Join<int>(",", comics));
            }

            if (stories != null && stories.Any())
            {
                request.AddParameter("stories", string.Join<int>(",", stories));
            }

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Name,
                OrderBy.NameDesc,
                OrderBy.StartDate,
                OrderBy.StartDateDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<EventDataWrapper> response = _client.Execute<EventDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic series in which a specific creator's work appears, with optional filters.
        /// </summary>
        /// <param name="creatorId">The creator ID.</param>
        /// <param name="title">Filter by series title.</param>
        /// <param name="titleStartsWith">Return titles that begin with the specified string (e.g. Sp).</param>
        /// <param name="modifiedSince">Return only series which have been modified since the specified date.</param>
        /// <param name="comics">Return only series which contain the specified comics.</param>
        /// <param name="stories">Return only series which contain the specified stories.</param>
        /// <param name="events">Return only series which have comics that take place during the specified events.</param>
        /// <param name="characters">Return only series which feature the specified characters.</param>
        /// <param name="seriesType">Filter the series by publication frequency type.</param>
        /// <param name="contains">Return only series containing one or more comics with the specified format.</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic series in which a specific creator's work appears
        /// </returns>
        public IEnumerable<Series> GetSeriesForCreator(int creatorId,
            string title = null,
            string titleStartsWith = null,
            DateTime? modifiedSince = null,
            IEnumerable<int> comics = null,
            IEnumerable<int> stories = null,
            IEnumerable<int> events = null,
            IEnumerable<int> characters = null,
            SeriesType? seriesType = null,
            IEnumerable<ComicFormat> contains = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest($"/creators/{creatorId}/series/");

            if (!string.IsNullOrWhiteSpace(title))
            {
                request.AddParameter("title", title);
            }

            if (!string.IsNullOrWhiteSpace(titleStartsWith))
            {
                request.AddParameter("titleStartsWith", titleStartsWith);
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(comics, "comics");
            request.AddParameterList(stories, "stories");
            request.AddParameterList(events, "events");
            request.AddParameterList(characters, "characters");

            if (seriesType.HasValue)
            {
                request.AddParameter("seriesType", seriesType.Value.ToParameter());
            }

            if (contains != null && contains.Any())
            {
                var containsParameters = contains.Select(contain => contain.ToParameter());
                request.AddParameter("contains", string.Join(",", containsParameters));
            }

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Title,
                OrderBy.TitleDesc,
                OrderBy.StartYear,
                OrderBy.StartYearDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<SeriesDataWrapper> response = _client.Execute<SeriesDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic stories by a specific creator with optional filters.
        /// </summary>
        /// <param name="creatorId">The ID of the creator.</param>
        /// <param name="modifiedSince">Return only stories which have been modified since the specified date.</param>
        /// <param name="comics">Return only stories contained in the specified comics.</param>
        /// <param name="series">Return only stories contained the specified series.</param>
        /// <param name="events">Return only stories which take place during the specified events.</param>
        /// <param name="characters">Return only stories which feature the specified characters.</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic stories by a specific creator
        /// </returns>
        public IEnumerable<Story> GetStoriesForCreator(int creatorId,
            DateTime? modifiedSince = null,
            IEnumerable<int> comics = null,
            IEnumerable<int> series = null,
            IEnumerable<int> events = null,
            IEnumerable<int> characters = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest($"/creators/{creatorId}/events/");

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(comics, "comics");
            request.AddParameterList(series, "series");
            request.AddParameterList(events, "events");
            request.AddParameterList(characters, "characters");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Id,
                OrderBy.IdDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<StoryDataWrapper> response = _client.Execute<StoryDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        #endregion

        #region Events

        /// <summary>
        /// Fetches lists of events with optional filters.
        /// </summary>
        /// <param name="name">Return only events which match the specified name.</param>
        /// <param name="nameStartsWith">Return characters with names that begin with the specified string (e.g. Sp).</param>
        /// <param name="modifiedSince">Return only events which have been modified since the specified date.</param>
        /// <param name="creators">Return only events which feature work by the specified creators.</param>
        /// <param name="characters">Return only events which feature the specified characters.</param>
        /// <param name="series">Return only events which are part of the specified series.</param>
        /// <param name="comics">Return only events which take place in the specified comics.</param>
        /// <param name="stories">Return only events which take place in the specified stories.</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of events
        /// </returns>
        public IEnumerable<Event> GetEvents(string name = null,
            string nameStartsWith = null,
            DateTime? modifiedSince = null,
            IEnumerable<int> creators = null,
            IEnumerable<int> characters = null,
            IEnumerable<int> series = null,
            IEnumerable<int> comics = null,
            IEnumerable<int> stories = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest("/events/");

            if (!string.IsNullOrWhiteSpace(name))
            {
                request.AddParameter("name", name);
            }

            if (!string.IsNullOrWhiteSpace(nameStartsWith))
            {
                request.AddParameter("nameStartsWith", nameStartsWith);
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(creators, "creators");
            request.AddParameterList(characters, "characters");
            request.AddParameterList(series, "series");
            request.AddParameterList(comics, "comics");
            request.AddParameterList(stories, "stories");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Name,
                OrderBy.NameDesc,
                OrderBy.StartDate,
                OrderBy.StartDateDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<EventDataWrapper> response = _client.Execute<EventDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }


        /// <summary>
        /// This method fetches a single event resource. It is the canonical URI for any event resource provided by the API.
        /// </summary>
        /// <param name="EventId">The event ID</param>
        /// <returns>
        /// A single event resource
        /// </returns>
        public Event GetEvent(int EventId)
        {
            var request = CreateRequest(string.Format("/events/{0}", EventId));

            IRestResponse<EventDataWrapper> response = _client.Execute<EventDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results.FirstOrDefault(ev => ev.Id == EventId);
        }

        /// <summary>
        /// Fetches lists of characters which appear in a specific event, with optional filters.
        /// </summary>
        /// <param name="eventId">The event ID</param>
        /// <param name="name">Return only characters matching the specified full character name (e.g. Spider-Man).</param>
        /// <param name="nameStartsWith">Return characters with names that begin with the specified string (e.g. Sp).</param>
        /// <param name="modifiedSince">Return only characters which have been modified since the specified date.</param>
        /// <param name="comics">Return only characters which appear in the specified comics.</param>
        /// <param name="series">Return only characters which appear the specified series.</param>
        /// <param name="stories">Return only characters which appear the specified stories.</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of characters which appear in a specific event
        /// </returns>
        public IEnumerable<Character> GetCharactersForEvent(int eventId,
            string name = null,
            string nameStartsWith = null,
            DateTime? modifiedSince = null,
            IEnumerable<int> comics = null,
            IEnumerable<int> series = null,
            IEnumerable<int> stories = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest($"/events/{eventId}/characters");

            if (!string.IsNullOrWhiteSpace(name))
            {
                request.AddParameter("name", name);
            }

            if (!string.IsNullOrWhiteSpace(nameStartsWith))
            {
                request.AddParameter("nameStartsWith", nameStartsWith);
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(comics, "comics");
            request.AddParameterList(series, "series");
            request.AddParameterList(stories, "stories");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Name,
                OrderBy.NameDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<CharacterDataWrapper> response = _client.Execute<CharacterDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comics which take place during a specific event, with optional filters.
        /// </summary>
        /// <param name="eventId">The event id.</param>
        /// <param name="format">Filter by the issue format.</param>
        /// <param name="formatType">Filter by the issue format type.</param>
        /// <param name="noVariants">Exclude variant comics from the result set.</param>
        /// <param name="dateDescript">Return comics within a predefined date range.</param>
        /// <param name="dateRangeBegin">Return comics within a predefined date range, this is the beginning date.</param>
        /// <param name="dateRangeEnd">Return comics within a predefined date range, this is the ending date.</param>
        /// <param name="hasDigitalIssue">Include only results which are available digitally.</param>
        /// <param name="modifiedSince">Return only comics which have been modified since the specified date.</param>
        /// <param name="creators">Return only comics which feature work by the specified creators.</param>
        /// <param name="characters">Return only comics which feature the specified characters.</param>
        /// <param name="series">Return only comics which are part of the specified series.</param>
        /// <param name="events">Return only comics which take place in the specified events.</param>
        /// <param name="stories">Return only comics which contain the specified stories.</param>
        /// <param name="sharedAppearances">Return only comics in which the specified characters appear together (for example in which BOTH Spider-Man and Wolverine appear).</param>
        /// <param name="collaborators">Return only comics in which the specified creators worked together (for example in which BOTH Stan Lee and Jack Kirby did work).</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comics which take place during a specific event
        /// </returns>
        public IEnumerable<Comic> GetComicsForEvent(int eventId,
            ComicFormat? format = null,
            ComicFormatType? formatType = null,
            bool? noVariants = null,
            DateDescriptor? dateDescript = null,
            DateTime? dateRangeBegin = null,
            DateTime? dateRangeEnd = null,
            bool? hasDigitalIssue = null,
            DateTime? modifiedSince = null,
            IEnumerable<int> creators = null,
            IEnumerable<int> characters = null,
            IEnumerable<int> series = null,
            IEnumerable<int> events = null, // Weird to see this here
            IEnumerable<int> stories = null,
            IEnumerable<int> sharedAppearances = null,
            IEnumerable<int> collaborators = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest($"/events/{eventId}/comics");

            if (format.HasValue)
            {
                request.AddParameter("format", format.Value.ToParameter());
            }

            if (formatType.HasValue)
            {
                request.AddParameter("formatType", formatType.Value.ToParameter());
            }

            if (noVariants.HasValue)
            {
                request.AddParameter("noVariants", noVariants.Value.ToString().ToLower());
            }

            if (dateDescript.HasValue)
            {
                request.AddParameter("dateDescriptor", dateDescript.Value.ToParameter());
            }

            if (dateRangeBegin.HasValue && dateRangeEnd.HasValue)
            {
                if (dateRangeBegin.Value <= dateRangeEnd.Value)
                {
                    request.AddParameter("dateRange",
                        string.Format("{0},{1}", dateRangeBegin.Value.ToString("YYYY-MM-DD"),
                            dateRangeEnd.Value.ToString("YYYY-MM-DD")));
                }
                else
                {
                    throw new ArgumentException("DateRangeBegin must be greater than DateRangeEnd");
                }
            }
            else if (dateRangeBegin.HasValue || dateRangeEnd.HasValue)
            {
                throw new ArgumentException("Date Range requires both a start and end date");
            }

            if (hasDigitalIssue.HasValue)
            {
                request.AddParameter("hasDigitalIssue", hasDigitalIssue.Value.ToString().ToLower());
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(creators, "creators");
            request.AddParameterList(characters, "characters");
            request.AddParameterList(series, "series");
            request.AddParameterList(events, "events");
            request.AddParameterList(stories, "stories");
            request.AddParameterList(sharedAppearances, "sharedAppearances");
            request.AddParameterList(collaborators, "collaborators");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.FocDate,
                OrderBy.FocDateDesc,
                OrderBy.OnSaleDate,
                OrderBy.OnSaleDateDesc,
                OrderBy.Title,
                OrderBy.TitleDesc,
                OrderBy.IssueNumber,
                OrderBy.IssueNumberDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<ComicDataWrapper> response = _client.Execute<ComicDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic creators whose work appears in a specific event, with optional filters.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="firstName">Filter by creator first name (e.g. brian).</param>
        /// <param name="middleName">Filter by creator middle name (e.g. Michael).</param>
        /// <param name="lastName">Filter by creator last name (e.g. Bendis).</param>
        /// <param name="suffix">Filter by suffix or honorific (e.g. Jr., Sr.).</param>
        /// <param name="nameStartsWith">Filter by creator names that match critera (e.g. B, St L).</param>
        /// <param name="firstNameStartsWith">Filter by creator first names that match critera (e.g. B, St L).</param>
        /// <param name="middleNameStartsWith">Filter by creator middle names that match critera (e.g. Mi).</param>
        /// <param name="lastNameStartsWith">Filter by creator last names that match critera (e.g. Ben).</param>
        /// <param name="modifiedSince">Return only creators which have been modified since the specified date.</param>
        /// <param name="comics">Return only creators who worked on in the specified comics.</param>
        /// <param name="series">Return only creators who worked on the specified series.</param>
        /// <param name="stories">Return only creators who worked on the specified stories.</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic creators whose work appears in a specific event
        /// </returns>
        public IEnumerable<Creator> GetCreatorsForEvent(int eventId,
            string firstName = null,
            string middleName = null,
            string lastName = null,
            string suffix = null,
            string nameStartsWith = null,
            string firstNameStartsWith = null,
            string middleNameStartsWith = null,
            string lastNameStartsWith = null,
            DateTime? modifiedSince = null,
            IEnumerable<int> comics = null,
            IEnumerable<int> series = null,
            IEnumerable<int> stories = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest($"/events/{eventId}/creators");

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                request.AddParameter("firstName", firstName);
            }

            if (!string.IsNullOrWhiteSpace(middleName))
            {
                request.AddParameter("middleName", middleName);
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                request.AddParameter("lastName", lastName);
            }

            if (!string.IsNullOrWhiteSpace(suffix))
            {
                request.AddParameter("suffix", suffix);
            }

            if (!string.IsNullOrWhiteSpace(nameStartsWith))
            {
                request.AddParameter("nameStartsWith", nameStartsWith);
            }

            if (!string.IsNullOrWhiteSpace(firstNameStartsWith))
            {
                request.AddParameter("firstNameStartsWith", firstNameStartsWith);
            }

            if (!string.IsNullOrWhiteSpace(middleNameStartsWith))
            {
                request.AddParameter("middleNameStartsWith", middleNameStartsWith);
            }

            if (!string.IsNullOrWhiteSpace(lastNameStartsWith))
            {
                request.AddParameter("lastNameStartsWith", lastNameStartsWith);
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(comics, "comics");
            request.AddParameterList(series, "series");
            request.AddParameterList(stories, "stories");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.FirstName,
                OrderBy.FirstNameDesc,
                OrderBy.MiddleName,
                OrderBy.MiddleNameDesc,
                OrderBy.LastName,
                OrderBy.LastNameDesc,
                OrderBy.Suffix,
                OrderBy.SuffixDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<CreatorDataWrapper> response = _client.Execute<CreatorDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic series in which a specific event takes place, with optional filters.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="title">Filter by series title.</param>
        /// <param name="titleStartsWith">Return titles that begin with the specified string (e.g. Sp).</param>
        /// <param name="modifiedSince">Return only series which have been modified since the specified date.</param>
        /// <param name="comics">Return only series which contain the specified comics.</param>
        /// <param name="stories">Return only series which contain the specified stories.</param>
        /// <param name="creators">Return only series which feature work by the specified creators.</param>
        /// <param name="characters">Return only series which feature the specified characters.</param>
        /// <param name="seriesType">Filter the series by publication frequency type.</param>
        /// <param name="contains">Return only series containing one or more comics with the specified format.</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic series in which a specific event takes place
        /// </returns>
        public IEnumerable<Series> GetSeriesForEvent(int eventId,
            string title = null,
            string titleStartsWith = null,
            DateTime? modifiedSince = null,
            IEnumerable<int> comics = null,
            IEnumerable<int> stories = null,
            IEnumerable<int> creators = null,
            IEnumerable<int> characters = null,
            SeriesType? seriesType = null,
            IEnumerable<ComicFormat> contains = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest($"/events/{eventId}/series/");

            if (!string.IsNullOrWhiteSpace(title))
            {
                request.AddParameter("title", title);
            }

            if (!string.IsNullOrWhiteSpace(titleStartsWith))
            {
                request.AddParameter("titleStartsWith", titleStartsWith);
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(comics, "comics");
            request.AddParameterList(stories, "stories");
            request.AddParameterList(creators, "creators");
            request.AddParameterList(characters, "characters");

            if (seriesType.HasValue)
            {
                request.AddParameter("seriesType", seriesType.Value.ToParameter());
            }

            if (contains != null && contains.Any())
            {
                var containsParameters = contains.Select(contain => contain.ToParameter());
                request.AddParameter("contains", string.Join(",", containsParameters));
            }

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Title,
                OrderBy.TitleDesc,
                OrderBy.StartYear,
                OrderBy.StartYearDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<SeriesDataWrapper> response = _client.Execute<SeriesDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic stories from a specific event, with optional filters.
        /// </summary>
        /// <param name="eventId">The ID of the event.</param>
        /// <param name="modifiedSince">Return only stories which have been modified since the specified date.</param>
        /// <param name="comics">Return only stories contained in the specified.</param>
        /// <param name="series">Return only stories contained the specified series.</param>
        /// <param name="creators">Return only stories which feature work by the specified creators.</param>
        /// <param name="characters">Return only stories which feature the specified characters.</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic stories from a specific event
        /// </returns>
        public IEnumerable<Story> GetStoriesForEvent(int eventId,
            DateTime? modifiedSince = null,
            IEnumerable<int> comics = null,
            IEnumerable<int> series = null,
            IEnumerable<int> creators = null,
            IEnumerable<int> characters = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest(string.Format("/events/{0}/stories", eventId));

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(comics, "comics");
            request.AddParameterList(series, "series");
            request.AddParameterList(creators, "creators");
            request.AddParameterList(characters, "characters");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Id,
                OrderBy.IdDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<StoryDataWrapper> response = _client.Execute<StoryDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        #endregion

        #region Series

        /// <summary>
        /// Fetches lists of comic series with optional filters.
        /// </summary>
        /// <param name="title">Return only series matching the specified title.</param>
        /// <param name="titleStartsWith">Return titles that begin with the specified string (e.g. Sp).</param>
        /// <param name="modifiedSince">Return only series which have been modified since the specified date.</param>
        /// <param name="comics">Return only series which contain the specified comics.</param>
        /// <param name="stories">Return only series which contain the specified stories.</param>
        /// <param name="events">Return only series which have comics that take place during the specified events.</param>
        /// <param name="creators">Return only series which feature work by the specified creators.</param>
        /// <param name="characters">Return only series which feature the specified characters.</param>
        /// <param name="type">Filter the series by publication frequency type.</param>
        /// <param name="contains">Return only series containing one or more comics with the specified format.</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic series
        /// </returns>
        public IEnumerable<Series> GetSeries(string title = null,
            string titleStartsWith = null,
            DateTime? modifiedSince = null,
            IEnumerable<int> comics = null,
            IEnumerable<int> stories = null,
            IEnumerable<int> events = null,
            IEnumerable<int> creators = null,
            IEnumerable<int> characters = null,
            SeriesType? type = null,
            ComicFormat? contains = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest("/series");

            if (!string.IsNullOrWhiteSpace(title))
            {
                request.AddParameter("title", title);
            }

            if (!string.IsNullOrWhiteSpace(titleStartsWith))
            {
                request.AddParameter("titleStartsWith", titleStartsWith);
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(comics, "comics");
            request.AddParameterList(stories, "stories");
            request.AddParameterList(events, "events");
            request.AddParameterList(creators, "creators");
            request.AddParameterList(characters, "characters");

            if (type.HasValue)
            {
                request.AddParameter("seriesType", type.Value.ToParameter());
            }

            if (contains.HasValue)
            {
                request.AddParameter("contains", contains.Value.ToParameter());
            }

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Title,
                OrderBy.TitleDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc,
                OrderBy.StartYear,
                OrderBy.StartYearDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<SeriesDataWrapper> response = _client.Execute<SeriesDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// This method fetches a single comic series resource. It is the canonical URI for any comic series resource provided by the API.
        /// </summary>
        /// <param name="seriesId">The series id.</param>
        /// <returns>
        /// A single comic series resource
        /// </returns>
        public Series GetSeries(int seriesId)
        {
            var request = CreateRequest($"/series/{seriesId}");

            IRestResponse<SeriesDataWrapper> response = _client.Execute<SeriesDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results.FirstOrDefault(series => series.Id == seriesId);
        }

        /// <summary>
        /// Fetches lists of characters which appear in specific series, with optional filters.
        /// </summary>
        /// <param name="seriesId">The series id.</param>
        /// <param name="name">Return only characters matching the specified full character name (e.g. Spider-Man).</param>
        /// <param name="nameStartsWith">Return characters with names that begin with the specified string (e.g. Sp).</param>
        /// <param name="modifiedSince">Return only characters which have been modified since the specified date.</param>
        /// <param name="comics">Return only characters which appear in the specified comics.</param>
        /// <param name="events">Return only characters which appear comics that took place in the specified events.</param>
        /// <param name="stories">Return only characters which appear the specified stories.</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of characters which appear in specific series
        /// </returns>
        public IEnumerable<Character> GetCharactersForSeries(int seriesId,
            string name = null,
            string nameStartsWith = null,
            DateTime? modifiedSince = null,
            IEnumerable<int> comics = null,
            IEnumerable<int> events = null,
            IEnumerable<int> stories = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest($"/series/{seriesId}/characters");

            if (!string.IsNullOrWhiteSpace(name))
            {
                request.AddParameter("name", name);
            }

            if (!string.IsNullOrWhiteSpace(nameStartsWith))
            {
                request.AddParameter("nameStartsWith", nameStartsWith);
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(comics, "comics");
            request.AddParameterList(events, "events");
            request.AddParameterList(stories, "stories");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Name,
                OrderBy.NameDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<CharacterDataWrapper> response = _client.Execute<CharacterDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comics which are published as part of a specific series, with optional filters.
        /// </summary>
        /// <param name="seriesId">The series ID.</param>
        /// <param name="format">Filter by the issue format.</param>
        /// <param name="formatType">Filter by the issue format type.</param>
        /// <param name="noVariants">Exclude variant comics from the result set.</param>
        /// <param name="dateDescript">Return comics within a predefined date range.</param>
        /// <param name="dateRangeBegin">Return comics within a predefined date range, this is the beginning date.</param>
        /// <param name="dateRangeEnd">Return comics within a predefined date range, this is the ending date.</param>
        /// <param name="hasDigitalIssue">Include only results which are available digitally.</param>
        /// <param name="modifiedSince">Return only comics which have been modified since the specified date.</param>
        /// <param name="creators">Return only comics which feature work by the specified creators.</param>
        /// <param name="characters">Return only comics which feature the specified characters.</param>
        /// <param name="events">Return only comics which take place in the specified events.</param>
        /// <param name="stories">Return only comics which contain the specified stories.</param>
        /// <param name="sharedAppearances">Return only comics in which the specified characters appear together (for example in which BOTH Spider-Man and Wolverine appear).</param>
        /// <param name="collaborators">Return only comics in which the specified creators worked together (for example in which BOTH Stan Lee and Jack Kirby did work).</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comics which are published as part of a specific series
        /// </returns>
        public IEnumerable<Comic> GetComicsForSeries(int seriesId,
            ComicFormat? format = null,
            ComicFormatType? formatType = null,
            bool? noVariants = null,
            DateDescriptor? dateDescript = null,
            DateTime? dateRangeBegin = null,
            DateTime? dateRangeEnd = null,
            bool? hasDigitalIssue = null,
            DateTime? modifiedSince = null,
            IEnumerable<int> creators = null,
            IEnumerable<int> characters = null,
            IEnumerable<int> events = null,
            IEnumerable<int> stories = null,
            IEnumerable<int> sharedAppearances = null,
            IEnumerable<int> collaborators = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest($"/series/{seriesId}/comics");

            if (format.HasValue)
            {
                request.AddParameter("format", format.Value.ToParameter());
            }

            if (formatType.HasValue)
            {
                request.AddParameter("formatType", formatType.Value.ToParameter());
            }

            if (noVariants.HasValue)
            {
                request.AddParameter("noVariants", noVariants.Value.ToString().ToLower());
            }

            if (dateDescript.HasValue)
            {
                request.AddParameter("dateDescriptor", dateDescript.Value.ToParameter());
            }

            if (dateRangeBegin.HasValue && dateRangeEnd.HasValue)
            {
                if (dateRangeBegin.Value <= dateRangeEnd.Value)
                {
                    request.AddParameter("dateRange",
                        $"{dateRangeBegin.Value:YYYY-MM-DD},{dateRangeEnd.Value:YYYY-MM-DD}");
                }
                else
                {
                    throw new ArgumentException("DateRangeBegin must be greater than DateRangeEnd");
                }
            }
            else if (dateRangeBegin.HasValue || dateRangeEnd.HasValue)
            {
                throw new ArgumentException("Date Range requires both a start and end date");
            }

            if (hasDigitalIssue.HasValue)
            {
                request.AddParameter("hasDigitalIssue", hasDigitalIssue.Value.ToString().ToLower());
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(creators, "creators");
            request.AddParameterList(characters, "characters");
            request.AddParameterList(events, "events");
            request.AddParameterList(stories, "stories");
            request.AddParameterList(sharedAppearances, "sharedAppearances");
            request.AddParameterList(collaborators, "collaborators");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.FocDate,
                OrderBy.FocDateDesc,
                OrderBy.OnSaleDate,
                OrderBy.OnSaleDateDesc,
                OrderBy.Title,
                OrderBy.TitleDesc,
                OrderBy.IssueNumber,
                OrderBy.IssueNumberDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<ComicDataWrapper> response = _client.Execute<ComicDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic creators whose work appears in a specific series, with optional filters.
        /// </summary>
        /// <param name="seriesId">The series ID.</param>
        /// <param name="firstName">Filter by creator first name (e.g. brian).</param>
        /// <param name="middleName">Filter by creator middle name (e.g. Michael).</param>
        /// <param name="lastName">Filter by creator last name (e.g. Bendis).</param>
        /// <param name="suffix">Filter by suffix or honorific (e.g. Jr., Sr.).</param>
        /// <param name="nameStartsWith">Filter by creator names that match critera (e.g. B, St L).</param>
        /// <param name="firstNameStartsWith">Filter by creator first names that match critera (e.g. B, St L).</param>
        /// <param name="middleNameStartsWith">Filter by creator middle names that match critera (e.g. Mi).</param>
        /// <param name="lastNameStartsWith">Filter by creator last names that match critera (e.g. Ben).</param>
        /// <param name="modifiedSince">Return only creators which have been modified since the specified date.</param>
        /// <param name="comics">Return only creators who worked on in the specified comics.</param>
        /// <param name="events">Return only creators who worked on comics that took place in the specified events.</param>
        /// <param name="stories">Return only creators who worked on the specified stories.</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic creators whose work appears in a specific series
        /// </returns>
        public IEnumerable<Creator> GetCreatorsForSeries(int seriesId,
            string firstName = null,
            string middleName = null,
            string lastName = null,
            string suffix = null,
            string nameStartsWith = null,
            string firstNameStartsWith = null,
            string middleNameStartsWith = null,
            string lastNameStartsWith = null,
            DateTime? modifiedSince = null,
            IEnumerable<int> comics = null,
            IEnumerable<int> events = null,
            IEnumerable<int> stories = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest($"/series/{seriesId}/creators");

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                request.AddParameter("firstName", firstName);
            }

            if (!string.IsNullOrWhiteSpace(middleName))
            {
                request.AddParameter("middleName", middleName);
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                request.AddParameter("lastName", lastName);
            }

            if (!string.IsNullOrWhiteSpace(suffix))
            {
                request.AddParameter("suffix", suffix);
            }

            if (!string.IsNullOrWhiteSpace(nameStartsWith))
            {
                request.AddParameter("nameStartsWith", nameStartsWith);
            }

            if (!string.IsNullOrWhiteSpace(firstNameStartsWith))
            {
                request.AddParameter("firstNameStartsWith", firstNameStartsWith);
            }

            if (!string.IsNullOrWhiteSpace(middleNameStartsWith))
            {
                request.AddParameter("middleNameStartsWith", middleNameStartsWith);
            }

            if (!string.IsNullOrWhiteSpace(lastNameStartsWith))
            {
                request.AddParameter("lastNameStartsWith", lastNameStartsWith);
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(comics, "comics");
            request.AddParameterList(events, "events");
            request.AddParameterList(stories, "stories");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.FirstName,
                OrderBy.FirstNameDesc,
                OrderBy.MiddleName,
                OrderBy.MiddleNameDesc,
                OrderBy.LastName,
                OrderBy.LastNameDesc,
                OrderBy.Suffix,
                OrderBy.SuffixDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<CreatorDataWrapper> response = _client.Execute<CreatorDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of events which occur in a specific series, with optional filters.
        /// </summary>
        /// <param name="seriesId">The series ID.</param>
        /// <param name="name">Filter the event list by name.</param>
        /// <param name="nameStartsWith">Return characters with names that begin with the specified string (e.g. Sp).</param>
        /// <param name="modifiedSince">Return only events which have been modified since the specified date.</param>
        /// <param name="creators">Return only events which feature work by the specified creators.</param>
        /// <param name="characters">Return only events which feature the specified characters.</param>
        /// <param name="comics">Return only events which take place in the specified comics.</param>
        /// <param name="stories">Return only events which contain the specified stories.</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of events which occur in a specific series
        /// </returns>
        public IEnumerable<Event> GetEventsForSeries(int seriesId,
            string name = null,
            string nameStartsWith = null,
            DateTime? modifiedSince = null,
            IEnumerable<int> creators = null,
            IEnumerable<int> characters = null,
            IEnumerable<int> comics = null,
            IEnumerable<int> stories = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest(string.Format("/series/{0}/events", seriesId));

            if (!string.IsNullOrWhiteSpace(name))
            {
                request.AddParameter("name", name);
            }

            if (!string.IsNullOrWhiteSpace(nameStartsWith))
            {
                request.AddParameter("nameStartsWith", nameStartsWith);
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(creators, "creators");
            request.AddParameterList(characters, "characters");
            request.AddParameterList(comics, "comics");
            request.AddParameterList(stories, "stories");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Name,
                OrderBy.NameDesc,
                OrderBy.StartDate,
                OrderBy.StartDateDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<EventDataWrapper> response = _client.Execute<EventDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic stories from a specific series with optional filters.
        /// </summary>
        /// <param name="seriesId">The series ID.</param>
        /// <param name="modifiedSince">Return only stories which have been modified since the specified date.</param>
        /// <param name="comics">Return only stories contained in the specified.</param>
        /// <param name="events">Return only stories which take place during the specified events.</param>
        /// <param name="creators">Return only stories which feature work by the specified creators.</param>
        /// <param name="characters">Return only stories which feature the specified characters.</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic stories from a specific series
        /// </returns>
        public IEnumerable<Story> GetStoriesForSeries(int seriesId,
            DateTime? modifiedSince = null,
            IEnumerable<int> comics = null,
            IEnumerable<int> events = null,
            IEnumerable<int> creators = null,
            IEnumerable<int> characters = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest($"/series/{seriesId}/stories");

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(comics, "comics");
            request.AddParameterList(events, "events");
            request.AddParameterList(creators, "creators");
            request.AddParameterList(characters, "characters");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Id,
                OrderBy.IdDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<StoryDataWrapper> response = _client.Execute<StoryDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        #endregion

        #region Stories

        /// <summary>
        /// Fetches lists of comic stories with optional filters.
        /// </summary>
        /// <param name="modifiedSince">Return only stories which have been modified since the specified date.</param>
        /// <param name="comics">Return only stories contained in the specified.</param>
        /// <param name="series">Return only stories contained the specified series.</param>
        /// <param name="events">Return only stories which take place during the specified events.</param>
        /// <param name="creators">Return only stories which feature work by the specified creators.</param>
        /// <param name="characters">Return only stories which feature the specified characters.</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic stories
        /// </returns>
        public IEnumerable<Story> GetStories(DateTime? modifiedSince = null,
            IEnumerable<int> comics = null,
            IEnumerable<int> series = null,
            IEnumerable<int> events = null,
            IEnumerable<int> creators = null,
            IEnumerable<int> characters = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest("/stories");

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(comics, "comics");
            request.AddParameterList(series, "series");
            request.AddParameterList(events, "events");
            request.AddParameterList(creators, "creators");
            request.AddParameterList(characters, "characters");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Id,
                OrderBy.IdDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.GetValueOrDefault()>0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.GetValueOrDefault()>0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<StoryDataWrapper> response = _client.Execute<StoryDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// This method fetches a single comic story resource. It is the canonical URI for any comic story resource provided by the API.
        /// </summary>
        /// <param name="StoryId">The story ID.</param>
        /// <returns>
        /// A single comic story resource
        /// </returns>
        public Story GetStory(int StoryId)
        {
            var request = CreateRequest(string.Format("/stories/{0}", StoryId));

            IRestResponse<StoryDataWrapper> response = _client.Execute<StoryDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results.FirstOrDefault(story => story.Id == StoryId);
        }

        /// <summary>
        /// Fetches lists of comic characters appearing in a single story, with optional filters.
        /// </summary>
        /// <param name="storyId">The story ID.</param>
        /// <param name="name">Return only characters matching the specified full character name (e.g. Spider-Man).</param>
        /// <param name="nameStartsWith">Return characters with names that begin with the specified string (e.g. Sp).</param>
        /// <param name="modifiedSince">Return only characters which have been modified since the specified date.</param>
        /// <param name="comics">Return only characters which appear in the specified comics.</param>
        /// <param name="series">Return only characters which appear the specified series.</param>
        /// <param name="events">Return only characters which appear comics that took place in the specified events.</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic characters appearing in a single story
        /// </returns>
        public IEnumerable<Character> GetCharactersForStory(int storyId,
            string name = null,
            string nameStartsWith = null,
            DateTime? modifiedSince = null,
            IEnumerable<int> comics = null,
            IEnumerable<int> series = null,
            IEnumerable<int> events = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest($"/stories/{storyId}/characters");

            if (!string.IsNullOrWhiteSpace(name))
            {
                request.AddParameter("name", name);
            }

            if (!string.IsNullOrWhiteSpace(nameStartsWith))
            {
                request.AddParameter("nameStartsWith", nameStartsWith);
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(comics, "comics");
            request.AddParameterList(events, "events");
            request.AddParameterList(series, "series");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Name,
                OrderBy.NameDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<CharacterDataWrapper> response = _client.Execute<CharacterDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comics in which a specific story appears, with optional filters.
        /// </summary>
        /// <param name="storyId">The story ID.</param>
        /// <param name="format">Filter by the issue format.</param>
        /// <param name="formatType">Filter by the issue format type.</param>
        /// <param name="noVariants">Exclude variant comics from the result set.</param>
        /// <param name="dateDescript">Return comics within a predefined date range.</param>
        /// <param name="dateRangeBegin">Return comics within a predefined date range, this is the beginning of range.</param>
        /// <param name="dateRangeEnd">Return comics within a predefined date range, this is the end of range.</param>
        /// <param name="hasDigitalIssue">Include only results which are available digitally.</param>
        /// <param name="modifiedSince">Return only comics which have been modified since the specified date.</param>
        /// <param name="creators">Return only comics which feature work by the specified creators.</param>
        /// <param name="characters">Return only comics which feature the specified characters.</param>
        /// <param name="series">Return only comics which are part of the specified series.</param>
        /// <param name="events">Return only comics which take place in the specified events.</param>
        /// <param name="sharedAppearances">Return only comics in which the specified characters appear together (for example in which BOTH Spider-Man and Wolverine appear).</param>
        /// <param name="collaborators">Return only comics in which the specified creators worked together (for example in which BOTH Stan Lee and Jack Kirby did work).</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comics in which a specific story appears
        /// </returns>
        public IEnumerable<Comic> GetComicsForStory(int storyId,
            ComicFormat? format = null,
            ComicFormatType? formatType = null,
            bool? noVariants = null,
            DateDescriptor? dateDescript = null,
            DateTime? dateRangeBegin = null,
            DateTime? dateRangeEnd = null,
            bool? hasDigitalIssue = null,
            DateTime? modifiedSince = null,
            IEnumerable<int> creators = null,
            IEnumerable<int> characters = null,
            IEnumerable<int> series = null,
            IEnumerable<int> events = null,
            IEnumerable<int> sharedAppearances = null,
            IEnumerable<int> collaborators = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest($"/stories/{storyId}/comics");

            if (format.HasValue)
            {
                request.AddParameter("format", format.Value.ToParameter());
            }

            if (formatType.HasValue)
            {
                request.AddParameter("formatType", formatType.Value.ToParameter());
            }

            if (noVariants.HasValue)
            {
                request.AddParameter("noVariants", noVariants.Value.ToString().ToLower());
            }

            if (dateDescript.HasValue)
            {
                request.AddParameter("dateDescriptor", dateDescript.Value.ToParameter());
            }

            if (dateRangeBegin.HasValue && dateRangeEnd.HasValue)
            {
                if (dateRangeBegin.Value <= dateRangeEnd.Value)
                {
                    request.AddParameter("dateRange",
                        string.Format("{0},{1}", dateRangeBegin.Value.ToString("YYYY-MM-DD"),
                            dateRangeEnd.Value.ToString("YYYY-MM-DD")));
                }
                else
                {
                    throw new ArgumentException("DateRangeBegin must be greater than DateRangeEnd");
                }
            }
            else if (dateRangeBegin.HasValue || dateRangeEnd.HasValue)
            {
                throw new ArgumentException("Date Range requires both a start and end date");
            }

            if (hasDigitalIssue.HasValue)
            {
                request.AddParameter("hasDigitalIssue", hasDigitalIssue.Value.ToString().ToLower());
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(creators, "creators");
            request.AddParameterList(characters, "characters");
            request.AddParameterList(series, "series");
            request.AddParameterList(events, "events");
            request.AddParameterList(sharedAppearances, "sharedAppearances");
            request.AddParameterList(collaborators, "collaborators");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.FocDate,
                OrderBy.FocDateDesc,
                OrderBy.OnSaleDate,
                OrderBy.OnSaleDateDesc,
                OrderBy.Title,
                OrderBy.TitleDesc,
                OrderBy.IssueNumber,
                OrderBy.IssueNumberDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<ComicDataWrapper> response = _client.Execute<ComicDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic creators whose work appears in a specific story, with optional filters.
        /// </summary>
        /// <param name="storyId">The story ID.</param>
        /// <param name="firstName">Filter by creator first name (e.g. brian).</param>
        /// <param name="middleName">Filter by creator middle name (e.g. Michael).</param>
        /// <param name="lastName">Filter by creator last name (e.g. Bendis).</param>
        /// <param name="suffix">Filter by suffix or honorific (e.g. Jr., Sr.).</param>
        /// <param name="nameStartsWith">Filter by creator names that match critera (e.g. B, St L).</param>
        /// <param name="firstNameStartsWith">Filter by creator first names that match critera (e.g. B, St L).</param>
        /// <param name="middleNameStartsWith">Filter by creator middle names that match critera (e.g. Mi).</param>
        /// <param name="lastNameStartsWith">Filter by creator last names that match critera (e.g. Ben).</param>
        /// <param name="modifiedSince">Return only creators which have been modified since the specified date.</param>
        /// <param name="comics">Return only creators who worked on in the specified comics.</param>
        /// <param name="series">Return only creators who worked on the specified series.</param>
        /// <param name="events">Return only creators who worked on comics that took place in the specified events.</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic creators whose work appears in a specific story
        /// </returns>
        public IEnumerable<Creator> GetCreatorsForStory(int storyId,
            string firstName = null,
            string middleName = null,
            string lastName = null,
            string suffix = null,
            string nameStartsWith = null,
            string firstNameStartsWith = null,
            string middleNameStartsWith = null,
            string lastNameStartsWith = null,
            DateTime? modifiedSince = null,
            IEnumerable<int> comics = null,
            IEnumerable<int> series = null,
            IEnumerable<int> events = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest($"/stories/{storyId}/creators");

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                request.AddParameter("firstName", firstName);
            }

            if (!string.IsNullOrWhiteSpace(middleName))
            {
                request.AddParameter("middleName", middleName);
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                request.AddParameter("lastName", lastName);
            }

            if (!string.IsNullOrWhiteSpace(suffix))
            {
                request.AddParameter("suffix", suffix);
            }

            if (!string.IsNullOrWhiteSpace(nameStartsWith))
            {
                request.AddParameter("nameStartsWith", nameStartsWith);
            }

            if (!string.IsNullOrWhiteSpace(firstNameStartsWith))
            {
                request.AddParameter("firstNameStartsWith", firstNameStartsWith);
            }

            if (!string.IsNullOrWhiteSpace(middleNameStartsWith))
            {
                request.AddParameter("middleNameStartsWith", middleNameStartsWith);
            }

            if (!string.IsNullOrWhiteSpace(lastNameStartsWith))
            {
                request.AddParameter("lastNameStartsWith", lastNameStartsWith);
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(comics, "comics");
            request.AddParameterList(series, "series");
            request.AddParameterList(events, "events");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.FirstName,
                OrderBy.FirstNameDesc,
                OrderBy.MiddleName,
                OrderBy.MiddleNameDesc,
                OrderBy.LastName,
                OrderBy.LastNameDesc,
                OrderBy.Suffix,
                OrderBy.SuffixDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<CreatorDataWrapper> response = _client.Execute<CreatorDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of events in which a specific story appears, with optional filters.
        /// </summary>
        /// <param name="storyId">The story ID.</param>
        /// <param name="name">Filter the event list by name.</param>
        /// <param name="nameStartsWith">Return characters with names that begin with the specified string (e.g. Sp).</param>
        /// <param name="modifiedSince">Return only events which have been modified since the specified date.</param>
        /// <param name="creators">Return only events which feature work by the specified creators.</param>
        /// <param name="characters">Return only events which feature the specified characters.</param>
        /// <param name="series">Return only events which are part of the specified series.</param>
        /// <param name="comics">Return only events which take place in the specified comics.</param>
        /// <param name="order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="limit">Limit the result set to the specified number of resources.</param>
        /// <param name="offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of events in which a specific story appears
        /// </returns>
        public IEnumerable<Event> GetEventsForStories(int storyId,
            string name = null,
            string nameStartsWith = null,
            DateTime? modifiedSince = null,
            IEnumerable<int> creators = null,
            IEnumerable<int> characters = null,
            IEnumerable<int> series = null,
            IEnumerable<int> comics = null,
            IEnumerable<OrderBy> order = null,
            int? limit = null,
            int? offset = null)
        {
            var request = CreateRequest(string.Format("/stories/{0}/events/", storyId));

            if (!string.IsNullOrWhiteSpace(name))
            {
                request.AddParameter("name", name);
            }

            if (!string.IsNullOrWhiteSpace(nameStartsWith))
            {
                request.AddParameter("nameStartsWith", nameStartsWith);
            }

            if (modifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", modifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(creators, "creators");
            request.AddParameterList(characters, "characters");
            request.AddParameterList(series, "series");
            request.AddParameterList(comics, "comics");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Name,
                OrderBy.NameDesc,
                OrderBy.StartDate,
                OrderBy.StartDateDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(order, availableOrderBy);

            if (limit.HasValue && limit.Value > 0)
            {
                request.AddParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue && offset.Value > 0)
            {
                request.AddParameter("offset", offset.Value.ToString());
            }

            IRestResponse<EventDataWrapper> response = _client.Execute<EventDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        #endregion
    }

    #region Extras

    public class MarvelError
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }

    public class MarvelUrl
    {
        public string Type { get; set; }
        public string Url { get; set; }
    }

    public class MarvelImage
    {
        public string Path { get; set; }
        public string Extension { get; set; }

        public override string ToString()
        {
            return $"{Path}.{Extension}";
        }

        public string ToString(Image size)
        {
            return $"{Path}{size.ToParameter()}.{Extension}";
        }
    }

    public class TextObject
    {
        public string Type { get; set; }
        public string Language { get; set; }
        public string Text { get; set; }
    }

    public static class RestExtensions
    {
        public static void AddParameterList(this RestRequest request, IEnumerable<int> parameter,
            string parameterString)
        {
            if ((parameter?.Any()).GetValueOrDefault(false))
            {
                request.AddParameter(parameterString, string.Join<int>(",", parameter));
            }
        }

        public static void AddOrderByParameterList(this RestRequest request, IEnumerable<OrderBy> parameter,
            IEnumerable<OrderBy> available)
        {
            if ((parameter?.Any()).GetValueOrDefault(false))
            {
                StringBuilder orderString = new StringBuilder();
                foreach (var order in parameter)
                {
                    if (available.Contains(order))
                    {
                        if (orderString.Length > 0)
                        {
                            orderString.Append(",");
                        }

                        orderString.Append(order.ToParameter());
                        break;
                    }
                }

                if (orderString.Length > 0)
                {
                    request.AddParameter("orderBy", orderString.ToString());
                }
            }
        }
    }

    #endregion
}