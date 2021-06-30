using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestSharp;
using System.Collections.Generic;

namespace MarvelAPI.Test.Requests.EventRequestTests
{
    [TestClass]
    public class GetEventTests : EventRequestTestBase
    {
        [TestMethod]
        public void Success()
        {
            // arrange
            var eventId = 1;
            var data = new Wrapper<Event>
            {
                Data = new Container<Event>
                {
                    Results = new List<Event>
                    {
                        new Event
                        {

                        }
                    }
                }
            };
            RestClientMock.Setup(s => s.Execute<Wrapper<Event>>(It.Is<IRestRequest>(r => r.Resource == $"/events/{eventId}")))
                .Returns(new RestResponse<Wrapper<Event>>
                {
                    Data = data
                }).Verifiable();

            // act
            var result = Request.GetEvent(eventId);

            // assert
            RestClientMock.VerifyAll();
        }
    }
}
