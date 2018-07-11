using IMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Common
{
    public class WeChatPay
    {
        public string appid { get; set; }= System.Configuration.ConfigurationManager.AppSettings["APPID"];
        public string mch_id { get; set; }
        public string nonce_str { get; set; } = CommonHelper.GetCaptcha(10);
        public string sign { get; set; }
        public string sign_type { get; set; } = "MD5";
        public string body { get; set; }
        public string detail { get; set; }
        public string out_trade_no { get; set; }
        public string fee_type { get; set; } = "CNY";
        public string total_fee { get; set; }
        public string notify_url { get; set; }
        public string trade_type { get; set; } = "JSAPI";
        public string openid { get; set; }
    }
}