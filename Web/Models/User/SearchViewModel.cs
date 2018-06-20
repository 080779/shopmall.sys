using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.User
{
    public class SearchViewModel
    {
        public string Mobile { get; set; }
        public string Code { get; set; }
        public long Integral { get; set; }
    }
}