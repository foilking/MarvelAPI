namespace MarvelAPI
{
    public class BaseWrapper
    {
        public int Code { get; set; }
        public string Status { get; set; }
    }
    public class Wrapper<T> : BaseWrapper where T : IMarvelItem
    {
        public Container<T> Data { get; set; }
        public string Etag { get; set; }
        public string Copyright { get; set; }
        public string AttributionText { get; set; }
        public string AttributionHTML { get; set; }
    }
}
