using MarvelAPI.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MSTestExtensions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI.Test.Requests.ComicsRequestTests
{
    [TestClass]
    public class GetCharactersForComicTests : ComicRequestTestBase
    {
        [TestMethod]
        public void Success()
        {
            // arrange
            var comicId = 1;
            var characterList = new List<Character>
            {
                new Character
                {
                }
            };

            RestClientMock.Setup(c => c.Execute<Wrapper<Character>>(It.Is<IRestRequest>(r => r.Resource == $"/comics/{comicId}/characters")))
                .Returns(new RestResponse<Wrapper<Character>>
                {
                    Data = new Wrapper<Character>
                    {
                        Data = new Container<Character>
                        {
                            Results = characterList
                        }
                    }
                })
                .Verifiable();

            // act
            var characters = Requests.GetCharactersForComic(new GetCharactersForComic
            {
                ComicId = comicId
            });

            // assert
            Assert.AreEqual(characterList.Count, characters.Count());
            RestClientMock.VerifyAll();
        }
    }
}
