using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class IntegralTypeConfig:EntityTypeConfiguration<IntegralTypeEntity>
    {
        public IntegralTypeConfig()
        {
            ToTable("T_IntegralTypes");
            Property(i => i.Name).HasMaxLength(30).IsRequired();
            Property(i => i.Description).HasMaxLength(100);
        }
    }
}
