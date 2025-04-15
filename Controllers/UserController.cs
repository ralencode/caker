using Caker.Models;
using Caker.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Caker.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController(UserRepository repository) : BaseController<User>(repository)
    {
        // GET api/users/phone/{phone-string}
        [HttpGet("phone/{phoneNumber}")]
        public async Task<ActionResult<User>> GetUserByPhoneNumber(string phoneNumber)
        {
            try
            {
                var user = await repository.GetByPhoneNumber(phoneNumber);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/users/have_chat_with/{user_id}
        [HttpGet("have_chat_with/{userId}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersWithChat(int userId)
        {
            try
            {
                var users = await repository.GetUsersWithChat(userId);
                if (users == null || !users.Any())
                {
                    return NotFound();
                }
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
