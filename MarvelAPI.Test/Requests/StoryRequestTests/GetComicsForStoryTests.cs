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

namespace MarvelAPI.Test.Requests.StoryRequestTests
{
    [TestClass]
    public class GetComicsForStoryTests : StoryRequestTestBase
    {
        [TestMethod]
        public void Success()
        {
            // arrange
            var storyId = 1;
            var comicList = new List<Comic>
            {
                new Comic { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Comic>>(It.Is<IRestRequest>(r => r.Resource == $"/stories/{storyId}/comics")))
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
            var results = Request.GetComicsForStory(new GetComicsForStory
            {
                StoryId = storyId
            });

            // assert
            Assert.AreEqual(comicList.Count, results.Count());
            RestClientMock.VerifyAll();
        }
    }
}
