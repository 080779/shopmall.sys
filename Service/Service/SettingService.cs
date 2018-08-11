using IMS.DTO;
using IMS.IService;
using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Service
{
    public class SettingService : ISettingService
    {
        private SettingDTO ToDTO(SettingEntity entity)
        {
            SettingDTO dto = new SettingDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.Name = entity.Name;
            dto.Description = entity.Description;
            dto.TypeName = entity.SettingType.Name;
            dto.TypeDescription = entity.SettingType.Description;
            dto.Parm = entity.Parm;
            return dto;
        }
        public async Task<long> AddAsync(string name, long sttingTypeId, string description)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SettingEntity entity = new SettingEntity();
                entity.Name = name;
                entity.SettingTypeId = sttingTypeId;
                entity.Description = description;
                dbc.Settings.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SettingEntity entity = await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<SettingDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SettingEntity entity = await dbc.GetAll<SettingEntity>().Include(s => s.SettingType).AsNoTracking().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return null;
                }                
                return ToDTO(entity);
            }
        }

        public async Task<SettingDTO> GetModelByNameAsync(string name)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SettingEntity entity = await dbc.GetAll<SettingEntity>().Include(s => s.SettingType).AsNoTracking().SingleOrDefaultAsync(g => g.Name == name);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<string> GetParmByNameAsync(string name)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                string parm = await dbc.GetParameterAsync<SettingEntity>(g => g.Name == name,g=>g.Parm);
                if (parm == null)
                {
                    return null;
                }
                return parm;
            }
        }

        public async Task<SettingDTO[]> GetModelListAsync(string settingTypeName)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                long settingTypeId = await dbc.GetIdAsync<IdNameEntity>(i => i.Name == settingTypeName);
                var entities = dbc.GetAll<SettingEntity>().Include(s => s.SettingType).AsNoTracking().Where(a=>a.SettingTypeId==settingTypeId);
                var settingsResult = await entities.ToListAsync();
                return settingsResult.Select(a => ToDTO(a)).ToArray();
            }
        }

        public async Task<SettingDTO[]> GetModelListAsync(long[] settingTypeIds)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entities = dbc.GetAll<SettingEntity>().Include(s => s.SettingType).AsNoTracking();
                if (settingTypeIds.Count() > 0)
                {
                    entities = entities.Where(a => settingTypeIds.Contains(a.SettingTypeId));
                }
                var settingsResult = await entities.ToListAsync();
                return settingsResult.Select(a => ToDTO(a)).ToArray();
            }
        }

        public async Task<SettingSearchResult> GetModelListAsync(long[] settingTypeIds,string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SettingSearchResult result = new SettingSearchResult();
                var entities = dbc.GetAll<SettingEntity>().Include(s => s.SettingType).AsNoTracking();
                if(settingTypeIds.Count()>0)
                {
                    entities = entities.Where(a => settingTypeIds.Contains(a.SettingTypeId));
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g => g.Name.Contains(keyword) || g.Description.Contains(keyword));
                }
                if (startTime != null)
                {
                    entities = entities.Where(a => a.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    entities = entities.Where(a => SqlFunctions.DateDiff("day", endTime, a.CreateTime) <= 0);
                }
                result.PageCount = (int)Math.Ceiling((await entities.LongCountAsync()) * 1.0f / pageSize);
                var settingsResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Settings = settingsResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<bool> UpdateAsync(long id, string parm)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SettingEntity entity = await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.Parm = parm;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> UpdateAsync(params SettingParm[] parms)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                foreach(SettingParm parm in parms)
                {
                    SettingEntity entity = await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(g => g.Id == parm.Id);
                    if (entity == null)
                    {
                        return false;
                    }
                    entity.Parm = parm.Parm.ToString();
                }
                await dbc.SaveChangesAsync();
                return true;
            }
        }
    }
}
