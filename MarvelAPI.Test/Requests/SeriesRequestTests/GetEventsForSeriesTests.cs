using MarvelAPI.Parameters;
using Moq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MarvelAPI.Test.Requests.SeriesRequestTests
{
    public class GetEventsForSeriesTests : SeriesRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // arrange
            var seriesId = 1;
            var eventList = new List<Event>
            {
                new Event { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Event>>(It.Is<IRestRequest>(r => r.Resource == $"/series/{seriesId}/events")))
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
            var comics = Request.GetEventsForSeries(new GetEventsForSeries
            {
                SeriesId = seriesId
            });

            // assert
            Assert.Equal(eventList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }
    }
}
