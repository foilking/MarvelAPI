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

namespace MarvelAPI.Test.Requests.EventRequestTests
{
    [TestClass]
    public class GetComicsForEventTests : EventRequestTestBase
    {
        [TestMethod]
        public void Success()
        {
            // arrange
            var eventId = 1;
            var comicList = new List<Comic>
            {
                new Comic { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Comic>>(It.Is<IRestRequest>(r => r.Resource == $"/events/{eventId}/comics")))
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
            var results = Request.GetComicsForEvent(new GetComicsForEvent
            {
                EventId = eventId
            });
            // assert
            Assert.AreEqual(comicList.Count, results.Count());
            RestClientMock.VerifyAll();
        }
    }
}
