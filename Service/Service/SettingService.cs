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

        public async Task<SettingSearchResult> GetModelListAsync(long[] settingTypeIds,string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SettingSearchResult result = new SettingSearchResult();
                var entities = dbc.GetAll<SettingEntity>();
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
                    entities = entities.Where(a => a.CreateTime.Year <= endTime.Value.Year && a.CreateTime.Month <= endTime.Value.Month && a.CreateTime.Day <= endTime.Value.Day);
                }
                result.PageCount = (int)Math.Ceiling((await entities.LongCountAsync()) * 1.0f / pageSize);
                var settingsResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Settings = settingsResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<bool> UpdateAsync(long id, string description)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SettingEntity entity = await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.Description = description;
                await dbc.SaveChangesAsync();
                return true;
            }
        }
    }
}
