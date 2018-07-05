using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Areas.Admin.Models.User
{
    public class UserBonusSetModel
    {
        public List<Setting> Settings { get; set; }
    }
    public class Setting
    {
        public long Id { get; set; }
        public string TypeName { get; set; }
        public string TypeDescription { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}