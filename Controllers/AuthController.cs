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
        public async Task<ActionResult<UserResponse>> Login([FromBody] LoginRequest request)
        {
            var user = await _userRepo.GetByPhoneNumber(request.Phone);
            if (user == null)
                return NotFound();
            if (!_passwordService.VerifyPassword(request.Password, user.Password))
                return Unauthorized();

            var accesToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = await CreateRefreshToken(user);

            SetTokenCookies(accesToken, refreshToken.Token);
            return Ok(user.ToDto());
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

            if (request.Type == UserType.CONFECTIONER)
            {
                user.Confectioner = new Confectioner
                {
                    UserId = user.Id,
                    Description = request.Description ?? "",
                    Address = request.Address ?? "",
                };
                await _confectionerRepo.Create(user.Confectioner);
            }

            if (request.Type == UserType.CUSTOMER)
            {
                user.Customer = new Customer { UserId = user.Id };
                await _customerRepo.Create(user.Customer);
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = await CreateRefreshToken(user);

            SetTokenCookies(accessToken, refreshToken.Token);

            return CreatedAtAction(nameof(Login), user.ToDto());
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();

            var storedToken = await _refreshTokenRepo.GetByToken(refreshToken);
            if (storedToken == null || storedToken.IsExpired || !storedToken.IsActive)
                return Unauthorized();

            var user = await _userRepo.GetById(storedToken.UserId);
            if (user == null)
                return Unauthorized();

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = await CreateRefreshToken(user);
            await RevokeRefreshToken(storedToken);

            SetTokenCookies(newAccessToken, newRefreshToken.Token);
            return Ok();
        }

        [HttpPost("revoke")]
        [Authorize]
        public async Task<IActionResult> RevokeToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest();

            var storedToken = await _refreshTokenRepo.GetByToken(refreshToken);
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

        private void SetTokenCookies(string accessToken, string refreshToken)
        {
            Response.Cookies.Append(
                "accessToken",
                accessToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(
                        _config.GetValue<int>("Jwt:AccessTokenExpiryMinutes")
                    ),
                }
            );

            Response.Cookies.Append(
                "refreshToken",
                refreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(
                        _config.GetValue<int>("Jwt:RefreshTokenExpiryDays")
                    ),
                }
            );
        }
    }
}
