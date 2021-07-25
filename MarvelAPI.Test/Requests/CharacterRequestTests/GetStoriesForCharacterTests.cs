using MarvelAPI.Parameters;
using Moq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MarvelAPI.Test.Requests.CharacterRequestTests
{
    public class GetStoriesForCharacterTests : CharacterRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // arrange
            var characterId = 1;
            var storyList = new List<Story>
            {
                new Story { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Story>>(It.Is<IRestRequest>(r => r.Resource == $"/characters/{characterId}/stories")))
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
            var comics = Requests.GetStoriesForCharacter(new GetStoriesForCharacter
            {
                CharacterId = characterId
            });

            // assert
            Assert.Equal(storyList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }
    }
}
