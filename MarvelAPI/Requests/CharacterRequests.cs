using MarvelAPI.Parameters;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI
{
    public class CharacterRequests : BaseRequest
    {
        public CharacterRequests(string publicApiKey, string privateApiKey, IRestClient client, bool? useGZip = null) 
            : base(publicApiKey, privateApiKey, client, useGZip)
        { }

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
                request.AddParameter("modifiedSince", model.ModifiedSince.Value.ToString("yyyy-MM-dd"));
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
            var request = CreateRequest($"/characters/{CharacterId}");

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
            var request = CreateRequest($"/characters/{model.CharacterId}/comics");
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
                    request.AddParameter("dateRange", $"{model.DateRangeBegin.Value.ToString("yyyy-MM-dd")},{model.DateRangeEnd.Value.ToString("yyyy-MM-dd")}");
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
        /// <returns>
        /// Lists of events
        /// </returns>
        public IEnumerable<Event> GetEventsForCharacter(GetEventsForCharacter model)
        {
            var request = CreateRequest($"/characters/{model.CharacterId}/events");

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
        /// Fetches lists of comic series in which a specific character appears, with optional filters.
        /// </summary>
        /// <returns>
        /// Lists of comic series
        /// </returns>
        public IEnumerable<Series> GetSeriesForCharacter(GetSeriesForCharacter model)
        {
            var request = CreateRequest($"/characters/{model.CharacterId}/series");

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
            request.AddParameterList(model.Events, "events");
            request.AddParameterList(model.Creators, "creators");

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
        /// Fetches lists of comic stories featuring a specific character with optional filters.
        /// </summary>
        /// <returns>
        /// Lists of stories
        /// </returns>
        public IEnumerable<Story> GetStoriesForCharacter(GetStoriesForCharacter model)
        {
            var request = CreateRequest($"/characters/{model.CharacterId}/stories");

            if (model.ModifiedSince.HasValue)
            {
                request.AddParameter("modifiedSince", model.ModifiedSince.Value.ToString("yyyy-MM-dd"));
            }
            request.AddParameterList(model.Creators, "creators");
            request.AddParameterList(model.Series, "series");
            request.AddParameterList(model.Comics, "comics");
            request.AddParameterList(model.Events, "events");

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