using System.Text.Json;
using Caker.Dto;
using Caker.Models;
using Caker.Repositories;
using Caker.Services.CurrentUserService;
using Caker.Services.PasswordService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Caker.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController(
        UserRepository repository,
        IPasswordService passwordService,
        ICurrentUserService currentUserService
    )
        : BaseController<User, UserResponse, CreateUserRequest, UpdateUserRequest>(
            repository,
            currentUserService
        )
    {
        private readonly IPasswordService _passwordService = passwordService;
        private readonly ICurrentUserService _currUserService = currentUserService;
        private readonly UserRepository _repo = repository;

        [Authorize]
        [HttpGet("current")]
        public async Task<ActionResult<IEnumerable<CakeResponse>>> GetCurrent()
        {
            try
            {
                var userId = _currUserService.GetUserId();
                var result = await _repo.GetById(userId);
                return Ok(result);
            }
            catch
            {
                return Forbid();
            }
        }

        [HttpPut("self")]
        public virtual async Task<ActionResult<UserResponse>> Update(
            [FromBody] UpdateUserRequest dto
        )
        {
            int userId;
            try
            {
                userId = _currUserService.GetUserId();
            }
            catch
            {
                return Forbid();
            }
            return await Update(userId, dto);
        }

        [HttpPatch("self")]
        public virtual async Task<ActionResult<UserResponse>> PartialUpdate(
            [FromBody] JsonElement patchDoc
        )
        {
            int userId;
            try
            {
                userId = _currUserService.GetUserId();
            }
            catch
            {
                return Forbid();
            }
            return await PartialUpdate(userId, patchDoc);
        }

        [HttpDelete("self")]
        public virtual async Task<IActionResult> Delete()
        {
            try
            {
                var userId = _currUserService.GetUserId();
                var result = await _repo.GetById(userId);
                await _repo.Delete(userId);
                return NoContent();
            }
            catch
            {
                return Forbid();
            }
        }

        protected override User CreateModel(CreateUserRequest dto) =>
            new()
            {
                Name = dto.Name,
                PhoneNumber = dto.Phone,
                Email = dto.Email,
                Password = _passwordService.HashPassword(dto.Password),
                Type = dto.Type,
            };

        protected override void UpdateModel(User model, UpdateUserRequest dto)
        {
            if (dto.Name != null)
                model.Name = dto.Name;
            if (dto.Phone != null)
                model.PhoneNumber = dto.Phone;
            if (dto.Email != null)
                model.Email = dto.Email;
            if (dto.Type.HasValue)
                model.Type = dto.Type.Value;
        }
    }
}
