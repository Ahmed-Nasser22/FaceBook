using api.DTOs;
using api.Entities;
using api.Helpers;
using api.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;

namespace api.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _Mapper;

        public UserRepository(DataContext context, IMapper Mapper)
        {
            _context = context;
            _Mapper = Mapper;
        }

        public async Task<MemberDto> GetMemberAsync(string UserName)
        {
            return await _context.Users
                .Where(s => s.UserName == UserName)
                .ProjectTo<MemberDto>(_Mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }



        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();
            query = query.Where(u=>u.UserName != userParams.CurrentUsername);
            query = query.Where (u=>u.Gender == userParams.Gender);

            var MinDob =    DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge-1));
            var MaxDob =    DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            query = query.Where(u => u.DateOfBirth >= MinDob && u.DateOfBirth <= MaxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending  (u=>u.Created),
                _ => query.OrderByDescending(u=>u.LastActive),
            };
            return await PagedList<MemberDto>.CreateAsync(query.AsNoTracking().ProjectTo<MemberDto>(_Mapper.ConfigurationProvider)
                  , userParams.PageNumber , userParams.PageSize);
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string UserName)
        {
            return await _context.Users.Include(x => x.Photos)
                .SingleOrDefaultAsync(s => s.UserName == UserName);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users.Include(p => p.Photos).ToListAsync();
        }


        public void Update(AppUser User)
        {
            _context.Entry(User).State = EntityState.Modified;
        }

        public async Task<string> GetUserGender(string username)
        {
            return await _context.Users.Where(x=>x.UserName == username).Select(g=>g.Gender).FirstOrDefaultAsync();
        }

    }
}
