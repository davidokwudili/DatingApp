using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Datas.IDatas;
using DatingApp.API.DTOs;
using DatingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();
            var mappedUser = _mapper.Map<IEnumerable<UserForListDto>>(users);

            return Ok(mappedUser);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            var mappedUser = _mapper.Map<UserForDetailedDto>(user);

            return Ok(mappedUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            //check if user is authorized
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            // get the user
            var mainUser = await _repo.GetUser(id);
            //map the data from the Dto source to main user class
            _mapper.Map(userForUpdateDto, mainUser);

            //save changes and check if saved successfully
            if (await _repo.SaveAll())
                return NoContent();

            throw new Exception($"Updating user with {id} failed on save.");
        }
    }
}