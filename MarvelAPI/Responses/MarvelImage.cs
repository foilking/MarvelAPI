namespace MarvelAPI
{
    public class MarvelImage
    {
        public string Path { get; set; }
        public string Extension { get; set; }
        public override string ToString()
        {
            return $"{Path}.{Extension}";
        }
        public string ToString(Image size)
        {
            return $"{Path}{size.ToParameter()}.{Extension}";
        }
    }
}
