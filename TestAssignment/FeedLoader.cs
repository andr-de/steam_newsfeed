using System;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace TestAssignment
{
    public static class FeedLoader
    {
        public static async Task<List<News>> LoadFeed(List<string> gameIds)
        {
            var temp = new List<News>();

            foreach (string str in gameIds)
            {
                foreach (News i in ParseData(await LoadData(str)))
                {
                    temp.Add(i);
                }
            }

            temp.Sort(CompareNewsByData);

            return temp;
        }

        private static int CompareNewsByData(News x, News y)
        {
            return -String.Compare(x.date, y.date);
        }

        public static async Task<string> LoadData(string _id)
        {
            string content;
            try
            {
                var uri = "https://api.steampowered.com/ISteamNews/GetNewsForApp/v0002/?appid=" + _id + "&count=10&maxlength=300&format=json";
                HttpClient client = new HttpClient();
                var response = await client.GetAsync(uri);
                content = await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                content = "internalError";
            }

            return content;
        }

        public static List<News> ParseData(string data)
        {
            JObject o = JObject.Parse(data);
            var temp = new List<News>();
            News temp_news;
            var str = o.SelectToken(@"$.appnews.newsitems");
            foreach (var i in str)
            {
                temp_news = JsonConvert.DeserializeObject<News>(i.ToString());
                temp_news.contents = Regex.Replace(temp_news.contents, "<.*?>", "").Trim();
                temp_news.contents = Regex.Replace(temp_news.contents, "\\{ST.*?(\\.png|\\.jpg) ", "").Trim();
                temp_news.contents = Regex.Replace(temp_news.contents, "http.*?( |\\.\\.\\.)", "...").Trim();
                temp.Add(temp_news);
            }

            return temp;
        }
    }
}
