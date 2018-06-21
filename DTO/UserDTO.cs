using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class UserDTO : BaseDTO
    {
        public string Mobile { get; set; }
        public string Code { get; set; }
        public string NickName { get; set; }
        public decimal Amount { get; set; }
        public long LevelId { get; set; }
        public string LevelName { get; set; }
        public long RecommendId { get; set; }
        public string RecommendMobile { get; set; }
        public string RecommendCode { get; set; }
        public long? BankAccountId { get; set; }
        public string Description { get; set; }
        public int ErrorCount { get; set; }
        public DateTime ErrorTime { get; set; }
        public bool IsEnabled { get; set; }
    }
}
