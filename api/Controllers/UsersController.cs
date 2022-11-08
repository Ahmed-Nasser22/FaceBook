using api.Data;
using api.DTOs;
using api.Entities;
using api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Authorize]

    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _UserRepository;
        private readonly IMapper _Mapper;

        public UsersController(IUserRepository  UserRepository , IMapper Mapper)
        {
            _UserRepository = UserRepository;
            _Mapper = Mapper;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _UserRepository.GetMembersAsync();

            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string UserName)
        {

            return await _UserRepository.GetMemberAsync(UserName);
             
        }

    }
}