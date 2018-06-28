﻿using IMS.Common;
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
    [AllowAnonymous]
    public class UserController : ApiController
    {
        private string TokenSecret = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";
        public IUserService userService { get; set; }
        public IUserTokenService userTokenService { get; set; }
        public IIdNameService idNameService { get; set; }
        [HttpPost]        
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
            //if (string.IsNullOrEmpty(code))
            //{
            //    return new ApiResult { status = 0, msg = "短信验证码不能为空" };
            //}
            long levelId= await idNameService.GetIdByNameAsync("会员等级");
            long id= await userService.AddAsync(model.Mobile, model.Password, levelId);
            if(id<=0)
            {
                return new ApiResult { status = 0, msg = "注册失败" };
            }
            else
            {
                long addRecommendId = await userService.AddRecommendAsync(id, model.RecommendMobile);
                if(addRecommendId<=0)
                {
                    return new ApiResult { status = 0, msg = "添加推荐人失败" };
                }
            }
            UserDTO user= await userService.GetModelAsync(id);
            User setUser = new User();
            setUser.HeadPic = user.HeadPic;
            setUser.LevelName = user.LevelName;
            setUser.Mobile = user.Mobile;
            setUser.NickName = user.NickName;
            setUser.UserId = user.Id;
            string token=JwtHelper.JwtEncrypt<User>(ControllerContext,setUser,TokenSecret);
            if(string.IsNullOrEmpty(token))
            {
                return new ApiResult { status = 0, msg = "生成token失败" };
            }
            long tokenId = await userTokenService.AddAsync(user.Id, token);
            if(tokenId<=0)
            {
                return new ApiResult { status = 0, msg = "添加token失败" };
            }
            return new ApiResult { status=1,msg="注册成功",data=new { token=token} };
        }
        [HttpPost]
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
            long userId= await userService.CheckLoginAsync(model.Mobile,model.Password);
            if(userId==-1 || userId==-2)
            {
                return new ApiResult { status = 0, msg = "登录账号或密码错误" };
            }
            UserDTO user = await userService.GetModelAsync(userId);
            User setUser = new User();
            setUser.HeadPic = user.HeadPic;
            setUser.LevelName = user.LevelName;
            setUser.Mobile = user.Mobile;
            setUser.NickName = user.NickName;
            setUser.UserId = user.Id;
            string token = JwtHelper.JwtEncrypt<User>(ControllerContext, setUser, TokenSecret);
            if (string.IsNullOrEmpty(token))
            {
                return new ApiResult { status = 0, msg = "生成token失败" };
            }
            long tokenId = await userTokenService.UpdateAsync(user.Id, token);
            if (tokenId <= 0)
            {
                return new ApiResult { status = 0, msg = "更新token失败" };
            }
            return new ApiResult { status = 1 ,msg="登录成功",data=new { token=token} };
        }
        //[HttpPost]
        //public async Task<ApiResult> Logout(UserRegisterModel model)
        //{

        //}
    }    
}