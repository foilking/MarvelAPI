using Moq;
using MSTestExtensions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI.Test.Requests.StoryRequestTests
{
    public class StoryRequestTestBase : BaseTest
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
