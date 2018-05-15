using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using myDotnetApp.API.Data;
using myDotnetApp.API.Dtos;
using myDotnetApp.API.Helpers;
using myDotnetApp.API.Model;

namespace myDotnetApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    public class PhotosController : Controller
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(
        IDatingRepository repo,
        IMapper mapper,
        IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _repo = repo;
            _mapper = mapper;
            Account account = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }
        [HttpGet("{id}", Name="GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);
            
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return Ok(photo);
        }
        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, PhotoForCreationDto photoForCreationDto)
        {
            var user = await _repo.GetUser(userId);
            if (user == null) 
            {
                return BadRequest("Could not find user");
            }
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if(currentUserId != user.Id)
            {
                return Unauthorized();
            }
            var file = photoForCreationDto.File;
            
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using(var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);
            photo.User = user;

            if (!user.Photos.Any(m => m.IsMain))
            {
                photo.IsMain = true;
            }
            user.Photos.Add(photo);
            
            // var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);

            if(await _repo.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto",new {id = photo.Id}, photoToReturn);
            }

            return BadRequest("Unknown Error");
        }
        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoth(int userId, int id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if(currentUserId != userId)
            {
                return Unauthorized();
            }
            var photoFromRepo = await _repo.GetPhoto(id);
            if (photoFromRepo == null)
            {
                return BadRequest("Photo is not found");
            }
            if (photoFromRepo.IsMain)
            {
                return BadRequest("Photo is already the main photo");
            }
            var currentMainPhoto = await _repo.GetMainPhoto(userId);
            if(currentMainPhoto != null)
            {
                currentMainPhoto.IsMain = false;
            }
            photoFromRepo.IsMain = true;
            if(await _repo.SaveAll())
            {
                return NoContent();
            }
            return BadRequest("Internal Server Error");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId,int id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if(currentUserId != userId)
            {
                return Unauthorized();
            }
            var photoFromRepo = await _repo.GetPhoto(id);
            if (photoFromRepo == null)
            {
                return BadRequest("Photo is not found");
            }
            if (photoFromRepo.IsMain)
            {
                return BadRequest("You cannot delete the Main Photo");
            }
            if(photoFromRepo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);

                var result = _cloudinary.Destroy(deleteParams);

                if (result.Result == "ok")
                {
                    _repo.Delete(photoFromRepo);
                }
            }
            else
            {
                _repo.Delete(photoFromRepo);
            }
            if (await _repo.SaveAll())
            {
                return Ok();
            }
            return BadRequest("Internal Server Error");
        }
    }
}