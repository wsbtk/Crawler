using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler
{
    class Program
    {
        static void Main(string[] args)
        {
            var thisUrl = new Uri("http://www.spsu.edu");
            var spider = new Spider(thisUrl, 40);

            Thread oThread = new Thread(spider.Crawl);
            oThread.Start();

            while (!oThread.IsAlive);
            Thread.Sleep(1);
            oThread.Join();
            //spider.Crawl();

        }
    }
}
