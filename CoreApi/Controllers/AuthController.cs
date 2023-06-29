using CoreApi.Dto.AuthDto;
using CoreApi.Repositories.AuthRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        protected readonly IAuthRepository authRepository;

        public AuthController(IAuthRepository _authRepository)
        {
            this.authRepository = _authRepository;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var result = await authRepository.Register(registerDto);

            if (!result.IsModelValid)
            {
                return BadRequest();
            }
            if (result.Succeeded)
                return Ok(new { token = result.Data });

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm]LoginDto loginDto)
        {
            var result = await authRepository.Login(loginDto);
            if (!result.IsModelValid)
            {
                return BadRequest();
            }
            if (result.Succeeded)
            {
                var token = new { token = result.Data };
                return Ok(token);
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromForm] TokenRefreshDto token)
        {
            var result = await authRepository.Refresh(token);
            if (!result.IsModelValid)
            {
                return BadRequest();
            }
            if (result.Succeeded)
            {
                var newtoken = new { token = result.Data };
                return Ok(newtoken);
            }

            return BadRequest(result.Errors);
        }

        [HttpGet, Authorize]
        public IEnumerable<string> Get()
        {
            return new string[] { "Hieu", "Nguyen" };
        }
    }
}
