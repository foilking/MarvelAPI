using MarvelAPI.Parameters;
using Moq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MarvelAPI.Test.Requests.SeriesRequestTests
{
    public class GetStoriesForSeriesTests : SeriesRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // arrange
            var seriesId = 1;
            var storyList = new List<Story>
            {
                new Story { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Story>>(It.Is<IRestRequest>(r => r.Resource == $"/series/{seriesId}/stories")))
                .Returns(new RestResponse<Wrapper<Story>>
                {
                    Data = new Wrapper<Story>
                    {
                        Data = new Container<Story>
                        {
                            Results = storyList
                        }
                    }
                })
                .Verifiable();

            // act
            var results = Request.GetStoriesForSeries(new GetStoriesForSeries
            {
                SeriesId = seriesId
            });

            // assert
            Assert.Equal(storyList.Count, results.Count());
            RestClientMock.VerifyAll();
        }
    }
}
