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

namespace MarvelAPI.Test.Requests.ComicsRequestTests
{
    [TestClass]
    public class GetEventsForComicTests : ComicRequestTestBase
    {
        [TestMethod]
        public void Success()
        {
            // arrange
            var comicId = 1;
            var eventList = new List<Event>
            {
                new Event { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Event>>(It.Is<IRestRequest>(r => r.Resource == $"/comics/{comicId}/events")))
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
            var comics = Requests.GetEventsForComic(new GetEventsForComic
            {
                ComicId = comicId
            });

            // assert
            Assert.AreEqual(eventList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }
    }
}
