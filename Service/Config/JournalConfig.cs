using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class JournalConfig : EntityTypeConfiguration<JournalEntity>
    {
        public JournalConfig()
        {
            ToTable("tb_journals");
            Property(s => s.Remark).HasMaxLength(100);
            Property(s => s.RemarkEn).HasMaxLength(100);
            HasRequired(s => s.JournalType).WithMany().HasForeignKey(s => s.JournalTypeId).WillCascadeOnDelete(false);
            HasRequired(s => s.User).WithMany().HasForeignKey(s => s.UserId).WillCascadeOnDelete(false);
        }
    }
}
