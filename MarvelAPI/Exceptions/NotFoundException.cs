using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
