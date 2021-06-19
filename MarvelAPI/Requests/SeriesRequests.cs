using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI.Requests
{
    public class SeriesRequests : BaseRequest
    {
        public SeriesRequests(string publicApiKey, string privateApiKey, RestClient client, bool? useGZip = null) 
            : base(publicApiKey, privateApiKey, client, useGZip)
        {
        }

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
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("yyyy-MM-dd"));
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

            IRestResponse<Wrapper<Series>> response = Client.Execute<Wrapper<Series>>(request);

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

            IRestResponse<Wrapper<Series>> response = Client.Execute<Wrapper<Series>>(request);

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
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("yyyy-MM-dd"));
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

            IRestResponse<Wrapper<Character>> response = Client.Execute<Wrapper<Character>>(request);

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
                    request.AddParameter("dateRange", String.Format("{0},{1}", DateRangeBegin.Value.ToString("yyyy-MM-dd"), DateRangeEnd.Value.ToString("yyyy-MM-dd")));
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
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("yyyy-MM-dd"));
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

            IRestResponse<Wrapper<Comic>> response = Client.Execute<Wrapper<Comic>>(request);

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
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("yyyy-MM-dd"));
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

            IRestResponse<Wrapper<Creator>> response = Client.Execute<Wrapper<Creator>>(request);

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
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("yyyy-MM-dd"));
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

            IRestResponse<Wrapper<Event>> response = Client.Execute<Wrapper<Event>>(request);

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
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("yyyy-MM-dd"));
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

            IRestResponse<Wrapper<Story>> response = Client.Execute<Wrapper<Story>>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }
    }
}
