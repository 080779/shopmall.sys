﻿using IMS.Common;
using IMS.Common.Newtonsoft;
using IMS.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Net;
using System.Web.Http;
using Newtonsoft.Json;
using IMS.Service.Service;

namespace IMS.Web.App_Start.Filter
{
    public class ApiSYSAuthorizationFilter : AuthorizationFilterAttribute
    {
        public IUserTokenService userTokenService = new UserTokenService();
        private string TokenSecret = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.ActionDescriptor.GetCustomAttributes<System.Web.Http.AllowAnonymousAttribute>().Any() || actionContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<System.Web.Http.AllowAnonymousAttribute>().Any())
            {
                return;
            }
            //KeyValuePair<string, string> keyValuePair = actionContext.Request.GetQueryNameValuePairs().SingleOrDefault(k => k.Key == "token");
            //if (keyValuePair.Value==null)
            //{
            //    actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new HttpError("Token不能为空"));
            //    return;
            //}
            //string token = keyValuePair.Value;
            IEnumerable<string> values;
            if(!actionContext.Request.Headers.TryGetValues("token",out values))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new HttpError("Token不能为空"));
                return;
            }
            string token = values.First();
            string res;
            if (!CommonHelper.JwtDecrypt(token, TokenSecret, out res))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new HttpError(res));
                return;
            }
            User user = JsonConvert.DeserializeObject<User>(res);
            object cache = CacheHelper.GetCache("App_User_CheckToken" + user.UserId);
            if ( cache != null)
            {
                long idres =(long)CacheHelper.GetCache("App_User_CheckToken" + user.UserId);
                if (idres == -1)
                {
                    actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new HttpError("用户不存在"));
                    return;
                }
                if (idres == -2)
                {
                    actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new HttpError("后台token为空，请重新登录"));
                    return;
                }
                if (idres == -3)
                {
                    actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new HttpError("token错误"));
                    return;
                }
            }
            else
            {
                long id = userTokenService.CheckToken(user.UserId, token);
                CacheHelper.SetCache("App_User_CheckToken" + user.UserId, id, DateTime.UtcNow.AddSeconds(10), TimeSpan.Zero);
                object cache1 = CacheHelper.GetCache("App_User_CheckToken" + user.UserId);
                if (id == -1)
                {
                    actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new HttpError("用户不存在"));
                    return;
                }
                if (id == -2)
                {
                    actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new HttpError("后台token为空，请重新登录"));
                    return;
                }
                if (id == -3)
                {
                    actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new HttpError("输入的token错误"));
                    return;
                }
            }                
        }
    }
}