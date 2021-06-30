using Moq;
using MSTestExtensions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI.Test.Requests.ComicsRequestTests
{
    public class ComicRequestTestBase : BaseTest
    {
        public string PublicApiKey { get; set; }
        public string PrivateApiKey { get; set; }
        public bool? UseGZip { get; set; }
        public Mock<IRestClient> RestClientMock { get; set; }

        public ComicRequests Requests { get; set; }

        public ComicRequestTestBase()

        {
            RestClientMock = new Mock<IRestClient>();

            Requests = new ComicRequests(PublicApiKey, PrivateApiKey, RestClientMock.Object, UseGZip);
        }
    }
}
