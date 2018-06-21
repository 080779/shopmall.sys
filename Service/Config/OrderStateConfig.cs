using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class OrderStateConfig : EntityTypeConfiguration<OrderStateEntity>
    {
        public OrderStateConfig()
        {
            ToTable("tb_orderstates");
            Property(p => p.Name).HasMaxLength(30).IsRequired();
            Property(p => p.Description).HasMaxLength(100);
        }
    }
}
