using Microsoft.AspNetCore.Identity;

namespace CoreApi.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public bool IsVisible { get; set; }
        public IList<RolePage> RolePages { get; set; }
    }
}
