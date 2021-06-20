using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestSharp;
using System.Collections.Generic;

namespace MarvelAPI.Test.Requests.CreatorRequestTests
{
    [TestClass]
    public class GetCreatorTests : CreatorRequestTestBase
    {

        [TestMethod]
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
