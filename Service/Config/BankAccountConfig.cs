using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class BankAccountConfig : EntityTypeConfiguration<BankAccountEntity>
    {
        public BankAccountConfig()
        {
            ToTable("tb_bankaccounts");
            Property(p => p.Name).HasMaxLength(50).IsRequired();
            Property(p => p.Account).HasMaxLength(50).IsRequired();
            Property(p => p.AccountName).HasMaxLength(50).IsRequired();
            Property(p => p.Description).HasMaxLength(100);
            HasRequired(p => p.PayCode).WithMany().HasForeignKey(p => p.PayCodeId).WillCascadeOnDelete(false);
        }
    }
}
