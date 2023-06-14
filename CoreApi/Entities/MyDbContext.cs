using CoreApi.Entities.ConfigurationEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoreApi.Entities
{
    public class MyDbContext : IdentityDbContext<ApplicationUser>
    {
        public MyDbContext(DbContextOptions<MyDbContext> opt): base(opt)
        {
            
        }

        #region Dbset
        public DbSet<PageMenu> PageMenu { get; set; }
        public DbSet<RolePage> RolePage { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PageMenu>(e =>
            {
                e.ToTable("PageMenu");
                e.HasKey(x => x.Id);
                e.Property(x => x.Title).IsRequired().HasMaxLength(256);
                e.Property(x => x.Link).IsRequired().HasMaxLength(512);
                e.Property(x => x.NumIndex).IsRequired();
                e.Property(x => x.Level).IsRequired();
                e.Property(x => x.ParentId).IsRequired();
                e.Property(x => x.IsVisible).IsRequired();
                e.HasIndex(x => x.Title).IsUnique();
            });

            modelBuilder.ApplyConfiguration(new RolePageConfiguration());
        }
    }
}
