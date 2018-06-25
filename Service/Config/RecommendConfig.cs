using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class RecommendConfig : EntityTypeConfiguration<RecommendEntity>
    {
        public RecommendConfig()
        {
            ToTable("tb_recommends");
            HasKey(r => r.UserId);
            HasRequired(p => p.User).WithOptional(p => p.Recommend).WillCascadeOnDelete(false);
        }
    }
}
