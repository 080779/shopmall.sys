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
    public class MainGoodsTypeService : IMainGoodsTypeService
    {
        private MainGoodsTypeDTO ToDTO(MainGoodsTypeEntity entity)
        {
            MainGoodsTypeDTO dto = new MainGoodsTypeDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.Description = entity.Description;
            dto.Name = entity.Name;
            dto.ImgUrl = entity.ImgUrl;
            return dto;
        }
        public async Task<long> AddAsync(string name, string imgUrl, string description)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                MainGoodsTypeEntity entity = new MainGoodsTypeEntity();
                entity.Name = name;
                entity.ImgUrl = imgUrl;
                entity.Description = description;
                dbc.MainGoodsTypes.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                MainGoodsTypeEntity entity = await dbc.GetAll<MainGoodsTypeEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<MainGoodsTypeSearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                MainGoodsTypeSearchResult result = new MainGoodsTypeSearchResult();
                var entities = dbc.GetAll<MainGoodsTypeEntity>();
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
                var mainGoodsTypesResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.MainGoodsTypes = mainGoodsTypesResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<bool> UpdateAsync(long id, string name, string imgUrl, string description)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                MainGoodsTypeEntity entity = await dbc.GetAll<MainGoodsTypeEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.Name = name;
                entity.ImgUrl = imgUrl;
                entity.Description = description;
                await dbc.SaveChangesAsync();
                return true;
            }
        }
    }
}
