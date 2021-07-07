using MarvelAPI.Parameters;
using Moq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MarvelAPI.Test.Requests.CreatorRequestTests
{
    public class GetEventsForCreatorTests : CreatorRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // arrange
            var creatorId = 1;
            var eventList = new List<Event>
            {
                new Event { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Event>>(It.Is<IRestRequest>(r => r.Resource == $"/creators/{creatorId}/events")))
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
            var comics = Requests.GetEventsForCreator(new GetEventsForCreator
            {
                CreatorId = creatorId
            });

            // assert
            Assert.Equal(eventList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }
    }
}
