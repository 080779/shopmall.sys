using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    public class TakeCashEntity:BaseEntity
    {
        public long PlatformUserId { get; set; }
        public long IntegralTypeId { get; set; }
        public long StateId { get; set; }
        public long? Integral { get; set; }
        public decimal? Amount { get; set; }
        public string Description { get; set; }
        public virtual PlatformUserEntity PlatformUser { get; set; }
        public virtual IntegralTypeEntity IntegralType { get; set; }
        public virtual StateEntity State { get; set; }
    }
}
