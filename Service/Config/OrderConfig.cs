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
            Property(p => p.Deliver).HasMaxLength(50);
            Property(p => p.DeliverName).HasMaxLength(50);
            Property(p => p.DeliverCode).HasMaxLength(150);
            HasRequired(p => p.Buyer).WithMany().HasForeignKey(p => p.BuyerId).WillCascadeOnDelete(false);
            HasRequired(p => p.PayType).WithMany().HasForeignKey(p => p.PayTypeId).WillCascadeOnDelete(false);
            HasRequired(p => p.OrderState).WithMany().HasForeignKey(p => p.OrderStateId).WillCascadeOnDelete(false);
            HasRequired(p => p.Address).WithMany().HasForeignKey(p => p.AddressId).WillCascadeOnDelete(false);
            HasRequired(p => p.AuditStatus).WithMany().HasForeignKey(p => p.AuditStatusId).WillCascadeOnDelete(false);
            HasRequired(p => p.DownCycled).WithMany().HasForeignKey(p => p.DownCycledId).WillCascadeOnDelete(false);
        }
    }
}
