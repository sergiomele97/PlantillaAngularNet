using Microsoft.AspNetCore.Mvc;
using Plantilla._2_Servicios;
using Plantilla.Modelos.DTOs;
using Plantilla.Services;


namespace MyApp.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly TokenService _tokenService;

        public UserController(IUserService userService, TokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService; 
        }

        // REGISTRO
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new ApiResponse<object>(false, "Validation failed", null, errors));
            }

            var createdUser = await _userService.CreateUserAsync(userDto);
            var createdUserDto = UserMapper.ToDto(createdUser);

            return CreatedAtAction("CreateUser", new ApiResponse<UserDto>(true, "User created successfully", createdUserDto));
        }

        // GET USER
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);

            if (user == null)
            {
                return NotFound(new ApiResponse<object>(false, "User not found", null));
            }

            var userDto = UserMapper.ToDto(user);
            return Ok(new ApiResponse<UserDto>(true, "User retrieved successfully", userDto));
        }

        // LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new ApiResponse<object>(false, "Validation failed", null, errors));
            }

            var user = await _userService.AuthenticateAsync(userDto.Email, userDto.Password); 

            if (user == null)
            {
                return Unauthorized(new ApiResponse<object>(false, "Invalid credentials", null));
            }

            // Generar el token usando TokenService
            var token = _tokenService.GenerateToken(user.Email); 
            return Ok(new ApiResponse<string>(true, "Login successful", token)); 
        }
    }
}
