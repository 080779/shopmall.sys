using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        private int pageSize = 10;
        public IUserService userService { get; set; }
        public IIdNameService idNameService { get; set; }
        public ISettingService settingService { get; set; }
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> List(long? levelId,string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            //long levelId = await idNameService.GetIdByNameAsync("会员等级");
            var result = await userService.GetModelListAsync(levelId, keyword, startTime, endTime, pageIndex, pageSize);
            UserListViewModel model = new UserListViewModel();
            model.PageCount = result.PageCount;
            model.Users = result.Users;
            model.Levels = await idNameService.GetByTypeNameAsync("会员等级");
            model.SettingTypes = await idNameService.GetByTypeNameAsync("推荐等级");
            long settingTypeId;
            model.Settings = (await settingService.GetModelListAsync(model.SettingTypes.Select(s=>settingTypeId= s.Id).ToArray())).Select(s=>new SettingModel { Id=s.Id,Parm=s.Parm}).ToList();
            return Json(new AjaxResult { Status = 1, Data = model });
        }
        public async Task<ActionResult> Add(string mobile,string recommendMobile,string password)
        {
            if(string.IsNullOrEmpty(mobile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "会员账号不能为空" });
            }
            if (string.IsNullOrEmpty(recommendMobile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "推荐人账号不能为空" });
            }
            if (string.IsNullOrEmpty(password))
            {
                return Json(new AjaxResult { Status = 0, Msg = "登录密码不能为空" });
            }
            long levelId= await idNameService.GetIdByNameAsync("普通会员");
            long id= await userService.AddAsync(mobile,password,levelId,recommendMobile);
            if(id<=0)
            {
                if (id == -1)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "会员账号已经存在" });
                }
                if (id == -2)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "推荐人不存在" });
                }
                return Json(new AjaxResult { Status = 0, Msg = "会员添加失败" });
            }            
            return Json(new AjaxResult { Status = 1, Msg = "会员添加成功" });
        }

        public async Task<ActionResult> BonusSet(List<Setting> settings)
        {
            SettingParm[] parms= settings.Select(s => new SettingParm { Id = s.Id, Parm = s.Parm }).ToArray();
            bool flag= await settingService.UpdateAsync(parms);
            if(!flag)
            {
                return Json(new AjaxResult { Status = 0, Msg = "修改失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "修改成功" });
        }

        public async Task<ActionResult> ResetPwd(long id, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return Json(new AjaxResult { Status = 0, Msg = "登录密码不能为空" });
            }
            long res = await userService.ResetPasswordAsync(id,password);
            if (res <= 0)
            {
                if (id == -1)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "会员不存在" });
                }
                return Json(new AjaxResult { Status = 0, Msg = "重置密码失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "重置密码成功" });
        }

        public async Task<ActionResult> Frozen(long id)
        {
            bool res= await userService.FrozenAsync(id);
            if (!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "冻结用户失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "冻结用户成功" });
        }
        public async Task<ActionResult> Delete(long id)
        {
            bool res = await userService.DeleteAsync(id);
            if (!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "删除用户失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "删除用户成功" });
        }
    }
}