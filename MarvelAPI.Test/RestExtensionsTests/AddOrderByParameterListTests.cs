using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI.Test.RestExtensionsTests
{
    [TestClass]
    public class AddOrderByParameterListTests
    {
        private IEnumerable<OrderBy> ValidOrderBy;
        private IEnumerable<OrderBy> InvalidOrderBy 
        {
            get
            {
                return ValidOrderBy != null ? AllOrderByOptions.Where(o => !ValidOrderBy.Contains(o)) : AllOrderByOptions;
            }
        }
        private IEnumerable<OrderBy> AllOrderByOptions;
        public AddOrderByParameterListTests()
        {
            AllOrderByOptions = ((OrderBy[])Enum.GetValues(typeof(OrderBy))).ToList();
        }

        [TestMethod]
        public void AnyAvailableParameters()
        {
            // arrange
            var request = new RestRequest();
            ValidOrderBy = new List<OrderBy> { OrderBy.FirstName, OrderBy.LastName, OrderBy.MiddleName };
            var validOrderByString = string.Join(",", ValidOrderBy.Select(o => o.ToParameter()));

            // act
            request.AddOrderByParameterList(ValidOrderBy, ValidOrderBy);

            // assert
            var orderByParameter = request.Parameters.FirstOrDefault(p => p.Name == "orderBy");
            Assert.IsNotNull(orderByParameter);
            var orderByValues = orderByParameter.Value.ToString();
            Assert.AreEqual(validOrderByString, orderByValues);
        }

        [TestMethod]
        public void NoAvailableParameters()
        {
            // arrange
            var request = new RestRequest();
            ValidOrderBy = new List<OrderBy> { OrderBy.FirstName, OrderBy.LastName, OrderBy.MiddleName };
            var validOrderByString = string.Join(",", ValidOrderBy.Select(o => o.ToParameter()));

            // act
            request.AddOrderByParameterList(ValidOrderBy, InvalidOrderBy);

            // assert
            var orderByParameter = request.Parameters.FirstOrDefault(p => p.Name == "orderBy");
            Assert.IsNull(orderByParameter);
        }
    }
}
