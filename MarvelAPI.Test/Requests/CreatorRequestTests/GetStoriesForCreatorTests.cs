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
    public class GetStoriesForCreatorTests : CreatorRequestTestBase
    {
        [TestMethod]
        public void Success()
        {
            // arrange
            var creatorId = 1;
            var storyList = new List<Story>
            {
                new Story { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Story>>(It.Is<IRestRequest>(r => r.Resource == $"/creators/{creatorId}/stories")))
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
            var comics = Requests.GetStoriesForCreator(new GetStoriesForCreator
            {
                CreatorId = creatorId
            });

            // assert
            Assert.AreEqual(storyList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }
    }
}
