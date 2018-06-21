using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class TakeCashDTO:BaseDTO
    {
        public long UserId { get; set; }
        public string UserMobile { get; set; }
        public string UserCode { get; set; }
        public long StateId { get; set; }
        public decimal? Amount { get; set; }
        public string Description { get; set; }
        public string StateName { get; set; }
    }
}
