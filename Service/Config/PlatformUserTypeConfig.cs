using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;
namespace IMS.Service.Config
{
    class PlatformUserTypeConfig:EntityTypeConfiguration<PlatformUserTypeEntity>
    {
        public PlatformUserTypeConfig()
        {
            ToTable("T_PlatformUserTypes");
            Property(p => p.Name).HasMaxLength(30).IsRequired();
            Property(p => p.Description).HasMaxLength(100);
        }
    }
}
