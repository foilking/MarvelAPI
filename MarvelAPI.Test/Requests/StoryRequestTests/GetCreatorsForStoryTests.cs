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
    public class GetCreatorsForStoryTests : StoryRequestTestBase
    {
        [TestMethod]
        public void Success()
        {
            // arrange
            var storyId = 1;
            var creatorList = new List<Creator>
            {
                new Creator
                {
                }
            };

            RestClientMock.Setup(c => c.Execute<Wrapper<Creator>>(It.Is<IRestRequest>(r => r.Resource == $"/stories/{storyId}/creators")))
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
            var results = Request.GetCreatorsForStory(new GetCreatorsForStory
            {
                StoryId = storyId
            });

            // assert
            Assert.AreEqual(creatorList.Count, results.Count());
            RestClientMock.VerifyAll();
        }
    }
}
