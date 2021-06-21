using MarvelAPI.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MSTestExtensions;
using RestSharp;
using System.Collections.Generic;
using System.Linq;

namespace MarvelAPI.Test.Requests.EventRequestTests
{
    [TestClass]
    public class GetCharactersForEventTests : EventRequestTestBase
    {
        [TestMethod]
        public void Success()
        {
            // arrange
            var eventId = 1; 
            var characterList = new List<Character>
            {
                new Character
                {
                }
            };

            RestClientMock.Setup(c => c.Execute<Wrapper<Character>>(It.Is<IRestRequest>(r => r.Resource == $"/events/{eventId}/characters")))
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
            var results = Request.GetCharactersForEvent(new GetCharactersForEvent
            {
                EventId = eventId
            });

            // assert
            Assert.AreEqual(characterList.Count, results.Count());
            RestClientMock.VerifyAll();
        }
    }
}
