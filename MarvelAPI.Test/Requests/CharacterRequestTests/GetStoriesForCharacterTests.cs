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

namespace MarvelAPI.Test.Requests.CharacterRequestTests
{
    [TestClass]
    public class GetStoriesForCharacterTests : CharacterRequestTestBase
    {
        [TestMethod]
        public void Success()
        {
            // arrange
            var characterId = 1;
            var storyList = new List<Story>
            {
                new Story { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Story>>(It.Is<IRestRequest>(r => r.Resource == $"/characters/{characterId}/stories")))
                .Returns(new RestResponse<Wrapper<Story>>
                {
                    Data = new Wrapper<Story>
                    {
                        Data = new Container<Story>
                        {
                            Results = storyList
                        }
                    }
                })
                .Verifiable();

            // act
            var comics = Requests.GetStoriesForCharacter(new GetStoriesForCharacter
            {
                CharacterId = characterId
            });

            // assert
            Assert.AreEqual(storyList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }
    }
}
