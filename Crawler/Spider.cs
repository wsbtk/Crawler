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
<<<<<<< HEAD
using MySql.Data;

=======
using MySql.Data.MySqlClient;
>>>>>>> de5493246401d4e665e3c73f60c326702ac68953


    class Spider
    {
        public HashSet<string> capture = new HashSet<string>();
        //public ConcurrentDictionary<string, string> dictionary = new ConcurrentDictionary<string, string>();
        public int position = 0;
        public int scanned = 0;
        public bool flag;
        public DateTime beginTime;
        public DateTime endTime;

        public Uri Url { get; set; }

        public Spider(Uri urlString)
        {
            Url = urlString;
            flag = true;
        }

<<<<<<< HEAD
        private void MySql_Conx()
        {
          MySql.Data.MySqlClient.MySqlConnection conn;
            string myConnectionString;
            
            //Variables for connecting to your database.
            //These variable values come from your hosting account.
            var hostname = "ForagerAdmin.db.10586941.hostedresource.com";
            var username = "ForagerAdmin";
            var dbname = "ForagerAdmin";
            var pass = "Te@mQu4tro";
            var tb1 = "Forager_User";

            //myConnectionString = 
            //    "server=127.0.0.1;uid=root;" +
            //    "pwd=12345;database=test;";
            myConnectionString = 
                "server=" + hostname + ";" +
                "uid=" + username +";" +
                "pwd=" + pass + ";" +
                "database=" + dbname +";";

            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = myConnectionString;
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }
=======
	public void MySQL_Conx() {
		//		conn_string.Server = "mysql7.000webhost.com";
		//		conn_string.UserID = "a455555_test";
		//		conn_string.Password = "a455555_me";
		//		conn_string.Database = "xxxxxxxx";

		MySqlConnectionStringBuilder conn_string = new MySqlConnectionStringBuilder();
		conn_string.Server = "ForagerAdmin.db.10586941.hostedresource.com";
		conn_string.Password = "Te@mQu4tro";
		conn_string.UserID = "ForagerAdmin";
		conn_string.Database = "ForagerAdmin";

		using (MySqlConnection conn = new MySqlConnection(conn_string.ToString()))
		using (MySqlCommand cmd = conn.CreateCommand())
		{    //watch out for this SQL injection vulnerability below
			cmd.CommandText = string.Format("INSERT Test (lat, long) VALUES ({0},{1})",
				OSGconv.deciLat, OSGconv.deciLon);
			connection.Open();
			cmd.ExecuteNonQuery();
		}


		myConnection.ConnectionString = myConnectionString;
		myConnection.Open();
		// execute queries, etc
		myConnection.Close();
	}
>>>>>>> de5493246401d4e665e3c73f60c326702ac68953

        public void Crawl()
        {
            beginTime = DateTime.Now;
            using (var client = new WebClient()) {
                while (flag) {
                    string htmlSource;
                    if (capture.Count == 0) {
                        htmlSource = client.DownloadString(Url);
                    }
                    else {
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
                        };
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
                                var resp = (HttpWebResponse)ex.Response;
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
