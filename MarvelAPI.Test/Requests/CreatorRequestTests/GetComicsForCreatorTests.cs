using MarvelAPI.Parameters;
using Moq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MarvelAPI.Test.Requests.CreatorRequestTests
{
    public class GetComicsForCreatorTests : CreatorRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // arrange
            var creatorId = 1;
            var comicList = new List<Comic>
            {
                new Comic { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Comic>>(It.Is<IRestRequest>(r => r.Resource == $"/creators/{creatorId}/comics")))
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
            var comics = Requests.GetComicsForCreator(new GetComicsForCreator
            {
                CreatorId = creatorId
            });

            // assert
            Assert.Equal(comicList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }
    }
}
