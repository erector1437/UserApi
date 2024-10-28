using Microsoft.AspNetCore.Mvc;
using UserApi.Models;
using UserApi.Services;
using UserApi.Validators;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace UserApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IValidator<User> _validator;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, IValidator<User> validator, ILogger<UsersController> logger)
        {
            _userService = userService;
            _validator = validator;
            _logger = logger;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            if (users.Any())
            {
                return Ok(users.Select(user => new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.DateOfBirth,
                    Age = CalculateAge(user.DateOfBirth),
                    user.PhoneNumber
                }));
            }
            else 
            {                 
                _logger.LogWarning("No users found.");
                return NotFound();
            }
           
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", id);
                return NotFound();
            }

            return Ok(new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.DateOfBirth,
                Age = CalculateAge(user.DateOfBirth),
                user.PhoneNumber
            });
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            ValidationResult result = await _validator.ValidateAsync(user);
            if (!result.IsValid)
            {
                _logger.LogWarning("Validation failed for user: {User}", user);
                return BadRequest(result.Errors);
                
            }

            var existingUser = await _userService.GetUsersAsync();
            if (existingUser.Any(u => u.Email == user.Email))
            {
                _logger.LogWarning("Validation failed for user: {User}", user);
                return BadRequest("Email must be unique.");
                
            }
            try
            {
                await _userService.CreateUserAsync(user);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Post failed for user: {User}", user);
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                _logger.LogWarning("Validation failed for user: {User}", user);
                return BadRequest();
            }

            ValidationResult result = await _validator.ValidateAsync(user);
            if (!result.IsValid)
            {
                _logger.LogWarning("Validation failed for user: {User}", user);
                return BadRequest(result.Errors);
            }

            try
            {
                await _userService.UpdateUserAsync(user);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await _userService.GetUserByIdAsync(id) == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogWarning("Put failed for user: {User}", user);
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", id);
                return NotFound();
            }

            await _userService.DeleteUserAsync(id);
            return NoContent();
        }

        private static int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
