using Moq;
using RestSharp;

namespace MarvelAPI.Test.Requests.CreatorRequestTests
{
    public class CreatorRequestTestBase
    {
        public string PublicApiKey { get; set; }
        public string PrivateApiKey { get; set; }
        public bool? UseGZip { get; set; }
        public Mock<IRestClient> RestClientMock { get; set; }

        public CreatorRequests Requests { get; set; }

        public CreatorRequestTestBase()
        {
            RestClientMock = new Mock<IRestClient>();

            Requests = new CreatorRequests(PublicApiKey, PrivateApiKey, RestClientMock.Object, UseGZip);
        }
    }
}
