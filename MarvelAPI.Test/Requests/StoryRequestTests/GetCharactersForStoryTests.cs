using MarvelAPI.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MSTestExtensions;
using RestSharp;
using System.Collections.Generic;
using System.Linq;

namespace MarvelAPI.Test.Requests.StoryRequestTests
{
    [TestClass]
    public class GetCharactersForStoryTests : StoryRequestTestBase
    {
        [TestMethod]
        public void Success()
        { 
            // arrange
            var storyId = 1;
            var characterList = new List<Character>
            {
                new Character
                {
                }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Character>>(It.Is<IRestRequest>(r => r.Resource == $"/stories/{storyId}/characters")))
                .Returns(new RestResponse<Wrapper<Character>>
                {
                    Data = new Wrapper<Character>
                    {
                        Data = new Container<Character>
                        {
                            Results = characterList
                        }
                    }
                })
                .Verifiable();

            // act
            var results = Request.GetCharactersForStory(new GetCharactersForStory
            {
                StoryId = storyId
            });

            // assert
            Assert.AreEqual(characterList.Count, results.Count());
            RestClientMock.VerifyAll();
        }
    }
}
