using MarvelAPI.Parameters;
using Moq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MarvelAPI.Test.Requests.ComicsRequestTests
{
    public class GetComicsTests : ComicRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // arrange
            var comicList = new List<Comic>
            {
                new Comic
                {
                }
            };

            RestClientMock.Setup(c => c.Execute<Wrapper<Comic>>(It.Is<IRestRequest>(r => r.Resource == "/comics")))
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
            var comics = Requests.GetComics(new GetComics
            {

            });

            // assert
            Assert.Equal(comicList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }
    }
}
