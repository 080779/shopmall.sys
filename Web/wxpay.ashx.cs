using IMS.Common;
using IMS.IService;
using IMS.Service.Service;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace IMS.Web
{
    /// <summary>
    /// wxpay 的摘要说明
    /// </summary>
    public class wxpay : IHttpHandler
    {
        private static ILog log = LogManager.GetLogger(typeof(wxpay));
        private IUserService userService = new UserService();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            StreamReader reader = new StreamReader(context.Request.InputStream);
            string xmlData = reader.ReadToEnd();

            log.DebugFormat("微信支付回调：{0}", xmlData);
            StringBuilder fail = new StringBuilder();
            fail.AppendLine("<xml>");
            fail.AppendLine("<return_code><![CDATA[FAIL]]></return_code>");
            fail.AppendLine("<return_msg></return_msg>");
            fail.AppendLine("<xml>");
            StringBuilder success = new StringBuilder();
            success.AppendLine("<xml>");
            success.AppendLine("<return_code><![CDATA[SUCCESS]]></return_code>");
            success.AppendLine("<return_msg><![CDATA[OK]]></return_msg>");
            success.AppendLine("<xml>");

            if (!xmlData.Contains("SUCCESS"))
            {
                context.Response.Write(fail.ToString());
            }
            else
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlData);
                XmlNode orderCode = xmlDoc.SelectSingleNode("xml/out_trade_no");

                long id = userService.WeChatPay(orderCode.InnerText);
                log.DebugFormat("支付后表操作：{0}", id);
                if(id<=0)
                {
                    if(id==-4)
                    {
                        context.Response.Write(success.ToString());
                    }
                    context.Response.Write(fail.ToString());
                }
                else
                {
                    context.Response.Write(success.ToString());
                }
            }            
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}