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
    public class UserController : ApiController
    {
        private string appid = System.Configuration.ConfigurationManager.AppSettings["APPID"];
        private string secret = System.Configuration.ConfigurationManager.AppSettings["SECRET"];
        public IUserService userService { get; set; }
        public IUserTokenService userTokenService { get; set; }
        public IIdNameService idNameService { get; set; }
        public IMessageService messageService { get; set; }
        public IBankAccountService bankAccountService { get; set; }
        public IPayCodeService payCodeService { get; set; }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResult> Register(UserRegisterModel model)
        {
            if(string.IsNullOrEmpty(model.Mobile))
            {
                return new ApiResult { status = 0, msg = "注册手机号不能为空" };
            }
            if(!Regex.IsMatch(model.Mobile, @"^1\d{10}$"))
            {
                return new ApiResult { status = 0, msg = "注册手机号格式不正确" };
            }
            if ((await userService.UserCheck(model.Mobile))>0)
            {
                return new ApiResult { status = 0, msg = "注册手机号已经存在" };
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                return new ApiResult { status = 0, msg = "密码不能为空" };
            }
            if (string.IsNullOrEmpty(model.RecommendMobile))
            {
                return new ApiResult { status = 0, msg = "推荐人手机号不能为空" };
            }
            if ((await userService.UserCheck(model.RecommendMobile)) == -1)
            {
                return new ApiResult { status = 0, msg = "推荐人不存在" };
            }
            if (string.IsNullOrEmpty(model.Code))
            {
                return new ApiResult { status = 0, msg = "短信验证码不能为空" };
            }
            object obj= CacheHelper.GetCache("App_User_SendMsg" + model.Mobile);
            if(obj==null)
            {
                return new ApiResult { status = 0, msg = "注册手机号不一致" };
            }
            if(obj.ToString()!=model.Code)
            {
                return new ApiResult { status = 0, msg = "手机验证码错误" };
            }
            long levelId= await idNameService.GetIdByNameAsync("普通会员");
            long id= await userService.AddAsync(model.Mobile, model.Password, levelId,model.RecommendMobile);
            if(id<=0)
            {
                return new ApiResult { status = 0, msg = "注册失败" };
            }
            User setUser = new User();
            setUser.Id = id;
            setUser.Code = "";
            string token = JwtHelper.JwtEncrypt<User>(setUser);
            //if (string.IsNullOrEmpty(token))
            //{
            //    return new ApiResult { status = 0, msg = "生成token失败" };
            //}
            long tokenId = await userTokenService.AddAsync(id, token);
            //if (tokenId <= 0)
            //{
            //    return new ApiResult { status = 0, msg = "添加token失败" };
            //}
            return new ApiResult { status = 1, msg = "注册成功" };
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResult> Login(UserLoginModel model)
        {
            if (string.IsNullOrEmpty(model.Mobile))
            {
                return new ApiResult { status = 0, msg = "登录手机号不能为空" };
            }
            if (!Regex.IsMatch(model.Mobile, @"^1\d{10}$"))
            {
                return new ApiResult { status = 0, msg = "登录手机号格式不正确" };
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                return new ApiResult { status = 0, msg = "密码不能为空" };
            }
            if (string.IsNullOrEmpty(model.Code))
            {
                return new ApiResult { status = 0, msg = "微信code不能为空" };
            }
            long userId= await userService.CheckLoginAsync(model.Mobile,model.Password);
            if(userId==-1 || userId==-2)
            {
                return new ApiResult { status = 0, msg = "登录账号或密码错误" };
            }
            HttpClient httpClient=new HttpClient();
            List<KeyValuePair<string, string>> parmArray=new List<KeyValuePair<string, string>>();
            parmArray.Add(new KeyValuePair<string, string>("appid", appid));
            parmArray.Add(new KeyValuePair<string, string>("secret", secret));
            parmArray.Add(new KeyValuePair<string, string>("js_code", model.Code));
            string result= await HttpClientHelper.GetResponseByGetAsync(httpClient, parmArray, "https://api.weixin.qq.com/sns/jscode2session");
            if (result.Contains("errcode"))
            {
                WeChatErrorResultModel errorModel= JsonConvert.DeserializeObject<WeChatErrorResultModel>(result);
                return new ApiResult { status = 0, msg = "微信返回errcode：" + errorModel.errcode+ ",errmsg:" + errorModel.errmsg };
            }
            WeChatResultModel rightModel= JsonConvert.DeserializeObject<WeChatResultModel>(result);
            User setUser = new User();
            setUser.Id = userId;
            if(string.IsNullOrEmpty(rightModel.OpenId))
            {
                setUser.Code = "";
            }
            setUser.Code = CommonHelper.GetCaptcha(3) + rightModel.OpenId + CommonHelper.GetCaptcha(2);
            string token = JwtHelper.JwtEncrypt<User>(setUser);
            long tokenId = await userTokenService.UpdateAsync(userId, token);
            return new ApiResult { status = 1, msg = "登录成功", data = new { token = token } };
        }
        [HttpPost]
        public async Task<ApiResult> CodeLogin(UserLoginModel1 model)
        {
            if (string.IsNullOrEmpty(model.Code))
            {
                return new ApiResult { status = 0, msg = "微信code不能为空" };
            }
            HttpClient httpClient = new HttpClient();
            List<KeyValuePair<string, string>> parmArray = new List<KeyValuePair<string, string>>();
            parmArray.Add(new KeyValuePair<string, string>("appid", appid));
            parmArray.Add(new KeyValuePair<string, string>("secret", secret));
            parmArray.Add(new KeyValuePair<string, string>("js_code", model.Code));
            string result = await HttpClientHelper.GetResponseByGetAsync(httpClient, parmArray, "https://api.weixin.qq.com/sns/jscode2session");
            if (result.Contains("errcode"))
            {
                WeChatErrorResultModel errorModel = JsonConvert.DeserializeObject<WeChatErrorResultModel>(result);
                return new ApiResult { status = 0, msg = "微信返回errcode：" + errorModel.errcode + ",errmsg:" + errorModel.errmsg };
            }
            WeChatResultModel rightModel = JsonConvert.DeserializeObject<WeChatResultModel>(result);
            User user= JwtHelper.JwtDecrypt<User>(ControllerContext);
            if (rightModel.OpenId != user.Code.Substring(3, 30))
            {
                return new ApiResult { status = 0, msg = "登录失败" };
            }
            return new ApiResult { status = 1, msg = "登录成功" };
        }
        //[HttpPost]
        //public async Task<ApiResult> Logout()
        //{

        //}
        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResult> SendMsg(UserSendMsgModel model)
        {
            if (string.IsNullOrEmpty(model.Mobile))
            {
                return new ApiResult { status = 0, msg = "注册手机号不能为空" };
            }
            if (!Regex.IsMatch(model.Mobile, @"^1\d{10}$"))
            {
                return new ApiResult { status = 0, msg = "注册手机号格式不正确" };
            }
            string state;
            string msgState;
            string code = CommonHelper.GetNumberCaptcha(4);            
            string content = string.Format(System.Configuration.ConfigurationManager.AppSettings["SMS_Template1"], code);
            string stateCode = CommonHelper.SendMessage2(model.Mobile, content, out state, out msgState);
            await messageService.AddAsync(0, model.Mobile, content + "," + msgState, Convert.ToInt32(state));
            //UserSendMsgCacheModel cacheModel = new UserSendMsgCacheModel();
            CacheHelper.SetCache("App_User_SendMsg" + model.Mobile, code, DateTime.UtcNow.AddMinutes(2), TimeSpan.Zero);
            if(stateCode!="0")
            {
                return new ApiResult { status = 0, msg = "发送短信返回消息：" + msgState };
            }
            return new ApiResult { status = 1, msg = "发送短信返回消息："+msgState };
        }        
    }    
}