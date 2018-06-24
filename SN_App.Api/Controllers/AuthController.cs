using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SN_App.Api.Dtos;
using SN_App.Repo.Data.Repositories.Authentication;
using SN_App.Repo.Models;

namespace SN_App.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthRepository _authRepository;
        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto userRegistrationDto)
        {
    
            if (await _authRepository.UserExists(userRegistrationDto.Username))
                ModelState.AddModelError("User Exists", "This username is already taken!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userToCreate = new User
            {
                Username = userRegistrationDto.Username.ToLower()
            };

            var createdUser = await _authRepository.Register(userToCreate, userRegistrationDto.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userRegistrationDto)
        {
            return Ok();
        }
    }
}