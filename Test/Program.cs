﻿using IMS.Service;
using IMS.Service.Entity;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main1(string[] args)
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


            //long amount = 8800;
            //int number = 6;
            //long[] amounts = new long[number];
            //Random rand = new Random();
            //int i1 = rand.Next(0, number);
            //long avg = amount / number;
            //long yue = amount % number;
            //if (yue != 0)
            //{
            //    amounts[i1] = yue;
            //}
            //for (int i = 0; i < number; i++)
            //{
            //    amounts[i] = amounts[i] + avg;
            //}

            //for (int i = 0; i < amount*3; i++)
            //{
            //    int r1 = rand.Next(0, (int)(avg / 4));
            //    int ii1 = rand.Next(0, number);
            //    int ii2 = rand.Next(0, number);
            //    amounts[ii1] = amounts[ii1] - r1;
            //    amounts[ii2] = amounts[ii2] + r1;
            //}
            //foreach(var a in amounts)
            //{
            //    Console.WriteLine(a+",");
            //}

            decimal a = (decimal)46.28841;
            string result = a.ToString("#0.00");
            Console.WriteLine(result);
            Console.ReadKey();
        }
        static void Main2(string[] args)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                dbc.Database.Log = (sql) =>
                {
                    Console.WriteLine(sql);
                };

                long stateId = dbc.GetId<IdNameEntity>(i => i.Name == "已完成");
                string val = dbc.GetParameter<SettingEntity>(s => s.Name == "自动确认收货时间", s => s.Parm);
                double day;
                double.TryParse(val, out day);
                if (day == 0)
                {
                    day = 7;
                }
                //Expression<Func<OrderEntity, bool>> timewhere = r => r.ConsignTime == null ? false : r.ConsignTime.Value.AddDays(Convert.ToDouble(val)) < DateTime.Now;
                //var orders = dbc.GetAll<OrderEntity>().Where(r => r.OrderState.Name == "已发货").Where(timewhere.Compile()).ToList();
                var orders = dbc.GetAll<OrderEntity>().Where(r => r.OrderState.Name == "已发货").Where(r => SqlFunctions.DateAdd("day", day, r.ConsignTime) < DateTime.Now);
                foreach (OrderEntity order in orders)
                {
                    order.EndTime = DateTime.Now;
                    order.OrderStateId = stateId;
                }
                val = dbc.GetParameter<SettingEntity>(s => s.Name == "不能退货时间", s => s.Parm);
                double.TryParse(val, out day);
                if (day == 0)
                {
                    day = 3;
                }
                //timewhere = r => r.EndTime == null ? false : r.EndTime.Value.AddDays(Convert.ToDouble(val)) < DateTime.Now;
                //orders = dbc.GetAll<OrderEntity>().Where(r => r.OrderState.Name == "已完成" || r.OrderState.Name == "退货审核").Where(timewhere.Compile()).ToList();
                var orders1 = dbc.GetAll<OrderEntity>().Where(r => r.CloseTime == null).Where(r => r.OrderState.Name == "已完成" || r.OrderState.Name == "退货审核").Where(r => SqlFunctions.DateAdd("day", day, r.EndTime) < DateTime.Now);
                List<string> orderCodes = new List<string>();
                foreach (OrderEntity order in orders1)
                {
                    order.OrderStateId = stateId;
                    order.CloseTime = DateTime.Now;
                    orderCodes.Add(order.Code);
                }
                //UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u=>u.Id==order.BuyerId);
                var journals = dbc.GetAll<JournalEntity>().Where(j => orderCodes.Contains(j.OrderCode) && j.JournalType.Name == "佣金收入" && j.IsEnabled == false);
                Dictionary<long, long> dicts = new Dictionary<long, long>();
                foreach (JournalEntity journal in journals)
                {
                    dicts.Add(journal.Id, journal.UserId);
                }
                foreach (var dict in dicts)
                {
                    //UserEntity user = dbc.GetAll<UserEntity>().SingleOrDefault(u => u.Id == journal.UserId);
                    var user = dbc.GetAll<UserEntity>().SingleOrDefault(u=>u.Id==dict.Value);
                    JournalEntity journal = dbc.GetAll<JournalEntity>().SingleOrDefault(j=>j.Id==dict.Key);
                    user.Amount = user.Amount + journal.InAmount.Value;
                    user.FrozenAmount = user.FrozenAmount - journal.InAmount.Value;
                    user.BonusAmount = user.BonusAmount + journal.InAmount.Value;
                    journal.BalanceAmount = user.Amount;
                    journal.IsEnabled = true;
                }

                dbc.SaveChanges();
            }
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                dbc.Database.Log = (sql) =>
                {
                    Console.WriteLine(sql);
                };

                RecommendEntity recommend = dbc.GetAll<RecommendEntity>().AsNoTracking().Where(u => u.IsNull == false).SingleOrDefault(r => r.UserId == 2);
                if (recommend == null)
                {
                    return;
                }
                var recommends = dbc.GetAll<RecommendEntity>().AsNoTracking().Where(u => u.IsNull == false);

                if (recommend.RecommendMobile == "superhero" && recommend.RecommendGenera == 1)
                {
                    recommends = recommends.Where(a => a.RecommendId == 2 ||
                 (a.RecommendPath.Contains("2" + "-") && a.RecommendGenera == recommend.RecommendGenera + 2) ||
                 (a.RecommendPath.Contains("2" + "-") && a.RecommendGenera == recommend.RecommendGenera + 3));
                }
                else
                {
                    recommends = recommends.Where(a => a.RecommendId == 2 ||
                 (a.RecommendPath.Contains("-" + "2" + "-") && a.RecommendGenera == recommend.RecommendGenera + 2) ||
                 (a.RecommendPath.Contains("-" + "2" + "-") && a.RecommendGenera == recommend.RecommendGenera + 3));
                }
                if (recommends.LongCount() <= 0)
                {
                    return;
                }
                decimal d = recommends.Sum(r => r.User.BuyAmount);
                Console.WriteLine();
            }
            Console.ReadKey();
        }
    }
}
