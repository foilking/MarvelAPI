using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI
{
    public interface IWrapper
    {
        int Code { get; set; }
        string Status { get; set; }
    }
    public class Wrapper<T> : IWrapper where T : IMarvelItem
    {
        public int Code { get; set; }
        public string Status { get; set; }
        public Container<T> Data { get; set; }
        public string Etag { get; set; }
        public string Copyright { get; set; }
        public string AttributionText { get; set; }
        public string AttributionHTML { get; set; }
    }
}
