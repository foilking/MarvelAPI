using MarvelAPI.Parameters;
using Moq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MarvelAPI.Test.Requests.CharacterRequestTests
{

    public class GetSeriesForCharacterTests : CharacterRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // arrange
            var characterId = 1;
            var seriesList = new List<Series>
            {
                new Series { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Series>>(It.Is<IRestRequest>(r => r.Resource == $"/characters/{characterId}/series")))
                .Returns(new RestResponse<Wrapper<Series>>
                {
                    Data = new Wrapper<Series>
                    {
                        Data = new Container<Series>
                        {
                            Results = seriesList
                        }
                    }
                })
                .Verifiable();

            // act
            var comics = Requests.GetSeriesForCharacter(new GetSeriesForCharacter
            {
                CharacterId = characterId
            });

            // assert
            Assert.Equal(seriesList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }
    }
}
