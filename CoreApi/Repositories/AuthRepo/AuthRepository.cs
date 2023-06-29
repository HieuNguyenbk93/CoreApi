using Azure.Core;
using CoreApi.Dto.AuthDto;
using CoreApi.Entities;
using CoreApi.Helpers;
using CoreApi.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CoreApi.Repositories.AuthRepo
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly MyDbContext _context;
        private readonly RoleManager<ApplicationRole> _roleManager;

        private readonly byte[] AccessSecret;
        private readonly byte[] RefreshSecret;
        private readonly string Issuer;
        private readonly string Audience;
        private readonly int AccessExpirationTime;
        private readonly int RefreshExpirationTime;

        public AuthRepository(UserManager<ApplicationUser> userManager, IConfiguration configuration, MyDbContext context, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
            _roleManager = roleManager;

            Issuer = _configuration["JWT:ValidIssuer"];
            Audience = _configuration["JWT:ValidAudience"];
            AccessSecret = Encoding.UTF8.GetBytes(_configuration["JWT:AccessSecret"]);
            AccessExpirationTime = Int32.Parse(_configuration["JWT:TokenValidityInMinutes"]);
            RefreshSecret = Encoding.UTF8.GetBytes(_configuration["JWT:RefreshSecret"]);
            RefreshExpirationTime = Int32.Parse(_configuration["JWT:RefreshTokenValidityInDays"]);
        }

        public async Task<AuthResult<TokenDto>> Login(LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                return AuthResult<TokenDto>.UnvalidatedResult;
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user != null && !string.IsNullOrEmpty(user.Id) && user.IsVisible)
            {
                if (await _userManager.CheckPasswordAsync(user, loginDto.Password))
                {
                    var accessToken = await CreateJwtToken(user, null);
                    var refreshToken = CreateRefreshToken();

                    user.RefreshToken = refreshToken;
                    await _userManager.UpdateAsync(user);

                    var token = new TokenDto
                    {
                        Expires_in = TimeSpan.FromMinutes(AccessExpirationTime).TotalMilliseconds,
                        Access_token = accessToken,
                        Refresh_token = refreshToken
                    };
                    return AuthResult<TokenDto>.TokenResult(token);
                }
            }

            return AuthResult<TokenDto>.UnauthorizedResult;
        }

        public async Task<AuthResult<TokenDto>> Refresh(TokenRefreshDto token)
        {
            if (token == null || string.IsNullOrEmpty(token.Access_token) || string.IsNullOrEmpty(token.Refresh_token))
                return AuthResult<TokenDto>.UnvalidatedResult;
            var principal = GetPrincipleFromExpiredToken(token.Access_token);
            if (principal == null) return AuthResult<TokenDto>.UnvalidatedResult;
            var username = principal.Name;
            var user = await _userManager.FindByNameAsync(username);
            if (user == null || user.RefreshToken != token.Refresh_token) return AuthResult<TokenDto>.UnvalidatedResult;
            var newAccessToken = await CreateJwtToken(user, principal);
            var newRefreshToken = CreateRefreshToken();
            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);
            var newToken = new TokenDto
            {
                Expires_in = TimeSpan.FromMinutes(AccessExpirationTime).TotalMilliseconds,
                Access_token = newAccessToken,
                Refresh_token = newRefreshToken
            };
            return AuthResult<TokenDto>.TokenResult(newToken);
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
            var result = await _userManager.CreateAsync(newUser, registerDto.Password.Trim());
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, "user");
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
        private async Task<string> CreateJwtToken(ApplicationUser user, ClaimsIdentity? claimsIdentity)
        {
            var _accessKey = new SymmetricSecurityKey(AccessSecret);
            var _accessTokenDescriptor = new SecurityTokenDescriptor();
            if (claimsIdentity == null)
            {
                _accessTokenDescriptor.Issuer = Issuer;
                _accessTokenDescriptor.Audience = Audience;
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, $"{user.UserName}"),
                    new Claim(ClaimTypes.GivenName, $"{user.FullName}"),
                    new Claim(ClaimTypes.Email, $"{user.Email}")
                });
                var roleByUser = await _userManager.GetRolesAsync(user);
                foreach (var role in roleByUser)
                {
                    var roleIdentity = await _roleManager.FindByNameAsync(role);
                    if (roleIdentity != null && roleIdentity.IsVisible)
                    {
                        identity.AddClaim(new Claim("role", role));
                    }
                }
                claimsIdentity = identity;
            }
            var credentialsAccess = new SigningCredentials(_accessKey, SecurityAlgorithms.HmacSha512Signature);
            _accessTokenDescriptor.Expires = DateTime.UtcNow.AddMinutes(AccessExpirationTime);
            _accessTokenDescriptor.SigningCredentials = credentialsAccess;
            _accessTokenDescriptor.Subject = claimsIdentity;
            //var _accessTokenDescriptor = new SecurityTokenDescriptor
            //{
            //    Issuer = Issuer,
            //    Audience = Audience,
            //    Subject = claimsIdentity,
            //    Expires = DateTime.UtcNow.AddMinutes(AccessExpirationTime),
            //    SigningCredentials = credentialsAccess,
            //};
            var _accesToken = new JwtSecurityTokenHandler().CreateToken(_accessTokenDescriptor);
            return new JwtSecurityTokenHandler().WriteToken(_accesToken);
        }
        private string CreateRefreshToken()
        {
            var _refreshKey = new SymmetricSecurityKey(RefreshSecret);
            var _refreshToken = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                expires: DateTime.UtcNow.AddDays(RefreshExpirationTime),
                signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(_refreshKey, SecurityAlgorithms.HmacSha512Signature)
            );
            return new JwtSecurityTokenHandler().WriteToken(_refreshToken);
        }

        private ClaimsIdentity GetPrincipleFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(AccessSecret),
                ValidateLifetime = true,
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }
            return principal.Identities.FirstOrDefault();
        }
    }
}
