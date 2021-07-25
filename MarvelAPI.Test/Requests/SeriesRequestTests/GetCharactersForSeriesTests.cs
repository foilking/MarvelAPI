using MarvelAPI.Parameters;
using Moq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MarvelAPI.Test.Requests.SeriesRequestTests
{
    public class GetCharactersForSeriesTests : SeriesRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // arrange
            var seriesId = 1; 
            var characterList = new List<Character>
            {
                new Character
                {
                }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Character>>(It.Is<IRestRequest>(r => r.Resource == $"/series/{seriesId}/characters")))
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
            var results = Request.GetCharactersForSeries(new GetCharactersForSeries
            {
                SeriesId = seriesId
            });

            // assert
            Assert.Equal(characterList.Count, results.Count());
            RestClientMock.VerifyAll();
        }
    }
}
