using System.Collections.Generic;

namespace MarvelAPI
{
    public class ResourceList<T>
    {
        public int Available { get; set; }
        public int Returned { get; set; }
        public string CollectionURI { get; set; }
        public List<T> Items { get; set; }
    }
}
