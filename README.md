MarvelAPI
=========


Simple way to access Marvel's REST API. For full details on getting started with the Marvel API, go check out the [official website](https://developer.marvel.com/).


## Walkthrough example:

### Create connection:
```csharp
var _Marvel = new Marvel(_MarvelPublicKey, _MarvelPrivateKey)
```

### Get an individual character:
```csharp
var characterId = 1009268; // Deadpool
Character character = _Marvel.GetCharacter(characterId);
```

### Get filtered characters:
```csharp
IEnumerable<Character> characters = _Marvel.GetCharacters(Name, ModifiedSince, Comics, Series, Events, Stories, Order, Limit, Offset);
```

## List of all available calls
```csharp
GetCharacters(string Name = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null, IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetCharacter(int CharacterId)

GetComicsForCharacter(int CharacterId, ComicFormat? Format = null, ComicFormatType? FormatType = null, bool? NoVariants = null, DateDescriptor? DateDescript = null, DateTime? DateRangeBegin = null, DateTime? DateRangeEnd = null, bool? HasDigitalIssue = null, DateTime? ModifiedSince = null, IEnumerable<int> Creators = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null, IEnumerable<int> Stories = null, IEnumerable<int> SharedAppearences = null, IEnumerable<int> Collaborators = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetEventsForCharacter(int CharacterId, string Name = null, DateTime? ModifiedSince = null, IEnumerable<int> Creators = null, IEnumerable<int> Series = null, IEnumerable<int> Comics = null, IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetSeriesForCharacter(int CharacterId, string Title = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Stories = null, IEnumerable<int> Events = null, IEnumerable<int> Creators = null, SeriesType? SeriesType = null, IEnumerable<ComicFormat> Contains = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetStoriessForCharacter(int CharacterId, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null, IEnumerable<int> Creators = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetComics(ComicFormat? Format = null, ComicFormatType? FormatType = null, bool? NoVariants = null, DateDescriptor? DateDescript = null, DateTime? DateRangeBegin = null, DateTime? DateRangeEnd = null, bool? HasDigitalIssue = null, DateTime? ModifiedSince = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null, IEnumerable<int> Stories = null, IEnumerable<int> SharedAppearences = null, IEnumerable<int> Collaborators = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetComic(int ComicId)

GetCharactersForComic(int ComicId, string Name = null, DateTime? ModifiedSince = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null, IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetCreatorsForComic(int ComicId, string FirstName = null, string MiddleName = null, string LastName = null, string Suffix = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetEventsForComic(int ComicId, string Name = null, DateTime? ModifiedSince = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<int> Series = null, IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetStoriesForComic(int ComicId, DateTime? ModifiedSince = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetCreators(string FirstName = null, string MiddleName = null, string LastName = null string Suffix = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null, IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetCreator(int CreatorId)

GetComicsForCreator(int CreatorId, ComicFormat? Format = null, ComicFormatType? FormatType = null, bool? NoVariants = null, DateDescriptor? DateDescript = null, DateTime? DateRangeBegin = null, DateTime? DateRangeEnd = null, bool? HasDigitalIssue = null, DateTime? ModifiedSince = null, IEnumerable<int> Characters = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null, IEnumerable<int> Stories = null, IEnumerable<int> SharedAppearences = null, IEnumerable<int> Collaborators = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetEventsForCreator(int CreatorId, string Name = null, DateTime? ModifiedSince = null, IEnumerable<int> Characters = null, IEnumerable<int> Series = null, IEnumerable<int> Comics = null, IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetSeriesForCreator(int CreatorId, string Title = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Stories = null, IEnumerable<int> Events = null, IEnumerable<int> Characters = null, SeriesType? SeriesType = null, IEnumerable<ComicFormat> Contains = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetStoriesForCreator(int CreatorId, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null, IEnumerable<int> Characters = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetEvents(string Name = null, DateTime? ModifiedSince = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<int> Series = null, IEnumerable<int> Comics = null, IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetEvent(int EventId)

GetCharactersForEvent(int EventId, string Name = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetComicsForEvent(int EventId, ComicFormat? Format = null, ComicFormatType? FormatType = null, bool? NoVariants = null, DateDescriptor? DateDescript = null, DateTime? DateRangeBegin = null DateTime? DateRangeEnd = null, bool? HasDigitalIssue = null, DateTime? ModifiedSince = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null, IEnumerable<int> Stories = null, IEnumerable<int> SharedAppearences = null, IEnumerable<int> Collaborators = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetCreatorsForEvent(int EventId, string FirstName = null, string MiddleName = null, string LastName = null, string Suffix = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetSeriesForEvent(int EventId, string Title = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Stories = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, SeriesType? SeriesType = null, IEnumerable<ComicFormat> Contains = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetStoriesForEvent(int EventId, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetSeries(string Title = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Stories = null, IEnumerable<int> Events = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, SeriesType? Type = null, ComicFormat? Contains = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetSeries(int SeriesId)

GetCharactersForSeries(int SeriesId, string Name = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Events = null, IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetComicsForSeries(int SeriesId, ComicFormat? Format = null, ComicFormatType? FormatType = null, bool? NoVariants = null, DateDescriptor? DateDescript = null, DateTime? DateRangeBegin = null, DateTime? DateRangeEnd = null, bool? HasDigitalIssue = null, DateTime? ModifiedSince = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<int> Events = null, IEnumerable<int> Stories = null, IEnumerable<int> SharedAppearences = null, IEnumerable<int> Collaborators = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetCreatorsForSeries(int SeriesId, string FirstName = null, string MiddleName = null, string LastName = null, string Suffix = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Events = null, IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetEventsForSeries(int SeriesId, string Name = null, DateTime? ModifiedSince = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<int> Comics = null, IEnumerable<int> Stories = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetStoriesForSeries(int SeriesId, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Events = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetStories(DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetStory(int StoryId)

GetCharactersForStory(int StoryId, string Name = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null,IEnumerable<int> Series = null, IEnumerable<int> Events = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetComicsForStory(int StoryId, ComicFormat? Format = null, ComicFormatType? FormatType = null, bool? NoVariants = null, DateDescriptor? DateDescript = null, DateTime? DateRangeBegin = null, DateTime? DateRangeEnd = null, bool? HasDigitalIssue = null, DateTime? ModifiedSince = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null, IEnumerable<int> SharedAppearences = null, IEnumerable<int> Collaborators = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetCreatorsForStory(int StoryId, string FirstName = null, string MiddleName = null, string LastName = null, string Suffix = null, DateTime? ModifiedSince = null, IEnumerable<int> Comics = null, IEnumerable<int> Series = null, IEnumerable<int> Events = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

GetEventsForStories(int StoryId, string Name = null, DateTime? ModifiedSince = null, IEnumerable<int> Creators = null, IEnumerable<int> Characters = null, IEnumerable<int> Series = null, IEnumerable<int> Comics = null, IEnumerable<OrderBy> Order = null, int? Limit = null, int? Offset = null)

```
