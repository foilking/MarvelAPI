using MarvelAPI.Parameters;
using Moq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MarvelAPI.Test.Requests.ComicsRequestTests
{
    public class GetEventsForComicTests : ComicRequestTestBase
    {
        [Fact]
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
            Assert.Equal(eventList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }
    }
}
