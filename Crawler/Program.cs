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
using System.Diagnostics;

namespace Crawler
{
    internal class Program
    {
        public static HashSet<string> src_need_status = new HashSet<string>();
        public static HashSet<string> href_need_status = new HashSet<string>();
        public static HashSet<string> capture = new HashSet<string>(); 
		
        private static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            //var spider = new Spider(thisUrl);
            //spider.Crawl();



            //var captured = 0;
            var client = new WebClient();
            var thisUrl = new Uri("http://www.spsu.edu");
            var spider = new Spider(true);

            //var test = new Uri("https://zimbra.spsu.edu");
            //Console.WriteLine(GetResponseCode(test));
            //sw.Stop();
            //Console.WriteLine(sw.Elapsed);
            //Console.ReadLine();

            var max = 0;
            var iterator = 0;
            while (max < 10000)
            {

                try
                {
                    //using (WebClient client1 = new WebClient())
                    string htmlSource;
                    //using (client)
                    //{
                    //    client.Headers[HttpRequestHeader.Accept] = "text/html, image/png, image/jpeg, image/gif, */*;q=0.1";
                    //    client.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows; U; Windows NT 6.1; de; rv:1.9.2.12) Gecko/20101026 Firefox/3.6.12";
                    client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");  
                    htmlSource = client.DownloadString(thisUrl);
                        //int i = 0;
                    //}
                    //var htmlSource = client.DownloadString(thisUrl);
                    href_need_status.Add(thisUrl.ToString());
                    //while (dbHelper.CheckRunning()) {
                    //  thisUrl = get new url from database;
                    // crawl method here 
                    spider.GetLinksFromWebsite(ref href_need_status, thisUrl, htmlSource);
                    spider.GetLinksFromImages(ref src_need_status, thisUrl, htmlSource);

                    // }
                    foreach (var item1 in src_need_status)
                    {
                        Console.WriteLine(item1);
                    }
                    foreach (var item2 in href_need_status)
                    {
                        Console.WriteLine(item2);
                    }
                    max = src_need_status.Count + href_need_status.Count;
                    var sumsum = "\n-------------" + thisUrl + "-----------------------   ";
                    Console.WriteLine(sumsum + max + "\n");

                    iterator++;
                    thisUrl = new Uri(href_need_status.ElementAt(iterator));

                    var respCode = GetResponseCode(thisUrl);
                    //Console.WriteLine(thisUrl + " -- " + respCode);
                    while (respCode != 200)
                    {
                        iterator++;
                        thisUrl = new Uri(href_need_status.ElementAt(iterator));
                        //if (thisUrl.ToString().Equals("https://zimbra.spsu.edu"))
                        //    iterator++;
                        //else
                            respCode = GetResponseCode(thisUrl);
                    }
                    Console.WriteLine(" ******  RUNNING  :: " + thisUrl + "\n");
                }
                catch (Exception)
                {
                    continue;
                    //throw;
                }
            }
            Console.WriteLine(max.ToString());
            //Console.WriteLine(captured);
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            Console.ReadLine();
            //----------------------
            
        }

            /*  **********    IDEA    **********  
             * 
             * 1. need to change spider.Crawl to return a value.
             * 2. Data Structure
             *      2a. ConcurrentDictionary
             *      2b. HashSet... however, using "lock"
             * 3. Syntax ->
             *      3a. ?
             *      3b. 
             *      var data = new ConcurrentDictionary<string, int>();
             *      Parallel.Invoke(
             *      
            */
        public static int GetResponseCode(Uri thisUrl)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(thisUrl);
            //webRequest.AllowAutoRedirect = true;
            webRequest.AllowAutoRedirect = false;
            webRequest.Timeout = 3000;
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
        public static List<string> GetLinksFromImages(string imgSource)
        {
            Match m;
            var listReturn = new List<string>();
            const string srcPattern = "<img.+?src=[\"'](.+?)[\"'].*?>";
            // Another change
            try
            {
                m = Regex.Match(imgSource, srcPattern,
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
    }
}

