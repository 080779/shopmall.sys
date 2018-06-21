using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class TakeCashConfig:EntityTypeConfiguration<TakeCashEntity>
    {
        public TakeCashConfig()
        {
            ToTable("tb_takecashes");
            HasRequired(t => t.User).WithMany().HasForeignKey(t => t.UserId).WillCascadeOnDelete(false);
            HasRequired(t => t.State).WithMany().HasForeignKey(t => t.StateId).WillCascadeOnDelete(false);
            Property(t => t.Description).HasMaxLength(100);
        }
    }
}
