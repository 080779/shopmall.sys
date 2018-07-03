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
    /// 订单列表实体类
    /// </summary>
    public class OrderListEntity : BaseEntity
    {
        public long GoodsId { get; set; }
        public virtual GoodsEntity Goods { get; set; }
        public long OrderId { get; set; }
        public virtual OrderEntity Order { get; set; }
        public long Number { get; set; }
        public decimal Price { get; set; }
        public decimal PostFee { get; set; } = 0;//邮费
        public decimal Poundage { get; set; } = 0;//手续费
        public decimal TotalFee { get; set; }
        public string ImgUrl { get; set; }
    }
}
