using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 提现状态实体类
    /// </summary>
    public class StateEntity:BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
