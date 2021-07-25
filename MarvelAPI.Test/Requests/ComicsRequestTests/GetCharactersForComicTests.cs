using MarvelAPI.Parameters;
using Moq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MarvelAPI.Test.Requests.ComicsRequestTests
{

    public class GetCharactersForComicTests : ComicRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // arrange
            var comicId = 1;
            var characterList = new List<Character>
            {
                new Character
                {
                }
            };

            RestClientMock.Setup(c => c.Execute<Wrapper<Character>>(It.Is<IRestRequest>(r => r.Resource == $"/comics/{comicId}/characters")))
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
            var characters = Requests.GetCharactersForComic(new GetCharactersForComic
            {
                ComicId = comicId
            });

            // assert
            Assert.Equal(characterList.Count, characters.Count());
            RestClientMock.VerifyAll();
        }
    }
}
