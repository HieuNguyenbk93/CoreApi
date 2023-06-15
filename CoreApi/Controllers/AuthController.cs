using CoreApi.Dto.AuthDto;
using CoreApi.Repositories.AuthRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> SignUp(RegisterDto registerDto)
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
    }
}
