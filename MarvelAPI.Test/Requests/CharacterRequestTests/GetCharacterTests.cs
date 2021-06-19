using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Globalization;
using Moq;
using RestSharp;

namespace MarvelAPI.Test.Requests.CharacterRequestTests
{
    [TestClass]
    public class GetCharacterTests : CharacterRequestTestBase
    {
        [TestMethod]
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
                });
            
            // Act
            var result = Requests.GetCharacter(characterId);

            // Assert

            RestClientMock.Verify(c => c.Execute<Wrapper<Character>>(It.IsAny<IRestRequest>()), Times.Once);
            
        }
    }
}
