using MarvelAPI.Exceptions;
using Moq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Security.Authentication;
using Xunit;

namespace MarvelAPI.Test.Requests.BaseRequestTests
{
    public class HandleResponseErrorsTests
    {
        [Fact]
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
            var error = Assert.Throws<ArgumentException>(() => baseRequest.HandleResponseErrors(response));
            Assert.Equal(message, error.Message);
        }

        [Fact]
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
            var response = new RestResponse<BaseWrapper>
            {
                ResponseStatus = ResponseStatus.Error,
                Content = errorString
            };

            // assert
            var error = Assert.Throws<NotFoundException>(() => baseRequest.HandleResponseErrors(response));
            Assert.Equal(message, error.Message);
        }

        [Fact]
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
            var response = new RestResponse<BaseWrapper>
            {
                ResponseStatus = ResponseStatus.Error,
                Content = errorString
            };

            // assert
            var error = Assert.Throws<InvalidCredentialException>(() => baseRequest.HandleResponseErrors(response));
            Assert.Equal(message, error.Message);
        }

        [Fact]
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
            var response = new RestResponse<BaseWrapper>
            {
                ResponseStatus = ResponseStatus.Error,
                Content = errorString
            };

            // assert
            var error = Assert.Throws<LimitExceededException>(() => baseRequest.HandleResponseErrors(response));
            Assert.Equal(message, error.Message);
        }
    }
}
