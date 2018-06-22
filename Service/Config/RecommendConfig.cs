using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class RecommendConfig : EntityTypeConfiguration<RecommendEntity>
    {
        public RecommendConfig()
        {
            ToTable("tb_recommends");
            HasRequired(p => p.User).WithMany().HasForeignKey(p => p.UserId).WillCascadeOnDelete(false);
            HasRequired(p => p.User).WithMany().HasForeignKey(p => p.UserId).WillCascadeOnDelete(false);
        }
    }
}
