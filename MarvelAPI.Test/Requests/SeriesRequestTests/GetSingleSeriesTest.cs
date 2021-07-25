using Moq;
using RestSharp;
using System.Collections.Generic;
using Xunit;

namespace MarvelAPI.Test.Requests.SeriesRequestTests
{
    public class GetSingleSeriesTest : SeriesRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // arrange
            var seriesId = 1;
            var data = new Wrapper<Series>
            {
                Data = new Container<Series>
                {
                    Results = new List<Series>
                    {
                        new Series
                        {

                        }
                    }
                }
            };
            RestClientMock.Setup(s => s.Execute<Wrapper<Series>>(It.Is<IRestRequest>(r => r.Resource == $"/series/{seriesId}")))
                .Returns(new RestResponse<Wrapper<Series>>
                {
                    Data = data
                }).Verifiable();

            // act
            var results = Request.GetSeries(seriesId);

            // assert
            RestClientMock.VerifyAll();
        }
    }
}
