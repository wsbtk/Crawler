using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler
{
    class Program
    {
		//private static BackgroundWorker _bw;// = new BackgroundWorker();
        static void Main(string[] args)
        {
            var thisUrl = new Uri("http://www.spsu.edu");
            var spider = new Spider(thisUrl);

            Parallel.Invoke(
                //20
                //spider.Crawl,
                //spider.Crawl,
                //spider.Crawl,
                //spider.Crawl,
                //spider.Crawl,
                //spider.Crawl,
                //spider.Crawl,
                //spider.Crawl,
                //spider.Crawl,
                //spider.Crawl,
                ////10
                //spider.Crawl,
                //spider.Crawl,
                //spider.Crawl,
                //spider.Crawl,
                //spider.Crawl,
                ////5
                //spider.Crawl,
                //spider.Crawl,
                //spider.Crawl,
                //spider.Crawl,
                spider.Crawl
                );
            /*  **********    Statistics (for Above)    **********  */
            /*  1 Thread(s)
             * Scanned 12114
             * 00:00:08.1030000 
             * 211
             */
            /*  2 Thread(s)
             * Scanned 12170
             * 00:00:05.7060000
             * 212
             */
            /*  5 Thread(s)
             * Scanned 12032
             * 00:00:03.3110000
             * 212
             */
            /*  10 Thread(s)
             * Scanned 12148
             * 00:00:01.3090000
             * 211
             */
            /*  **********    Statistics (end)    **********  */

            

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

        }
    }
}
