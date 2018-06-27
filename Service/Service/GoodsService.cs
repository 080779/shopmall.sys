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
    public class GoodsService : IGoodsService
    {
        private GoodsDTO ToDTO(GoodsEntity entity)
        {
            GoodsDTO dto = new GoodsDTO();
            dto.Code = entity.Code;
            dto.CreateTime = entity.CreateTime;
            dto.Description = entity.Description;
            dto.GoodsAreaId = entity.GoodsAreaId;
            dto.GoodsAreaTitle = entity.GoodsArea.Title;
            dto.GoodsSecondTypeId = entity.GoodsSecondTypeId;
            dto.GoodsSecondTypeName = entity.GoodsSecondType.Name;
            dto.GoodsTypeId = entity.GoodsTypeId;
            dto.GoodsTypeName = entity.GoodsType.Name;
            dto.Id = entity.Id;
            dto.Inventory = entity.Inventory;
            dto.IsPutaway = entity.IsPutaway;
            dto.IsRecommend = entity.IsRecommend;
            dto.Name = entity.Name;
            dto.Price = entity.Price;
            dto.RealityPrice = entity.RealityPrice;
            dto.SaleNum = entity.SaleNum;
            dto.Standard = entity.Standard;
            return dto;
        }
        public async Task<long> AddAsync(GoodsAddEditModel goods)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsEntity entity = new GoodsEntity();
                entity.Code = CommonHelper.GetRandom4();
                entity.Description = goods.Description;
                entity.GoodsAreaId = goods.GoodsAreaId;
                entity.GoodsSecondTypeId = goods.GoodsSecondTypeId;
                entity.GoodsTypeId = goods.GoodsTypeId;
                entity.Inventory = goods.Inventory;
                entity.IsPutaway = goods.IsPutaway;
                entity.IsRecommend = goods.IsRecommend;
                entity.Name = goods.Name;
                entity.Price = goods.Price;
                entity.RealityPrice = goods.RealityPrice;
                entity.Standard = goods.Standard;
                dbc.Goods.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsEntity entity = await dbc.GetAll<GoodsEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<GoodsSearchResult> GetModelListAsync(long? goodsTypeId,long? goodsSecondTypeId,string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsSearchResult result = new GoodsSearchResult();
                var entities = dbc.GetAll<GoodsEntity>();
                if(goodsTypeId!=null)
                {
                    entities = entities.Where(a => a.GoodsTypeId == goodsTypeId);
                }
                if (goodsSecondTypeId != null)
                {
                    entities = entities.Where(a => a.GoodsSecondTypeId == goodsSecondTypeId);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g =>g.Code.Contains(keyword) || g.Name.Contains(keyword) || g.Description.Contains(keyword));
                }
                if (startTime != null)
                {
                    entities = entities.Where(a => a.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    entities = entities.Where(a => a.CreateTime.Year <= endTime.Value.Year && a.CreateTime.Month <= endTime.Value.Month && a.CreateTime.Day <= endTime.Value.Day);
                }
                result.TotalCount = entities.LongCount();
                var goodsTypesResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Goods = goodsTypesResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<bool> UpdateAsync(GoodsAddEditModel goods)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsEntity entity = await dbc.GetAll<GoodsEntity>().SingleOrDefaultAsync(g=>g.Id==goods.Id);                
                if (entity==null)
                {
                    return false;
                }
                entity.Description = goods.Description;
                entity.GoodsAreaId = goods.GoodsAreaId;
                entity.GoodsSecondTypeId = goods.GoodsSecondTypeId;
                entity.GoodsTypeId = goods.GoodsTypeId;
                entity.Inventory = goods.Inventory;
                entity.IsPutaway = goods.IsPutaway;
                entity.IsRecommend = goods.IsRecommend;
                entity.Name = goods.Name;
                entity.Price = goods.Price;
                entity.RealityPrice = goods.RealityPrice;
                entity.Standard = goods.Standard;
                await dbc.SaveChangesAsync();
                return true;
            }
        }
    }
}
