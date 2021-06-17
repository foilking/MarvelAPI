using MarvelAPI.Parameters;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI
{
    public interface ICharacterRequests
    { }

    public class CharacterRequests : BaseRequest
    {
        public CharacterRequests(string publicApiKey, string privateApiKey, IRestClient client, bool? useGZip = null) 
            : base(publicApiKey, privateApiKey, client, useGZip)
        {
        }

        /// <summary>
        /// Fetches lists of comic characters with optional filters.
        /// </summary>
        /// <returns>
        /// List of comic characters
        /// </returns>
        public IEnumerable<Character> GetCharacters(GetCharacters model)
        {
            var request = CreateRequest("/characters");
            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                request.AddParameter("name", model.Name);
            }
            if (!string.IsNullOrWhiteSpace(model.NameStartsWith))
            {
                request.AddParameter("nameStartsWith", model.NameStartsWith);
            }
            if (model.ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", model.ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }

            request.AddParameterList(model.Comics, "comics");
            request.AddParameterList(model.Series, "series");
            request.AddParameterList(model.Events, "events");
            request.AddParameterList(model.Stories, "stories");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Name,
                OrderBy.NameDesc,
                OrderBy.Modified,
                OrderBy.ModifiedDesc
            };
            request.AddOrderByParameterList(model.Order, availableOrderBy);

            if (model.Limit.HasValue && model.Limit.Value > 0)
            {
                request.AddParameter("limit", model.Limit.Value.ToString());
            }
            if (model.Offset.HasValue && model.Offset.Value >= 0)
            {
                request.AddParameter("offset", model.Offset.Value.ToString());
            }

            IRestResponse<Wrapper<Character>> response = Client.Execute<Wrapper<Character>>(request);

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

            IRestResponse<Wrapper<Character>> response = Client.Execute<Wrapper<Character>>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results.FirstOrDefault(character => character.Id == CharacterId);
        }

        /// <summary>
        /// Fetches lists of comics containing specific character, with optional filters.
        /// </summary>
        /// <returns>
        /// Lists of comics
        /// </returns>
        public IEnumerable<Comic> GetComicsForCharacter(GetComicsForCharacter model)
        {
            var request = CreateRequest(string.Format("/characters/{0}/comics", model.CharacterId));
            if (model.Format.HasValue)
            {
                request.AddParameter("format", model.Format.Value.ToParameter());
            }
            if (model.FormatType.HasValue)
            {
                request.AddParameter("formatType", model.FormatType.Value.ToParameter());
            }
            if (model.NoVariants.HasValue)
            {
                request.AddParameter("noVariants", model.NoVariants.Value.ToString().ToLower());
            }
            if (model.DateDescript.HasValue)
            {
                request.AddParameter("dateDescriptor", model.DateDescript.Value.ToParameter());
            }
            if (model.DateRangeBegin.HasValue && model.DateRangeEnd.HasValue)
            {
                if (model.DateRangeBegin.Value <= model.DateRangeEnd.Value)
                {
                    request.AddParameter("dateRange", string.Format("{0},{1}", model.DateRangeBegin.Value.ToString("YYYY-MM-DD"), model.DateRangeEnd.Value.ToString("YYYY-MM-DD")));
                }
                else
                {
                    throw new ArgumentException("DateRangeBegin must be greater than DateRangeEnd");
                }
            }
            else if (model.DateRangeBegin.HasValue || model.DateRangeEnd.HasValue)
            {
                throw new ArgumentException("Date Range requires both a start and end date");
            }
            if (model.HasDigitalIssue.HasValue)
            {
                request.AddParameter("hasDigitalIssue", model.HasDigitalIssue.Value.ToString().ToLower());
            }
            if (model.ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", model.ModifiedSince.Value.ToString("YYYY-MM-DD"));
            }
            request.AddParameterList(model.Creators, "creators");
            request.AddParameterList(model.Series, "series");
            request.AddParameterList(model.Events, "events");
            request.AddParameterList(model.Stories, "stories");
            request.AddParameterList(model.SharedAppearances, "sharedAppearances");
            request.AddParameterList(model.Collaborators, "collaborators");

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
            request.AddOrderByParameterList(model.Order, availableOrderBy);

            if (model.Limit.HasValue && model.Limit.Value > 0)
            {
                request.AddParameter("limit", model.Limit.Value.ToString());
            }
            if (model.Offset.HasValue && model.Offset.Value > 0)
            {
                request.AddParameter("offset", model.Offset.Value.ToString());
            }

            IRestResponse<Wrapper<Comic>> response = Client.Execute<Wrapper<Comic>>(request);

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

            IRestResponse<Wrapper<Event>> response = Client.Execute<Wrapper<Event>>(request);

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

            IRestResponse<Wrapper<Series>> response = Client.Execute<Wrapper<Series>>(request);

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

            IRestResponse<Wrapper<Story>> response = Client.Execute<Wrapper<Story>>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }
    }
}