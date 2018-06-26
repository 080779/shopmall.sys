using IMS.Common;
using IMS.Common.Newtonsoft;
using IMS.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.Http.Filters;

namespace IMS.Web.App_Start.Filter
{
    public class ApiSYSAuthorizationFilter : AuthorizationFilterAttribute
    {
        public IAdminService adminUserService = DependencyResolver.Current.GetService<IAdminService>();
        public IPermissionService permissionService = DependencyResolver.Current.GetService<IPermissionService>();
        public IAdminLogService adminLogService = DependencyResolver.Current.GetService<IAdminLogService>();
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var v = filterContext.HttpContext.Request.Url;
           
            if(v.ToString().ToLower().Contains("/api/"))
            {
                long? UserId = (long?)filterContext.HttpContext.Session["Merchant_User_Id"];
                if (UserId == null)
                {
                    if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
                    {
                        return;
                    }
                    if (filterContext.HttpContext.Request.IsAjaxRequest())//判断是否是ajax请求
                    {
                        filterContext.Result = new JsonNetResult { Data = new AjaxResult { Status = 302, Data = "/api/login" } };
                    }
                    else
                    {
                        filterContext.Result = new RedirectResult("/api/login");
                    }
                    return;
                }
            }            
        }
    }
}