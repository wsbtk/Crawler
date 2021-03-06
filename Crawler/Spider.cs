﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;

namespace Crawler
{
    class Spider
    {
        private bool flag;
        private int captured = 0;
        //private int _scan;
        //private readonly MySqlConnectionStringBuilder _connString;
        private readonly DateTime _startTime;
        private string _startTimeGuid;
        private readonly DatabaseHelper _dbHelper;

        public Spider(bool go)
        {
            flag = go;
            _dbHelper = new DatabaseHelper();
            _startTime = DateTime.Now;
           _dbHelper.BeginScan(_startTime);
        }

        public void Crawl(Uri thisUrl)
        {
            //var capture = new HashSet<string>();
            var dict1 = new Dictionary<string, string>();
            var client = new WebClient();
            //var thisUrl = new Uri("http://www.spsu.edu");
            var htmlSource = client.DownloadString(thisUrl);
            //var imgSource = client.DownloadString(thisUrl);

            //var respCode = GetResponseCode(thisUrl);
            //Console.WriteLine(thisUrl + " -- " + respCode);
            //if (respCode == 200)
            //{
            ////////////var returnedLinks = GetLinksFromWebsite(thisUrl, htmlSource);
            ////////////var returnedimgs = GetLinksFromImages(thisUrl, htmlSource);
            //if (returnedLinks != null) {
            //    foreach (var item in returnedLinks)
            //    {
            //        if ((item.Contains("#"))
            //            || (!item.Contains("spsu.edu"))
            //            || (item.Contains(".xml"))
            //            || (item.Contains("omniupdate"))
            //            || (item.Contains("mailto"))
            //            || (item.Equals(""))) continue;
            //        //|| (item.Contains("go.view.usg.edu")) 
            //        var temp = new Uri(thisUrl, item);
            //        var line = temp.AbsoluteUri;
            //        if (dict1.ContainsKey(line)) continue;

            //        dict1.Add(line, thisUrl.ToString());
            //        captured++;
            //    }
            //}
            //if (returnedimgs != null)
            //{
            //    //foreach (var line in returnedimgs.Where(item => (item.Contains(".gif"))
            //    //                && (item.Contains(".jpeg")) 
            //      //              && (!item.Equals(""))).Select(item => new Uri(thisUrl, item)).Select(temp => temp.AbsoluteUri).Where(line => !dict1.ContainsKey(line)))
            //    foreach(var item in returnedimgs)
            //    {
            //        var temp = new Uri(thisUrl, item);
            //        var line = temp.AbsoluteUri;
            //        dict1.Add(line, thisUrl.ToString());
            //        captured++;
            //    }
            //}
            //}
            Console.WriteLine("Dictionary Keys - " + dict1.Keys.Count);
            Console.WriteLine("Dictionary Values - " + dict1.Values.Count);
            Console.WriteLine("Dictionary Values (distinct) - " + dict1.Values.Distinct().Count());
            Console.WriteLine(captured);
            foreach (var link in dict1)
            {
                Console.WriteLine(link.Key + " - " + GetResponseCode(new Uri(link.Key)));
                //Uri new_link = link;
                //Crawl(new_link);
            }

             foreach (var link in dict1)
             {
                 _startTimeGuid = _dbHelper.StartTimeGuid;
                var scanid = _dbHelper.GetId(_startTimeGuid);
                _dbHelper.Sql_Insert_FoundLinks(link.Value, link.Key, "href", scanid);
                Console.WriteLine(link.Key + " - " + GetResponseCode(new Uri(link.Key)));
                //Uri new_link = new Uri(link.Key);
                //Crawl(new_link);
                //Console.WriteLine(new_link);
            }
            Console.WriteLine("done");
            Console.ReadLine();
        }

        public int GetResponseCode(Uri thisUrl)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(thisUrl);
            webRequest.AllowAutoRedirect = false;
            webRequest.Timeout = 2000;
            try
            {
                var resp = (HttpWebResponse)webRequest.GetResponse();
                return (int)resp.StatusCode;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    if (((HttpWebResponse) ex.Response).StatusCode == HttpStatusCode.NotFound)
                    {
                        // handle the 404 here
                        return 404;
                    }
                    return (int) ex.Status;
                }
                else if (ex.Status == WebExceptionStatus.NameResolutionFailure)
                {
                    return (int) ex.Status;
                }
            }
            return -1;
        }
        public void GetLinksFromWebsite(ref HashSet<string> hrefHash, Uri thisUrl, string htmlSource)
        {
            Match m;
            //var tempHash = new HashSet<string>();
            var listReturn = new List<string>();
            const string HRefPattern = "href\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))";
            // Another change
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
                lock (hrefHash) {
                    foreach (var item in listReturn)
                    {
                        if ((item.Contains("#"))
                            || (!item.Contains("spsu.edu"))
                            || (item.Contains(".xml"))
                            || (item.Contains("omniupdate"))
                            || (item.Contains("go.view.usg.edu"))
                            || (item.Contains("mailto"))
                            || (item.Contains("file"))
                            //|| (item.Contains("zimbra"))
                            || (item.Equals(""))) continue;
                        //if (temp.Contains(item)) continue;
                        var temp = new Uri(thisUrl, item);
                        var line = temp.AbsoluteUri;
                        hrefHash.Add(line);
                        //respCode = GetResponseCode(new Uri(line));
                        //Console.WriteLine(thisUrl + "\n -- " + line);
                    }
                }
            }
            catch (RegexMatchTimeoutException)
            {
                Console.WriteLine("The matching operation timed out.");
            }   
        }
        public void GetLinksFromImages(ref HashSet<string> srcHash, Uri thisUrl, string imgSource)
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
                lock (srcHash)
                {
                    foreach(var item in listReturn)
                    {
                        var temp = new Uri(thisUrl, item);
                        var line = temp.AbsoluteUri;
                        srcHash.Add(line);
                    }
                }
            }
            catch (RegexMatchTimeoutException)
            {
                Console.WriteLine("The matching operation timed out.");
            }
        }

    }
}
