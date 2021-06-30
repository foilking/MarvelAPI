using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI.Test.Requests.ComicsRequestTests
{
    [TestClass]
    public class GetComicTests : ComicRequestTestBase
    {
        [TestMethod]
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
