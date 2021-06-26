using MarvelAPI.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MSTestExtensions;
using RestSharp;
using System.Collections.Generic;
using System.Linq;

namespace MarvelAPI.Test.Requests.StoryRequestTests
{
    [TestClass]
    public class GetStoriesTests : StoryRequestTestBase
    {
        [TestMethod]
        public void Success()
        {
            // arrange
            var data = new Wrapper<Story>
            {
                Data = new Container<Story>
                {
                    Results = new List<Story>
                    {
                        new Story
                        {

                        }
                    }
                }
            };
            RestClientMock.Setup(s => s.Execute<Wrapper<Story>>(It.Is<IRestRequest>(r => r.Resource == "/stories")))
                .Returns(new RestResponse<Wrapper<Story>>
                {
                    Data = data
                }).Verifiable();

            // act
            var result = Request.GetStories(new GetStories
            {

            });

            // assert
            RestClientMock.VerifyAll();
        }
    }
}
