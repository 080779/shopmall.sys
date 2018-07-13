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
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            var res= await userService.GetModelTeamListAsync(user.Id,model.TeamLevelId,null,null,null,model.PageIndex, model.PageSize);
            var result = res.Members.Select(u => new
            {
                id = u.Id,
                mobile = u.Mobile,
                nickName = u.NickName,
                levelId = u.LevelId,
                levelName = u.LevelName,
                status = u.IsEnabled == true ? "正常" : "已冻结",
                bonusAmount=u.BonusAmount,
                amount=u.Amount,
                buyAmount=u.BuyAmount,
                recommender = u.Recommender
            });
            return new ApiResult { status = 1,data= result };
        }
        [HttpPost]
        public async Task<ApiResult> Levels()
        {
            var res= await settingService.GetModelListAsync("会员等级");
            var result = res.Select(s => new { id = Convert.ToInt32(s.Parm), name = s.Name });
            return new ApiResult { status = 1, data = result };
        }
    }    
}