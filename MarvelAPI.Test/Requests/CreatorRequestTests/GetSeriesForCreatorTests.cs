using MarvelAPI.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MSTestExtensions;
using RestSharp;
using System.Collections.Generic;
using System.Linq;

namespace MarvelAPI.Test.Requests.CreatorRequestTests
{
    [TestClass]
    public class GetSeriesForCreatorTests : CreatorRequestTestBase
    {
        [TestMethod]
        public void Success()
        {
            // arrange
            var creatorId = 1;
            var seriesList = new List<Series>
            {
                new Series { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Series>>(It.Is<IRestRequest>(r => r.Resource == $"/creators/{creatorId}/series")))
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
            var comics = Requests.GetSeriesForCreator(new GetSeriesForCreator
            {
                CreatorId = creatorId
            });

            // assert
            Assert.AreEqual(seriesList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }
    }
}
