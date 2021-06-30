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
    public class GetCreatorsTests : CreatorRequestTestBase
    {
        [TestMethod]
        public void Success()
        {
            // arrange
            var creatorList = new List<Creator>
            {
                new Creator
                {
                }
            };

            RestClientMock.Setup(c => c.Execute<Wrapper<Creator>>(It.Is<IRestRequest>(r => r.Resource == "/creators")))
                .Returns(new RestResponse<Wrapper<Creator>>
                {
                    Data = new Wrapper<Creator>
                    {
                        Data = new Container<Creator>
                        {
                            Results = creatorList
                        }
                    }
                })
                .Verifiable();

            // act
            var comics = Requests.GetCreators(new GetCreators
            {

            });

            // assert
            Assert.AreEqual(creatorList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }
    }
}
