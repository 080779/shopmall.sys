using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class SettingConfig:EntityTypeConfiguration<SettingEntity>
    {
        public SettingConfig()
        {
            ToTable("tb_settings");
            Property(s => s.Name).HasMaxLength(50).IsRequired();
            Property(s => s.Parm).HasMaxLength(50).IsRequired();
            Property(s => s.Description).HasMaxLength(100);
            HasRequired(s => s.SettingType).WithMany().HasForeignKey(s => s.SettingTypeId).WillCascadeOnDelete(false);
        }
    }
}
