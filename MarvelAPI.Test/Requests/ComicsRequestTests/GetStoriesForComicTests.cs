using MarvelAPI.Parameters;
using Moq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MarvelAPI.Test.Requests.ComicsRequestTests
{
    public class GetStoriesForComicTests : ComicRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // arrange
            var comicId = 1;
            var storyList = new List<Story>
            {
                new Story { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Story>>(It.Is<IRestRequest>(r => r.Resource == $"/comics/{comicId}/stories")))
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
            var results = Requests.GetStoriesForComic(new GetStoriesForComic
            {
                ComicId = comicId
            });

            // assert
            Assert.Equal(storyList.Count, results.Count());
            RestClientMock.VerifyAll();
        }
    }
}
