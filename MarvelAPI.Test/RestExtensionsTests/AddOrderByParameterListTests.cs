using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MarvelAPI.Test.RestExtensionsTests
{
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

        [Fact]
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
            Assert.NotNull(orderByParameter);
            var orderByValues = orderByParameter.Value.ToString();
            Assert.Equal(validOrderByString, orderByValues);
        }

        [Fact]
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
            Assert.Null(orderByParameter);
        }
    }
}
