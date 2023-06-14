using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApi.Entities.ConfigurationEntities
{
    public class RolePageConfiguration : IEntityTypeConfiguration<RolePage>
    {
        public void Configure(EntityTypeBuilder<RolePage> builder)
        {
            builder.HasKey(rp => new { rp.RoleId, rp.PageId });

            builder.HasOne(rp => rp.Role)
                .WithMany(ar => ar.RolePages)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rp => rp.Menu)
                .WithMany(pm => pm.RolePages)
                .HasForeignKey(rp => rp.PageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
