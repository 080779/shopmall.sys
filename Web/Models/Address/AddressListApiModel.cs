﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.Address
{
    public class AddressApiModel
    {
        public long id { get; set; }
        public string name { get; set; }//收货人姓名
        public string mobile { get; set; }//收货人手机号
        public string address { get; set; }//收货人地址
        public bool isDefault { get; set; }
    }
}