using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 银行账号实体类
    /// </summary>
    public class BankAccountEntity : BaseEntity
    {
        public string Name { get; set; }
        public string Account { get; set; }
        public string AccountName { get; set; }
        public long PayCodeId { get; set; }
        public PayCodeEntity PayCode { get; set; }
        public string Description { get; set; }
    }
}
