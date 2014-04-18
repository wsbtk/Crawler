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
    internal class Program
    {
        private static void Main(string[] args)
        {
            var thisUrl = new Uri("http://www.spsu.edu/");
            var spider = new Spider(true);
            spider.Crawl(thisUrl);
            Console.WriteLine(spider.GetId());
            Console.ReadLine();
        }
    }
}
