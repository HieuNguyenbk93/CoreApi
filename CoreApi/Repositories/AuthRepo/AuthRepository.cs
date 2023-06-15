using CoreApi.Dto.AuthDto;
using CoreApi.Entities;
using CoreApi.Model;
using Microsoft.AspNetCore.Identity;

namespace CoreApi.Repositories.AuthRepo
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<ApplicationUser> userManager;

        public AuthRepository(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<AuthResult<TokenDto>> Register(RegisterDto registerDto)
        {
            if (registerDto == null
                || string.IsNullOrEmpty(registerDto.FullName.Trim())
                || string.IsNullOrEmpty(registerDto.Email.Trim())
                || string.IsNullOrEmpty(registerDto.UserName)
                || string.IsNullOrEmpty(registerDto.Password)
                || string.IsNullOrEmpty(registerDto.ConfirmPassword)
                || registerDto.Password.Trim() != registerDto.ConfirmPassword.Trim()
                )
                return AuthResult<TokenDto>.UnvalidatedResult;

            var newUser = new ApplicationUser
            {
                FullName = registerDto.FullName.Trim(),
                UserName = registerDto.UserName.Trim(),
                Email = registerDto.Email.Trim(),
            };
            var result = await userManager.CreateAsync(newUser, registerDto.Password.Trim());
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newUser, "user");
                return AuthResult<TokenDto>.SucceededResult;
            }
            else
            {
                var errors = result.Errors;
                if (errors != null)
                {
                    return AuthResult<TokenDto>.UnsucceededResult(errors);
                }
                return AuthResult<TokenDto>.UnauthorizedResult;
            }
        }
        
    }
}
