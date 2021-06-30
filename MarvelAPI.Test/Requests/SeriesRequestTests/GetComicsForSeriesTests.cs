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
    public class GetComicsForSeriesTests : SeriesRequestTestBase
    {
        [TestMethod]
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
            Assert.AreEqual(comicList.Count, results.Count());
            RestClientMock.VerifyAll();
        }
    }
}
