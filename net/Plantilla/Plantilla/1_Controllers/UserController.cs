using Microsoft.AspNetCore.Mvc;
using Plantilla.Modelos.DTOs;
using Plantilla.Services;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

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

            return CreatedAtAction(nameof(GetUser), new { id = createdUserDto.Email }, new ApiResponse<UserDto>(true, "User created successfully", createdUserDto));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(new ApiResponse<object>(false, "User not found", null));
            }

            var userDto = UserMapper.ToDto(user);
            return Ok(new ApiResponse<UserDto>(true, "User retrieved successfully", userDto));
        }
    }
}
