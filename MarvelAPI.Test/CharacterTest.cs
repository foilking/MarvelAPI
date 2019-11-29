﻿using ApprovalTests;
using ApprovalTests.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MarvelAPI.Test
{
    using ApprovalTests.Reporters.Windows;

    [UseReporter(typeof(WinMergeReporter))]
    [TestClass]
    public class CharacterTest
    {
        private Marvel _marvel { get; set; }
        private string _marvelPublicKey { get; set; }
        private string _marvelPrivateKey { get; set; }
        private const int TOTAL_CHARACTERS = 1402;
        private readonly CompareInfo _comparer;

        public CharacterTest()
        {
            _marvelPublicKey = "67d146c4c462f0b55bf12bb7d60948af";
            _marvelPrivateKey = "54fd1a8ac788767cc91938bcb96755186074970b";

            _marvel = new Marvel(_marvelPublicKey, _marvelPrivateKey);
            _comparer = CompareInfo.GetCompareInfo("en-US");
        }

        #region GetCharacters
        [TestMethod]
        public void GetCharactersTest()
        {
            // Arrange

            // Act
            var characters = _marvel.GetCharacters();


            string characterText = JsonConvert.SerializeObject(characters);
            // Assert
            Assert.IsInstanceOfType(characters, typeof(IEnumerable<Character>));
            Assert.AreEqual(characters.Count(), 20);
        }

        [TestMethod]
        public void GetCharactersByNameTest()
        {
            // Arrange
            var name = "Deadpool";

            // Act
            var characters = _marvel.GetCharacters(name: name);

            // Assert
            Assert.IsInstanceOfType(characters, typeof(IEnumerable<Character>));
            Assert.IsTrue(characters.All(character => character.Name.Contains(name)));
            Approvals.VerifyAll(characters.Select(character => JsonConvert.SerializeObject(character)), "Character");
        }

        //[TestMethod]
        //public void GetCharactersByComicsTest()
        //{
        //    // Arrange
        //    var comics = new List<int> {35297, 38028, 36153};

        //    // Act
        //    var characters = _Marvel.GetCharacters(Comics: comics);

        //    // Assert
        //    Assert.IsInstanceOfType(characters, typeof(IEnumerable<Character>));
        //    Approvals.VerifyAll(characters.Select(character => JsonConvert.SerializeObject(character)), "Character");
        //}

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentException))]
        //public void GetCharacters_TooManyValuesInComicMultiListError()
        //{
        //    // Arrange
        //    var comics = new List<int> { 43331, 45821, 46713, 46394, 42337, 43339, 41278, 43197, 37360, 41247, 42339 };

        //    // Act
        //    var characters = _Marvel.GetCharacters(Comics: comics);

        //    // Assert
        //    Assert.Fail("Exception Should Be Caught.");
        //}

        //[TestMethod]
        //public void GetCharactersBySeriesTest()
        //{
        //    // Arrange
        //    var series = new List<int> { 15276, 12429 };

        //    // Act
        //    var characters = _Marvel.GetCharacters(Series: series);

        //    // Assert
        //    Assert.IsInstanceOfType(characters, typeof(IEnumerable<Character>));
        //    Approvals.VerifyAll(characters.Select(character => JsonConvert.SerializeObject(character)), "Character");
        //}

        //[TestMethod]
        //public void GetCharactersByEventsTest()
        //{
        //    // Arrange
        //    var events = new List<int> { 227, 238, 318 };

        //    // Act
        //    var characters = _Marvel.GetCharacters(Events: events);

        //    // Assert
        //    Assert.IsInstanceOfType(characters, typeof(IEnumerable<Character>));
        //    Approvals.VerifyAll(characters.Select(character => JsonConvert.SerializeObject(character)), "Character");
        //}

        //[TestMethod]
        //public void GetCharactersByStoriesTest()
        //{
        //    // Arrange
        //    var stories = new List<int> { 2464, 2484, 2489 };

        //    // Act
        //    var characters = _Marvel.GetCharacters(Stories: stories);

        //    // Assert
        //    Assert.IsInstanceOfType(characters, typeof(IEnumerable<Character>));
        //    Approvals.VerifyAll(characters.Select(character => JsonConvert.SerializeObject(character)), "Character");
        //}

        [TestMethod]
        public void GetCharactersByDateModifiedTest()
        {
            // Arrange
            var dateModified = new DateTime(2000, 1, 1);

            // Act
            var characters = _marvel.GetCharacters(modifiedSince: dateModified);

            // Assert
            Assert.IsInstanceOfType(characters, typeof(IEnumerable<Character>));
            Assert.IsTrue(characters.All(character => character.Modified >= dateModified));
        }

        [TestMethod]
        public void GetCharactersOrderByName()
        {
            // Arrange

            // Act
            var characters = _marvel.GetCharacters(order: new List<OrderBy> { OrderBy.Name });

            // Assert
            Assert.IsInstanceOfType(characters, typeof(IEnumerable<Character>));
            var inOrder = true;
            var character = characters.FirstOrDefault();
            foreach (var nextCharacter in characters.Skip(1))
            {
                if (_comparer.Compare(character.Name, nextCharacter.Name, CompareOptions.StringSort) < 0)
                {
                    character = nextCharacter;
                }
                else
                {
                    inOrder = false;
                    break;
                }
            }
            Assert.IsTrue(inOrder, "Characters are out of name order.");
        }

        [TestMethod]
        public void GetCharactersOrderByNameDescending()
        {
            // Arrange

            // Act
            var characters = _marvel.GetCharacters(order: new List<OrderBy> { OrderBy.NameDesc });

            // Assert
            Assert.IsInstanceOfType(characters, typeof(IEnumerable<Character>));
            var inOrder = true;
            var character = characters.FirstOrDefault();
            foreach (var nextCharacter in characters.Skip(1))
            {
                if (_comparer.Compare(character.Name, nextCharacter.Name, CompareOptions.StringSort) > 0)
                {
                    character = nextCharacter;
                }
                else
                {
                    inOrder = false;
                    break;
                }
            }
            Assert.IsTrue(inOrder, "Characters are out of name descending order.");
        }

        [TestMethod]
        public void GetCharactersOrderByModified()
        {
            // Arrange

            // Act
            var characters = _marvel.GetCharacters(order: new List<OrderBy> { OrderBy.Modified });

            // Assert
            Assert.IsInstanceOfType(characters, typeof(IEnumerable<Character>));
            var inOrder = true;
            var character = characters.FirstOrDefault();
            foreach (var nextCharacter in characters.Skip(1))
            {
                if (character.Modified <= nextCharacter.Modified)
                {
                    character = nextCharacter;
                }
                else
                {
                    inOrder = false;
                    break;
                }
            }
            Assert.IsTrue(inOrder, "Characters are out of modified order.");
        }

        [TestMethod]
        public void GetCharactersOrderByModifiedDescending()
        {
            // Arrange

            // Act
            var characters = _marvel.GetCharacters(order: new List<OrderBy> { OrderBy.ModifiedDesc });

            // Assert
            Assert.IsInstanceOfType(characters, typeof(IEnumerable<Character>));
            var inOrder = true;
            var character = characters.FirstOrDefault();
            foreach (var nextCharacter in characters.Skip(1))
            {
                if (character.Modified >= nextCharacter.Modified)
                {
                    character = nextCharacter;
                }
                else
                {
                    inOrder = false;
                    break;
                }
            }
            Assert.IsTrue(inOrder, "Characters are out of modified descending order.");
        }

        [TestMethod]
        public void GetCharactersWithLimitTest()
        {
            // Arrange
            var limit = 50;

            // Act
            var characters = _marvel.GetCharacters(limit: limit);

            // Assert
            Assert.IsInstanceOfType(characters, typeof(IEnumerable<Character>));
            Assert.AreEqual(limit, characters.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetCharacters_Where_LimitGreaterThan100_ThrowArgumentException()
        {
            // Arrange

            // Act
            var characters = _marvel.GetCharacters(limit: 101);

            // Assert
            Assert.Fail("Exception Should Be Caught.");
        }

        [TestMethod]
        public void GetCharacters_Where_LimitLessThan1_DoNotAddLimitParameter()
        {
            // Arrange

            // Act
            var characters = _marvel.GetCharacters(limit: 0);

            // Assert
            Assert.IsInstanceOfType(characters, typeof(IEnumerable<Character>));
            Assert.AreEqual(characters.Count(), 20);
        }

        [TestMethod]
        public void GetCharactersWithOffsetTest()
        {
            // Arrange
            var offset = 20;

            // Act
            var firstCharacters = _marvel.GetCharacters(limit: 20);
            var firstListLastCharacterName = firstCharacters.Last().Name;
            var secondCharacters = _marvel.GetCharacters(offset: offset);
            var secondListFirstCharacterName = secondCharacters.FirstOrDefault().Name;
            var exempt = secondCharacters.Except(firstCharacters);

            // Assert
            Assert.IsInstanceOfType(firstCharacters, typeof(IEnumerable<Character>));
            Assert.IsInstanceOfType(secondCharacters, typeof(IEnumerable<Character>));
            Assert.IsTrue(exempt.Count() == 20);
            Assert.IsTrue(string.CompareOrdinal(firstListLastCharacterName, secondListFirstCharacterName) < 0);
        }

        [TestMethod]
        public void GetCharacters_Where_OffsetGreaterThanTotalCharacters_ReturnNoResults()
        {
            // Arrange

            // Act
            var characters = _marvel.GetCharacters(offset: TOTAL_CHARACTERS + 1);

            // Assert
            Assert.IsInstanceOfType(characters, typeof(IEnumerable<Character>));
            Assert.AreEqual(characters.Count(), 0);
        }

        [TestMethod]
        public void GetCharacters_Where_OffsetLessThan1_DoNotAddOfsetParameter()
        {
            // Arrange

            // Act
            var characters = _marvel.GetCharacters(offset: 0);

            // Assert
            Assert.IsInstanceOfType(characters, typeof(IEnumerable<Character>));
            Assert.AreEqual(characters.Count(), 20);
        }

        #endregion

        #region GetCharacter
        [TestMethod]
        public void GetCharacterTest()
        {
            // Arrange
            var characterId = 1009268;

            // Act
            var character = _marvel.GetCharacter(characterId);

            // Assert
            Approvals.Verify(JsonConvert.SerializeObject(character));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetCharacter_InvalidCharacterId()
        {
            // Arrange
            var invalidCharacterId = 0;

            // Act
            var character = _marvel.GetCharacter(invalidCharacterId);

            // Assert
            Assert.Fail("Exception Should Be Caught.");
        }

        #endregion

        #region GetComicsForCharacter
        [TestMethod]
        public void GetComicsForCharacterTest()
        {
            // Arrange
            var characterId = 1009268;

            // Act
            var comics = _marvel.GetComicsForCharacter(characterId);

            // Assert
            Assert.IsInstanceOfType(comics, typeof(IEnumerable<Comic>));
            Approvals.VerifyAll(comics.Select(comic => JsonConvert.SerializeObject(comic)), "Comics");
        }

        [TestMethod]
        public void GetComicsForCharacterByComicFormatTest()
        {
            // Arrange
            var characterId = 1009268;
            var comicFormat = ComicFormat.GraphicNovel;

            // Act
            var comics = _marvel.GetComicsForCharacter(characterId, format: comicFormat);

            // Assert
            Assert.IsInstanceOfType(comics, typeof(IEnumerable<Comic>));
            Assert.IsTrue(comics.All(comic => comic.Format.ToLower().Equals(comicFormat.ToParameter())));
        }

        [TestMethod]
        public void GetComicsForCharacterByComicFormatType_Comic_Test()
        {
            // Arrange
            var characterId = 1009268;
            var comicFormatType = ComicFormatType.Comic;

            // Act
            var comics = _marvel.GetComicsForCharacter(characterId, formatType: comicFormatType);

            // Assert
            Assert.IsInstanceOfType(comics, typeof(IEnumerable<Comic>));
            Assert.IsTrue(
                comics.All(
                    comic => comic.Format.ToLower().Equals(ComicFormat.Comic.ToParameter())
                        || comic.Format.ToLower().Equals(ComicFormat.Digest.ToParameter())
                        || comic.Format.ToLower().Equals(ComicFormat.DigitalComic.ToParameter())
                        || comic.Format.ToLower().Equals(ComicFormat.InfiniteComic.ToParameter())
                        || comic.Format.ToLower().Equals(ComicFormat.Magazine.ToParameter())
                        ));
        }

        [TestMethod]
        public void GetComicsForCharacterByComicFormatType_Collection_Test()
        {
            // Arrange
            var characterId = 1009268;
            var comicFormatType = ComicFormatType.Collection;

            // Act
            var comics = _marvel.GetComicsForCharacter(characterId, formatType: comicFormatType);

            // Assert
            Assert.IsInstanceOfType(comics, typeof(IEnumerable<Comic>));
            Assert.IsTrue(
                comics.All(
                    comic => comic.Format.ToLower().Equals(ComicFormat.GraphicNovel.ToParameter())
                        || comic.Format.ToLower().Equals(ComicFormat.Hardcover.ToParameter())
                        || comic.Format.ToLower().Equals(ComicFormat.TradePaperback.ToParameter())));
        }

        [TestMethod]
        public void GetComicsForCharacterNoVariantsTest()
        {
            // Arrange
            var characterId = 1009268;

            // Act
            var comics = _marvel.GetComicsForCharacter(characterId, noVariants: true);

            // Assert
            Assert.IsInstanceOfType(comics, typeof(IEnumerable<Comic>));
            Assert.IsTrue(comics.All(comic => string.IsNullOrWhiteSpace(comic.VariantDescription)));
        }

        /*  GetComicsForCharacterByDateDescriptor 
         * ^ Can't seem to find comics using the DateDescriptor field with characters
        */

        [TestMethod]
        public void GetComicsForCharacterByDateRange()
        {
            // Arrange
            var characterId = 1009268;
            var dateBeginning = new DateTime(2000, 1, 1);
            var dateEnding = new DateTime(2001, 1, 1);

            // Act
            var comics = _marvel.GetComicsForCharacter(characterId, dateRangeBegin: dateBeginning, dateRangeEnd: dateEnding);

            // Assert
            Assert.IsInstanceOfType(comics, typeof(IEnumerable<Comic>));
            Assert.IsTrue(comics.All(comic =>
                comic.Dates.All(date =>
                    date.Type == "onsaleDate" && date.Date >= dateBeginning && date.Date <= dateEnding)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetComicsForCharacterByDateRange_DateEndingBeforeDateBeginning()
        {
            // Arrange
            var characterId = 1009268;
            var dateBeginning = new DateTime(2001, 1, 1);
            var dateEnding = new DateTime(2000, 1, 1);

            // Act
            var comics = _marvel.GetComicsForCharacter(characterId, dateRangeBegin: dateBeginning, dateRangeEnd: dateEnding);

            // Assert
            Assert.Fail("Exception Should Be Caught.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetComicsForCharacterByDateRange_DateEndingMissing()
        {
            // Arrange
            var characterId = 1009268;
            var dateBeginning = new DateTime(2001, 1, 1);

            // Act
            var comics = _marvel.GetComicsForCharacter(characterId, dateRangeBegin: dateBeginning);

            // Assert
            Assert.Fail("Exception Should Be Caught.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetComicsForCharacterByDateRange_DateBeginningMissing()
        {
            // Arrange
            var characterId = 1009268;
            var dateEnding = new DateTime(2000, 1, 1);

            // Act
            var comics = _marvel.GetComicsForCharacter(characterId, dateRangeEnd: dateEnding);

            // Assert
            Assert.Fail("Exception Should Be Caught.");
        }

        [TestMethod]
        public void GetComicsForCharacterHasDigitalIssue()
        {
            // Arrange
            var characterId = 1009268;
            var hasDigitalIssue = true;

            // Act
            var comics = _marvel.GetComicsForCharacter(characterId, hasDigitalIssue: hasDigitalIssue);

            // Assert
            Assert.IsInstanceOfType(comics, typeof(IEnumerable<Comic>));
            Assert.IsTrue(comics.All(comic => comic.DigitalId > 0));
        }

        [TestMethod]
        public void GetComicsForCharacterByModifiedSinceTest()
        {
            // Arrange
            var characterId = 1009268;
            var modifiedDate = new DateTime(2000, 1, 1);

            // Act
            var comics = _marvel.GetComicsForCharacter(characterId, modifiedSince: modifiedDate);

            // Assert
            Assert.IsInstanceOfType(comics, typeof(IEnumerable<Comic>));
            Assert.IsTrue(comics.All(comic => comic.Modified >= modifiedDate));
        }

        // GetComicsForCharacterByCreators

        // GetComicsForCharacterBySeries

        // GetComicsForCharacterByEvents

        // GetComicsForCharacterByStories

        // GetComicsForCharacterBySharedAppearences

        // GetComicsForCharacterByCollaborators

        // Not ordered properly
        //[TestMethod]
        //public void GetComicsForCharacterOrderByFocDateTest()
        //{
        //    // Arrange
        //    var characterId = 1009268;

        //    // Act
        //    var comics = _Marvel.GetComicsForCharacter(characterId, Order: new List<OrderBy> { OrderBy.FocDate });

        //    // Assert
        //    Assert.IsInstanceOfType(comics, typeof(IEnumerable<Comic>));
        //    var inOrder = true;
        //    var comic = comics.FirstOrDefault();
        //    foreach (var nextComic in comics.Skip(1))
        //    {
        //        var currentDate = comic.Dates.FirstOrDefault(date => date.Type.Equals("focDate"));
        //        var nextDate = nextComic.Dates.FirstOrDefault(date => date.Type.Equals("focDate"));
        //        if (currentDate != null && nextDate != null && currentDate.Date >= nextDate.Date)
        //        {
        //            comic = nextComic;
        //        }
        //        else
        //        {
        //            inOrder = false;
        //            break;
        //        }
        //    }
        //    Assert.IsTrue(inOrder, "Comics are out of foc date order.");
        //}

        [TestMethod]
        public void GetComicsForCharacterLimitTest()
        {
            // Arrange
            var characterId = 1009268;
            var limit = 50;

            // Act
            var comics = _marvel.GetComicsForCharacter(characterId, limit: limit);

            // Assert
            Assert.IsInstanceOfType(comics, typeof(IEnumerable<Comic>));
            Assert.IsTrue(comics.Count() == limit);
        }

        [TestMethod]
        public void GetComicsForCharacterOffsetTest()
        {
            // Arrange
            var characterId = 1009268;
            var offset = 20;

            // Act
            var firstComics = _marvel.GetComicsForCharacter(characterId, limit: offset);
            var firstListLastComicTitle = firstComics.Last().Title;
            var secondComics = _marvel.GetComicsForCharacter(characterId, offset: offset);
            var secondListFirstComicTitle = secondComics.FirstOrDefault().Title;
            var exempt = secondComics.Except(firstComics);

            // Assert
            Assert.IsInstanceOfType(firstComics, typeof(IEnumerable<Comic>));
            Assert.IsInstanceOfType(secondComics, typeof(IEnumerable<Comic>));
            Assert.IsTrue(exempt.Count() == 20);
            Assert.IsTrue(string.CompareOrdinal(firstListLastComicTitle, secondListFirstComicTitle) < 0);
        }
        #endregion
    }
}
