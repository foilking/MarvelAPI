using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarvelAPI
{
    public static class RestExtensions
    {
        public static void AddParameterList(this RestRequest request, IEnumerable<int> parameter, string parameterString)
        {
            if (parameter != null && parameter.Count() > 0)
            {
                request.AddParameter(parameterString, string.Join<int>(",", parameter));
            }
        }

        public static void AddOrderByParameterList(this RestRequest request, IEnumerable<OrderBy> parameter, IEnumerable<OrderBy> available)
        {
            if (parameter != null && parameter.Count() > 0)
            {
                StringBuilder orderString = new StringBuilder();
                foreach (var order in parameter)
                {
                    if (available.Contains(order))
                    {
                        if (orderString.Length > 0)
                        {
                            orderString.Append(",");
                        }
                        orderString.Append(order.ToParameter());
                        break;
                    }
                }
                if (orderString.Length > 0)
                {
                    request.AddParameter("orderBy", orderString.ToString());
                }
            }
        }
    }
}
