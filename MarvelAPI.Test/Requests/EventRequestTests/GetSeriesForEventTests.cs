using MarvelAPI.Parameters;
using Moq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MarvelAPI.Test.Requests.EventRequestTests
{
    public class GetSeriesForEventTests : EventRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // arrange
            var eventId = 1;
            var seriesList = new List<Series>
            {
                new Series { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Series>>(It.Is<IRestRequest>(r => r.Resource == $"/events/{eventId}/series")))
                .Returns(new RestResponse<Wrapper<Series>>
                {
                    Data = new Wrapper<Series>
                    {
                        Data = new Container<Series>
                        {
                            Results = seriesList
                        }
                    }
                })
                .Verifiable();

            // act
            var series = Request.GetSeriesForEvent(new GetSeriesForEvent
            {
                EventId = eventId
            });

            // assert
            Assert.Equal(seriesList.Count, series.Count());
            RestClientMock.VerifyAll();
        }
    }
}
