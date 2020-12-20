using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Poc.ReadRSS
{
    public class RssPost
    {
        public RssPost(string title, string sumary, DateTime publishDate, string postUri, string author)
        {
            Title = title;
            Sumary = sumary;
            PublishedDate = publishDate;
            PostUri = postUri;
            Author = author;

            PictureUrls = new List<string>();
        }

        [BsonId]
        public Guid Id { get; set; }
        [BsonElement("title")]
        public string Title { get; set; }
        [BsonElement("sumary")]
        public string Sumary { get; set; }
        [BsonElement("publishDate")]
        public DateTime PublishedDate { get; set; }
        [BsonElement("postUri")]
        public string PostUri { get; set; }
        [BsonElement("author")]
        public string Author { get; set; }
        [BsonElement("pictureUrls")]
        public List<string> PictureUrls { get; set; }
        [BsonElement("feedUrlSource")]
        public string FeedUrlSource { get; set; }

        public void SetPictureUrl(string url)
        {
            if (url != "" && url != null)
            {
                PictureUrls.Add(url);

            }

        }

    }
}