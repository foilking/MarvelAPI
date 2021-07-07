using MarvelAPI.Parameters;
using Moq;
using RestSharp;
using System.Collections.Generic;
using Xunit;

namespace MarvelAPI.Test.Requests.EventRequestTests
{
    public class GetEventsTests : EventRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // arrange
            var data = new Wrapper<Event>
            {
                Data = new Container<Event>
                {
                    Results = new List<Event>
                    {
                        new Event
                        {

                        }
                    }
                }
            };
            RestClientMock.Setup(s => s.Execute<Wrapper<Event>>(It.Is<IRestRequest>(r => r.Resource == "/events")))
                .Returns(new RestResponse<Wrapper<Event>>
                {
                    Data = data
                }).Verifiable();

            // act
            var result = Request.GetEvents(new GetEvents
            {

            });

            // assert
            RestClientMock.VerifyAll();
        }
    }
}
