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
    public class GetEventsForCreatorTests : CreatorRequestTestBase
    {
        [TestMethod]
        public void Success()
        {
            // arrange
            var creatorId = 1;
            var eventList = new List<Event>
            {
                new Event { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Event>>(It.Is<IRestRequest>(r => r.Resource == $"/creators/{creatorId}/events")))
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
            var comics = Requests.GetEventsForCreator(new GetEventsForCreator
            {
                CreatorId = creatorId
            });

            // assert
            Assert.AreEqual(eventList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }
    }
}
