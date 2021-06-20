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
    public class GetStoriesForComicTests : ComicRequestTestBase
    {

        [TestMethod]
        public void Success()
        {
            // arrange
            var comicId = 1;
            var storyList = new List<Story>
            {
                new Story { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Story>>(It.Is<IRestRequest>(r => r.Resource == string.Format($"/comics/{comicId}/stories"))))
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
            Assert.AreEqual(storyList.Count, results.Count());
            RestClientMock.VerifyAll();
        }
    }
}
