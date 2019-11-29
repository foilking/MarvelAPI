using System;

namespace MarvelAPI.Exceptions
{

    /// <summary>
    /// Raised if the API requests are throttled after exceeding the limit of 3,000 requests per day.
    /// </summary>
    public class LimitExceededException : Exception
    {


        public LimitExceededException(string status) : base(status)
        {



        }
    }
}
