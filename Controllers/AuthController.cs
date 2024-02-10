using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using TestApi.Services;

namespace TestApi.Controllers
{
    [Route("api/controller")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid) 
            return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(model);
            if(!result.IsAuthenticated)
                return BadRequest(ModelState);
            return Ok(result);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(model);
            if (!result.IsAuthenticated)
                return BadRequest(ModelState);
            return Ok(result);
        }
    }
}
