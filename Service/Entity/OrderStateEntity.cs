using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 订单状态实体类
    /// </summary>
    public class OrderStateEntity : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
