using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class OrderConfig : EntityTypeConfiguration<OrderEntity>
    {
        public OrderConfig()
        {
            ToTable("tb_orders");
            Property(p => p.Code).HasMaxLength(50).IsRequired();
            Property(p => p.BuyerMessage).HasMaxLength(156);
            HasRequired(p => p.Buyer).WithMany().HasForeignKey(p => p.BuyerId).WillCascadeOnDelete(false);
            HasRequired(p => p.PayType).WithMany().HasForeignKey(p => p.PayTypeId).WillCascadeOnDelete(false);
            HasRequired(p => p.OrderState).WithMany().HasForeignKey(p => p.OrderStateId).WillCascadeOnDelete(false);
        }
    }
}
