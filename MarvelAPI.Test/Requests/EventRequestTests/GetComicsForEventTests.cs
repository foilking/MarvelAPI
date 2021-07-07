using MarvelAPI.Parameters;
using Moq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MarvelAPI.Test.Requests.EventRequestTests
{
    public class GetComicsForEventTests : EventRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // arrange
            var eventId = 1;
            var comicList = new List<Comic>
            {
                new Comic { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Comic>>(It.Is<IRestRequest>(r => r.Resource == $"/events/{eventId}/comics")))
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
            var results = Request.GetComicsForEvent(new GetComicsForEvent
            {
                EventId = eventId
            });
            // assert
            Assert.Equal(comicList.Count, results.Count());
            RestClientMock.VerifyAll();
        }
    }
}
