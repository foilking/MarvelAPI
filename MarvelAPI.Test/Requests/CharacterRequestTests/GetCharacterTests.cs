using Moq;
using RestSharp;
using System.Collections.Generic;
using Xunit;

namespace MarvelAPI.Test.Requests.CharacterRequestTests
{
    public class GetCharacterTests : CharacterRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // Arrange
            var characterId = 1;
            RestClientMock.Setup(c => c.Execute<Wrapper<Character>>(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse<Wrapper<Character>>
                {
                    Data = new Wrapper<Character>
                    {
                        Data = new Container<Character>
                        {
                            Results = new List<Character>
                            {
                                new Character
                                {
                                    Id = characterId
                                }
                            }
                        }
                    }
                }).Verifiable();
            
            // Act
            var result = Requests.GetCharacter(characterId);

            // Assert

            RestClientMock.Verify(c => c.Execute<Wrapper<Character>>(It.IsAny<IRestRequest>()), Times.Once);
            
        }
    }
}
