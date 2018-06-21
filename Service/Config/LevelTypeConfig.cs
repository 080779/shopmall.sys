using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class LevelTypeConfig : EntityTypeConfiguration<LevelTypeEntity>
    {
        public LevelTypeConfig()
        {
            ToTable("tb_leveltypes");
            Property(p => p.Name).HasMaxLength(30).IsRequired();
            Property(p => p.Description).HasMaxLength(100);
        }
    }
}
