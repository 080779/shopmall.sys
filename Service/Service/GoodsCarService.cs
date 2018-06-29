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
    public class GoodsCarService : IGoodsCarService
    {
        private GoodsCarDTO ToDTO(GoodsCarEntity entity,string imgUrl)
        {
            GoodsCarDTO dto = new GoodsCarDTO();
            dto.GoodsId = entity.GoodsId;
            dto.UserId = entity.UserId;
            dto.Code = entity.Goods.Code;
            dto.CreateTime = entity.CreateTime;
            dto.Description = entity.Goods.Description;
            dto.Id = entity.Id;
            dto.ImgUrl = imgUrl;
            dto.Name = entity.Goods.Name;
            dto.Price = entity.Goods.Price;
            dto.RealityPrice = entity.Goods.RealityPrice;
            dto.Standard = entity.Goods.Standard;
            dto.Number = entity.Number;
            dto.GoodsAmount = entity.Goods.RealityPrice * entity.Number;
            return dto;
        }
        public async Task<long> AddAsync(long userId, long goodsId, long num)
        {
            using (MyDbContext dbc=new MyDbContext())
            {
                GoodsCarEntity entity = new GoodsCarEntity();
                entity.GoodsId = goodsId;
                entity.UserId = userId;
                entity.Number = num;
                dbc.GoodsCars.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsCarEntity entity = await dbc.GetAll<GoodsCarEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<GoodsCarSearchResult> GetModelListAsync(long? userId,string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsCarSearchResult result = new GoodsCarSearchResult();
                var entities = dbc.GetAll<GoodsCarEntity>();
                if(userId!=null)
                {
                    entities = entities.Where(g => g.UserId == userId);
                }
                if(!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g=>g.Goods.Code.Contains(keyword) || g.Goods.Name.Contains(keyword) || g.Goods.Description.Contains(keyword));
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
                var goodsAreaResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                var imgUrls = dbc.GetAll<GoodsImgEntity>();
                result.GoodsCars = goodsAreaResult.Select(a => ToDTO(a,imgUrls.First(g=>g.GoodsId==a.GoodsId).ImgUrl)).ToArray();
                return result;
            }
        }

        public async Task<bool> UpdateAsync(long id,long num)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsCarEntity entity = await dbc.GetAll<GoodsCarEntity>().SingleOrDefaultAsync(g=>g.Id==id);
                if (entity==null)
                {
                    return false;
                }
                entity.Number = num;
                await dbc.SaveChangesAsync();
                return true;
            }
        }
    }
}
