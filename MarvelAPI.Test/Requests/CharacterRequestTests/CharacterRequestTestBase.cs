using Moq;
using MSTestExtensions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI.Test.Requests.CharacterRequestTests
{
    public class CharacterRequestTestBase : BaseTest
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
