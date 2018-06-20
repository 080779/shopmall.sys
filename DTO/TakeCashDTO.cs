using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class TakeCashDTO:BaseDTO
    {
        public long PlatformUserId { get; set; }
        public long IntegralTypeId { get; set; }
        public long StateId { get; set; }
        public long? Integral { get; set; }
        public decimal? Amount { get; set; }
        public string Description { get; set; }
        public string PlatformUserMobile { get; set; }
        public string IntegralTypeName { get; set; }
        public string StateName { get; set; }
    }
}
