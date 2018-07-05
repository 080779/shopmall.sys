using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IMS.Common.Pagination;

namespace IMS.Web.Areas.Admin.Models.User
{
    public class UserListViewModel
    {
        public UserDTO[] Users { get; set; }
        public long PageCount { get; set; }
        public IdNameDTO[] Levels { get; set; }
        public IdNameDTO[] SettingTypes { get; set; }
        public List<SettingDTO> Settings { get; set; }
    }
}