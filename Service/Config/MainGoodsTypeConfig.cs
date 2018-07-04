using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class MainGoodsTypeConfig : EntityTypeConfiguration<MainGoodsTypeEntity>
    {
        public MainGoodsTypeConfig()
        {
            ToTable("tb_maingoodstypes");
            Property(p => p.Name).HasMaxLength(50).IsRequired();
            Property(p => p.ImgUrl).HasMaxLength(156).IsRequired();
            Property(p => p.Description).HasMaxLength(100);
        }
    }
}
