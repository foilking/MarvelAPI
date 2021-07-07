using Moq;
using RestSharp;
using System.Collections.Generic;
using Xunit;

namespace MarvelAPI.Test.Requests.StoryRequestTests
{
    public class GetStoryTests : StoryRequestTestBase
    {
        [Fact]
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
