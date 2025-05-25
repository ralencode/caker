using Caker.Dto;
using Caker.Models;
using Caker.Repositories;
using Caker.Services.PasswordService;
using Microsoft.AspNetCore.Mvc;

namespace Caker.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController(UserRepository repository, IPasswordService passwordService)
        : BaseController<User, UserResponse, CreateUserRequest, UpdateUserRequest>(repository)
    {
        readonly IPasswordService _passwordService = passwordService;

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

        protected override UserResponse ToDto(User model)
        {
            return new(
                model.Id,
                model.Name,
                model.PhoneNumber,
                model.Email,
                model.Type,
                model.Confectioner?.Description,
                model.Confectioner?.Address
            );
        }
    }
}
