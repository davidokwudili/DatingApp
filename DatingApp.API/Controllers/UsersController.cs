using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Datas.IDatas;
using DatingApp.API.DTOs;
using DatingApp.API.helpers;
using DatingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))] // anytime any method get called, we'll make use of our LogUserActivityFilter to update last active
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
        public async Task<IActionResult> GetUsers([FromQuery]ParamsUser param)
        {
            //get the logged in user ID
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //get single user
            var userFromRepo = await _repo.GetUser(currentUserId, true);

            //set the param User Id
            param.UserId = currentUserId;

            //check if gender was set
            if (string.IsNullOrEmpty(param.Gender))
            {
                //check if the logged in user is a male and display for him female, vise visa
                param.Gender = userFromRepo.Gender == "male" ? "female" : "male";
            }

            //get the users based on the parameters
            var usersPg = await _repo.GetUsers(param);
            //retured the Dto'ed class list
            var mappedUser = _mapper.Map<IEnumerable<UserForListDto>>(usersPg);


            //add to the response header, the pagination details, using our Response extension method
            Response.AddPagination(usersPg.CurrentPage, usersPg.PageSize,
                usersPg.TotalCount, usersPg.TotalPages);

            return Ok(mappedUser);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            bool isCurrentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) == id;

            var user = await _repo.GetUser(id, isCurrentUser);

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
            var mainUser = await _repo.GetUser(id, true);
            //map the data from the Dto source to main user class
            _mapper.Map(userForUpdateDto, mainUser);

            //save changes and check if saved successfully
            if (await _repo.SaveAll())
                return NoContent();

            throw new Exception($"Updating user with {id} failed on save.");
        }


        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            //check if the user is authorise
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            //get the like
            var like = await _repo.GetLike(id, recipientId);

            //check if user has already been liked
            if (like != null)
                return BadRequest("You already like this user");

            //check if the recipient(user) exist
            if (await _repo.GetUser(recipientId, false) == null)
                return NotFound();

            //set new intance
            like = new Like
            {
                LikerId = id,
                LikeeId = recipientId
            };

            //add to memory
            _repo.Add<Like>(like);

            //save to db
            if (await _repo.SaveAll())
                return Ok();

            return BadRequest("Failed to like user");
        }

    }
}