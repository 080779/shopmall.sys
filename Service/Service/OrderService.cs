﻿using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Service
{
    public class OrderService : IOrderService
    {
        private OrderDTO ToDTO(OrderEntity entity)
        {
            OrderDTO dto = new OrderDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Amount = entity.Amount;
            dto.BuyerId = entity.BuyerId;
            dto.BuyerNickName = entity.Buyer.NickName;
            dto.BuyerMobile = entity.Buyer.Mobile;
            dto.Code = entity.Code;
            dto.Id = entity.Id;
            dto.OrderStateId = entity.OrderStateId;
            dto.OrderStateName = entity.OrderState.Name;
            dto.PayTypeId = entity.PayTypeId;
            dto.PayTypeName = entity.PayType.Name;
            dto.ReceiverAddress = entity.Address.Address;
            dto.ReceiverMobile = entity.Address.Mobile;
            dto.ReceiverName = entity.Address.Name;
            dto.Deliver = entity.Deliver;
            dto.DeliverName = entity.DeliverName;
            dto.DeliverCode = entity.DeliverCode;
            dto.PostFee = entity.PostFee;
            dto.ApplyTime = entity.ApplyTime;
            dto.CloseTime = entity.CloseTime;
            dto.ConsignTime = entity.ConsignTime;
            dto.DeductAmount = entity.DeductAmount;
            dto.EndTime = entity.EndTime;
            dto.IsRated = entity.IsRated;
            dto.PayTime = entity.PayTime;
            dto.RefundAmount = entity.RefundAmount;
            dto.AuditStatusId = entity.AuditStatusId;
            dto.DownCycledId = entity.DownCycledId;
            dto.AuditTime = entity.AuditTime;
            dto.AuditMobile = entity.AuditMobile;
            dto.AuditStatusName = entity.AuditStatus.Name;
            dto.DownCycledName = entity.DownCycled.Name;
            dto.ReturnAmount = entity.ReturnAmount;
            dto.DiscountAmount = entity.DiscountAmount;
            dto.UpAmount = entity.UpAmount;
            dto.UserDeliverCode = entity.UserDeliverCode;
            dto.UserDeliverName = entity.UserDeliverName;
            return dto;
        }

        public async Task<long> AddAsync(long buyerId, long addressId, long payTypeId, long orderStateId, long goodsId, long number)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity entity = new OrderEntity();
                entity.Code = CommonHelper.GetRandom3();
                entity.BuyerId = buyerId;
                entity.AddressId = addressId;
                entity.PayTypeId = payTypeId;
                entity.OrderStateId = orderStateId;
                dbc.Orders.Add(entity);
                await dbc.SaveChangesAsync();

                GoodsEntity goods = await dbc.GetAll<GoodsEntity>().SingleOrDefaultAsync(g => g.Id == goodsId);
                if (goods == null)
                {
                    return -1;
                }
                OrderListEntity listEntity = new OrderListEntity();
                listEntity.OrderId = entity.Id;
                listEntity.GoodsId = goodsId;
                listEntity.Number = number;
                listEntity.Price = goods.RealityPrice;
                GoodsImgEntity imgEntity = await dbc.GetAll<GoodsImgEntity>().FirstOrDefaultAsync(g => g.GoodsId == goodsId);
                if (imgEntity == null)
                {
                    listEntity.ImgUrl = "";
                }
                else
                {
                    listEntity.ImgUrl = imgEntity.ImgUrl;
                }
                listEntity.TotalFee = listEntity.Price * number;
                entity.Amount = listEntity.TotalFee;
                dbc.OrderLists.Add(listEntity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<long> AddAsync(long? deliveryTypeId,decimal? postFee,long buyerId, long addressId, long payTypeId, long orderStateId, params OrderApplyDTO[] orderApplies)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                using (var scope = dbc.Database.BeginTransaction())
                {
                    try
                    {
                        OrderEntity entity = new OrderEntity();
                        entity.Code = CommonHelper.GetRandom3();
                        entity.BuyerId = buyerId;
                        entity.AddressId = addressId;
                        entity.PayTypeId = payTypeId;
                        entity.OrderStateId = orderStateId;
                        entity.PostFee = postFee.Value;
                        if(deliveryTypeId==1)
                        {
                            entity.Deliver = "有快递单号";
                        }
                        if (deliveryTypeId == 2)
                        {
                            entity.Deliver = "无需物流";
                        }
                        if (deliveryTypeId == 3)
                        {
                            entity.Deliver = "同城自取";
                        }

                        UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == buyerId);
                        if (user == null)
                        {
                            scope.Rollback();
                            return -2;
                        }
                        //decimal up1 = 0;
                        //decimal up2 = 0;
                        //decimal up3 = 0;
                        //SettingEntity upSetting1 = await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(i => i.Name == "普通会员→黄金会员");
                        //SettingEntity upSetting2 = await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(i => i.Name == "普普通会员→→铂金会员");
                        //SettingEntity upSetting3 = await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(i => i.Name == "黄金会员→铂金会员");
                        //if (upSetting1 != null)
                        //{
                        //    decimal.TryParse(upSetting1.Parm, out up1);
                        //}
                        //if (upSetting2 != null)
                        //{
                        //    decimal.TryParse(upSetting2.Parm, out up2);
                        //}
                        //if (upSetting3 != null)
                        //{
                        //    decimal.TryParse(upSetting3.Parm, out up3);
                        //}
                        
                        decimal discount1 = 1;
                        decimal discount2 = 1;
                        decimal discount3 = 1;

                        SettingEntity discountSetting1 = await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(i => i.Name == "普通会员优惠");
                        SettingEntity discountSetting2 = await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(i => i.Name == "黄金会员优惠");
                        SettingEntity discountSetting3 = await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(i => i.Name == "铂金会员优惠");

                        if (discountSetting1 != null)
                        {
                            decimal.TryParse(discountSetting1.Parm, out discount1);
                        }
                        if (discountSetting2 != null)
                        {
                            decimal.TryParse(discountSetting2.Parm, out discount2);
                        }
                        if (discountSetting3 != null)
                        {
                            decimal.TryParse(discountSetting3.Parm, out discount3);
                        }

                        if (user.Level.Name == "普通会员")
                        {
                            //entity.DiscountAmount = entity.Amount * ((discount1 * 10) / 100);
                            entity.UpAmount = ((discount1 * 10) / 100);
                        }
                        else if (user.Level.Name == "黄金会员")
                        {
                            //entity.DiscountAmount = entity.Amount * ((discount2 * 10) / 100);
                            entity.UpAmount = ((discount2 * 10) / 100);
                        }
                        else if (user.Level.Name == "铂金会员")
                        {
                            //entity.DiscountAmount = entity.Amount * ((discount3 * 10) / 100);
                            entity.UpAmount = ((discount3 * 10) / 100);
                        }

                        dbc.Orders.Add(entity);
                        await dbc.SaveChangesAsync();

                        foreach (var orderApply in orderApplies)
                        {
                            GoodsEntity goods = await dbc.GetAll<GoodsEntity>().SingleOrDefaultAsync(g => g.Id == orderApply.GoodsId);
                            if (goods == null)
                            {
                                scope.Rollback();
                                return -1;
                            }
                            OrderListEntity listEntity = new OrderListEntity();
                            listEntity.OrderId = entity.Id;
                            listEntity.GoodsId = goods.Id;
                            listEntity.Number = orderApply.Number;
                            listEntity.Price = goods.RealityPrice;
                            listEntity.ImgUrl = orderApply.ImgUrl;
                            listEntity.TotalFee = listEntity.Price * listEntity.Number;
                            entity.Amount = entity.Amount + listEntity.TotalFee;
                            listEntity.DiscountFee = listEntity.TotalFee * entity.UpAmount.Value;
                            dbc.OrderLists.Add(listEntity);
                        }
                        entity.DiscountAmount = entity.Amount * entity.UpAmount;
                        entity.Amount = entity.DiscountAmount.Value + entity.PostFee;
                        await dbc.SaveChangesAsync();
                        scope.Commit();
                        return entity.Id;
                    }
                    catch (Exception ex)
                    {
                        scope.Rollback();
                        return -3;
                    }
                }                       
            }
        }

        public async Task<bool> AddUserDeliverAsync(long orderId, string userDeliverCode, string userDeliverName)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity entity = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(g => g.Id == orderId);
                if (entity == null)
                {
                    return false;
                }
                entity.UserDeliverCode = userDeliverCode;
                entity.UserDeliverName = userDeliverName;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> FrontMarkDel(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity entity = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsRated = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity entity = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<OrderDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity entity = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<OrderDTO[]> GetAllAsync()
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entities = await dbc.GetAll<OrderEntity>().OrderByDescending(a => a.CreateTime).ToListAsync();

                return entities.Select(o => ToDTO(o)).ToArray();
            }
        }

        public async Task<OrderSearchResult> GetModelListAsync(long? buyerId, long? orderStateId,long? auditStatusId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderSearchResult result = new OrderSearchResult();
                var entities = dbc.GetAll<OrderEntity>();
                if(buyerId!=null)
                {
                    entities = entities.Where(a => a.BuyerId ==buyerId);
                }
                if (orderStateId != null)
                {
                    entities = entities.Where(a => a.OrderStateId ==orderStateId);
                }
                if (auditStatusId != null)
                {
                    entities = entities.Where(a => a.AuditStatusId == auditStatusId);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g =>g.Code.Contains(keyword) ||  g.Buyer.Mobile.Contains(keyword));
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
                var goodsTypesResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Orders = goodsTypesResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<OrderSearchResult> GetRefundModelListAsync(long? buyerId, long? orderStateId, long? auditStatusId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderSearchResult result = new OrderSearchResult();
                var entities = dbc.GetAll<OrderEntity>().Where(o => o.OrderState.Name == "退单审核" || o.OrderState.Name == "退单中" || o.OrderState.Name == "退单完成");
                if (buyerId != null)
                {
                    entities = entities.Where(a => a.BuyerId == buyerId);
                }
                if (orderStateId != null)
                {
                    entities = entities.Where(a => a.OrderStateId == orderStateId);
                }
                if (auditStatusId != null)
                {
                    entities = entities.Where(a => a.AuditStatusId == auditStatusId);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g => g.Code.Contains(keyword) || g.Buyer.Mobile.Contains(keyword));
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
                var goodsTypesResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Orders = goodsTypesResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<OrderSearchResult> GetReturnModelListAsync(long? buyerId, long? orderStateId, long? auditStatusId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderSearchResult result = new OrderSearchResult();
                long returnStateId1 = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "退货审核")).Id;
                long returnStateId2 = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "退货中")).Id;
                long returnStateId3 = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "退货完成")).Id;
                var entities = dbc.GetAll<OrderEntity>().Where(o => o.OrderStateId == returnStateId1 || o.OrderStateId == returnStateId2 || o.OrderStateId == returnStateId3);
                if (buyerId != null)
                {
                    entities = entities.Where(a => a.BuyerId == buyerId);
                }
                if (orderStateId != null)
                {
                    entities = entities.Where(a => a.OrderStateId == orderStateId);
                }
                if (auditStatusId != null)
                {
                    entities = entities.Where(a => a.AuditStatusId == auditStatusId);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g => g.Code.Contains(keyword) || g.Buyer.Mobile.Contains(keyword));
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
                var goodsTypesResult = await entities.OrderByDescending(a => a.ApplyTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Orders = goodsTypesResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<OrderSearchResult> GetDeliverModelListAsync(long? buyerId, long? orderStateId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderSearchResult result = new OrderSearchResult();
                var entities = dbc.GetAll<OrderEntity>().Where(o => o.OrderState.Name == "待发货" || o.OrderState.Name == "退单审核" || o.OrderState.Name == "已发货" || o.Deliver== "无需物流" || o.Deliver== "同城自取");
                if (buyerId != null)
                {
                    entities = entities.Where(a => a.BuyerId == buyerId);
                }
                if (orderStateId !=null)
                {
                    entities = entities.Where(a => a.OrderStateId==orderStateId);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g => g.Code.Contains(keyword) || g.Buyer.Mobile.Contains(keyword));
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
                var goodsTypesResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Orders = goodsTypesResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<bool> UpdateAsync(long id, long? addressId, long? payTypeId, long? orderStateId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity entity = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                if(addressId!=null)
                {
                    entity.AddressId = addressId.Value;
                }
                if (payTypeId != null)
                {
                    entity.PayTypeId = payTypeId.Value;
                }
                if (orderStateId != null)
                {
                    entity.OrderStateId = orderStateId.Value;
                }
                await dbc.SaveChangesAsync();
                return true;
            }           
        }

        public async Task<long> UpdateDeliverStateAsync(long id,string deliver, string deliverName, string deliverCode)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity entity = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return -1 ;
                }
                if(entity.OrderState.Name=="退单中" || entity.OrderState.Name=="退单完成")
                {
                    return -2;
                }
                if(deliver== "无需物流")
                {
                    entity.OrderStateId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "已完成")).Id;
                    entity.EndTime = DateTime.Now;
                }
                else
                {
                    entity.OrderStateId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "已发货")).Id;
                }
                entity.Deliver = deliver;
                entity.DeliverName = deliverName;
                entity.DeliverCode = deliverCode;               
                entity.ConsignTime = DateTime.Now;
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == entity.BuyerId);
                if(user==null)
                {
                    return -3;
                }
                JournalEntity journal = await dbc.GetAll<JournalEntity>().SingleOrDefaultAsync(j => j.UserId == user.Id && j.OrderCode == entity.Code && j.JournalType.Name == "购物");
                if(journal==null)
                {
                    return -4;
                }
                if(journal.LevelId>user.LevelId)
                {
                    user.LevelId = journal.LevelId;
                }

                await dbc.SaveChangesAsync();
                return 1;
            }
        }

        public async Task<bool> Receipt(long id, long orderStateId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity order = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(o=>o.Id==id);
                if(order==null)
                {
                    return false;
                }
                order.EndTime = DateTime.Now;
                order.OrderStateId = orderStateId;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<long> ApplyReturnOrderAsync(long orderId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity order = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(o => o.Id == orderId);
                if (order == null)
                {
                    return -1;
                }
                if(order.OrderState.Name!="待发货")
                {
                    return -2;
                }
                order.ApplyTime = DateTime.Now;
                order.AuditStatusId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "未审核")).Id;
                order.OrderStateId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "退单审核")).Id;
                await dbc.SaveChangesAsync();
                return 1;
            }
        }

        public async Task<long> ReturnOrderAsync(long orderId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity order = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(o => o.Id == orderId);
                if (order == null)
                {
                    return -1;
                }
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == order.BuyerId);
                if(user==null)
                {
                    return -2;
                }

                JournalEntity journal1 = await dbc.GetAll<JournalEntity>().SingleOrDefaultAsync(j => j.UserId == user.Id && j.OrderCode == order.Code && j.JournalType.Name== "购物");
                if(journal1==null)
                {
                    return -3;
                }
                if(order.OrderState.Name!="退单中")
                {
                    return -4;
                }
                order.OrderStateId= (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "退单完成")).Id;

                user.Amount = user.Amount + journal1.OutAmount.Value;
                user.BuyAmount = user.BuyAmount - journal1.OutAmount.Value;

                var journals = await dbc.GetAll<JournalEntity>().Where(j => j.OrderCode == order.Code && j.JournalType.Name == "佣金收入" && j.IsEnabled == false).ToListAsync();
                foreach (JournalEntity journal2 in journals)
                {
                    UserEntity user1 = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == journal2.UserId);
                    user1.FrozenAmount = user1.FrozenAmount - journal2.InAmount.Value;
                    //journal2.IsEnabled = true;
                    journal2.IsDeleted = true;
                }

                //添加流水记录
                JournalEntity journal = new JournalEntity();
                journal.OrderCode = order.Code;
                journal.UserId = user.Id;
                journal.InAmount = journal1.OutAmount.Value;
                journal.Remark = "退单退款";
                journal.JournalTypeId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "退单退款")).Id;
                journal.BalanceAmount = user.Amount;
                dbc.Journals.Add(journal);
                await dbc.SaveChangesAsync();
                return 1;
            }
        }

        public async Task<long> ApplyReturnAsync(long orderId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity order = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(o => o.Id == orderId);
                if (order == null)
                {
                    return -1;
                }
                if(order.OrderState.Name!="已完成")
                {
                    return -2;
                }
                string val = (await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(i => i.Name == "不能退货时间")).Parm;

                if (order.EndTime!=null && DateTime.Now > order.EndTime.Value.AddDays(Convert.ToDouble(val)))
                {
                    return -4;
                }
                var orderLists = dbc.GetAll<OrderListEntity>().Where(o => o.OrderId == order.Id).ToList();
                decimal totalAmount = 0;
                decimal totalReturnAmount = 0;
                decimal totalDiscountReturnAmount = 0;
                decimal totalDiscountAmount = 0;
                foreach (var item in orderLists)
                {
                    totalAmount = totalAmount + item.TotalFee;
                    if (item.IsReturn == true)
                    {
                        totalReturnAmount = totalReturnAmount + item.TotalFee;
                    }
                }                
                if (order.DiscountAmount != null && totalAmount != 0 && order.UpAmount!=null)
                {
                    totalDiscountAmount = totalAmount * order.UpAmount.Value;
                    totalDiscountReturnAmount = totalReturnAmount * order.UpAmount.Value;
                }
                if (totalDiscountReturnAmount <= 0)
                {
                    return -2;
                }
                decimal percent = Convert.ToDecimal((await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(s => s.Name == "退货扣除比例")).Parm) / 100;
                order.ApplyTime = DateTime.Now;
                order.ReturnAmount = totalDiscountReturnAmount;
                order.DeductAmount = totalDiscountReturnAmount * percent;
                order.RefundAmount = order.ReturnAmount - order.DeductAmount;
                order.DownCycledId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "--不降级")).Id;
                order.AuditStatusId= (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "未审核")).Id;
                order.OrderStateId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "退货审核")).Id;
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == order.BuyerId);
                if (user == null)
                {
                    return -3;
                }
                //会员扣除金额、降级
                //user.Amount = user.Amount - order.RefundAmount.Value;
                //普通会员id
                long levelId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "普通会员")).Id;
                //黄金会员id
                long levelId1 = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "黄金会员")).Id;
                //铂金会员id
                long levelId2 = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "铂金会员")).Id;
                if (user.LevelId == levelId1 && !user.IsReturned)
                {
                    if (totalReturnAmount / totalAmount >= (decimal)0.5)
                    {
                        order.DownCycledId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "↓普通会员")).Id;
                    }
                }
                if (user.LevelId == levelId2 && !user.IsReturned)
                {
                    if (totalReturnAmount / totalAmount >= (decimal)0.5)
                    {
                        order.DownCycledId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "↓普通会员")).Id;
                    }
                }
                await dbc.SaveChangesAsync();
                return 1;
            }
        }

        public async Task<long> ReturnAsync(long orderId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity order = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(o=>o.Id==orderId);
                if(order==null)
                {
                    return -1;
                }

                var journals = await dbc.GetAll<JournalEntity>().Where(j => j.OrderCode == order.Code && j.JournalType.Name == "佣金收入" && j.IsEnabled == false).ToListAsync();
                foreach (JournalEntity journal1 in journals)
                {
                    UserEntity user1 = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == journal1.UserId);
                    user1.FrozenAmount = user1.FrozenAmount - journal1.InAmount.Value;
                    //journal1.IsEnabled = true;
                    journal1.IsDeleted = true;
                }

                order.OrderStateId= (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "退货完成")).Id;
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u=>u.Id==order.BuyerId);
                if(user==null)
                {
                    return -3;
                }

                JournalEntity journal2 = await dbc.GetAll<JournalEntity>().SingleOrDefaultAsync(j => j.UserId == user.Id && j.OrderCode == order.Code && j.JournalType.Name == "购物");
                if (journal2 == null)
                {
                    return -4;
                }
                //会员扣除金额、降级
                user.Amount = user.Amount + order.RefundAmount.Value;
                user.BuyAmount = user.BuyAmount - journal2.OutAmount.Value;
                //普通会员id
                long levelId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "普通会员")).Id;
                ////黄金会员id
                //long levelId1 = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "黄金会员")).Id;
                ////铂金会员id
                //long levelId2 = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "铂金会员")).Id;

                if (order.DownCycledId == (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "↓普通会员")).Id)
                {
                    user.LevelId = levelId;
                    user.IsReturned = true;
                }

                //添加流水记录
                JournalEntity journal = new JournalEntity();
                journal.OrderCode = order.Code;
                journal.UserId = user.Id;
                journal.InAmount = order.RefundAmount;
                journal.Remark = "退货退款";
                journal.JournalTypeId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "退货退款")).Id;
                journal.BalanceAmount = user.Amount;
                dbc.Journals.Add(journal);
                await dbc.SaveChangesAsync();
                return 1;
            }
        }

        public async Task<long> RefundAuditAsync(long orderId, long adminId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity order = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(o => o.Id == orderId);
                if (order == null)
                {
                    return -1;
                }
                order.AuditStatusId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "已审核")).Id;
                order.OrderStateId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "退单中")).Id;
                order.AuditMobile = (await dbc.GetAll<AdminEntity>().SingleOrDefaultAsync(a => a.Id == adminId)).Mobile;
                order.AuditTime = DateTime.Now;
                await dbc.SaveChangesAsync();
                return order.Id;
            }
        }

        public async Task<long> ReturnAuditAsync(long orderId, long adminId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity order = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(o => o.Id == orderId);
                if (order == null)
                {
                    return -1;
                }
                order.AuditStatusId= (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "已审核")).Id;
                order.OrderStateId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "退货中")).Id;
                order.AuditMobile = (await dbc.GetAll<AdminEntity>().SingleOrDefaultAsync(a => a.Id == adminId)).Mobile;
                order.AuditTime = DateTime.Now;
                await dbc.SaveChangesAsync();
                return order.Id;
            }
        }

        public async Task AutoConfirm()
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                long stateId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "已完成")).Id;
                string val= (await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(i => i.Name == "自动确认收货时间")).Parm;
                Expression<Func<OrderEntity, bool>> timewhere = r => r.ConsignTime == null ? false : r.ConsignTime.Value.AddDays(Convert.ToDouble(val)) < DateTime.Now;
                var orders = dbc.GetAll<OrderEntity>().Where(r=>r.OrderState.Name=="已发货").Where(timewhere.Compile()).ToList();
                foreach(OrderEntity order in orders)
                {
                    order.EndTime = DateTime.Now;
                    order.OrderStateId = stateId;
                }
                val = (await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(i => i.Name == "不能退货时间")).Parm;
                timewhere = r => r.EndTime == null ? false : r.EndTime.Value.AddDays(Convert.ToDouble(val)) < DateTime.Now;
                orders = dbc.GetAll<OrderEntity>().Where(r => r.OrderState.Name == "已完成" || r.OrderState.Name == "退货审核").Where(timewhere.Compile()).ToList();
                foreach (OrderEntity order in orders)
                {
                    order.OrderStateId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "已完成")).Id;
                    //UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u=>u.Id==order.BuyerId);
                    var journals = await dbc.GetAll<JournalEntity>().Where(j => j.OrderCode == order.Code && j.JournalType.Name == "佣金收入" && j.IsEnabled==false).ToListAsync();
                    foreach(JournalEntity journal in journals)
                    {
                        UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u=>u.Id==journal.UserId);
                        user.Amount = user.Amount + journal.InAmount.Value;
                        user.FrozenAmount = user.FrozenAmount - journal.InAmount.Value;
                        journal.BalanceAmount = user.Amount;
                        journal.IsEnabled = true;
                    }
                }
                await dbc.SaveChangesAsync();
            }
        }

        public async Task<long> ValidOrder(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity order = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(o => o.Id == id);
                if (order == null)
                {
                    return -1;
                }
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == order.BuyerId);
                if (user == null)
                {
                    return -2;
                }
                var orderlists = dbc.GetAll<OrderListEntity>().Where(o => o.OrderId == order.Id).ToList();
                foreach (var orderlist in orderlists)
                {
                    GoodsEntity goods = await dbc.GetAll<GoodsEntity>().SingleOrDefaultAsync(g => g.Id == orderlist.GoodsId);
                    if (goods == null)
                    {
                        continue;
                    }

                    if (goods.Inventory < orderlist.Number)
                    {
                        return -3;
                    }
                }
                return 1;
            }
        }
    }
}
