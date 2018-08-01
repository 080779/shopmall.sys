using JWT;
using JWT.Algorithms;
using JWT.Serializers;
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
            //using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("192.168.1.238:6379"))
            //{
            //    IDatabase db = redis.GetDatabase();
            //    Console.WriteLine(db.StringGet("name").IsNullOrEmpty);
            //    db.KeyExpire("name", DateTime.Now.AddSeconds(10), CommandFlags.None);
            //    Console.ReadKey();
            //}
            //Console.WriteLine(DateTime.Now.Date.AddSeconds(-1));

            //var payload = new Dictionary<string, object>{
            //    { "UserId", 123 },
            //    { "UserName", "admin" }};
            //var secret = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";//不要泄露
            //IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            //IJsonSerializer serializer = new JsonNetSerializer();
            //IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            //IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            //var token = encoder.Encode(payload, secret);
            //Console.WriteLine(token);

            //var token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJVc2VySWQiOjEyMywiVXNlck5hbWUiOiJhZG1pbiJ9.Qjw1epD5P6p4Yy2yju3-fkq28PddznqRj3ESfALQy_U";
            //var secret = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";
            //try
            //{
            //    IJsonSerializer serializer = new JsonNetSerializer();
            //    IDateTimeProvider provider = new UtcDateTimeProvider();
            //    IJwtValidator validator = new JwtValidator(serializer, provider);
            //    IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            //    IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);
            //    var json = decoder.Decode(token, secret, verify: true);
            //    Console.WriteLine(json);
            //}
            //catch (TokenExpiredException)
            //{
            //    Console.WriteLine("Token has expired");
            //}
            //catch (SignatureVerificationException)
            //{
            //    Console.WriteLine("Token has invalid signature");
            //}
            long amount = 8800;
            int number = 6;
            long[] amounts = new long[number];
            Random rand = new Random();
            int i1 = rand.Next(0, number);
            long avg = amount / number;
            long yue = amount % number;
            if (yue != 0)
            {
                amounts[i1] = yue;
            }
            for (int i = 0; i < number; i++)
            {
                amounts[i] = amounts[i] + avg;
            }
            
            for (int i = 0; i < amount*3; i++)
            {
                int r1 = rand.Next(0, (int)(avg / 4));
                int ii1 = rand.Next(0, number);
                int ii2 = rand.Next(0, number);
                amounts[ii1] = amounts[ii1] - r1;
                amounts[ii2] = amounts[ii2] + r1;
            }
            foreach(var a in amounts)
            {
                Console.WriteLine(a+",");
            }
            Console.ReadKey();
        }
    }
}
