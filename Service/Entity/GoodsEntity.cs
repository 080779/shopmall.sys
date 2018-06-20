using IMS.Service;
using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 商品实体类
    /// </summary>
    public class GoodsEntity : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Standard { get; set; }//规格
        public decimal Price { get; set; }
        public decimal RealityPrice { get; set; }
        public long GoodsAreaId { get; set; }
        public GoodsAreaEntity GoodsArea { get; set; }
        public long GoodsTypeId { get; set; }
        public GoodsTypeEntity GoodsType { get; set; }
        public long GoodsSecondTypeId { get; set; }
        public GoodsSecondTypeEntity GoodsSecondType { get; set; }
        public bool IsPutaway { get; set; }//是否上架
        public bool IsRecommend { get; set; }//是否推荐
        public string Description { get; set; }
        public long GoodsImgId { get; set; }
        public GoodsImgEntity GoodsImg { get; set; }
        public long Inventory { get; set; }
        public long SaleNum { get; set; }
    }
}
