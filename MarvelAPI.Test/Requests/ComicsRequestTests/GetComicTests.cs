using Moq;
using RestSharp;
using System.Collections.Generic;
using Xunit;

namespace MarvelAPI.Test.Requests.ComicsRequestTests
{
    public class GetComicTests : ComicRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // Arrange
            var comicId = 1;
            RestClientMock.Setup(c => c.Execute<Wrapper<Comic>>(It.Is<IRestRequest>(r => r.Resource == $"/comics/{comicId}")))
                .Returns(new RestResponse<Wrapper<Comic>>
                {
                    Data = new Wrapper<Comic>
                    {
                        Data = new Container<Comic>
                        {
                            Results = new List<Comic>
                            {
                                new Comic
                                {
                                    Id = comicId
                                }
                            }
                        }
                    }
                });

            // Act
            var result = Requests.GetComic(comicId);

            // Assert

            RestClientMock.VerifyAll();
        }
    }
}
