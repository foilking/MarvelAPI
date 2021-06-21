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
    public class GetSeriesForEventTests : EventRequestTestBase
    {
        [TestMethod]
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
            Assert.AreEqual(seriesList.Count, series.Count());
            RestClientMock.VerifyAll();
        }
    }
}
