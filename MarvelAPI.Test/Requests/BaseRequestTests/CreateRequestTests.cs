using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI.Test.Requests.BaseRequestTests
{
    [TestClass]
    public class CreateRequestTests
    {
        [TestMethod]
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
            Assert.AreEqual(requestUrl, restRequest.Resource);

            var parameters = restRequest.Parameters;
            Assert.IsNotNull(parameters.FirstOrDefault(p => p.Name == "ts"));

            var apikeyParameter = parameters.FirstOrDefault(p => p.Name == "apikey");
            Assert.IsNotNull(apikeyParameter);
            Assert.AreEqual(publicApiKey, apikeyParameter.Value.ToString());

            Assert.IsNotNull(parameters.FirstOrDefault(p => p.Name == "hash"));

            var acceptParameter = parameters.FirstOrDefault(p => p.Name == "Accept");
            Assert.IsNotNull(acceptParameter);
            Assert.AreEqual("*/*", acceptParameter.Value.ToString());
        }

        [TestMethod]
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
            Assert.AreEqual(requestUrl, restRequest.Resource);

            var parameters = restRequest.Parameters;
            Assert.IsNotNull(parameters.FirstOrDefault(p => p.Name == "ts"));

            var apikeyParameter = parameters.FirstOrDefault(p => p.Name == "apikey");
            Assert.IsNotNull(apikeyParameter);
            Assert.AreEqual(publicApiKey, apikeyParameter.Value.ToString());

            Assert.IsNotNull(parameters.FirstOrDefault(p => p.Name == "hash"));

            var acceptParameter = parameters.FirstOrDefault(p => p.Name == "Accept-Encoded");
            Assert.IsNotNull(acceptParameter);
            Assert.AreEqual("gzip", acceptParameter.Value.ToString());
        }
    }
}
