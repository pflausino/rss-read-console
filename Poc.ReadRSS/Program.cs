using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Text.Json;
using System.Text;
using System.Xml.Linq;
using System.Web;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Threading.Tasks;

namespace Poc.ReadRSS
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("feedData");

            var collectionEntity = database.GetCollection<Entity>("entities");


            var colResult = collectionEntity.Find(new BsonDocument()).ToList();

            var feedLists = new List<string>();

            foreach (var item in colResult)
            {
                feedLists.Add(item.GetUrl());
            }

            var contador = 0;

            foreach (var url in feedLists)
            {

                contador++;
                System.Console.WriteLine("####################################\n");
                System.Console.WriteLine($"Starting Load - {url}  \n");
                System.Console.WriteLine($"Total {contador}/{feedLists.Count}\n");
                System.Console.WriteLine("####################################");

                System.Threading.Thread.Sleep(2000);

                using (var reader = XmlReader.Create(url))
                {
                    var feed = SyndicationFeed.Load(reader);
                    var topTen = feed.Items.Take(20);



                    var rssPost = Print(topTen, url);
                    SaveOnDataBase(rssPost);


                }
            }

        }

        private static void SaveOnDataBase(List<RssPost> rssPost)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("feedData");

            System.Console.Clear();

            var collectionRssPosts = database.GetCollection<RssPost>("rssPosts");
            System.Console.WriteLine("####################################\n");
            System.Console.WriteLine($"Saving on DB ................  \n");

            System.Threading.Thread.Sleep(1000);

            collectionRssPosts.InsertMany(rssPost);
        }

        static List<RssPost> Print(IEnumerable<SyndicationItem> topTen, string url)
        {
            var postList = new List<RssPost>();

            foreach (var item in topTen)
            {


                var feedDeepV = GetDeepValues(item);

                var rssPost = new RssPost(
                        item.Title.Text,
                        item.Summary.Text,
                        item.PublishDate.UtcDateTime.Date,
                        item.Id,
                        feedDeepV.Author
                    );

                rssPost.FeedUrlSource = url;
                rssPost.SetPictureUrl(feedDeepV.ImageUrlMethod1);
                rssPost.SetPictureUrl(feedDeepV.ImageUrlMethod2);
                rssPost.SetPictureUrl(feedDeepV.ImageUrlMethod3);



                // System.Console.WriteLine("-------------------------------------------------------");
                // System.Console.WriteLine(item.Title.Text);
                // System.Console.WriteLine(item.Summary.Text);
                // System.Console.WriteLine(item.PublishDate.UtcDateTime);
                // System.Console.WriteLine($"Uri: {item.Id}");
                // System.Console.WriteLine(feedDeepV.Author);
                // System.Console.WriteLine(feedDeepV.ImageUrlMethod1);
                // System.Console.WriteLine(feedDeepV.ImageUrlMethod2);
                // System.Console.WriteLine(feedDeepV.ImageUrlMethod3);

                // // System.Console.WriteLine(item.)
                // System.Console.WriteLine("-------------------------------------------------------");

                postList.Add(rssPost);

            }

            return postList;
        }
        static FeedDeepValues GetDeepValues(SyndicationItem item)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sc = new StringBuilder();
            foreach (SyndicationElementExtension extension in item.ElementExtensions)
            {
                XElement ele = extension.GetObject<XElement>();

                if (ele.Name.LocalName == "encoded" && ele.Name.Namespace.ToString().Contains("content"))
                {

                    sb.Append(ele.Value + "<br/>");
                }
                if (ele.Name.LocalName == "creator" && ele.Name.Namespace.ToString().Contains("dc"))
                {

                    sc.Append(ele.Value);
                }

            }

            var htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(sb.ToString());

            var attrMetodo1 = htmlDoc.DocumentNode.ChildNodes?.FirstOrDefault(x => x.Name == "p");
            var resultMetodo1 = attrMetodo1?.ChildNodes.FirstOrDefault(x => x.Name == "a")?.GetAttributes("href")?.ToList()[0].Value;


            var attrMetodo2 = htmlDoc.DocumentNode.ChildNodes?.FirstOrDefault(x => x.Name == "div");
            var resultMetodo2 = attrMetodo2?.ChildNodes.FirstOrDefault(x => x.Name == "img")?.GetAttributes("data-orig-file")?.ToList()[0].Value;

            var attrMetodo3 = htmlDoc.DocumentNode.ChildNodes?.FirstOrDefault(x => x.Name == "div");
            var resultMetodo3 = attrMetodo3?.ChildNodes.FirstOrDefault(x => x.Name == "a")?.GetAttributes("href")?.ToList()[0].Value;

            var result = new FeedDeepValues(sc.ToString(), resultMetodo1, resultMetodo2, resultMetodo3);

            return result;
        }
    }
}
