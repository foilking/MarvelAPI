using Moq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI.Test.MarvelTests
{
    public class MarvelTestBase
    {
        public string PublicApiKey { get; set; }
        public string PrivateApiKey { get; set; }
        public bool? UseGZip { get; set; }

        public Marvel MarvelClient { get; set; }

        public MarvelTestBase()
        {
            PublicApiKey = "67d146c4c462f0b55bf12bb7d60948af";
            PrivateApiKey = "54fd1a8ac788767cc91938bcb96755186074970b";
            MarvelClient = new Marvel(PublicApiKey, PrivateApiKey, UseGZip);
        }
    }
}
