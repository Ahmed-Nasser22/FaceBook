using api.Data;
using api.DTOs;
using api.Entities;
using api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace api.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly IUserToken _TokenService;

        public AccountController(DataContext context, IUserToken TokenService)
        {
            _context = context;
            _TokenService = TokenService;
        }


        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            if (await UserExists(registerDTO.UserName)) return BadRequest("User Already Exists");
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = registerDTO.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
                PasswordSalt = hmac.Key
            };
            _context.Add(user);
            await _context.SaveChangesAsync();
            return new UserDTO
            {
                UserName = user.UserName,
                Token = _TokenService.CreateUserToken(user)
            };

        }

        private async Task<bool> UserExists(string UserName)
        {
            return await _context.Users.AnyAsync(user => user.UserName == UserName.ToLower());

        }


        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO LoginDTO)
        {

            var user = await _context.Users.SingleOrDefaultAsync(user => user.UserName == LoginDTO.UserName);

            if (user == null) return Unauthorized("User Does Not Exsit ");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var ComputedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(LoginDTO.Password));

            for (int i = 0; i < ComputedHash.Length; i++)
            {
                if (ComputedHash[i] != user.PasswordHash[i]) return Unauthorized("password invalid");
            }
            return new UserDTO
            {
                UserName = user.UserName,
                Token = _TokenService.CreateUserToken(user)
            };
        }




    }
}