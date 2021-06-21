using MarvelAPI.Parameters;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI.Requests
{
    public class EventRequest : BaseRequest
    {
        public EventRequest(string publicApiKey, string privateApiKey, IRestClient client, bool? useGZip = null)
            : base(publicApiKey, privateApiKey, client, useGZip)
        {
        }

        /// <summary>
        /// Fetches lists of events with optional filters.
        /// </summary>
        /// <returns>
        /// Lists of events
        /// </returns>
        public IEnumerable<Event> GetEvents(GetEvents model)
        {
            var request = CreateRequest("/events");

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
                request.AddParameter("modifiedSince", model.ModifiedSince.Value.ToString("yyyy-MM-dd"));
            }

            request.AddParameterList(model.Creators, "creators");
            request.AddParameterList(model.Characters, "characters");
            request.AddParameterList(model.Series, "series");
            request.AddParameterList(model.Comics, "comics");
            request.AddParameterList(model.Stories, "stories");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Name,
                OrderBy.NameDesc,
                OrderBy.StartDate,
                OrderBy.StartDateDesc,
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

            IRestResponse<Wrapper<Event>> response = Client.Execute<Wrapper<Event>>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }


        /// <summary>
        /// This method fetches a single event resource. It is the canonical URI for any event resource provided by the API.
        /// </summary>
        /// <returns>
        /// A single event resource
        /// </returns>
        public Event GetEvent(int eventId)
        {
            var request = CreateRequest($"/events/{eventId}");

            IRestResponse<Wrapper<Event>> response = Client.Execute<Wrapper<Event>>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results.FirstOrDefault(ev => ev.Id == eventId);
        }

        /// <summary>
        /// Fetches lists of characters which appear in a specific event, with optional filters.
        /// </summary>
        /// <returns>
        /// Lists of characters which appear in a specific event
        /// </returns>
        public IEnumerable<Character> GetCharactersForEvent(GetCharactersForEvent model)
        {
            var request = CreateRequest($"/events/{model.EventId}/characters");

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
                request.AddParameter("modifiedSince", model.ModifiedSince.Value.ToString("yyyy-MM-dd"));
            }

            request.AddParameterList(model.Comics, "comics");
            request.AddParameterList(model.Series, "series");
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
            if (model.Offset.HasValue && model.Offset.Value > 0)
            {
                request.AddParameter("offset", model.Offset.Value.ToString());
            }

            IRestResponse<Wrapper<Character>> response = Client.Execute<Wrapper<Character>>(request);

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
        public IEnumerable<Comic> GetComicsForEvent(GetComicsForEvent model)
        {
            var request = CreateRequest($"/events/{model.EventId}/comics");

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
                    request.AddParameter("dateRange", $"{model.DateRangeBegin.Value.ToString("yyyy -MM-dd")},{model.DateRangeEnd.Value.ToString("yyyy-MM-dd")}");
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
                request.AddParameter("modifiedSince", model.ModifiedSince.Value.ToString("yyyy-MM-dd"));
            }

            request.AddParameterList(model.Creators, "creators");
            request.AddParameterList(model.Characters, "characters");
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
        /// Fetches lists of comic creators whose work appears in a specific event, with optional filters.
        /// </summary>
        /// <returns>
        /// Lists of comic creators whose work appears in a specific event
        /// </returns>
        public IEnumerable<Creator> GetCreatorsForEvent(GetCreatorsForEvent model)
        {
            var request = CreateRequest($"/events/{model.EventId}/creators");

            if (!string.IsNullOrWhiteSpace(model.FirstName))
            {
                request.AddParameter("firstName", model.FirstName);
            }
            if (!string.IsNullOrWhiteSpace(model.MiddleName))
            {
                request.AddParameter("middleName", model.MiddleName);
            }
            if (!string.IsNullOrWhiteSpace(model.LastName))
            {
                request.AddParameter("lastName", model.LastName);
            }
            if (!string.IsNullOrWhiteSpace(model.Suffix))
            {
                request.AddParameter("suffix", model.Suffix);
            }
            if (!string.IsNullOrWhiteSpace(model.NameStartsWith))
            {
                request.AddParameter("nameStartsWith", model.NameStartsWith);
            }
            if (!string.IsNullOrWhiteSpace(model.FirstNameStartsWith))
            {
                request.AddParameter("firstNameStartsWith", model.FirstNameStartsWith);
            }
            if (!string.IsNullOrWhiteSpace(model.MiddleNameStartsWith))
            {
                request.AddParameter("middleNameStartsWith", model.MiddleNameStartsWith);
            }
            if (!string.IsNullOrWhiteSpace(model.LastNameStartsWith))
            {
                request.AddParameter("lastNameStartsWith", model.LastNameStartsWith);
            }
            if (model.ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", model.ModifiedSince.Value.ToString("yyyy-MM-dd"));
            }

            request.AddParameterList(model.Comics, "comics");
            request.AddParameterList(model.Series, "series");
            request.AddParameterList(model.Stories, "stories");

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
            request.AddOrderByParameterList(model.Order, availableOrderBy);

            if (model.Limit.HasValue && model.Limit.Value > 0)
            {
                request.AddParameter("limit", model.Limit.Value.ToString());
            }
            if (model.Offset.HasValue && model.Offset.Value > 0)
            {
                request.AddParameter("offset", model.Offset.Value.ToString());
            }

            IRestResponse<Wrapper<Creator>> response = Client.Execute<Wrapper<Creator>>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic series in which a specific event takes place, with optional filters.
        /// </summary>
        /// <returns>
        /// Lists of comic series in which a specific event takes place
        /// </returns>
        public IEnumerable<Series> GetSeriesForEvent(GetSeriesForEvent model)
        {
            var request = CreateRequest($"/events/{model.EventId}/series");

            if (!string.IsNullOrWhiteSpace(model.Title))
            {
                request.AddParameter("title", model.Title);
            }
            if (!string.IsNullOrWhiteSpace(model.TitleStartsWith))
            {
                request.AddParameter("titleStartsWith", model.TitleStartsWith);
            }
            if (model.ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", model.ModifiedSince.Value.ToString("yyyy-MM-dd"));
            }

            request.AddParameterList(model.Comics, "comics");
            request.AddParameterList(model.Stories, "stories");
            request.AddParameterList(model.Creators, "creators");
            request.AddParameterList(model.Characters, "characters");

            if (model.SeriesType.HasValue)
            {
                request.AddParameter("seriesType", model.SeriesType.Value.ToParameter());
            }
            if (model.Contains != null && model.Contains.Any())
            {
                var containsParameters = model.Contains.Select(contain => contain.ToParameter());
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
            request.AddOrderByParameterList(model.Order, availableOrderBy);

            if (model.Limit.HasValue && model.Limit.Value > 0)
            {
                request.AddParameter("limit", model.Limit.Value.ToString());
            }
            if (model.Offset.HasValue && model.Offset.Value > 0)
            {
                request.AddParameter("offset", model.Offset.Value.ToString());
            }

            IRestResponse<Wrapper<Series>> response = Client.Execute<Wrapper<Series>>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }

        /// <summary>
        /// Fetches lists of comic stories from a specific event, with optional filters.
        /// </summary>
        /// <returns>
        /// Lists of comic stories from a specific event
        /// </returns>
        public IEnumerable<Story> GetStoriesForEvent(GetStoriesForEvent model)
        {
            var request = CreateRequest($"/events/{model.EventId}/stories");

            if (model.ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", model.ModifiedSince.Value.ToString("yyyy-MM-dd"));
            }

            request.AddParameterList(model.Comics, "comics");
            request.AddParameterList(model.Series, "series");
            request.AddParameterList(model.Creators, "creators");
            request.AddParameterList(model.Characters, "characters");

            var availableOrderBy = new List<OrderBy>
            {
                OrderBy.Id,
                OrderBy.IdDesc,
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

            IRestResponse<Wrapper<Story>> response = Client.Execute<Wrapper<Story>>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }
    }
}
