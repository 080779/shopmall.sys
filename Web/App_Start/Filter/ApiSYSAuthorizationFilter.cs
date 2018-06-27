using IMS.Common;
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

namespace IMS.Web.App_Start.Filter
{
    public class ApiSYSAuthorizationFilter : AuthorizationFilterAttribute
    {
        public IUserTokenService userTokenService = DependencyResolver.Current.GetService<IUserTokenService>();
        private string TokenSecret = "fde3ffewtwtegfw2dsd4rrjhkffnvb";
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            //if (!context.HttpContext.Request.Headers.TryGetValue("AppKey", out values))
            //{
            //    res.Content = "AppKey不能为空";
            //    res.StatusCode = 401;
            //    context.Result = res;
            //    return;
            //}
            IEnumerable<string> lists;
            if (actionContext.Request.Headers.TryGetValues("token", out lists))
            {
                actionContext.Response= actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new HttpError("Token不能为空"));
                return;
            }
            string token= lists.First();
            string res;
            if(!CommonHelper.JwtDecrypt(token, TokenSecret,out res))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new HttpError(res));
                return;
            }
            User user = JsonConvert.DeserializeObject<User>(res);
            if(CacheHelper.GetCache("App_User_Info" + user.UserId)!=null)
            {
                return;
            }
            long id= userTokenService.CheckToken(user.UserId, token);
            if(id==-1)
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
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new HttpError("token错误"));
                return;
            }
            CacheHelper.SetCache("App_User_Info" + user.UserId, user, TimeSpan.FromSeconds(30));
        }
    }
}