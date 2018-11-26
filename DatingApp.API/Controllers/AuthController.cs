using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.Datas;
using DatingApp.DTOs;
using DatingApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        //private readonly UserManager<User> _userManager;
        //private readonly SignInManager<User> _signInManager;

        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            _mapper = mapper;
            //_userManager = userManager;
            //_signInManager = signInManager;
            _repo = repo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(DtoUserForRegister userForRegister)
        {
            userForRegister.Username = userForRegister.Username.ToLower();

            if (await _repo.UserExist(userForRegister.Username))
                return BadRequest("Username already exist!");

            var userToCreate = _mapper.Map<User>(userForRegister);

            var createdUser = await _repo.Register(userToCreate, userForRegister.Password);

            var returningUser = _mapper.Map<UserForDetailedDto>(createdUser);

            //when you insert a new data, it's good to create a CreatedToute.
            return CreatedAtRoute("GetUser",
                new { controller = "Users", id = createdUser.Id }, returningUser);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(DtoUserForLogin userForLogin)
        {
            var loginUser = await _repo.Login(userForLogin.Username.ToLower(), userForLogin.Password);

            if (loginUser == null)
                return Unauthorized();

            var userToReturn = _mapper.Map<UserForListDto>(loginUser);

            return Ok(new
            {
                token = GenerateJwtToken(loginUser),
                user = userToReturn
            });


            //var user = await _userManager.FindByNameAsync(userForLogin.Username);

            //var result = await _signInManager
            //    .CheckPasswordSignInAsync(user, userForLogin.Password, false);

            //if (result.Succeeded)
            //{
            //    //var appUser = await _userManager.Users.Include(p => p.Photos)
            //    //    .FirstOrDefaultAsync(u => u.NormalizedUserName == userForLoginDto.Username.ToUpper());

            //    //var userToReturn = _mapper.Map<UserForListDto>(appUser);

            //    //return Ok(new
            //    //{
            //    //    token = GenerateJwtToken(appUser).Result,
            //    //    user = userToReturn
            //    //});
            //} 
        }



        private string GenerateJwtToken(User user)
        {
            var claims = new[]
           {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = cred
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }
}