using System;

namespace MarvelAPI.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message, Exception innerException) : base(message, innerException)
        {


        }

    }
}
