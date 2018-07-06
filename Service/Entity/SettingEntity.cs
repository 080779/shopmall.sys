using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    public class SettingEntity:BaseEntity
    {
        public string Name { get; set; }
        public string Parm { get; set; }
        public string Description { get; set; }
        public long SettingTypeId { get; set; }
        public virtual IdNameEntity SettingType { get; set; }
    }
}
