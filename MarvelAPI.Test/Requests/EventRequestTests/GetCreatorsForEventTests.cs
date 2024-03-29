﻿using MarvelAPI.Parameters;
using Moq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MarvelAPI.Test.Requests.EventRequestTests
{
    public class GetCreatorsForEventTests : EventRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // arrange
            var eventId = 1;
            var creatorList = new List<Creator>
            {
                new Creator
                {
                }
            };

            RestClientMock.Setup(c => c.Execute<Wrapper<Creator>>(It.Is<IRestRequest>(r => r.Resource == $"/events/{eventId}/creators")))
                .Returns(new RestResponse<Wrapper<Creator>>
                {
                    Data = new Wrapper<Creator>
                    {
                        Data = new Container<Creator>
                        {
                            Results = creatorList
                        }
                    }
                })
                .Verifiable();

            // act
            var results = Request.GetCreatorsForEvent(new GetCreatorsForEvent
            {
                EventId = eventId
            });

            // assert
            Assert.Equal(creatorList.Count, results.Count());
            RestClientMock.VerifyAll();
        }
    }
}
