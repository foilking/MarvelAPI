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

```
