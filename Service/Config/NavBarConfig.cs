using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Config
{
    class NavBarConfig:EntityTypeConfiguration<NavBarEntity>
    {
        public NavBarConfig()
        {
            ToTable("tb_navbars");
            Property(n => n.MenuName).HasMaxLength(50);
            Property(n => n.Url).HasMaxLength(500);
        }
    }
}
