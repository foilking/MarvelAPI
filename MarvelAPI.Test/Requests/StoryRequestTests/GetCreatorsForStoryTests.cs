using MarvelAPI.Parameters;
using Moq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MarvelAPI.Test.Requests.StoryRequestTests
{
    public class GetCreatorsForStoryTests : StoryRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // arrange
            var storyId = 1;
            var creatorList = new List<Creator>
            {
                new Creator
                {
                }
            };

            RestClientMock.Setup(c => c.Execute<Wrapper<Creator>>(It.Is<IRestRequest>(r => r.Resource == $"/stories/{storyId}/creators")))
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
            var results = Request.GetCreatorsForStory(new GetCreatorsForStory
            {
                StoryId = storyId
            });

            // assert
            Assert.Equal(creatorList.Count, results.Count());
            RestClientMock.VerifyAll();
        }
    }
}
