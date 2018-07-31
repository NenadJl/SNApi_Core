using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SN_App.Api.Dtos;
using SN_App.Repo.Data.Repositories.Users;
namespace SN_App.Api.Controllers
{
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
        public async Task<IActionResult> GetUsers()
        {
            var users = await _datingRepo.GetUsers();

            var usersForResponse = _mapper.Map<IEnumerable<UserForListDto>>(users);

            return Ok(usersForResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _datingRepo.GetUser(id);

            var userForResponse = _mapper.Map<UserForDetailDto>(user);

            return Ok(userForResponse);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserForUpdateDto userForUpdateDto)
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var userFromRepo = await _datingRepo.GetUser(id);

            _mapper.Map(userForUpdateDto, userFromRepo);

            if(await _datingRepo.SaveAll())
                return NoContent();
            
            throw new Exception($"Updating user {id} failed on save");
        }
    }
}