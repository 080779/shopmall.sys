using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.App_Start.Filter
{
    public class User
    {
        public long UserId { get; set; }
        public string Mobile { get; set; }
        public string NickName { get; set; }
        public string HeadPic { get; set; }
        public string LevelName { get; set; }
    }
}