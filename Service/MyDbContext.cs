﻿using log4net;
using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace IMS.Service
{
    public class MyDbContext:DbContext
    {
        //private static ILog log = LogManager.GetLogger(typeof(MyDbContext));
        public MyDbContext() : base("name=connStr") //“connStr”数据库连接字符串
        {
            //this.Database.Log = sql => log.DebugFormat("EF执行SQL：{0}", sql);//用log4NET记录数据操作日志
            //Database.SetInitializer<MyDbContext>(null);
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());
        }
        public IQueryable<T> GetAll<T>() where T:BaseEntity
        {
            return this.Set<T>().Where(e => e.IsDeleted == false);
        }

        public long GetId<T>(Expression<Func<T, bool>> expression) where T : BaseEntity
        {
            return this.Set<T>().AsNoTracking().Where(e => e.IsDeleted == false).Where(expression).Select(e => e.Id).SingleOrDefault();
        }

        public async Task<long> GetIdAsync<T>(Expression<Func<T, bool>> expression) where T : BaseEntity
        {
            return await this.Set<T>().AsNoTracking().Where(e => e.IsDeleted == false).Where(expression).Select(e => e.Id).SingleOrDefaultAsync();
        }

        public IQueryable<long> GetIds<T>(Expression<Func<T, bool>> expression) where T : BaseEntity
        {
            return this.Set<T>().AsNoTracking().Where(e => e.IsDeleted == false).Where(expression).Select(e => e.Id);
        }

        public string GetParameter<T>(Expression<Func<T, bool>> expression, Expression<Func<T, string>> parameterName) where T : BaseEntity
        {
            return this.Set<T>().AsNoTracking().Where(e => e.IsDeleted == false).Where(expression).Select(parameterName).SingleOrDefault();
        }

        public async Task<string> GetParameterAsync<T>(Expression<Func<T, bool>> expression, Expression<Func<T, string>> parameterName) where T : BaseEntity
        {
            return await this.Set<T>().AsNoTracking().Where(e => e.IsDeleted == false).Where(expression).Select(parameterName).SingleOrDefaultAsync();
        }

        public decimal GetDecimalParameter<T>(Expression<Func<T, bool>> expression, Expression<Func<T, decimal>> parameterName) where T : BaseEntity
        {
            return this.Set<T>().AsNoTracking().Where(e => e.IsDeleted == false).Where(expression).Select(parameterName).SingleOrDefault();
        }

        public async Task<decimal> GetDecimalParameterAsync<T>(Expression<Func<T, bool>> expression, Expression<Func<T, decimal>> parameterName) where T : BaseEntity
        {
            return await this.Set<T>().AsNoTracking().Where(e => e.IsDeleted == false).Where(expression).Select(parameterName).SingleOrDefaultAsync();
        }

        public DbSet<AddressEntity> Addresses { get; set; }
        public DbSet<BankAccountEntity> BankAccounts { get; set; }
        public DbSet<GoodsAreaEntity> GoodsAreas { get; set; }
        public DbSet<GoodsCarEntity> GoodsCars { get; set; }
        public DbSet<GoodsEntity> Goods { get; set; }
        public DbSet<GoodsImgEntity> GoodsImgs { get; set; }
        public DbSet<GoodsSecondTypeEntity> GoodsSecondTypes { get; set; }
        public DbSet<GoodsTypeEntity> GoodsTypes { get; set; }
        public DbSet<LogisticsEntity> Logistics { get; set; }
        public DbSet<NoticeEntity> Notices { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<PayCodeEntity> PayCodes { get; set; }
        public DbSet<SlideEntity> Slides { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<NavBarEntity> NavBars { get; set; }
        public DbSet<AdminEntity> Admins { get; set; }
        public DbSet<AdminLogEntity> AdminLogs { get; set; }
        public DbSet<PermissionEntity> Permissions { get; set; }
        public DbSet<PermissionTypeEntity> PermissionTypes { get; set; }
        public DbSet<SettingEntity> Settings { get; set; }
        public DbSet<TakeCashEntity> TakeCashes { get; set; }
        public DbSet<RecommendEntity> Recommends { get; set; }
        public DbSet<OrderListEntity> OrderLists { get; set; }
        public DbSet<DeliveryEntity> Deliveries { get; set; }
        public DbSet<IdNameEntity> IdNames { get; set; }
        public DbSet<UserTokenEntity> UserTokens { get; set; }
        public DbSet<MessageEntity> Messages { get; set; }
        public DbSet<MainGoodsTypeEntity> MainGoodsTypes { get; set; }
        public DbSet<OrderApplyEntity> OrderApplies { get; set; }
        public DbSet<JournalEntity> Journals { get; set; }
        public DbSet<BonusRatioEntity> BonusRatios { get; set; }
    }
}
