using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI
{
    public class Wrapper<T>
    {
        public int Code { get; set; }
        public string Status { get; set; }
        public T Data { get; set; }
        public string Etag { get; set; }
        public string Copyright { get; set; }
        public string AttributionText { get; set; }
        public string AttributionHTML { get; set; }
    }
}
