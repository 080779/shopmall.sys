using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class JournalConfig:EntityTypeConfiguration<JournalEntity>
    {
        public JournalConfig()
        {
            ToTable("T_Journals");
            HasRequired(j => j.PlatformUser).WithMany().HasForeignKey(j => j.PlatformUserId).WillCascadeOnDelete(false);
            HasRequired(j => j.ToPlatformUser).WithMany().HasForeignKey(j => j.ToPlatformUserId).WillCascadeOnDelete(false);
            HasRequired(j => j.FormPlatformUser).WithMany().HasForeignKey(j => j.FormPlatformUserId).WillCascadeOnDelete(false);
            HasRequired(j => j.IntegralType).WithMany().HasForeignKey(j => j.IntegralTypeId).WillCascadeOnDelete(false);
            HasRequired(j => j.ToIntegralType).WithMany().HasForeignKey(j => j.ToIntegralTypeId).WillCascadeOnDelete(false);
            HasRequired(j => j.JournalType).WithMany().HasForeignKey(j => j.JournalTypeId).WillCascadeOnDelete(false);
            Property(j => j.Journal01).HasMaxLength(100);
            Property(j => j.Description).HasMaxLength(100);
            Property(j => j.Tip).HasMaxLength(100);
        }
    }
}
