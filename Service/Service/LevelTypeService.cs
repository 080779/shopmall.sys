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
    public class LevelTypeService : ILevelTypeService
    {
        public LevelTypeDTO ToDTO(LevelTypeEntity entity)
        {
            LevelTypeDTO dto = new LevelTypeDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Description = entity.Description;
            dto.Id = entity.Id;
            dto.Name = entity.Name;
            return dto;
        }
        public async Task<LevelTypeDTO[]> GetModelListAsync()
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var levelTypes = await dbc.GetAll<LevelTypeEntity>().ToListAsync();
                return levelTypes.Select(s => ToDTO(s)).ToArray();
            }
        }

        public async Task<bool> UpdateAsync(KeyValuePair<long, string>[] keyValues)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                foreach(var item in keyValues)
                {
                    var levelType = await dbc.GetAll<LevelTypeEntity>().SingleOrDefaultAsync(l => l.Id == item.Key);
                    if(levelType==null)
                    {
                        return false;
                    }
                    levelType.Name = item.Value;
                }
                await dbc.SaveChangesAsync();
                return true;
            }
        }
    }
}
