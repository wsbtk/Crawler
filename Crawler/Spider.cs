using System;
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
        private readonly MySqlConnectionStringBuilder _connString;
        private DateTime _startTime;
        private string _startTimeGuid;

        public Spider(bool go)
        {
            flag = go;
        }

        public void Crawl(Uri thisUrl)
        {
            //var capture = new HashSet<string>();
            var dict1 = new Dictionary<string, string>();
            var client = new WebClient();
            //var thisUrl = new Uri("http://www.spsu.edu");
            var htmlSource = client.DownloadString(thisUrl);

            //var respCode = GetResponseCode(thisUrl);
            //Console.WriteLine(thisUrl + " -- " + respCode);
            //if (respCode == 200)
            //{
            var returnedLinks = GetLinksFromWebsite(htmlSource);
            if (returnedLinks != null) {
                foreach (var item in returnedLinks)
                {
                    if ((item.Contains("#"))
                        || (!item.Contains("spsu.edu"))
                        || (item.Contains(".xml")) 
                        || (item.Contains("omniupdate"))
                        || (item.Contains("mailto")) 
                        || (item.Equals(""))) continue;
                        //|| (item.Contains("go.view.usg.edu")) 
                    var temp = new Uri(thisUrl, item);
                    var line = temp.AbsoluteUri;
                    if (dict1.ContainsKey(line)) continue;
                    dict1.Add(line,thisUrl.ToString());
                    captured++;
                    //respCode = GetResponseCode(new Uri(line));
                    //Console.WriteLine(thisUrl + "\n -- " + line);
                }
            }
            //}
            Console.WriteLine("Dictionary Keys - " + dict1.Keys.Count);
            Console.WriteLine("Dictionary Values - " + dict1.Values.Count);
            Console.WriteLine("Dictionary Values (distinct) - " + dict1.Values.Distinct().Count());
            Console.WriteLine(captured);
            foreach (var link in dict1)
            {
                Console.WriteLine(link.Key + " - " + GetResponseCode(new Uri(link.Key)));
            }
            Console.WriteLine("done");
            Console.ReadLine();
        }

        public static int GetResponseCode(Uri thisUrl)
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
        public void SqlInsert(string parent, string uniqueUrl, string urlType, int scanId)
        {
            var conn = new MySqlConnection();
            var cmd = new MySqlCommand();
            conn.ConnectionString = _connString.ToString();
            try
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO `ForagerAdmin`.`Found_Links` VALUES(NULL, @Scan_ID, @Parent, @URL, @URL_type)";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@Scan_ID", scanId);
                cmd.Parameters.AddWithValue("@Parent", parent);
                cmd.Parameters.AddWithValue("@URL", uniqueUrl);
                cmd.Parameters.AddWithValue("@URL_type", urlType);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error " + ex.Number + " has occurred: " + ex.Message, "Error");
            }
        }

    }
}
