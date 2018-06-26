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
    public class GoodsCarService : IGoodsService
    {

        private GoodsCarDTO ToDTO(GoodsCarEntity entity)
        {
            GoodsCarDTO dto = new GoodsCarDTO();
            dto.GoodsId = entity.GoodsId;
            dto.Code = entity.Goods.Code;
            dto.CreateTime = entity.CreateTime;
            dto.Description = entity.Goods.Description;
            dto.Id = entity.Id;
            dto.Name = entity.Goods.Name;
            dto.Price = entity.Goods.Price;
            dto.RealityPrice = entity.Goods.RealityPrice;
            dto.Standard = entity.Goods.Standard;
            dto.Number = entity.Number;
            dto.TotalAmount = entity.TotalAmount;
            return dto;
        }
        public Task<long> AddAsync(GoodsAddEditModel goods)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<GoodsCarSearchResult> GetModelListAsync(long? goodsTypeId, long? goodsSecondTypeId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(GoodsAddEditModel goods)
        {
            throw new NotImplementedException();
        }
    }
}
