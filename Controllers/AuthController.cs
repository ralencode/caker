using Caker.Dto;
using Caker.Models;
using Caker.Repositories;
using Caker.Services.PasswordService;
using Caker.Services.TokenService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Caker.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(
        UserRepository userRepo,
        CustomerRepository customerRepo,
        ConfectionerRepository confectionerRepo,
        RefreshTokenRepository refreshTokenRepo,
        IPasswordService passwordService,
        ITokenService tokenService,
        IConfiguration config
    ) : ControllerBase
    {
        private readonly UserRepository _userRepo = userRepo;
        private readonly CustomerRepository _customerRepo = customerRepo;
        private readonly ConfectionerRepository _confectionerRepo = confectionerRepo;
        private readonly RefreshTokenRepository _refreshTokenRepo = refreshTokenRepo;
        private readonly IPasswordService _passwordService = passwordService;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IConfiguration _config = config;

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            var user = await _userRepo.GetByPhoneNumber(request.Phone);
            if (user == null || !_passwordService.VerifyPassword(request.Password, user.Password))
                return Unauthorized();

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = await CreateRefreshToken(user);

            var userDto = user.ToDto();

            return Ok(
                new AuthResponse(
                    userDto.Id,
                    userDto.Name,
                    userDto.Phone,
                    userDto.Email,
                    userDto.Address,
                    userDto.Description,
                    userDto.Type,
                    userDto.Customer,
                    userDto.Confectioner,
                    accessToken,
                    refreshToken.Token
                )
            );
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
                Description = request.Description ?? "",
                Address = request.Address ?? "",
                Password = _passwordService.HashPassword(request.Password),
                Type = request.Type,
            };

            await _userRepo.Create(user);

            if (request.Type == UserType.CONFECTIONER)
            {
                user.Confectioner = new Confectioner { UserId = user.Id };
                await _confectionerRepo.Create(user.Confectioner);
            }

            if (request.Type == UserType.CUSTOMER)
            {
                user.Customer = new Customer { UserId = user.Id };
                await _customerRepo.Create(user.Customer);
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = await CreateRefreshToken(user);

            var userDto = user.ToDto();

            return CreatedAtAction(
                nameof(Login),
                new AuthResponse(
                    userDto.Id,
                    userDto.Name,
                    userDto.Phone,
                    userDto.Email,
                    userDto.Address,
                    userDto.Description,
                    userDto.Type,
                    userDto.Customer,
                    userDto.Confectioner,
                    accessToken,
                    refreshToken.Token
                )
            );
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                return Unauthorized("Missing Authorization header");

            var headerValue = authorizationHeader.ToString();
            if (!headerValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                return Unauthorized("Invalid token scheme");

            var refreshToken = headerValue["Bearer ".Length..].Trim();

            var storedToken = await _refreshTokenRepo.GetByToken(refreshToken);
            if (storedToken == null || storedToken.IsExpired || !storedToken.IsActive)
                return Unauthorized();

            var user = await _userRepo.GetById(storedToken.UserId);
            if (user == null)
                return Unauthorized();

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = await CreateRefreshToken(user);
            await RevokeRefreshToken(storedToken);

            return Ok(new RefreshResponse(newAccessToken, newRefreshToken.Token));
        }

        [HttpPost("revoke")]
        [Authorize]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequest request)
        {
            var storedToken = await _refreshTokenRepo.GetByToken(request.Token);
            if (storedToken == null)
                return BadRequest();
            await RevokeRefreshToken(storedToken);
            return NoContent();
        }

        private async Task<RefreshToken> CreateRefreshToken(User user)
        {
            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = _tokenService.GenerateRefreshToken(),
                Expires = DateTime.UtcNow.AddDays(
                    _config.GetValue<int>("Jwt:RefreshTokenExpiryDays")
                ),
            };

            await _refreshTokenRepo.Create(refreshToken);
            return refreshToken;
        }

        private async Task RevokeRefreshToken(RefreshToken token)
        {
            token.Revoked = DateTime.UtcNow;
            await _refreshTokenRepo.Update(token);
        }
    }
}
