using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI
{
    public class StoryRequests : BaseRequest
    {
        public StoryRequests(string publicApiKey, string privateApiKey, RestClient client, bool? useGZip = null) 
            : base(publicApiKey, privateApiKey, client, useGZip)
        {
        }

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
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("yyyy-MM-dd"));
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

            IRestResponse<Wrapper<Story>> response = Client.Execute<Wrapper<Story>>(request);

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

            IRestResponse<Wrapper<Story>> response = Client.Execute<Wrapper<Story>>(request);

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
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("yyyy-MM-dd"));
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

            IRestResponse<Wrapper<Character>> response = Client.Execute<Wrapper<Character>>(request);

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

            IRestResponse<Wrapper<Comic>> response = Client.Execute<Wrapper<Comic>>(request);

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
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("yyyy-MM-dd"));
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

            IRestResponse<Wrapper<Creator>> response = Client.Execute<Wrapper<Creator>>(request);

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
                request.AddParameter("modifiedSince", ModifiedSince.Value.ToString("yyyy-MM-dd"));
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

            IRestResponse<Wrapper<Event>> response = Client.Execute<Wrapper<Event>>(request);

            HandleResponseErrors(response);

            return response.Data.Data.Results;
        }
    }
}
