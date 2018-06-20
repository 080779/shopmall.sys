using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class PlatformUserConfig:EntityTypeConfiguration<PlatformUserEntity>
    {
        public PlatformUserConfig()
        {
            ToTable("T_PlatformUsers");
            HasRequired(p => p.PlatformUserType).WithMany().HasForeignKey(p => p.PlatformUserTypeId).WillCascadeOnDelete(false);
            Property(p => p.AdderMobile).HasMaxLength(50).IsRequired();
            Property(p => p.Mobile).HasMaxLength(50).IsRequired();
            Property(p => p.Code).HasMaxLength(100).IsRequired();
            Property(p => p.Description).HasMaxLength(100);
            Property(p => p.Salt).HasMaxLength(20).IsRequired().IsUnicode();
            Property(p => p.Password).HasMaxLength(100).IsRequired().IsUnicode();
            Property(p => p.TradePassword).HasMaxLength(100).IsRequired().IsUnicode();
        }
    }
}
