using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;


    class Spider
    {
        public HashSet<string> capture = new HashSet<string>();
        public int position = 0;
        public int scanned = 0;
        public bool flag;
        public Hashtable HashCount;

        public Uri Url { get; set; }

        public Spider(Uri urlString, int MaxCount)
        {
            HashCount = new Hashtable(MaxCount);
            Url = urlString;
            flag = true;
        }

        public void Crawl() {
            using (var client = new WebClient()) {
                while (flag) {
                    string htmlSource;
                    if (capture.Count == 0) {
                        htmlSource = client.DownloadString(Url);
                    }
                    else {
                        //Console.WriteLine("hello " + position);
                        Console.WriteLine("Scanned " + scanned);
                        htmlSource = client.DownloadString(capture.ElementAt(position));
                        position++;
                    }
                    var returnedLinks = GetLinksFromWebsite(htmlSource);
                    if (returnedLinks == null) continue;
                    foreach (var item in GetLinksFromWebsite(htmlSource))
                    {
                        if ((item.Contains("#")) || (item.Contains(".xml")) || (item.Contains("omniupdate")) || (item.Contains("mailto"))) continue;
                        var firstChar = (item[0]);
                        string line;
                        var temp = new Uri(Url, item.ToString());
                        line = temp.AbsoluteUri;
                        //if (!firstChar.Equals('/')) {
                        //    if (firstChar.Equals('h')) { line = (item); }
                        //    else { line = ("http://www.spsu.edu/" + item); }
                        //}
                        //else { line = ("http://www.spsu.edu" + item); }
                        scanned++;
                        capture.Add(line);
                        Console.WriteLine(line);
                    }
                    Console.WriteLine("Testing the Value: " + capture.ElementAt<string>(position));
                }
            }
            Console.ReadKey();
    }
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
}
