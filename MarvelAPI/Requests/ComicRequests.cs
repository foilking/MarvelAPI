using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI
{
    internal class ComicRequests : BaseRequest
    {
        public ComicRequests(string publicApiKey, string privateApiKey, RestClient client, bool? useGZip = null) 
            : base(publicApiKey, privateApiKey, client, useGZip)
        {
        }

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

            IRestResponse<Wrapper<Comic>> response = Client.Execute<Wrapper<Comic>>(request);

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

            IRestResponse<Wrapper<Comic>> response = Client.Execute<Wrapper<Comic>>(request);

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

            IRestResponse<Wrapper<Character>> response = Client.Execute<Wrapper<Character>>(request);

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

            IRestResponse<Wrapper<Creator>> response = Client.Execute<Wrapper<Creator>>(request);

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

            IRestResponse<Wrapper<Event>> response = Client.Execute<Wrapper<Event>>(request);

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

            IRestResponse<Wrapper<Story>> response = Client.Execute<Wrapper<Story>>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }
    }
}
