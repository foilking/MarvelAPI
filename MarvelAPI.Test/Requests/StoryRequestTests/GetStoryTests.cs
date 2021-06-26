using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI.Test.Requests.StoryRequestTests
{
    [TestClass]
    public class GetStoryTests : StoryRequestTestBase
    {
        [TestMethod]
        public void Success()
        {
            // arrange
            var storyId = 1;
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
            RestClientMock.Setup(s => s.Execute<Wrapper<Story>>(It.Is<IRestRequest>(r => r.Resource == $"/stories/{storyId}")))
                .Returns(new RestResponse<Wrapper<Story>>
                {
                    Data = data
                }).Verifiable();

            // act
            var result = Request.GetStory(storyId);

            // assert
            RestClientMock.VerifyAll();
        }
    }
}
