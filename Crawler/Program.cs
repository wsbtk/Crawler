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
        DatabaseHelper _dbHelper = new DatabaseHelper();

        private static void Main(string[] args)
        {
            var thisUrl = new Uri("http://www.spsu.edu/");
            var spider = new Spider(true);

            var sw = new Stopwatch();
            sw.Start();
            //while (dbHelper.CheckRunning()) {
            //  thisUrl = get new url from database;
            
            spider.Crawl(thisUrl);
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            Console.ReadLine();

            // }
            //Console.WriteLine(spider.GetId());
            Console.ReadLine();
        }
    }
}
