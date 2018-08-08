using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SN_App.Api.Dtos;
using SN_App.Api.Helpers;
using SN_App.Repo.Data.Repositories.Users;
using SN_App.Repo.Models;

namespace SN_App.Api.Controllers
{
    [Authorize]
    [Route("api/users/{userid}/photos/")]
    [ApiController]
    public class PhotosController : Controller
    {
        private readonly IDatingRepository _datingRepo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudineryConfig;
        private readonly Cloudinary _cloudinary;

        public PhotosController(
            IDatingRepository datingRepo,
            IMapper mapper,
            IOptions<CloudinarySettings> cloudineryConfig)
        {
            _datingRepo = datingRepo;
            _mapper = mapper;
            _cloudineryConfig = cloudineryConfig;

            Account acc = new Account(
                _cloudineryConfig.Value.CloudName,
                _cloudineryConfig.Value.ApiKey,
                _cloudineryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _datingRepo.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm] PhotoForCreationDto photoForCreationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _datingRepo.GetUser(userId);

            var file = photoForCreationDto.File;

            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation()
                            .Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);

            if (!userFromRepo.Photos.Any(p => p.IsMain))
                photo.IsMain = true;

            userFromRepo.Photos.Add(photo);

            if (await _datingRepo.SaveAll())
            {
                var photoForReturnDto = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photoForReturnDto.Id }, photoForReturnDto);
            }

            return BadRequest("Could not add the photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {

            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await _datingRepo.GetUser(userId);

            if (!user.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromRepo = await _datingRepo.GetPhoto(id);

            if (photoFromRepo.IsMain)
                return BadRequest("This is already a main photo");

            var currentMainPhoto = await _datingRepo.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;

            if (await _datingRepo.SaveAll())
                return NoContent();

            return BadRequest("Could not set main photo.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id) 
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
        
            var user = await _datingRepo.GetUser(userId);

            if (!user.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoToBeDeleted = await _datingRepo.GetPhoto(id);

            if (photoToBeDeleted.IsMain)
                return BadRequest("This is already a main photo");

            if (photoToBeDeleted.PublicId != null)
            {
                var deleitionParams = new DeletionParams(photoToBeDeleted.PublicId);

                var cloudinaryResponse = _cloudinary.Destroy(deleitionParams);

                if(cloudinaryResponse.Result == "ok")
                    _datingRepo.Delete(photoToBeDeleted);
            }
            else
            {
                _datingRepo.Delete(photoToBeDeleted);
            }



            if(await _datingRepo.SaveAll())
                return Ok();

                return BadRequest("Smth went wrong!");
        }


    }
}