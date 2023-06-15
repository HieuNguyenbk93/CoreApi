using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace CoreApi.Entities.ConfigurationEntities
{
    public class PageMenuConfiguration : IEntityTypeConfiguration<PageMenu>
    {
        public void Configure(EntityTypeBuilder<PageMenu> builder)
        {
            builder.ToTable("PageMenu");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(256);
            builder.Property(x => x.Link).IsRequired().HasMaxLength(512);
            builder.Property(x => x.NumIndex).IsRequired();
            builder.Property(x => x.Level).IsRequired();
            builder.Property(x => x.ParentId).IsRequired();
            builder.Property(x => x.IsVisible).IsRequired();
            builder.HasIndex(x => x.Title).IsUnique();
        }
    }
}
