using log4net;
using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service
{
    public class MyDbContext:DbContext
    {
        private static ILog log = LogManager.GetLogger(typeof(MyDbContext));
        public MyDbContext() : base("name=connStr") //“connStr”数据库连接字符串
        {
            this.Database.Log = sql => log.DebugFormat("EF执行SQL：{0}", sql);//用log4NET记录数据操作日志            
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
        public DbSet<NavBarEntity> NavBars { get; set; }
        public DbSet<AdminEntity> Admins { get; set; }
        public DbSet<AdminLogEntity> AdminLogs { get; set; }
        public DbSet<IntegralTypeEntity> IntegralTypes { get; set; }
        public DbSet<JournalEntity> Journals { get; set; }
        public DbSet<JournalTypeEntity> JournalTypes { get; set; }
        public DbSet<PermissionEntity> Permissions { get; set; }
        public DbSet<PermissionTypeEntity> PermissionTypes { get; set; }
        public DbSet<PlatformUserEntity> PlatformUsers { get; set; }
        public DbSet<PlatformUserTypeEntity> PlatformUserTypes { get; set; }
        public DbSet<StateEntity> States { get; set; }
        public DbSet<SettingEntity> Settings { get; set; }
        public DbSet<SettingTypeEntity> SettingTypes { get; set; }
        public DbSet<TakeCashEntity> TakeCashes { get; set; }
    }
}
