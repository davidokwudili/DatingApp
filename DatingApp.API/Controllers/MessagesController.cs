

using AutoMapper;
using DatingApp.Api.DTOs;
using DatingApp.Api.helpers;
using DatingApp.API.Datas.IDatas;
using DatingApp.API.helpers;
using DatingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingApp.API.Controllers
{

    [ServiceFilter(typeof(LogUserActivity))] // anytime any method get called, we'll make use of our LogUserActivityFilter to update last active
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;

        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }


        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            //check if user is authorized
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            //get a single message by it ID
            var messageFromRepo = await _repo.GetMessage(id);

            //check if it's doesnt exist and return notfound
            if (messageFromRepo == null)
                return NotFound();

            //return okay with it object data
            return Ok(messageFromRepo);
        }


        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery]ParamsMessage messageParams)
        {
            //check if logged in user is authorized
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            //set the logedin userId as USerID Params before it's been passed
            messageParams.UserId = userId;

            //get all messages from repo based on the params
            var messagesFromRepo = await _repo.GetMessagesForUser(messageParams);

            //map the data from database to the object data we would be returning
            var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            //add the PAGINATION detail to our response header helper method
            Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize,
                messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);

            //return Okay, with the mapped messages data
            return Ok(messages);
        }


        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            //check if logged in user is authorized
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            //get messages i sent and messages a user has sent to me
            var messagesFromRepo = await _repo.GetMessageThread(userId, recipientId);

            //map to the returning dto
            var messageThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            //return object
            return Ok(messageThread);
        }


        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {
            //get the user that's sending the message
            var sender = await _repo.GetUser(userId, false);

            //check if logged in user is authorized
            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            //set the user ID
            messageForCreationDto.SenderId = userId;

            //get the user that the message is been sent to
            var recipient = await _repo.GetUser(messageForCreationDto.RecipientId, false);

            //confirm the user exist
            if (recipient == null)
                return BadRequest("Could not find user");

            //map the collected data frm the client to the MESSAGE object
            var message = _mapper.Map<Message>(messageForCreationDto);

            //add to memory
            _repo.Add(message);

            //save all changes to database
            if (await _repo.SaveAll())
            {
                //remap the returning message from the saved message
                var messageToReturn = _mapper.Map<MessageToReturnDto>(message);
                //create a repose with the saved message details
                return CreatedAtRoute("GetMessage", new { id = message.Id }, messageToReturn);
            }

            throw new Exception("Creating the message failed on save");
        }



        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId)
        {
            //get the user that's sending the message
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            //get the message from repo, based on the message id
            var messageFromRepo = await _repo.GetMessage(id);

            //check if the sender was the current user, then set sender deleted to true
            if (messageFromRepo.SenderId == userId)
                messageFromRepo.SenderDeleted = true;

            //check if the recipient was the current user, then set recipient deleted to true
            if (messageFromRepo.RecipientId == userId)
                messageFromRepo.RecipientDeleted = true;

            //check if both user has deleted their messages from their ends, then totally delete from database
            if (messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted)
                _repo.Delete(messageFromRepo);

            //save changes
            if (await _repo.SaveAll())
                return NoContent();

            throw new Exception("Error deleting the message");
        }


        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int userId, int id)
        {
            //get the user that's sending the message
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            //get the message based on id
            var message = await _repo.GetMessage(id);

            //only mark as read when the recipeint which it was sent to has read it.
            if (message.RecipientId != userId)
                return Unauthorized();

            //set read values
            message.IsRead = true;
            message.DateRead = DateTime.Now;

            //update
            await _repo.SaveAll();

            //we returning no content content
            return NoContent();
        }

    }
}
