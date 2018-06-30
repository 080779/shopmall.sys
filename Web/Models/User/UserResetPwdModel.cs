using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.User
{
    public class UserResetPwdModel
    {
        public string Password { get; set; }
        public string NewPassword { get; set; }
    }
}