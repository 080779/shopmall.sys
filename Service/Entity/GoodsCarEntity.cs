﻿using IMS.Service;
using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 购物车实体类
    /// </summary>
    public class GoodsCarEntity : BaseEntity
    {
        public long GoodsId { get; set; }
        public GoodsEntity Goods { get; set; }
        public long Number { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
