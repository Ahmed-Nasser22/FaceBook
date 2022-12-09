using api.DTOs;
using api.Entities;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize]

    public class UsersController : BaseApiController
    {
        private readonly IPhotoService _PhotoService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _Mapper;

        public UsersController( IMapper Mapper, IPhotoService PhotoService , IUnitOfWork unitOfWork)
        {
            _PhotoService = PhotoService;
            _unitOfWork = unitOfWork;
            _Mapper = Mapper;
        }


        [HttpGet]
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            var gender = await _unitOfWork.UserRepository.GetUserGender(User.GetUsserName());
            userParams.CurrentUsername = User.GetUsserName();

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = gender == "male" ? "female" : "male";
            }

            var users = await _unitOfWork.UserRepository.GetMembersAsync(userParams);

             Response.AddPaginationHeader( new PaginationHeader( users.CurrentPage , users.PageSize , users.TotalCount , users.TotalPages));


            return Ok(users);
        }

        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string UserName)
        {

            return await _unitOfWork.UserRepository.GetMemberAsync(UserName);

        }


        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsserName());
            _Mapper.Map(memberUpdateDto, user);
            _unitOfWork.UserRepository.Update(user);

            if (await _unitOfWork.Complete()) return NoContent();
            return BadRequest("update filure");
        }


        [HttpPost("add-photo")]

        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsserName());
            var result = await _PhotoService.AddPhotoAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
            };

            if (user.Photos.Count == 0) photo.IsMain = true;
            user.Photos.Add(photo);

            if (await _unitOfWork.Complete())
                return CreatedAtRoute("GetUser", new { username = user.UserName }, _Mapper.Map<PhotoDto>(photo));

            return BadRequest("Add Photo Failure");

        }

        [HttpPut("set-main-photo/{photoId}")]

        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsserName());
            
            var photo = user.Photos.FirstOrDefault(ph => ph.Id == photoId);
           
            if (photo.IsMain) return BadRequest("Already Main Photo");
           
            var currentMain = user.Photos.FirstOrDefault(ph => ph.IsMain);
            
            if (currentMain != null) currentMain.IsMain = false;
            
            photo.IsMain = true;
           
            if (await _unitOfWork.Complete()) return NoContent();
            
            return BadRequest("failed to set main");

        }


        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsserName());
            var Photo = user.Photos.FirstOrDefault(ph => ph.Id == photoId);
            if (Photo == null) return NotFound();
            if (Photo.IsMain) return BadRequest("cannot delete Main");
            if(Photo.PublicId != null)
            {
                var result = await _PhotoService.DeletePhotoAsync(Photo.PublicId);
                if (result.Error != null)  return BadRequest(result.Error.Message);

            }
            user.Photos.Remove(Photo);
            if (await _unitOfWork.Complete()) return Ok();
            return BadRequest("delete photo failed");
        }
    }
}