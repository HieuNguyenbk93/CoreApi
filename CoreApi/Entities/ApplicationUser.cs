using Microsoft.AspNetCore.Identity;

namespace CoreApi.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public bool IsVisible { get; set; }
    }
}
