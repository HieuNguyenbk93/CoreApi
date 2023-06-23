using CoreApi.Dto.AuthDto;
using CoreApi.Entities;
using CoreApi.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace CoreApi.Repositories.AuthRepo
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;

        public AuthRepository(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }

        public async Task<AuthResult<TokenDto>> Login(LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                return AuthResult<TokenDto>.UnvalidatedResult;
            var user = await userManager.FindByEmailAsync(loginDto.Email);

            if (user != null && !string.IsNullOrEmpty(user.Id) && user.IsVisible)
            {
                if (await userManager.CheckPasswordAsync(user, loginDto.Password))
                {
                    var _issuer = configuration["JWT:ValidIssuer"];
                    var _audience = configuration["JWT:ValidAudience"];
                    var _accessSecret = Encoding.UTF8.GetBytes(configuration["JWT:AccessSecret"]);
                    var _refreshSecret = Encoding.UTF8.GetBytes(configuration["JWT:RefreshSecret"]);
                    var _accessExprire = Int32.Parse(configuration["JWT:TokenValidityInMinutes"]);
                    var _refreshExprire = Int32.Parse(configuration["JWT:RefreshTokenValidityInDays"]);
                    var _accessKey = new SymmetricSecurityKey(_accessSecret);
                    var _refreshKey = new SymmetricSecurityKey(_refreshSecret);

                    var _accessToken = new JwtSecurityToken(
                        issuer: _issuer,
                        audience: _audience,
                        expires: DateTime.UtcNow.AddMinutes(_accessExprire),
                        signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(_accessKey, SecurityAlgorithms.HmacSha512Signature)
                        );
                    var accesToken = new JwtSecurityTokenHandler().WriteToken(_accessToken);

                    var _refreshToken = new JwtSecurityToken(
                        issuer: _issuer,
                        audience: _audience,
                        expires: DateTime.UtcNow.AddMinutes(_refreshExprire),
                        signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(_refreshKey, SecurityAlgorithms.HmacSha512Signature)
                        );
                    var refreshToken = new JwtSecurityTokenHandler().WriteToken(_refreshToken);

                    var token = new TokenDto
                    {
                        Expires_in = TimeSpan.FromMinutes(_accessExprire).TotalMilliseconds,
                        Access_token = accesToken,
                        Refresh_token = refreshToken
                    };
                    return AuthResult<TokenDto>.TokenResult(token);
                }
            }

            return AuthResult<TokenDto>.UnauthorizedResult;
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
