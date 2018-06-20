using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("192.168.1.238:6379"))
            {
                IDatabase db = redis.GetDatabase();
                Console.WriteLine(db.StringGet("name").IsNullOrEmpty);
                db.KeyExpire("name", DateTime.Now.AddSeconds(10), CommandFlags.None);
                Console.ReadKey();
            }
            Console.WriteLine(DateTime.Now.Date.AddSeconds(-1));
            Console.ReadKey();
        }
    }
}
