﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Common
{
    public static class HttpClientHelper
    {
        /// <summary>
        /// get请求，可以对请求头进行多项设置
        /// </summary>
        /// <param name="paramArray"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string> GetResponseByGetAsync(HttpClient httpClient, List<KeyValuePair<string, string>> paramArray, string url)
        {
            string result = "";
            url = url + "?" + BuildParam(paramArray);
            var response = httpClient.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                Stream myResponseStream = await response.Content.ReadAsStreamAsync();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                result = await myStreamReader.ReadToEndAsync();
                myStreamReader.Close();
                myResponseStream.Close();
            }
            return result;
        }

        public static async Task<string> GetResponseByPostAsync<T>(HttpClient httpClient, T obj, string url)
        {
            string result="";
            var content = new FormUrlEncodedContent(ToKeyValue(obj));
            var response = await httpClient.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                Stream myResponseStream = await response.Content.ReadAsStreamAsync();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                result = await myStreamReader.ReadToEndAsync();
                myStreamReader.Close();
                myResponseStream.Close();
            }
            return result;
        }

        private static string Encode(string content, Encoding encode = null)
        {
            if (encode == null) return content;

            return System.Web.HttpUtility.UrlEncode(content, Encoding.UTF8);

        }

        private static string BuildParam(List<KeyValuePair<string, string>> paramArray, Encoding encode = null)
        {
            string url = "";

            if (encode == null) encode = Encoding.UTF8;

            paramArray= paramArray.OrderBy(k=>k.Key).ToList();

            if (paramArray != null && paramArray.Count > 0)
            {
                var parms = "";
                foreach (var item in paramArray)
                {
                    parms += string.Format("{0}={1}&", Encode(item.Key, encode), Encode(item.Value, encode));
                }
                if (parms != "")
                {
                    parms = parms.TrimEnd('&');
                }
                url += parms;

            }
            return url;
        }

        private static string BuildParam<T>(T obj, Encoding encode = null)
        {
            string url = "";

            if (encode == null) encode = Encoding.UTF8;
            Type type = typeof(T);
            var props = type.GetProperties();
            List<PropertyInfo> lists = props.OrderBy(p => p.Name).ToList(); ;

            if (lists != null && lists.Count > 0)
            {
                var parms = "";
                foreach (var item in lists)
                {
                    parms += string.Format("{0}={1}&", Encode(item.Name, encode), Encode(item.GetValue(obj).ToString(), encode));
                }
                if (parms != "")
                {
                    parms = parms.TrimEnd('&');
                }
                url += parms;

            }
            return url;
        }

        private static List<KeyValuePair<string,string>> ToKeyValue<T>(T obj, Encoding encode = null)
        {
            if (encode == null) encode = Encoding.UTF8;
            Type type = typeof(T);
            var props = type.GetProperties();
            List<PropertyInfo> lists = props.OrderBy(p => p.Name).ToList();
            List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();
            if (lists != null && lists.Count > 0)
            {
                foreach (var item in lists)
                {
                    values.Add(new KeyValuePair<string, string>(Encode(item.Name, encode), Encode(item.GetValue(obj).ToString(),encode)));
                }              
            }
            return values;
        }
    }
}
