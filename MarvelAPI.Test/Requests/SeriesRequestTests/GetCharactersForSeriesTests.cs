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

namespace MarvelAPI.Test.Requests.SeriesRequestTests
{
    [TestClass]
    public class GetCharactersForSeriesTests : SeriesRequestTestBase
    {
        [TestMethod]
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
            Assert.AreEqual(characterList.Count, results.Count());
            RestClientMock.VerifyAll();
        }
    }
}
