using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 平台用户类型实体类
    /// </summary>
    public class PlatformUserTypeEntity:BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
