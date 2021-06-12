using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Globalization;
using Moq;
using RestSharp;

namespace MarvelAPI.Test
{
    [TestClass]
    public class CharacterRequestsTests
    {
        public CharacterRequestsTests()
        {
        }

        [TestMethod]
        public void GetCharactersTest()
        {
            // Arrange
            var characterId = 1;
            var clientMock = new Mock<IRestClient>();
            clientMock.Setup(c => c.Execute<Wrapper<Container<Character>>>(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse<Wrapper<Container<Character>>>
                {
                    Data = new Wrapper<Container<Character>>
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
                });
            var requests = new CharacterRequests(It.IsAny<string>(), It.IsAny<string>(), clientMock.Object);

            // Act
            var result = requests.GetCharacter(characterId);

            // Assert

            clientMock.Verify(c => c.Execute<Wrapper<Container<Character>>>(It.IsAny<IRestRequest>()), Times.Once);
            
        }
    }
}
