using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.Login
{
    public class LoginViewModel
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public bool Checked { get; set; }
    }
}