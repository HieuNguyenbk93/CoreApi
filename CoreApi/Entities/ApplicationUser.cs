using Microsoft.AspNetCore.Identity;

namespace CoreApi.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public bool IsVisible { get; set; }
        public string? RefreshToken { get; set; }
        public string? ResetPasswordToken { get; set; }
    }
}
