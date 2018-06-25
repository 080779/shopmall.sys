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
        public async Task<long> AddAsync(GoodsAddEditModel goods)
        {
            using (MyDbContext dbc = new MyDbContext())
            {

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

        public async Task<GoodsSearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsSearchResult result = new GoodsSearchResult();
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
                GoodsImgEntity imgEntity = await dbc.GetAll<GoodsImgEntity>().SingleOrDefaultAsync(g => g.Id == entity.GoodsImgId);
                entity.Description = goods.Description;
                entity.GoodsAreaId = goods.GoodsAreaId;

            }
        }
    }
}
