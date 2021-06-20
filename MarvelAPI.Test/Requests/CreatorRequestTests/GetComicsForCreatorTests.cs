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
    public class GetComicsForCreatorTests : CreatorRequestTestBase
    {
        [TestMethod]
        public void Success()
        {
            // arrange
            var creatorId = 1;
            var comicList = new List<Comic>
            {
                new Comic { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Comic>>(It.Is<IRestRequest>(r => r.Resource == $"/creators/{creatorId}/comics")))
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
            var comics = Requests.GetComicsForCreator(new GetComicsForCreator
            {
                CreatorId = creatorId
            });

            // assert
            Assert.AreEqual(comicList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }
    }
}
