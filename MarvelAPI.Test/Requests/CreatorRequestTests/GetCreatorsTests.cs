using MarvelAPI.Parameters;
using Moq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MarvelAPI.Test.Requests.CreatorRequestTests
{
    public class GetCreatorsTests : CreatorRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // arrange
            var creatorList = new List<Creator>
            {
                new Creator
                {
                }
            };

            RestClientMock.Setup(c => c.Execute<Wrapper<Creator>>(It.Is<IRestRequest>(r => r.Resource == "/creators")))
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
            var comics = Requests.GetCreators(new GetCreators
            {

            });

            // assert
            Assert.Equal(creatorList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }
    }
}
