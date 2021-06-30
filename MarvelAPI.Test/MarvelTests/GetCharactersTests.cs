using MarvelAPI.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace MarvelAPI.Test.MarvelTests
{
    [TestClass]
    public class GetCharactersTests : MarvelTestBase
    {
        [TestMethod]
        public void Success()
        {
            // arrange

            // act
            var results = MarvelClient.GetCharacters(new GetCharacters
            {

            });

            // assert
            Assert.IsNotNull(results);
            Assert.IsTrue(results.Count() > 0);
        }
    }
}
