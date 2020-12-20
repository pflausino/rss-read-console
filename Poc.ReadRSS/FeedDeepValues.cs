namespace Poc.ReadRSS
{
    public class FeedDeepValues
    {
        public FeedDeepValues(string author, string imageUrlMethod1, string imageUrlMethod2, string imageUrlMethod3)
        {
            Author = author;
            ImageUrlMethod1 = imageUrlMethod1;
            ImageUrlMethod2 = imageUrlMethod2;
            ImageUrlMethod3 = imageUrlMethod3;
        }

        public string Author { get; set; }
        public string ImageUrlMethod1 { get; set; }
        public string ImageUrlMethod2 { get; set; }
        public string ImageUrlMethod3 { get; set; }
    }
}