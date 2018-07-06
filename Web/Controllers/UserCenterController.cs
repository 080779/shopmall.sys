﻿using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Models.UserCenter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace IMS.Web.Controllers
{
    [AllowAnonymous]
    public class UserCenterController : ApiController
    {
        public IUserService userService { get; set; }
        public IBankAccountService bankAccountService { get; set; }
        public IPayCodeService payCodeService { get; set; }

        [HttpPost]
        public async Task<ApiResult> Detail()
        {
            //User user= JwtHelper.JwtDecrypt<User>(ControllerContext);
            User user = new User();
            user.Id = 1;
            UserDTO userdto = await userService.GetModelAsync(user.Id);
            PayCodeDTO[] payCodes = await payCodeService.GetModelByUserIdAsync(user.Id);
            BankAccountDTO[] bankAccounts = await bankAccountService.GetModelByUserIdAsync(user.Id);
            UserCenterDetailApiModel model = new UserCenterDetailApiModel();
            model.bankAccountId = bankAccounts.Count() <= 0 ? 0 : bankAccounts.First().Id;
            model.qrCode = payCodes.Count() <= 0 ? null : payCodes.First().CodeUrl;
            if (userdto != null)
            {
                model.headPic = userdto.HeadPic;
                model.nickName = userdto.NickName;
            }
            return new ApiResult { status = 1, data = model };
        }
        [HttpPost]
        public async Task<ApiResult> EditHeadPic(UserCenterEditHeadPicModel model)
        {
            if (string.IsNullOrEmpty(model.File))
            {
                return new ApiResult { status = 0, msg = "图片文件不能为空" };
            }
            if (!model.File.Contains(";base64"))
            {
                return new ApiResult { status = 0, msg = "请上传编码后的base64图片文件" };
            }
            string res;
            if(!ImageHelper.SaveBase64(model.File, out res))
            {
                return new ApiResult { status = 0, msg = res };
            }

            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            bool flag = await userService.UpdateInfoAsync(user.Id, null, res);
            if (!flag)
            {
                return new ApiResult { status = 0, msg = "头像添加修改失败" };
            }
            return new ApiResult { status = 1, msg = "头像添加修改成功" };
        }
        [HttpPost]
        public async Task<ApiResult> EditQrCode(UserCenterEditQrCodeModel model)
        {
            if (string.IsNullOrEmpty(model.File))
            {
                return new ApiResult { status = 0, msg = "图片文件不能为空" };
            }
            if (!model.File.Contains(";base64"))
            {
                return new ApiResult { status = 0, msg = "请上传编码后的base64图片文件" };
            }
            string res;
            if (!ImageHelper.SaveBase64(model.File, out res))
            {
                return new ApiResult { status = 0, msg = res };
            }

            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            PayCodeDTO[] payCodes = await payCodeService.GetModelByUserIdAsync(user.Id);
            if(payCodes==null)
            {
                long id= await payCodeService.AddAsync(user.Id, "微信收款码", res, null);
                if(id<=0)
                {
                    return new ApiResult { status = 0, msg = "收款码添加修改失败" };
                }
            }
            else
            {
                bool flag = await payCodeService.UpdateAsync(payCodes.First().Id,null,res,null);
                if (!flag)
                {
                    return new ApiResult { status = 0, msg = "收款码添加修改失败" };
                }
            }            
            return new ApiResult { status = 1, msg = "收款码添加修改成功" };
        }
        [HttpPost]
        public async Task<ApiResult> EditNickName(UserCenterEditNickNameModel model)
        {
            if (string.IsNullOrEmpty(model.NickName))
            {
                return new ApiResult { status = 0, msg = "昵称不能为空" };
            }
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            bool flag = await userService.UpdateInfoAsync(user.Id, model.NickName, null);
            if (!flag)
            {
                return new ApiResult { status = 0, msg = "昵称添加修改失败" };
            }
            return new ApiResult { status = 1, msg = "昵称添加修改成功" };
        }
        [HttpPost]
        public async Task<ApiResult> ResetPwd(UserCenterResetPwdModel model)
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
            long id = await userService.ResetPasswordAsync(user.Id, model.Password, model.NewPassword);
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