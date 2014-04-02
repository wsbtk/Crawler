using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using System.Collections;
using System.Threading;
using System.Data.SqlClient;
using System.IO;

class Program
{
    public static HashSet<string> capture = new HashSet<string>();
    public static int position = 0;
    public static bool flag = true;
    public static int counter = 0;
    public static String connection = "server=68.178.143.42;user id=ForagerAdmin;password=Te@mQu4tro;persistsecurityinfo=True;database=ForagerAdmin";
    SqlConnection connect = new SqlConnection(connection);

    static void Main()
    {
        using (var client = new WebClient())
        {
            while (flag)
            {
                var htmlSource = "";
                if (capture.Count == 0)
                {                    
                    htmlSource = client.DownloadString("http://www.spsu.edu");
                }
                //var htmlSource = client.DownloadString("http://www.spsu.edu");
                else
                {
                    System.Console.WriteLine("hello" + position);
                    htmlSource = client.DownloadString(capture.ElementAt<string>(position));
                    //System.Console.WriteLine(htmlSource);
                    position++;

                }
                foreach (var item in GetLinksFromWebsite(htmlSource))
                {
                    string test = item.ToString();
                    ////if (test.Contains(".xml") == true)
                    //{
                    //    //System.Console.Write("We found an XML");
                    //}
                    ////else if (test.Contains(".pcf") == true)
                    ////{

                    ////}
                    //else
                    if (test.Contains("spsu.edu") == true)
                    {
                        char first_char = (item[0]);
                        string line;

                        if (first_char.Equals('/'))
                        {
                            line = ("http://www.spsu.edu" + item);
                        }
                        else if (first_char.Equals('h'))
                        {
                            line = (item);
                        }
                        else
                        {
                            line = ("http://www.spsu.edu/" + item);
                        }
                        capture.Add(line);
                        Console.WriteLine(line);
                    }
                }
                //System.Console.WriteLine(counter);
                System.Console.WriteLine("Testing the Value: " + capture.ElementAt<string>(position));
                flag = true;
            }
        }
        Console.ReadKey();
    }

    public static List<String> GetLinksFromWebsite(string htmlSource)
    {
        //try
        //{
            var doc = new HtmlDocument();

            //Thread main_thread = new Thread(new ThreadStart(HtmlDocument);
            
            doc.LoadHtml(htmlSource);
            //main_thread.Start();

            //HtmlNodeCollection refs = doc.DocumentNode.SelectNodes("//a[@ref]");
            // var refs = doc.DocumentNode.SelectNodes("//a[@ref]");
            //.SelectNodes("//li");
            //string val = doc.DocumentNode.SelectNodes;
            //System.Console.WriteLine("Checking HTML:" + htmlSource);
            ////if(doc == null)
            ////    {
            //if ()
            //{
            //    d
            //    return doc
            //    .DocumentNode
            //    .SelectNodes("//a[@ref]")
            //    .Select(node => node.Attributes["ref"].Value)
            //    .ToList();
            //}
            //else
            //{
            // <link>Link</link>
            //System.Console.WriteLine(htmlSource);
            if (doc.DocumentNode.SelectNodes("//a[@ref]") == null)
            {
                //counter++;
                return doc
                   .DocumentNode
                   .SelectNodes("//a[@href]")
                   .Select(node => node.Attributes["href"].Value)
                   .ToList();
            }
            //else if (doc.DocumentNode.SelectNodes("//") != null)
            //{
            //        ;
            //}
            else
            {
                //counter++;
                return doc
                   .DocumentNode
                   .SelectNodes("//a[@ref]")
                   .Select(node => node.Attributes["ref"].Value)
                   .ToList();
            }
        //}
        //catch (InvalidOperationException e)
        //{
          //  if(e is WebException)
          //  {
            //    if(e.Message.Contains("404"))
              //  {
                    
                   // handle 404;
                //}
            //}
        //}
    }
}