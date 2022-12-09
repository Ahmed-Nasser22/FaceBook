using api.DTOs;
using api.Entities;
using api.Helpers;
using api.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class LikesRepository : ILikesRepository 
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public LikesRepository(DataContext context , IMapper Mapper)
        {
            _context = context;
            _mapper = Mapper;
        }
        public async Task<UserLikes> GetUserLike(int sourceUserId, int targetUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, targetUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(u=>u.UserName).AsQueryable();    
            var likes = _context.Likes.AsQueryable();
            if (likesParams.Predicate == "liked")
            {
                likes = likes.Where(likes => likes.SourceUserId == likesParams.UserId);
                users = likes.Select(like => like.TargetUser); 
            }
            if (likesParams.Predicate == "likedBy")
            {
                likes = likes.Where(likes => likes.TargetUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }
            var likedUsers = users.ProjectTo<LikeDto>(_mapper.ConfigurationProvider);

            return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber , likesParams.PageSize);
        }

        public async  Task<AppUser> GetUserWithLikes(int userId)
        {
             return await _context.Users.Include(x=>x.LikedUsers).FirstOrDefaultAsync(x=>x.Id==userId);
        }
    }
}
