using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class StateConfig:EntityTypeConfiguration<StateEntity>
    {
        public StateConfig()
        {
            ToTable("T_States");
            Property(s => s.Name).HasMaxLength(30).IsRequired();
            Property(s => s.Description).HasMaxLength(100);
        }
    }
}
