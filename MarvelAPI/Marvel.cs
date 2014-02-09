using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI
{
    /* TODO SECTION
     * 
     * TODO: Refactor client and request creation
     * TODO: TESTS
     * TODO: Handle Etags
     * TODO: Handle when only give one end of date range (maybe defaults?)
     * TOOD: Handle 409
     * TODO: Handle 404
     * TODO: Create messages when using invalid OrderBy options
     * 
    */

    public class Marvel
    {
        private const string BASE_URL = "http://gateway.marvel.com/v1/public";
        private string PublicApiKey { get; set; }
        private string PrivateApiKey { get; set; }

        public Marvel(string publicApiKey, string privateApiKey)
        {
            PublicApiKey = publicApiKey;
            PrivateApiKey = privateApiKey;
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

        #region Characters
        /// <summary>
        /// Fetches lists of comic characters with optional filters.
        /// </summary>
        /// <param name="Name">Return only characters matching the specified full character name (e.g. Spider-Man).</param>
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
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Events = null,
                                            IEnumerable<int> Stories = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest("/characters", Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
            if (!String.IsNullOrWhiteSpace(Name))
            {
                request.AddParameter("name", Name);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Comics != null && Comics.Any())
            {
                request.AddParameter("comics", string.Join<int>(",", Comics));
            }
            if (Series != null && Series.Any())
            {
                request.AddParameter("series", string.Join<int>(",", Series));
            }
            if (Events != null && Events.Any())
            {
                request.AddParameter("events", string.Join<int>(",", Events));
            }
            if (Stories != null && Stories.Any())
            {
                request.AddParameter("stories", string.Join<int>(",", Stories));
            }
            if (Order != null && Order.Any())
            {
                StringBuilder orderString = new StringBuilder();
                foreach (var orderOption in Order)
                {
                    switch (orderOption)
                    {
                        case OrderBy.Name:
                        case OrderBy.NameDesc:
                        case OrderBy.Modified:
                        case OrderBy.ModifiedDesc:
                            if (orderString.Length > 0)
                            {
                                orderString.Append(",");
                            }
                            orderString.Append(orderOption.ToParameter());
                            break;
                    }
                }
                if (orderString.Length > 0)
                {
                    request.AddParameter("orderBy", orderString.ToString());
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Offset.Value >= 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<CharacterDataWrapper> response = client.Execute<CharacterDataWrapper>(request);

            if(response.Data.Code == 409)
            {
                throw new ArgumentException(response.Data.Status);
            }

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetch a single character by Id.
        /// </summary>
        /// <param name="CharacterId">A single character id.</param>
        /// <returns>Character details</returns>
        public Character GetCharacter(int CharacterId)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest("/characters", Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
            request.AddParameter("characterId", CharacterId);
            request.AddHeader("Accept", "*/*");

            IRestResponse<CharacterDataWrapper> response = client.Execute<CharacterDataWrapper>(request);

            if (response.Data.Code == 409)
            {
                throw new ArgumentException(response.Data.Status);
            }

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
        /// <param name="SharedAppearences">Return only comics in which the specified characters appear together (for example in which BOTH Spider-Man and Wolverine appear).</param>
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
                                                        IEnumerable<int> SharedAppearences = null,
                                                        IEnumerable<int> Collaborators = null,
                                                        IEnumerable<OrderBy> Order = null,
                                                        int? Limit = null,
                                                        int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/characters/{0}/comics", CharacterId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
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
                throw new ArgumentException("DateRangeBegin must be greater than DateRangeEnd");
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
            if (Creators != null && Creators.Any())
            {
                request.AddParameter("creators", string.Join<int>(",", Creators));
            }
            if (Series != null && Series.Any())
            {
                request.AddParameter("series", string.Join<int>(",", Series));
            }
            if (Events != null && Events.Any())
            {
                request.AddParameter("events", string.Join<int>(",", Events));
            }
            if (Stories != null && Stories.Any())
            {
                request.AddParameter("stories", string.Join<int>(",", Stories));
            }
            if (SharedAppearences != null && SharedAppearences.Any())
            {
                request.AddParameter("sharedAppearences", string.Join<int>(",", SharedAppearences));
            }
            if (Collaborators != null && Collaborators.Any())
            {
                request.AddParameter("collaborators", string.Join<int>(",", Collaborators));
            }
            if (Order != null && Order.Any())
            {
                StringBuilder orderString = new StringBuilder();
                foreach (var orderOption in Order)
                {
                    switch (orderOption)
                    {
                        case OrderBy.FocDate:
                        case OrderBy.FocDateDesc:
                        case OrderBy.OnSaleDate:
                        case OrderBy.OnSaleDateDesc:
                        case OrderBy.Title:
                        case OrderBy.TitleDesc:
                        case OrderBy.IssueNumber:
                        case OrderBy.IssueNumberDesc:
                        case OrderBy.Modified:
                        case OrderBy.ModifiedDesc:
                            if(orderString.Length > 0)
                            {
                                orderString.Append(",");
                            }
                            orderString.Append(orderOption.ToParameter());
                            break;
                    }
                }
                if (orderString.Length > 0)
                {
                    request.AddParameter("orderBy", orderString.ToString());
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<ComicDataWrapper> response = client.Execute<ComicDataWrapper>(request);

            if (response.Data.Code == 409)
            {
                throw new ArgumentException(response.Data.Status);
            }

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of events in which a specific character appears, with optional filters.
        /// </summary>
        /// <param name="CharacterId">The character id</param>
        /// <param name="Name">Filter the event list by name.</param>
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
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Creators = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Stories = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/characters/{0}/events/", CharacterId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
            if (!String.IsNullOrWhiteSpace(Name))
            {
                request.AddParameter("name", Name);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Creators != null && Creators.Any())
            {
                request.AddParameter("creators", string.Join<int>(",", Creators));
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
            if (Order != null && Order.Any())
            {
                StringBuilder orderString = new StringBuilder();
                foreach (var orderOption in Order)
                {
                    switch (orderOption)
                    {
                        case OrderBy.Name:
                        case OrderBy.NameDesc:
                        case OrderBy.StartDate:
                        case OrderBy.StartDateDesc:
                        case OrderBy.Modified:
                        case OrderBy.ModifiedDesc:
                            if (orderString.Length > 0)
                            {
                                orderString.Append(",");
                            }
                            orderString.Append(orderOption.ToParameter());
                            break;
                    }
                }
                if (orderString.Length > 0)
                {
                    request.AddParameter("orderBy", orderString.ToString());
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<EventDataWrapper> response = client.Execute<EventDataWrapper>(request);
            
            if (response.Data.Code == 409)
            {
                throw new ArgumentException(response.Data.Status);
            }

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic series in which a specific character appears, with optional filters.
        /// </summary>
        /// <param name="CharacterId">The character ID</param>
        /// <param name="Title">Filter by series title.</param>
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
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/characters/{0}/series/", CharacterId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
            if (!String.IsNullOrWhiteSpace(Title))
            {
                request.AddParameter("title", Title);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Comics != null && Comics.Any())
            {
                request.AddParameter("comics", string.Join<int>(",", Comics));
            }
            if (Stories != null && Stories.Any())
            {
                request.AddParameter("stories", string.Join<int>(",", Stories));
            }
            if (Events != null && Events.Any())
            {
                request.AddParameter("events", string.Join<int>(",", Events));
            }
            if (Creators != null && Creators.Any())
            {
                request.AddParameter("creators", string.Join<int>(",", Creators));
            }
            if (SeriesType.HasValue)
            {
                request.AddParameter("seriesType", SeriesType.Value.ToParameter());
            }
            if (Contains != null && Contains.Any())
            {
                var containsParameters = Contains.Select(contain => contain.ToParameter());
                request.AddParameter("contains", string.Join(",", containsParameters));
            }
            if (Order != null && Order.Any())
            {
                StringBuilder orderString = new StringBuilder();
                foreach (var orderOption in Order)
                {
                    switch (orderOption)
                    {
                        case OrderBy.Title:
                        case OrderBy.TitleDesc:
                        case OrderBy.StartYear:
                        case OrderBy.StartYearDesc:
                        case OrderBy.Modified:
                        case OrderBy.ModifiedDesc:
                            if (orderString.Length > 0)
                            {
                                orderString.Append(",");
                            }
                            orderString.Append(orderOption.ToParameter());
                            break;
                    }
                }
                if (orderString.Length > 0)
                {
                    request.AddParameter("orderBy", orderString.ToString());
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<SeriesDataWrapper> response = client.Execute<SeriesDataWrapper>(request);

            if (response.Data.Code == 409)
            {
                throw new ArgumentException(response.Data.Status);
            }

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
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/characters/{0}/stories/", CharacterId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Creators != null && Creators.Any())
            {
                request.AddParameter("creators", string.Join<int>(",", Creators));
            }
            if (Series != null && Series.Any())
            {
                request.AddParameter("series", string.Join<int>(",", Series));
            }
            if (Comics != null && Comics.Any())
            {
                request.AddParameter("comics", string.Join<int>(",", Comics));
            }
            if (Events != null && Events.Any())
            {
                request.AddParameter("events", string.Join<int>(",", Events));
            }
            if (Order != null && Order.Any())
            {
                StringBuilder orderString = new StringBuilder();
                foreach (var orderOption in Order)
                {
                    switch (orderOption)
                    {
                        case OrderBy.Id:
                        case OrderBy.IdDesc:
                        case OrderBy.Modified:
                        case OrderBy.ModifiedDesc:
                            if (orderString.Length > 0)
                            {
                                orderString.Append(",");
                            }
                            orderString.Append(orderOption.ToParameter());
                            break;
                    }
                }
                if (orderString.Length > 0)
                {
                    request.AddParameter("orderBy", orderString.ToString());
                }
                
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<StoryDataWrapper> response = client.Execute<StoryDataWrapper>(request);

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
        /// <param name="SharedAppearences">Return only comics in which the specified characters appear together (for example in which BOTH Spider-Man and Wolverine appear).</param>
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
                                            IEnumerable<int> SharedAppearences = null,
                                            IEnumerable<int> Collaborators = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest("/comics", Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
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
                throw new ArgumentException("DateRangeBegin must be greater than DateRangeEnd");
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
            if (Creators != null && Creators.Any())
            {
                request.AddParameter("creators", string.Join<int>(",", Creators));
            }
            if (Characters != null && Characters.Any())
            {
                request.AddParameter("characters", string.Join<int>(",", Characters));
            }
            if (Series != null && Series.Any())
            {
                request.AddParameter("series", string.Join<int>(",", Series));
            }
            if (Events != null && Events.Any())
            {
                request.AddParameter("events", string.Join<int>(",", Events));
            }
            if (Stories != null && Stories.Any())
            {
                request.AddParameter("stories", string.Join<int>(",", Stories));
            }
            if (SharedAppearences != null && SharedAppearences.Any())
            {
                request.AddParameter("sharedAppearences", string.Join<int>(",", SharedAppearences));
            }
            if (Collaborators != null && Collaborators.Any())
            {
                request.AddParameter("collaborators", string.Join<int>(",", Collaborators));
            }
            if (Order != null && Order.Any())
            {
                StringBuilder orderString = new StringBuilder();
                foreach (var orderOption in Order)
                {
                    switch (orderOption)
                    {
                        case OrderBy.FocDate:
                        case OrderBy.FocDateDesc:
                        case OrderBy.OnSaleDate:
                        case OrderBy.OnSaleDateDesc:
                        case OrderBy.Title:
                        case OrderBy.TitleDesc:
                        case OrderBy.IssueNumber:
                        case OrderBy.IssueNumberDesc:
                        case OrderBy.Modified:
                        case OrderBy.ModifiedDesc:
                            if (orderString.Length > 0)
                            {
                                orderString.Append(",");
                            }
                            orderString.Append(orderOption.ToParameter());
                            break;
                    }
                }
                if (orderString.Length > 0)
                {
                    request.AddParameter("orderBy", orderString.ToString());
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<ComicDataWrapper> response = client.Execute<ComicDataWrapper>(request);

            if (response.Data.Code == 409)
            {
                throw new ArgumentException(response.Data.Status);
            }

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
            var client = new RestClient(BASE_URL);
            var request = new RestRequest("/comics", Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
            request.AddParameter("comicId", ComicId);
            request.AddHeader("Accept", "*/*");

            IRestResponse<ComicDataWrapper> response = client.Execute<ComicDataWrapper>(request);

            if (response.Data.Code == 409)
            {
                throw new ArgumentException(response.Data.Status);
            }

            return response.Data.Data.Results.FirstOrDefault(comic => comic.Id == ComicId);
        }

        /// <summary>
        /// Fetches lists of characters who appear in a specific comic with optional filters.
        /// </summary>
        /// <param name="ComicId">The comic id.</param>
        /// <param name="Name">Return only characters matching the specified full character name (e.g. Spider-Man).</param>
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
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Events = null,
                                            IEnumerable<int> Stories = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/comics/{0}/characters", ComicId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
            if (!String.IsNullOrWhiteSpace(Name))
            {
                request.AddParameter("name", Name);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Series != null && Series.Any())
            {
                request.AddParameter("series", string.Join<int>(",", Series));
            }
            if (Events != null && Events.Any())
            {
                request.AddParameter("events", string.Join<int>(",", Events));
            }
            if (Stories != null && Stories.Any())
            {
                request.AddParameter("stories", string.Join<int>(",", Stories));
            }
            if (Order != null && Order.Any())
            {
                StringBuilder orderString = new StringBuilder();
                foreach (var orderOption in Order)
                {
                    switch (orderOption)
                    {
                        case OrderBy.Name:
                        case OrderBy.NameDesc:
                        case OrderBy.Modified:
                        case OrderBy.ModifiedDesc:
                            if (orderString.Length > 0)
                            {
                                orderString.Append(",");
                            }
                            orderString.Append(orderOption.ToParameter());
                            break;
                    }
                }
                if (orderString.Length > 0)
                {
                    request.AddParameter("orderBy", orderString.ToString());
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<CharacterDataWrapper> response = client.Execute<CharacterDataWrapper>(request);

            if (response.Data.Code == 409)
            {
                throw new ArgumentException(response.Data.Status);
            }

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
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Stories = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/comics/{0}/characters", ComicId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
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
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Series != null && Series.Any())
            {
                request.AddParameter("series", string.Join<int>(",", Series));
            }
            if (Comics != null && Comics.Any())
            {
                request.AddParameter("events", string.Join<int>(",", Comics));
            }
            if (Stories != null && Stories.Any())
            {
                request.AddParameter("stories", string.Join<int>(",", Stories));
            }
            if (Order != null && Order.Any())
            {
                StringBuilder orderString = new StringBuilder();
                foreach (var orderOption in Order)
                {
                    switch (orderOption)
                    {
                        case OrderBy.FirstName:
                        case OrderBy.FirstNameDesc:
                        case OrderBy.MiddleName:
                        case OrderBy.MiddleNameDesc:
                        case OrderBy.LastName:
                        case OrderBy.LastNameDesc:
                        case OrderBy.Suffix:
                        case OrderBy.SuffixDesc:
                        case OrderBy.Modified:
                        case OrderBy.ModifiedDesc:
                            if (orderString.Length > 0)
                            {
                                orderString.Append(",");
                            }
                            orderString.Append(orderOption.ToParameter());
                            break;
                    }
                }
                if (orderString.Length > 0)
                {
                    request.AddParameter("orderBy", orderString.ToString());
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<CreatorDataWrapper> response = client.Execute<CreatorDataWrapper>(request);

            if (response.Data.Code == 409)
            {
                throw new ArgumentException(response.Data.Status);
            }

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of events in which a specific comic appears, with optional filters.
        /// </summary>
        /// <param name="ComicId">The comic ID.</param>
        /// <param name="Name">Filter the event list by name.</param>
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
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Creators = null,
                                            IEnumerable<int> Characters = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Stories = null,
                                            IEnumerable<OrderBy> Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/comics/{0}/events/", ComicId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
            if (!String.IsNullOrWhiteSpace(Name))
            {
                request.AddParameter("name", Name);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Creators != null && Creators.Any())
            {
                request.AddParameter("creators", string.Join<int>(",", Creators));
            }
            if (Characters != null && Characters.Any())
            {
                request.AddParameter("characters", string.Join<int>(",", Characters));
            }
            if (Series != null && Series.Any())
            {
                request.AddParameter("series", string.Join<int>(",", Series));
            }
            if (Stories != null && Stories.Any())
            {
                request.AddParameter("stories", string.Join<int>(",", Stories));
            }
            if (Order != null && Order.Any())
            {
                StringBuilder orderString = new StringBuilder();
                foreach (var orderOption in Order)
                {
                    switch (orderOption)
                    {
                        case OrderBy.Name:
                        case OrderBy.NameDesc:
                        case OrderBy.StartDate:
                        case OrderBy.StartDateDesc:
                        case OrderBy.Modified:
                        case OrderBy.ModifiedDesc:
                            if (orderString.Length > 0)
                            {
                                orderString.Append(",");
                            }
                            orderString.Append(orderOption.ToParameter());
                            break;
                    }
                }
                if (orderString.Length > 0)
                {
                    request.AddParameter("orderBy", orderString.ToString());
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<EventDataWrapper> response = client.Execute<EventDataWrapper>(request);

            if (response.Data.Code == 409)
            {
                throw new ArgumentException(response.Data.Status);
            }

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
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/comics/{0}/events/", ComicId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Series != null && Series.Any())
            {
                request.AddParameter("series", string.Join<int>(",", Series));
            }
            if (Events != null && Events.Any())
            {
                request.AddParameter("events", string.Join<int>(",", Events));
            }
            if (Creators != null && Creators.Any())
            {
                request.AddParameter("creators", string.Join<int>(",", Creators));
            }
            if (Characters != null && Characters.Any())
            {
                request.AddParameter("characters", string.Join<int>(",", Characters));
            }
            if (Order != null && Order.Any())
            {
                StringBuilder orderString = new StringBuilder();
                foreach (var orderOption in Order)
                {
                    switch (orderOption)
                    {
                        case OrderBy.Id:
                        case OrderBy.IdDesc:
                        case OrderBy.Modified:
                        case OrderBy.ModifiedDesc:
                            if (orderString.Length > 0)
                            {
                                orderString.Append(",");
                            }
                            orderString.Append(orderOption.ToParameter());
                            break;
                    }
                }
                if (orderString.Length > 0)
                {
                    request.AddParameter("orderBy", orderString.ToString());
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<StoryDataWrapper> response = client.Execute<StoryDataWrapper>(request);

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
                                                DateTime? ModifiedSince = null,
                                                IEnumerable<int> Comics = null,
                                                IEnumerable<int> Series = null,
                                                IEnumerable<int> Events = null,
                                                IEnumerable<int> Stories = null,
                                                IEnumerable<OrderBy> Order = null,
                                                int? Limit = null,
                                                int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest("/creators", Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
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
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Comics != null && Comics.Any())
            {
                request.AddParameter("comics", string.Join<int>(",", Comics));
            }
            if (Series != null && Series.Any())
            {
                request.AddParameter("series", string.Join<int>(",", Series));
            }
            if (Events != null && Events.Any())
            {
                request.AddParameter("events", string.Join<int>(",", Events));
            }
            if (Stories != null && Stories.Any())
            {
                request.AddParameter("stories", string.Join<int>(",", Stories));
            }
            if (Order != null && Order.Any())
            {
                StringBuilder orderString = new StringBuilder();
                foreach (var orderOption in Order)
                {
                    switch (orderOption)
                    {
                        case OrderBy.FirstName:
                        case OrderBy.FirstNameDesc:
                        case OrderBy.MiddleName:
                        case OrderBy.MiddleNameDesc:
                        case OrderBy.LastName:
                        case OrderBy.LastNameDesc:
                        case OrderBy.Suffix:
                        case OrderBy.SuffixDesc:
                        case OrderBy.Modified:
                        case OrderBy.ModifiedDesc:
                            if (orderString.Length > 0)
                            {
                                orderString.Append(",");
                            }
                            orderString.Append(orderOption.ToParameter());
                            break;
                    }
                }
                if (orderString.Length > 0)
                {
                    request.AddParameter("orderBy", orderString.ToString());
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<CreatorDataWrapper> response = client.Execute<CreatorDataWrapper>(request);

            if (response.Data.Code == 409)
            {
                throw new ArgumentException(response.Data.Status);
            }

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
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/creators/{0}", CreatorId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
            request.AddHeader("Accept", "*/*");

            IRestResponse<CreatorDataWrapper> response = client.Execute<CreatorDataWrapper>(request);

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
        /// <param name="SharedAppearences">Return only comics in which the specified characters appear together (for example in which BOTH Spider-Man and Wolverine appear).</param>
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
                                                        IEnumerable<int> SharedAppearences = null,
                                                        IEnumerable<int> Collaborators = null,
                                                        IEnumerable<OrderBy> Order = null,
                                                        int? Limit = null,
                                                        int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/creators/{0}/comics", CreatorId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
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
                throw new ArgumentException("DateRangeBegin must be greater than DateRangeEnd");
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
            if (Characters != null && Characters.Any())
            {
                request.AddParameter("characters", string.Join<int>(",", Characters));
            }
            if (Series != null && Series.Any())
            {
                request.AddParameter("series", string.Join<int>(",", Series));
            }
            if (Events != null && Events.Any())
            {
                request.AddParameter("events", string.Join<int>(",", Events));
            }
            if (Stories != null && Stories.Any())
            {
                request.AddParameter("stories", string.Join<int>(",", Stories));
            }
            if (SharedAppearences != null && SharedAppearences.Any())
            {
                request.AddParameter("sharedAppearences", string.Join<int>(",", SharedAppearences));
            }
            if (Collaborators != null && Collaborators.Any())
            {
                request.AddParameter("collaborators", string.Join<int>(",", Collaborators));
            }
            if (Order.HasValue)
            {
                switch(Order.Value)
                { 
                    case OrderBy.FocDate:
                    case OrderBy.FocDateDesc:
                    case OrderBy.OnSaleDate:
                    case OrderBy.OnSaleDateDesc:
                    case OrderBy.Title:
                    case OrderBy.TitleDesc:
                    case OrderBy.IssueNumber:
                    case OrderBy.IssueNumberDesc:
                    case OrderBy.Modified:
                    case OrderBy.ModifiedDesc:
                        request.AddParameter("orderBy", Order.Value.ToParameter());
                        break;
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<ComicDataWrapper> response = client.Execute<ComicDataWrapper>(request);

            return response.Data.Data.Results;
        }

        public IEnumerable<Event> GetEventsForCreator(int CreatorId,
                                            string Name = null,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Characters = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Stories = null,
                                            OrderBy? Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/creators/{0}/events/", CreatorId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
            if (!String.IsNullOrWhiteSpace(Name))
            {
                request.AddParameter("name", Name);
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
            if (Order.HasValue)
            {
                switch (Order.Value)
                { 
                    case OrderBy.Name:
                    case OrderBy.NameDesc:
                    case OrderBy.StartDate:
                    case OrderBy.StartDateDesc:
                    case OrderBy.Modified:
                    case OrderBy.ModifiedDesc:
                        request.AddParameter("orderBy", Order.Value.ToParameter());
                        break;
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<EventDataWrapper> response = client.Execute<EventDataWrapper>(request);

            return response.Data.Data.Results;
        }

        public IEnumerable<Story> GetStoriesForCreator(int CreatorId,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Events = null,
                                            IEnumerable<int> Characters = null,
                                            OrderBy? Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/creators/{0}/events/", CreatorId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Comics != null && Comics.Any())
            {
                request.AddParameter("comics", string.Join<int>(",", Comics));
            }
            if (Series != null && Series.Any())
            {
                request.AddParameter("series", string.Join<int>(",", Series));
            }
            if (Events != null && Events.Any())
            {
                request.AddParameter("stories", string.Join<int>(",", Events));
            }
            if (Characters != null && Characters.Any())
            {
                request.AddParameter("characters", string.Join<int>(",", Characters));
            }
            if (Order.HasValue)
            {
                switch (Order.Value)
                { 
                    case OrderBy.Id:
                    case OrderBy.IdDesc:
                    case OrderBy.Modified:
                    case OrderBy.ModifiedDesc:
                        request.AddParameter("orderBy", Order.Value.ToParameter());
                        break;
                }   
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<StoryDataWrapper> response = client.Execute<StoryDataWrapper>(request);

            return response.Data.Data.Results;
        }
        #endregion

        #region Events

        public IEnumerable<Event> GetEvents(string Name = null,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Creators = null,
                                            IEnumerable<int> Characters = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Stories = null,
                                            OrderBy? Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest("/events/", Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
            if (!String.IsNullOrWhiteSpace(Name))
            {
                request.AddParameter("name", Name);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Creators != null && Creators.Any())
            {
                request.AddParameter("creators", string.Join<int>(",", Creators));
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
            if (Order.HasValue)
            {
                switch (Order.Value)
                {
                    case OrderBy.Name:
                    case OrderBy.NameDesc:
                    case OrderBy.StartDate:
                    case OrderBy.StartDateDesc:
                    case OrderBy.Modified:
                    case OrderBy.ModifiedDesc:
                        request.AddParameter("orderBy", Order.Value.ToParameter());
                        break;
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<EventDataWrapper> response = client.Execute<EventDataWrapper>(request);

            return response.Data.Data.Results;
        }

        public Event GetEvent(int EventId)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/events/{0}", EventId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
            request.AddHeader("Accept", "*/*");

            IRestResponse<EventDataWrapper> response = client.Execute<EventDataWrapper>(request);

            return response.Data.Data.Results.FirstOrDefault(ev => ev.Id == EventId);
        }

        public IEnumerable<Character> GetCharactersForEvent(int EventId,
                                            string Name = null,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Stories = null,
                                            OrderBy? Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/events/{0}/characters", EventId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
            if (!String.IsNullOrWhiteSpace(Name))
            {
                request.AddParameter("name", Name);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Comics != null && Comics.Any())
            {
                request.AddParameter("comics", string.Join<int>(",", Comics));
            }
            if (Series != null && Series.Any())
            {
                request.AddParameter("series", string.Join<int>(",", Series));
            }
            if (Stories != null && Stories.Any())
            {
                request.AddParameter("stories", string.Join<int>(",", Stories));
            }
            if (Order.HasValue)
            {
                switch (Order.Value)
                {
                    case OrderBy.Name:
                    case OrderBy.NameDesc:
                    case OrderBy.Modified:
                    case OrderBy.ModifiedDesc:
                        request.AddParameter("orderBy", Order.Value.ToParameter());
                        break;
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<CharacterDataWrapper> response = client.Execute<CharacterDataWrapper>(request);

            return response.Data.Data.Results;
        }

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
                                                        IEnumerable<int> SharedAppearences = null,
                                                        IEnumerable<int> Collaborators = null,
                                                        OrderBy? Order = null,
                                                        int? Limit = null,
                                                        int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/events/{0}/comics", EventId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
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
                request.AddParameter("dateRange", String.Format("{0},{1}", DateRangeBegin.Value.ToString("YYYY-MM-DD"), DateRangeEnd.Value.ToString("YYYY-MM-DD")));
            }
            else if (DateRangeBegin.HasValue || DateRangeEnd.HasValue)
            {
                // Give error message here, need both start and end for range
            }
            if (HasDigitalIssue.HasValue)
            {
                request.AddParameter("hasDigitalIssue", HasDigitalIssue.Value.ToString().ToLower());
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Creators != null && Creators.Any())
            {
                request.AddParameter("creators", string.Join<int>(",", Creators));
            }
            if (Characters != null && Characters.Any())
            {
                request.AddParameter("characters", string.Join<int>(",", Characters));
            }
            if (Series != null && Series.Any())
            {
                request.AddParameter("series", string.Join<int>(",", Series));
            }
            if (Events != null && Events.Any())
            {
                request.AddParameter("events", string.Join<int>(",", Events));
            }
            if (Stories != null && Stories.Any())
            {
                request.AddParameter("stories", string.Join<int>(",", Stories));
            }
            if (SharedAppearences != null && SharedAppearences.Any())
            {
                request.AddParameter("sharedAppearences", string.Join<int>(",", SharedAppearences));
            }
            if (Collaborators != null && Collaborators.Any())
            {
                request.AddParameter("collaborators", string.Join<int>(",", Collaborators));
            }
            if (Order.HasValue)
            {
                switch(Order.Value)
                {
                    case OrderBy.FocDate:
                    case OrderBy.FocDateDesc:
                    case OrderBy.OnSaleDate:
                    case OrderBy.OnSaleDateDesc:
                    case OrderBy.Title:
                    case OrderBy.TitleDesc:
                    case OrderBy.IssueNumber:
                    case OrderBy.IssueNumberDesc:
                    case OrderBy.Modified:
                    case OrderBy.ModifiedDesc:
                        request.AddParameter("orderBy", Order.Value.ToParameter());
                        break;
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<ComicDataWrapper> response = client.Execute<ComicDataWrapper>(request);

            return response.Data.Data.Results;
        }

        public IEnumerable<Creator> GetCreatorsForEvent(int EventId,
                                            string FirstName = null,
                                            string MiddleName = null,
                                            string LastName = null,
                                            string Suffix = null,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Stories = null,
                                            OrderBy? Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/events/{0}/creators", EventId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
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
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Comics != null && Comics.Any())
            {
                request.AddParameter("comics", string.Join<int>(",", Comics));
            }
            if (Series != null && Series.Any())
            {
                request.AddParameter("series", string.Join<int>(",", Series));
            }
            if (Stories != null && Stories.Any())
            {
                request.AddParameter("stories", string.Join<int>(",", Stories));
            }
            if (Order.HasValue)
            {
                switch (Order.Value)
                {
                    case OrderBy.FirstName:
                    case OrderBy.FirstNameDesc:
                    case OrderBy.MiddleName:
                    case OrderBy.MiddleNameDesc:
                    case OrderBy.LastName:
                    case OrderBy.LastNameDesc:
                    case OrderBy.Suffix:
                    case OrderBy.SuffixDesc:
                    case OrderBy.Modified:
                    case OrderBy.ModifiedDesc:
                        request.AddParameter("orderBy", Order.Value.ToParameter());
                        break;
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<CreatorDataWrapper> response = client.Execute<CreatorDataWrapper>(request);

            return response.Data.Data.Results;
        }

        public IEnumerable<Story> GetStoriesForEvent(int EventId,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Creators = null,
                                            IEnumerable<int> Characters = null,
                                            OrderBy? Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/events/{0}/stories", EventId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Comics != null && Comics.Any())
            {
                request.AddParameter("comics", string.Join<int>(",", Comics));
            }
            if (Series != null && Series.Any())
            {
                request.AddParameter("series", string.Join<int>(",", Series));
            }
            if (Creators != null && Creators.Any())
            {
                request.AddParameter("creators", string.Join<int>(",", Creators));
            }
            if (Characters != null && Characters.Any())
            {
                request.AddParameter("characters", string.Join<int>(",", Characters));
            }
            if (Order.HasValue)
            {
                switch (Order.Value)
                {
                    case OrderBy.Id:
                    case OrderBy.IdDesc:
                    case OrderBy.Modified:
                    case OrderBy.ModifiedDesc:
                        request.AddParameter("orderBy", Order.Value.ToParameter());
                        break;
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<StoryDataWrapper> response = client.Execute<StoryDataWrapper>(request);

            return response.Data.Data.Results;
        }
        #endregion

        #region Series
        public IEnumerable<Series> GetSeries(string Title = null,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Stories = null,
                                            IEnumerable<int> Events = null,
                                            IEnumerable<int> Creators = null,
                                            IEnumerable<int> Characters = null,
                                            SeriesType? Type = null,
                                            ComicFormat? Contains = null,
                                            OrderBy? Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest("/series", Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
            if (!String.IsNullOrWhiteSpace(Title))
            {
                request.AddParameter("title", Title);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Comics != null && Comics.Any())
            {
                request.AddParameter("comics", string.Join<int>(",", Comics));
            }
            if (Stories != null && Stories.Any())
            {
                request.AddParameter("stories", string.Join<int>(",", Stories));
            }
            if (Events != null && Events.Any())
            {
                request.AddParameter("events", string.Join<int>(",", Events));
            }
            if (Creators != null && Creators.Any())
            {
                request.AddParameter("creators", string.Join<int>(",", Creators));
            }
            if (Characters != null && Characters.Any())
            {
                request.AddParameter("characters", string.Join<int>(",", Characters));
            }
            if (Type.HasValue)
            {
                request.AddParameter("seriesType", Type.Value.ToParameter());
            }
            if (Contains.HasValue)
            {
                request.AddParameter("contains", Contains.Value.ToParameter());
            }
            if (Order.HasValue)
            {
                switch (Order.Value)
                {
                    case OrderBy.Title:
                    case OrderBy.TitleDesc:
                    case OrderBy.Modified:
                    case OrderBy.ModifiedDesc:
                    case OrderBy.StartYear:
                    case OrderBy.StartYearDesc:
                        request.AddParameter("orderBy", Order.Value.ToParameter());
                        break;
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<SeriesDataWrapper> response = client.Execute<SeriesDataWrapper>(request);

            return response.Data.Data.Results;
        }

        public Series GetSeries(int SeriesId)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/series/{0}", SeriesId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
            request.AddHeader("Accept", "*/*");

            IRestResponse<SeriesDataWrapper> response = client.Execute<SeriesDataWrapper>(request);

            return response.Data.Data.Results.FirstOrDefault(series => series.Id == SeriesId);
        }

        public IEnumerable<Character> GetCharactersForSeries(int SeriesId,
                                            string Name = null,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Events = null,
                                            IEnumerable<int> Stories = null,
                                            OrderBy? Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/series/{0}/characters", SeriesId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
            if (!String.IsNullOrWhiteSpace(Name))
            {
                request.AddParameter("name", Name);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Comics != null && Comics.Any())
            {
                request.AddParameter("comics", string.Join<int>(",", Comics));
            }
            if (Events != null && Events.Any())
            {
                request.AddParameter("events", string.Join<int>(",", Events));
            }
            if (Stories != null && Stories.Any())
            {
                request.AddParameter("stories", string.Join<int>(",", Stories));
            }
            if (Order.HasValue)
            {
                switch (Order.Value)
                {
                    case OrderBy.Name:
                    case OrderBy.NameDesc:
                    case OrderBy.Modified:
                    case OrderBy.ModifiedDesc:
                        request.AddParameter("orderBy", Order.Value.ToParameter());
                        break;
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<CharacterDataWrapper> response = client.Execute<CharacterDataWrapper>(request);

            return response.Data.Data.Results;
        }

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
                                                        IEnumerable<int> SharedAppearences = null,
                                                        IEnumerable<int> Collaborators = null,
                                                        OrderBy? Order = null,
                                                        int? Limit = null,
                                                        int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/series/{0}/comics", SeriesId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
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
                request.AddParameter("dateRange", String.Format("{0},{1}", DateRangeBegin.Value.ToString("YYYY-MM-DD"), DateRangeEnd.Value.ToString("YYYY-MM-DD")));
            }
            else if (DateRangeBegin.HasValue || DateRangeEnd.HasValue)
            {
                // Give error message here, need both start and end for range
            }
            if (HasDigitalIssue.HasValue)
            {
                request.AddParameter("hasDigitalIssue", HasDigitalIssue.Value.ToString().ToLower());
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Creators != null && Creators.Any())
            {
                request.AddParameter("creators", string.Join<int>(",", Creators));
            }
            if (Characters != null && Characters.Any())
            {
                request.AddParameter("characters", string.Join<int>(",", Characters));
            }
            if (Events != null && Events.Any())
            {
                request.AddParameter("events", string.Join<int>(",", Events));
            }
            if (Stories != null && Stories.Any())
            {
                request.AddParameter("stories", string.Join<int>(",", Stories));
            }
            if (SharedAppearences != null && SharedAppearences.Any())
            {
                request.AddParameter("sharedAppearences", string.Join<int>(",", SharedAppearences));
            }
            if (Collaborators != null && Collaborators.Any())
            {
                request.AddParameter("collaborators", string.Join<int>(",", Collaborators));
            }
            if (Order.HasValue)
            {
                switch (Order.Value)
                {
                    case OrderBy.FocDate:
                    case OrderBy.FocDateDesc:
                    case OrderBy.OnSaleDate:
                    case OrderBy.OnSaleDateDesc:
                    case OrderBy.Title:
                    case OrderBy.TitleDesc:
                    case OrderBy.IssueNumber:
                    case OrderBy.IssueNumberDesc:
                    case OrderBy.Modified:
                    case OrderBy.ModifiedDesc:
                        request.AddParameter("orderBy", Order.Value.ToParameter());
                        break;
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<ComicDataWrapper> response = client.Execute<ComicDataWrapper>(request);

            return response.Data.Data.Results;
        }

        public IEnumerable<Creator> GetCreatorsForSeries(int SeriesId,
                                            string FirstName = null,
                                            string MiddleName = null,
                                            string LastName = null,
                                            string Suffix = null,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Events = null,
                                            IEnumerable<int> Stories = null,
                                            OrderBy? Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/series/{0}/creators", SeriesId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
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
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Comics != null && Comics.Any())
            {
                request.AddParameter("comics", string.Join<int>(",", Comics));
            }
            if (Events != null && Events.Any())
            {
                request.AddParameter("events", string.Join<int>(",", Events));
            }
            if (Stories != null && Stories.Any())
            {
                request.AddParameter("stories", string.Join<int>(",", Stories));
            }
            if (Order.HasValue)
            {
                switch (Order.Value)
                {
                    case OrderBy.FirstName:
                    case OrderBy.FirstNameDesc:
                    case OrderBy.MiddleName:
                    case OrderBy.MiddleNameDesc:
                    case OrderBy.LastName:
                    case OrderBy.LastNameDesc:
                    case OrderBy.Suffix:
                    case OrderBy.SuffixDesc:
                    case OrderBy.Modified:
                    case OrderBy.ModifiedDesc:
                        request.AddParameter("orderBy", Order.Value.ToParameter());
                        break;
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<CreatorDataWrapper> response = client.Execute<CreatorDataWrapper>(request);

            return response.Data.Data.Results;
        }

        public IEnumerable<Event> GetEventsForSeries(int SeriesId,
                                            string Name = null,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Creators = null,
                                            IEnumerable<int> Characters = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Stories = null,
                                            OrderBy? Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/series/{0}/events", SeriesId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
            if (!String.IsNullOrWhiteSpace(Name))
            {
                request.AddParameter("name", Name);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Creators != null && Creators.Any())
            {
                request.AddParameter("series", string.Join<int>(",", Creators));
            }
            if (Characters != null && Characters.Any())
            {
                request.AddParameter("characters", string.Join<int>(",", Characters));
            }
            if (Comics != null && Comics.Any())
            {
                request.AddParameter("comics", string.Join<int>(",", Comics));
            }
            if (Stories != null && Stories.Any())
            {
                request.AddParameter("stories", string.Join<int>(",", Stories));
            }
            if (Order.HasValue)
            {
                switch (Order.Value)
                {
                    case OrderBy.Name:
                    case OrderBy.NameDesc:
                    case OrderBy.StartDate:
                    case OrderBy.StartDateDesc:
                    case OrderBy.Modified:
                    case OrderBy.ModifiedDesc:
                        request.AddParameter("orderBy", Order.Value.ToParameter());
                        break;
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<EventDataWrapper> response = client.Execute<EventDataWrapper>(request);

            return response.Data.Data.Results;
        }

        public IEnumerable<Story> GetStoriesForSeries(int SeriesId,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Events = null,
                                            IEnumerable<int> Creators = null,
                                            IEnumerable<int> Characters = null,
                                            OrderBy? Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/series/{0}/stories", SeriesId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Comics != null && Comics.Any())
            {
                request.AddParameter("comics", string.Join<int>(",", Comics));
            }
            if (Events != null && Events.Any())
            {
                request.AddParameter("events", string.Join<int>(",", Events));
            }
            if (Creators != null && Creators.Any())
            {
                request.AddParameter("series", string.Join<int>(",", Creators));
            }
            if (Characters != null && Characters.Any())
            {
                request.AddParameter("characters", string.Join<int>(",", Characters));
            }
            if (Order.HasValue)
            {
                switch (Order.Value)
                {
                    case OrderBy.Id:
                    case OrderBy.IdDesc:
                    case OrderBy.Modified:
                    case OrderBy.ModifiedDesc:
                        request.AddParameter("orderBy", Order.Value.ToParameter());
                        break;
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<StoryDataWrapper> response = client.Execute<StoryDataWrapper>(request);

            return response.Data.Data.Results;
        }
        #endregion

        #region Stories
        public IEnumerable<Story> GetStories(DateTime? ModifiedSince = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Events = null,
                                            IEnumerable<int> Creators = null,
                                            IEnumerable<int> Characters = null,
                                            OrderBy? Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest("/stories", Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Comics != null && Comics.Any())
            {
                request.AddParameter("comics", string.Join<int>(",", Comics));
            }
            if (Series != null && Series.Any())
            {
                request.AddParameter("series", string.Join<int>(",", Series));
            }
            if (Events != null && Events.Any())
            {
                request.AddParameter("events", string.Join<int>(",", Events));
            }
            if (Creators != null && Creators.Any())
            {
                request.AddParameter("creators", string.Join<int>(",", Creators));
            }
            if (Characters != null && Characters.Any())
            {
                request.AddParameter("characters", string.Join<int>(",", Characters));
            }
            if (Order.HasValue)
            {
                switch(Order.Value)
                {
                    case OrderBy.Id:
                    case OrderBy.IdDesc:
                    case OrderBy.Modified:
                    case OrderBy.ModifiedDesc:
                        request.AddParameter("orderBy", Order.Value.ToParameter());
                        break;
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<StoryDataWrapper> response = client.Execute<StoryDataWrapper>(request);

            return response.Data.Data.Results;
        }

        public Story GetStory(int StoryId)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/stories/{0}", StoryId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
            
            request.AddHeader("Accept", "*/*");

            IRestResponse<StoryDataWrapper> response = client.Execute<StoryDataWrapper>(request);

            return response.Data.Data.Results.FirstOrDefault(story => story.Id == StoryId);
        }

        public IEnumerable<Character> GetCharactersForStory(int StoryId,
                                            string Name = null,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Comics = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Events = null,
                                            OrderBy? Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/stories/{0}/characters", StoryId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
            if (!String.IsNullOrWhiteSpace(Name))
            {
                request.AddParameter("name", Name);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Comics != null && Comics.Any())
            {
                request.AddParameter("comics", string.Join<int>(",", Comics));
            }
            if (Events != null && Events.Any())
            {
                request.AddParameter("events", string.Join<int>(",", Events));
            }
            if (Series != null && Series.Any())
            {
                request.AddParameter("series", string.Join<int>(",", Series));
            }
            if (Order.HasValue)
            {
                switch (Order.Value)
                {
                    case OrderBy.Name:
                    case OrderBy.NameDesc:
                    case OrderBy.Modified:
                    case OrderBy.ModifiedDesc:
                        request.AddParameter("orderBy", Order.Value.ToParameter());
                        break;
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<CharacterDataWrapper> response = client.Execute<CharacterDataWrapper>(request);

            return response.Data.Data.Results;
        }

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
                                                        IEnumerable<int> SharedAppearences = null,
                                                        IEnumerable<int> Collaborators = null,
                                                        OrderBy? Order = null,
                                                        int? Limit = null,
                                                        int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/stories/{0}/comics", StoryId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
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
                request.AddParameter("dateRange", String.Format("{0},{1}", DateRangeBegin.Value.ToString("YYYY-MM-DD"), DateRangeEnd.Value.ToString("YYYY-MM-DD")));
            }
            else if (DateRangeBegin.HasValue || DateRangeEnd.HasValue)
            {
                // Give error message here, need both start and end for range
            }
            if (HasDigitalIssue.HasValue)
            {
                request.AddParameter("hasDigitalIssue", HasDigitalIssue.Value.ToString().ToLower());
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Creators != null && Creators.Any())
            {
                request.AddParameter("creators", string.Join<int>(",", Creators));
            }
            if (Characters != null && Characters.Any())
            {
                request.AddParameter("characters", string.Join<int>(",", Characters));
            }
            if (Series != null && Series.Any())
            {
                request.AddParameter("series", string.Join<int>(",", Series));
            }
            if (Events != null && Events.Any())
            {
                request.AddParameter("events", string.Join<int>(",", Events));
            }
            if (SharedAppearences != null && SharedAppearences.Any())
            {
                request.AddParameter("sharedAppearences", string.Join<int>(",", SharedAppearences));
            }
            if (Collaborators != null && Collaborators.Any())
            {
                request.AddParameter("collaborators", string.Join<int>(",", Collaborators));
            }
            if (Order.HasValue)
            {
                switch (Order.Value)
                {
                    case OrderBy.FocDate:
                    case OrderBy.FocDateDesc:
                    case OrderBy.OnSaleDate:
                    case OrderBy.OnSaleDateDesc:
                    case OrderBy.Title:
                    case OrderBy.TitleDesc:
                    case OrderBy.IssueNumber:
                    case OrderBy.IssueNumberDesc:
                    case OrderBy.Modified:
                    case OrderBy.ModifiedDesc:
                        request.AddParameter("orderBy", Order.Value.ToParameter());
                        break;
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<ComicDataWrapper> response = client.Execute<ComicDataWrapper>(request);

            return response.Data.Data.Results;
        }

        public IEnumerable<Creator> GetCreatorsForStory(int StoryId,
                                                        string FirstName = null,
                                                        string MiddleName = null,
                                                        string LastName = null,
                                                        string Suffix = null,
                                                        DateTime? ModifiedSince = null,
                                                        IEnumerable<int> Comics = null,
                                                        IEnumerable<int> Series = null,
                                                        IEnumerable<int> Events = null,
                                                        OrderBy? Order = null,
                                                        int? Limit = null,
                                                        int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/stories/{0}/creators", StoryId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
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
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Comics != null && Comics.Any())
            {
                request.AddParameter("comics", string.Join<int>(",", Comics));
            }
            if (Series != null && Series.Any())
            {
                request.AddParameter("series", string.Join<int>(",", Series));
            }
            if (Events != null && Events.Any())
            {
                request.AddParameter("events", string.Join<int>(",", Events));
            }
            if (Order.HasValue)
            {
                switch (Order.Value)
                {
                    case OrderBy.FirstName:
                    case OrderBy.FirstNameDesc:
                    case OrderBy.MiddleName:
                    case OrderBy.MiddleNameDesc:
                    case OrderBy.LastName:
                    case OrderBy.LastNameDesc:
                    case OrderBy.Suffix:
                    case OrderBy.SuffixDesc:
                    case OrderBy.Modified:
                    case OrderBy.ModifiedDesc:
                        request.AddParameter("orderBy", Order.Value.ToParameter());
                        break;
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<CreatorDataWrapper> response = client.Execute<CreatorDataWrapper>(request);

            return response.Data.Data.Results;
        }

        public IEnumerable<Event> GetEventsForStories(int StoryId,
                                            string Name = null,
                                            DateTime? ModifiedSince = null,
                                            IEnumerable<int> Creators = null,
                                            IEnumerable<int> Characters = null,
                                            IEnumerable<int> Series = null,
                                            IEnumerable<int> Comics = null,
                                            OrderBy? Order = null,
                                            int? Limit = null,
                                            int? Offset = null)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest(String.Format("/stories/{0}/events/", StoryId), Method.GET);
            var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            request.AddParameter("apikey", PublicApiKey);
            request.AddParameter("ts", timestamp);
            request.AddParameter("hash", CreateHash(String.Format("{0}{1}{2}", timestamp, PrivateApiKey, PublicApiKey)));
            if (!String.IsNullOrWhiteSpace(Name))
            {
                request.AddParameter("name", Name);
            }
            if (ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            if (Creators != null && Creators.Any())
            {
                request.AddParameter("series", string.Join<int>(",", Creators));
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
            if (Order.HasValue)
            {
                switch (Order.Value)
                {
                    case OrderBy.Name:
                    case OrderBy.NameDesc:
                    case OrderBy.StartDate:
                    case OrderBy.StartDateDesc:
                    case OrderBy.Modified:
                    case OrderBy.ModifiedDesc:
                        request.AddParameter("orderBy", Order.Value.ToParameter());
                        break;
                }
            }
            if (Limit.HasValue && Limit.Value > 0)
            {
                request.AddParameter("limit", Limit.Value.ToString());
            }
            if (Offset.HasValue && Limit.Value > 0)
            {
                request.AddParameter("offset", Offset.Value.ToString());
            }

            request.AddHeader("Accept", "*/*");

            IRestResponse<EventDataWrapper> response = client.Execute<EventDataWrapper>(request);

            return response.Data.Data.Results;
        }
        #endregion
    }

    #region Extras
    public class MarvelUrl
    {
        public string Type { get; set; }
        public string UrlString { get; set; }
    }

    public class MarvelImage
    {
        public string Path { get; set; }
        public string Extension { get; set; }
    }

    public class TextObject
    {
        public string Type { get; set; }
        public string Language { get; set; }
        public string Text { get; set; }
    }
    #endregion
}
