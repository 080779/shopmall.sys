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
        public string HeadPic { get; set; }
        public decimal Amount { get; set; }
        public decimal FrozenAmount { get; set; }
        public decimal BonusAmount { get; set; }
        public decimal BuyAmount { get; set; }
        public long LevelId { get; set; }
        public string LevelName { get; set; }
        public string Description { get; set; }
        public string ShareCode { get; set; }
        public int ErrorCount { get; set; }
        public DateTime ErrorTime { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsReturned { get; set; } //是否退过货
        public bool IsUpgraded { get; set; } //是否升级退过货
        public string Recommender { get; set; }
    }
}
