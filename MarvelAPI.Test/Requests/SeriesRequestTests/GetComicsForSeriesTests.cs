using MarvelAPI.Parameters;
using Moq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MarvelAPI.Test.Requests.SeriesRequestTests
{
    public class GetComicsForSeriesTests : SeriesRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // arrange
            var seriesId = 1;
            var comicList = new List<Comic>
            {
                new Comic { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Comic>>(It.Is<IRestRequest>(r => r.Resource == $"/series/{seriesId}/comics")))
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
            var results = Request.GetComicsForSeries(new GetComicsForSeries
            {
                SeriesId = seriesId
            });

            // assert
            Assert.Equal(comicList.Count, results.Count());
            RestClientMock.VerifyAll();
        }
    }
}
