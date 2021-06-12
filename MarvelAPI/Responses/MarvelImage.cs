namespace MarvelAPI
{
    public class MarvelImage
    {
        public string Path { get; set; }
        public string Extension { get; set; }
        public override string ToString()
        {
            return string.Format("{0}.{1}", Path, Extension);
        }
        public string ToString(Image size)
        {
            return string.Format("{0}{1}.{2}", Path, size.ToParameter(), Extension);
        }
    }
}
