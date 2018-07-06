using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class SettingDTO : BaseDTO
    {
        public string TypeName { get; set; }
        public string TypeDescription { get; set; }
        public string Name { get; set; }
        public string Parm { get; set; }
        public string Description { get; set; }
    }
}
