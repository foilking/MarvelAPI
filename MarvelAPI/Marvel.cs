using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IdentityModel;

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
        private RestClient _client;

        public Marvel(string publicApiKey, string privateApiKey, bool? useGZip = null)
        {
            _publicApiKey = publicApiKey;
            _privateApiKey = privateApiKey;
            _useGZip = useGZip.HasValue ? useGZip.Value : false;

            _client = new RestClient(BASE_URL);
        }

        private string CreateHash(string input)
        {
            var hash = String.Empty;
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

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

            request.AddParameter("apikey", _publicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, _privateApiKey, _publicApiKey)));

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
            var status = String.Empty;
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

                if (data is CharacterDataWrapper)
                {
                    code = (data as CharacterDataWrapper).Code;
                    status = (data as CharacterDataWrapper).Status;
                }
                else if (data is ComicDataWrapper)
                {
                    code = (data as ComicDataWrapper).Code;
                    status = (data as ComicDataWrapper).Status;
                }
                else if (data is CreatorDataWrapper)
                {
                    code = (data as CreatorDataWrapper).Code;
                    status = (data as CreatorDataWrapper).Status;
                }
                else if (data is EventDataWrapper)
                {
                    code = (data as EventDataWrapper).Code;
                    status = (data as EventDataWrapper).Status;
                }
                else if (data is SeriesDataWrapper)
                {
                    code = (data as SeriesDataWrapper).Code;
                    status = (data as SeriesDataWrapper).Status;
                }
                else if (data is StoryDataWrapper)
                {
                    code = (data as StoryDataWrapper).Code;
                    status = (data as StoryDataWrapper).Status;
                }
            }

            switch (code)
            {
                case 409:
                    throw new ArgumentException(status);
                case 404:
                    throw new ApplicationException(status, response.ErrorException);
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
        /// <param name="Name">Return only characters matching the specified full character name (e.g. Spider-Man).</param>
        /// <param name="NameStartsWith">Return characters with names that begin with the specified string (e.g. Sp).</param>
        /// <param name="ModifiedSince">Return only characters which have been modified since the specified date.</param>
        /// <param name="Comics">Return only characters which appear in the specified comics.</param>
        /// <param name="Series">Return only characters which appear the specified series.</param>
        /// <param name="Events">Return only characters which appear in the specified events.</param>
        /// <param name="Stories">Return only characters which appear the specified stories.</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources, between 1 - 100.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// List of comic characters
        /// </returns>
        public IEnumerable<Character> GetCharacters(string Name = null,
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
            var request = CreateRequest("/characters");
            if (!String.IsNullOrWhiteSpace(Name))
            {
                request.AddParameter("name", Name);
            }
            if (!String.IsNullOrWhiteSpace(NameStartsWith))
            {
                request.AddParameter("nameStartsWith", NameStartsWith);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Comics, "comics");
            request.AddParameterList(Series, "series");
            request.AddParameterList(Events, "events");
            request.AddParameterList(Stories, "stories");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Name,
                OrderBy.NameDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value >= 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            IRestResponse<CharacterDataWrapper> response = _client.Execute<CharacterDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetch a single character by Id.
        /// </summary>
        /// <param name="CharacterId">A single character id.</param>
        /// <returns>Character details</returns>
        public Character GetCharacter(int CharacterId)
        {
            var request = CreateRequest(String.Format("/characters/{0}", CharacterId));

            IRestResponse<CharacterDataWrapper> response = _client.Execute<CharacterDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results.FirstOrDefault(character => character.Id == CharacterId);
        }

        /// <summary>
        /// Fetches lists of comics containing specific character, with optional filters.
        /// </summary>
        /// <param name="CharacterId">The character id.</param>
        /// <param name="Format">Filter by the issue format.</param>
        /// <param name="FormatType">Filter by the issue format type.</param>
        /// <param name="NoVariants">Exclude variant comics from the result set.</param>
        /// <param name="DateDescript">Return comics within a predefined date range</param>
        /// <param name="DateRangeBegin">Return comics within a predefined date range, this date being the beginning.</param>
        /// <param name="DateRangeEnd">Return comics within a predefined date range, this date being the ending.</param>
        /// <param name="HasDigitalIssue">Include only results which are available digitally.</param>
        /// <param name="ModifiedSince">Return only comics which have been modified since the specified date.</param>
        /// <param name="Creators">Return only comics which feature work by the specified creators.</param>
        /// <param name="Series">Return only comics which are part of the specified series.</param>
        /// <param name="Events">Return only comics which take place in the specified events.</param>
        /// <param name="Stories">Return only comics which contain the specified stories.</param>
        /// <param name="SharedAppearances">Return only comics in which the specified characters appear together (for example in which BOTH Spider-Man and Wolverine appear).</param>
        /// <param name="Collaborators">Return only comics in which the specified creators worked together (for example in which BOTH Stan Lee and Jack Kirby did work).</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comics
        /// </returns>
        public IEnumerable<Comic> GetComicsForCharacter(int CharacterId,
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
            var request = CreateRequest(String.Format("/characters/{0}/comics", CharacterId));
            if (Format.HasValue)
            {
                request.AddParameter("format", Format.Value.ToParameter());
            }
            if (FormatType.HasValue)
            {
                request.AddParameter("formatType", FormatType.Value.ToParameter());
            }
            if (NoVariants.HasValue)
            {
                request.AddParameter("noVariants", NoVariants.Value.ToString().ToLower());
            }
            if (DateDescript.HasValue)
            {
                request.AddParameter("dateDescriptor", DateDescript.Value.ToParameter());
            }
            if (DateRangeBegin.HasValue && DateRangeEnd.HasValue)
            {
                if (DateRangeBegin.Value <= DateRangeEnd.Value)
                {
                    request.AddParameter("dateRange", String.Format("{0},{1}", DateRangeBegin.Value.ToString("YYYY-MM-DD"), DateRangeEnd.Value.ToString("YYYY-MM-DD")));
                }
                else
                {
                    throw new ArgumentException("DateRangeBegin must be greater than DateRangeEnd");
                }
            }
            else if (DateRangeBegin.HasValue || DateRangeEnd.HasValue)
            {
                throw new ArgumentException("Date Range requires both a start and end date");
            }
            if (HasDigitalIssue.HasValue)
            {
                request.AddParameter("hasDigitalIssue", HasDigitalIssue.Value.ToString().ToLower());
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            request.AddParameterList(Creators, "creators");
            request.AddParameterList(Series, "series");
            request.AddParameterList(Events, "events");
            request.AddParameterList(Stories, "stories");
            request.AddParameterList(SharedAppearances, "sharedAppearances");
            request.AddParameterList(Collaborators, "collaborators");

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
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            IRestResponse<ComicDataWrapper> response = _client.Execute<ComicDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of events in which a specific character appears, with optional filters.
        /// </summary>
        /// <param name="CharacterId">The character id</param>
        /// <param name="Name">Filter the event list by name.</param>
        /// <param name="NameStartsWith">Return characters with names that begin with the specified string (e.g. Sp).</param>
        /// <param name="ModifiedSince">Return only events which have been modified since the specified date.</param>
        /// <param name="Creators">Return only events which feature work by the specified creators.</param>
        /// <param name="Series">Return only events which are part of the specified series.</param>
        /// <param name="Comics">Return only events which take place in the specified comics.</param>
        /// <param name="Stories">Return only events which contain the specified stories.</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of events
        /// </returns>
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
            var request = CreateRequest(String.Format("/characters/{0}/events/", CharacterId));

            if (!String.IsNullOrWhiteSpace(Name))
            {
                request.AddParameter("name", Name);
            }

            if (!String.IsNullOrWhiteSpace(NameStartsWith))
            {
                request.AddParameter("nameStartsWith", NameStartsWith);
            }

            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Creators, "creators");
            request.AddParameterList(Series, "series");
            request.AddParameterList(Comics, "comics");
            request.AddParameterList(Stories, "stories");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Name,
                OrderBy.NameDesc,
                OrderBy.StartDate,
                OrderBy.StartDateDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc

            };
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            IRestResponse<EventDataWrapper> response = _client.Execute<EventDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic series in which a specific character appears, with optional filters.
        /// </summary>
        /// <param name="CharacterId">The character ID</param>
        /// <param name="Title">Filter by series title.</param>
        /// <param name="TitleStartsWith">Return titles that begin with the specified string (e.g. Sp).</param>
        /// <param name="ModifiedSince">Return only series which have been modified since the specified date.</param>
        /// <param name="Comics">Return only series which contain the specified comics.</param>
        /// <param name="Stories">Return only series which contain the specified stories.</param>
        /// <param name="Events">Return only series which have comics that take place during the specified events.</param>
        /// <param name="Creators">Return only series which feature work by the specified creators.</param>
        /// <param name="SeriesType">Filter the series by publication frequency type.</param>
        /// <param name="Contains">Return only series containing one or more comics with the specified format.</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic series
        /// </returns>
        public IEnumerable<Series> GetSeriesForCharacter(int CharacterId,
                                            string Title = null,
                                            string TitleStartsWith = null,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Stories = null,
                                            IEnumerable<int> Events = null,
                                            IEnumerable<int> Creators = null,
                                            SeriesType? SeriesType = null,
                                            IEnumerable<ComicFormat> Contains = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var request = CreateRequest(String.Format("/characters/{0}/series/", CharacterId));

            if (!String.IsNullOrWhiteSpace(Title))
            {
                request.AddParameter("title", Title);
            }
            if (!String.IsNullOrWhiteSpace(TitleStartsWith))
            {
                request.AddParameter("titleStartsWith", TitleStartsWith);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Comics, "comics");
            request.AddParameterList(Stories, "stories");
            request.AddParameterList(Events, "events");
            request.AddParameterList(Creators, "creators");

            if (SeriesType.HasValue)
            {
                request.AddParameter("seriesType", SeriesType.Value.ToParameter());
            }
            if (Contains != null && Contains.Any())
            {
                var containsParameters = Contains.Select(contain => contain.ToParameter());
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
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            IRestResponse<SeriesDataWrapper> response = _client.Execute<SeriesDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic stories featuring a specific character with optional filters.
        /// </summary>
        /// <param name="CharacterId">The character ID.</param>
        /// <param name="ModifiedSince">Return only stories which have been modified since the specified date.</param>
        /// <param name="Comics">Return only stories contained in the specified.</param>
        /// <param name="Series">Return only stories contained the specified series.</param>
        /// <param name="Events">Return only stories which take place during the specified events.</param>
        /// <param name="Creators">Return only stories which feature work by the specified creators.</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns></returns>
        public IEnumerable<Story> GetStoriessForCharacter(int CharacterId,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Events = null,
                                            IEnumerable<int> Creators = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var request = CreateRequest(String.Format("/characters/{0}/stories/", CharacterId));

            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            request.AddParameterList(Creators, "creators");
            request.AddParameterList(Series, "series");
            request.AddParameterList(Comics, "comics");
            request.AddParameterList(Events, "events");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Id,
                OrderBy.IdDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
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
        /// <param name="Format">Filter by the issue format.</param>
        /// <param name="FormatType">Filter by the issue format type.</param>
        /// <param name="NoVariants">Exclude variants (alternate covers, secondary printings, director's cuts, etc.) from the result set.</param>
        /// <param name="DateDescript">Return comics within a predefined date range.</param>
        /// <param name="DateRangeBegin">Return comics within a predefined date range, this date being the beginning.</param>
        /// <param name="DateRangeEnd">Return comics within a predefined date range, this date being the ending.</param>
        /// <param name="HasDigitalIssue">Include only results which are available digitally.</param>
        /// <param name="ModifiedSince">Return only comics which have been modified since the specified date.</param>
        /// <param name="Creators">Return only comics which feature work by the specified creators.</param>
        /// <param name="Characters">Return only comics which feature the specified characters.</param>
        /// <param name="Series">Return only comics which are part of the specified series.</param>
        /// <param name="Events">Return only comics which take place in the specified events.</param>
        /// <param name="Stories">Return only comics which contain the specified stories.</param>
        /// <param name="SharedAppearances">Return only comics in which the specified characters appear together (for example in which BOTH Spider-Man and Wolverine appear).</param>
        /// <param name="Collaborators">Return only comics in which the specified creators worked together (for example in which BOTH Stan Lee and Jack Kirby did work).</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comics
        /// </returns>
        public IEnumerable<Comic> GetComics(ComicFormat? Format = null,
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
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var request = CreateRequest("/comics");

            if (Format.HasValue)
            {
                request.AddParameter("format", Format.Value.ToParameter());
            }
            if (FormatType.HasValue)
            {
                request.AddParameter("formatType", FormatType.Value.ToParameter());
            }
            if (NoVariants.HasValue)
            {
                request.AddParameter("noVariants", NoVariants.Value.ToString().ToLower());
            }
            if (DateDescript.HasValue)
            {
                request.AddParameter("dateDescriptor", DateDescript.Value.ToParameter());
            }
            if (DateRangeBegin.HasValue && DateRangeEnd.HasValue)
            {
                if (DateRangeBegin.Value <= DateRangeEnd.Value)
                {
                    request.AddParameter("dateRange", String.Format("{0},{1}", DateRangeBegin.Value.ToString("YYYY-MM-DD"), DateRangeEnd.Value.ToString("YYYY-MM-DD")));
                }
                else
                {
                    throw new ArgumentException("DateRangeBegin must be greater than DateRangeEnd");
                }
            }
            else if (DateRangeBegin.HasValue || DateRangeEnd.HasValue)
            {
                throw new ArgumentException("Date Range requires both a start and end date");
            }
            if (HasDigitalIssue.HasValue)
            {
                request.AddParameter("hasDigitalIssue", HasDigitalIssue.Value.ToString().ToLower());
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Creators, "creators");
            request.AddParameterList(Characters, "characters");
            request.AddParameterList(Series, "series");
            request.AddParameterList(Events, "events");
            request.AddParameterList(Stories, "stories");
            request.AddParameterList(SharedAppearances, "sharedAppearances");
            request.AddParameterList(Collaborators, "collaborators");

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
            request.AddOrderByParameterList(Order, availableOrder);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
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
        public Comic GetComic(int ComicId)
        {
            var request = CreateRequest(String.Format("/comics/{0}", ComicId));

            IRestResponse<ComicDataWrapper> response = _client.Execute<ComicDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results.FirstOrDefault(comic => comic.Id == ComicId);
        }

        /// <summary>
        /// Fetches lists of characters which appear in a specific comic with optional filters.
        /// </summary>
        /// <param name="ComicId">The comic id.</param>
        /// <param name="Name">Return only characters matching the specified full character name (e.g. Spider-Man).</param>
        /// <param name="NameStartsWith">Return characters with names that begin with the specified string (e.g. Sp).</param>
        /// <param name="ModifiedSince">Return only characters which have been modified since the specified date.</param>
        /// <param name="Series">Return only characters which appear the specified series.</param>
        /// <param name="Events">Return only characters which appear comics that took place in the specified events.</param>
        /// <param name="Stories">Return only characters which appear the specified stories .</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of characters who appear in a specific comic
        /// </returns>
        public IEnumerable<Character> GetCharactersForComic(int ComicId,
                                            string Name = null,
                                            string NameStartsWith = null,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Events = null,
                                            IEnumerable<int> Stories = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var request = CreateRequest(String.Format("/comics/{0}/characters", ComicId));

            if (!String.IsNullOrWhiteSpace(Name))
            {
                request.AddParameter("name", Name);
            }
            if (!String.IsNullOrWhiteSpace(NameStartsWith))
            {
                request.AddParameter("nameStartsWith", NameStartsWith);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Series, "series");
            request.AddParameterList(Events, "events");
            request.AddParameterList(Stories, "stories");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Name,
                OrderBy.NameDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            IRestResponse<CharacterDataWrapper> response = _client.Execute<CharacterDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic creators whose work appears in a specific comic, with optional filters.
        /// </summary>
        /// <param name="ComicId">The comic id.</param>
        /// <param name="FirstName">Filter by creator first name (e.g. brian).</param>
        /// <param name="MiddleName">Filter by creator middle name (e.g. Michael).</param>
        /// <param name="LastName">Filter by creator last name (e.g. Bendis).</param>
        /// <param name="Suffix">Filter by suffix or honorific (e.g. Jr., Sr.).</param>
        /// <param name="NameStartsWith">Filter by creator names that match critera (e.g. B, St L).</param>
        /// <param name="FirstNameStartsWith">Filter by creator first names that match critera (e.g. B, St L).</param>
        /// <param name="MiddleNameStartsWith">Filter by creator middle names that match critera (e.g. Mi).</param>
        /// <param name="LastNameStartsWith">Filter by creator last names that match critera (e.g. Ben).</param>
        /// <param name="ModifiedSince">Return only creators which have been modified since the specified date.</param>
        /// <param name="Comics">Return only creators who worked on in the specified comics.</param>
        /// <param name="Series">Return only creators who worked on the specified series.</param>
        /// <param name="Stories">Return only creators who worked on the specified stories.</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic creators whose work appears in a specific comic
        /// </returns>
        public IEnumerable<Creator> GetCreatorsForComic(int ComicId,
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
                                            IEnumerable<int> Stories = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var request = CreateRequest(String.Format("/comics/{0}/characters", ComicId));

            if (!String.IsNullOrWhiteSpace(FirstName))
            {
                request.AddParameter("firstName", FirstName);
            }
            if (!String.IsNullOrWhiteSpace(MiddleName))
            {
                request.AddParameter("middleName", MiddleName);
            }
            if (!String.IsNullOrWhiteSpace(LastName))
            {
                request.AddParameter("lastName", LastName);
            }
            if (!String.IsNullOrWhiteSpace(Suffix))
            {
                request.AddParameter("suffix", Suffix);
            }
            if (!String.IsNullOrWhiteSpace(NameStartsWith))
            {
                request.AddParameter("nameStartsWith", NameStartsWith);
            }
            if (!String.IsNullOrWhiteSpace(FirstNameStartsWith))
            {
                request.AddParameter("firstNameStartsWith", FirstNameStartsWith);
            }
            if (!String.IsNullOrWhiteSpace(MiddleNameStartsWith))
            {
                request.AddParameter("middleNameStartsWith", MiddleNameStartsWith);
            }
            if (!String.IsNullOrWhiteSpace(LastNameStartsWith))
            {
                request.AddParameter("lastNameStartsWith", LastNameStartsWith);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Series, "series");
            request.AddParameterList(Comics, "comics");
            request.AddParameterList(Stories, "stories");

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
            request.AddOrderByParameterList(Order, availableOrderBy);

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
        /// Fetches lists of events in which a specific comic appears, with optional filters.
        /// </summary>
        /// <param name="ComicId">The comic ID.</param>
        /// <param name="Name">Filter the event list by name.</param>
        /// <param name="NameStartsWith">Return characters with names that begin with the specified string (e.g. Sp).</param>
        /// <param name="ModifiedSince">Return only events which have been modified since the specified date.</param>
        /// <param name="Creators">Return only events which feature work by the specified creators.</param>
        /// <param name="Characters">Return only events which feature the specified characters.</param>
        /// <param name="Series">Return only events which are part of the specified series.</param>
        /// <param name="Stories">Return only events which contain the specified stories.</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of events in which a specific comic appears
        /// </returns>
        public IEnumerable<Event> GetEventsForComic(int ComicId,
                                            string Name = null,
                                            string NameStartsWith = null,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Creators = null,
                                            IEnumerable<int> Characters = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Stories = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var request = CreateRequest(String.Format("/comics/{0}/events/", ComicId));

            if (!String.IsNullOrWhiteSpace(Name))
            {
                request.AddParameter("name", Name);
            }
            if (!String.IsNullOrWhiteSpace(NameStartsWith))
            {
                request.AddParameter("nameStartsWith", NameStartsWith);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Creators, "creators");
            request.AddParameterList(Characters, "characters");
            request.AddParameterList(Series, "series");
            request.AddParameterList(Stories, "stories");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Name,
                OrderBy.NameDesc,
                OrderBy.StartDate,
                OrderBy.StartDateDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            IRestResponse<EventDataWrapper> response = _client.Execute<EventDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic stories in a specific comic issue, with optional filters.
        /// </summary>
        /// <param name="ComicId">The comic ID.</param>
        /// <param name="ModifiedSince">Return only stories which have been modified since the specified date.</param>
        /// <param name="Series">Return only stories contained the specified series.</param>
        /// <param name="Events">Return only stories which take place during the specified event.</param>
        /// <param name="Creators">Return only stories which feature work by the specified creators.</param>
        /// <param name="Characters">Return only stories which feature the specified characters.</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources.</param>
        /// <returns>
        /// Lists of comic stories in a specific comic issue
        /// </returns>
        public IEnumerable<Story> GetStoriesForComic(int ComicId,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Events = null,
                                            IEnumerable<int> Creators = null,
                                            IEnumerable<int> Characters = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var request = CreateRequest(String.Format("/comics/{0}/events/", ComicId));

            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Series, "series");
            request.AddParameterList(Events, "events");
            request.AddParameterList(Creators, "creators");
            request.AddParameterList(Characters, "characters");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Id,
                OrderBy.IdDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
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
        /// <param name="FirstName">Filter by creator first name (e.g. Brian).</param>
        /// <param name="MiddleName">Filter by creator middle name (e.g. Michael).</param>
        /// <param name="LastName">Filter by creator last name (e.g. Bendis).</param>
        /// <param name="Suffix">Filter by suffix or honorific (e.g. Jr., Sr.).</param>
        /// <param name="NameStartsWith">Filter by creator names that match critera (e.g. B, St L).</param>
        /// <param name="FirstNameStartsWith">Filter by creator first names that match critera (e.g. B, St L).</param>
        /// <param name="MiddleNameStartsWith">Filter by creator middle names that match critera (e.g. Mi).</param>
        /// <param name="LastNameStartsWith">Filter by creator last names that match critera (e.g. Ben).</param>
        /// <param name="ModifiedSince">Return only creators which have been modified since the specified date.</param>
        /// <param name="Comics">Return only creators who worked on in the specified comics.</param>
        /// <param name="Series">Return only creators who worked on the specified series.</param>
        /// <param name="Events">Return only creators who worked on comics that took place in the specified events.</param>
        /// <param name="Stories">Return only creators who worked on the specified stories.</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic creators
        /// </returns>
        public IEnumerable<Creator> GetCreators(string FirstName = null,
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
                                                IEnumerable<int> Stories = null,
                                                IEnumerable<OrderBy> Order = null,
                                                int? Limit = null,
                                                int? Offset = null)
        {
            var request = CreateRequest("/creators");

            if (!String.IsNullOrWhiteSpace(FirstName))
            {
                request.AddParameter("firstName", FirstName);
            }
            if (!String.IsNullOrWhiteSpace(MiddleName))
            {
                request.AddParameter("middleName", MiddleName);
            }
            if (!String.IsNullOrWhiteSpace(LastName))
            {
                request.AddParameter("lastName", LastName);
            }
            if (!String.IsNullOrWhiteSpace(Suffix))
            {
                request.AddParameter("suffix", Suffix);
            }
            if (!String.IsNullOrWhiteSpace(NameStartsWith))
            {
                request.AddParameter("nameStartsWith", NameStartsWith);
            }
            if (!String.IsNullOrWhiteSpace(FirstNameStartsWith))
            {
                request.AddParameter("firstNameStartsWith", FirstNameStartsWith);
            }
            if (!String.IsNullOrWhiteSpace(MiddleNameStartsWith))
            {
                request.AddParameter("middleNameStartsWith", MiddleNameStartsWith);
            }
            if (!String.IsNullOrWhiteSpace(LastNameStartsWith))
            {
                request.AddParameter("lastNameStartsWith", LastNameStartsWith);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Comics, "comics");
            request.AddParameterList(Series, "series");
            request.AddParameterList(Events, "events");
            request.AddParameterList(Stories, "stories");

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
            request.AddOrderByParameterList(Order, availableOrderBy);

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
            var request = CreateRequest(String.Format("/creators/{0}", CreatorId));

            IRestResponse<CreatorDataWrapper> response = _client.Execute<CreatorDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results.FirstOrDefault(creator => creator.Id == CreatorId);
        }

        /// <summary>
        /// Fetches lists of comics in which the work of a specific creator appears, with optional filters.
        /// </summary>
        /// <param name="CreatorId">The creator ID.</param>
        /// <param name="Format">Filter by the issue format.</param>
        /// <param name="FormatType">Filter by the issue format type.</param>
        /// <param name="NoVariants">Exclude variant comics from the result set.</param>
        /// <param name="DateDescript">Return comics within a predefined date range.</param>
        /// <param name="DateRangeBegin">Return comics within a predefined date range, this is the beginning date.</param>
        /// <param name="DateRangeEnd">Return comics within a predefined date range, this is the ending date.</param>
        /// <param name="HasDigitalIssue">Include only results which are available digitally.</param>
        /// <param name="ModifiedSince">Return only comics which have been modified since the specified date.</param>
        /// <param name="Characters">Return only comics which feature the specified characters.</param>
        /// <param name="Series">Return only comics which are part of the specified series.</param>
        /// <param name="Events">Return only comics which take place in the specified events.</param>
        /// <param name="Stories">Return only comics which contain the specified stories.</param>
        /// <param name="SharedAppearances">Return only comics in which the specified characters appear together (for example in which BOTH Spider-Man and Wolverine appear).</param>
        /// <param name="Collaborators">Return only comics in which the specified creators worked together (for example in which BOTH Stan Lee and Jack Kirby did work).</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comics in which the work of a specific creator appears.
        /// </returns>
        public IEnumerable<Comic> GetComicsForCreator(int CreatorId,
                                                        ComicFormat? Format = null,
                                                        ComicFormatType? FormatType = null,
                                                        bool? NoVariants = null,
                                                        DateDescriptor? DateDescript = null,
                                                        DateTime? DateRangeBegin = null,
                                                        DateTime? DateRangeEnd = null,
                                                        bool? HasDigitalIssue = null,
                                                        DateTime? ModifiedSince = null,
                                                        IEnumerable<int> Characters = null,
                                                        IEnumerable<int> Series = null,
                                                        IEnumerable<int> Events = null,
                                                        IEnumerable<int> Stories = null,
                                                        IEnumerable<int> SharedAppearances = null,
                                                        IEnumerable<int> Collaborators = null,
                                                        IEnumerable<OrderBy> Order = null,
                                                        int? Limit = null,
                                                        int? Offset = null)
        {
            var request = CreateRequest(String.Format("/creators/{0}/comics", CreatorId));

            if (Format.HasValue)
            {
                request.AddParameter("format", Format.Value.ToParameter());
            }
            if (FormatType.HasValue)
            {
                request.AddParameter("formatType", FormatType.Value.ToParameter());
            }
            if (NoVariants.HasValue)
            {
                request.AddParameter("noVariants", NoVariants.Value.ToString().ToLower());
            }
            if (DateDescript.HasValue)
            {
                request.AddParameter("dateDescriptor", DateDescript.Value.ToParameter());
            }
            if (DateRangeBegin.HasValue && DateRangeEnd.HasValue)
            {
                if (DateRangeBegin.Value <= DateRangeEnd.Value)
                {
                    request.AddParameter("dateRange", String.Format("{0},{1}", DateRangeBegin.Value.ToString("YYYY-MM-DD"), DateRangeEnd.Value.ToString("YYYY-MM-DD")));
                }
                else
                {
                    throw new ArgumentException("DateRangeBegin must be greater than DateRangeEnd");
                }
            }
            else if (DateRangeBegin.HasValue || DateRangeEnd.HasValue)
            {
                throw new ArgumentException("Date Range requires both a start and end date");
            }
            if (HasDigitalIssue.HasValue)
            {
                request.AddParameter("hasDigitalIssue", HasDigitalIssue.Value.ToString().ToLower());
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Characters, "characters");
            request.AddParameterList(Series, "series");
            request.AddParameterList(Events, "events");
            request.AddParameterList(Stories, "stories");
            request.AddParameterList(SharedAppearances, "sharedAppearances");
            request.AddParameterList(Collaborators, "collaborators");

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
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            IRestResponse<ComicDataWrapper> response = _client.Execute<ComicDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of events featuring the work of a specific creator with optional filters.
        /// </summary>
        /// <param name="CreatorId">The creator ID.</param>
        /// <param name="Name">Filter the event list by name.</param>
        /// <param name="NameStartsWith">Return characters with names that begin with the specified string (e.g. Sp).</param>
        /// <param name="ModifiedSince">Return only events which have been modified since the specified date.</param>
        /// <param name="Characters">Return only events which feature the specified characters.</param>
        /// <param name="Series">Return only events which are part of the specified series.</param>
        /// <param name="Comics">Return only events which take place in the specified comics.</param>
        /// <param name="Stories">Return only events which contain the specified stories.</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of events featuring the work of a specific creator
        /// </returns>
        public IEnumerable<Event> GetEventsForCreator(int CreatorId,
                                            string Name = null,
                                            string NameStartsWith = null,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Characters = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Stories = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var request = CreateRequest(String.Format("/creators/{0}/events/", CreatorId));

            if (!String.IsNullOrWhiteSpace(Name))
            {
                request.AddParameter("name", Name);
            }
            if (!String.IsNullOrWhiteSpace(NameStartsWith))
            {
                request.AddParameter("nameStartsWith", NameStartsWith);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            if (Characters != null && Characters.Any())
            {
                request.AddParameter("characters", string.Join<int>(",", Characters));
            }
            if (Series != null && Series.Any())
            {
                request.AddParameter("series", string.Join<int>(",", Series));
            }
            if (Comics != null && Comics.Any())
            {
                request.AddParameter("comics", string.Join<int>(",", Comics));
            }
            if (Stories != null && Stories.Any())
            {
                request.AddParameter("stories", string.Join<int>(",", Stories));
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
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            IRestResponse<EventDataWrapper> response = _client.Execute<EventDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic series in which a specific creator's work appears, with optional filters.
        /// </summary>
        /// <param name="CreatorId">The creator ID.</param>
        /// <param name="Title">Filter by series title.</param>
        /// <param name="TitleStartsWith">Return titles that begin with the specified string (e.g. Sp).</param>
        /// <param name="ModifiedSince">Return only series which have been modified since the specified date.</param>
        /// <param name="Comics">Return only series which contain the specified comics.</param>
        /// <param name="Stories">Return only series which contain the specified stories.</param>
        /// <param name="Events">Return only series which have comics that take place during the specified events.</param>
        /// <param name="Characters">Return only series which feature the specified characters.</param>
        /// <param name="SeriesType">Filter the series by publication frequency type.</param>
        /// <param name="Contains">Return only series containing one or more comics with the specified format.</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic series in which a specific creator's work appears
        /// </returns>
        public IEnumerable<Series> GetSeriesForCreator(int CreatorId,
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
                                            int? Offset = null)
        {
            var request = CreateRequest(String.Format("/creators/{0}/series/", CreatorId));

            if (!String.IsNullOrWhiteSpace(Title))
            {
                request.AddParameter("title", Title);
            }
            if (!String.IsNullOrWhiteSpace(TitleStartsWith))
            {
                request.AddParameter("titleStartsWith", TitleStartsWith);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Comics, "comics");
            request.AddParameterList(Stories, "stories");
            request.AddParameterList(Events, "events");
            request.AddParameterList(Characters, "characters");

            if (SeriesType.HasValue)
            {
                request.AddParameter("seriesType", SeriesType.Value.ToParameter());
            }
            if (Contains != null && Contains.Any())
            {
                var containsParameters = Contains.Select(contain => contain.ToParameter());
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
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            IRestResponse<SeriesDataWrapper> response = _client.Execute<SeriesDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic stories by a specific creator with optional filters.
        /// </summary>
        /// <param name="CreatorId">The ID of the creator.</param>
        /// <param name="ModifiedSince">Return only stories which have been modified since the specified date.</param>
        /// <param name="Comics">Return only stories contained in the specified comics.</param>
        /// <param name="Series">Return only stories contained the specified series.</param>
        /// <param name="Events">Return only stories which take place during the specified events.</param>
        /// <param name="Characters">Return only stories which feature the specified characters.</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic stories by a specific creator
        /// </returns>
        public IEnumerable<Story> GetStoriesForCreator(int CreatorId,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Events = null,
                                            IEnumerable<int> Characters = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var request = CreateRequest(String.Format("/creators/{0}/events/", CreatorId));

            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Comics, "comics");
            request.AddParameterList(Series, "series");
            request.AddParameterList(Events, "events");
            request.AddParameterList(Characters, "characters");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Id,
                OrderBy.IdDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
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
        /// <param name="Name">Return only events which match the specified name.</param>
        /// <param name="NameStartsWith">Return characters with names that begin with the specified string (e.g. Sp).</param>
        /// <param name="ModifiedSince">Return only events which have been modified since the specified date.</param>
        /// <param name="Creators">Return only events which feature work by the specified creators.</param>
        /// <param name="Characters">Return only events which feature the specified characters.</param>
        /// <param name="Series">Return only events which are part of the specified series.</param>
        /// <param name="Comics">Return only events which take place in the specified comics.</param>
        /// <param name="Stories">Return only events which take place in the specified stories.</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of events
        /// </returns>
        public IEnumerable<Event> GetEvents(string Name = null,
                                            string NameStartsWith = null,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Creators = null,
                                            IEnumerable<int> Characters = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Stories = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var request = CreateRequest("/events/");

            if (!String.IsNullOrWhiteSpace(Name))
            {
                request.AddParameter("name", Name);
            }
            if (!String.IsNullOrWhiteSpace(NameStartsWith))
            {
                request.AddParameter("nameStartsWith", NameStartsWith);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Creators, "creators");
            request.AddParameterList(Characters, "characters");
            request.AddParameterList(Series, "series");
            request.AddParameterList(Comics, "comics");
            request.AddParameterList(Stories, "stories");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Name,
                OrderBy.NameDesc,
                OrderBy.StartDate,
                OrderBy.StartDateDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
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
            var request = CreateRequest(String.Format("/events/{0}", EventId));

            IRestResponse<EventDataWrapper> response = _client.Execute<EventDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results.FirstOrDefault(ev => ev.Id == EventId);
        }

        /// <summary>
        /// Fetches lists of characters which appear in a specific event, with optional filters.
        /// </summary>
        /// <param name="EventId">The event ID</param>
        /// <param name="Name">Return only characters matching the specified full character name (e.g. Spider-Man).</param>
        /// <param name="NameStartsWith">Return characters with names that begin with the specified string (e.g. Sp).</param>
        /// <param name="ModifiedSince">Return only characters which have been modified since the specified date.</param>
        /// <param name="Comics">Return only characters which appear in the specified comics.</param>
        /// <param name="Series">Return only characters which appear the specified series.</param>
        /// <param name="Stories">Return only characters which appear the specified stories.</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of characters which appear in a specific event
        /// </returns>
        public IEnumerable<Character> GetCharactersForEvent(int EventId,
                                            string Name = null,
                                            string NameStartsWith = null,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Stories = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var request = CreateRequest(String.Format("/events/{0}/characters", EventId));

            if (!String.IsNullOrWhiteSpace(Name))
            {
                request.AddParameter("name", Name);
            }
            if (!String.IsNullOrWhiteSpace(NameStartsWith))
            {
                request.AddParameter("nameStartsWith", NameStartsWith);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Comics, "comics");
            request.AddParameterList(Series, "series");
            request.AddParameterList(Stories, "stories");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Name,
                OrderBy.NameDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            IRestResponse<CharacterDataWrapper> response = _client.Execute<CharacterDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comics which take place during a specific event, with optional filters.
        /// </summary>
        /// <param name="EventId">The event id.</param>
        /// <param name="Format">Filter by the issue format.</param>
        /// <param name="FormatType">Filter by the issue format type.</param>
        /// <param name="NoVariants">Exclude variant comics from the result set.</param>
        /// <param name="DateDescript">Return comics within a predefined date range.</param>
        /// <param name="DateRangeBegin">Return comics within a predefined date range, this is the beginning date.</param>
        /// <param name="DateRangeEnd">Return comics within a predefined date range, this is the ending date.</param>
        /// <param name="HasDigitalIssue">Include only results which are available digitally.</param>
        /// <param name="ModifiedSince">Return only comics which have been modified since the specified date.</param>
        /// <param name="Creators">Return only comics which feature work by the specified creators.</param>
        /// <param name="Characters">Return only comics which feature the specified characters.</param>
        /// <param name="Series">Return only comics which are part of the specified series.</param>
        /// <param name="Events">Return only comics which take place in the specified events.</param>
        /// <param name="Stories">Return only comics which contain the specified stories.</param>
        /// <param name="SharedAppearances">Return only comics in which the specified characters appear together (for example in which BOTH Spider-Man and Wolverine appear).</param>
        /// <param name="Collaborators">Return only comics in which the specified creators worked together (for example in which BOTH Stan Lee and Jack Kirby did work).</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comics which take place during a specific event
        /// </returns>
        public IEnumerable<Comic> GetComicsForEvent(int EventId,
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
                                                        IEnumerable<int> Events = null, // Weird to see this here
                                                        IEnumerable<int> Stories = null,
                                                        IEnumerable<int> SharedAppearances = null,
                                                        IEnumerable<int> Collaborators = null,
                                                        IEnumerable<OrderBy> Order = null,
                                                        int? Limit = null,
                                                        int? Offset = null)
        {
            var request = CreateRequest(String.Format("/events/{0}/comics", EventId));

            if (Format.HasValue)
            {
                request.AddParameter("format", Format.Value.ToParameter());
            }
            if (FormatType.HasValue)
            {
                request.AddParameter("formatType", FormatType.Value.ToParameter());
            }
            if (NoVariants.HasValue)
            {
                request.AddParameter("noVariants", NoVariants.Value.ToString().ToLower());
            }
            if (DateDescript.HasValue)
            {
                request.AddParameter("dateDescriptor", DateDescript.Value.ToParameter());
            }
            if (DateRangeBegin.HasValue && DateRangeEnd.HasValue)
            {
                if (DateRangeBegin.Value <= DateRangeEnd.Value)
                {
                    request.AddParameter("dateRange", String.Format("{0},{1}", DateRangeBegin.Value.ToString("YYYY-MM-DD"), DateRangeEnd.Value.ToString("YYYY-MM-DD")));
                }
                else
                {
                    throw new ArgumentException("DateRangeBegin must be greater than DateRangeEnd");
                }
            }
            else if (DateRangeBegin.HasValue || DateRangeEnd.HasValue)
            {
                throw new ArgumentException("Date Range requires both a start and end date");
            }
            if (HasDigitalIssue.HasValue)
            {
                request.AddParameter("hasDigitalIssue", HasDigitalIssue.Value.ToString().ToLower());
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Creators, "creators");
            request.AddParameterList(Characters, "characters");
            request.AddParameterList(Series, "series");
            request.AddParameterList(Events, "events");
            request.AddParameterList(Stories, "stories");
            request.AddParameterList(SharedAppearances, "sharedAppearances");
            request.AddParameterList(Collaborators, "collaborators");

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
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            IRestResponse<ComicDataWrapper> response = _client.Execute<ComicDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic creators whose work appears in a specific event, with optional filters.
        /// </summary>
        /// <param name="EventId">The event ID.</param>
        /// <param name="FirstName">Filter by creator first name (e.g. brian).</param>
        /// <param name="MiddleName">Filter by creator middle name (e.g. Michael).</param>
        /// <param name="LastName">Filter by creator last name (e.g. Bendis).</param>
        /// <param name="Suffix">Filter by suffix or honorific (e.g. Jr., Sr.).</param>
        /// <param name="NameStartsWith">Filter by creator names that match critera (e.g. B, St L).</param>
        /// <param name="FirstNameStartsWith">Filter by creator first names that match critera (e.g. B, St L).</param>
        /// <param name="MiddleNameStartsWith">Filter by creator middle names that match critera (e.g. Mi).</param>
        /// <param name="LastNameStartsWith">Filter by creator last names that match critera (e.g. Ben).</param>
        /// <param name="ModifiedSince">Return only creators which have been modified since the specified date.</param>
        /// <param name="Comics">Return only creators who worked on in the specified comics.</param>
        /// <param name="Series">Return only creators who worked on the specified series.</param>
        /// <param name="Stories">Return only creators who worked on the specified stories.</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic creators whose work appears in a specific event
        /// </returns>
        public IEnumerable<Creator> GetCreatorsForEvent(int EventId,
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
                                            IEnumerable<int> Stories = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var request = CreateRequest(String.Format("/events/{0}/creators", EventId));

            if (!String.IsNullOrWhiteSpace(FirstName))
            {
                request.AddParameter("firstName", FirstName);
            }
            if (!String.IsNullOrWhiteSpace(MiddleName))
            {
                request.AddParameter("middleName", MiddleName);
            }
            if (!String.IsNullOrWhiteSpace(LastName))
            {
                request.AddParameter("lastName", LastName);
            }
            if (!String.IsNullOrWhiteSpace(Suffix))
            {
                request.AddParameter("suffix", Suffix);
            }
            if (!String.IsNullOrWhiteSpace(NameStartsWith))
            {
                request.AddParameter("nameStartsWith", NameStartsWith);
            }
            if (!String.IsNullOrWhiteSpace(FirstNameStartsWith))
            {
                request.AddParameter("firstNameStartsWith", FirstNameStartsWith);
            }
            if (!String.IsNullOrWhiteSpace(MiddleNameStartsWith))
            {
                request.AddParameter("middleNameStartsWith", MiddleNameStartsWith);
            }
            if (!String.IsNullOrWhiteSpace(LastNameStartsWith))
            {
                request.AddParameter("lastNameStartsWith", LastNameStartsWith);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Comics, "comics");
            request.AddParameterList(Series, "series");
            request.AddParameterList(Stories, "stories");

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
            request.AddOrderByParameterList(Order, availableOrderBy);

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
        /// Fetches lists of comic series in which a specific event takes place, with optional filters.
        /// </summary>
        /// <param name="EventId">The event ID.</param>
        /// <param name="Title">Filter by series title.</param>
        /// <param name="TitleStartsWith">Return titles that begin with the specified string (e.g. Sp).</param>
        /// <param name="ModifiedSince">Return only series which have been modified since the specified date.</param>
        /// <param name="Comics">Return only series which contain the specified comics.</param>
        /// <param name="Stories">Return only series which contain the specified stories.</param>
        /// <param name="Creators">Return only series which feature work by the specified creators.</param>
        /// <param name="Characters">Return only series which feature the specified characters.</param>
        /// <param name="SeriesType">Filter the series by publication frequency type.</param>
        /// <param name="Contains">Return only series containing one or more comics with the specified format.</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic series in which a specific event takes place
        /// </returns>
        public IEnumerable<Series> GetSeriesForEvent(int EventId,
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
                                            int? Offset = null)
        {
            var request = CreateRequest(String.Format("/events/{0}/series/", EventId));

            if (!String.IsNullOrWhiteSpace(Title))
            {
                request.AddParameter("title", Title);
            }
            if (!String.IsNullOrWhiteSpace(TitleStartsWith))
            {
                request.AddParameter("titleStartsWith", TitleStartsWith);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Comics, "comics");
            request.AddParameterList(Stories, "stories");
            request.AddParameterList(Creators, "creators");
            request.AddParameterList(Characters, "characters");

            if (SeriesType.HasValue)
            {
                request.AddParameter("seriesType", SeriesType.Value.ToParameter());
            }
            if (Contains != null && Contains.Any())
            {
                var containsParameters = Contains.Select(contain => contain.ToParameter());
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
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            IRestResponse<SeriesDataWrapper> response = _client.Execute<SeriesDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic stories from a specific event, with optional filters.
        /// </summary>
        /// <param name="EventId">The ID of the event.</param>
        /// <param name="ModifiedSince">Return only stories which have been modified since the specified date.</param>
        /// <param name="Comics">Return only stories contained in the specified.</param>
        /// <param name="Series">Return only stories contained the specified series.</param>
        /// <param name="Creators">Return only stories which feature work by the specified creators.</param>
        /// <param name="Characters">Return only stories which feature the specified characters.</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic stories from a specific event
        /// </returns>
        public IEnumerable<Story> GetStoriesForEvent(int EventId,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Creators = null,
                                            IEnumerable<int> Characters = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var request = CreateRequest(String.Format("/events/{0}/stories", EventId));

            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Comics, "comics");
            request.AddParameterList(Series, "series");
            request.AddParameterList(Creators, "creators");
            request.AddParameterList(Characters, "characters");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Id,
                OrderBy.IdDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
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
        /// <param name="Title">Return only series matching the specified title.</param>
        /// <param name="TitleStartsWith">Return titles that begin with the specified string (e.g. Sp).</param>
        /// <param name="ModifiedSince">Return only series which have been modified since the specified date.</param>
        /// <param name="Comics">Return only series which contain the specified comics.</param>
        /// <param name="Stories">Return only series which contain the specified stories.</param>
        /// <param name="Events">Return only series which have comics that take place during the specified events.</param>
        /// <param name="Creators">Return only series which feature work by the specified creators.</param>
        /// <param name="Characters">Return only series which feature the specified characters.</param>
        /// <param name="Type">Filter the series by publication frequency type.</param>
        /// <param name="Contains">Return only series containing one or more comics with the specified format.</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic series
        /// </returns>
        public IEnumerable<Series> GetSeries(string Title = null,
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
                                            int? Offset = null)
        {
            var request = CreateRequest("/series");

            if (!String.IsNullOrWhiteSpace(Title))
            {
                request.AddParameter("title", Title);
            }
            if (!String.IsNullOrWhiteSpace(TitleStartsWith))
            {
                request.AddParameter("titleStartsWith", TitleStartsWith);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Comics, "comics");
            request.AddParameterList(Stories, "stories");
            request.AddParameterList(Events, "events");
            request.AddParameterList(Creators, "creators");
            request.AddParameterList(Characters, "characters");

            if (Type.HasValue)
            {
                request.AddParameter("seriesType", Type.Value.ToParameter());
            }
            if (Contains.HasValue)
            {
                request.AddParameter("contains", Contains.Value.ToParameter());
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
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            IRestResponse<SeriesDataWrapper> response = _client.Execute<SeriesDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// This method fetches a single comic series resource. It is the canonical URI for any comic series resource provided by the API.
        /// </summary>
        /// <param name="SeriesId">The series id.</param>
        /// <returns>
        /// A single comic series resource
        /// </returns>
        public Series GetSeries(int SeriesId)
        {
            var request = CreateRequest(String.Format("/series/{0}", SeriesId));

            IRestResponse<SeriesDataWrapper> response = _client.Execute<SeriesDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results.FirstOrDefault(series => series.Id == SeriesId);
        }

        /// <summary>
        /// Fetches lists of characters which appear in specific series, with optional filters.
        /// </summary>
        /// <param name="SeriesId">The series id.</param>
        /// <param name="Name">Return only characters matching the specified full character name (e.g. Spider-Man).</param>
        /// <param name="NameStartsWith">Return characters with names that begin with the specified string (e.g. Sp).</param>
        /// <param name="ModifiedSince">Return only characters which have been modified since the specified date.</param>
        /// <param name="Comics">Return only characters which appear in the specified comics.</param>
        /// <param name="Events">Return only characters which appear comics that took place in the specified events.</param>
        /// <param name="Stories">Return only characters which appear the specified stories.</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of characters which appear in specific series
        /// </returns>
        public IEnumerable<Character> GetCharactersForSeries(int SeriesId,
                                            string Name = null,
                                            string NameStartsWith = null,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Events = null,
                                            IEnumerable<int> Stories = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var request = CreateRequest(String.Format("/series/{0}/characters", SeriesId));

            if (!String.IsNullOrWhiteSpace(Name))
            {
                request.AddParameter("name", Name);
            }
            if (!String.IsNullOrWhiteSpace(NameStartsWith))
            {
                request.AddParameter("nameStartsWith", NameStartsWith);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Comics, "comics");
            request.AddParameterList(Events, "events");
            request.AddParameterList(Stories, "stories");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Name,
                OrderBy.NameDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            IRestResponse<CharacterDataWrapper> response = _client.Execute<CharacterDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comics which are published as part of a specific series, with optional filters.
        /// </summary>
        /// <param name="SeriesId">The series ID.</param>
        /// <param name="Format">Filter by the issue format.</param>
        /// <param name="FormatType">Filter by the issue format type.</param>
        /// <param name="NoVariants">Exclude variant comics from the result set.</param>
        /// <param name="DateDescript">Return comics within a predefined date range.</param>
        /// <param name="DateRangeBegin">Return comics within a predefined date range, this is the beginning date.</param>
        /// <param name="DateRangeEnd">Return comics within a predefined date range, this is the ending date.</param>
        /// <param name="HasDigitalIssue">Include only results which are available digitally.</param>
        /// <param name="ModifiedSince">Return only comics which have been modified since the specified date.</param>
        /// <param name="Creators">Return only comics which feature work by the specified creators.</param>
        /// <param name="Characters">Return only comics which feature the specified characters.</param>
        /// <param name="Events">Return only comics which take place in the specified events.</param>
        /// <param name="Stories">Return only comics which contain the specified stories.</param>
        /// <param name="SharedAppearances">Return only comics in which the specified characters appear together (for example in which BOTH Spider-Man and Wolverine appear).</param>
        /// <param name="Collaborators">Return only comics in which the specified creators worked together (for example in which BOTH Stan Lee and Jack Kirby did work).</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comics which are published as part of a specific series
        /// </returns>
        public IEnumerable<Comic> GetComicsForSeries(int SeriesId,
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
                                                        IEnumerable<int> Events = null,
                                                        IEnumerable<int> Stories = null,
                                                        IEnumerable<int> SharedAppearances = null,
                                                        IEnumerable<int> Collaborators = null,
                                                        IEnumerable<OrderBy> Order = null,
                                                        int? Limit = null,
                                                        int? Offset = null)
        {
            var request = CreateRequest(String.Format("/series/{0}/comics", SeriesId));

            if (Format.HasValue)
            {
                request.AddParameter("format", Format.Value.ToParameter());
            }
            if (FormatType.HasValue)
            {
                request.AddParameter("formatType", FormatType.Value.ToParameter());
            }
            if (NoVariants.HasValue)
            {
                request.AddParameter("noVariants", NoVariants.Value.ToString().ToLower());
            }
            if (DateDescript.HasValue)
            {
                request.AddParameter("dateDescriptor", DateDescript.Value.ToParameter());
            }
            if (DateRangeBegin.HasValue && DateRangeEnd.HasValue)
            {
                if (DateRangeBegin.Value <= DateRangeEnd.Value)
                {
                    request.AddParameter("dateRange", String.Format("{0},{1}", DateRangeBegin.Value.ToString("YYYY-MM-DD"), DateRangeEnd.Value.ToString("YYYY-MM-DD")));
                }
                else
                {
                    throw new ArgumentException("DateRangeBegin must be greater than DateRangeEnd");
                }
            }
            else if (DateRangeBegin.HasValue || DateRangeEnd.HasValue)
            {
                throw new ArgumentException("Date Range requires both a start and end date");
            }
            if (HasDigitalIssue.HasValue)
            {
                request.AddParameter("hasDigitalIssue", HasDigitalIssue.Value.ToString().ToLower());
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Creators, "creators");
            request.AddParameterList(Characters, "characters");
            request.AddParameterList(Events, "events");
            request.AddParameterList(Stories, "stories");
            request.AddParameterList(SharedAppearances, "sharedAppearances");
            request.AddParameterList(Collaborators, "collaborators");

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
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            IRestResponse<ComicDataWrapper> response = _client.Execute<ComicDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic creators whose work appears in a specific series, with optional filters.
        /// </summary>
        /// <param name="SeriesId">The series ID.</param>
        /// <param name="FirstName">Filter by creator first name (e.g. brian).</param>
        /// <param name="MiddleName">Filter by creator middle name (e.g. Michael).</param>
        /// <param name="LastName">Filter by creator last name (e.g. Bendis).</param>
        /// <param name="Suffix">Filter by suffix or honorific (e.g. Jr., Sr.).</param>
        /// <param name="NameStartsWith">Filter by creator names that match critera (e.g. B, St L).</param>
        /// <param name="FirstNameStartsWith">Filter by creator first names that match critera (e.g. B, St L).</param>
        /// <param name="MiddleNameStartsWith">Filter by creator middle names that match critera (e.g. Mi).</param>
        /// <param name="LastNameStartsWith">Filter by creator last names that match critera (e.g. Ben).</param>
        /// <param name="ModifiedSince">Return only creators which have been modified since the specified date.</param>
        /// <param name="Comics">Return only creators who worked on in the specified comics.</param>
        /// <param name="Events">Return only creators who worked on comics that took place in the specified events.</param>
        /// <param name="Stories">Return only creators who worked on the specified stories.</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic creators whose work appears in a specific series
        /// </returns>
        public IEnumerable<Creator> GetCreatorsForSeries(int SeriesId,
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
                                            IEnumerable<int> Events = null,
                                            IEnumerable<int> Stories = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var request = CreateRequest(String.Format("/series/{0}/creators", SeriesId));

            if (!String.IsNullOrWhiteSpace(FirstName))
            {
                request.AddParameter("firstName", FirstName);
            }
            if (!String.IsNullOrWhiteSpace(MiddleName))
            {
                request.AddParameter("middleName", MiddleName);
            }
            if (!String.IsNullOrWhiteSpace(LastName))
            {
                request.AddParameter("lastName", LastName);
            }
            if (!String.IsNullOrWhiteSpace(Suffix))
            {
                request.AddParameter("suffix", Suffix);
            }
            if (!String.IsNullOrWhiteSpace(NameStartsWith))
            {
                request.AddParameter("nameStartsWith", NameStartsWith);
            }
            if (!String.IsNullOrWhiteSpace(FirstNameStartsWith))
            {
                request.AddParameter("firstNameStartsWith", FirstNameStartsWith);
            }
            if (!String.IsNullOrWhiteSpace(MiddleNameStartsWith))
            {
                request.AddParameter("middleNameStartsWith", MiddleNameStartsWith);
            }
            if (!String.IsNullOrWhiteSpace(LastNameStartsWith))
            {
                request.AddParameter("lastNameStartsWith", LastNameStartsWith);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Comics, "comics");
            request.AddParameterList(Events, "events");
            request.AddParameterList(Stories, "stories");

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
            request.AddOrderByParameterList(Order, availableOrderBy);

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
        /// Fetches lists of events which occur in a specific series, with optional filters.
        /// </summary>
        /// <param name="SeriesId">The series ID.</param>
        /// <param name="Name">Filter the event list by name.</param>
        /// <param name="NameStartsWith">Return characters with names that begin with the specified string (e.g. Sp).</param>
        /// <param name="ModifiedSince">Return only events which have been modified since the specified date.</param>
        /// <param name="Creators">Return only events which feature work by the specified creators.</param>
        /// <param name="Characters">Return only events which feature the specified characters.</param>
        /// <param name="Comics">Return only events which take place in the specified comics.</param>
        /// <param name="Stories">Return only events which contain the specified stories.</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of events which occur in a specific series
        /// </returns>
        public IEnumerable<Event> GetEventsForSeries(int SeriesId,
                                            string Name = null,
                                            string NameStartsWith = null,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Creators = null,
                                            IEnumerable<int> Characters = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Stories = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var request = CreateRequest(String.Format("/series/{0}/events", SeriesId));

            if (!String.IsNullOrWhiteSpace(Name))
            {
                request.AddParameter("name", Name);
            }
            if (!String.IsNullOrWhiteSpace(NameStartsWith))
            {
                request.AddParameter("nameStartsWith", NameStartsWith);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Creators, "creators");
            request.AddParameterList(Characters, "characters");
            request.AddParameterList(Comics, "comics");
            request.AddParameterList(Stories, "stories");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Name,
                OrderBy.NameDesc,
                OrderBy.StartDate,
                OrderBy.StartDateDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            IRestResponse<EventDataWrapper> response = _client.Execute<EventDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic stories from a specific series with optional filters.
        /// </summary>
        /// <param name="SeriesId">The series ID.</param>
        /// <param name="ModifiedSince">Return only stories which have been modified since the specified date.</param>
        /// <param name="Comics">Return only stories contained in the specified.</param>
        /// <param name="Events">Return only stories which take place during the specified events.</param>
        /// <param name="Creators">Return only stories which feature work by the specified creators.</param>
        /// <param name="Characters">Return only stories which feature the specified characters.</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic stories from a specific series
        /// </returns>
        public IEnumerable<Story> GetStoriesForSeries(int SeriesId,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Events = null,
                                            IEnumerable<int> Creators = null,
                                            IEnumerable<int> Characters = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var request = CreateRequest(String.Format("/series/{0}/stories", SeriesId));

            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Comics, "comics");
            request.AddParameterList(Events, "events");
            request.AddParameterList(Creators, "creators");
            request.AddParameterList(Characters, "characters");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Id,
                OrderBy.IdDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
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
        /// <param name="ModifiedSince">Return only stories which have been modified since the specified date.</param>
        /// <param name="Comics">Return only stories contained in the specified.</param>
        /// <param name="Series">Return only stories contained the specified series.</param>
        /// <param name="Events">Return only stories which take place during the specified events.</param>
        /// <param name="Creators">Return only stories which feature work by the specified creators.</param>
        /// <param name="Characters">Return only stories which feature the specified characters.</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic stories
        /// </returns>
        public IEnumerable<Story> GetStories(DateTime? ModifiedSince = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Events = null,
                                            IEnumerable<int> Creators = null,
                                            IEnumerable<int> Characters = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var request = CreateRequest("/stories");

            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Comics, "comics");
            request.AddParameterList(Series, "series");
            request.AddParameterList(Events, "events");
            request.AddParameterList(Creators, "creators");
            request.AddParameterList(Characters, "characters");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Id,
                OrderBy.IdDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
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
            var request = CreateRequest(String.Format("/stories/{0}", StoryId));

            IRestResponse<StoryDataWrapper> response = _client.Execute<StoryDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results.FirstOrDefault(story => story.Id == StoryId);
        }

        /// <summary>
        /// Fetches lists of comic characters appearing in a single story, with optional filters.
        /// </summary>
        /// <param name="StoryId">The story ID.</param>
        /// <param name="Name">Return only characters matching the specified full character name (e.g. Spider-Man).</param>
        /// <param name="NameStartsWith">Return characters with names that begin with the specified string (e.g. Sp).</param>
        /// <param name="ModifiedSince">Return only characters which have been modified since the specified date.</param>
        /// <param name="Comics">Return only characters which appear in the specified comics.</param>
        /// <param name="Series">Return only characters which appear the specified series.</param>
        /// <param name="Events">Return only characters which appear comics that took place in the specified events.</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic characters appearing in a single story
        /// </returns>
        public IEnumerable<Character> GetCharactersForStory(int StoryId,
                                            string Name = null,
                                            string NameStartsWith = null,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Events = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var request = CreateRequest(String.Format("/stories/{0}/characters", StoryId));

            if (!String.IsNullOrWhiteSpace(Name))
            {
                request.AddParameter("name", Name);
            }
            if (!String.IsNullOrWhiteSpace(NameStartsWith))
            {
                request.AddParameter("nameStartsWith", NameStartsWith);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Comics, "comics");
            request.AddParameterList(Events, "events");
            request.AddParameterList(Series, "series");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Name,
                OrderBy.NameDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            IRestResponse<CharacterDataWrapper> response = _client.Execute<CharacterDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comics in which a specific story appears, with optional filters.
        /// </summary>
        /// <param name="StoryId">The story ID.</param>
        /// <param name="Format">Filter by the issue format.</param>
        /// <param name="FormatType">Filter by the issue format type.</param>
        /// <param name="NoVariants">Exclude variant comics from the result set.</param>
        /// <param name="DateDescript">Return comics within a predefined date range.</param>
        /// <param name="DateRangeBegin">Return comics within a predefined date range, this is the beginning of range.</param>
        /// <param name="DateRangeEnd">Return comics within a predefined date range, this is the end of range.</param>
        /// <param name="HasDigitalIssue">Include only results which are available digitally.</param>
        /// <param name="ModifiedSince">Return only comics which have been modified since the specified date.</param>
        /// <param name="Creators">Return only comics which feature work by the specified creators.</param>
        /// <param name="Characters">Return only comics which feature the specified characters.</param>
        /// <param name="Series">Return only comics which are part of the specified series.</param>
        /// <param name="Events">Return only comics which take place in the specified events.</param>
        /// <param name="SharedAppearances">Return only comics in which the specified characters appear together (for example in which BOTH Spider-Man and Wolverine appear).</param>
        /// <param name="Collaborators">Return only comics in which the specified creators worked together (for example in which BOTH Stan Lee and Jack Kirby did work).</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comics in which a specific story appears
        /// </returns>
        public IEnumerable<Comic> GetComicsForStory(int StoryId,
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
                                                        int? Offset = null)
        {
            var request = CreateRequest(String.Format("/stories/{0}/comics", StoryId));

            if (Format.HasValue)
            {
                request.AddParameter("format", Format.Value.ToParameter());
            }
            if (FormatType.HasValue)
            {
                request.AddParameter("formatType", FormatType.Value.ToParameter());
            }
            if (NoVariants.HasValue)
            {
                request.AddParameter("noVariants", NoVariants.Value.ToString().ToLower());
            }
            if (DateDescript.HasValue)
            {
                request.AddParameter("dateDescriptor", DateDescript.Value.ToParameter());
            }
            if (DateRangeBegin.HasValue && DateRangeEnd.HasValue)
            {
                if (DateRangeBegin.Value <= DateRangeEnd.Value)
                {
                    request.AddParameter("dateRange", String.Format("{0},{1}", DateRangeBegin.Value.ToString("YYYY-MM-DD"), DateRangeEnd.Value.ToString("YYYY-MM-DD")));
                }
                else
                {
                    throw new ArgumentException("DateRangeBegin must be greater than DateRangeEnd");
                }
            }
            else if (DateRangeBegin.HasValue || DateRangeEnd.HasValue)
            {
                throw new ArgumentException("Date Range requires both a start and end date");
            }
            if (HasDigitalIssue.HasValue)
            {
                request.AddParameter("hasDigitalIssue", HasDigitalIssue.Value.ToString().ToLower());
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Creators, "creators");
            request.AddParameterList(Characters, "characters");
            request.AddParameterList(Series, "series");
            request.AddParameterList(Events, "events");
            request.AddParameterList(SharedAppearances, "sharedAppearances");
            request.AddParameterList(Collaborators, "collaborators");

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
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            IRestResponse<ComicDataWrapper> response = _client.Execute<ComicDataWrapper>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic creators whose work appears in a specific story, with optional filters.
        /// </summary>
        /// <param name="StoryId">The story ID.</param>
        /// <param name="FirstName">Filter by creator first name (e.g. brian).</param>
        /// <param name="MiddleName">Filter by creator middle name (e.g. Michael).</param>
        /// <param name="LastName">Filter by creator last name (e.g. Bendis).</param>
        /// <param name="Suffix">Filter by suffix or honorific (e.g. Jr., Sr.).</param>
        /// <param name="NameStartsWith">Filter by creator names that match critera (e.g. B, St L).</param>
        /// <param name="FirstNameStartsWith">Filter by creator first names that match critera (e.g. B, St L).</param>
        /// <param name="MiddleNameStartsWith">Filter by creator middle names that match critera (e.g. Mi).</param>
        /// <param name="LastNameStartsWith">Filter by creator last names that match critera (e.g. Ben).</param>
        /// <param name="ModifiedSince">Return only creators which have been modified since the specified date.</param>
        /// <param name="Comics">Return only creators who worked on in the specified comics.</param>
        /// <param name="Series">Return only creators who worked on the specified series.</param>
        /// <param name="Events">Return only creators who worked on comics that took place in the specified events.</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of comic creators whose work appears in a specific story
        /// </returns>
        public IEnumerable<Creator> GetCreatorsForStory(int StoryId,
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
                                                        int? Offset = null)
        {
            var request = CreateRequest(String.Format("/stories/{0}/creators", StoryId));

            if (!String.IsNullOrWhiteSpace(FirstName))
            {
                request.AddParameter("firstName", FirstName);
            }
            if (!String.IsNullOrWhiteSpace(MiddleName))
            {
                request.AddParameter("middleName", MiddleName);
            }
            if (!String.IsNullOrWhiteSpace(LastName))
            {
                request.AddParameter("lastName", LastName);
            }
            if (!String.IsNullOrWhiteSpace(Suffix))
            {
                request.AddParameter("suffix", Suffix);
            }
            if (!String.IsNullOrWhiteSpace(NameStartsWith))
            {
                request.AddParameter("nameStartsWith", NameStartsWith);
            }
            if (!String.IsNullOrWhiteSpace(FirstNameStartsWith))
            {
                request.AddParameter("firstNameStartsWith", FirstNameStartsWith);
            }
            if (!String.IsNullOrWhiteSpace(MiddleNameStartsWith))
            {
                request.AddParameter("middleNameStartsWith", MiddleNameStartsWith);
            }
            if (!String.IsNullOrWhiteSpace(LastNameStartsWith))
            {
                request.AddParameter("lastNameStartsWith", LastNameStartsWith);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Comics, "comics");
            request.AddParameterList(Series, "series");
            request.AddParameterList(Events, "events");

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
            request.AddOrderByParameterList(Order, availableOrderBy);

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
        /// Fetches lists of events in which a specific story appears, with optional filters.
        /// </summary>
        /// <param name="StoryId">The story ID.</param>
        /// <param name="Name">Filter the event list by name.</param>
        /// <param name="NameStartsWith">Return characters with names that begin with the specified string (e.g. Sp).</param>
        /// <param name="ModifiedSince">Return only events which have been modified since the specified date.</param>
        /// <param name="Creators">Return only events which feature work by the specified creators.</param>
        /// <param name="Characters">Return only events which feature the specified characters.</param>
        /// <param name="Series">Return only events which are part of the specified series.</param>
        /// <param name="Comics">Return only events which take place in the specified comics.</param>
        /// <param name="Order">Order the result set by a field or fields. Multiple values are given priority in the order in which they are passed.</param>
        /// <param name="Limit">Limit the result set to the specified number of resources.</param>
        /// <param name="Offset">Skip the specified number of resources in the result set.</param>
        /// <returns>
        /// Lists of events in which a specific story appears
        /// </returns>
        public IEnumerable<Event> GetEventsForStories(int StoryId,
                                            string Name = null,
                                            string NameStartsWith = null,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Creators = null,
                                            IEnumerable<int> Characters = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var request = CreateRequest(String.Format("/stories/{0}/events/", StoryId));

            if (!String.IsNullOrWhiteSpace(Name))
            {
                request.AddParameter("name", Name);
            }
            if (!String.IsNullOrWhiteSpace(NameStartsWith))
            {
                request.AddParameter("nameStartsWith", NameStartsWith);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(Creators, "creators");
            request.AddParameterList(Characters, "characters");
            request.AddParameterList(Series, "series");
            request.AddParameterList(Comics, "comics");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Name,
                OrderBy.NameDesc,
                OrderBy.StartDate,
                OrderBy.StartDateDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(Order, availableOrderBy);

            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
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
            return string.Format("{0}.{1}", Path, Extension);
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
        public static void AddParameterList(this RestRequest request, IEnumerable<int> parameter, string parameterString)
        {
            if (parameter != null && parameter.Count() > 0)
            {
                request.AddParameter(parameterString, string.Join<int>(",", parameter));
            }
        }

        public static void AddOrderByParameterList(this RestRequest request, IEnumerable<OrderBy> parameter, IEnumerable<OrderBy> available)
        {
            if (parameter != null && parameter.Count() > 0)
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
