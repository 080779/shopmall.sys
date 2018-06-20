using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class JournalTypeDTO:BaseDTO
    {
        public string Name { get; set; }
        public string Platform { get; set; }
        public string Merchant { get; set; }
        public string Customer { get; set; }
        public string Description { get; set; }
    }
}
