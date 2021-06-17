using MarvelAPI.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MSTestExtensions;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI.Test.Requests
{
    [TestClass]
    public class HandleResponseErrorsTests : BaseTest
    {
        [TestMethod]
        public void NonError_409()
        {
            // arrange
            var publicApiKey = "publicKey";
            var privateApiKey = "privateApiKey";
            var clientMock = new Mock<IRestClient>();
            var baseRequest = new BaseRequest(publicApiKey, privateApiKey, clientMock.Object);

            var message = "409 Message";
            var response = new RestResponse<Wrapper<Character>>
            {
                Data = new Wrapper<Character>
                {
                    Code = 409,
                    Status = message
                }
            };

            // assert
            Assert.Throws<ArgumentException>(() => baseRequest.HandleResponseErrors(response), message);
        }

        [TestMethod]
        public void Error_404()
        {
            // arrange
            var publicApiKey = "publicKey";
            var privateApiKey = "privateApiKey";
            var clientMock = new Mock<IRestClient>();
            var baseRequest = new BaseRequest(publicApiKey, privateApiKey, clientMock.Object);

            var message = "404 Message";
            var errorString = JsonConvert.SerializeObject(new MarvelError
            {
                Message = message
            });
            var response = new RestResponse<IWrapper>
            {
                ResponseStatus = ResponseStatus.Error,
                Content = errorString
            };

            // assert
            Assert.Throws<NotFoundException>(() => baseRequest.HandleResponseErrors(response), message);
        }

        [TestMethod]
        public void Error_401()
        {
            // arrange
            var publicApiKey = "publicKey";
            var privateApiKey = "privateApiKey";
            var clientMock = new Mock<IRestClient>();
            var baseRequest = new BaseRequest(publicApiKey, privateApiKey, clientMock.Object);

            var message = "401 Message";
            var errorString = JsonConvert.SerializeObject(new MarvelError
            {
                Message = message,
                Code = "InvalidCredentials"
            });
            var response = new RestResponse<IWrapper>
            {
                ResponseStatus = ResponseStatus.Error,
                Content = errorString
            };

            // assert
            Assert.Throws<InvalidCredentialException>(() => baseRequest.HandleResponseErrors(response), message);
        }

        [TestMethod]
        public void Error_429()
        {
            // arrange
            var publicApiKey = "publicKey";
            var privateApiKey = "privateApiKey";
            var clientMock = new Mock<IRestClient>();
            var baseRequest = new BaseRequest(publicApiKey, privateApiKey, clientMock.Object);

            var message = "429 Message";
            var errorString = JsonConvert.SerializeObject(new MarvelError
            {
                Message = message,
                Code = "RequestThrottled"
            });
            var response = new RestResponse<IWrapper>
            {
                ResponseStatus = ResponseStatus.Error,
                Content = errorString
            };

            // assert
            Assert.Throws<LimitExceededException>(() => baseRequest.HandleResponseErrors(response), message);
        }
    }
}
