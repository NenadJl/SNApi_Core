using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SN_App.Api.Dtos;
using SN_App.Api.Helpers;
using SN_App.Repo.Data.Repositories.Users;
using SN_App.Repo.Helpers;
using SN_App.Repo.Models;

namespace SN_App.Api.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IDatingRepository _datingRepo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository datingRepo, IMapper mapper)
        {
            _mapper = mapper;
            _datingRepo = datingRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var userMakingRequest = await _datingRepo.GetUser(userId);

            userParams.UserId = userMakingRequest.Id;

            if (userParams.Gender == null)
                userParams.Gender = userMakingRequest.Gender;

            var users = await _datingRepo.GetUsers(userParams);

            var usersForResponse = _mapper.Map<IEnumerable<UserForListDto>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(usersForResponse);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _datingRepo.GetUser(id);

            var userForResponse = _mapper.Map<UserForDetailDto>(user);

            return Ok(userForResponse);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserForUpdateDto userForUpdateDto)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _datingRepo.GetUser(id);

            _mapper.Map(userForUpdateDto, userFromRepo);

            if (await _datingRepo.SaveAll())
                return NoContent();

            throw new Exception($"Updating user {id} failed on save");
        }

        [HttpPost("{userId}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int userId, int recipientId)
        {
             if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var like = await _datingRepo.GetLike(userId, recipientId);

            if(like != null)
                return BadRequest("You already like this user");

            if(await _datingRepo.GetUser(recipientId) == null)
                return NotFound();

            like = new Like
            {
                LikerId = userId,
                LikeeId = recipientId
            };

            _datingRepo.Add<Like>(like);

            if (await _datingRepo.SaveAll())
                return Ok();

            return BadRequest("ERRRRROOOOR");
        }
    }
}