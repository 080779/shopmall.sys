using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class SettingTypeConfig:EntityTypeConfiguration<SettingTypeEntity>
    {
        public SettingTypeConfig()
        {
            ToTable("tb_settingtypes");
            Property(s => s.Name).HasMaxLength(30).IsRequired();
            Property(s => s.Description).HasMaxLength(100);
        }
    }
}
