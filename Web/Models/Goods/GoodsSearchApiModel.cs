﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.Goods
{
    public class GoodsSearchApiModel
    {
        public List<SearchResultModel> goods { get; set; }
        public long pageCount { get; set; }
    }
    public class SearchResultModel
    {
        public long id { get; set; }
        public string name { get; set; }
        public decimal realityPrice { get; set; }//现价
        public long saleNum { get; set; } = 0;//销售数量
    }
}