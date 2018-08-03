using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Models.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace IMS.Web.Controllers
{    
    public class UserTeamController : ApiController
    {
        public IUserService userService { get; set; }
        public ISettingService settingService { get; set; }
        [HttpPost]
        public async Task<ApiResult> List(UserTeamListModel model)
        {
            string parm = await settingService.GetParmByNameAsync("网站域名");
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            var res= await userService.GetModelTeamListAsync(user.Id,model.TeamLevelId,null,null,null,model.PageIndex, model.PageSize);
            UserTeamListApiModel result = new UserTeamListApiModel();
            result.members = res.Members.Select(u => new member
            {
                id = u.Id,
                mobile = u.Mobile,
                nickName = u.NickName,
                levelId = u.LevelId,
                levelName = u.LevelName,
                status = (u.IsEnabled == true ? "已启用" : "已冻结"),
                bonusAmount = u.BonusAmount,
                amount = u.Amount,
                buyAmount = u.BuyAmount + (userService.GetTeamBuyAmount(u.Id)),
                recommender = u.Recommender,
                headPic = (!string.IsNullOrEmpty(u.HeadPic) && u.HeadPic.Contains("https://")) ? u.HeadPic : parm + u.HeadPic
            }).ToList();
            result.totalCount = res.TotalCount;
            result.pageCount = res.PageCount;
            return new ApiResult { status = 1,data= result };
        }
        [HttpPost]
        public async Task<ApiResult> Levels()
        {
            var res= await settingService.GetModelListAsync("代理等级");
            string parm = await settingService.GetParmByNameAsync("第三级显示");
            if(parm=="1")
            {
                var result = res.Select(s => new { id = Convert.ToInt32(s.Parm), name = s.Name });
                return new ApiResult { status = 1, data = result };
            }
            else
            {
                var result = res.Where(r=>r.Parm!="3").Select(s => new { id = Convert.ToInt32(s.Parm), name = s.Name });
                return new ApiResult { status = 1, data = result };
            }            
        }
    }    
}