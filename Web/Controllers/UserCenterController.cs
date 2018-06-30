using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Models.User;
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
    public class UserCenterController : ApiController
    {        
        public IUserService userService { get; set; }
        public IBankAccountService bankAccountService { get; set; }
        public IPayCodeService payCodeService { get; set; }
        
        [HttpPost]
        public async Task<ApiResult> Detail()
        {
            User user= JwtHelper.JwtDecrypt<User>(ControllerContext);
            UserDTO userdto= await userService.GetModelAsync(user.UserId);
            PayCodeDTO[] payCodes = await payCodeService.GetModelByUserIdAsync(user.UserId);
            BankAccountDTO[] bankAccounts = await bankAccountService.GetModelByUserIdAsync(user.UserId);
            UserInfoApiModel model = new UserInfoApiModel();
            model.bankAccountId = bankAccounts.First().Id;
            model.qrCode = payCodes.First().CodeUrl;
            model.headPic = userdto.HeadPic;
            model.id = userdto.Id;
            model.nickName = userdto.NickName;
            return new ApiResult { status =1,data=model};
        }
        [HttpPost]
        public async Task<ApiResult> ResetPwd(UserResetPwdModel model)
        {            
            if (string.IsNullOrEmpty(model.Password))
            {
                return new ApiResult { status = 0, msg = "原密码不能为空" };
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                return new ApiResult { status = 0, msg = "新密码不能为空" };
            }
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            long id= await userService.ResetPasswordAsync(user.UserId,model.Password,model.NewPassword);
            if (id == -1)
            {
                return new ApiResult { status = 0, msg = "用户不存在" };
            }
            if (id == -2)
            {
                return new ApiResult { status = 0, msg = "原密码错误" };
            }
            return new ApiResult { status = 1, msg = "密码修改成功！" };
        }
    }    
}