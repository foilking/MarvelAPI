using Moq;
using RestSharp;

namespace MarvelAPI.Test.Requests.CharacterRequestTests
{
    public class CharacterRequestTestBase
    {
        public string PublicApiKey { get; set; }
        public string PrivateApiKey { get; set; }
        public bool? UseGZip { get; set; }
        public Mock<IRestClient> RestClientMock { get; set; }
        public CharacterRequests Requests { get; set; }
        public CharacterRequestTestBase()
        {
            RestClientMock = new Mock<IRestClient>();

            Requests = new CharacterRequests(PublicApiKey, PrivateApiKey, RestClientMock.Object, UseGZip);
        }
    }
}
