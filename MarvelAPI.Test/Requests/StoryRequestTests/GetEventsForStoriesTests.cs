using MarvelAPI.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MSTestExtensions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI.Test.Requests.StoryRequestTests
{
    [TestClass]
    public class GetEventsForStoriesTests : StoryRequestTestBase
    {
        [TestMethod]
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
            Assert.AreEqual(eventList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }
    }
}
