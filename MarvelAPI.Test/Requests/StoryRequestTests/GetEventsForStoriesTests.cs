using MarvelAPI.Parameters;
using Moq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MarvelAPI.Test.Requests.StoryRequestTests
{
    public class GetEventsForStoriesTests : StoryRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // arrange
            var storyId = 1;
            var eventList = new List<Event>
            {
                new Event { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Event>>(It.Is<IRestRequest>(r => r.Resource == $"/stories/{storyId}/events")))
                .Returns(new RestResponse<Wrapper<Event>>
                {
                    Data = new Wrapper<Event>
                    {
                        Data = new Container<Event>
                        {
                            Results = eventList
                        }
                    }
                })
                .Verifiable();

            // act
            var comics = Request.GetEventsForStory(new GetEventsForStory
            {
                StoryId = storyId
            });

            // assert
            Assert.Equal(eventList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }
    }
}
