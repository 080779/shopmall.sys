using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.BankAccount
{
    public class BankAccountEditModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string BankAccount { get; set; }
        public string BankName { get; set; }
    }
}