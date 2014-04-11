using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

using MySql.Data;
using MySql.Data.MySqlClient;


class Spider_old
    {
        public HashSet<string> capture = new HashSet<string>();
        //public ConcurrentDictionary<string, string> dictionary = new ConcurrentDictionary<string, string>();
        public int position = 0;
        public int scanned = 0;
        public bool flag;
        public DateTime beginTime;
        public DateTime endTime;

        public Uri Url { get; set; }

        public Spider_old(Uri urlString)
        {
            Url = urlString;
            flag = true;
        }

        public void SqlInsert(string parent, string uniqueUrl, string urlType, int scanId)
        {
		    //		conn_string.Server = "mysql7.000webhost.com";
		    //		conn_string.UserID = "a455555_test";
		    //		conn_string.Password = "a455555_me";
		    //		conn_string.Database = "xxxxxxxx";

		    var connString = new MySqlConnectionStringBuilder();
		    connString.Server = "ForagerAdmin.db.10586941.hostedresource.com";
		    connString.Password = "Te@mQu4tro";
		    connString.UserID = "ForagerAdmin";
		    connString.Database = "ForagerAdmin";
            //var sqlTable = "Found_Links";

		    var conn = new MySqlConnection();
	        var cmd = new MySqlCommand();
	        conn.ConnectionString = connString.ToString();
            try
            {
                conn.Open();
                cmd.Connection = conn;

                //cmd.CommandText = "INSERT INTO " + sqlTable +" VALUES(NULL, @number, @text)";
                //cmd.CommandText =  "INSERT INTO `ForagerAdmin`.`Found_Links` (`ID`, `Scan_ID`, `Parent`, `URL`, `URL_type`) " +
                //                   "VALUES (NULL, " + scanId + ", " + Url + ", " + uniqueUrl + ", " + urlType + ")";
                cmd.CommandText = "INSERT INTO `ForagerAdmin`.`Found_Links` VALUES(NULL, @Scan_ID, @Parent, @URL, @URL_type)";
                cmd.Prepare();

                cmd.Parameters.AddWithValue("@Scan_ID", scanId);
                cmd.Parameters.AddWithValue("@Parent", Url);
                cmd.Parameters.AddWithValue("@URL", uniqueUrl);
                cmd.Parameters.AddWithValue("@URL_type", urlType);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error " + ex.Number + " has occurred: " + ex.Message, "Error");
            }
	    }

        public void Crawl()
        {
            beginTime = DateTime.Now;
            using (var client = new WebClient()) {
                while (flag) {
                    string htmlSource;
                    if (capture.Count != 0)
                    {
                        //Console.WriteLine("hello " + position);
                        Console.WriteLine("Scanned " + scanned);
                        if (scanned > 12000)
                        {
                            endTime = DateTime.Now;

                            Console.WriteLine(endTime - beginTime);
                            var count = capture.Count();
                            Console.WriteLine(count);
                            Console.ReadLine();
                            break;
                        }
                        try
                        {
                            if (capture.ElementAt(position) == null)
                            {
                                position++;
                                continue;
                            }
                            htmlSource = client.DownloadString(capture.ElementAt(position));
                        }
                        catch (WebException ex)
                        {
                            if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
                            {
                                var resp = (HttpWebResponse) ex.Response;
                                if (resp.StatusCode == HttpStatusCode.NotFound) // HTTP 404
                                {
                                    //the page was not found, continue with next in the for loop
                                    continue;
                                }
                            }
                            //throw any other exception - this should not occur
                            throw;
                        }
                    }
                    else
                    {
                        htmlSource = client.DownloadString(Url);
                    }
                    var returnedLinks = GetLinksFromWebsite(htmlSource);
                    if (returnedLinks == null) continue;
                    foreach (var item in returnedLinks)
                    {
                        if ((item.Contains("#")) || (item.Contains(".xml")) || (item.Contains("omniupdate"))
                            || (item.Contains("go.view.usg.edu")) || (item.Contains("mailto")) || (item.Equals(""))) continue;
                        if (capture.Contains(item)) continue;
                        //if (!item.Contains("spsu.edu")) continue;
                    //
                        //var firstChar = (item[0]);
                        string line;
                        var temp = new Uri(Url, item);
                        line = temp.AbsoluteUri;
                        //if (!firstChar.Equals('/')) {
                        //    if (firstChar.Equals('h')) { line = (item); }
                        //    else { line = ("http://www.spsu.edu/" + item); }
                        //}
                        //else { line = ("http://www.spsu.edu" + item); }
                        scanned++;
                        SqlInsert(Url.ToString(), line, "href", 1);
                        capture.Add(line);
                        //Console.WriteLine(line);
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
