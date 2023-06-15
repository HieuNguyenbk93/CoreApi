using CoreApi.Dto;
using CoreApi.Entities;
using Microsoft.AspNetCore.Identity;

namespace CoreApi.Repositories.UserRepo
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }
    }
}
