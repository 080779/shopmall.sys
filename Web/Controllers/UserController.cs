using IMS.Common;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace IMS.Web.Controllers
{
    public class UserController : ApiController
    {
        public IUserService userService { get; set; }
        public IIdNameService idNameService { get; set; }
        [HttpPost]
        public async Task<ApiResult> Register(string mobile,string password,string recommendMobile,string code)
        {
            if(string.IsNullOrEmpty(mobile))
            {
                return new ApiResult { status = 0, msg = "注册手机号不能为空" };
            }
            if((await userService.UserCheck(mobile))>0)
            {
                return new ApiResult { status = 0, msg = "注册手机号已经存在" };
            }
            if (string.IsNullOrEmpty(password))
            {
                return new ApiResult { status = 0, msg = "密码不能为空" };
            }
            if (string.IsNullOrEmpty(recommendMobile))
            {
                return new ApiResult { status = 0, msg = "推荐人手机号不能为空" };
            }
            if ((await userService.UserCheck(recommendMobile)) == -1)
            {
                return new ApiResult { status = 0, msg = "推荐人不存在" };
            }
            //if (string.IsNullOrEmpty(code))
            //{
            //    return new ApiResult { status = 0, msg = "短信验证码不能为空" };
            //}
            long levelId= await idNameService.GetIdByNameAsync("会员等级");
            long id= await userService.AddAsync(mobile, password, levelId);
            if(id<=0)
            {
                return new ApiResult { status = 0, msg = "注册失败" };
            }
            long addRecommendId= await userService.AddRecommendAsync(id, recommendMobile);
            return new ApiResult { status=1,msg="注册成功" };
        } 
    }    
}