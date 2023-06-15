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

            modelBuilder.ApplyConfiguration(new PageMenuConfiguration());
            modelBuilder.ApplyConfiguration(new RolePageConfiguration());
        }
    }
}
