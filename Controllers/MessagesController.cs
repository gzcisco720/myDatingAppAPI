using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myDotnetApp.API.Data;
using myDotnetApp.API.Dtos;
using myDotnetApp.API.Helpers;
using myDotnetApp.API.Model;

namespace myDotnetApp.API.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/users/{userId}/[controller]")]
    public class MessagesController:Controller
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _map;

        public MessagesController(IDatingRepository repo, IMapper map)
        {
            _repo = repo;
            _map = map;
        }
        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var messageFromRepo = await _repo.GetMessage(id);
            if(messageFromRepo==null)
            {
                return NotFound();
            }
            return Ok(messageFromRepo);
        }
        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, [FromBody] MessageForCreationDto messageForCreateionDto)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            messageForCreateionDto.SenderId = userId;
            var recipient = await _repo.GetUser(messageForCreateionDto.RecipientId);
            if(recipient == null)
            {
                return BadRequest("Could not find the user");
            }
            var message = _map.Map<Message>(messageForCreateionDto);
            _repo.Add(message);
            var messageToReturn = _map.Map<MessageForCreationDto>(message);
            if(await _repo.SaveAll())
            {
                return CreatedAtRoute("GetMessage", new { id = message.Id }, message);
            }
            throw new Exception("Internal Server Error");
        }
    }
}