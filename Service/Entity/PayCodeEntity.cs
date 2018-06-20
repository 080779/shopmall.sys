using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 付款码实体类
    /// </summary>
    public class PayCodeEntity : BaseEntity
    {
        public string Name { get; set; }
        public string CodeUrl { get; set; }
        public string Description { get; set; }
    }
}
