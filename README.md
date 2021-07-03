MarvelAPI
=========


Simple way to access Marvel's REST API. For full details on getting started with the Marvel API, go check out the [official website](https://developer.marvel.com/).


## Walkthrough example:

### Create connection:
```csharp
var marvel = new Marvel(_MarvelPublicKey, _MarvelPrivateKey)
```

### Get an individual character:
```csharp
var characterId = 1009268; // Deadpool
Character character = marvel.GetCharacter(characterId);
```

### Get filtered characters:
```csharp
IEnumerable<Character> characters = marvel.GetCharacters(new GetCharacters{Name, ModifiedSince, Comics, Series, Events, Stories, Order, Limit, Offset});
```
