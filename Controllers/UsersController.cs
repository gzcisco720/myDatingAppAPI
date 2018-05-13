using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myDotnetApp.API.Data;
using myDotnetApp.API.Dtos;

namespace myDotnetApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDtos>>(users);
            return Ok(usersToReturn);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailedDtos>(user);
            return Ok(userToReturn);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserForUpdateDtos userForUpdateDtos)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _repo.GetUser(id);

            if(userFromRepo == null) 
            {
                return NotFound("Could not fount the User");
            }
            if(currentUserId != userFromRepo.Id)
            {
                return Unauthorized();
            }
            _mapper.Map(userForUpdateDtos,userFromRepo);
            if(await _repo.SaveAll())
            {
                return NoContent();
            }
            throw new Exception("User Update Failed");
        }
    }
}