using Caker.Dto;
using Caker.Models;
using Caker.Repositories;
using Caker.Services.PasswordService;
using Microsoft.AspNetCore.Mvc;

namespace Caker.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(
        UserRepository userRepo,
        CustomerRepository customerRepo,
        ConfectionerRepository confectionerRepo,
        IPasswordService passwordService
    ) : ControllerBase
    {
        private readonly UserRepository _userRepo = userRepo;
        private readonly CustomerRepository _customerRepo = customerRepo;
        private readonly ConfectionerRepository _confectionerRepo = confectionerRepo;
        private readonly IPasswordService _passwordService = passwordService;

        [HttpPost("login")]
        public async Task<ActionResult<UserResponse>> Login([FromBody] LoginRequest request)
        {
            var user = await _userRepo.GetByPhoneNumber(request.Phone);
            if (user == null)
                return NotFound();
            if (!_passwordService.VerifyPassword(request.Password, user.Password))
                return Unauthorized();

            return Ok(MapToResponse(user));
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponse>> Register([FromBody] RegisterRequest request)
        {
            var existingUser = await _userRepo.GetByPhoneNumber(request.Phone);
            if (existingUser != null)
                return Conflict("Phone already exists");

            var user = new User
            {
                Name = request.Name,
                PhoneNumber = request.Phone,
                Email = request.Email,
                Password = _passwordService.HashPassword(request.Password),
                Type = request.Type,
            };

            await _userRepo.Create(user);

            if (request.Type == UserType.Confectioner)
            {
                user.Confectioner = new Confectioner
                {
                    UserId = user.Id!.Value,
                    Description = request.Description ?? "",
                    Address = request.Address ?? "",
                };
                await _confectionerRepo.Create(user.Confectioner);
            }

            if (request.Type == UserType.Customer)
            {
                user.Customer = new Customer { UserId = user.Id!.Value };
                await _customerRepo.Create(user.Customer);
            }

            return CreatedAtAction(nameof(Login), MapToResponse(user));
        }

        private static UserResponse MapToResponse(User user) =>
            new(
                user.Id!.Value,
                user.Name,
                user.PhoneNumber,
                user.Email,
                user.Type,
                user.Confectioner?.Description,
                user.Confectioner?.Address
            );
    }
}
