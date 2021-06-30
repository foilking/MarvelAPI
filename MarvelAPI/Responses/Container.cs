using System.Collections.Generic;

namespace MarvelAPI
{
    public class Container<T>
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
        public int Total { get; set; }
        public int Count { get; set; }
        public List<T> Results { get; set; }
    }
}
