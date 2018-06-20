using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class SettingConfig:EntityTypeConfiguration<SettingEntity>
    {
        public SettingConfig()
        {
            ToTable("T_Settings");
            Property(s => s.Name).HasMaxLength(30).IsRequired();
            Property(s => s.Description).HasMaxLength(100);
            HasRequired(s => s.SettingType).WithMany().HasForeignKey(s => s.SettingTypeId).WillCascadeOnDelete(false);
        }
    }
}
