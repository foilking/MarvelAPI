using MarvelAPI.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MSTestExtensions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI.Test.Requests.SeriesRequestTests
{
    [TestClass]
    public class GetStoriesForSeriesTests : SeriesRequestTestBase
    {
        [TestMethod]
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
            Assert.AreEqual(storyList.Count, results.Count());
            RestClientMock.VerifyAll();
        }
    }
}
