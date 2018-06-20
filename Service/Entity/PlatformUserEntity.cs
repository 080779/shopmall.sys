using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 平台实体类
    /// </summary>
    public class PlatformUserEntity : BaseEntity
    {
        public string Mobile { get; set; }
        public string Code { get; set; }
        public string AdderMobile { get; set; }
        public string Description { get; set; }
        public long PlatformIntegral { get; set; } = 0;
        public long GivingIntegral { get; set; } = 0;
        public long UseIntegral { get; set; } = 0;
        public string Salt { get; set; } = string.Empty;
        public string Password { get; set; }
        public string TradePassword { get; set; }
        public int ErrorCount { get; set; } = 0;
        public DateTime ErrorTime { get; set; } = DateTime.Now;
        public bool IsEnabled { get; set; } = true;
        public long PlatformUserTypeId { get; set; }
        public virtual PlatformUserTypeEntity PlatformUserType { get; set; }
    }
}
