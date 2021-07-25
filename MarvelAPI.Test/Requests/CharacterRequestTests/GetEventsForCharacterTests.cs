using MarvelAPI.Parameters;
using Moq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MarvelAPI.Test.Requests.CharacterRequestTests
{
    public class GetEventsForCharacterTests : CharacterRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // arrange
            var characterId = 1;
            var eventList = new List<Event>
            {
                new Event { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Event>>(It.Is<IRestRequest>(r => r.Resource == $"/characters/{characterId}/events")))
                .Returns(new RestResponse<Wrapper<Event>>
                {
                    Data = new Wrapper<Event>
                    {
                        Data = new Container<Event>
                        {
                            Results = eventList
                        }
                    }
                })
                .Verifiable();

            // act
            var comics = Requests.GetEventsForCharacter(new GetEventsForCharacter
            {
                CharacterId = characterId
            });

            // assert
            Assert.Equal(eventList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }
    }
}
