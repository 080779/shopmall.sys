using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 流水实体类
    /// </summary>
    public class JournalEntity:BaseEntity
    {
        public long PlatformUserId { get; set; }
        public long ToPlatformUserId { get; set; }
        public long FormPlatformUserId { get; set; }
        public long JournalTypeId { get; set; }
        public long IntegralTypeId { get; set; }
        public long ToIntegralTypeId { get; set; }
        public string Journal01 { get; set; }
        public string Description { get; set; }
        public long? InIntegral { get; set; }
        public long? OutIntegral { get; set; }
        public long? Integral { get; set; }
        public decimal? Amount { get; set; }
        public string Tip { get; set; }
        public virtual JournalTypeEntity JournalType { get; set; }
        public virtual PlatformUserEntity PlatformUser { get; set; }
        public virtual PlatformUserEntity ToPlatformUser { get; set; }
        public virtual PlatformUserEntity FormPlatformUser { get; set; }
        public virtual IntegralTypeEntity ToIntegralType { get; set; }
        public virtual IntegralTypeEntity IntegralType { get; set; }
    }
}
