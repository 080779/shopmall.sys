using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class JournalDTO:BaseDTO
    {
        public long PlatformUserId { get; set; }
        public long FromPlatformUserId { get; set; }
        public long ToPlatformUserId { get; set; }
        public long JournalTypeId { get; set; }
        public long IntegralTypeId { get; set; }
        public string Journal01 { get; set; }
        public string Description { get; set; }
        public long? InIntegral { get; set; }
        public long? OutIntegral { get; set; }
        public long? Integral { get; set; }
        public decimal? Amount { get; set; }
        public string JournalTypeName { get; set; }
        public string IntegralTypeName { get; set; }
        public string ToIntegralTypeName { get; set; }
        public string PlatformUserMobile { get; set; }
        public string ToPlatformUserMobile { get; set; }
        public string ToPlatformUserCode { get; set; }
        public string FromPlatformUserMobile { get; set; }
        public string FormPlatformUserCode { get; set; }
        public string Tip { get; set; }
    }
}
