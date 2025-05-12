using Caker.Dto;
using Caker.Models;
using Caker.Repositories;
using Caker.Services.PasswordService;
using Microsoft.AspNetCore.Mvc;

namespace Caker.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository _userRepo;
        private readonly ConfectionerRepository _confectionerRepo;
        private readonly CustomerRepository _customerRepo;
        private readonly IPasswordService _passwordService;

        public AuthController(
            UserRepository userRepo,
            ConfectionerRepository confectionerRepo,
            CustomerRepository customerRepo,
            IPasswordService passwordService
        )
        {
            _userRepo = userRepo;
            _confectionerRepo = confectionerRepo;
            _customerRepo = customerRepo;
            _passwordService = passwordService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserResponse>> Login(LoginRequest request)
        {
            var user = await _userRepo.GetByPhoneNumber(request.Phone);
            if (user == null)
                return NotFound();

            if (!_passwordService.VerifyPassword(request.Password, user.Password))
                return Unauthorized();

            return Ok(MapToUserResponse(user));
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponse>> Register(RegisterRequest request)
        {
            if (await _userRepo.GetByPhoneNumber(request.Phone) != null)
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
                await _confectionerRepo.Create(
                    new Confectioner
                    {
                        UserId = user.Id!.Value,
                        Description = request.Description ?? "",
                        Address = request.Address ?? "",
                    }
                );
            }
            else if (request.Type == UserType.Customer)
            {
                await _customerRepo.Create(new Customer { UserId = user.Id!.Value });
            }

            return CreatedAtAction(nameof(Login), MapToUserResponse(user));
        }

        private static UserResponse MapToUserResponse(User user) =>
            new()
            {
                Id = user.Id!.Value,
                Name = user.Name,
                Phone = user.PhoneNumber,
                Email = user.Email,
                Type = user.Type,
                Description = user.Confectioner?.Description,
                Address = user.Confectioner?.Address,
            };
    }
}
