using Moq;
using RestSharp;
using System.Linq;
using Xunit;

namespace MarvelAPI.Test.Requests.BaseRequestTests
{
    public class CreateRequestTests
    {
        [Fact]
        public void Success()
        {
            // arrange
            var requestUrl = "http://www.example.com";
            var publicApiKey = "publicKey";
            var privateApiKey = "privateApiKey";
            var clientMock = new Mock<IRestClient>();
            var baseRequest = new BaseRequest(publicApiKey, privateApiKey, clientMock.Object);

            // act
            var restRequest = baseRequest.CreateRequest(requestUrl);

            // assert
            Assert.Equal(requestUrl, restRequest.Resource);

            var parameters = restRequest.Parameters;
            Assert.NotNull(parameters.FirstOrDefault(p => p.Name == "ts"));

            var apikeyParameter = parameters.FirstOrDefault(p => p.Name == "apikey");
            Assert.NotNull(apikeyParameter);
            Assert.Equal(publicApiKey, apikeyParameter.Value.ToString());

            Assert.NotNull(parameters.FirstOrDefault(p => p.Name == "hash"));

            var acceptParameter = parameters.FirstOrDefault(p => p.Name == "Accept");
            Assert.NotNull(acceptParameter);
            Assert.Equal("*/*", acceptParameter.Value.ToString());
        }

        [Fact]
        public void GZip_Success()
        {
            // arrange
            var requestUrl = "http://www.example.com";
            var publicApiKey = "publicKey";
            var privateApiKey = "privateApiKey";
            var clientMock = new Mock<IRestClient>();
            var baseRequest = new BaseRequest(publicApiKey, privateApiKey, clientMock.Object, true);

            // act
            var restRequest = baseRequest.CreateRequest(requestUrl);

            // assert
            Assert.Equal(requestUrl, restRequest.Resource);

            var parameters = restRequest.Parameters;
            Assert.NotNull(parameters.FirstOrDefault(p => p.Name == "ts"));

            var apikeyParameter = parameters.FirstOrDefault(p => p.Name == "apikey");
            Assert.NotNull(apikeyParameter);
            Assert.Equal(publicApiKey, apikeyParameter.Value.ToString());

            Assert.NotNull(parameters.FirstOrDefault(p => p.Name == "hash"));

            var acceptParameter = parameters.FirstOrDefault(p => p.Name == "Accept-Encoded");
            Assert.NotNull(acceptParameter);
            Assert.Equal("gzip", acceptParameter.Value.ToString());
        }
    }
}
