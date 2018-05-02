using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using myDotnetApp.API.Data;
using myDotnetApp.API.Dtos;
using myDotnetApp.API.Model;

namespace myDotnetApp.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthRepository _repo;
        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserForRegisterDtos userForRegisterDtos)
        {
            userForRegisterDtos.Username = userForRegisterDtos.Username.ToLower();
            if(await _repo.UserExists(userForRegisterDtos.Username))
            {
                ModelState.AddModelError("Username","Username is Already taken");
                return BadRequest(ModelState);
            }
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }           
            var userToCreate = new User
            {
                Username = userForRegisterDtos.Username
            };
            var createUser = await _repo.Register(userToCreate, userForRegisterDtos.Password);

            return StatusCode(201);
        }
    }
}