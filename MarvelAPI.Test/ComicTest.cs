using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MarvelAPI.Test
{

    [TestClass]
    public class ComicTest
    {
        private Marvel _marvel { get; set; }
        private string _marvelPublicKey { get; set; }
        private string _marvelPrivateKey { get; set; }
        private const int TOTAL_CHARACTERS = 1402;
        private readonly CompareInfo _comparer;

        public ComicTest()
        {

            _marvelPublicKey = "ae9f4edf31b262cf13acb7b7f972bc61";
            _marvelPrivateKey = "ac741b4559b405ef585151ef6abe146c23e5d96a";

            _marvel = new Marvel(_marvelPublicKey, _marvelPrivateKey);
            _comparer = CompareInfo.GetCompareInfo("en-US");
        }


        [TestMethod]
        public void GetComicsTest()
        {
            var comics = _marvel.GetComics(dateDescript: DateDescriptor.ThisWeek);

            // Assert
            Assert.IsInstanceOfType(comics, typeof(IEnumerable<Comic>));
            Assert.AreEqual(comics.Count(), 20);



        }


    }
}
