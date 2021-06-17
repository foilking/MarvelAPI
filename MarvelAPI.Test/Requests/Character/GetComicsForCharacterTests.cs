using MarvelAPI.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI.Test.Requests
{
    [TestClass]
    public class GetComicsForCharacterTests : CharacterRequestTestBase
    {
        [TestMethod]
        public void Success()
        {
            // arrange
            var characterId = 1;
            var comicList = new List<Comic>
            {
                new Comic { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Comic>>(It.Is<IRestRequest>(r => r.Resource == string.Format($"/characters/{characterId}/comics"))))
                .Returns(new RestResponse<Wrapper<Comic>>
                {
                    Data = new Wrapper<Comic>
                    {
                        Data = new Container<Comic>
                        {
                            Results = comicList
                        }
                    }
                })
                .Verifiable();

            // act
            var comics = Requests.GetComicsForCharacter(new GetComicsForCharacter
            {
                CharacterId = characterId
            });

            // assert
            Assert.AreEqual(comicList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }

        [TestMethod]
        public void Format_Filter()
        {
            // arrange
            var characterId = 1;
            var format = ComicFormat.Comic;
            var comicList = new List<Comic>
            {
                new Comic { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Comic>>(
                It.Is<IRestRequest>(r => 
                r.Resource == string.Format($"/characters/{characterId}/comics")
                && r.Parameters.Any(p => p.Name == "format" && p.Value.ToString() == format.ToParameter()))))
                .Returns(new RestResponse<Wrapper<Comic>>
                {
                    Data = new Wrapper<Comic>
                    {
                        Data = new Container<Comic>
                        {
                            Results = comicList
                        }
                    }
                })
                .Verifiable();

            // act
            var comics = Requests.GetComicsForCharacter(new GetComicsForCharacter
            {
                CharacterId = characterId,
                Format = format
            });

            // assert
            Assert.AreEqual(comicList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }
    }
}
