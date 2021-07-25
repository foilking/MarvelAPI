using Moq;
using RestSharp;
using System.Collections.Generic;
using Xunit;

namespace MarvelAPI.Test.Requests.CreatorRequestTests
{
    public class GetCreatorTests : CreatorRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // Arrange
            var creatorId = 1;
            RestClientMock.Setup(c => c.Execute<Wrapper<Creator>>(It.Is<IRestRequest>(r => r.Resource == $"/creators/{creatorId}")))
                .Returns(new RestResponse<Wrapper<Creator>>
                {
                    Data = new Wrapper<Creator>
                    {
                        Data = new Container<Creator>
                        {
                            Results = new List<Creator>
                            {
                                new Creator
                                {
                                    Id = creatorId
                                }
                            }
                        }
                    }
                });

            // Act
            var result = Requests.GetCreator(creatorId);

            // Assert

            RestClientMock.VerifyAll();
        }
    }
}
