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
        public long Parm { get; set; }
    }
}