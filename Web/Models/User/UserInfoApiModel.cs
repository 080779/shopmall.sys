using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.User
{
    public class UserInfoApiModel
    {
        public long id { get; set; }
        public string nickName { get; set; }
        public string headPic { get; set; }
        public string qrCode { get; set; }
        public long bankAccountId { get; set; }
    }
}