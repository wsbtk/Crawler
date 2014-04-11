using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Crawler
{
    class Program
    {
		//private static BackgroundWorker _bw;// = new BackgroundWorker();
        static void Main(string[] args)
        {
            //var spider = new Spider(thisUrl);
            //spider.Crawl();
            var capture = new HashSet<string>();
            var dict1 = new Dictionary<string, string>();
            var dict2 = new Dictionary<string, string>();
            var captured = 0;
            var client = new WebClient();
            var thisUrl = new Uri("http://www.spsu.edu");
            var htmlSource = client.DownloadString(thisUrl);

            var respCode = GetResponseCode(thisUrl);
            Console.WriteLine(thisUrl + " -- " + respCode);
            if (respCode == 200)
            {
                var returnedLinks = GetLinksFromWebsite(htmlSource);
                if (returnedLinks != null) {
                    foreach (var item in returnedLinks)
                    {
                        if ((item.Contains("#")) || (item.Contains(".xml")) || (item.Contains("omniupdate"))
                            || (item.Contains("go.view.usg.edu")) || (item.Contains("mailto")) || (item.Equals(""))) continue;
                        //if (capture.Contains(item)) continue;
                        var temp = new Uri(thisUrl, item);
                        var line = temp.AbsoluteUri;
                        //capture.Add(line);
                        if (dict1.ContainsKey(line)) continue;
                        dict1.Add(line,thisUrl.ToString());
                        captured++;
                        //respCode = GetResponseCode(new Uri(line));
                        //Console.WriteLine(thisUrl + "\n -- " + line);
                    }
                }
            }
            //foreach (var items in capture)
            foreach (var items in dict1.Keys)
            {
                htmlSource = client.DownloadString(new Uri(items));
                var returnedLinks = GetLinksFromWebsite(htmlSource);
                if (returnedLinks == null) continue;
                // NON LINQ
                foreach (var item in returnedLinks)
                {
                    if ((item.Contains("#")) || (item.Contains(".xml")) || (item.Contains("omniupdate")) || (item.Contains("file"))
                        || (item.Contains("go.view.usg.edu")) || (item.Contains("mailto")) || (item.Equals(""))) continue;
                    var temp = new Uri(thisUrl, item);
                    var line = temp.AbsoluteUri;
                    
                //foreach (var line in from item in returnedLinks
                //                     where (!item.Contains("#"))
                //                           && (!item.Contains(".xml")) 
                //                           && (!item.Contains("omniupdate"))
                //                           && (!item.Contains("file")) 
                //                           && (!item.Contains("go.view.usg.edu"))
                //                           && (!item.Contains("mailto")) 
                //                           && (!item.Equals(""))
                //                     select new Uri(thisUrl, item) into temp
                //                     select temp.AbsoluteUri)
                //{
                    if (dict2.ContainsKey(line)) continue;
                    dict2.Add(line, thisUrl.ToString());
                    //capture.Add(line);
                    captured++;
                    //respCode = GetResponseCode(new Uri(line));
                    //Console.WriteLine(items + "\n -- " + line);
                }
            }
            Console.WriteLine("Dictionary Keys - " + (int)(dict2.Keys.Count + dict1.Keys.Count));
            Console.WriteLine("Dictionary Values - " + (int)(dict1.Values.Count + dict2.Values.Count));
            Console.WriteLine("Dictionary Values (distinct) - " + (int)(dict1.Values.Distinct().Count() + dict2.Values.Distinct().Count()));
            Console.WriteLine(captured);
            Console.ReadLine();
        }

        public static int GetResponseCode(Uri thisUrl)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(thisUrl);
            webRequest.AllowAutoRedirect = true;
            var resp = (HttpWebResponse) webRequest.GetResponse();
            return (int)resp.StatusCode;
        }
        public static List<string> GetLinksFromWebsite(string htmlSource)
        {
            Match m;
            var listReturn = new List<string>();
            const string HRefPattern = "href\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))";

            try
            {
                m = Regex.Match(htmlSource, HRefPattern,
                                RegexOptions.IgnoreCase | RegexOptions.Compiled,
                                TimeSpan.FromSeconds(1));
                while (m.Success)
                {
                    listReturn.Add(m.Groups[1].ToString());
                    //Console.WriteLine("Found href " + m.Groups[1] + " at " + m.Groups[1].Index);
                    m = m.NextMatch();
                }
                return listReturn;
            }
            catch (RegexMatchTimeoutException)
            {
                Console.WriteLine("The matching operation timed out.");
                return null;
            }   
        }
        /*
        public static List<string> GetLinksFromWebsite(string htmlSource)
        {
            var doc = new HtmlDocument();
            try
            {
                doc.LoadHtml(htmlSource);
                if (doc.DocumentNode.InnerHtml == null) return null;
                if (doc.DocumentNode.SelectNodes("//a[@ref]") == null)
                {
                    return doc
                       .DocumentNode
                       .SelectNodes("//a[@href]")
                       .Select(node => node.Attributes["href"].Value)
                       .ToList();
                }
                return doc
                    .DocumentNode
                    .SelectNodes("//a[@ref]")
                    .Select(node => node.Attributes["ref"].Value)
                    .ToList();
            }
            catch (Exception)
            {
            }
            return null;
        }
         * */
    }
}
