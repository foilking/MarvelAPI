using MarvelAPI.Parameters;
using Moq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MarvelAPI.Test.Requests.ComicsRequestTests
{
    public class GetCreatorsForComicTests : ComicRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // arrange
            var comicId = 1;
            var creatorList = new List<Creator>
            {
                new Creator
                {
                }
            };

            RestClientMock.Setup(c => c.Execute<Wrapper<Creator>>(It.Is<IRestRequest>(r => r.Resource == $"/comics/{comicId}/creators")))
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
            var results = Requests.GetCreatorsForComic(new GetCreatorsForComic
            {
                ComicId = comicId
            });

            // assert
            Assert.Equal(creatorList.Count, results.Count());
            RestClientMock.VerifyAll();
        }
    }
}
