using Moq;
using RestSharp;

namespace MarvelAPI.Test.Requests.StoryRequestTests
{
    public class StoryRequestTestBase
    {
        public string PublicApiKey { get; set; }
        public string PrivateApiKey { get; set; }
        public bool? UseGZip { get; set; }
        public Mock<IRestClient> RestClientMock { get; set; }

        public StoryRequest Request { get; set; }

        public StoryRequestTestBase()
        {
            RestClientMock = new Mock<IRestClient>();

            Request = new StoryRequest(PublicApiKey, PrivateApiKey, RestClientMock.Object, UseGZip);
        }
    }
}
