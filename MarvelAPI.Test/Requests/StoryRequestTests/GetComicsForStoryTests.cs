using MarvelAPI.Parameters;
using Moq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MarvelAPI.Test.Requests.StoryRequestTests
{
    public class GetComicsForStoryTests : StoryRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // arrange
            var storyId = 1;
            var comicList = new List<Comic>
            {
                new Comic { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Comic>>(It.Is<IRestRequest>(r => r.Resource == $"/stories/{storyId}/comics")))
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
            var results = Request.GetComicsForStory(new GetComicsForStory
            {
                StoryId = storyId
            });

            // assert
            Assert.Equal(comicList.Count, results.Count());
            RestClientMock.VerifyAll();
        }
    }
}
