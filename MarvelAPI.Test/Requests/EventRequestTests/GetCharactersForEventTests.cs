using MarvelAPI.Parameters;
using Moq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MarvelAPI.Test.Requests.EventRequestTests
{
    public class GetCharactersForEventTests : EventRequestTestBase
    {
        [Fact]
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
            Assert.Equal(characterList.Count, results.Count());
            RestClientMock.VerifyAll();
        }
    }
}
