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

namespace MarvelAPI.Test.Requests.StoryRequestTests
{
    [TestClass]
    public class GetSeriesForStoryTests : StoryRequestTestBase
    {
        [TestMethod]
        public void Success()
        {
            // arrange
            var storyId = 1;
            var seriesList = new List<Series>
            {
                new Series { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Series>>(It.Is<IRestRequest>(r => r.Resource == $"/stories/{storyId}/series")))
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
            var series = Request.GetSeriesForStory(new GetSeriesForStory
            {
                StoryId = storyId
            });

            // assert
            Assert.AreEqual(seriesList.Count, series.Count());
            RestClientMock.VerifyAll();
        }
    }
}
