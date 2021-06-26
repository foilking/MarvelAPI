using MarvelAPI.Requests;
using Moq;
using MSTestExtensions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI.Test.Requests.SeriesRequestTests
{
    public class SeriesRequestTestBase : BaseTest
    {
        public string PublicApiKey { get; set; }
        public string PrivateApiKey { get; set; }
        public bool? UseGZip { get; set; }
        public Mock<IRestClient> RestClientMock { get; set; }

        public SeriesRequest Request { get; set; }

        public SeriesRequestTestBase()
        {
            RestClientMock = new Mock<IRestClient>();

            Request = new SeriesRequest(PublicApiKey, PrivateApiKey, RestClientMock.Object, UseGZip);
        }
    }
}
