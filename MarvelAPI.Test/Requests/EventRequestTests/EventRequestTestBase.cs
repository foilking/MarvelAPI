using MarvelAPI.Requests;
using Moq;
using MSTestExtensions;
using RestSharp;

namespace MarvelAPI.Test.Requests.EventRequestTests
{
    public class EventRequestTestBase : BaseTest
    {
        public string PublicApiKey { get; set; }
        public string PrivateApiKey { get; set; }
        public bool? UseGZip { get; set; }
        public Mock<IRestClient> RestClientMock { get; set; }

        public EventRequest Request { get; set; }

        public EventRequestTestBase()
        {
            RestClientMock = new Mock<IRestClient>();

            Request = new EventRequest(PublicApiKey, PrivateApiKey, RestClientMock.Object, UseGZip);
        }
    }
}
