using MarvelAPI.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MSTestExtensions;
using RestSharp;
using System.Collections.Generic;
using System.Linq;

namespace MarvelAPI.Test.Requests.ComicsRequestTests
{
    [TestClass]
    public class GetComicsTests : ComicRequestTestBase
    {
        [TestMethod]
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
            Assert.AreEqual(comicList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }
    }
}
