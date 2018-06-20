using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class TakeCashConfig:EntityTypeConfiguration<TakeCashEntity>
    {
        public TakeCashConfig()
        {
            ToTable("T_TakeCashes");
            HasRequired(t => t.PlatformUser).WithMany().HasForeignKey(t => t.PlatformUserId).WillCascadeOnDelete(false);
            HasRequired(t => t.IntegralType).WithMany().HasForeignKey(t => t.IntegralTypeId).WillCascadeOnDelete(false);
            HasRequired(t => t.State).WithMany().HasForeignKey(t => t.StateId).WillCascadeOnDelete(false);
            Property(t => t.Description).HasMaxLength(100);
        }
    }
}
