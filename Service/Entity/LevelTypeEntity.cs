using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 会员等级实体类
    /// </summary>
    public class LevelTypeEntity : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
