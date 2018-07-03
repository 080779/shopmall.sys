﻿using IMS.Common;
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
    public class OrderListService : IOrderListService
    {
        private OrderListDTO ToDTO(OrderListEntity entity)
        {
            OrderListDTO dto = new OrderListDTO();
            dto.CreateTime = entity.CreateTime;
            dto.GoodsId = entity.GoodsId;
            dto.GoodsName = entity.Goods.Name;
            dto.Id = entity.Id;
            dto.ImgUrl = entity.ImgUrl;
            dto.Number = entity.Number;
            dto.OrderCode = entity.Order.Code;
            dto.OrderId = entity.OrderId;
            dto.PostFee = entity.PostFee;
            dto.Poundage = entity.Poundage;
            dto.Price = entity.Price;
            dto.TotalFee = entity.TotalFee;
            return dto;
        }
        public async Task<long> AddAsync(long orderId, long goodsId, long number)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsEntity goods = await dbc.GetAll<GoodsEntity>().SingleOrDefaultAsync(g => g.Id == goodsId);
                if(goods==null)
                {
                    return -1;
                }
                OrderListEntity entity = new OrderListEntity();
                entity.OrderId = orderId;
                entity.GoodsId = goodsId;
                entity.Number = number;
                entity.Price = goods.RealityPrice;
                GoodsImgEntity imgEntity = await dbc.GetAll<GoodsImgEntity>().FirstOrDefaultAsync(g => g.GoodsId == goodsId);
                if(imgEntity==null)
                {
                    entity.ImgUrl = "";
                }
                else
                {
                    entity.ImgUrl = imgEntity.ImgUrl;
                }                
                entity.TotalFee = entity.Price * number+entity.PostFee+entity.Poundage;
                dbc.OrderLists.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<long> AddListAsync(List<OrderListAdd> goodsLists)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                foreach (var goods in goodsLists)
                {
                    GoodsEntity goodsEntity = await dbc.GetAll<GoodsEntity>().SingleOrDefaultAsync(g => g.Id == goods.GoodsId);
                    if (goodsEntity == null)
                    {
                        return -1;
                    }
                    OrderListEntity entity = new OrderListEntity();
                    entity.OrderId = goods.OrderId;
                    entity.GoodsId = goods.GoodsId;
                    entity.Number = goods.Number;
                    entity.Price = goodsEntity.RealityPrice;
                    GoodsImgEntity imgEntity = await dbc.GetAll<GoodsImgEntity>().FirstOrDefaultAsync(g => g.GoodsId == goods.GoodsId);
                    if (imgEntity == null)
                    {
                        entity.ImgUrl = "";
                    }
                    else
                    {
                        entity.ImgUrl = imgEntity.ImgUrl;
                    }
                    entity.TotalFee = entity.Price * entity.Number + entity.PostFee + entity.Poundage;
                    dbc.OrderLists.Add(entity);
                }
                await dbc.SaveChangesAsync();
                return 1;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderListEntity entity = await dbc.GetAll<OrderListEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<OrderListSearchResult> GetModelListAsync(long? orderId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderListSearchResult result = new OrderListSearchResult();
                var entities = dbc.GetAll<OrderListEntity>();
                if (orderId != null)
                {
                    entities = entities.Where(a => a.OrderId == orderId);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g => g.Goods.Name.Contains(keyword) || g.Goods.Code.Contains(keyword));
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
                var orderListResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.OrderLists = orderListResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<bool> UpdateAsync(long id, long number)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderListEntity entity = await dbc.GetAll<OrderListEntity>().SingleOrDefaultAsync(o => o.Id == id);
                if(entity==null)
                {
                    return false;
                }
                entity.Number = number;
                await dbc.SaveChangesAsync();
                return true;
            }
        }
    }
}
