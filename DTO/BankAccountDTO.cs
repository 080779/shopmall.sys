using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class BankAccountDTO : BaseDTO
    {
        public string Name { get; set; }
        public string Account { get; set; }
        public string AccountName { get; set; }
        public long PayCodeId { get; set; }
        public string PayCodeName { get; set; }
        public string Description { get; set; }
    }
}
