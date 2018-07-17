using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IMS.Common.Pagination;

namespace IMS.Web.Areas.Admin.Models.Setting
{
    public class SettingListViewModel
    {
        public SettingModel Title { get; set; }
        public SettingModel Phone1 { get; set; }
        public SettingModel Phone2 { get; set; }
        public SettingModel Logo { get; set; }
        public SettingModel About { get; set; }
    }
    public class SettingModel
    {
        public long Id { get; set; }
        public string Parm { get; set; }
    }
}