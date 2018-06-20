using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Service
{
    public class AdminService : IAdminService
    {
        public AdminDTO ToDTO(AdminEntity entity)
        {
            AdminDTO dto = new AdminDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Description = entity.Description;
            dto.Id = entity.Id;
            dto.Mobile = entity.Mobile;
            dto.IsEnabled = entity.IsEnabled;
            dto.PermissionIds = entity.Permissions.Select(p => p.Id).ToArray();
            return dto;
        }
        public async Task<long> AddAsync(string adminMobile,string mobile, string description, string password)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var user = dbc.GetAll<AdminEntity>().SingleOrDefault(a => a.Mobile == mobile);
                var p_user_m = dbc.GetAll<PlatformUserEntity>().SingleOrDefault(p => p.Mobile == mobile);
                var p_user_c = dbc.GetAll<PlatformUserEntity>().SingleOrDefault(p => p.Mobile == mobile);
                if (user!=null)
                {
                    return -2;
                }
                if(p_user_m!=null)
                {
                    return -3;
                }
                if(p_user_c!=null)
                {
                    return -4;
                }
                long type = dbc.GetAll<PlatformUserTypeEntity>().SingleOrDefault(p=>p.Name=="平台").Id;
                PlatformUserEntity u_entity = new PlatformUserEntity();
                u_entity.Code = mobile;
                u_entity.Mobile = mobile;
                u_entity.AdderMobile = adminMobile;
                u_entity.Salt = CommonHelper.GetCaptcha(6);
                u_entity.Password = CommonHelper.GetMD5(password);
                u_entity.TradePassword = CommonHelper.GetMD5(password);
                u_entity.PlatformUserTypeId = type;
                dbc.PlatformUsers.Add(u_entity);

                AdminEntity entity = new AdminEntity();
                entity.Mobile = mobile;
                entity.Description = description;
                entity.Salt = CommonHelper.GetCaptcha(4);
                entity.Password = CommonHelper.GetMD5(password + entity.Salt);
                dbc.Admins.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }
        public async Task<bool> UpdateAsync(long id, string mobile, string description, string password, long[] permissionIds)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<AdminEntity>().SingleOrDefaultAsync(a => a.Id == id);
                if(entity==null)
                {
                    return false;
                }
                var platformUser = await dbc.GetAll<PlatformUserEntity>().SingleOrDefaultAsync(p => p.Mobile == entity.Mobile);
                if(platformUser==null)
                {
                    return false;
                }
                platformUser.Mobile = mobile;
                platformUser.Code = mobile;

                entity.Mobile = mobile;
                entity.Description = description;
                entity.Password = CommonHelper.GetMD5(password + entity.Salt);
                entity.Permissions.Clear();
                var perms = dbc.GetAll<PermissionEntity>().Where(p => permissionIds.Contains(p.Id));
                await perms.ForEachAsync(p => entity.Permissions.Add(p));
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> UpdateAsync(long id, long[] permissionIds)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<AdminEntity>().SingleOrDefaultAsync(a => a.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.Permissions.Clear();
                var perms = dbc.GetAll<PermissionEntity>().Where(p => permissionIds.Contains(p.Id));
                await perms.ForEachAsync(p => entity.Permissions.Add(p));
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> UpdateAsync(long id, string password)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<AdminEntity>().SingleOrDefaultAsync(a => a.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.Password = CommonHelper.GetMD5(password + entity.Salt);
                await dbc.SaveChangesAsync();
                return true;
            }
        }
        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<AdminEntity>().SingleOrDefaultAsync(a => a.Id == id);
                if (entity == null)
                {
                    return false;
                }
                var platformUser = await dbc.GetAll<PlatformUserEntity>().SingleOrDefaultAsync(p => p.Mobile == entity.Mobile);
                if (platformUser == null)
                {
                    return false;
                }
                platformUser.IsDeleted = true;

                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> FrozenAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<AdminEntity>().SingleOrDefaultAsync(a => a.Id == id);
                if (entity == null)
                {
                    return false;
                }
                var platformUser = await dbc.GetAll<PlatformUserEntity>().SingleOrDefaultAsync(p => p.Mobile == entity.Mobile);
                if (platformUser == null)
                {
                    return false;
                }
                platformUser.IsEnabled = !platformUser.IsEnabled;

                entity.IsEnabled = !entity.IsEnabled;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<AdminDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<AdminEntity>().SingleOrDefaultAsync(a => a.Id == id);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<AdminSearchResult> GetModelListAsync(string isAdmin,string mobile, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                AdminSearchResult result = new AdminSearchResult();
                var admins = dbc.GetAll<AdminEntity>();
                if(isAdmin!="admin")
                {
                    admins = admins.Where(a => a.Mobile== isAdmin);
                }
                else
                {
                    if (!string.IsNullOrEmpty(mobile))
                    {
                        admins = admins.Where(a => a.Mobile.Contains(mobile));
                    }
                    if (startTime != null)
                    {
                        admins = admins.Where(a => a.CreateTime.Year >= startTime.Value.Year && a.CreateTime.Month >= startTime.Value.Month && a.CreateTime.Day >= startTime.Value.Day);
                    }
                    if (endTime != null)
                    {
                        admins = admins.Where(a => a.CreateTime.Year <= endTime.Value.Year && a.CreateTime.Month <= endTime.Value.Month && a.CreateTime.Day <= endTime.Value.Day);
                    }
                }                
                result.TotalCount = await admins.LongCountAsync();
                var adminsResult= await admins.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Admins = adminsResult.Select(a => ToDTO(a)).ToArray();                
                return result;
            }
        }

        public async Task<AdminSearchResult> GetModelListHasPerAsync(string isAdmin, string mobile, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                AdminSearchResult result = new AdminSearchResult();
                var admins = dbc.GetAll<AdminEntity>();
                if (isAdmin != "admin")
                {
                    admins = admins.Where(a => a.Mobile != "admin");
                }
                if (!string.IsNullOrEmpty(mobile))
                {
                    admins = admins.Where(a => a.Mobile.Contains(mobile));
                }
                if (startTime != null)
                {
                    admins = admins.Where(a => a.CreateTime.Year >= startTime.Value.Year && a.CreateTime.Month >= startTime.Value.Month && a.CreateTime.Day >= startTime.Value.Day);
                }
                if (endTime != null)
                {
                    admins = admins.Where(a => a.CreateTime.Year <= endTime.Value.Year && a.CreateTime.Month <= endTime.Value.Month && a.CreateTime.Day <= endTime.Value.Day);
                }
                result.TotalCount = await admins.LongCountAsync();
                var adminsResult = await admins.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Admins = adminsResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public bool HasPermission(long id, string description)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var admin = dbc.GetAll<AdminEntity>().SingleOrDefault(a => a.Id == id);
                if(admin==null)
                {
                    return false;
                }
                return admin.Permissions.Any(p => p.Description.Contains(description));
            }
        }

        public async Task<long> CheckLogin(string mobile, string password)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var admin = await dbc.GetAll<AdminEntity>().SingleOrDefaultAsync(a => a.Mobile == mobile);
                if (admin == null)
                {
                    return -1;
                }
                if(!admin.IsEnabled)
                {
                    return -2;
                }
                string pwd = CommonHelper.GetMD5(password + admin.Salt);
                if (admin.Password!=pwd)
                {
                    return -3;
                }
                return admin.Id;
            }
        }
    }
}
