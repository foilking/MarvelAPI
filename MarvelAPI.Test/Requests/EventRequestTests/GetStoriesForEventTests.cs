using MarvelAPI.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MSTestExtensions;
using RestSharp;
using System.Collections.Generic;
using System.Linq;

namespace MarvelAPI.Test.Requests.EventRequestTests
{
    [TestClass]
    public class GetStoriesForEventTests : EventRequestTestBase
    {
        [TestMethod]
        public void Success()
        {
            // arrange
            var eventId = 1;
            var storyList = new List<Story>
            {
                new Story { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Story>>(It.Is<IRestRequest>(r => r.Resource == $"/events/{eventId}/stories")))
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
            var results = Request.GetStoriesForEvent(new GetStoriesForEvent
            {
                EventId = eventId
            });

            // assert
            Assert.AreEqual(storyList.Count, results.Count());
            RestClientMock.VerifyAll();
        }
    }
}
